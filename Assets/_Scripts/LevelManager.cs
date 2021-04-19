using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : NetworkBehaviour
{
    public List<GameObject> pointOrbs = new List<GameObject>();
    public bool isStarted = false;
    public int globalScore = 0;

    [Header ("Prefabs")]
    public GameObject pointOrb;

    [Header ("X Spawn Range")]
    public float xMin;
    public float xMax;

    // the range of y
    [Header ("Y Spawn Range")]
    public float yMin;
    public float yMax;

    [ClientRpc]
    public void StartGame()
    {

        isStarted = true;
        ScoreChanged(0);
    }

    public void FixedUpdate()
    {
        if(isStarted)
        {

        }
    }

    public void ScoreChanged(int score)
    {

        globalScore = score;

        if(globalScore < 5)
        {
            Invoke("SpawnPointOrb", 1.2f);
        }
        else if (globalScore < 10)
        {
            Invoke("SpawnPointOrb", 1.2f);
            if(pointOrbs.Count < 3 && Random.Range(0, 100) < 20)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }

        foreach(GameObject pointOrb in pointOrbs)
        {
            if(!pointOrb.activeSelf)
            {
                pointOrbs.Remove(pointOrb);
                Destroy(pointOrb);

                break;
            }
        }

    }
    
    public void SpawnPointOrb()
    {
        Vector3 pos = new Vector3 (Random.Range (xMin, xMax), Random.Range (yMin, yMax), -2.13236f);
        GameObject orb = (GameObject)Instantiate(pointOrb, pos,  Quaternion.identity);
        pointOrbs.Add(orb );
        NetworkServer.Spawn(orb);

    }
}
