using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    Player player;
    Interaction interaction;
    GuardManager guardManager;
    UseMap useMap;
    Hiace hiace;

    public TextMeshProUGUI statusText;
    public Image  interactIcon;
    public Image throwbar;
    public Image Crosshair;
    public TextMeshProUGUI useMapText;
    public TextMeshProUGUI score;
    public Toggle objective1Toggle;
    public Toggle objective2Toggle;
    public TextMeshProUGUI objectiveScoreText;
    public int objectiveScore = 10;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        player = FindObjectOfType<Player>();
        interaction = FindObjectOfType<Interaction>();
        guardManager = FindObjectOfType<GuardManager>();
        useMap = FindObjectOfType<UseMap>();
        hiace = FindObjectOfType<Hiace>();

        interactIcon.enabled = false;
        useMapText.enabled = false;

        SetStatusText(0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInteraction();
        
        HandleMap();
        
        HandleThrowbar();

        HandleStatus();

        AddScore();

        Objective();
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

    void HandleStatus()
    {

        if (guardManager == null)
            return;

        if (guardManager.IsPlayerChased())
        {
            SetStatusText(2);
        }
        else
        {
            if (statusText.text.Contains("chasing"))
                SetStatusText(0);
        }
    }

    public void SetStatusText(int status)
    {
        if (status == 0)
        {
            statusText.text = "Unnoticed";
            statusText.color = Color.white;
        }          
        else if (status == 1)
        {
            statusText.text = "Guards alerted!";
            statusText.color = Color.yellow;
        }
        else if (status == 2)
        {
            statusText.text = "Guards chasing!";
            statusText.color = Color.red;
        }
    }

    void HandleMap()
    {
        if (useMap == null)
            return;

        if (useMap.AtBoard())
        {
            Crosshair.enabled = false;
            useMapText.enabled = true;
        }
        else
        {
            useMapText.enabled = false;
            Crosshair.enabled = true;
        }
    }

    void AddScore()
    {
        if (hiace != null)
            score.text = hiace.currentScore.ToString() + "€";
    }

    void Objective()
    {
        if (hiace != null)
        {
            if (hiace.objectiveDone)
            {
                objective1Toggle.isOn = true;
            }

            objectiveScoreText.text = "Collect at least " + objectiveScore + "€ Worth Of items!";

            if (hiace.currentScore >= objectiveScore)
            {
                objective2Toggle.isOn = true;
            }
        }
    }
}
