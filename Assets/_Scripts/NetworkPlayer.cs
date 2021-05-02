using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer localPlayer;
    public PlayerPosition pos;
    public WingControl wing = null;
    public NetworkStart gameManager = null;
    bool isInitialized = false;
    public bool isAlive = true;
    [SerializeField] GameObject playerLobbyUI;
    public int score = 0;

    // Start is called before the first frame update
    private void Awake() {
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
        if(wing == null)
        {
        }
        if(this.isLocalPlayer && this.isInitialized && this.isAlive) 
        {
            float movement = Input.GetAxis("Fire1");
            this.Flap(movement);
        }
    }

    public void GetPoint()
    {
        if(isLocalPlayer)
        {
            Debug.Log("Player Get Point " + score);
            CmdGetPoint();
            //gameManager.RpcUpdateScore(this.pos, this.score);
        }

    }
    [Command]
    public void CmdGetPoint()
    {

        Debug.Log("CmdGetPoint " + score);
        //gameManager.CmdUpdateScore(this.pos, this.score);

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
        //gameManager.levelManager.RpcScoreChanged(gameManager.globalScore + 1);
    }

    public void Flap(float moveRate)
    {
        if(isLocalPlayer)
        {
            if(moveRate > 0)
                wing.SetFlap(pos, true);
            else 
                wing.SetFlap(pos, false);
        }


    }

}
