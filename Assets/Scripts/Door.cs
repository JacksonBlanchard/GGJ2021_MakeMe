using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public override void Interact()
    {
        if(player.GetComponent<Player>().InventoryContains(Item.ItemType.Arm))
        {
            Debug.Log("Open door");
        }
        else
        {
            Debug.Log("Unable to open door");
        }
    }
}
