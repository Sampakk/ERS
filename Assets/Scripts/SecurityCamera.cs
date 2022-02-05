using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Stats")]
    public Transform pivotPoint;
    public float angle = 90;
    public float speed = 0.5f;

    [Header("Effects")]
    public Light cameraLight;
    public MeshRenderer viewCone;

    Transform target;
    Vector3 startEulers;

    // Start is called before the first frame update
    void Start()
    {
        startEulers = pivotPoint.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        //Rotating camera when has target
        if (target != null)
        {
            pivotPoint.LookAt(target);

            return;
        }

        //Rotating camera when no target
        Vector3 localEulers = startEulers;
        localEulers.y += Mathf.Sin(Time.time * speed) * (angle / 2);

        Quaternion rotation = Quaternion.Euler(localEulers);
        pivotPoint.localRotation = rotation;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
