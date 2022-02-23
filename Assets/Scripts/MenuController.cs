using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text MouseSensTextValue;
    [SerializeField] private Slider MouseSensSlider;
    [SerializeField] private float defaultSens = 1f;
    [SerializeField] private Toggle BhopToggle = null;
    public float MouseSens = 1f;
    public static int Bhop = 0;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Levels To Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        //PlaceHolder for LoadGame from tutorial
        //Temp Logic but most save data Gets should be done here
        if(PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);

        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        MouseSens = sensitivity;
        MouseSensTextValue.text = sensitivity.ToString("0.0");
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSen", MouseSens);
        PlayerPrefs.SetInt("Bhop", Bhop);
        StartCoroutine(ConfirmationBox());
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string Menutype)
    {
        if (Menutype == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

    public void BindJumpToMouseWheel()
    {
        if (BhopToggle.isOn)
        {
            Bhop = 1;
           
        }
        else
        {
            Bhop = 0;
           
        }
    }
}
