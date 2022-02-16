using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Outline outline;

    public float minForceToDMG = 10f;
    public float maxForceToDMG = 20f;  
    public enum WeaponType {Light, Medium, Heavy}
    public WeaponType type;

    LayerMask playerMask;
    Guard guard;

    public int itemWorth = 0;

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
                Debug.Log("Tappo medium aseella");

                guard.Die();
            }
            else if (collision.relativeVelocity.magnitude > minForceToDMG)
            {
                if (type == WeaponType.Heavy)
                {
                    Debug.Log("Tappo heavy aseella");

                    guard.Die();
                }
                else if (type == WeaponType.Medium)
                {
                    Debug.Log("Hidastus medium aseella");

                    guard.Slowdown(3f);
                }
                else if (type == WeaponType.Light)
                {
                    Debug.Log("Hidastus light aseella");

                    guard.Slowdown(3f);
                }            
            }
        }
    }

    public float MoveSpeedMultiplier()
    {
        if (type == Item.WeaponType.Heavy) return 0.6f;
        else if (type == Item.WeaponType.Medium) return 0.8f;
        else return 1;
    }
}

