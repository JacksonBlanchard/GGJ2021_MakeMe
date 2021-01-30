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
        Eye_Orthographic,
        Eye_Perspective,
        Ear,
        Nose,
        Body,
        Head,
        Mouth,
    }

    public ItemType itemType;

    override public void Interact()
    {
        switch(itemType)
        {
            case ItemType.Nose: // display Smell layer when Nose is equipped
                player.GetComponent<Player>().Show("Smell");
                break;
            case ItemType.Eye_Orthographic: // set camera to Orthographic
                player.GetComponent<Player>().cam.orthographic = true;
                break;
            case ItemType.Eye_Perspective: // set camera to Perspective
                player.GetComponent<Player>().cam.orthographic = false;
                break;
        }

        player.GetComponent<Player>().AddToInventory(this);
        gameObject.SetActive(false);
        Debug.Log("Added " + name + " to player inventory");
    }
}
