using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : NetworkBehaviour
{
    public WingControl angel = null;
    public List<GameObject> pointOrbs = new List<GameObject>();
    public bool isStarted = false;
    public int globalScore = 0;
    private NetworkMatchChecker networkMatchChecker = null;

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


    private void Awake() {
        //StartGame();
    }

    private void Start() {
        networkMatchChecker = this.GetComponent<NetworkMatchChecker>();

        if(NetworkPlayer.localPlayer != null)
        {
            NetworkPlayer.localPlayer.StartNetworkGame();
        }

        foreach(NetworkPlayer playerobj in angel.players)
        {
            playerobj.levelManager = this;
        }
    }

    [Server]
    public void StartGame()
    {
        difficulty = Difficulty.TUTORIAL;
        RpcScoreChanged();
        isStarted = true;

        xMin += angel.transform.position.x;
        xMax += angel.transform.position.x;


    }


    public void RpcScoreChanged()
    {
        int score = globalScore;

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
                StartCoroutine(RandomSelfInvoke(Difficulty.EASY, 30, 4, 12, "SpawnRock"));
            }

            Invoke("SpawnPointOrb", 1.2f);
            if(Random.Range(0, 100) < 2)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }
        else if (globalScore < medPts)
        {
            if(difficulty != Difficulty.MEDIOCRE)
            {
                difficulty = Difficulty.MEDIOCRE;
                StartCoroutine(RandomSelfInvoke(Difficulty.MEDIOCRE, 50, 3, 13, "SpawnRock"));
                StartCoroutine(RandomSelfInvoke(Difficulty.MEDIOCRE, 30, 9, 20, "SpawnSpikyBall"));
            }

            Invoke("SpawnPointOrb", 1.2f);
            if(Random.Range(0, 100) < 5)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }
        else if (globalScore < hardPts)
        {
            if(difficulty != Difficulty.HARD)
            {
                difficulty = Difficulty.HARD;
                StartCoroutine(RandomSelfInvoke(Difficulty.HARD, 80, 2, 15, "SpawnRock"));
                StartCoroutine(RandomSelfInvoke(Difficulty.HARD, 50, 7, 17, "SpawnSpikyBall"));
            }

            Invoke("SpawnPointOrb", 1.2f);
            if(Random.Range(0, 100) < 8)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }
        else if (globalScore < hellPts)
        {
            if(difficulty != Difficulty.HELL)
            {
                difficulty = Difficulty.HELL;
                StartCoroutine(RandomSelfInvoke(Difficulty.HELL, 100, 2, 12, "SpawnRock"));
                StartCoroutine(RandomSelfInvoke(Difficulty.HELL, 80, 4, 12, "SpawnSpikyBall"));
                StartCoroutine(RandomSelfInvoke(Difficulty.HELL, 30, 9, 20, "SpawnGroundSpike"));
            }

            Invoke("SpawnPointOrb", 1.2f);
            if(Random.Range(0, 100) < 10)
            {
                Invoke("SpawnPointOrb", 2.5f);
            }
        }


        /*
        foreach(GameObject pointOrbx in pointOrbs)
        {
            if(pointOrbx == null)
            {
                //pointOrbs.Remove(pointOrbx);
            }
        }*/
    }


    public IEnumerator RandomSelfInvoke(Difficulty diffReq, float chance, float minDelay, float maxDelay, string func)
    {
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

        Vector3 pos = new Vector3 (Random.Range (xMin, xMax), 6, -2.13236f);
        GameObject rock = (GameObject)Instantiate(prefab_rock, pos,  Quaternion.identity);
        rock.GetComponent<NetworkMatchChecker>().matchId = networkMatchChecker.matchId;

        NetworkServer.Spawn(rock);
    }

    public void SpawnSpikyBall()
    {
        bool direction = Random.Range(0, 100) < 25 ? false : true;
        Vector3 pos = new Vector3 (Random.Range (xMin, xMax), direction ? 6 : -6, -2.13236f);
        GameObject spike = null;

        if(direction)
            spike = (GameObject)Instantiate(prefab_spikyball, pos,  Quaternion.identity);
        else
            spike = (GameObject)Instantiate(prefab_spikyballUp, pos,  Quaternion.identity);
        
        spike.GetComponent<NetworkMatchChecker>().matchId = networkMatchChecker.matchId;

        NetworkServer.Spawn(spike);

    }    
    
    public void SpawnGroundSpike()
    {


    }
    
    public void SpawnPointOrb()
    {

        Vector3 pos = new Vector3 (Random.Range (xMin, xMax), Random.Range (yMin, yMax), -2.13236f);
        GameObject orb = (GameObject)Instantiate(prefab_pointOrb, pos,  Quaternion.identity);
        //NetworkPlayer.localPlayer.SpawnObjectInNetwork(pos);
        orb.GetComponent<NetworkMatchChecker>().matchId = networkMatchChecker.matchId;
        pointOrbs.Add(orb);
        NetworkServer.Spawn(orb);

    }

    public void RemovePointOrb(GameObject orb)
    {
        pointOrbs.Remove(orb);
        NetworkServer.Destroy(orb);
        Destroy(orb);

    }
}
