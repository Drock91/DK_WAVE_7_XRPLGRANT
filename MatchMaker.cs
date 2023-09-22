using System.Security.Cryptography;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


    [System.Serializable]
    public class Match {
        public string matchID;
        public bool publicMatch;
        public bool inMatch;
        public bool matchFull;


        public List<GameObject> players = new List<GameObject> ();
        public Match(string matchID, GameObject player) {
            this.matchID = matchID;
            players.Add (player);
        }
        public Match () { }
    }
public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker instance;

    public SyncList<Match> matches = new SyncList<Match>();
    public SyncList<String> matchIDs = new SyncList<String>();
    [SerializeField] GameObject turnManagerPrefab;
    public string subScene = "Game";

    void Start()
    {
        instance = this;
    }
    public bool HostGame(string _matchID, GameObject _player, bool publicMatch, out int playerIndex){
        playerIndex = -1;
      
        if (!matchIDs.Contains(_matchID)) {
            matchIDs.Add (_matchID);
            Match match = new Match (_matchID, _player);
            match.publicMatch = publicMatch;
            matches.Add (match);
            Debug.Log($"Match generated");
            _player.GetComponent<PlayerP>().currentMatch = match;
            
            //current match stores the player and _match ID we might be able to pass the player name from there getting close dont give up!!

            playerIndex = 1;
            return true;
        } else {
            Debug.Log($"Match ID already exists");
            return false;
        }
    }

    
    public bool JoinGame(string _matchID, GameObject _player, out int playerIndex){
        playerIndex = -1;
        if(matchIDs.Contains(_matchID)) {

            for (int i = 0; i < matches.Count; i++)
                if (matches[i].matchID == _matchID){
                    matches[i].players.Add(_player);
                    _player.GetComponent<PlayerP>().currentMatch = matches[i];
                    playerIndex = matches[i].players.Count;
                    break;
                }
        
        Debug.Log($"Match joined");
        return true;
        } else {
            Debug.Log($"Match ID does not exist");
            return false;

        }
    }
    public bool SearchGame(GameObject _player, out int playerIndex, out string matchID){
        playerIndex = -1;
        matchID = string.Empty;
        for (int i = 0; i < matches.Count; i++) {
            if (matches[i].publicMatch && !matches[i].matchFull && !matches[i].inMatch){
                matchID = matches[i].matchID;
                if(JoinGame(matchID, _player, out playerIndex)) {
                    return true;
                } 
            }
        }
        return false;
    }

    public void BeginGame(string _matchID){
        GameObject newTurnManager = Instantiate (turnManagerPrefab);
        NetworkServer.Spawn(newTurnManager, connectionToServer);
        newTurnManager.GetComponent<NetworkMatch>().matchId = _matchID.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();
        //UILobby.instance.lobbyCanvas.enabled = false;

        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == _matchID){
                foreach(var player in matches[i].players){
                    PlayerP _player = player.GetComponent<PlayerP>();
                    
                    
                    turnManager.AddPlayer(_player);
                    
                    _player.StartGame();
                }
                break;
            }
        }

    }
    public static string GetRandomMatchID(){
        string _id = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0,36);
            if (random < 26){
                _id += (char)(random + 65);
            } else {
                _id += (random - 26).ToString();
            }
        }
        Debug.Log($"Random Match ID: {_id}");
        return _id;
    }
    public void PlayerDisconnected (PlayerP player, string _matchID){
        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == _matchID){
                int playerIndex = matches[i].players.IndexOf(player.gameObject);
                matches[i].players.RemoveAt(playerIndex);
                Debug.Log($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");

                if(matches[i].players.Count == 0) {
                    Debug.Log($"No more players in Match. Terminating {_matchID}");
                    matches.RemoveAt(i);
                    matchIDs.Remove(_matchID);
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
        byte[] hashBytes = provider.ComputeHash(inputBytes);
        return new Guid (hashBytes);
    }
}

