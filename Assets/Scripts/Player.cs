using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    Transform cam;
    Interaction interact;
    CapsuleCollider col;
    CapsuleCollider wallCol;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 3f;
    public float crouchHeight;
    bool iscrouched = false;

    [Header("Ground check")]
    public Transform groundCheck;
    public float groundDistance = 0.45f;
    public LayerMask groundMask;
    public Transform ceilingCheck;
    public float ceilingDistance = 0.45f;


    [Header("Mouselook")]
    public float mouseSensitivity = 2f;
    
    float moveSpeedMultiplier = 1f;
    float moveX, moveZ;
    float moveSpeed;
    float mouseX, mouseY;
    float xRotation = 0f;
    float originalHeight;
    float groundCheckHeight;

    [Header("Footsteps")]
    public AudioClip footsteps;
    AudioSource audioSource;
    float TimeToNextFootsteps = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>().transform;
        interact = GetComponent<Interaction>();

        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        col = GetComponent<CapsuleCollider>();
        wallCol = transform.GetChild(0).GetComponent<CapsuleCollider>();
        originalHeight = col.height;
        groundCheckHeight = groundCheck.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (interact.HasItemInHands())
            moveSpeedMultiplier = interact.GetItemInHands().MoveSpeedMultiplier();
        else 
            moveSpeedMultiplier = 1f;

        //Get input
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        if (IsGrounded())
        {
            //Apply walk speed
            moveSpeed = walkSpeed * moveSpeedMultiplier;        

            if (interact.HasItemInHands() != true)
            {
                //Override to run speed
                if (Input.GetKey(KeyCode.LeftShift) && moveZ > 0)
                    moveSpeed = sprintSpeed;
            }

            //Jump
            if (Input.GetButtonDown("Jump"))
                rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
        }

        //Get mouse movement
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        //Apply vertical rotation and clamp it
        xRotation = cam.localEulerAngles.x + mouseY;
        Quaternion cameraRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cam.localRotation = cameraRotation;

        //Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        //Crouching
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouch();
            iscrouched = true;

        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StandUp();
        }
        else if (CanStandUp() == true)
        {
            StandUp();
            iscrouched = false;
        }

    }

    void FixedUpdate()
    {

        Footsteps();


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

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Camera"))
        {
            SecurityCamera securityCam = collider.GetComponentInParent<SecurityCamera>();
            securityCam.SetTarget(transform);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Camera"))
        {
            SecurityCamera securityCam = collider.GetComponentInParent<SecurityCamera>();
            securityCam.SetTarget(null);
        }
    }

    bool IsGrounded()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask)) return true;
        return false;
    }

    bool IsUnder()
    {
        if (Physics.CheckSphere(ceilingCheck.position, ceilingDistance, groundMask)) return true;
        return false;
    }

    void Crouch()
    {
        col.height = crouchHeight;
        wallCol.height = crouchHeight;
        //groundCheck.position = groundCheck.position + new Vector3(0,0.5f,0);
    }

    void StandUp()
    {   
        if (IsUnder() == false)
        {
            col.height = originalHeight;
            wallCol.height = originalHeight;
            //groundCheck.position = groundCheck.position + new Vector3(0, -0.5f, 0);
        }
    }

    bool CanStandUp()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) return false;
        return true;
    }

    void Footsteps()
    {
        TimeToNextFootsteps = TimeToNextFootsteps - Time.deltaTime;
        Vector3 vel = rb.velocity;
        Debug.Log(vel.magnitude);

         //sprint footsteps
        if (IsGrounded() && vel.magnitude > 6.5f && TimeToNextFootsteps <= 0f && !iscrouched)
        {
            audioSource.Play();

            TimeToNextFootsteps = 0.25f;

        }

        //walking footsteps
        else if (IsGrounded() && vel.magnitude > 3f && TimeToNextFootsteps <= 0f && !iscrouched)
        {
            audioSource.Play();

            TimeToNextFootsteps = 0.5f;
        }

        //crouched footsteps
        else if (IsGrounded() && vel.magnitude > 3f && TimeToNextFootsteps <= 0f && iscrouched)
        {
            audioSource.Play();

            TimeToNextFootsteps = 1f;
        }

       
    }
}
