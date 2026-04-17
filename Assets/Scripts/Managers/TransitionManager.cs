using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Loading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager instance;
    public static TransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<TransitionManager>();
                if (instance == null)
                {
                    Debug.Log("No TransitionManager found in the scene!");
                }
            }
            return instance;
        }
    }
    [Header("动画参数")]
    [SerializeField] private float slideInDuration = 0.5f;   // 滑入时间
    [SerializeField] private float slideOutDuration = 0.5f;  // 滑出时间
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float offScreenOffset = 2000f;

    private void Start()
    {

    }

    public void Loading(GameObject loading, float time)
    {
        StartCoroutine(Loadings(loading, time,null));
    }

    public void Loading(GameObject loading, float time,UnityEvent fun)
    {
        StartCoroutine(Loadings(loading, time,fun));
    }

    IEnumerator Loadings(GameObject loading, float time,UnityEvent fun)
    {
        TextMeshProUGUI loadingText;
        Animator[] animations;
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
            loadingText = loadingObj.GetComponentInChildren<TextMeshProUGUI>();
            animations = GetComponentsInChildren<Animator>();

        // 激活Loading元素
        if (loadingText != null)
        {
            loadingText.alpha = 0;
            loadingText.gameObject.SetActive(true);
        }
        //if (loadingSpinner != null)
       // {
        //    loadingSpinner.gameObject.SetActive(true);
        //    loadingSpinner.rotation = Quaternion.identity;
        //}

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

        yield return new WaitForSeconds(0.2f);
        fun?.Invoke();
        foreach(var anim in animations)
        {
            anim.SetTrigger("StartAni");
        }
        // ========== 阶段三：等待时间 ==========
        elapsed = 0;
        while (elapsed < time-0.2f)
        {
            elapsed += Time.deltaTime;
            //if (loadingSpinner != null)
            //    loadingSpinner.Rotate(0, 0, -360 * Time.deltaTime);
            if (loadingText != null)
                loadingText.alpha = 0.8f + Mathf.PingPong(elapsed * 2f, 0.2f);
            yield return null;
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
}
