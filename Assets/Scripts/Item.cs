using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Item : MonoBehaviour
{
    public float minForceToDMG = 10f;
    public float maxForceToDMG = 20f;
    Guard guard;
    public enum WeaponType {Light, Medium, Heavy}
    public WeaponType type;
    void Start()
    {

    }

    void Update()
    {
        
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
}

