using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private RectTransform cursorImage;
    [SerializeField] private Vector2 hotSpotOffset; // 额外微调（像素）

    void Start()
    {
        if (cursorImage == null)
        {
            Debug.LogError("cursorImage 未赋值！请拖到 Inspector");
            enabled = false;
            return;
        }

        Cursor.visible = false;

    }

    void Update()
    {
        if (cursorImage == null) return;

        // 强制隐藏系统光标（防止 ESC 弹出）
        if (Cursor.visible) Cursor.visible = false;

        // 显式用 Vector3，避免 Vector2 混算报错
        Vector3 pos = Input.mousePosition;
        pos.x += hotSpotOffset.x;
        pos.y += hotSpotOffset.y;

        cursorImage.position = pos;
    }
}