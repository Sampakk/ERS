using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    Camera cam;
    Player player;

    public float headbobAmount = 0.12f;
    public float headbobSpeed = 12f;

    public float fovKickAmount = 6f;
    float normalFov;

    Vector3 normalPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
        cam = GetComponentInChildren<Camera>();

        normalFov = cam.fieldOfView;
        normalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        FovKicking();

        HeadBobbing();
    }

    void FovKicking()
    {
        //Get target fov amount
        float fovTarget = (player.IsRunning()) ? normalFov + fovKickAmount : normalFov;

        //Lerp fov to target
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovTarget, 8f * Time.deltaTime);
    }

    void HeadBobbing()
    {
        //Add groundcheck to headbobbing - no getter in player.cs !!!!
        if (player.IsRunning())
        {
            float offset = Mathf.Sin(Time.time * headbobSpeed) * headbobAmount;

            Vector3 targetPos = new Vector3(0, normalPos.y + offset, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 10f * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, normalPos, 10f * Time.deltaTime);
        }
    }
}
