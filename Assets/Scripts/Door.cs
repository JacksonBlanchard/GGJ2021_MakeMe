using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public override void Interact()
    {
        if(player.GetComponent<Player>().InventoryContains(Item.ItemType.Arm))
        {
            Debug.Log("Door opened");
        }
        else
        {
            Debug.Log("Unable to open door");
        }
    }
}
