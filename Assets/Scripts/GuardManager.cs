using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GuardManager : MonoBehaviour
{
    public GameObject guardPrefab;

    [Header("Paths")]
    public Transform waypoints;
    public Transform[] paths;

    List<Guard> guards = new List<Guard>();

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            //Instantiate guards to scene on start, each at the start of their own path
            for (int i = 0; i < paths.Length; i++)
            {
                Transform path = paths[i];
                SpawnGuardOnPath(path);
            }
        }
    }

    void SpawnGuardOnPath(Transform path)
    {
        GameObject guard = Instantiate(guardPrefab, path.GetChild(0).position, Quaternion.identity);
        guard.GetComponent<Guard>().SetupPath(path);

        //Add to list
        guards.Add(guard.GetComponent<Guard>());
    }

    public Transform GetRandomWaypoint()
    {
        Transform waypoint = null;

        int randomWaypoint = Random.Range(0, waypoints.childCount);
        waypoint = waypoints.GetChild(randomWaypoint);

        return waypoint;
    }

    public bool IsPlayerChased()
    {
        bool chased = false;
        foreach(Guard guard in guards)
        {
            if (guard.IsChasing())
                chased = true;
        }

        return chased;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        foreach (Transform path in paths)
        {
            Transform lastPoint = null;
            foreach (Transform waypoint in path)
            {
                if (lastPoint != null)
                {
                    Gizmos.DrawLine(lastPoint.position, waypoint.position);
                }

                lastPoint = waypoint;
            }
        }
    }
}
