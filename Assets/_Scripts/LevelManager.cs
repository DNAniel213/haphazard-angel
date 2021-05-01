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
    public GameObject prefab_pointOrb;
    public GameObject prefab_rock;
    public GameObject prefab_spikyball;
    public GameObject prefab_spikyballUp;

    [Header ("X Spawn Range")]
    public float xMin;
    public float xMax;

    // the range of y
    [Header ("Y Spawn Range")]
    public float yMin;
    public float yMax;

    [Header("Difficulty")]
    public Difficulty difficulty;
    public int tutorialPts, easyPts, medPts, hardPts, hellPts;


    public void StartGame()
    {
        difficulty = Difficulty.TUTORIAL;
        isStarted = true;
        RpcScoreChanged(0);
    }

    public void FixedUpdate()
    {
        if(isStarted)
        {

        }
    }
    public void RpcScoreChanged(int score)
    {
        Debug.Log("RpcScoreChanged " + score);

        globalScore = score;

        if(globalScore < tutorialPts)
        {
            difficulty = Difficulty.TUTORIAL;
            Invoke("SpawnPointOrb", 1.2f);
        }
        else if (globalScore < easyPts)
        {
            if(difficulty != Difficulty.EASY)
            {
                difficulty = Difficulty.EASY;
                StartCoroutine(RandomSelfInvoke(Difficulty.EASY, 50, 5, 8, "SpawnRock"));
            }

            Invoke("SpawnPointOrb", 1.2f);
            if(pointOrbs.Count < 3 && Random.Range(0, 100) < 20)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }
        else if (globalScore < medPts)
        {
            if(difficulty != Difficulty.MEDIOCRE)
            {
                difficulty = Difficulty.MEDIOCRE;
                StartCoroutine(RandomSelfInvoke(Difficulty.MEDIOCRE, 80, 7, 10, "SpawnRock"));
                StartCoroutine(RandomSelfInvoke(Difficulty.MEDIOCRE, 30, 9, 20, "SpawnSpikyBall"));
            }

            Invoke("SpawnPointOrb", 1.2f);
            if(pointOrbs.Count < 3 && Random.Range(0, 100) < 20)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }
        else if (globalScore < hardPts)
        {
            if(difficulty != Difficulty.HARD)
            {
                difficulty = Difficulty.HARD;
                StartCoroutine(RandomSelfInvoke(Difficulty.HARD, 100, 1, 10, "SpawnRock"));
                StartCoroutine(RandomSelfInvoke(Difficulty.HARD, 100, 7, 20, "SpawnSpikyBall"));
            }

            Invoke("SpawnPointOrb", 1.2f);
            if(pointOrbs.Count < 3 && Random.Range(0, 100) < 40)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }
        else if (globalScore < hellPts)
        {

        }

        foreach(GameObject pointOrbx in pointOrbs)
        {
            if(!pointOrbx.activeSelf)
            {
                pointOrbs.Remove(pointOrbx);
                Destroy(pointOrbx);

                break;
            }
        }

    }

    public IEnumerator RandomSelfInvoke(Difficulty diffReq, float chance, float minDelay, float maxDelay, string func)
    {
        Debug.Log("Random Self Invoke for " + func);
        if(this.difficulty == diffReq)
        {
            if(Random.Range(0, 100) < chance)
            {
                Invoke(func, 1f);
            }
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            StartCoroutine(RandomSelfInvoke(diffReq, chance, minDelay,maxDelay, func));
        }
    }

    public void SpawnRock()
    {
        Debug.Log("Spawning Rock!");

        Vector3 pos = new Vector3 (Random.Range (xMin, xMax), 6, -2.13236f);
        GameObject rock = (GameObject)Instantiate(prefab_rock, pos,  Quaternion.identity);
        NetworkServer.Spawn(rock);
    }
    public void SpawnSpikyBall()
    {
        Debug.Log("Spawning Spiky Ball!");
        bool direction = Random.Range(0, 100) < 25 ? false : true;
        Vector3 pos = new Vector3 (Random.Range (xMin, xMax), direction ? 6 : -6, -2.13236f);
        GameObject spike = null;

        if(direction)
            spike = (GameObject)Instantiate(prefab_spikyball, pos,  Quaternion.identity);
        else
            spike = (GameObject)Instantiate(prefab_spikyballUp, pos,  Quaternion.identity);
        
        NetworkServer.Spawn(spike);

    }
    
    public void SpawnPointOrb()
    {
        Debug.Log("Spawning Point Orb!");

        Vector3 pos = new Vector3 (Random.Range (xMin, xMax), Random.Range (yMin, yMax), -2.13236f);
        GameObject orb = (GameObject)Instantiate(prefab_pointOrb, pos,  Quaternion.identity);
        pointOrbs.Add(orb );
        NetworkServer.Spawn(orb);

    }
}
