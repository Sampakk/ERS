using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    //Kametan käännös rajoitukset ja nopeus
    public float angle = 90;
    public float speed = 0.5f;

    //Tallennettu rotatio
    Vector3 startEulers;
    Transform target;

    void Start()
    {
        //Talletetaan starttiin objectin aloitus rotatio startissa
        startEulers = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) //Look towards target
        {
            transform.LookAt(target);

            return;
        }

        //Tallennetaan joka freimille väliaikaisesti referenssi alkuperäisestä rotatiosta
        Vector3 localEulers = startEulers;

        //lisätään alkuperäiseen rotatioon offsettiä haluttumäärä
        //Muokataan Mathf.Sin Y vectoria. Time.time (aika joka alkaa kun scene alkaa) * nopeus.
        //angle pitää jakaa 2, koska Mathf.sin antaa arvon -1 ja 1 välillä. Jos anglea ei jaa 2 niin 90 astetta olisikin todellisuudessa 180.
        localEulers.y += Mathf.Sin(Time.time * speed) * (angle / 2);

        //localEulerssista tehdään quoternion
        Quaternion rotation = Quaternion.Euler(localEulers);
        transform.localRotation = rotation;
        //transform.rotation = Quaternion.Euler(localEulers);
    }

    /*Sitte tänne alle if lauseilla pelaajan havainnointi ja Kameran valon värimuutos (aika katkaisu takaisin "normaali" tilaan.
     Jos kamera huomaa pelaajan se myös ilmoittaa vartijoille pelaajan paikan ja hälyttää ne kameran lokaatioon (tyhjä gameobjecti maassa).
    Kameran olisi tarkoitus myös seurata pelaajaa (lock on target)*/

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
