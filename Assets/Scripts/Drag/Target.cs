using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    [Header("设置")]
    public Drag.BlockType acceptedType;
    public bool showGhost = true;

    [Header("虚影显示")]
    public Sprite ghostSprite;           // 虚影图片（半透明边框）
    public Color ghostColor = new Color(1, 1, 1, 0.3f);

    private Image ghostImage;
    private bool isFilled = false;

    void Start()
    {
        SetupGhost();
    }

    void SetupGhost()
    {
        // 创建虚影子对象
        GameObject ghost = new GameObject("Ghost");
        ghost.transform.SetParent(transform, false);

        RectTransform ghostRect = ghost.AddComponent<RectTransform>();
        ghostRect.anchorMin = Vector2.zero;
        ghostRect.anchorMax = Vector2.one;
        ghostRect.sizeDelta = Vector2.zero;
        ghostRect.anchoredPosition = Vector2.zero;

        ghostImage = ghost.AddComponent<Image>();
        ghostImage.sprite = ghostSprite;
        ghostImage.color = ghostColor;
        ghostImage.raycastTarget = false;  

        
        if (ghostSprite == null)
        {
            ghostImage.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        }
    }

    public void OnBlockPlaced(Drag block)
    {
        isFilled = true;

       
        if (ghostImage != null)
        {
            ghostImage.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        isFilled = false;
        if (ghostImage != null)
        {
            ghostImage.color = ghostColor;
        }
    }
}