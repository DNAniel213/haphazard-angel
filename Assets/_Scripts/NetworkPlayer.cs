using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    
    public static NetworkPlayer localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public string playerName;
    [SyncVar] public int playerIndex;

    public Text scoreText;
    public Text nameText;

    NetworkMatchChecker networkMatchChecker;

    [SyncVar] public Match currentMatch;

    [SerializeField] GameObject playerLobbyUI;

    [SyncVar]
    public PlayerPosition pos;
    public WingControl angel = null;
    public WingTrigger wingTrigger = null;

    public NetworkStart gameManager = null;
    public LevelManager levelManager = null;
    public bool isInitialized = false;
    [SyncVar]
    public bool isAlive = true;
    [SyncVar]
    public int score = 0;


    // Start is called before the first frame update
    private void Awake() {
        networkMatchChecker = GetComponent<NetworkMatchChecker> ();
        DontDestroyOnLoad(this.gameObject);
    }
    public override void OnStartClient()
    {
        if (isLocalPlayer) {
            localPlayer = this;
        } else {
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab (this);
        }
    }    
    
    void FixedUpdate()
    {
        if(scoreText!=null)
            scoreText.text = score + "";
    }




    public void StartNetworkGame()
    {
        InvokeRepeating("FlapSync", 0, NetworkManagerLobby.tickrate);
        if(playerIndex == 1)
            CmdStartGame();
    }

    public void FlapSync()
    {
        if(this.isLocalPlayer && this.isInitialized && this.isAlive && angel != null) 
        {
            float movement = Input.GetAxis("Fire1");
            this.Flap(movement);
        }

    }

    [Command]
    void CmdStartGame()
    {
        levelManager.StartGame();

        //RpcStartGame();
    }

 


    public void Die()
    {
        CmdDie();
        isAlive = false;
        wingTrigger.Explode();
    }
    [Command]
    public void CmdDie()
    {
        RpcDie();
        isAlive = false;
        wingTrigger.wing.SetActive(false);
        wingTrigger.gameObject.SetActive(false);
        //wingTrigger.Explode();
    }

    [ClientRpc]
    public void RpcDie()
    {
        isAlive = false;
        wingTrigger.Explode();
    }





    public void GetPoint(GameObject orb)
    {

        score++;
        CmdGetPoint(orb);
    }
    [Command]
    public void CmdGetPoint(GameObject orb)
    {
        score++;
        levelManager.globalScore += 1;
        levelManager.RpcScoreChanged();
        levelManager.RemovePointOrb(orb);
        RpcGetPoint();
    }

    [ClientRpc]
    public void RpcGetPoint()
    {
        wingTrigger.wingAnim.SetTrigger("Bite");
        //gameManager.RpcUpdateScore(this.pos, this.score);
    }
    
    public void Flap(float moveRate)
    {  
        CmdFlap(moveRate);
    }


    [Command]
    public void CmdFlap(float moveRate)
    {
        if(angel!= null)
        {
            if(moveRate > 0)
            {
                angel.SetFlap(pos, true);
            }
            else
            {
                angel.SetFlap(pos, false);
            }
        }
    }

    
    public void ResetWingFlap()
    {
        CmdResetWingFlap();
    }

    [Command]
    public void CmdResetWingFlap()
    {
        if(angel!= null)
        {
            angel.SetFlap(PlayerPosition.LLEFT, false);
            angel.SetFlap(PlayerPosition.LRIGHT, false);
            angel.SetFlap(PlayerPosition.ULEFT, false);
            angel.SetFlap(PlayerPosition.URIGHT, false);
    }
    }

    public void SetAngel()
    {
        this.isInitialized = true;
        switch(playerIndex)
        {
            case 1 : this.pos = PlayerPosition.LLEFT; break;
            case 2 : this.pos = PlayerPosition.LRIGHT; break;
            case 3 : this.pos = PlayerPosition.ULEFT; break; 
            case 4 : this.pos = PlayerPosition.URIGHT; break;
        }
    }



    public void SaveName(string name)
    {
        this.playerName = name;
        CmdSaveName(name);
    }

    [Command]
    public void CmdSaveName(string name)
    {
        this.playerName = name;
    }

    
        /* 
            HOST MATCH
        */

        public void HostGame (bool publicMatch) {
            string matchID = MatchMaker.GetRandomMatchID ();
            CmdHostGame (matchID, publicMatch);
        }

        [Command]
        void CmdHostGame (string _matchID, bool publicMatch) {
            matchID = _matchID;
            if (MatchMaker.instance.HostGame (_matchID, gameObject, publicMatch, out playerIndex)) {
                Debug.Log ($"<color=green>Game hosted successfully</color>");
                networkMatchChecker.matchId = _matchID.ToGuid ();
                TargetHostGame (true, _matchID, playerIndex);
            } else {
                Debug.Log ($"<color=red>Game hosted failed</color>");
                TargetHostGame (false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetHostGame (bool success, string _matchID, int _playerIndex) {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log ($"MatchID: {matchID} == {_matchID}");
            UILobby.instance.HostSuccess (success, _matchID);
        }

        /* 
            JOIN MATCH
        */

        public void JoinGame (string _inputID) {
            CmdJoinGame (_inputID);
        }

        [Command]
        void CmdJoinGame (string _matchID) {
            matchID = _matchID;
            if (MatchMaker.instance.JoinGame (_matchID, gameObject, out playerIndex)) {
                Debug.Log ($"<color=green>Game Joined successfully</color>");
                networkMatchChecker.matchId = _matchID.ToGuid ();
                TargetJoinGame (true, _matchID, playerIndex);
            } else {
                Debug.Log ($"<color=red>Game Joined failed</color>");
                TargetJoinGame (false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame (bool success, string _matchID, int _playerIndex) {
            playerName = PlayerPrefs.GetString("PlayerName");
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log ($"MatchID: {matchID} == {_matchID}");
            UILobby.instance.JoinSuccess (success, _matchID);

        }

        /* 
            DISCONNECT
        */

        public void DisconnectGame () {
            CmdDisconnectGame ();
        }

        [Command]
        void CmdDisconnectGame () {
            ServerDisconnect ();
        }

        public void ServerDisconnect () {
            MatchMaker.instance.PlayerDisconnected (this, matchID);
            RpcDisconnectGame ();
            networkMatchChecker.matchId = string.Empty.ToGuid ();
        }

        [ClientRpc]
        void RpcDisconnectGame () {
            ClientDisconnect ();
        }

        public void ClientDisconnect () {
            if (playerLobbyUI != null) {
                Destroy (playerLobbyUI);
            }
        }

        /*
            LIST MATCHES
        */

        public void GetMatchList()
        {
            
        }

        /* 
            SEARCH MATCH
        */

        public void SearchGame () {
            CmdSearchGame ();
        }

        [Command]
        void CmdSearchGame () {
            if (MatchMaker.instance.SearchGame (gameObject, out playerIndex, out matchID)) {
                Debug.Log ($"<color=green>Game Found Successfully</color>");
                networkMatchChecker.matchId = matchID.ToGuid ();
                TargetSearchGame (true, matchID, playerIndex);
            } else {
                Debug.Log ($"<color=red>Game Search Failed</color>");
                TargetSearchGame (false, matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetSearchGame (bool success, string _matchID, int _playerIndex) {
            playerName = PlayerPrefs.GetString("PlayerName");
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log ($"MatchID: {matchID} == {_matchID} | {success}");
            UILobby.instance.SearchGameSuccess (success, _matchID);
        }

        /* 
            BEGIN MATCH
        */

        public void BeginGame () {
            CmdBeginGame ();        


        }

        [Command]
        void CmdBeginGame () {
            MatchMaker.instance.BeginGame (matchID);

            Debug.Log ($"<color=red>Game Beginning</color>");
        }

        public void StartGame () { //Server
            TargetBeginGame ();
            //CmdSpawnAngel();
        }



        [TargetRpc]
        void TargetBeginGame () {
            Debug.Log ($"MatchID: {matchID} | Beginning");

            SceneManager.LoadScene (2, LoadSceneMode.Additive);

        }


        public void SpawnTestObject()
        {
            CmdSpawnTestObject();
        }

        [Command]
        public void CmdSpawnTestObject()
        {
            levelManager.SpawnTestRock();
            Debug.Log("TEST OBJECT SPAWNED");
        }

}
