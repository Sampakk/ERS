using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Stats")]
    public Transform pivotPoint;
    public float angle = 90;
    public float speed = 0.5f;


    Transform target;
    Vector3 startEulers;
    Color normalLightColor;

    // Start is called before the first frame update
    void Start()
    {
        //Setup values
        startEulers = pivotPoint.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotation();
    }

    void CameraRotation()
    {
        //Rotating camera when has target
        if (target != null)
        {
            //Camera looks towards player position
            Vector3 direction = target.position - pivotPoint.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction, pivotPoint.up);

            //Clamp rotation so its limited by angle
            float cameraAngle = lookRotation.eulerAngles.y;
            float clampedAngle = ClampAngle(cameraAngle, -(angle / 2), angle / 2);

            //Apply rotation only on y axis and then slerp to target rotation
            Vector3 clampedEulers = new Vector3(0, clampedAngle, 0);
            Quaternion rotation = Quaternion.Euler(clampedEulers);
            pivotPoint.localRotation = Quaternion.Slerp(pivotPoint.localRotation, rotation, 2f * Time.deltaTime);
        }
        else
        {
            //Add rotation
            Vector3 localEulers = startEulers;
            localEulers.y += Mathf.Sin(Time.time * speed) * (angle / 2);

            //Slerp to target rotation
            Quaternion rotation = Quaternion.Euler(localEulers);
            pivotPoint.localRotation = Quaternion.Slerp(pivotPoint.localRotation, rotation, 2f * Time.deltaTime);
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
