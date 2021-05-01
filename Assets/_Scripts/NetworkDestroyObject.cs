using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class NetworkDestroyObject : NetworkBehaviour {
    
    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        NetworkServer.Destroy(other.gameObject);
        Destroy(other.gameObject);
    }
}
