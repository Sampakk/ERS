using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Outline outline;

    [Header("Impacts")]
    public AudioClip impactSound;
    public float delayBetweenImpacts = 0.5f;
    public float alertRadius = 10f;
    float lastTimeImpacted;

    [Header("Throwing")]
    public float minForceToDMG = 10f;
    public float maxForceToDMG = 20f;  
    public enum WeaponType {Light, Medium, Heavy}
    public WeaponType type;
    public bool isAbleToBreakDoors = false;

    LayerMask playerMask;
    Guard guard;

    [Header("Value & Objective")]
    public int itemWorth = 0;
    public bool isObjective = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        playerMask = LayerMask.GetMask("Player");
    }

    void Update()
    {
        Outlining();
    }

    void Outlining()
    {
        if (outline != null)
        {
            if (Physics.CheckSphere(transform.position, 5f, playerMask))
            {
                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.gameObject.tag == "enemy")
        {
            guard = collision.gameObject.GetComponent<Guard>();

            if (collision.relativeVelocity.magnitude > maxForceToDMG && type == WeaponType.Medium)
            {
                guard.Die();
            }
            else if (collision.relativeVelocity.magnitude > minForceToDMG)
            {
                if (type == WeaponType.Heavy)
                {
                    guard.Die();
                }
                else if (type == WeaponType.Medium)
                {
                    guard.Slowdown(3f);
                }
                else if (type == WeaponType.Light)
                {
                    guard.Slowdown(3f);
                }            
            }
        }

        //Impact audio & alerting guards
        if (impactSound != null)
        {
            if (Time.time >= lastTimeImpacted + delayBetweenImpacts)
            {
                lastTimeImpacted = Time.time;

                //Get position
                ContactPoint contact = collision.contacts[0];

                //Play audio at position
                AudioSource.PlayClipAtPoint(impactSound, contact.point);

                //Check if guards are near and alert them
                GameObject[] guards = GameObject.FindGameObjectsWithTag("enemy");
                foreach(GameObject guard in guards)
                {
                    //Get distance
                    float distance = Vector3.Distance(guard.transform.position, transform.position);
                    if (distance <= alertRadius)
                    {
                        Debug.Log("Guard heard that!");
                        guard.GetComponent<Guard>().CheckSoundAtPlace(transform.position);
                    }
                }
            }           
        }

        if (collision.gameObject.tag == "BreakableDoor" && isAbleToBreakDoors)
        {
            collision.rigidbody.constraints = RigidbodyConstraints.None;
            collision.collider.material = null;
        }        
    }

    public float MoveSpeedMultiplier()
    {
        if (type == Item.WeaponType.Heavy) return 0.6f;
        else if (type == Item.WeaponType.Medium) return 0.8f;
        else return 1;
    }
}

