using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    Camera cam;
    Player player;

    public float fovKickAmount = 5f;
    float normalFov;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
