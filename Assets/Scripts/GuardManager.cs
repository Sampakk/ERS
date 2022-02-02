using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardManager : MonoBehaviour
{
    public int maxGuards = 3;
    int guardsAlive;

    [Header("Map Positions")]
    public Transform spawnpoints;
    public Transform waypoints;
    Transform[] allWaypoints;

    // Start is called before the first frame update
    void Start()
    {
        allWaypoints = new Transform[waypoints.childCount];
        for (int i = 0; i < waypoints.childCount; i++)
        {
            allWaypoints[i] = waypoints.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetRandomWaypoint()
    {
        Vector3 waypointPos = new Vector3();

        int random = Random.Range(0, allWaypoints.Length);
        waypointPos = allWaypoints[random].position;

        return waypointPos;
    }
}
