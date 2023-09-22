using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace dragon.mirror
{
    [System.Serializable]
    public class PlayerSlotPair
    {
        public ScenePlayer player;
        public string slot;
        public PlayerSlotPair() { }
        public PlayerSlotPair(ScenePlayer player, string slot)
        {
            this.player = player;
            this.slot = slot;
        }
    }
    [System.Serializable]
    public class Match {
        public string matchID;
        public string matchNode;
        public bool publicMatch;
        public int matchSerial;
        public ScenePlayer matchLeader;
        public void SetMatchLeader(ScenePlayer leader){
            matchLeader = leader;
        }
        public ScenePlayer GetMatchLeader(){
            return matchLeader;
        }
        public void AddPlayerSlotPair(PlayerSlotPair newSlot){
            Debug.Log($"adding player slot {newSlot.player} player and {newSlot.slot} slot");
            if(!playerSlotPairs.Contains(newSlot)){
                playerSlotPairs.Add(newSlot);
            }
            //UpdatePlayerSlots();
        }
        public void RemovePlayerSlotPair(PlayerSlotPair removingSlot){
            Debug.Log($"removing player slot {removingSlot.player} player and {removingSlot.slot} slot");

            if(playerSlotPairs.Contains(removingSlot)){
                playerSlotPairs.Remove(removingSlot);
            }
            //UpdatePlayerSlots();
        }
        void UpdatePlayerSlots(){
            foreach(var player in players){
                player.currentMatch = this;
            }
        }
        public List<ScenePlayer> players = new List<ScenePlayer> ();
        public List<PlayerSlotPair> playerSlotPairs = new List<PlayerSlotPair>();
        public Match(string matchID, ScenePlayer player) {
            this.matchID = matchID;
            players.Add (player);
        }
        public Match () { }
    }
public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker instance;
    public static UnityEvent<ScenePlayer> playerToBeAdded = new UnityEvent<ScenePlayer>();
    public static UnityEvent<string, int, Match> CLEARTHEMATCH = new UnityEvent<string, int, Match>();
    public static UnityEvent<PlayerCharacter, ScenePlayer, Match, string> MakeCharacters = new UnityEvent<PlayerCharacter, ScenePlayer, Match, string>();
    public static UnityEvent<ChatManagerNode> MoveChatNodeOVM = new UnityEvent<ChatManagerNode>();
    public static UnityEvent<Match, TurnManager, PlayerCharacter> SendCuratorTMToPlayer = new UnityEvent<Match, TurnManager, PlayerCharacter>();
    [SerializeField] public static UnityEvent<MovingObject>  Energize = new UnityEvent<MovingObject>();
    public static UnityEvent<TurnManager, int, Match> moveTurnManager = new UnityEvent<TurnManager, int, Match>();
    public static UnityEvent<ChatManagerNode, int, Match> moveChatManagerNode = new UnityEvent<ChatManagerNode, int, Match>();
    public static UnityEvent<Mob, Match> moveMob = new UnityEvent<Mob, Match>();
    public static UnityEvent<ScenePlayer, float> EnterNodeCost = new UnityEvent<ScenePlayer, float>();
    public readonly SyncList<Match> matches = new SyncList<Match>();
    public readonly SyncList<String> matchIDs = new SyncList<String>();
    Dictionary<ChatManagerNode, Match> ChatManagerNodesDictionary = new Dictionary<ChatManagerNode, Match>();
    //ChatManagers
    [SerializeField] GameObject ChatNodePrefab;
    //Characters
    [SerializeField] GameObject turnManagerPrefab;
    // Generates new Vector2s in a grid-like formation around the given points of origin
    public Vector2[] GenerateVector2sR(Vector2[] pointsOfOrigin, int numToGenerate)
    {
        // Create a list to store the newly generated Vector2s
        List<Vector2> generatedVector2s = new List<Vector2>();
        // Create a variable to keep track of the total number of Vector2s generated
        int totalNumGenerated = 0;
        // Iterate through each point of origin
        foreach (Vector2 pointOfOrigin in pointsOfOrigin)
        {
            // Generate a random number of Vector2s to generate for this point of origin
            int numToGenerateForPoint = UnityEngine.Random.Range(1, 8);
            // Make sure that the total number of Vector2s generated is not greater than numToGenerate
            if (totalNumGenerated + numToGenerateForPoint > numToGenerate)
            {
                numToGenerateForPoint = numToGenerate - totalNumGenerated;
            }
            // Generate the new Vector2s in a grid-like formation around the point of origin
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Check if the new Vector2 is on the same coordinate as the point of origin
                    if (x == 0 && y == 0) continue;
                    // Add the new Vector2 to the list
                    generatedVector2s.Add(new Vector2(pointOfOrigin.x + x, pointOfOrigin.y + y));
                    // Increment the total number of Vector2s generated
                    totalNumGenerated++;
                    // Decrement the number of Vector2s left to generate for this point of origin
                    numToGenerateForPoint--;
                    // Break out of the loops if we have generated the desired number of Vector2s
                    if (numToGenerateForPoint == 0 || totalNumGenerated == numToGenerate) break;
                }
                    // Break out of the loop if we have generated the desired number of Vector2s
                    if (numToGenerateForPoint == 0 || totalNumGenerated == numToGenerate) break;
                }
            }
        // Check if the total number of Vector2s generated is less than numToGenerate
        while (totalNumGenerated < numToGenerate)
        {
            // Generate a new Vector2 at a random point of origin from the original list
            Vector2 newVector2 = new Vector2(pointsOfOrigin[UnityEngine.Random.Range(0, pointsOfOrigin.Length)].x, pointsOfOrigin[UnityEngine.Random.Range(0, pointsOfOrigin.Length)].y);
            // Add the new Vector2 to the list
            generatedVector2s.Add(newVector2);
            // Increment the total number of Vector2s generated
            totalNumGenerated++;
        }
        // Return the list of generated Vector2s as an array
        return generatedVector2s.ToArray();
    }
    private Dictionary<string, string> GetInspectedCharacterList(ScenePlayer inspectedPlayer, string id)
    {
        Dictionary<string, string> characterStats = new Dictionary<string, string>();
        foreach(var key in inspectedPlayer.GetInformationSheets()){
            if(key.CharacterID == id){
                foreach(var KVP in key.CharStatData){
                    characterStats.Add(KVP.Key, KVP.Value);
                }
            }
        }
        return characterStats;
    }
    public List<Vector2> GeneratePoints(ScenePlayer player, int numPoints)
    {
        float spacing = 1f;
        List<Vector2> points = new List<Vector2>();
        Vector2 startPos = player.gameObject.transform.position;
        startPos.y += 1; // move 1 unit up on the y-axis
        for (int i = 0; i < numPoints; i++)
        {
            points.Add(startPos);
            startPos.x += spacing;
        }
        return points;
    }
    bool AreTilesWithinOneHalf(Vector2 tile1, Vector2 tile2)
	{
	    float xDiff = Mathf.Abs(tile1.x - tile2.x);
	    float yDiff = Mathf.Abs(tile1.y - tile2.y);
	    return xDiff <= 1 && yDiff <= 1;
	}
    Mob GetRandomMob(Mob[] mobs)
    {
        return mobs[UnityEngine.Random.Range(0, mobs.Length)];
    }
    public Vector3 minPosition;
    public Vector3 maxPosition;
    void RandomVector3()
    {
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(minPosition.x, maxPosition.x), //x
            UnityEngine.Random.Range(minPosition.y, maxPosition.y), //y
            UnityEngine.Random.Range(minPosition.z, maxPosition.z)  //z
        );
        transform.position = randomPosition;
    }
    void Start()
    {
        instance = this;
    }
    public bool HostGame(string _matchID, ScenePlayer _player, bool publicMatch, out int playerIndex){
        playerIndex = -1;
        if (!matchIDs.Contains(_matchID)) {
            matchIDs.Add (_matchID);
            // Lets build the ChatNode
            GameObject newChatNode = Instantiate (ChatNodePrefab);
            NetworkServer.Spawn(newChatNode);
            print("Spawned new chat node");
            //we need to give chatNode a link to us with the match thats how we will control it maybe?
            Match match = new Match (_matchID, _player);
            match.publicMatch = publicMatch;
            match.matchNode = _player.currentNode;
            match.matchSerial = matches.Count;
            match.SetMatchLeader(_player);
            _player.inLobby = true;
            _player.inMatch = false; 
            _player.currentMatch = match;
            matches.Add (match);
            ChatManagerNode chatManager = newChatNode.GetComponent<ChatManagerNode>();
            chatManager.SetMatchSerial(match.matchSerial, match);
            chatManager.AddPlayer(_player);
            MoveChatNodeOVM.Invoke(chatManager);
            ChatManagerNodesDictionary.Add(chatManager, match);
            playerIndex = 1;
            Debug.Log($"Match generated");
            return true;
        } else {
            Debug.Log($"Match ID already exists");
            return false;
        }
    }
    public bool HostSoloGame(string _matchID, ScenePlayer _player, bool publicMatch, out int playerIndex){
        playerIndex = -1;
        if (!matchIDs.Contains(_matchID)) {
            matchIDs.Add (_matchID);
            Match match = new Match (_matchID, _player);
            match.publicMatch = publicMatch;
            match.matchNode = _player.currentNode;
            match.matchSerial = matches.Count;
            _player.inLobby = false;
            _player.inMatch = true;
            _player.currentMatch = match;
            playerIndex = 1;
            Debug.Log($"Match generated");
            foreach(var character in _player.GetParty()){
                
                PlayerSlotPair newPair = new PlayerSlotPair(_player, character);
                //match.playerSlotPairs.Add(newPair);
                match.AddPlayerSlotPair(newPair);
            }
            matches.Add (match);
            CreateSoloGame(_player, _matchID, match);
            return true;
        } else {
            Debug.Log($"Match ID already exists");
            return false;
        }
    }
    public void CreateSoloGame(ScenePlayer host, string _matchID, Match currentMatch){
        #if UNITY_SERVER
        PlayFabServer.instance.StartTheGameSolo(host, matches.Count, host.currentNode, currentMatch); 
        #endif
        //StartCoroutine(SetUpSoloMatch(host, _matchID, currentMatch));
    }
    public bool JoinGame(string _matchID, ScenePlayer _player, out int playerIndex, out ScenePlayer Host){
        playerIndex = -1;
        //ChatManagerNode chatNode = ChatManagerNodesDictionary
        int matchNumber = -1;
        if(matchIDs.Contains(_matchID)) {
            for (int i = 0; i < matches.Count; i++){
                if (matches[i].matchID == _matchID && matches[i].matchNode == _player.currentNode){
                    matchNumber = i;
                    matches[i].matchLeader.currentMatch.players.Add(_player);
                    _player.inLobby = true;
                    _player.currentMatch = matches[i].matchLeader.currentMatch;
                    playerIndex = matches[i].matchLeader.currentMatch.players.Count;
                    Match targetMatch = matches[i];
                    ChatManagerNode targetChatManagerNode = null;
                    foreach (KeyValuePair<ChatManagerNode, Match> entry in ChatManagerNodesDictionary)
                    {
                        if (entry.Value.Equals(targetMatch))
                        {
                            targetChatManagerNode = entry.Key;
                            break;
                        }
                    }
                    if (targetChatManagerNode != null)
                    {
                        targetChatManagerNode.AddPlayer(_player);
                        // Found the ChatManagerNode associated with the targetMatch
                    }
                    else
                    {
                        // ChatManagerNode not found for the targetMatch
                    }
                    StartCoroutine(SplitPacketForClear(matches[i].matchLeader.currentMatch));
                    foreach(var splayer in matches[i].matchLeader.currentMatch.players){
                        if(splayer == matches[i].matchLeader){
                            continue;
                        }
                        splayer.currentMatch = matches[i].matchLeader.currentMatch;
                    }
                    break;
                }
            }
            //for (int x = 0; x < matches.Count; x++){
            //    if (matches[x].matchID == _matchID){
            //        matches[x].playerSlotPairs.Clear();// Clear the playerSlotPairs list here                    // updates all the players to include the new player
            //        foreach(var sPlayer in matches[x].players){
            //            sPlayer.currentMatch = matches[x];
            //        }
            //        StartCoroutine(SplitPacketForClear(matches[x]));
            //        break;
            //    }
            //}
            Host = matches[matchNumber].matchLeader;
            Debug.Log($"Match joined");
            return true;
        } else {
            Debug.Log($"Match ID does not exist");
            Host = null;
            return false;
        }
    }
    IEnumerator SplitPacketForClear(Match match){
        yield return new WaitForEndOfFrame();
        foreach(var partyMember in match.players){
            List<SavedPartyList> savedParty = new List<SavedPartyList>();
            foreach(var member in partyMember.GetParty()){
                foreach(var sheet in partyMember.GetInformationSheets()){
                    if(sheet.CharacterID == member){
                        string charName = string.Empty;
                        string charSprite = string.Empty;
                        bool nameFound = false;
                        bool spriteFound = false;
                        foreach(var stat in sheet.CharStatData){
                            if(stat.Key == "CharName"){
                                charName = stat.Value;
                                nameFound = true;
                            }
                            if(stat.Key == "CharacterSprite"){
                                charSprite = stat.Value;
                                spriteFound = true;
                            }
                            if(nameFound && spriteFound){
                                break;
                            }
                        }
                        SavedPartyList KVP = (new SavedPartyList{
                            Key = charName,
                            Value = charSprite
                        });
                        savedParty.Add(KVP);
                    }
                }
            }
            ClientPartyInformation partyList = (new ClientPartyInformation
            {
                Party = savedParty,
                owner = partyMember
            });
            partyMember.RpcBuildPartyInspector(partyList);
            partyMember.RpcSpawnPlayerUI(partyMember.loadSprite);
        }
    }
    public bool SearchGame(ScenePlayer _player, out int playerIndex, out string matchID){
        playerIndex = -1;
        matchID = string.Empty;
        //bool partyLead = false;
        for (int i = 0; i < matches.Count; i++) {
            if (matches[i].players.Count < 6 && matches[i].matchNode == _player.currentNode && matches[i].GetMatchLeader().inLobby){
                matchID = matches[i].matchID;
                if(JoinGame(matchID, _player, out playerIndex, out ScenePlayer Host)) {
                    return true;
                } 
            }
        }
        return false;
    }
    [Server]
    public void RemoveCharacterVote(ScenePlayer sPlayer, string characterSlot, Match match){
        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == match.matchID && matches[i].matchNode == sPlayer.currentNode){
                PlayerSlotPair pairToRemove = matches[i].matchLeader.currentMatch.playerSlotPairs.FirstOrDefault(pair => pair.player == sPlayer && pair.slot == characterSlot);
                // If pairToRemove is not null, remove the pair and notify the players
                if(pairToRemove != null){
                    // Remove the pair
                    matches[i].matchLeader.currentMatch.playerSlotPairs.Remove(pairToRemove);
                    // Send updates to each player
                    foreach(var player in match.players){
                        player.TargetRemoveAdventurer(sPlayer, characterSlot);
                        if(player != matches[i].matchLeader){
                            player.currentMatch = matches[i].matchLeader.currentMatch;  // Manually update currentMatch to trigger a sync
                        }
                    }
                }
            }
        }
        /*
        foreach(var pairkey in match.playerSlotPairs){
            if(pairkey.player == sPlayer && pairkey.slot == characterSlot){
                foreach(var player in match.players){
                    player.TargetRemoveAdventurer(sPlayer, characterSlot);
                }
                //match.playerSlotPairs.Remove(pairkey);
                match.RemovePlayerSlotPair(pairkey);

            }
        }
        PlayerSlotPair pairToRemove = match.playerSlotPairs.FirstOrDefault(pair => pair.player == sPlayer && pair.slot == characterSlot);
        // If pairToRemove is not null, remove the pair and notify the players
        if(pairToRemove != null){
            // Remove the pair
            match.RemovePlayerSlotPair(pairToRemove);
            // Send updates to each player
            foreach(var player in match.players){
                player.TargetRemoveAdventurer(sPlayer, characterSlot);
                player.currentMatch = match;  // Manually update currentMatch to trigger a sync
            }
        }
        */

    }
    [Server]
    public void CastCharacterVote(ScenePlayer sPlayer, string characterSlot, Match match, string charName, string spriteName)
    {
        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == match.matchID && matches[i].matchNode == sPlayer.currentNode){
                int index = sPlayer.playerIndex;
                int activeCharCount = 0;
                int allowedCharAmount = 0;
                foreach (PlayerSlotPair pair in matches[i].matchLeader.currentMatch.playerSlotPairs)
                {
                    if (pair.player == sPlayer)
                    {
                        activeCharCount++;
                        // Check if the characterSlot already exists for this sPlayer
                        if (pair.slot == characterSlot)
                        {
                            return;
                        }
                    }
                }
                int playerCount = match.players.Count;
                if (playerCount == 1)
                {
                    allowedCharAmount = 6;
                }
                else if (playerCount == 2)
                {
                    allowedCharAmount = 3;
                }
                else if (playerCount == 3)
                {
                    allowedCharAmount = 2;
                }
                else if (playerCount == 4)
                {
                    if (index == 1 || index == 2)
                    {
                        allowedCharAmount = 2;
                    }
                    else
                    {
                        allowedCharAmount = 1;
                    }
                }
                else if (playerCount == 5)
                {
                    if (index == 1)
                    {
                        allowedCharAmount = 2;
                    }
                    else
                    {
                        allowedCharAmount = 1;
                    }
                }
                else if (playerCount == 6)
                {
                    allowedCharAmount = 1;
                }
                if (activeCharCount < allowedCharAmount)
                {
                    PlayerSlotPair newPair = new PlayerSlotPair(sPlayer, characterSlot);

                    matches[i].matchLeader.currentMatch.playerSlotPairs.Add(newPair);
                    //match.playerSlotPairs.Add(newPair);
                    foreach(var activePlayer in matches[i].matchLeader.currentMatch.players){
                        if(activePlayer == matches[i].matchLeader){
                            activePlayer.TargetSpawnAdventurer(newPair.player, charName, spriteName, characterSlot);
                            continue;
                        }
                        activePlayer.currentMatch = matches[i].matchLeader.currentMatch;
                        activePlayer.TargetSpawnAdventurer(newPair.player, charName, spriteName, characterSlot);
                        print($"{charName}, {spriteName}");
                    }
                }
                    }
                }
        /*
        int index = sPlayer.playerIndex;
        int activeCharCount = 0;
        int allowedCharAmount = 0;
        foreach (PlayerSlotPair pair in match.playerSlotPairs)
        {
            if (pair.player == sPlayer)
            {
                activeCharCount++;
                // Check if the characterSlot already exists for this sPlayer
                if (pair.slot == characterSlot)
                {
                    return;
                }
            }
        }
        int playerCount = match.players.Count;
        if (playerCount == 1)
        {
            allowedCharAmount = 6;
        }
        else if (playerCount == 2)
        {
            allowedCharAmount = 3;
        }
        else if (playerCount == 3)
        {
            allowedCharAmount = 2;
        }
        else if (playerCount == 4)
        {
            if (index == 1 || index == 2)
            {
                allowedCharAmount = 2;
            }
            else
            {
                allowedCharAmount = 1;
            }
        }
        else if (playerCount == 5)
        {
            if (index == 1)
            {
                allowedCharAmount = 2;
            }
            else
            {
                allowedCharAmount = 1;
            }
        }
        else if (playerCount == 6)
        {
            allowedCharAmount = 1;
        }
        if (activeCharCount < allowedCharAmount)
        {
            PlayerSlotPair newPair = new PlayerSlotPair(sPlayer, characterSlot);
            
            matches[i].AddPlayerSlotPair(newPair);
            match.AddPlayerSlotPair(newPair);
            //match.playerSlotPairs.Add(newPair);
            foreach(var activePlayer in match.players){
                activePlayer.TargetSpawnAdventurer(newPair.player, charName, spriteName, characterSlot);
            }
        }
        */
    }
    public ChatManagerNode GetChatManagerNodeByMatch(Match match)
    {
        foreach (var pair in ChatManagerNodesDictionary)
        {
            if (pair.Value == match)
            {
                return pair.Key;
            }
        }
        print("Did not find a chat manager this is null ************ DOINK!!!!!!!!!!");
        return null;
    }
    public void CreateGame(ScenePlayer host, string _matchID, Match currentMatch){
        List<ScenePlayer> playerScripts = new List<ScenePlayer>();
        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == _matchID){
                foreach(var _player in matches[i].players){
                    _player.inLobby = false;
                    _player.inMatch = true;
                    //_player.currentMatch = match;
                    EnterNodeCost.Invoke(_player, 50f);
                    _player.TargetGetReadyForStart();
                    playerScripts.Add(_player);
                }
            }
        }
        #if UNITY_SERVER
        PlayFabServer.instance.StartTheGame(matches.Count, host.currentNode, currentMatch, host, _matchID, playerScripts); 
        #endif
        //StartCoroutine(SetUpMatch(host, _matchID, currentMatch, playerScripts));
        Debug.Log($"Game Beginning CREATEGAME ************");

    }
    IEnumerator EnergizeThem(TurnManager tmCurator){
        yield return new WaitForSeconds(5f);
        List<PlayerCharacter> PCs = tmCurator.GetPCList();
        foreach(var pc in PCs){
            Energize.Invoke(pc);
        }
        List<Mob> Mobs = tmCurator.GetENEMYList();
        foreach(var mob in Mobs){
            Energize.Invoke(mob);
        }
        foreach(var player in tmCurator.GetPlayers()){
            player.TargetSendMobList(Mobs);
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
    public void PlayerDisconnectedFromLobby(ScenePlayer player, string _matchID){
        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == _matchID){
                Match targetMatch = matches[i];
                ChatManagerNode targetChatManagerNode = null;
                foreach (KeyValuePair<ChatManagerNode, Match> entry in ChatManagerNodesDictionary)
                {
                    if (entry.Value.Equals(targetMatch))
                    {
                        targetChatManagerNode = entry.Key;
                        break;
                    }
                }
                if (targetChatManagerNode != null)
                {
                    targetChatManagerNode.RemovePlayer(player);
                    if(targetChatManagerNode.partyPlayers.Count == 0){
                        Destroy(targetChatManagerNode.gameObject);
                    }
                    // Found the ChatManagerNode associated with the targetMatch
                }
                else
                {
                    // ChatManagerNode not found for the targetMatch
                }
                int playerIndex = matches[i].players.IndexOf(player);
                matches[i].players.RemoveAt(playerIndex);
                Debug.Log($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");
                //player.currentMatch = null;
                Match match;
                bool isLeader = false;
                if(player == player.currentMatch.GetMatchLeader()){
                    isLeader = true;
                }
                matches[i].playerSlotPairs.Clear();
                player.currentMatch.playerSlotPairs.Clear(); 
                if(matches[i].players.Count == 0) {
                    Debug.Log($"No more players in Match. Terminating {_matchID}");
                    match = matches[i];
                    matches.RemoveAt(i);
                    matchIDs.Remove(_matchID);
                    match = null;
                    player.inLobby = false;
                    player.inMatch = false;
                } else{
                    Debug.Log($"Trying to pass the match leader");
                    if(isLeader){
                        foreach(var splayer in player.currentMatch.players){
                            if(splayer == player){
                                continue;
                            }
                            splayer.currentMatch = player.currentMatch;
                            splayer.currentMatch.matchLeader = splayer;
                            matches.RemoveAt(i);
                            matches.Add(splayer.currentMatch);
                            splayer.TargetPassLeader(true);
                            Debug.Log($"Passing match leader to player {splayer.playerName} ");
                            break;
                        }

                    }
                    //foreach(var ssPlayer in matches[i].players){
                    //    ssPlayer.TargetDisconnectFromMatchLobby(player);
                    //}
                }
                    player.rpcDisconnectFromMatchLobby();

                player.currentMatch = null;

                break;     
            }
        }
    }
    public void PlayerDisconnected (ScenePlayer player, string _matchID){
        if(player.currentMatch == null)
        {
            return;
        }
        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == _matchID){
                Match targetMatch = matches[i];
                ChatManagerNode targetChatManagerNode = null;
                foreach (KeyValuePair<ChatManagerNode, Match> entry in ChatManagerNodesDictionary)
                {
                    if (entry.Value.Equals(targetMatch))
                    {
                        targetChatManagerNode = entry.Key;
                        break;
                    }
                }
                if (targetChatManagerNode != null)
                {
                    targetChatManagerNode.RemovePlayer(player);
                    if(targetChatManagerNode.partyPlayers.Count == 0){
                        Destroy(targetChatManagerNode.gameObject);
                    }
                    // Found the ChatManagerNode associated with the targetMatch
                }
                else
                {
                    // ChatManagerNode not found for the targetMatch
                }
                int playerIndex = matches[i].players.IndexOf(player);
                matches[i].players.RemoveAt(playerIndex);
                print($"{playerIndex} is the players index that was removed");
                Debug.Log($"Player disconnected from match {_matchID} | {matches[i].players.Count} players remaining");
                bool isLeader = false;
                if(player == player.currentMatch.GetMatchLeader()){
                    isLeader = true;
                }
                if(matches[i].players.Count == 0) {
                    int serial = matches[i].matchSerial;
                    if(player.inMatch){
                        CLEARTHEMATCH.Invoke(_matchID, player.currentMatch.matchSerial, matches[i]);
                    }
                    Debug.Log($"No more players in Match. Terminating {_matchID}");
                    matches.RemoveAt(i);
                    matchIDs.Remove(_matchID);
                } else{
                    Debug.Log($"Trying to pass the match leader");
                    if(isLeader){
                        int leaderIndex = matches[i].players.Count;
                        int randomLeader = UnityEngine.Random.Range(0, leaderIndex);
                        ScenePlayer newLeader = matches[i].players[randomLeader].GetComponent<ScenePlayer>();
                        print($"Player: {newLeader.playerName} , is now the party leader out of {leaderIndex} players in the match: {_matchID}");
                        matches[i].SetMatchLeader(newLeader);
                        Debug.Log($"Passing match leader to player {newLeader.playerName} ");
                    }
                }   
            }
        }
    }
    /*
    public void FinishedMatch (Match match){
        print("Got to Finished Match in matchmaker call");
        if(!matches.Contains(match))
        {
            return;
        }
        for (int i = 0; i < matches.Count; i++){
            if (matches[i].matchID == match.matchID){
                //player.currentMatch.inMatch = false;
                foreach(var playerP in matches[i].players){
                    playerP.currentMatch = null;
                }
                int serial = matches[i].matchSerial;
                matchIDs.Remove(match.matchID);
                matches.RemoveAt(i);
                match = null;
                return;
            }
        }
    }
    */
    public void FinishedMatch (Match match, List<ScenePlayer> players) {
    print("Got to Finished Match in matchmaker call");
    
    // Check if match is null
    if (match == null) {
        Debug.LogError("Match is null");
        return;
    }
    
    if (!matches.Contains(match)) {
        return;
    }

    for (int i = 0; i < matches.Count; i++) {
        // Check if matches[i] is null
        if (matches[i] == null) {
            Debug.LogError("matches[" + i + "] is null");
            continue;
        }
        
        if (matches[i].matchID == match.matchID) {
            // Check if matches[i].players is null
            foreach (var playerP in players) {
                // Check if playerP is null
                if (playerP == null) {
                    Debug.LogError("playerP is null");
                    continue;
                }
                playerP.currentMatch = null;
            }
            matchIDs.Remove(match.matchID);
            matches.RemoveAt(i);
            match = null;
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
}
