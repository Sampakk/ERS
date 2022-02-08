using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    Interaction interaction;

    public Image  interactIcon;
    public Image throwbar;
    public Image Crosshair;

    // Start is called before the first frame update
    void Start()
    {
        interactIcon.enabled = false;

        interaction = FindObjectOfType<Interaction>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInteraction();

        HandleThrowbar();
    }

    void HandleInteraction()
    {
       
            if (interaction.IsLookingObject())
            {
            Crosshair.enabled = false;
            interactIcon.enabled = true;
        }
            else
            {
            interactIcon.enabled = false;
            Crosshair.enabled = true;
           
        }
        
    }

    void HandleThrowbar()
    {
        if (interaction.HasItemInHands())
        {
            throwbar.fillAmount = interaction.GetThrowMult();
        }
        else
        {
            throwbar.fillAmount = 0;
        }
    }
}
