using System;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;


[System.Serializable]
public class Match {
    public string matchID;
    public bool publicMatch;
    public bool inMatch;
    public bool matchFull;
    public SyncListGameObject players = new SyncListGameObject ();

    public Match (string matchID, GameObject player, bool publicMatch) {
        matchFull = false;
        inMatch = false;
        this.matchID = matchID;
        this.publicMatch = publicMatch;
        players.Add (player);
    }

    public Match () { }
}

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> { }

[System.Serializable]
public class SyncListMatch : SyncList<Match> { }

public class MatchMaker : NetworkBehaviour {

    public GameObject orbPrefab;
    public static GameObject pointOrb_prefab;
    public static MatchMaker instance;
    public GameObject prefab_angel;

    public SyncListMatch matches = new SyncListMatch ();
    public SyncListString matchIDs = new SyncListString ();

    [SerializeField] int maxMatchPlayers = 12;

    void Start () {
        pointOrb_prefab = orbPrefab;
        instance = this;

    }

    public bool HostGame (string _matchID, GameObject _player, bool publicMatch, out int playerIndex) {
        playerIndex = -1;

        if (!matchIDs.Contains (_matchID)) {
            matchIDs.Add (_matchID);
            Match match = new Match (_matchID, _player, publicMatch);
            matches.Add (match);
            Debug.Log ($"Match generated");
            _player.GetComponent<NetworkPlayer> ().currentMatch = match;
            //_player.GetComponent<NetworkPlayer> ().playerName = PlayerPrefs.GetString("PlayerName");
            playerIndex = 1;
            return true;
        } else {
            Debug.Log ($"Match ID already exists");
            return false;
        }
    }

    public bool JoinGame (string _matchID, GameObject _player, out int playerIndex) {
        playerIndex = -1;

        if (matchIDs.Contains (_matchID)) {

            for (int i = 0; i < matches.Count; i++) {
                if (matches[i].matchID == _matchID) {
                    if (!matches[i].inMatch && !matches[i].matchFull) {
                        matches[i].players.Add (_player);
                        _player.GetComponent<NetworkPlayer> ().currentMatch = matches[i];
                        //_player.GetComponent<NetworkPlayer> ().playerName = PlayerPrefs.GetString("PlayerName");

                        playerIndex = matches[i].players.Count; 

                        if (matches[i].players.Count == maxMatchPlayers) {
                            matches[i].matchFull = true;
                        }

                        break;
                    } else {
                        return false;
                    }
                }
            }

            Debug.Log ($"Match joined");
            return true;
        } else {
            Debug.Log ($"Match ID does not exist");
            return false;
        }
    }

    public bool SearchGame (GameObject _player, out int playerIndex, out string matchID) {
        playerIndex = -1;
        matchID = "";

        for (int i = 0; i < matches.Count; i++) {
            Debug.Log ($"Checking match {matches[i].matchID} | inMatch {matches[i].inMatch} | matchFull {matches[i].matchFull} | publicMatch {matches[i].publicMatch}");
            if (!matches[i].inMatch && !matches[i].matchFull && matches[i].publicMatch) {
                if (JoinGame (matches[i].matchID, _player, out playerIndex)) {
                    matchID = matches[i].matchID;
                    return true;
                }
            }
        }

        return false;
    }

    public void BeginGame (string _matchID) {
        GameObject newAngel = (GameObject)Instantiate(prefab_angel);
        newAngel.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();

        for (int i = 0; i < matches.Count; i++) {
            if (matches[i].matchID == _matchID) {
                newAngel.GetComponent<WingControl>().currentMatch = matches[i];
                matches[i].inMatch = true;

                foreach (var player in matches[i].players) {
                    NetworkPlayer _player = player.GetComponent<NetworkPlayer> ();
                    newAngel.GetComponent<WingControl>().players.Add(player.GetComponent<NetworkPlayer>());
                    _player.StartGame ();
                }
                break;
            }
        }
        NetworkServer.Spawn(newAngel);

    }

    public static string GetRandomMatchID () {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++) {
            int random = UnityEngine.Random.Range (0, 36);
            if (random < 26) {
                _id += (char) (random + 65);
            } else {
                _id += (random - 26).ToString ();
            }
        }
        Debug.Log ($"Random Match ID: {_id}");
        return _id;
    }

    public void PlayerDisconnected (NetworkPlayer player, string _matchID) {
        for (int i = 0; i < matches.Count; i++) {
            if (matches[i].matchID == _matchID) {
                int playerIndex = matches[i].players.IndexOf (player.gameObject);
                matches[i].players.RemoveAt (playerIndex);
                Debug.Log ($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

                if (matches[i].players.Count == 0) {
                    Debug.Log ($"No more players in Match. Terminating {_matchID}");
                    matches.RemoveAt (i);
                    matchIDs.Remove (_matchID);
                }
                break;
            }
        }
    }


}

public static class MatchExtensions {
    public static Guid ToGuid (this string id) {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider ();
        byte[] inputBytes = Encoding.Default.GetBytes (id);
        byte[] hashBytes = provider.ComputeHash (inputBytes);

        return new Guid (hashBytes);
    }
}


