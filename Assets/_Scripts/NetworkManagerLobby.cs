﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Mirror;

public class NetworkManagerLobby : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public static GameObject pointOrb_prefab;

    //public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    private void Start() {
        NetworkManagerLobby.pointOrb_prefab = this.spawnPrefabs[0];

    }



    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("New Player Connected: " + conn.connectionId);

        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("Client Truomg tp cpmmect Started");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        if(SceneManager.GetActiveScene().path == menuScene)
        {
            Debug.Log("New Player Added");

            //NetworkPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);
            //NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }


}
