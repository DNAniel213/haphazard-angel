using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    public PlayerPosition pos;
    public WingControl wing = null;
    public NetworkStart gameManager = null;
    bool isInitialized = false;
    public int score = 0;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        wing = GameObject.Find("Angel").GetComponent<WingControl>();
        gameManager = GameObject.Find("__NetworkManager").GetComponent<NetworkStart>();

        wing.players.Add(this);

        switch(wing.players.Count)
        {
            case 1 : pos = PlayerPosition.LLEFT; break;
            case 2 : pos = PlayerPosition.LRIGHT; break;
            case 3 : pos = PlayerPosition.ULEFT; break;
            case 4 : pos = PlayerPosition.URIGHT; break;
        }
        if(wing != null)
            isInitialized = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(wing == null)
        {
        }
        if(this.isLocalPlayer && wing != null) 
        {
            float movement = Input.GetAxis("Fire1");
            this.Flap(movement);
        }
    }

    public void GetPoint()
    {
        Debug.Log("Player Get Point " + score);
        CmdGetPoint();
    }
    
    [Command]
    public void CmdGetPoint()
    {

        Debug.Log("CmdGetPoint " + score);
        //score++;
        //gameManager.CmdUpdateScore(this.pos, this.score);

        RpcGetPoint();
        gameManager.levelManager.RpcScoreChanged(gameManager.globalScore + 1);
    }

    [ClientRpc]
    public void RpcGetPoint()
    {
        Debug.Log("RpcGetPoint " + score);
        
        score++;
        gameManager.RpcUpdateScore(this.pos, this.score);
    }

    [Command]
    public void Flap(float moveRate)
    {

        if(moveRate > 0)
        {
            switch(pos)
            {
                case PlayerPosition.ULEFT : wing.ulFlap = true; break;
                case PlayerPosition.URIGHT : wing.urFlap = true; break;
                case PlayerPosition.LLEFT : wing.llFlap = true; break;
                case PlayerPosition.LRIGHT : wing.lrFlap = true; break;
            }
        }
        else
        {
            switch(pos)
            {
                case PlayerPosition.ULEFT : wing.ulFlap = false; break;
                case PlayerPosition.URIGHT : wing.urFlap = false; break;
                case PlayerPosition.LLEFT : wing.llFlap = false; break;
                case PlayerPosition.LRIGHT : wing.lrFlap = false; break;
            }
        }

    }
}
