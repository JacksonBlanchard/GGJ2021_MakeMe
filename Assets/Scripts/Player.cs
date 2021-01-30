using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera cam;
    private Inventory inventory;

    public Interactable focus;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        cam.cullingMask = 7 << 0;
        inventory = new Inventory();
    }

    // Update is called once per frame
    void Update()
    {
        // press left mouse button
        if(Input.GetMouseButtonDown(0))
        {
            // create ray
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // if the ray hits an object
            if(Physics.Raycast(ray, out hit, 100))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null)
                {
                    SetFocus(interactable);
                }
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if(newFocus != focus)
        {
            if(focus != null)
                focus.onDefocused();
            focus = newFocus;
        }

        newFocus.onFocused(gameObject);
    }

    void RemoveFocus()
    {
        if(focus != null)
            focus.onDefocused();
        focus = null;
    }

    public void AddToInventory(Item item)
    {
        inventory.AddItem(item);
        UpdateCameraCullingMask(item);
    }

    public bool InventoryContains(Item.ItemType itemType)
    {
        return inventory.Contains(itemType);
    }

    private void UpdateCameraCullingMask(Item item)
    {
        if(item.itemType == Item.ItemType.Nose)
        {
            
        }
    }
}
