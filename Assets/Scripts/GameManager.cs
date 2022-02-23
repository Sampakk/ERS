using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    AudioSource audioSrc;
    GuardManager guardManager;
    HUD hud;

    public bool alerted;
    public float timeBeforeCopsArrive = 300f;

    [Header("Musics")]
    public AudioSource music;
    public AudioClip stealthMusic;
    public AudioClip chaseMusic;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        hud = FindObjectOfType<HUD>();
        guardManager = FindObjectOfType<GuardManager>();

        //Stop siren
        audioSrc = GetComponent<AudioSource>();
        audioSrc.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        HandleSiren();

        HandleTimer();

        HandleMusic();
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
        if (alerted)
        {
            timeBeforeCopsArrive -= Time.deltaTime;
            
            if (hud != null) hud.Timer(timeBeforeCopsArrive);
            else return;

            if (timeBeforeCopsArrive <= 0)
            {
                timeBeforeCopsArrive = 0;
                alerted = false;
                GameOver();
            }
        }
    }

    void HandleMusic()
    {
        if (guardManager.IsPlayerChased())
        {
            if (music.isPlaying)
            {
                if (music.clip != chaseMusic)
                {
                    music.Stop();
                    music.clip = chaseMusic;
                    music.Play();
                }
            }
        }
        else
        {
            if (music.isPlaying)
            {
                if (music.clip != stealthMusic)
                {
                    music.Stop();
                    music.clip = stealthMusic;
                    music.Play();
                }
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
