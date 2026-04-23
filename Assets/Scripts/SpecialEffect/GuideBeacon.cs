using UnityEngine;

public class GuideBeacon : MonoBehaviour
{
    [Header("地面光圈")]
    [SerializeField] private Transform groundRing;      // Quad 物体
    [SerializeField] private Material ringMaterial;   // 运行时实例化，避免共享材质冲突
    [SerializeField] private float ringScale = 2f;    // 地面光圈大小

    [Header("悬浮箭头")]
    [SerializeField] private Transform arrow;         // SpriteRenderer 物体
    [SerializeField] private float arrowHeight = 2f;  // 悬浮高度
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] private float bobHeight = 0.3f;

    [Header("脉冲参数")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;

    private Material instanceMat;
    private Vector3 arrowBasePos;
    private float randomPhase;

    void Start()
    {
        // 实例化材质，避免多个信标互相影响
        if (ringMaterial != null)
        {
            instanceMat = new Material(ringMaterial);
            var renderer = groundRing.GetComponent<Renderer>();
            if (renderer != null) renderer.material = instanceMat;
        }

        // 初始化箭头位置
        arrowBasePos = new Vector3(0, arrowHeight, 0);
        arrow.localPosition = arrowBasePos;

        // 随机相位，多个信标错开动画
        randomPhase = Random.Range(0f, Mathf.PI * 2);

        // 地面光圈初始旋转（平躺）
        groundRing.rotation = Quaternion.Euler(90, 0, 0);
        groundRing.localScale = Vector3.one * ringScale;
    }

    void Update()
    {
        // 脉冲缩放
        float t = Mathf.PingPong((Time.time + randomPhase) * pulseSpeed, 1f);
        float s = Mathf.Lerp(minScale, maxScale, t);
        groundRing.localScale = Vector3.one * (ringScale * s);

        // 脉冲强度写入 Shader
        if (instanceMat != null)
            instanceMat.SetFloat("_Intensity", Mathf.Lerp(1.5f, 3f, t));

        // 箭头上下浮动
        float bob = Mathf.Sin((Time.time + randomPhase) * bobSpeed) * bobHeight;
        arrow.localPosition = arrowBasePos + Vector3.up * bob;
    }

    // 设置信标位置（自动贴合地面高度）
    public void SetPosition(Vector3 worldPos, float groundY)
    {
        transform.position = new Vector3(worldPos.x, groundY + 0.02f, worldPos.z);
    }

    // 直接设置位置（不贴合地面，手动控制Y）
    public void SetPosition(Vector3 worldPos)
    {
        transform.position = worldPos;
    }

    void OnDestroy()
    {
        if (instanceMat != null) Destroy(instanceMat);
    }
}