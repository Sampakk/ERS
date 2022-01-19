using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public Transform cam;

    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.45f;
    public LayerMask groundMask;

    float moveX, moveZ;
    float moveSpeed;

    public float mouseSensitivity = 2f;
    float mouseX, mouseY;
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        if (IsGrounded())
        {
            //Apply walk speed
            moveSpeed = walkSpeed;

            //Override to run speed
            if (Input.GetKey(KeyCode.LeftShift) && moveZ > 0 )
                moveSpeed = sprintSpeed;

            //Jump
            if (Input.GetButtonDown("Jump"))
                rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);


        }
            //Get mouse movement
            mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        //Apply vertical rotation and clamp it
        xRotation = cam.localEulerAngles.x + mouseY;
        Quaternion cameraRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cam.localRotation = cameraRotation;

        //Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
    }
    void FixedUpdate()
    {
        //Get move direction
        Vector3 moveDir = transform.right * moveX + transform.forward * moveZ;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        //Add force
        rb.AddForce(moveDir, ForceMode.Impulse);

        //Clamp movement
        float verticalVelocity = Mathf.Clamp(rb.velocity.y, float.MinValue, jumpHeight);
        Vector3 horizontalVelocity = new Vector3(moveDir.x * moveSpeed, verticalVelocity, moveDir.z * moveSpeed);
        rb.velocity = horizontalVelocity;
    }
    bool IsGrounded()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask)) return true;
        return false;
    }

}
