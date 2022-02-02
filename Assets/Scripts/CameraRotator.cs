using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{

    public GameObject target;

    //Kametan k‰‰nnˆs rajoitukset ja nopeus
    public float angle = 90;
    public float speed = 0.5f;
    //Tallennettu rotatio
    Vector3 startEulers;


    void Start()
    {
        //Talletetaan starttiin objectin aloitus rotatio startissa
        startEulers = transform.localEulerAngles;

    }
    // Update is called once per frame
    void Update()
    {
        //Tallennetaan joka freimille v‰liaikaisesti referenssi alkuper‰isest‰ rotatiosta
        Vector3 localEulers = startEulers;

        //lis‰t‰‰n alkuper‰iseen rotatioon offsetti‰ haluttum‰‰r‰
        //Muokataan Mathf.Sin Y vectoria. Time.time (aika joka alkaa kun scene alkaa) * nopeus.
        //angle pit‰‰ jakaa 2, koska Mathf.sin antaa arvon -1 ja 1 v‰lill‰. Jos anglea ei jaa 2 niin 90 astetta olisikin todellisuudessa 180.
        localEulers.y += Mathf.Sin(Time.time * speed) * (angle / 2);

        //localEulerssista tehd‰‰n quoternion
        Quaternion rotation = Quaternion.Euler(localEulers);
        transform.rotation = rotation;
        //transform.rotation = Quaternion.Euler(localEulers);

    }
}
