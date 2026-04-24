using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BillboardY : MonoBehaviour
{
    [SerializeField] private bool lockY = true; // true = 只在水平面旋转，箭头保持直立

    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 toCamera = cam.transform.position - transform.position;

        if (lockY)
        {
            toCamera.y = 0;
        }

        if (toCamera.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up);
        }
    }
}