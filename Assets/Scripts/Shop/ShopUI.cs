using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("商店管理器")]
    public ShopManager shopManager;

    [Header("4个商品槽位")]
    public List<ShopSlotUI> slots = new List<ShopSlotUI>(4);

    [Header("刷新按钮")]
    public Button refreshButton;
    public TextMeshProUGUI refreshCostText;

    [Header("金钱显示")]
    public TextMeshProUGUI moneyText;

    void Start()
    {
        // 绑定刷新按钮
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefresh);

        // 绑定4个槽位购买按钮
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            if (slots[i] != null && slots[i].buyButton != null)
                slots[i].buyButton.onClick.AddListener(() => OnBuy(index));
        }

        UpdateUI();
    }

    void OnEnable()
    {
        // 每次面板打开时，确保商店数据是最新的
        if (shopManager != null)
        {
            var slots = shopManager.GetSlots();
            // 如果槽位为空或者第一个商品是空的，就刷新商店
            if (slots == null || slots.Count == 0 || slots[0] == null || slots[0].material == null)
            {
                shopManager.RefreshShop();
            }
        }
        UpdateUI();
    }

    void OnRefresh()
    {
        if (shopManager != null)
        {
            shopManager.RefreshShop();
            UpdateUI();
        }
    }

    void OnBuy(int index)
    {
        if (shopManager != null && shopManager.BuyItem(index))
        {
            UpdateUI();
        }
    }

    // 更新所有UI
    public void UpdateUI()
    {
        // 防止空引用
        if (shopManager == null)
        {
            Debug.LogError("ShopUI: shopManager 为空！请在 Inspector 中赋值");
            return;
        }

        // 更新金钱显示
        if (moneyText != null)
            moneyText.text = $"金钱: {GameDataManager.Instance.playermoney}";

        // 更新刷新按钮状态
        if (refreshButton != null)
        {
            bool canRefresh = shopManager.CanRefresh();
            refreshButton.interactable = canRefresh;
            if (refreshCostText != null)
                refreshCostText.text = $"刷新 (-{shopManager.refreshCost})";
        }

        // 获取槽位数据
        var slotDatas = shopManager.GetSlots();

        // 更新4个槽位
        for (int i = 0; i < slots.Count; i++)
        {
            var ui = slots[i];

            // 如果UI组件为空，跳过
            if (ui == null) continue;

            // 检查是否有有效的商品数据
            if (slotDatas != null && i < slotDatas.Count && slotDatas[i].material != null)
            {
                var data = slotDatas[i];
                var mat = data.material;

                // 显示物资信息
                if (ui.nameText != null) ui.nameText.text = mat.materialName;
                if (ui.kindText != null) ui.kindText.text = mat.kind.ToString();
                if (ui.levelText != null) ui.levelText.text = new string('★', mat.level);
                if (ui.priceText != null) ui.priceText.text = $"${data.price}";

                // 按钮状态
                if (ui.buyButton != null)
                {
                    bool canBuy = !data.isSold && shopManager.CanBuy(i);
                    ui.buyButton.interactable = canBuy;
                }
                if (ui.soldOutObj != null) ui.soldOutObj.SetActive(data.isSold);
            }
            else
            {
                // 空槽位显示
                if (ui.nameText != null) ui.nameText.text = "空";
                if (ui.kindText != null) ui.kindText.text = "";
                if (ui.levelText != null) ui.levelText.text = "";
                if (ui.priceText != null) ui.priceText.text = "";
                if (ui.buyButton != null) ui.buyButton.interactable = false;
                if (ui.soldOutObj != null) ui.soldOutObj.SetActive(true);
            }
        }
    }
}

[System.Serializable]
public class ShopSlotUI
{
    public Button buyButton;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI kindText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI priceText;
    public GameObject soldOutObj;
}