using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMoveManager : MonoBehaviour
{
    private static PlayerMoveManager instance;
    public static PlayerMoveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerMoveManager>();
            }
            return instance;
        }
    }

    public CharacterController controller;
    public Camera playerCamera;
    private Transform cameraTransform;

    [Header("移动设置")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;

    [Header("旋转设置")]
    public float mouseSensitivity = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;
    public bool invertMouseY = false;
    [Tooltip("忽略小于此值的鼠标输入，防止抖动")]
    public float inputThreshold = 0.001f;

    [Header("摄像机抖动")]
    [Tooltip("抖动强度系数，越大抖动越明显")]
    public float shakeIntensity = 0.05f;
    [Tooltip("抖动频率")]
    public float shakeFrequency = 15f;
    [Tooltip("最小移动速度阈值，低于此值不抖动")]
    public float minShakeSpeed = 0.1f;
    [Tooltip("冲刺时的额外抖动倍数")]
    public float sprintShakeMultiplier = 1.5f;

    [Header("摄像机防穿墙")]
    [Tooltip("摄像机与角色之间的最大距离（第三人称用，第一人称设为0）")]
    public float cameraMaxDistance = 0f;
    [Tooltip("摄像机碰撞检测半径")]
    public float cameraRadius = 0.1f;
    [Tooltip("摄像机避障层")]
    public LayerMask cameraCollisionLayers = ~0;
    [Tooltip("第一人称模式下是否启用头部碰撞检测")]
    public bool enableHeadCollision = true;
    [Tooltip("头部碰撞检测距离")]
    public float headCheckDistance = 0.5f;

    [Header("交互设置")]
    [Tooltip("交互检测距离")]
    public float interactDistance = 3f;
    [Tooltip("交互检测角度（视野中心多少度内）")]
    public float interactAngle = 30f;
    [Tooltip("交互提示UI（可选，需自行实现显示逻辑）")]
    public GameObject interactPromptUI;
    [Tooltip("射线检测层")]
    public LayerMask interactLayer = ~0;

    [Header("功能开关")]
    public bool enableSprint = true;
    public bool enableCameraShake = true;
    public bool enableInteract = true;

    [Header("UI引用")]
    public TextMeshProUGUI promptText;      // UI Text组件
    public GameObject promptPanel; // 提示面板

    // 内部状态
    private float yaw;
    private float pitch;
    private bool cursorLocked = true;
    private Vector3 currentVelocity;
    private Vector3 cameraLocalPos;
    private float shakeTimer;
    private float currentCameraDistance; // 当前摄像机距离（用于防穿墙）

    // 交互状态
    private IInteractable currentInteractable;
    private bool isPromptShowing = false;

    void Awake()
    {
        // 确保单例
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 查找摄像机
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera != null)
            {
                cameraTransform = playerCamera.transform;
                cameraLocalPos = cameraTransform.localPosition;
                currentCameraDistance = cameraLocalPos.magnitude;

                // 确保摄像机在本物体下
                if (cameraTransform.parent != transform)
                {
                    cameraTransform.SetParent(transform);
                    cameraTransform.localPosition = new Vector3(0, 1.6f, 0);
                    cameraLocalPos = cameraTransform.localPosition;
                    currentCameraDistance = cameraLocalPos.magnitude;
                }
            }
        }
        else
        {
            cameraTransform = playerCamera.transform;
            cameraLocalPos = cameraTransform.localPosition;
            currentCameraDistance = cameraLocalPos.magnitude;
        }

        // 查找CharacterController
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        // 初始化旋转角度
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    void Start()
    {
        LockCursor(true);

        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        // ESC键切换鼠标锁定
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    ToggleCursorLock();
        //}

        // 处理旋转（只在鼠标锁定时）
        if (cursorLocked)
        {
            HandleRotation();
        }

        // 处理移动
        HandleMovement();

        // 处理摄像机位置（防穿墙）
        HandleCameraPosition();

        // 处理摄像机抖动
        if (enableCameraShake && cameraTransform != null)
        {
            HandleCameraShake();
        }

        // 处理交互
        if (enableInteract)
        {
            HandleInteraction();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // 绘制交互范围
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, interactDistance);

        // 绘制摄像机碰撞检测
        if (cameraTransform != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Vector3 headPos = transform.position + Vector3.up * cameraLocalPos.y;
            Gizmos.DrawWireSphere(headPos, cameraRadius);
            Gizmos.DrawLine(transform.position, headPos);
        }
    }

    #region 核心功能

    void ToggleCursorLock()
    {
        cursorLocked = !cursorLocked;
        LockCursor(cursorLocked);
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertMouseY ? 1 : -1);

        if (Mathf.Abs(mouseX) < inputThreshold) mouseX = 0;
        if (Mathf.Abs(mouseY) < inputThreshold) mouseY = 0;

        if (mouseX != 0 || mouseY != 0)
        {
            yaw += mouseX;
            pitch = Mathf.Clamp(pitch + mouseY, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(0, yaw, 0); // 角色只水平旋转
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f;

        float speed = walkSpeed;
        if (enableSprint && Input.GetKey(KeyCode.LeftShift) && isMoving)
        {
            speed = sprintSpeed;
        }

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = forward * vertical + right * horizontal;

        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        move *= speed;
        currentVelocity = move;

        if (controller != null)
        {
            controller.SimpleMove(move);
        }
        else
        {
            transform.position += move * Time.deltaTime;
        }
    }

    void HandleCameraPosition()
    {
        if (cameraTransform == null) return;

        // 计算理想的摄像机位置（基于角色位置和偏移）
        Vector3 targetCameraPos = transform.position + Vector3.up * cameraLocalPos.y;

        // 第一人称：检测头部前方是否有墙，有则拉近摄像机
        if (enableHeadCollision && cameraMaxDistance <= 0.1f)
        {
            // 从角色头部向前发射射线检测
            Vector3 headPos = transform.position + Vector3.up * cameraLocalPos.y;
            Vector3 lookDir = Quaternion.Euler(pitch, yaw, 0) * Vector3.forward;

            // 检测前方障碍物
            if (Physics.SphereCast(headPos, cameraRadius, lookDir, out RaycastHit hit, headCheckDistance, cameraCollisionLayers))
            {
                // 前方有墙，将摄像机拉到碰撞点后方
                float safeDistance = Mathf.Max(0, hit.distance - 0.1f);
                Vector3 safePos = headPos + lookDir * safeDistance;
                cameraTransform.position = safePos;
                cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0);
                return;
            }
        }
        // 第三人称：从角色向后拉摄像机，检测中间是否有障碍
        else if (cameraMaxDistance > 0.1f)
        {
            Vector3 headPos = transform.position + Vector3.up * cameraLocalPos.y;
            Vector3 backDir = Quaternion.Euler(-pitch, -yaw, 0) * Vector3.forward;

            // 向后发射射线找空位
            float targetDistance = Mathf.Min(cameraMaxDistance, currentCameraDistance);

            if (Physics.SphereCast(headPos, cameraRadius, backDir, out RaycastHit hit, targetDistance, cameraCollisionLayers))
            {
                // 有障碍，摄像机放在碰撞点前方
                currentCameraDistance = Mathf.Max(0.1f, hit.distance - 0.2f);
            }
            else
            {
                // 无障碍，恢复到最大距离
                currentCameraDistance = Mathf.Lerp(currentCameraDistance, cameraMaxDistance, Time.deltaTime * 5f);
            }

            targetCameraPos = headPos + backDir * currentCameraDistance;
        }

        // 应用位置和旋转
        cameraTransform.position = targetCameraPos;
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    void HandleCameraShake()
    {
        float speed = currentVelocity.magnitude;

        if (speed < minShakeSpeed)
        {
            // 不抖动时平滑恢复，但保持防穿墙逻辑
            shakeTimer = 0;
            return;
        }

        float shakeMagnitude = speed * shakeIntensity;

        if (enableSprint && Input.GetKey(KeyCode.LeftShift))
        {
            shakeMagnitude *= sprintShakeMultiplier;
        }

        shakeTimer += Time.deltaTime * shakeFrequency * (speed / walkSpeed);

        float offsetX = Mathf.Sin(shakeTimer) * shakeMagnitude;
        float offsetY = Mathf.Cos(shakeTimer * 1.3f) * shakeMagnitude * 0.5f;
        float offsetZ = Mathf.Sin(shakeTimer * 0.7f) * shakeMagnitude * 0.3f;

        // 抖动作为额外的局部偏移，在HandleCameraPosition后应用
        // 注意：抖动可能导致穿墙，所以限制幅度
        Vector3 shakeOffset = cameraTransform.right * offsetX +
                             cameraTransform.up * offsetY +
                             cameraTransform.forward * offsetZ;

        // 检测抖动后的位置是否安全
        Vector3 desiredPos = cameraTransform.position + shakeOffset;
        if (!Physics.CheckSphere(desiredPos, cameraRadius * 0.5f, cameraCollisionLayers))
        {
            cameraTransform.position = desiredPos;
        }
    }

    void HandleInteraction()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        IInteractable target = null;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            target = hit.collider.GetComponent<IInteractable>();
            if (target == null)
            {
                target = hit.collider.GetComponentInParent<IInteractable>();
            }
        }
        else
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, interactDistance, interactLayer);
            float closestAngle = interactAngle;

            foreach (var col in nearbyColliders)
            {
                var interactable = col.GetComponent<IInteractable>();
                if (interactable == null) continue;

                Vector3 dirToObject = (col.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToObject);

                if (angle < closestAngle)
                {
                    closestAngle = angle;
                    target = interactable;
                }
            }
        }

        if (target != currentInteractable)
        {
            if (currentInteractable != null && isPromptShowing)
            {
                HideInteractPrompt();
            }

            currentInteractable = target;

            if (currentInteractable != null)
            {
                ShowInteractPrompt(currentInteractable.GetInteractPrompt());
            }
        }

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.OnInteract(this);
            ShowInteractPrompt(currentInteractable.GetInteractPrompt());
        }
    }

    void ShowInteractPrompt(string text)
    {
        isPromptShowing = true;

        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
        }

        if (promptText != null)
        {
            promptText.text = $"{text}";
        }
    }

    void HideInteractPrompt()
    {
        isPromptShowing = false;

        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }

    void LockCursor(bool lockState)
    {
        if (lockState)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #endregion

    #region 公共接口

    public void Teleport(Vector3 position, float? newYaw = null, float? newPitch = null)
    {
        if (controller != null)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
        }
        else
        {
            transform.position = position;
        }

        if (newYaw.HasValue) yaw = newYaw.Value;
        if (newPitch.HasValue) pitch = newPitch.Value;
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        // 强制更新摄像机位置
        HandleCameraPosition();

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = cameraLocalPos;
        }
    }
    #endregion
}