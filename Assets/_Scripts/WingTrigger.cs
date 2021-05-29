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

    public GameObject tutorialScreen = null;

    public void Start()
    {
        StartCoroutine(LateStart(5));

    }
    

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        resultPanel = GameObject.Find("ResultPanel");

        //yield return new WaitForSeconds(1);


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

                    switch(wingPosition)
                    {
                        case PlayerPosition.LLEFT :tutorialScreen = GameObject.Find("LLTutorial"); break;
                        case PlayerPosition.LRIGHT : tutorialScreen = GameObject.Find("LRTutorial");break;
                        case PlayerPosition.ULEFT : tutorialScreen = GameObject.Find("ULTutorial");break;
                        case PlayerPosition.URIGHT : tutorialScreen = GameObject.Find("URTutorial");break;
                    }
                    p.nameText.text = p.playerName;
                }

                if(p == NetworkPlayer.localPlayer)
                {   
                    Invoke("EndTutorial", waitTime);
                }
                else
                {
                    tutorialScreen.SetActive(false);
                }
            }
        }

        if(NetworkPlayer.localPlayer!=null )
        {
            if(NetworkPlayer.localPlayer == this.player)
            {
                resultPanel.SetActive(false);
            }
        }


        if(this.player == null)
        {
            wing.SetActive(false);
            if(NetworkPlayer.localPlayer!=null)
            {
                switch(wingPosition)
                {
                    case PlayerPosition.LLEFT : GameObject.Find("LL").SetActive(false); GameObject.Find("LLTutorial").SetActive(false); break;
                    case PlayerPosition.LRIGHT : GameObject.Find("LR").SetActive(false);GameObject.Find("LRTutorial").SetActive(false); break;
                    case PlayerPosition.ULEFT : GameObject.Find("UL").SetActive(false); GameObject.Find("ULTutorial").SetActive(false);break;
                    case PlayerPosition.URIGHT : GameObject.Find("UR").SetActive(false); GameObject.Find("URTutorial").SetActive(false);break;
                }
            }

        }

        initialized = true;
    }

    public void EndTutorial()
    {
        tutorialScreen.SetActive(false);
    }

    public void FixedUpdate() {
        if(this.player == null && initialized)
        {
            CheckGameEnd();

            if(NetworkPlayer.localPlayer!=null)
            {
                NetworkPlayer.localPlayer.ResetWingFlap();  
                switch(wingPosition)
                {
                    case PlayerPosition.LLEFT : GameObject.Find("LLWing").SetActive(false); break;
                    case PlayerPosition.LRIGHT : GameObject.Find("LRWing").SetActive(false);break;
                    case PlayerPosition.ULEFT : GameObject.Find("ULWing").SetActive(false);break;
                    case PlayerPosition.URIGHT : GameObject.Find("URWing").SetActive(false);break;
                }
                wing.SetActive(false);
            }

        }
    }




    public void CheckGameEnd()
    {
        if(NetworkPlayer.localPlayer !=null)
        {

            int alive = 0;
            foreach(NetworkPlayer playerobj in wingControl.players)
            {
                if(playerobj.isAlive && playerobj!=null)
                    alive++;
            }

            if(alive > 1)
            {
                
            }
            else 
            {
                wingControl.DestroyAngel();
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
        player.GetPoint(orb);
    }


}
