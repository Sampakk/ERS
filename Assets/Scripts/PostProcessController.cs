using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessController : MonoBehaviour
{
    Player player;

    PostProcessVolume volume;
    Vignette vignette;

    float intensity;

    // Start is called before the first frame update
    void Start()
    {
        //Get player
        player = GetComponentInParent<Player>();

        //Try to get postprocessing
        volume = FindObjectOfType<PostProcessVolume>();
        if (volume != null)
        {
            //Get vignette from volume
            PostProcessProfile profile = volume.profile;

            vignette = profile.GetSetting<Vignette>();
            intensity = vignette.intensity;

            //Make vignette clear
            vignette.intensity.Override(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleVignette();
    }

    void HandleVignette()
    {
        if (player.IsCrouched())
        {
            float value = Mathf.MoveTowards(vignette.intensity, intensity, Time.deltaTime);
            vignette.intensity.Override(value);
        }
        else
        {
            float value = Mathf.MoveTowards(vignette.intensity, 0, Time.deltaTime);
            vignette.intensity.Override(value);
        }
    }
}
