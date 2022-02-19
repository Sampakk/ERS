using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GuardManager : MonoBehaviour
{
    public GameObject guardPrefab;

    [Header("Troops")]
    public Transform troopsSpawnpoint;
    public int troopsCount = 1;

    [Header("Paths & Waypoints")]
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
        //Instantiate
        GameObject guard = Instantiate(guardPrefab, path.GetChild(0).position, Quaternion.identity);
        guard.GetComponent<Guard>().SetupPath(path);

        //Add to list
        guards.Add(guard.GetComponent<Guard>());
    }

    public void StartSpawningTroops()
    {
        StartCoroutine(SpawnTroops());
    }

    IEnumerator SpawnTroops()
    {
        int count = 0;
        while (count < troopsCount)
        {
            //Instantiate
            GameObject guard = Instantiate(guardPrefab, troopsSpawnpoint.position, Quaternion.identity);

            //Add to list
            guards.Add(guard.GetComponent<Guard>());

            count++;
            yield return new WaitForSeconds(1f);
        }
    }

    public Transform GetRandomWaypoint()
    {
        Transform waypoint = null;

        //Get random waypoint
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
