using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class WingTrigger : MonoBehaviour
{
    public NetworkStart netStart = null;
    public NetworkPlayer player = null; 
    public Animator wingAnim = null;
    public WingControl wingControl;
    public PlayerPosition wingPosition = PlayerPosition.NULL;
    public bool initialized = false;
    public GameObject wing = null;

    public GameObject resultPanel = null;
    
    public Text scoreText = null;

    public void Start()
    {

        StartCoroutine(LateStart(1));

    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        initialized = true;


        foreach(NetworkPlayer p in wingControl.players)
        {
            p.SetAngel();

            if(p.pos == wingPosition)
            {
                this.player = p;
                player.wingTrigger =  this;
                if(NetworkPlayer.localPlayer != null)
                {
                    switch(wingPosition)
                    {
                        case PlayerPosition.LLEFT : p.scoreText = GameObject.Find("llScore").GetComponent<Text>(); break;
                        case PlayerPosition.LRIGHT : p.scoreText = GameObject.Find("lrScore").GetComponent<Text>();break;
                        case PlayerPosition.ULEFT : p.scoreText = GameObject.Find("ulScore").GetComponent<Text>();break;
                        case PlayerPosition.URIGHT : p.scoreText = GameObject.Find("urScore").GetComponent<Text>();break;
                    }

                    switch(wingPosition)
                    {
                        case PlayerPosition.LLEFT : p.nameText = GameObject.Find("llName").GetComponent<Text>(); break;
                        case PlayerPosition.LRIGHT : p.nameText = GameObject.Find("lrName").GetComponent<Text>();break;
                        case PlayerPosition.ULEFT : p.nameText = GameObject.Find("ulName").GetComponent<Text>();break;
                        case PlayerPosition.URIGHT : p.nameText = GameObject.Find("urName").GetComponent<Text>();break;
                    }

                    p.nameText.text = p.playerName;

                }
            }
        }

        if(wingPosition == NetworkPlayer.localPlayer.pos)
        {
            resultPanel = GameObject.Find("ResultPanel");
            resultPanel.SetActive(false);
        }

        if(player == null)
        {
            wing.SetActive(false);
            switch(wingPosition)
            {
                case PlayerPosition.LLEFT : GameObject.Find("LL").SetActive(false); break;
                case PlayerPosition.LRIGHT : GameObject.Find("LR").SetActive(false);break;
                case PlayerPosition.ULEFT : GameObject.Find("UL").SetActive(false);break;
                case PlayerPosition.URIGHT : GameObject.Find("UR").SetActive(false);break;
            }
        }

        if(NetworkPlayer.localPlayer != null)
        {

        }
        else
        {
            //this.gameObject.SetActive(false);
        }

    }



    public void CheckGameEnd()
    {
        if(wingPosition == NetworkPlayer.localPlayer.pos)
        {
            print("SOMEONE FUCKING DIED");

            int alive = 0;
            foreach(NetworkPlayer playerobj in wingControl.players)
            {
                if(playerobj.isAlive)
                    alive++;
            }

            if(alive > 1)
            {
                
            }
            else 
            {
                resultPanel.SetActive(true);
                resultPanel.GetComponent<ResultGenerator>().GenerateScores(wingControl);
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
        if(other.gameObject.CompareTag("Point") && player != null )
        {   
            if(NetworkPlayer.localPlayer!=null)
            {
                if(player.gameObject == NetworkPlayer.localPlayer.gameObject && player.isAlive)
                {
                    other.gameObject.SetActive(false);
                    GetPoint(other.gameObject);
                }
            }

            //CmdDisposeOrb(other.gameObject);
        }   
    }
    private void OnCollisionEnter2D(Collision2D other) {


        if(other.gameObject.CompareTag("Spikes") && player != null )
        {
            if(NetworkPlayer.localPlayer!=null)
            {
                if(player.gameObject == NetworkPlayer.localPlayer.gameObject && player.isAlive)
                {
                    player.Die();
                }
            }

            //CmdEliminatePlayer();
        }


        

    }

    public void Explode()
    {
        StartCoroutine(DoExplodeAnim());
        print("Die");
    }

    public IEnumerator DoExplodeAnim()
    {
        wingAnim.SetTrigger("Die");
        yield return new WaitForSeconds(2);
        CheckGameEnd();
        wing.SetActive(false);

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
        player.GetPoint(orb);
    }


}
