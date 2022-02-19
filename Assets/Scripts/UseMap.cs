using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UseMap : MonoBehaviour
{
    public Transform usePosition;
    public Transform cam;
    bool useable = false;
    Rigidbody rb;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (useable && Input.GetKeyDown(KeyCode.E))
        {
            UseGameMap();
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
    void UseGameMap()
    {
        player.transform.parent = usePosition;
        player.transform.localPosition = Vector3.zero;
        player.GetComponent<Player>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        //Unlock cursor
        Cursor.lockState = CursorLockMode.None;

        player.transform.LookAt(gameObject.transform.position);
        cam.localRotation = Quaternion.Euler(-2f,0f,0f);
        
    }
    public void ExitGameMap()
    {
        player.transform.parent = null;
        player.GetComponent<Player>().enabled = true;
        rb.constraints = ~RigidbodyConstraints.FreezePosition;
        
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        player.transform.rotation = Quaternion.identity;
        player.transform.rotation = Quaternion.Euler(0f, 180, 0);
    }
    public bool AtBoard()
    {
        if (useable) return true;
        else return false;
    }
    public void SelectToolShop()
    {
        ExitGameMap();
        SceneManager.LoadScene(1);
    }
}
    