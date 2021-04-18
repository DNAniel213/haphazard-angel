using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkWingTrigger : NetworkBehaviour
{

    public NetworkPlayer player; 
    public WingControl wingControl;
    public PlayerPosition wingPosition;
    public bool initialized = false;

    public void InitWingTrigger()
    {
        initialized = true;

        foreach(NetworkPlayer player in wingControl.players)
        {
            if(player.pos == wingPosition)
            {
                this.player = player;

                Debug.Log("WingTrigger Initialized for " + wingPosition.ToString() );
            }

        }
    }



    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Point"))
        {   
            other.gameObject.SetActive(false);
            GetPoint();
        }
    }

    public void GetPoint()
    {
        player.GetPoint();
    }

}
