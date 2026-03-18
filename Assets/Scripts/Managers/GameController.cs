using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameController>();
            }
            return instance;
        }
    }

    // 组件引用
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

    [Header("功能开关")]
    public bool enableSprint = true;
    public bool enableCameraShake = true;

    // 内部状态
    private float yaw;
    private float pitch;
    private bool cursorLocked = true;
    private Vector3 currentVelocity;
    private Vector3 cameraLocalPos;
    private float shakeTimer;

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

                // 确保摄像机在本物体下
                if (cameraTransform.parent != transform)
                {
                    cameraTransform.SetParent(transform);
                    cameraTransform.localPosition = new Vector3(0, 1.6f, 0);
                    cameraLocalPos = cameraTransform.localPosition;
                }
            }
        }
        else
        {
            cameraTransform = playerCamera.transform;
            cameraLocalPos = cameraTransform.localPosition;
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
    }

    void Update()
    {
        // ESC键切换鼠标锁定 - 使用GetKeyDown确保单次触发
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }

        // 处理旋转（只在鼠标锁定时）
        if (cursorLocked)
        {
            HandleRotation();
        }

        // 处理移动（无论鼠标是否锁定都可以移动）
        HandleMovement();

        // 处理摄像机抖动
        if (enableCameraShake && cameraTransform != null)
        {
            HandleCameraShake();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }

    #region 核心功能

    void ToggleCursorLock()
    {
        cursorLocked = !cursorLocked;
        LockCursor(cursorLocked);
    }

    void HandleRotation()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertMouseY ? 1 : -1);

        // 阈值过滤
        if (Mathf.Abs(mouseX) < inputThreshold) mouseX = 0;
        if (Mathf.Abs(mouseY) < inputThreshold) mouseY = 0;

        // 更新旋转
        if (mouseX != 0 || mouseY != 0)
        {
            yaw += mouseX;
            pitch = Mathf.Clamp(pitch + mouseY, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }
    }

    void HandleMovement()
    {
        // 获取输入 - 使用GetAxis确保平滑输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 检查是否有移动输入
        bool isMoving = Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f;

        // 确定速度
        float speed = walkSpeed;
        if (enableSprint && Input.GetKey(KeyCode.LeftShift) && isMoving)
        {
            speed = sprintSpeed;
        }

        // 计算移动方向
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        // 计算移动向量
        Vector3 move = forward * vertical + right * horizontal;

        // 归一化防止斜向移动过快
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        // 应用速度
        move *= speed;

        // 记录当前速度用于抖动计算
        currentVelocity = move;

        // 执行移动
        if (controller != null)
        {
            // 使用SimpleMove会自动应用重力
            controller.SimpleMove(move);
        }
        else
        {
            // 备用：直接移动
            transform.position += move * Time.deltaTime;
        }
    }

    void HandleCameraShake()
    {
        float speed = currentVelocity.magnitude;

        // 速度低于阈值，平滑回到原位
        if (speed < minShakeSpeed)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraLocalPos, Time.deltaTime * 10f);
            shakeTimer = 0;
            return;
        }

        // 计算抖动幅度
        float shakeMagnitude = speed * shakeIntensity;

        // 冲刺时增加抖动
        if (enableSprint && Input.GetKey(KeyCode.LeftShift))
        {
            shakeMagnitude *= sprintShakeMultiplier;
        }

        // 生成抖动
        shakeTimer += Time.deltaTime * shakeFrequency * (speed / walkSpeed);

        float offsetX = Mathf.Sin(shakeTimer) * shakeMagnitude;
        float offsetY = Mathf.Cos(shakeTimer * 1.3f) * shakeMagnitude * 0.5f;
        float offsetZ = Mathf.Sin(shakeTimer * 0.7f) * shakeMagnitude * 0.3f;

        Vector3 shakeOffset = new Vector3(offsetX, offsetY, offsetZ);
        cameraTransform.localPosition = cameraLocalPos + shakeOffset;
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
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = cameraLocalPos;
        }
    }

    public void SetSpeed(float walk, float? sprint = null)
    {
        walkSpeed = walk;
        if (sprint.HasValue) sprintSpeed = sprint.Value;
    }

    public void SetCameraShake(bool enable)
    {
        enableCameraShake = enable;
        if (!enable && cameraTransform != null)
        {
            cameraTransform.localPosition = cameraLocalPos;
        }
    }

    #endregion
}