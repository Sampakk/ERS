using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    AudioSource audioSrc;
    HUD hud;

    public bool alerted;
    public float timeBeforeCopsArrive = 300f;

    public bool timerIsActive = false;

    // Start is called before the first frame update
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        audioSrc = GetComponent<AudioSource>();
        audioSrc.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        HandleSiren();

        HandleTimer();
    }

    void HandleSiren()
    {
        if (alerted)
        {
            if (!audioSrc.isPlaying)
            {
                audioSrc.Play();
                audioSrc.loop = true;
            }
        }
    }

    void HandleTimer()
    {
        if (alerted) timerIsActive = true;

        if (timerIsActive)
        {
            timeBeforeCopsArrive -= Time.deltaTime;
            
            if (hud != null) hud.Timer(timeBeforeCopsArrive);
            else return;

            

            if (timeBeforeCopsArrive <= 0)
            {
                timeBeforeCopsArrive = 0;
                timerIsActive = false;
                GameOver();
            }
        }
    }

    public void EscapeToHQ()
    {
        StartCoroutine(LoadToHQ(5f));
    }

    IEnumerator LoadToHQ(float delay)
    {
        Time.timeScale = 0;

        //Show win screen
        hud.ShowWinScreen();

        //Wait
        yield return new WaitForSecondsRealtime(delay);

        //Load to HQ
        SceneManager.LoadScene(1);
    }

    void GameOver()
    {
        SceneManager.LoadScene(0);
    }
}
