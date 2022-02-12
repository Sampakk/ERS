using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCameras : MonoBehaviour
{
    GameObject player;
    GameObject[] cameraGameObject;
    GameObject[] cameraConeGameObject;
    bool useable = false;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraGameObject = GameObject.FindGameObjectsWithTag("SecurityCamera");
        cameraConeGameObject = GameObject.FindGameObjectsWithTag("ViewCone");

    }

    // Update is called once per frame
    void Update()
    {

        if (useable && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("DisableCameras");
            disableCameras();
        }


        

    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player") useable = true;
    }
    void OnTriggerExit(Collider other)
    {
      
        if (other.tag == "Player") useable = false;
    }




    private void disableCameras()
    {
        for (int i = 0; i < cameraGameObject.Length; i++)
        {
            cameraGameObject[i].GetComponent<SecurityCamera>().enabled = false;
            cameraConeGameObject[i].SetActive(false);
        }
    }
}
