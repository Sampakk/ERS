using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    HUD hud;
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
    Quaternion targetRotation;
    bool isCrouched;

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

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaRegen = 5f;
    public float staminaDrain = 5f;
    [HideInInspector] public float currentStamina = 100f;
    public Slider staminaSlider;
    public float jumpStaminaCost = 20f;

    [Header("Health")]
    public float maxHP = 100f;
    [HideInInspector]public float currentHP = 100f;
    public float regenHPAmount = 5f;
    public float regenHPDelay = 10f;
    float regenTimer = 10f;

    [Header("Sounds")]
    public AudioClip[] hurtSounds;
    public AudioClip[] alertSounds;

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
        hud = FindObjectOfType<HUD>();

        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        col = GetComponent<CapsuleCollider>();
        wallCol = transform.GetChild(0).GetComponent<CapsuleCollider>();
        originalHeight = col.height;
        groundCheckHeight = groundCheck.position.y;

        staminaSlider.maxValue = maxStamina;
        staminaSlider.minValue = 0;

        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        staminaSlider.value = currentStamina;

        if (interact.HasItemInHands()) //Carrying item
        {
            if (isCrouched) moveSpeedMultiplier = 0.5f;
            else moveSpeedMultiplier = interact.GetItemInHands().MoveSpeedMultiplier();
        }
        else //Free hands
        {
            if (isCrouched) moveSpeedMultiplier = 0.5f;
            else moveSpeedMultiplier = 1f;
        }
            
        //Get input
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        if (IsGrounded())
        {
            //Apply walk speed
            moveSpeed = walkSpeed * moveSpeedMultiplier;        

            if (!interact.HasItemInHands())
            {
                //Override to run speed
                if (Input.GetKey(KeyCode.LeftShift) && moveZ > 0)
                    moveSpeed = sprintSpeed;
            }

            //Jump
            if (Input.GetButtonDown("Jump") && currentStamina >= jumpStaminaCost)
            {
                rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
                currentStamina -= jumpStaminaCost;
            }
        }

        //Get mouse movement
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        //Apply vertical rotation and clamp it
        xRotation = cam.localEulerAngles.x + mouseY;
        Quaternion cameraRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cam.localRotation = cameraRotation;

        //Crouching
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouch();
            isCrouched = true;

        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StandUp();
        }
        else if (CanStandUp() == true)
        {
            StandUp();
            isCrouched = false;
        }

        //Drain stamina when running
        if (IsRunning() && currentStamina > 0)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
        }

        //Regen stamina when not running
        else if (!IsRunning() && currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
        }

        if (currentStamina <= 0)
        {
            moveSpeed = walkSpeed * moveSpeedMultiplier;
        }

        //Horizontal mouselook
        targetRotation = Quaternion.Euler(0, transform.localEulerAngles.y + mouseX, 0);
        rb.MoveRotation(targetRotation);

        if (currentHP < 100f) RegenHP();
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
        //Alert guards
        if (collider.gameObject.layer == LayerMask.NameToLayer("Camera"))
        {
            SecurityCamera securityCam = collider.GetComponentInParent<SecurityCamera>();
            securityCam.SetTarget(transform);

            //Update hud
            hud.SetStatusText(1);

            //Play eesti
            int random = Random.Range(0, alertSounds.Length);
            audioSource.PlayOneShot(alertSounds[random]);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        //Leaves security cam
        if (collider.gameObject.layer == LayerMask.NameToLayer("Camera"))
        {
            SecurityCamera securityCam = collider.GetComponentInParent<SecurityCamera>();
            securityCam.SetTarget(null);

            //Update hud
            hud.SetStatusText(0);
        }
    }

    public bool IsRunning()
    {
        if (moveSpeed == sprintSpeed)
            return true;

        return false;
    }

    public bool IsCrouched()
    {
        return isCrouched;
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask)) return true;
        return false;
    }

    bool IsUnder()
    {
        if (Physics.CheckSphere(ceilingCheck.position, ceilingDistance, groundMask)) return true;
        return false;
    }

    bool CanStandUp()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) return false;
        return true;
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

    void Footsteps()
    {
        TimeToNextFootsteps = TimeToNextFootsteps - Time.deltaTime;
        Vector3 vel = rb.velocity;
        

         //sprint footsteps
        if (IsGrounded() && vel.magnitude > 6.5f && TimeToNextFootsteps <= 0f && !isCrouched)
        {
            audioSource.volume = 0.4f;
            audioSource.Play();
            TimeToNextFootsteps = 0.3f;
        }

        //walking footsteps
        else if (IsGrounded() && vel.magnitude > 3f && TimeToNextFootsteps <= 0f && !isCrouched)
        {
            audioSource.volume = 0.3f;
            audioSource.Play();
            TimeToNextFootsteps = 0.5f;
        }

        //crouched footsteps
        else if (IsGrounded() && vel.magnitude > 3f && TimeToNextFootsteps <= 0f && isCrouched)
        {
            audioSource.volume = 0.25f;
            audioSource.Play();
            TimeToNextFootsteps = 0.75f;
        }
    }

    public void TakeDamage(float dmgAmount)
    {
        currentHP -= dmgAmount;
        regenTimer = regenHPDelay;

        if (currentHP <= 0) Die();

        //Audio
        int random = Random.Range(0, hurtSounds.Length);
        audioSource.PlayOneShot(hurtSounds[random]);
    }

    void RegenHP()
    {
        regenTimer -= Time.deltaTime;

        if (regenTimer < 0)
        {
            currentHP += regenHPAmount * Time.deltaTime;
            if (currentHP > 100f) currentHP = 100f;
        }
    }

    void Die()
    {
        SceneManager.LoadScene(0);
    }
}
