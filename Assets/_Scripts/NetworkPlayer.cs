using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    
    public static NetworkPlayer localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public string playerName;
    [SyncVar] public int playerIndex;

    NetworkMatchChecker networkMatchChecker;

    [SyncVar] public Match currentMatch;

    [SerializeField] GameObject playerLobbyUI;
    public GameObject prefab_angel;

    [SyncVar]
    public PlayerPosition pos;
    public WingControl angel = null;
    public NetworkStart gameManager = null;
    bool isInitialized = false;
    [SyncVar]
    public bool isAlive = true;
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
            Debug.Log ($"Spawning other player UI Prefab");
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab (this);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(angel == null)
        {
        }
        if(this.isLocalPlayer && this.isInitialized && this.isAlive && angel != null) 
        {
            float movement = Input.GetAxis("Fire1");
            this.Flap(movement);
        }
    }


    public void StartNetworkGame()
    {
        if(playerIndex == 1 && isLocalPlayer)
            LevelManager.levelManager.StartGame();
    }

    [Command]
    void CmdStartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    void RpcStartGame()
    {
        LevelManager.levelManager.StartGame();
    }


    public void SpawnObjectInNetwork(Vector3 pos)
    {

        CmdSpawnObjectInNetwork( pos);
    }

    [Command]
    public void CmdSpawnObjectInNetwork(Vector3 pos)
    {
        GameObject obj = (GameObject)Instantiate(NetworkManagerLobby.pointOrb_prefab, pos,  Quaternion.identity);
        NetworkServer.Spawn(obj);
        obj.GetComponent<NetworkMatchChecker>().matchId = this.matchID.ToGuid();

        //RpcSpawnObjectInNetwork(pos);
    }
    
    [ClientRpc]
    public void RpcSpawnObjectInNetwork( Vector3 pos)
    {
        GameObject obj = (GameObject)Instantiate(LevelManager.levelManager.prefab_pointOrb, pos,  Quaternion.identity);

    }
    




    public void GetPoint()
    {

        score++;
        //Debug.Log("Player Get Point " + score);
        //CmdGetPoint();
    }
    [Command]
    public void CmdGetPoint()
    {
        Debug.Log("CmdGetPoint " + score);

        gameManager.levelManager.RpcScoreChanged(gameManager.globalScore + 1);
        gameManager.RpcUpdateScore(this.pos, this.score);
        RpcGetPoint();
    }

    [ClientRpc]
    public void RpcGetPoint()
    {
        Debug.Log("RpcGetPoint " + score);
        
        score++;
        gameManager.RpcUpdateScore(this.pos, this.score);
    }
    
    public void Flap(float moveRate)
    {
        CmdFlap(moveRate, pos, angel.transform.position, angel.transform.rotation.z, angel.rb2d.velocity, angel.rb2d.angularVelocity);
    }

    [Command]
    public void CmdFlap(float moveRate, PlayerPosition pos, Vector2 angelPos, float angelRot, Vector2 velocity, float angularVelocity)
    {
        if(moveRate > 0)
        {
            angel.SetFlap(pos, true);
        }

        else
        {
            angel.SetFlap(pos, false);
        }

        RpcFlap(moveRate, pos, angelPos, angelRot, velocity, angularVelocity);
    }

    [ClientRpc]
    public void RpcFlap(float moveRate, PlayerPosition pos, Vector2 angelPos, float angelRot, Vector2 velocity, float angularVelocity)
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
        CmdSetPos();
    }

    [Command]
    public void CmdSetPos()
    {
        switch(playerIndex)
        {
            case 1 : this.pos = PlayerPosition.LLEFT; break;
            case 2 : this.pos = PlayerPosition.LRIGHT; break;
            case 3 : this.pos = PlayerPosition.ULEFT; break; 
            case 4 : this.pos = PlayerPosition.URIGHT; break;
        }
        RpcSetPos();
    }

    [ClientRpc]
    public void RpcSetPos()
    {
        switch(playerIndex)
        {
            case 1 : this.pos = PlayerPosition.LLEFT; break;
            case 2 : this.pos = PlayerPosition.LRIGHT; break;
            case 3 : this.pos = PlayerPosition.ULEFT; break; 
            case 4 : this.pos = PlayerPosition.URIGHT; break;
        }
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

        void ServerDisconnect () {
            MatchMaker.instance.PlayerDisconnected (this, matchID);
            RpcDisconnectGame ();
            networkMatchChecker.matchId = string.Empty.ToGuid ();
        }

        [ClientRpc]
        void RpcDisconnectGame () {
            ClientDisconnect ();
        }

        void ClientDisconnect () {
            if (playerLobbyUI != null) {
                Destroy (playerLobbyUI);
            }
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

            SceneManager.LoadScene (1, LoadSceneMode.Additive);
            angel = GameObject.Find("Angel(Clone)").GetComponent<WingControl>();

            //Additively load game scene
            //NetworkManager.singleton.ServerChangeScene("Main");
            //LevelManager.levelManager.StartGame();
            /*if(isLocalPlayer)
            {
                print("HII AM LOCAL");
                this.wing = GameObject.Find("Angel").GetComponent<WingControl>();
                gameManager = GameObject.Find("_GameManager").GetComponent<NetworkStart>();
                this.wing.players.Add(this);        
                switch(wing.players.Count)
                {
                    case 1 : pos = PlayerPosition.LLEFT; break;
                    case 2 : pos = PlayerPosition.LRIGHT; break;
                    case 3 : pos = PlayerPosition.ULEFT; break; 
                    case 4 : pos = PlayerPosition.URIGHT; break;
                }
                if(this.wing != null)
                    this.isInitialized = true;
                //SceneManager.LoadScene (1, LoadSceneMode.Additive);
            }*/

        }


        

}
