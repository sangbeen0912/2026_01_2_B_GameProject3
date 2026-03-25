using System;
using UnityEngine;

public class ItemData
{
    public int id;
    public string itemName;
    public string description;
    public string nameEng;
    public string itemTypyString;

    [NonSerialized]
    public ItemType itemType;
    public int price;
    public int power;
    public int level;
    public bool isStackale;
    public string iconPath;


    public void InitalizeEnums()
    {
        if (Enum.TryParse(itemTypyString, out ItemType parsedType))
        {
            itemType = parsedType;
        }
        else
        {
            Debug.LogError($"아이템'{itemName} 에 유효하지 않은 아이템 타입;{itemTypyString}");
            itemType = ItemType.Consumable;
        }
    }
}
