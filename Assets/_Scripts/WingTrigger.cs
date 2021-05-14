using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class WingTrigger : MonoBehaviour
{
    public NetworkStart netStart = null;
    public NetworkPlayer player = null; 
    public WingControl wingControl;
    public PlayerPosition wingPosition = PlayerPosition.NULL;
    public bool initialized = false;
    public GameObject wing = null;
    
    public Text scoreText = null;

    public void Start()
    {
        StartCoroutine(LateStart(0.5f));

    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        initialized = true;

        GameObject[] players = GameObject.FindGameObjectsWithTag("NetworkPlayer");

        foreach(GameObject playerobj in players)
        {
            NetworkPlayer p = playerobj.GetComponent<NetworkPlayer>();
            p.SetAngel();
            Debug.Log(p.pos + " ??? "  + wingPosition);
            if(p.pos == wingPosition)
            {
                this.player = playerobj.GetComponent<NetworkPlayer>();
            }
        }

        if(player == null)
        {
            wing.SetActive(false);
        }

        if(NetworkPlayer.localPlayer != null)
        {
            switch(wingPosition)
            {
                case PlayerPosition.LLEFT : scoreText = GameObject.Find("llScore").GetComponent<Text>(); break;
                case PlayerPosition.LRIGHT : scoreText = GameObject.Find("lrScore").GetComponent<Text>();break;
                case PlayerPosition.ULEFT : scoreText = GameObject.Find("ulScore").GetComponent<Text>();break;
                case PlayerPosition.URIGHT : scoreText = GameObject.Find("urScore").GetComponent<Text>();break;
            }
        }
        else
        {
            //this.gameObject.SetActive(false);
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

        if(other.gameObject.CompareTag("Point") && NetworkPlayer.localPlayer != null)
        {   
            other.gameObject.SetActive(false);


            if(player.gameObject == NetworkClient.localPlayer.gameObject && player.isAlive)
                GetPoint(other.gameObject);

            //CmdDisposeOrb(other.gameObject);
        }


    }

     private void OnCollisionEnter2D(Collision2D other) {

            if(other.gameObject.CompareTag("Spikes") && player.gameObject == NetworkClient.localPlayer.gameObject )
            {
                //CmdEliminatePlayer();
            }
    }

    public void CmdEliminatePlayer()
    {
        RpcEliminatePlayer();
    }

    public void RpcEliminatePlayer()
    {

        wing.SetActive(false);
        player.isAlive = false;
    }

    public void CmdDisposeOrb(GameObject orb)
    {
        if(orb)
        {
            orb.SetActive(false);
            RpcDisposeOrb(orb);
        }

    }

    public void RpcDisposeOrb(GameObject orb)
    {
        if(orb)
            orb.SetActive(false);

    }

    public void GetPoint(GameObject orb)
    {
        Debug.Log("Getting Point");
        player.GetPoint();
        scoreText.text = player.score + "";
    }

}
