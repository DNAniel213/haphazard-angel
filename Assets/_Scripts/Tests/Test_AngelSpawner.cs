using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Test_AngelSpawner : MonoBehaviour
{   
    public GameObject obj_prefab;
    public int objCount =0;
    public Text text_number;
    public void SpawnLoop(int number)
    {
        for(int i= 0; i<number;i++)
        {
            SpawnTestAngel();
        }
    }
    public void SpawnTestAngel()
    {
        NetworkPlayer.localPlayer.SpawnTestObject();
        //NetworkServer.Spawn(angel);
        objCount++;
        text_number.text =objCount +"";
    }   
}
