using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 10f;

    public GameObject player;
    bool isFocus = false;
    bool hasInteracted = false;

    public virtual void Interact()
    {
        // overwrite by inhereted interactable object
        Debug.Log("Interacting with " + transform.name);
    }

    void Update()
    {
        if(isFocus && !hasInteracted)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public void onFocused(GameObject playerGO)
    {
        isFocus = true;
        player = playerGO;
        hasInteracted = false;
    }

    public void onDefocused()
    {
        isFocus = false;
        player = null;
        hasInteracted = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
