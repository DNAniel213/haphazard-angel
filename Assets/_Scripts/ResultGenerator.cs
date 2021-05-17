using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultGenerator : MonoBehaviour
{
    public Text teamScoreText, teamQuote;
    public Text[] playerScores, playerNames; 
    public WingControl angel;

    int teamScore = 0;

    public void GenerateScores(WingControl angel)
    {
        this.angel = angel;
        int[] scores = {0, 0, 0, 0};
        int[] sortedScores = {0, 0, 0, 0};
        int index = 0;
        foreach(NetworkPlayer player in angel.players)
        {
            int score = angel.players[index].score;
            scores[index] = score;

            teamScore += score;
            playerScores[index].text = sortedScores[index] + "";
            playerNames[index].text = angel.players[index].playerName;
            index++;

        }
        sortedScores = scores;

        Array.Sort(sortedScores);
        Array.Reverse(sortedScores);

        

        for(int i =0; i <4 ;i++)
        {
            playerScores[i].text = sortedScores[i] + "";
            for(int j= 0; j<4; j++)
            {
                if(scores[j] == sortedScores[i])
                {
                    playerNames[j].text = angel.players[i].playerName; 
                }
            }
        }

        teamScoreText.text = teamScore + "";
        
    }
}
