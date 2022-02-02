using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float minForceToDMG = 10f;
    public float maxForceToDMG = 20f;
    public float maxDMG = 50f;
    public float minDMG = 20f;
    Guard hp;
    void Start()
    {

    }

    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        //Dont do this if player


        if (collision.gameObject.tag == "enemy")
        {
            hp = collision.gameObject.GetComponent<Guard>();

            if (collision.relativeVelocity.magnitude > maxForceToDMG)
            {
                hp.health -= maxDMG;
                Debug.Log("Took a lot of damage");
            }
            else if (collision.relativeVelocity.magnitude > minForceToDMG)
            {
                hp.health -= minDMG;
                Debug.Log("Took some damage");
            }
        }
    }
}

