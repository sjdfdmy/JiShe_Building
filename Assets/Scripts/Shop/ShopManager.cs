using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("数据")]
    public ShopSlotData shopSlotData;           // 商店槽位数据（ScriptableObject）
    public List<MaterialData> allMaterials;     // 所有可用物资池

    [Header("配置")]
    public int slotCount = 4;                   // 固定4个槽位
    public int refreshCost = 50;                // 刷新花费

    // 刷新商店 - 调用这个函数即可
    public void RefreshShop()
    {
        // 检查是否有物资池
        if (allMaterials == null || allMaterials.Count == 0)
        {
            Debug.LogError("RefreshShop: allMaterials 为空！请在 Inspector 中添加物资");
            return;
        }

        // 检查金钱是否足够
        if (GameDataManager.Instance.playermoney < refreshCost)
        {
            Debug.Log("金钱不足，无法刷新！");
            return;
        }

        // 扣钱
        GameDataManager.Instance.playermoney -= refreshCost;

        // 清空并重新生成
        shopSlotData.slots.Clear();

        List<MaterialData> pool = new List<MaterialData>(allMaterials);
        List<MaterialData> selected = new List<MaterialData>();

        // 按权重选4个
        for (int i = 0; i < slotCount && pool.Count > 0; i++)
        {
            int totalWeight = pool.Sum(m => m.simvalravity);
            if (totalWeight <= 0) break;

            int random = Random.Range(0, totalWeight);
            int current = 0;

            foreach (var mat in pool)
            {
                current += mat.simvalravity;
                if (random < current)
                {
                    selected.Add(mat);
                    pool.Remove(mat);
                    break;
                }
            }
        }

        // 保证不全是同种类（至少有一个不同种类）
        if (selected.Count >= 3 && selected.All(m => m.kind == selected[0].kind))
        {
            var different = allMaterials.FirstOrDefault(m => m.kind != selected[0].kind);
            if (different != null) selected[selected.Count - 1] = different;
        }

        // 填充槽位
        for (int i = 0; i < slotCount; i++)
        {
            if (i < selected.Count && selected[i] != null)
            {
                shopSlotData.slots.Add(new ShopSlot
                {
                    material = selected[i],
                    price = selected[i].value,
                    isSold = false
                });
            }
            else
            {
                // 空槽位（理论上不会发生，但以防万一）
                shopSlotData.slots.Add(new ShopSlot
                {
                    material = null,
                    price = 0,
                    isSold = true
                });
            }
        }

        Debug.Log($"商店已刷新！当前金钱：{GameDataManager.Instance.playermoney}");
    }

    // 购买商品
    public bool BuyItem(int slotIndex)
    {
        if (shopSlotData == null || shopSlotData.slots == null)
            return false;

        if (slotIndex < 0 || slotIndex >= shopSlotData.slots.Count)
            return false;

        var slot = shopSlotData.slots[slotIndex];
        if (slot.isSold || slot.material == null)
            return false;

        int money = GameDataManager.Instance.playermoney;
        if (money < slot.price)
        {
            Debug.Log("金钱不足！");
            return false;
        }

        // 扣钱并标记售出
        GameDataManager.Instance.playermoney -= slot.price;
        slot.isSold = true;

        Debug.Log($"购买 {slot.material.materialName}，剩余金钱：{GameDataManager.Instance.playermoney}");
        return true;
    }

    // 获取当前槽位
    public List<ShopSlot> GetSlots()
    {
        if (shopSlotData == null)
            return new List<ShopSlot>();
        return shopSlotData.slots;
    }

    // 检查刷新按钮是否可用
    public bool CanRefresh()
    {
        if (GameDataManager.Instance == null) return false;
        return GameDataManager.Instance.playermoney >= refreshCost;
    }

    // 检查购买按钮是否可用
    public bool CanBuy(int slotIndex)
    {
        if (shopSlotData == null || shopSlotData.slots == null)
            return false;
        if (slotIndex < 0 || slotIndex >= shopSlotData.slots.Count)
            return false;
        var slot = shopSlotData.slots[slotIndex];
        return !slot.isSold && slot.material != null
            && GameDataManager.Instance != null
            && GameDataManager.Instance.playermoney >= slot.price;
    }

    void Awake()
    {
        // 在 Awake 中初始化，确保数据准备好
        if (shopSlotData != null && (shopSlotData.slots == null || shopSlotData.slots.Count == 0))
        {
            // 注意：不在 Awake 中调用 RefreshShop，因为需要扣钱，等 Start 中再调用
        }
    }

    void Start()
    {
        // 如果槽位为空，刷新商店（不扣钱的首次初始化？）
        // 注意：首次刷新不应该扣钱，所以这里需要特殊处理
        if (shopSlotData != null && (shopSlotData.slots == null || shopSlotData.slots.Count == 0 ||
            (shopSlotData.slots.Count > 0 && shopSlotData.slots[0].material == null)))
        {
            // 首次初始化，不扣钱的刷新
            InitializeShop();
        }
    }

    // 首次初始化商店（不扣钱）
    void InitializeShop()
    {
        if (allMaterials == null || allMaterials.Count == 0)
        {
            Debug.LogError("InitializeShop: allMaterials 为空！");
            return;
        }

        shopSlotData.slots.Clear();

        List<MaterialData> pool = new List<MaterialData>(allMaterials);
        List<MaterialData> selected = new List<MaterialData>();

        // 按权重选4个
        for (int i = 0; i < slotCount && pool.Count > 0; i++)
        {
            int totalWeight = pool.Sum(m => m.simvalravity);
            if (totalWeight <= 0) break;

            int random = Random.Range(0, totalWeight);
            int current = 0;

            foreach (var mat in pool)
            {
                current += mat.simvalravity;
                if (random < current)
                {
                    selected.Add(mat);
                    pool.Remove(mat);
                    break;
                }
            }
        }

        // 填充槽位
        for (int i = 0; i < slotCount; i++)
        {
            if (i < selected.Count && selected[i] != null)
            {
                shopSlotData.slots.Add(new ShopSlot
                {
                    material = selected[i],
                    price = selected[i].value,
                    isSold = false
                });
            }
            else
            {
                shopSlotData.slots.Add(new ShopSlot
                {
                    material = null,
                    price = 0,
                    isSold = true
                });
            }
        }

        Debug.Log($"商店首次初始化完成！");
    }
}