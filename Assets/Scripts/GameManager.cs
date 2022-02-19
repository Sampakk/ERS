using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    AudioSource audioSrc;

    public bool alerted;
    public float timeBeforeCopsArrive = 300f;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        audioSrc.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        HandleSiren();
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
}
