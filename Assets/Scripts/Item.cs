using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public enum ItemType
    {
        Leg,
        Arm,
        Eye,
        Ear,
        Nose,
    }

    public ItemType itemType;
    public int amount;

    override public void Interact()
    {
        player.GetComponent<Player>().AddToInventory(this);
        gameObject.SetActive(false);
        Debug.Log("Adding " + name + " to player inventory");
    }
}
