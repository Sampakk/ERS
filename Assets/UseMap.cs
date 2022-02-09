using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMap : MonoBehaviour
{
    public GameObject player;
    public Transform usePosition;
    public Transform cam;
    bool useable = false;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (useable && Input.GetKeyDown(KeyCode.E))
        {
            UseGameMap();
        }
        Debug.Log(useable);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") useable = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") useable = false;
    }
    void UseGameMap()
    {
        player.transform.parent = usePosition;
        player.transform.localPosition = Vector3.zero;
        player.GetComponent<Player>().enabled = false;

        //Unlock cursor
        Cursor.lockState = CursorLockMode.None;

        player.transform.LookAt(gameObject.transform.position);
        cam.localRotation = Quaternion.Euler(-5f,0f,0f);
    }
    public void ExitGameMap()
    {
        player.transform.parent = null;
        player.GetComponent<Player>().enabled = true;

        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
}
    