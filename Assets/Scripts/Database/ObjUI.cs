using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public enum Type
    {
        inbag,
        pickobj,
        pickbag,
        store
    }
    public MaterialData obj;
    public Type type;
    private GameObject stars;
    private Image back;


    private float enterTime;
    private bool isHovering;
    private bool isShown;

    public void OnPointerEnter(PointerEventData eventData)
    {
        enterTime = Time.time;
        isHovering = true;
        isShown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        TooltipManager.Instance.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!isShown)
        {
            // 检查延迟
            if (Time.time - enterTime >= TooltipManager.Instance.showDelay)
            {
                Show(eventData);
            }
        }
        else
        {
            // 已显示，更新位置
            TooltipManager.Instance.UpdatePosition(eventData.position + TooltipManager.Instance.offset);
        }
    }

    private void Show(PointerEventData eventData)
    {
        isShown = true;
        if (string.IsNullOrEmpty(obj.materialName))
            TooltipManager.Instance.Show(obj.description);
        else
            TooltipManager.Instance.Show(obj.materialName, obj.description);

        TooltipManager.Instance.UpdatePosition(eventData.position + TooltipManager.Instance.offset);
    }

    void Start()
    {

    }

    public void Init()
    {
        if (stars == null)
        {
            stars = transform.GetChild(4).gameObject;
            for (int i = 0; i <= 2; i++)
            {
                stars.transform.GetChild(i).gameObject.SetActive(i < obj.level);
            }
        }

        if (back == null)
        {
            back = transform.GetChild(0).GetComponent<Image>();
        }

        Button btn=gameObject.AddComponent<Button>();
        switch (type)
        {
            case Type.inbag:

                break;
            case Type.pickobj:

                break;
            case Type.pickbag:

                break;
            case Type.store:

                break;
        }
    }

    void Update()
    {

    }

    void Move()
    {

    }
}
