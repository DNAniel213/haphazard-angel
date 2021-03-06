using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{

    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")] 
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject playerPanel = null;
    [SerializeField] private InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable() {
        Debug.Log("enabled");
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
        
    }

    private void OnDisable() {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;

    }
    
    public void SetIP()
    {
        string name = ipAddressInputField.text;
        joinButton.interactable = !string.IsNullOrEmpty(name);
    }
    public void JoinLobby()
    {
        
        if(networkManager.isNetworkActive)
        {
            Debug.Log("Aborting current connection");
            networkManager.StopClient();
        }

        string ipAddress =ipAddressInputField.text;
        Debug.Log("Joining on: " + ipAddress);

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;

    }

    private void HandleClientConnected()
    {
        Debug.Log("Game found");
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
        playerPanel.SetActive(true);
    }

    private void HandleClientDisconnected()
    {
        Debug.Log("Game not found");
        joinButton.interactable = true;
    }


}

