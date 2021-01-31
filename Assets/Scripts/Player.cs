using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera cam;
    private Inventory inventory;
    public Animator animator;

    public Interactable focus;

    public GameObject playerModel;
    public GameObject playerSphere;
    public GameObject playerStand;

    private Rigidbody sphereBody;

    public GameObject parentStartPoint;

    //Used to scale the player after eating
    public float sizeMultiplier;

    //Used to test different movement modes
    //0 = no limbs, 1 = hopping on one limb, 2 = walking normally
    public int limbCount;
    private int prevLimbCount;

    public int armCount;
    public int legCount;
    
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

    /* Animated GameObject Prefabs */
    // arms
    public GameObject humanArmL;
    public GameObject humanArmR;
    public GameObject noFingerArmL;
    public GameObject noFingerArmR;
    public GameObject wingL;
    public GameObject wingR;
    // legs
    public GameObject humanLegL;
    public GameObject humanLegR;
    public GameObject miniLegL;
    public GameObject miniLegR;
    public GameObject octoLegL;
    public GameObject octoLegR;
    // tails
    public GameObject dolphinTail;
    public GameObject hairBallTail;
    public GameObject reptilesTail;

    // Start is called before the first frame update
    void Awake()
    {
        Hide("Player");
        //cam = Camera.main;
        //cam.cullingMask = -1;
        ///cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Smell"));
        /*
#if DEBUG
        cam.cullingMask = -1; // see everything
#else
        cam.cullingMask = 0; // player starts off completely blind
#endif
        */
        Debug.Log("culling mask: " + cam.cullingMask);

        inventory = new Inventory();
        //animator = gameObject.GetComponentInChildren<Animator>();

        sphereBody = playerSphere.GetComponent<Rigidbody>();
        controller = playerStand.GetComponent<CharacterController>();
        groundCheckRadius = 0.25f;
        SetUpNewBody();
        sizeMultiplier = 1f;
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

            //TEMPORARY, FOR TESTING
            ResizePlayer(0.1f);
        }

        // press left mouse button
        if (Input.GetMouseButtonDown(0))
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
        armCount = inventory.ItemTypeCount(Item.ItemType.Arm);
        legCount = inventory.ItemTypeCount(Item.ItemType.Leg);

        limbCount = armCount + legCount;

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

        animator.SetInteger("armCount", armCount);
        animator.SetInteger("legCount", legCount);
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

        playerModel.transform.position = playerSphere.transform.position;
        playerModel.transform.rotation = playerSphere.transform.rotation;
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

        float verticalOffset = 0f;
        if (legCount > 0)
            verticalOffset = 1.25f;
        else if (armCount > 0)
            verticalOffset = 0.5f;

        playerModel.transform.position = playerStand.transform.position + new Vector3(0, verticalOffset, 0);
        playerModel.transform.forward = playerStand.transform.forward;
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

        float verticalOffset = 0f;
        if (legCount > 0)
            verticalOffset = 1.25f;
        else if (armCount > 0)
            verticalOffset = 0.5f;

        playerModel.transform.position = playerStand.transform.position + new Vector3(0, verticalOffset, 0);
        playerModel.transform.forward = playerStand.transform.forward;
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
        Debug.Log("Setting up new body");

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


    //Author: RIT_Kyle
    //Physically resize player bodies by scaling the parent. Then adjust forces to compensate for increased size
    //Should be called when the player eats things
    public void ResizePlayer(float sizeChange)
    {
        //Temporarily reset values to their initial states
        rollSpeed /= sizeMultiplier;
        hopSpeed /= sizeMultiplier;
        hopHeight /= sizeMultiplier;
        walkSpeed /= sizeMultiplier;
        jumpHeight /= sizeMultiplier;

        //Modify sizeMultiplier to new value
        sizeMultiplier += sizeChange;

        //Set values to match new size
        rollSpeed *= sizeMultiplier;
        hopSpeed *= sizeMultiplier;
        hopHeight *= sizeMultiplier;
        walkSpeed *= sizeMultiplier;
        jumpHeight *= sizeMultiplier;

        //Resize parent player tranform to scale all child bodies
        parentStartPoint.transform.localScale = Vector3.one * sizeMultiplier;
    }

    public void AddArm(string armName)
    {
        switch(armName)
        {
            case "Arm_L_Human":
                Instantiate(humanArmL, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "Arm_R_Human":
                Instantiate(humanArmR, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "NoFingerArm_L":
                Instantiate(noFingerArmL, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "NoFingerArm_R":
                Instantiate(noFingerArmR, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "Wing_L":
                Instantiate(wingL, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "Wing_R":
                Instantiate(wingR, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            default:
                Instantiate(humanArmL, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
        }
        Debug.Log("Added animated arm to model");
    }

    public void AddLeg(string legName)
    {
        switch (legName)
        {
            case "Leg_L_Human":
                Instantiate(humanLegL, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "Leg_R_Human":
                Instantiate(humanLegR, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "MiniLeg_L":
                Instantiate(miniLegL, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "MiniLeg_R":
                Instantiate(miniLegR, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "OctoLeg_L":
                Instantiate(octoLegL, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "OctoLeg_R":
                Instantiate(octoLegR, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
        }
        Debug.Log("Added animated leg to model");
    }

    public void AddTail(string tailName)
    {
        switch (tailName)
        {
            case "DolphinTail":
                Instantiate(dolphinTail, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "HairBallTail":
                Instantiate(hairBallTail, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
            case "ReptilesTail":
                Instantiate(reptilesTail, playerModel.transform.position, playerModel.transform.rotation, playerModel.transform);
                break;
        }
        Debug.Log("Added animated tail to model");
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
