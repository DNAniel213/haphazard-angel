using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class NetworkStart : MonoBehaviour
{

    
    public GameObject angel;
    [Header("Vars")]
    public LevelManager levelManager;
    public int globalScore = 0;

    [Header("Wing Triggers")]
    public WingTrigger llwCol;
    public WingTrigger lrwCol, ulwCol, urwCol;

    [Header("Player Score Text")]
    public Text llscoreText;
    public Text lrscoreText, ulscoreText, urscoreText;

    [Header("Player Names")]
    public Text[] playerNames;
    public Text player2Text, player3Text, player4Text;
    // Start is called before the first frame update
    

    public void Start()
    {
        if(NetworkPlayer.localPlayer != null)
            NetworkPlayer.localPlayer.gameManager = this;
        //angel.transform.position= new Vector3(0,0,0);


        
        GameObject[] players = GameObject.FindGameObjectsWithTag("NetworkPlayer");

        foreach(GameObject playerobj in players)
        {
            for (int i = 0; i < 5; i++)
            {
                if((i + 1) == playerobj.GetComponent<NetworkPlayer>().playerIndex)
                {
                    //playerNames[i].text = playerobj.GetComponent<NetworkPlayer>().playerName;
                }
            }
        }

        //SetWingTriggers();
        //levelManager.StartGame();

    }

    public void CmdUpdateScore(PlayerPosition pos, int score)
    {
        globalScore++;
        switch(pos)
        {
            case PlayerPosition.ULEFT : ulscoreText.text = score+""; break;
            case PlayerPosition.URIGHT : urscoreText.text = score+""; break;
            case PlayerPosition.LLEFT : llscoreText.text = score+""; break;
            case PlayerPosition.LRIGHT : lrscoreText.text = score+""; break;
        }
    }

    public void RpcUpdateScore(PlayerPosition pos, int score)
    {

        globalScore++;
        //levelManager.RpcScoreChanged(globalScore);
        switch(pos)
        {
            case PlayerPosition.ULEFT : ulscoreText.text = score+""; break;
            case PlayerPosition.URIGHT : urscoreText.text = score+""; break;
            case PlayerPosition.LLEFT : llscoreText.text = score+""; break;
            case PlayerPosition.LRIGHT : lrscoreText.text = score+""; break;
        }

    }
    /*
    void SetWingTriggers()
    {
        llwCol.InitWingTrigger();
        lrwCol.InitWingTrigger();
        ulwCol.InitWingTrigger();
        urwCol.InitWingTrigger();
    }*/
}
