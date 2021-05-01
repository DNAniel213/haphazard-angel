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
    public GameObject wing = null;


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

            this.wing.SetActive(false);
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
        if(!isServer)
        {
            if(other.gameObject.CompareTag("Point")  && player.gameObject == NetworkClient.localPlayer.gameObject && player.isAlive)
            {   
                GetPoint();
                CmdDisposeOrb(other.gameObject);
            }

        }

    }

     private void OnCollisionEnter2D(Collision2D other) {
        if(!isServer)
        {
            if(other.gameObject.CompareTag("Spikes") && player.gameObject == NetworkClient.localPlayer.gameObject )
            {
                CmdEliminatePlayer();
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdEliminatePlayer()
    {
        RpcEliminatePlayer();
    }

    [ClientRpc]
    public void RpcEliminatePlayer()
    {

        wing.SetActive(false);
        player.isAlive = false;
    }

    [Command(requiresAuthority = false)]
    public void CmdDisposeOrb(GameObject orb)
    {
        if(orb)
        {
            orb.SetActive(false);
            RpcDisposeOrb(orb);
        }

    }

    [ClientRpc]
    public void RpcDisposeOrb(GameObject orb)
    {
        if(orb)
            orb.SetActive(false);

    }

    public void GetPoint()
    {
        Debug.Log("Getting Point");
        player.GetPoint();

    }

}
