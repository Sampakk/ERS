using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hiace : MonoBehaviour
{
    Item item;
    public int currentScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Objects"))
        {
            Destroy(other.gameObject);

            item = other.gameObject.GetComponent<Item>();
            currentScore += item.itemWorth;
        }
    }
}
