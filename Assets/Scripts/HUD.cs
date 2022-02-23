using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    Player player;
    Interaction interaction;
    GuardManager guardManager;
    UseMap useMap;
    Hiace hiace;

    public TextMeshProUGUI score;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI HpText;

    [Header("Interaction")]
    public TextMeshProUGUI statusText;
    public Image  interactIcon;
    public Image throwbar;
    public Image Crosshair;
    public TextMeshProUGUI useText;
    
    [Header("Objectives")]
    public Toggle objective1Toggle;
    public Toggle objective2Toggle;
    public TextMeshProUGUI objective2Text;
    public TextMeshProUGUI objective1Text;
    public int objectiveScore = 10;

    [Header("Player Icon")]
    public Image playerIcon;
    public Sprite standIcon;
    public Sprite crouchIcon;
    public Sprite runIcon;

    [Header("Root Windows")]
    public GameObject inGameRoot;
    public GameObject winRoot;
    public GameObject menuRoot;
    public static bool gameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        player = FindObjectOfType<Player>();
        interaction = FindObjectOfType<Interaction>();
        guardManager = FindObjectOfType<GuardManager>();
        useMap = FindObjectOfType<UseMap>();
        hiace = FindObjectOfType<Hiace>();

        //Setup roots
        inGameRoot.SetActive(true);
        winRoot.SetActive(false);
        menuRoot.SetActive(false);

        //Setup
        interactIcon.enabled = false;
        useText.enabled = false;

        SetStatusText(0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInteraction();

        HandleUseButton();

        HandleThrowbar();

        HandleStatus();

        HandleIcon();

        AddScore();

        Objective();

        HandleHP();

        if(Input.GetKeyDown(KeyCode.Escape) && !winRoot.activeSelf)
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
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

    void HandleUseButton()
    {
        if (useMap != null)
        {
            if (useMap.AtBoard())
            {
                Crosshair.enabled = false;
                useText.enabled = true;
                useText.text = "Press 'E' To Use Map";
            }
            else
            {
                useText.enabled = false;
                Crosshair.enabled = true;
            }
        }
        if (hiace != null)
        {
            if (hiace.atDoor)
            {
                useText.enabled = true;
                useText.text = "Press 'E' To Escape";
            }
            else useText.enabled = false;
        }
    }

    void HandleIcon()
    {
        if (player.IsCrouched())
        {
            playerIcon.sprite = crouchIcon;
        }
        else if (player.IsRunning())
        {
            playerIcon.sprite = runIcon;
        }
        else
        {
            playerIcon.sprite = standIcon;
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
                objective1Text.text = "Escape with your Hiace";
            }

            objective2Text.text = "Collect at least " + objectiveScore + "€ Worth Of items!";

            if (hiace.currentScore >= objectiveScore)
            {
                objective2Toggle.isOn = true;
            }
        }
    }

    public void Timer(float time)
    {
        timerText.text = "Time Before The Cops Arrive: " + time.ToString("f0");
    }

    void HandleHP()
    {
        HpText.text = "Health: " + player.currentHP.ToString("f0");
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        //Disable menu
        inGameRoot.SetActive(true);
        menuRoot.SetActive(false);
        gameIsPaused = false;

        //Hide & lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        Time.timeScale = 0f;

        //Enable menu
        inGameRoot.SetActive(false);
        menuRoot.SetActive(true);   
        gameIsPaused = true;

        //Show & unlock cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowWinScreen()
    {
        //Enable win root
        inGameRoot.SetActive(false);
        menuRoot.SetActive(false);
        winRoot.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
