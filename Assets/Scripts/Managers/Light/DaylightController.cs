using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // Post-Processing Stack v2

[ExecuteAlways]
public class DaylightController : MonoBehaviour
{
    [Tooltip("拖入Directional Light，或留空自动查找")]
    public Light sunLight;
    public float its;

    [Range(0, 24)]
    public float hour = 12f;

    [Tooltip("太阳水平方向：0=北, 90=东, 180=南, 270=西")]
    [Range(0, 360)]
    public float direction = 180f;

    [Header("时间缩放")]
    public float timeScale = 60f;

    [Header("后处理控制 - Post Processing Stack v2")]
    [Tooltip("场景中的Post Process Volume (Global)")]
    public PostProcessVolume postProcessVolume;

    [Tooltip("白天曝光值")]
    public float dayPostExposure = 0f;

    [Tooltip("夜晚曝光值 - 调低压暗烘焙贴图")]
    public float nightPostExposure = -2f;

    [Header("室内灯光")]
    public Light[] indoorLights;
    public float nightIndoorMult = 2.5f;
    public float dayIndoorMult = 0.2f;

    private float[] originalIntensities;
    private ColorGrading colorGrading;
    private float currentPostExposure;

    void OnEnable()
    {
        FindSun();
        SetupPostProcess();
        CacheIndoorLights();
    }

    void SetupPostProcess()
    {
        if (postProcessVolume == null)
        {
            Debug.LogWarning("请把 Post Process Volume 拖到脚本上！");
            return;
        }

        // 获取 Color Grading 组件（包含 Post Exposure）
        if (postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            Debug.Log("成功绑定 Color Grading");
        }
        else
        {
            Debug.LogWarning("Post Process Volume 缺少 Color Grading！请手动添加");
        }
    }

    void CacheIndoorLights()
    {
        if (indoorLights == null || indoorLights.Length == 0) return;
        originalIntensities = new float[indoorLights.Length];
        for (int i = 0; i < indoorLights.Length; i++)
            if (indoorLights[i] != null)
                originalIntensities[i] = indoorLights[i].intensity;
    }

    void Update()
    {
        hour += (Time.deltaTime * timeScale) / 3600f;
        if (hour >= 24f) hour = 0f;

        if (sunLight == null) FindSun();
        if (sunLight == null) return;

        float sunFactor = GetSunFactor();

        UpdateSun(sunFactor);
        UpdatePostExposure(sunFactor);
        UpdateIndoorLights(sunFactor);
    }

    float GetSunFactor()
    {
        float t = (hour - 5f) / 14f;
        return Mathf.Clamp01(Mathf.Sin(t * Mathf.PI));
    }

    void UpdateSun(float sunFactor)
    {
        float elevation = (sunFactor * 2f - 1f) * 80f;
        sunLight.transform.rotation = Quaternion.Euler(elevation, direction, 0);
        sunLight.intensity = Mathf.Lerp(0f, 1.3f, sunFactor) * its;
        sunLight.enabled = sunFactor > 0.05f;

        Color night = new Color(0.1f, 0.15f, 0.3f);
        Color sunrise = new Color(1f, 0.5f, 0.2f);
        Color noon = new Color(1f, 0.98f, 0.95f);

        sunLight.color = sunFactor < 0.2f
            ? Color.Lerp(night, sunrise, sunFactor * 5f)
            : Color.Lerp(sunrise, noon, (sunFactor - 0.2f) * 1.25f);
    }

    /// <summary>
    /// 核心：通过 Color Grading 的 Post Exposure 控制整体亮度
    /// </summary>
    void UpdatePostExposure(float sunFactor)
    {
        if (colorGrading == null) return;

        float targetExp = Mathf.Lerp(nightPostExposure, dayPostExposure, sunFactor);
        currentPostExposure = Mathf.Lerp(currentPostExposure, targetExp, Time.deltaTime * 2f);

        // Post-Processing Stack v2 的设置方式
        colorGrading.postExposure.value = currentPostExposure;
        colorGrading.postExposure.overrideState = true;
    }

    void UpdateIndoorLights(float sunFactor)
    {
        if (indoorLights == null || originalIntensities == null) return;

        float mult = Mathf.Lerp(nightIndoorMult, dayIndoorMult, sunFactor);

        for (int i = 0; i < indoorLights.Length; i++)
        {
            if (indoorLights[i] == null) continue;

            float target = originalIntensities[i] * mult;
            indoorLights[i].intensity = Mathf.Lerp(indoorLights[i].intensity, target, Time.deltaTime * 3f);

            // 夜晚变暖光
            if (sunFactor < 0.3f)
                indoorLights[i].color = Color.Lerp(indoorLights[i].color, new Color(1f, 0.8f, 0.6f), Time.deltaTime * 2f);
        }
    }

    void FindSun()
    {
        if (RenderSettings.sun != null) sunLight = RenderSettings.sun;
        else
        {
            foreach (var l in FindObjectsOfType<Light>())
                if (l.type == LightType.Directional) { sunLight = l; return; }
        }
    }

    [ContextMenu("自动收集室内灯光")]
    void AutoCollect()
    {
        var list = new System.Collections.Generic.List<Light>();
        foreach (var l in FindObjectsOfType<Light>())
            if (l.type != LightType.Directional && l != sunLight) list.Add(l);

        indoorLights = list.ToArray();
        CacheIndoorLights();
        Debug.Log($"收集到 {indoorLights.Length} 个室内灯");
    }

    [ContextMenu("测试夜晚 (23点)")]
    void TestNight() { hour = 23f; }

    [ContextMenu("测试白天 (12点)")]
    void TestDay() { hour = 12f; }
}