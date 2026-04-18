using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableManager : MonoBehaviour
{
    private static InteractableManager instance;
    public static InteractableManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<InteractableManager>();
                if (instance == null)
                {
                    Debug.Log("No InteractableManager found in the scene!");
                }
            }
            return instance;
        }
    }

    public enum InteractableType
    {
        Talent,
        Drag,
        Shop,
        Module,
        Quest,
        NPC
    }

    public PlayerMoveManager movemanager;
    public GameObject talent;
    public GameObject drag;
    public GameObject shop;
    public GameObject module;
    public GameObject purchase;

    [Header("Loading设置")]
    [SerializeField] private TextMeshProUGUI loadingText;   // Loading文字
    [SerializeField] private RectTransform loadingSpinner;  // 旋转图标

    [Header("动画参数")]
    [SerializeField] private float slideInDuration = 0.5f;   // 滑入时间
    [SerializeField] private float slideOutDuration = 0.5f;  // 滑出时间
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float offScreenOffset = 2000f;

    private void Start()
    {
        talent.SetActive(false);
        drag.SetActive(false);
        //shop.SetActive(false);
        module.SetActive(false);
        purchase.SetActive(false);
    }

    public void Interactable(InteractableType type)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        movemanager.enabled = false;
        switch (type)
        {
            case InteractableType.Talent:
                talent.SetActive(true);
                break;
            case InteractableType.Drag:
                drag.SetActive(true);
                break;
            case InteractableType.Shop:
                shop.SetActive(true);
                break;
            case InteractableType.Module:
                module.SetActive(true);
                break;
            case InteractableType.Quest:

                break;
            case InteractableType.NPC:
                purchase.SetActive(true);
                break;
        }
    }

    public void Interactable(InteractableType type, GameObject loading)
    {
        UnityEvent fun = new UnityEvent();
        fun.AddListener(() => Interactable(type));
        TransitionManager.Instance.Loading(loading, 1.2f,fun);
        //StartCoroutine(Loading(type,loading,1.2f));
        movemanager.enabled = false;
    }

    IEnumerator Loading(InteractableType type,GameObject loading, float time)
    {
        GameObject loadingObj = Instantiate(loading, transform);
        loadingObj.SetActive(true);

        RectTransform panel = loadingObj.GetComponent<RectTransform>();

        // 确保是UI层级
        panel.SetParent(transform, false);

        Vector2 hiddenRight = new Vector2(offScreenOffset, 0);
        Vector2 center = Vector2.zero;
        Vector2 hiddenLeft = new Vector2(-offScreenOffset, 0);

        // 初始位置在右侧外
        panel.anchoredPosition = hiddenRight;
        // 获取LoadingText
        if (loadingText == null)
            loadingText = loadingObj.GetComponentInChildren<TextMeshProUGUI>();

        // 激活Loading元素
        if (loadingText != null)
        {
            loadingText.alpha = 0;
            loadingText.gameObject.SetActive(true);
        }
        if (loadingSpinner != null)
        {
            loadingSpinner.gameObject.SetActive(true);
            loadingSpinner.rotation = Quaternion.identity;
        }

        // ========== 阶段一：从右侧滑入 ==========
        float elapsed = 0;
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideInDuration);
            panel.anchoredPosition = Vector2.Lerp(hiddenRight, center, t);
            yield return null;
        }
        panel.anchoredPosition = center;

        // ========== 阶段二：Loading淡入 ==========
        if (loadingText != null)
        {
            elapsed = 0;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                loadingText.alpha = Mathf.Lerp(0, 1, elapsed / 0.2f);
                yield return null;
            }
            loadingText.alpha = 1;
        }

        // ========== 阶段三：等待时间 ==========
        elapsed = 0;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            if (loadingSpinner != null)
                loadingSpinner.Rotate(0, 0, -360 * Time.deltaTime);
            if (loadingText != null)
                loadingText.alpha = 0.8f + Mathf.PingPong(elapsed * 2f, 0.2f);
            yield return null;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        switch (type)
        {
            case InteractableType.Talent:
                talent.SetActive(true);
                break;
            case InteractableType.Drag:
                drag.SetActive(true);
                break;
            case InteractableType.Shop:
                shop.SetActive(true);
                break;
            case InteractableType.Module:
                module.SetActive(true);
                break;
            case InteractableType.Quest:

                break;
            case InteractableType.NPC:
                purchase.SetActive(true);
                break;
        }
        // ========== 阶段四：Loading淡出 ==========
        if (loadingText != null)
        {
            elapsed = 0;
            while (elapsed < 0.15f)
            {
                elapsed += Time.deltaTime;
                loadingText.alpha = Mathf.Lerp(1, 0, elapsed / 0.15f);
                yield return null;
            }
        }

        // ========== 阶段五：向左侧滑出 ==========
        elapsed = 0;
        while (elapsed < slideOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideOutDuration);
            panel.anchoredPosition = Vector2.Lerp(center, hiddenLeft, t);
            yield return null;
        }

        // ========== 清理：销毁预制体 ==========
        Destroy(loadingObj);

    }

    public void CloseInteractable(InteractableType type)
    {
        movemanager.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        switch (type)
        {
            case InteractableType.Talent:
                talent.SetActive(false);
                break;
            case InteractableType.Drag:
                drag.SetActive(false);
                break;
            case InteractableType.Shop:
                shop.SetActive(false);
                break;
            case InteractableType.Module:
                module.SetActive(false);
                break;
            case InteractableType.Quest:

                break;
            case InteractableType.NPC:
                purchase.SetActive(false);
                break;
        }
    }
}
