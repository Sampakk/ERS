using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    Camera cam;

    Player player;
    public Transform hands;
    public LayerMask Objects;
    Transform item;
    Rigidbody itemrb;
    Collider itemcol;

    float chargeTimer = 0f;
    float chargeTimeMax = 1f;
    public float throwForce = 25f;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        cam = GetComponentInChildren<Camera>();

        if (itemcol == null) 
            itemcol = GetComponentInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //Pick up item
        if (Input.GetMouseButtonDown(0))
        {
            if (IsLookingObject())
            {
                PickItemUp();
            }        
        } 
        else
        {
            if (HasItemInHands())
            {
                //Drop item
                if (Input.GetMouseButtonDown(1))
                {
                    DropItem();
                }

                //Charge timer starts
                if (Input.GetMouseButton(0))
                {
                    chargeTimer += Time.deltaTime;
                    if (chargeTimer >= chargeTimeMax)
                    {
                        chargeTimer = chargeTimeMax;
                    }
                }

                //Throws with the force of the timer
                if (Input.GetMouseButtonUp(0))
                {
                    if (chargeTimer > 0.2f && player.currentStamina > 15f) Throw();
                    else chargeTimer = 0;
                }
            }
        }
    }

    public bool IsLookingObject()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2f, Objects) && hands.transform.childCount == 0)
        {
            item = hit.transform;
            itemrb = hit.rigidbody;
            itemcol = hit.collider;
            return true;
        }

        return false;
    }

    public bool HasItemInHands()
    {
        if (hands.transform.childCount > 0)
            return true;

        return false;
    }

    public void PickItemUp()
    {
        //itemrb.isKinematic = false;

        //Put object in correct position in "hands"
        item.parent = hands;
        item.localPosition = Vector3.zero;

        //this one stops the momentum so it doesnt float away from your hands
        itemrb.velocity = Vector3.zero;
        itemrb.angularVelocity = Vector3.zero;
        
        //These ones freeze and disable collider once you pick up the object
        itemcol.enabled = false;
        itemrb.useGravity = false;
        itemrb.freezeRotation = true;
        itemrb.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void DropItem()
    {
        //Disable parent
        item.parent = null;

        //Enable collider
        itemcol.enabled = true;

        //Activate rigidbody
        itemrb.useGravity = true;
        itemrb.freezeRotation = false;
        itemrb.constraints = RigidbodyConstraints.None;
    }

    void Throw()
    {
        //Get direction and torgue
        Vector3 throwDir = hands.forward;
        Vector3 throwTorgue = new Vector3(Random.Range(-1f, 1f), Random.Range(-2f, 2f), Random.Range(-1f, 1f));

        //Throw item and reset charging
        ThrowItem(throwDir, throwTorgue, throwForce * GetThrowMult());
        chargeTimer = 0;
        player.currentStamina -= 15f;
    }

    public float GetThrowMult()
    {
        return chargeTimer / chargeTimeMax;
    }

    void ThrowItem(Vector3 direction, Vector3 torgue, float throwForce)
    {
        DropItem();

        itemrb.AddForce(direction * throwForce, ForceMode.Impulse);
        itemrb.AddTorque(torgue, ForceMode.Impulse);
    }

    public Item GetItemInHands()
    {
        if (HasItemInHands())
        {
            return GetComponentInChildren<Item>();
        }

        return null;
    }
}
