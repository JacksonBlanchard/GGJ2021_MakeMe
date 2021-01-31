using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();

        //AddItem(new Item { itemType = Item.ItemType.Eye, amount = 1 });
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
        Debug.Log("item " + item.name + " added to inventory (size " + itemList.Count + ")");
    }

    public bool Contains(Item.ItemType itemType)
    {
        foreach(Item i in itemList)
        {
            if(i.itemType == itemType)
            {
                return true;
            }
        }
        return false;
    }

    public int ItemTypeCount(Item.ItemType itemType)
    {
        int count = 0;
        foreach(Item item in itemList)
        {
            if(item.itemType == itemType)
                count++;
        }
        return count;
    }
}
