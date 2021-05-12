using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class UILobby : MonoBehaviour {

    public static UILobby instance;

    [Header ("Host Join")]
    [SerializeField] InputField joinMatchInput;
    [SerializeField] InputField nameInputField;
    [SerializeField] List<Selectable> lobbySelectables = new List<Selectable> ();
    [SerializeField] Canvas lobbyCanvas;
    [SerializeField] Canvas searchCanvas;
    [SerializeField] GameObject connectCanvas;
    bool searching = false;

    [Header ("Lobby")]
    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] Text matchIDText;
    [SerializeField] GameObject beginGameButton;
    private const string PlayerPrefsNameKey = "PlayerName";

    GameObject localPlayerLobbyUI;

    void Start () {
        instance = this;
        SetUpInputField();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }


    private void SetUpInputField()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text= defaultName;
    }

    

    public void SavePlayerName()
    {
        PlayerPrefs.SetString(PlayerPrefsNameKey, nameInputField.text);
        NetworkPlayer.localPlayer.playerName = nameInputField.text;
    }

    public void HostPublic () {
        lobbySelectables.ForEach (x => x.interactable = false);

        NetworkPlayer.localPlayer.HostGame (true);
        SavePlayerName();
    }

    public void HostPrivate () {
        lobbySelectables.ForEach (x => x.interactable = false);

        NetworkPlayer.localPlayer.HostGame (false);
        SavePlayerName();
    }

    public void HostSuccess (bool success, string matchID) {
        if (success) {
            lobbyCanvas.enabled = true;

            if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
            localPlayerLobbyUI = SpawnPlayerUIPrefab (NetworkPlayer.localPlayer);
            matchIDText.text = matchID;
            beginGameButton.SetActive (true);
        } else {
            lobbySelectables.ForEach (x => x.interactable = true);
        }
    }

    public void Join () {
        lobbySelectables.ForEach (x => x.interactable = false);

        NetworkPlayer.localPlayer.JoinGame (joinMatchInput.text.ToUpper ());
        SavePlayerName();
    }

    public void JoinSuccess (bool success, string matchID) {
        if (success) {
            lobbyCanvas.enabled = true;

            if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
            localPlayerLobbyUI = SpawnPlayerUIPrefab (NetworkPlayer.localPlayer);
            matchIDText.text = matchID;
        } else {
            lobbySelectables.ForEach (x => x.interactable = true);
        }
    }

    public void DisconnectGame () {
        if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
        NetworkPlayer.localPlayer.DisconnectGame ();

        lobbyCanvas.enabled = false;
        lobbySelectables.ForEach (x => x.interactable = true);
        beginGameButton.SetActive (false);
    }

    public GameObject SpawnPlayerUIPrefab (NetworkPlayer player) {
        GameObject newUIPlayer = Instantiate (UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer> ().SetPlayer (player);
        newUIPlayer.transform.SetSiblingIndex (player.playerIndex - 1);

        return newUIPlayer;
    }

    public void BeginGame () {
        NetworkPlayer.localPlayer.BeginGame ();
        lobbyCanvas.enabled = false;
    }

    public void SearchGame () {
        StartCoroutine (Searching ());
        SavePlayerName();
    }

    public void CancelSearchGame () {
        searching = false;
    }

    public void SearchGameSuccess (bool success, string matchID) {
        if (success) {
            searchCanvas.enabled = false;
            searching = false;
            JoinSuccess (success, matchID);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        connectCanvas.SetActive(false);
        lobbyCanvas.enabled = false;
    }


    IEnumerator Searching () {
        searchCanvas.enabled = true;
        searching = true;

        float searchInterval = 1;
        float currentTime = 1;

        while (searching) {
            if (currentTime > 0) {
                currentTime -= Time.deltaTime;
            } else {
                currentTime = searchInterval;
                NetworkPlayer.localPlayer.SearchGame ();
            }
            yield return null;
        }
        searchCanvas.enabled = false;
    }

}