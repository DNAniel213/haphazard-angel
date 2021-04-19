using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkStart : NetworkBehaviour
{
    public LevelManager levelManager;
    public int globalScore = 0;

    [Header("Wing Triggers")]
    public WingTrigger llwCol;
    public WingTrigger lrwCol, ulwCol, urwCol;

    [Header("Player Score Text")]
    public Text llscoreText;
    public Text lrscoreText, ulscoreText, urscoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void StartGame()
    {

        SetWingTriggers();
        levelManager.StartGame();

    }
    public void CmdUpdateScore(PlayerPosition pos, int score)
    {
        globalScore++;
        levelManager.ScoreChanged(globalScore);
        switch(pos)
        {
            case PlayerPosition.ULEFT : ulscoreText.text = score+""; break;
            case PlayerPosition.URIGHT : urscoreText.text = score+""; break;
            case PlayerPosition.LLEFT : llscoreText.text = score+""; break;
            case PlayerPosition.LRIGHT : lrscoreText.text = score+""; break;
        }
        RpcUpdateScore(pos, score);
    }

    public void RpcUpdateScore(PlayerPosition pos, int score)
    {
        globalScore++;
        levelManager.ScoreChanged(globalScore);
        switch(pos)
        {
            case PlayerPosition.ULEFT : ulscoreText.text = score+""; break;
            case PlayerPosition.URIGHT : urscoreText.text = score+""; break;
            case PlayerPosition.LLEFT : llscoreText.text = score+""; break;
            case PlayerPosition.LRIGHT : lrscoreText.text = score+""; break;
        }

    }
    
    void SetWingTriggers()
    {
        llwCol.InitWingTrigger();
        lrwCol.InitWingTrigger();
        ulwCol.InitWingTrigger();
        urwCol.InitWingTrigger();
    }
}
