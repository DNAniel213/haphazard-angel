using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultGenerator : MonoBehaviour
{
    public Text teamScoreText, teamQuote, introQuote;
    public Text[] playerScores, playerNames; 
    public WingControl angel;

    int teamScore = 0;

    public GameObject introQuoteObject = null;

    string[] quotes_overwhelming =
    {
        "\"Flapping with a single wing will only get you in circles.\"",
        "\"Individual commitment to a group effort—that is what makes a team work, a company work, a society work, a civilization work.\"",
        "\"Coming together is a beginning. Keeping together is progress. Working together is success.\"",
        "\"Alone we can do so little, together we can do so much\"",
        "\"None of us can fly as high as all of us\"",
        "\"The strength of the team is each individual member. The strength of each member is the team.\"",
        "\"It takes two flints to make a fire, four wings to make an angel.\"",
        "\"If you want to lift yourself up, lift up someone else.\"",
        "\"If you can laugh together, you can work together.\"",
        "\"One cannot fly with a single wing\"",
    };

    private void Start() {
        //this.gameObject.SetActive(false);
    }

    public void GenerateScores(WingControl angel)
    {
        introQuoteObject.SetActive(false);
        this.angel = angel;
        int[] scores = {0, 0, 0, 0};
        int[] sortedScores = {0, 0, 0, 0};
        int index = 0;
        foreach(NetworkPlayer player in angel.players)
        {
            teamScore += player.score;

            playerScores[index].text = player.score + "";
            playerNames[index].text = player.playerName;
            index++;

        }



        //sortedScores = scores;

        //Array.Sort(sortedScores);
        //Array.Reverse(sortedScores);
        

        

        teamQuote.text = quotes_overwhelming[UnityEngine.Random.Range(0, quotes_overwhelming.Length -1)];

        teamScoreText.text = teamScore + "";
        
    }

    public void Button_Disconnect()
    {
        NetworkPlayer.localPlayer.DisconnectGame();
        NetworkManagerLobby.singleton.StopClient();
    }
}
