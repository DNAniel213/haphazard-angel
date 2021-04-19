using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class NetworkStart : NetworkBehaviour
{
    [Header("Vars")]
    public LevelManager levelManager;
    public int globalScore = 0;

    [Header("Wing Triggers")]
    public WingTrigger llwCol;
    public WingTrigger lrwCol, ulwCol, urwCol;

    [Header("Player Score Text")]
    public Text llscoreText;
    public Text lrscoreText, ulscoreText, urscoreText;
    // Start is called before the first frame update
    

    [Command(requiresAuthority = false)]
    public void StartGame()
    {

        SetWingTriggers();
        levelManager.StartGame();

    }

    [Command]
    public void CmdUpdateScore(PlayerPosition pos, int score)
    {
        Debug.Log("CmdUpdateScore");
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
        Debug.Log("RpcUpdateScore");

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

    [ClientRpc]
    void SetWingTriggers()
    {

        llwCol.InitWingTrigger();
        lrwCol.InitWingTrigger();
        ulwCol.InitWingTrigger();
        urwCol.InitWingTrigger();
    }
}
