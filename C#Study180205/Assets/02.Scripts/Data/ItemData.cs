﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int ItemID = 0;
    public string ItemName = "";
    public string Category = "";
    public string IconName = "";
    public int Cost = 0;
    public string MaterialItemID = "";
}

[System.Serializable]
public class ItemDataArray
{
    public List<Item> ItemList;
}

public class ItemData : MonoBehaviour {

    Item item;
    static ItemDataArray list = new ItemDataArray();

    //public List<Item> disposableItemList
    //{
    //    private set;
    //    get;
    //}
    //public List<Item> wearableItemList
    //{
    //    private set;
    //    get;
    //}

    //public List<Item> foodItemList
    //{
    //    private set;
    //    get;
    //}

    //private static ItemData instance = null;

    //public static ItemData Instance
    //{
    //    get
    //    {
    //        return instance;
    //    }
    //}

    //private void Awake()
    //{
    //    if(instance)
    //    {
    //        DestroyImmediate(gameObject);
    //        return;
    //    }

    //    instance = this;
    //    DontDestroyOnLoad(this);

    //    TextAsset data = Resources.Load("Data/" + "ItemJSON") as TextAsset;
    //    list = JsonUtility.FromJson<ItemDataArray>(data.text);
    //    //SortingItemCategory();
    //}
	
    public static void Read()
    {
        TextAsset data = Resources.Load("Data/" + "ItemJSON") as TextAsset;
        list = JsonUtility.FromJson<ItemDataArray>(data.text);
    }

    public static Item FindItemDataByID(int id)
    {
        foreach(Item i in list.ItemList)
        {
            if (i.ItemID == id)
                return i;
        }
        return null;
    }

    public static Item FindItemDataByIconName(string name)
    {
        foreach(Item i in list.ItemList)
        {
            if (i.IconName == name)
                return i;
        }
        return null;
    }

    //void SortingItemCategory()
    //{
    //    disposableItemList = new List<Item>();
    //    wearableItemList = new List<Item>();
    //    foodItemList = new List<Item>();
    //    foreach(Item i in list.ItemList)
    //    {
    //        if (i.Category == "Disposal")
    //            disposableItemList.Add(i);
    //        if (i.Category == "Wearable")
    //            wearableItemList.Add(i);
    //        if (i.Category == "Food")
    //            foodItemList.Add(i);
    //    }
    //}

}
