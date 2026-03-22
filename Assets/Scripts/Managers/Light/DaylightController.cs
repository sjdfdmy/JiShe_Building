using UnityEngine;

[ExecuteAlways]
public class DaylightController : MonoBehaviour
{
    [Tooltip("拖入Directional Light，或留空自动查找")]
    public Light sunLight;

    [Range(0, 24)]
    public float hour = 12f;

    [Tooltip("太阳水平方向：0=北, 90=东, 180=南, 270=西")]
    [Range(0, 360)]
    public float direction = 180f;

    [Header("时间缩放")]
    [Tooltip("现实中1秒 = 游戏中多少秒")]
    public float timeScale = 60f;

    void OnEnable()
    {
        FindSun();
    }

    void Update()
    {
        hour += (Time.deltaTime * timeScale) / 3600f;
        if (hour >= 24f) hour = 0f;

        if (sunLight == null || sunLight.type != LightType.Directional)
        {
            FindSun();
            if (sunLight == null) return;
        }

        // 全天计算太阳高度（包括夜晚负值）
        float t = (hour - 5f) / 14f;
        float height = Mathf.Sin(t * Mathf.PI);

        // 平滑限制到0-1范围（夜晚为负值， clamp后变暗）
        float sunIntensityFactor = Mathf.Clamp01(height);

        // 应用旋转（允许夜晚负角度，太阳在地平线下）
        float elevation = height * 80f;
        sunLight.transform.rotation = Quaternion.Euler(elevation, direction, 0);

        // 平滑强度：夜晚微光而不是完全消失
        sunLight.intensity = Mathf.Lerp(0.05f, 1.3f, sunIntensityFactor);
        sunLight.enabled = true;

        // 平滑颜色过渡
        Color nightColor = new Color(0.1f, 0.15f, 0.3f);  // 夜晚深蓝
        Color sunriseColor = new Color(1f, 0.5f, 0.2f);     // 日出橙红
        Color noonColor = new Color(1f, 0.98f, 0.95f);      // 正午白偏暖

        if (sunIntensityFactor < 0.1f)
        {
            // 夜晚到日出前
            sunLight.color = Color.Lerp(nightColor, sunriseColor, sunIntensityFactor * 10f);
        }
        else
        {
            // 日出到正午到日落
            sunLight.color = Color.Lerp(sunriseColor, noonColor, sunIntensityFactor);
        }
    }

    void FindSun()
    {
        if (RenderSettings.sun != null)
        {
            sunLight = RenderSettings.sun;
            return;
        }

        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light l in lights)
        {
            if (l.type == LightType.Directional)
            {
                sunLight = l;
                return;
            }
        }

        Debug.LogWarning("未找到Directional Light，请手动拖到Sun Light槽位");
    }
}