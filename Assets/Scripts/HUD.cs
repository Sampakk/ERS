using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    Interaction interaction;

    public TextMeshProUGUI interactionText;
    public Image throwbar;

    // Start is called before the first frame update
    void Start()
    {
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
        if (interaction.HasItemInHands())
        {
            interactionText.text = "Hold LMB to throw, press RMB to drop";
        }
        else
        {
            if (interaction.IsLookingObject())
            {
                interactionText.text = "Press LMB to pick up item";
            }
            else
            {
                interactionText.text = "";
            }
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
