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
        Tail,
        Wing,
    }

    public ItemType itemType;

    override public void Interact()
    {
        Debug.Log("Interacting");
        switch(itemType)
        {
            case ItemType.Arm: // destroy non animated item
                player.GetComponent<Player>().AddArm(this.name);
                Destroy(gameObject);
                break;
            case ItemType.Leg: // destroy non animated item
                player.GetComponent<Player>().AddLeg(this.name);
                Destroy(gameObject);
                break;
            case ItemType.Tail: // destroy non animated item
                player.GetComponent<Player>().AddTail(this.name);
                Destroy(gameObject);
                break;
            case ItemType.Nose: // display Smell layer when Nose is equipped
                player.GetComponent<Player>().Show("Smell");
                attachToPlayer();
                break;
            case ItemType.Eye_Orthographic: // set camera to Orthographic
                player.GetComponent<Player>().Orthographic();
                attachToPlayer();
                break;
            case ItemType.Eye_Perspective: // set camera to Perspective
                player.GetComponent<Player>().Perspective();
                attachToPlayer();
                break;
            default:
                attachToPlayer();
                break;
        }

        player.GetComponent<Player>().AddToInventory(this);
        Debug.Log("Added " + name + " to player");
    }

    public void attachToPlayer()
    {
        Destroy(gameObject.GetComponent<Rigidbody>());
        gameObject.transform.parent = player.GetComponent<Player>().playerModel.transform;
        gameObject.transform.localPosition = Vector3.zero;
    }
}
