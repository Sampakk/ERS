using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class Hiace : MonoBehaviour
{
    Item item;
    public int currentScore = 0;
    public bool objectiveDone = false;

    public bool atDoor = false;

    HUD hud;
    // Start is called before the first frame update
    void Start()
    {
        hud = FindObjectOfType<HUD>();
    }
    // Update is called once per frame
    void Update()
    {
        if (objectiveDone && Input.GetKeyDown(KeyCode.E) && atDoor)
        {
            EditorSceneManager.LoadScene(0);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Objects"))
        {
            Destroy(other.gameObject);

            item = other.gameObject.GetComponent<Item>();
            currentScore += item.itemWorth;

            if (item.isObjective) objectiveDone = true;
        }
        if ( other.tag == "Player")
        {
            atDoor = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") atDoor = false;
    }
}
