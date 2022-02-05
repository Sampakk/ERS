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
    Color normalLightColor;

    // Start is called before the first frame update
    void Start()
    {
        startEulers = pivotPoint.localEulerAngles;
        normalLightColor = cameraLight.color;
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotation();

        CameraLights();
    }

    void CameraRotation()
    {
        //Rotating camera when has target
        if (target != null)
        {
            //Camera looks towards player position
            Vector3 direction = target.position - pivotPoint.position;
            pivotPoint.localRotation = Quaternion.LookRotation(direction, pivotPoint.up);

            //Clamp rotation so its limited by angle
            float cameraAngle = pivotPoint.localEulerAngles.y;
            float clampedAngle = ClampAngle(cameraAngle, -(angle / 2), angle / 2);

            //Apply only rotation on y axis
            Vector3 clampedEulers = new Vector3(0, clampedAngle, 0);
            pivotPoint.localRotation = Quaternion.Euler(clampedEulers);

            return;
        }

        //Rotating camera when no target
        Vector3 localEulers = startEulers;
        localEulers.y += Mathf.Sin(Time.time * speed) * (angle / 2);

        Quaternion rotation = Quaternion.Euler(localEulers);
        pivotPoint.localRotation = rotation;
    }

    void CameraLights()
    {
        if (target == null)
        {
            if (cameraLight.color != normalLightColor)
                cameraLight.color = normalLightColor;
        }
        else
        {
            if (cameraLight.color == normalLightColor)
                cameraLight.color = Color.red;
        }
    }

    public void SetTarget(Transform target)
    {
        //Toggle target
        if (this.target != target || target == null)
            this.target = target;

        //If target is player, alert guards
        if (target != null)
        {
            foreach(Guard guard in FindObjectsOfType<Guard>())
            {
                guard.SetTargetDestination(target.position);
            }
        }
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }
}
