using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float minForceToDMG = 10f;
    public float maxForceToDMG = 20f;
    public float maxDMG = 50f;
    public float minDMG = 20f;
    TempHealth hp;
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
            Debug.Log("Took damage :D");
            hp = collision.gameObject.GetComponent<TempHealth>();

            if (collision.relativeVelocity.magnitude > maxForceToDMG)
            {
                hp.health -= maxDMG;
            }
            else if (collision.relativeVelocity.magnitude > minForceToDMG)
            {
                hp.health -= minDMG;
            }
        }
        Debug.Log(collision.relativeVelocity.magnitude);
    }

}

