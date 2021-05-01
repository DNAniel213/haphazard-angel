using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private NetworkManagerLobby networkManager = null;
    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject playerPanel = null;
    [SerializeField] private GameObject nameInputPanel = null;

    public void Start()
    {
        nameInputPanel.SetActive(true);
    }

    public void HostLobby()
    {
        networkManager.StartHost();

        landingPagePanel.SetActive(false);
        playerPanel.SetActive(true);
    }
}
