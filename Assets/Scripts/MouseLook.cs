using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera rotates to follow the player's mouse cursor

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public Transform PlayerBody { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.position = PlayerBody.position;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, transform.rotation.z);

        PlayerBody.Rotate(Vector3.up * mouseX);
    }

    public void ResetForNewBody(GameObject newBody)
    {
        PlayerBody = newBody.transform;
        transform.position = PlayerBody.transform.position;
        yRotation = PlayerBody.transform.localEulerAngles.y;
        
    }
}
