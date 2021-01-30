using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera cam;
    private Inventory inventory;

    public Interactable focus;

    public GameObject playerSphere;
    public GameObject playerStand;

    private Rigidbody sphereBody;

    //Used to test different movement modes
    //0 = no limbs, 1 = hopping on one limb, 2 = walking normally
    public int limbCount;
    private int prevLimbCount;
    
    public float rollSpeed;
    
    public float hopSpeed;
    public float hopHeight;

    public float walkSpeed;
    public float jumpHeight;

    public float gravity;
    public Vector3 velocity;

    public bool onGround;
    public Transform groundCheck;
    private float groundCheckRadius;
    public LayerMask groundMask;

    public CharacterController controller;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        cam.cullingMask = -1;
        cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Smell"));
        /*
#if DEBUG
        cam.cullingMask = -1; // see everything
#else
        cam.cullingMask = 0; // player starts off completely blind
#endif
        */
        Debug.Log("culling mask: " + cam.cullingMask);

        inventory = new Inventory();
        sphereBody = playerSphere.GetComponent<Rigidbody>();
        controller = playerStand.GetComponent<CharacterController>();
        groundCheckRadius = 0.25f;
        SetUpNewBody();
    }

    // Update is called once per frame
    void Update()
    {
        if (limbCount == 0)
        {
            MoveRoll();
        }
        else if (limbCount == 1)
        {
            MoveHop();
        }
        else
        {
            MoveWalk();
        }

        //Check if the player is grounded
        onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        //Reset velocity when grounded
        if (onGround && velocity.y < 0)
        {
            velocity.y = -0.2f;
        }

        //Allow the player to jump if they have 2 limbs and are grounded
        if (limbCount >= 2 && onGround && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

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
                    Debug.Log("Ray collided with " + interactable.name);
                    SetFocus(interactable);
                }
            }
        }
    }

    void FixedUpdate()
    {
        //If the player's number of limbs has changed, give them the corresponding body
        if (prevLimbCount != limbCount)
        {
            SetUpNewBody();
            Debug.Log("Setting up new body");
        }
        prevLimbCount = limbCount;
    }

    // Author: RIT_Jackson
    // Set the interactable object as the focus for interaction
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

    // Author: RIT_Jackson
    // Reset the current focus object
    void RemoveFocus()
    {
        if(focus != null)
            focus.onDefocused();
        focus = null;
    }

    // Author: RIT_Jackson
    // Add the item to the inventory
    public void AddToInventory(Item item)
    {
        inventory.AddItem(item);

        //TEMPORARY, FOR TESTING
        limbCount++;
    }
    
    // Author: RIT_Jackson
    // Check if the player's inventory contains an Item of the specific ItemType
    public bool InventoryContains(Item.ItemType itemType)
    {
        return inventory.Contains(itemType);
    }
            
    //Author: RIT_Kyle
    //If the player has no arms or legs, they move by rolling
    public void MoveRoll()
    {
        //Get player input
        Vector3 inputDirection = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));

        //Make input relative to camera orientation
        Vector3 actualDirection = cam.transform.TransformDirection(inputDirection);

        sphereBody.AddTorque(actualDirection * rollSpeed * Time.deltaTime);
    }

    //Author: RIT_Kyle
    //If the player has a single arm or leg, they move by hopping
    public void MoveHop()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        velocity.y += gravity;

        Vector3 move = controller.transform.right * x + velocity + controller.transform.forward * z;

        //Force the player to jump if they try to move while grounded
        if (onGround == true && (move.x != 0 || move.z != 0))
        {
            Hop();
        }

        controller.Move(move * hopSpeed * Time.deltaTime);
    }

    //Author: RIT_Kyle
    //If the player has two or more arms or legs, they move by walking
    public void MoveWalk()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        velocity.y += gravity;

        Vector3 move = controller.transform.right * x + velocity + controller.transform.forward * z;

        controller.Move(move * hopSpeed * Time.deltaTime);
    }

    //Author: RIT_Kyle
    //The player can perform short hops when they have one limb
    public void Hop()
    {
        velocity.y = Mathf.Sqrt(hopHeight * -2f * gravity);
    }

    //Author: RIT_Kyle
    //The player can perform higher jumps when they have two limbs
    public void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    //Author: RIT_Kyle
    //Set information necessary for switching from Sphere to Standing body
    public void SetUpNewBody()
    {
        //Set player movement mode based on number of limbs in possession
        if (limbCount == 0)
        {           
            playerSphere.SetActive(true);
            playerStand.SetActive(false);
            playerSphere.transform.position = playerStand.transform.position;
            playerStand.transform.localEulerAngles = new Vector3(0, playerSphere.transform.localEulerAngles.y, 0);

            cam.gameObject.GetComponent<MouseLook>().ResetForNewBody(playerSphere);
        }
        else if (limbCount == 1)
        {
            playerSphere.SetActive(false);
            playerStand.SetActive(true);

            if (prevLimbCount == 0)
            {
                playerStand.transform.position = playerSphere.transform.position;
                playerStand.transform.localEulerAngles = new Vector3(0, playerSphere.transform.localEulerAngles.y, 0);
            }

            cam.gameObject.GetComponent<MouseLook>().ResetForNewBody(playerStand);
        }
        else
        {
            playerSphere.SetActive(false);
            playerStand.SetActive(true);

            if (prevLimbCount == 0)
            {
                playerStand.transform.position = playerSphere.transform.position;
                playerStand.transform.localEulerAngles = new Vector3(0, playerSphere.transform.localEulerAngles.y, 0);
            }           

            cam.gameObject.GetComponent<MouseLook>().ResetForNewBody(playerStand);
        }
    }

    // Author: RIT_Jackson
    // Turn on the bit using an OR operation:
    public void Show(string layerName)
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
    }

    // Author: RIT_Jackson
    // Turn off the bit using an AND operation with the complement of the shifted int:
    public void Hide(string layerName)
    {
        cam.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
    }

    // Author: RIT_Jackson
    // Toggle the bit using a XOR operation:
    public void Toggle(string layerName)
    {
        cam.cullingMask ^= 1 << LayerMask.NameToLayer(layerName);
    }

    // Author: RIT_Jackson
    // Make camera orthographic
    public void Orthographic()
    {
        cam.orthographic = true;
    }

    // Author: RIT_Jackson
    // Make camera perspective
    public void Perspective()
    {
        cam.orthographic = false;
    }
}
