using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopSlot
{
    public MaterialData material;  // 物资数据
    public int price;              // 实际售价
    public bool isSold;           // 是否已售出
}

[CreateAssetMenu(fileName = "ShopSlot", menuName = "CreateData/ShopSlot", order = 2)]
public class ShopSlotData : ScriptableObject
{
    public List<ShopSlot> slots = new List<ShopSlot>(4);  // 4个商品位置
}