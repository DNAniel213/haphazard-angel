using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WingTrigger : NetworkBehaviour
{

    public NetworkPlayer player = null; 
    public WingControl wingControl;
    public PlayerPosition wingPosition = PlayerPosition.NULL;
    public bool initialized = false;

    [ClientRpc]
    public void InitWingTrigger()
    {
        initialized = true;

        foreach(NetworkPlayer iPlayer in wingControl.players)
        {
            if(iPlayer.pos == wingPosition )
            {
                this.player = iPlayer;

                Debug.Log("WingTrigger Initialized for " + wingPosition.ToString() );
            }
        }
        if(player == null)
        {
            this.gameObject.SetActive(false);
        }
    }



    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    /// 
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Point")  && player.gameObject == NetworkClient.localPlayer.gameObject)
        {   
            GetPoint();
            CmdDisposeOrb(other.gameObject);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDisposeOrb(GameObject orb)
    {
        RpcDisposeOrb(orb);
    }

    [ClientRpc]
    public void RpcDisposeOrb(GameObject orb)
    {
        orb.SetActive(false);

    }

    public void GetPoint()
    {
        Debug.Log("Getting Point");
        player.GetPoint();

    }

}
