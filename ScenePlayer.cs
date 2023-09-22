using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;
using System.Text.RegularExpressions;
using UnityEngine.Tilemaps;

namespace dragon.mirror{
    public class ScenePlayer : NetworkBehaviour
{
    [SerializeField] GameObject rightClickPrefab;
    bool playerOVMReady = false;
    //INVENTORY CODE
    //ENDINVENTORYCODE
    public static UnityEvent NewWave = new UnityEvent();
    public static UnityEvent<List<Vector2>> BlockedWave = new UnityEvent<List<Vector2>>();
    [SerializeField] public static ScenePlayer localPlayer;
    [SerializeField] public static UnityEvent<ScenePlayer>  ClearSelectedTiles = new UnityEvent<ScenePlayer>();
    [SerializeField] public static UnityEvent<ScenePlayer>  SendUnselectedTarget = new UnityEvent<ScenePlayer>();
    [SerializeField] public static UnityEvent<MovingObject>  CancelCast = new UnityEvent<MovingObject>();
    [SerializeField] public static UnityEvent<Match>  PermissionToFinish = new UnityEvent<Match>();
    [SerializeField] public static UnityEvent  SendPlayers = new UnityEvent();
    [SerializeField] public static UnityEvent<PlayerCharacter> BuildCombatPlayerUI = new UnityEvent<PlayerCharacter>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, string, string, string> SpellChange = new UnityEvent<NetworkConnectionToClient, string, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, LearnSpell, string> SpellPurchase = new UnityEvent<NetworkConnectionToClient, LearnSpell, string>();
    [SerializeField] public static UnityEvent<string, string> MoveRequest = new UnityEvent<string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, string, string, string> ServerCharacterBuildRequest = new UnityEvent<NetworkConnectionToClient, string, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, string> FinalRequest = new UnityEvent<NetworkConnectionToClient, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, string> ResetOVM = new UnityEvent<NetworkConnectionToClient, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, string> PartyRemoval = new UnityEvent<NetworkConnectionToClient, string>();
    [SerializeField] public static UnityEvent<string, string, string, ScenePlayer> VoteNeed = new UnityEvent<string, string, string, ScenePlayer>();
    [SerializeField] public static UnityEvent<string, string, string, ScenePlayer> VoteGreed = new UnityEvent<string, string, string, ScenePlayer>();

    [SerializeField] public static UnityEvent<string, string, ScenePlayer> VotePass = new UnityEvent<string, string, ScenePlayer>();
    [SerializeField] public static UnityEvent<string> OVMRequest = new UnityEvent<string>();
    [SerializeField] public static UnityEvent<float> UIToggle = new UnityEvent<float>();
    [SerializeField] public static UnityEvent MapOn = new UnityEvent();
    [SerializeField] public static UnityEvent MapOff = new UnityEvent();
    [SerializeField] public static UnityEvent NewTarget = new UnityEvent();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, float> EnergyUpdate = new UnityEvent<NetworkConnectionToClient, float>();
    [SerializeField] public static UnityEvent EnteringTown = new UnityEvent();
    [SerializeField] public static UnityEvent LeavingTown = new UnityEvent();
    [SerializeField] public static UnityEvent OpenWalletBuilder = new UnityEvent();
    [SerializeField] public static UnityEvent BuildInventories = new UnityEvent();
    [SerializeField] public static UnityEvent<string>  OnUI = new UnityEvent<string> ();
    [SerializeField] public static UnityEvent WalletAwake = new UnityEvent ();
    [SerializeField] public static UnityEvent walletTransmute = new UnityEvent ();
    [SerializeField] public static UnityEvent<string, float, float>  TriggerAsk = new UnityEvent<string, float, float> ();
    //[SerializeField] public static UnityEvent<NetworkConnectionToClient, string, float, float>  GoodbyeForNow = new UnityEvent<NetworkConnectionToClient, string, float, float> ();
    //[SerializeField] public static UnityEvent ReopenConnect = new UnityEvent();
    //[SerializeField] public static UnityEvent ReopenTown = new UnityEvent();
    [SerializeField] public static UnityEvent<ScenePlayer> RemoveLobby = new UnityEvent<ScenePlayer>();
    [SerializeField] public static UnityEvent BeginGameClearLobby = new UnityEvent();
    [SerializeField] public static UnityEvent PlayerDisconnectedFromMatchLobby = new UnityEvent();
    [SerializeField] public static UnityEvent<ScenePlayer, string> RemoveAdventurer = new UnityEvent<ScenePlayer, string>();
    [SerializeField] public static UnityEvent<List<Mob>> SendFogMobs = new UnityEvent<List<Mob>>();
    [SerializeField] public bool menuOpened = false;
    public static UnityEvent<NetworkConnectionToClient, string, string, string> OnPlayerDataUpdateRequest = new UnityEvent<NetworkConnectionToClient, string, string, string>();
    public static UnityEvent<NetworkConnectionToClient, string, string> LevelUpStarted = new UnityEvent<NetworkConnectionToClient, string, string>();
    public static UnityEvent<NetworkConnectionToClient, string, string> LevelUpEnded = new UnityEvent<NetworkConnectionToClient, string, string>();

    public static event Action LevelUpEndedSound;
    public static event Action LevelUpStartedSound;
    public static UnityEvent<NetworkConnectionToClient> LogoutPlayer = new UnityEvent<NetworkConnectionToClient>();
    [SerializeField] public static UnityEvent innReset = new UnityEvent();

    [SerializeField] public static UnityEvent TargetHighlightReset = new UnityEvent();
    public static UnityEvent<NetworkConnectionToClient, StackingMessage> StackingItem = new UnityEvent<NetworkConnectionToClient, StackingMessage>();

    public static UnityEvent<NetworkConnectionToClient> HealPartyServer = new UnityEvent<NetworkConnectionToClient>();
    public static UnityEvent<NetworkConnectionToClient, string> ResCharacter = new UnityEvent<NetworkConnectionToClient, string>();

    public static event Action<string> LeftTownOpenNode;
    public static event Action PurchaseButtonAvailable;
    string charliesTicket; 
    float requirement = 100f;
    Transform transformProcessing;
    bool processingMoveRequest;
    //****************************
    //***********Energy***********
    //****************************
    [SerializeField] public static UnityEvent<float> Charging = new UnityEvent<float>();
    [SyncVar] [SerializeField] public float Energy;
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,uint,string> ServerTransmitTX = new UnityEvent<NetworkConnectionToClient,uint,string>();
    [SerializeField] public static UnityEvent ToggleEndMatch = new UnityEvent();
    [SerializeField] public static UnityEvent ToggleCloseEndMatch = new UnityEvent();
    [SerializeField] public static UnityEvent ToggleSpellsOff = new UnityEvent();
    [SerializeField] public static UnityEvent LoadbarOnToggle = new UnityEvent();
    [SerializeField] public static UnityEvent LoadbarOffToggle = new UnityEvent();
    [SerializeField] public static UnityEvent<string> FinalWeight = new UnityEvent<string>();
    //Tactician Inventory
    [SerializeField] public static UnityEvent ContentSizer = new UnityEvent();
    [SerializeField] public static UnityEvent<CharacterInventoryListItem> PurchasedItem = new UnityEvent<CharacterInventoryListItem>();
    [SerializeField] public static UnityEvent<CharacterInventoryListItem> PickedUpItemTactician = new UnityEvent<CharacterInventoryListItem>();
    [SerializeField] public static UnityEvent<CharacterInventoryListItem, string, string> PickedUpItemCharacter = new UnityEvent<CharacterInventoryListItem, string, string>();

    [SerializeField] public static UnityEvent<ItemSelectable> StaffBuild = new UnityEvent<ItemSelectable>();
    [SerializeField] public static UnityEvent<string, int, string> BuildingItemDrop = new UnityEvent<string, int, string>();
    //trading server calls
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> StashToTactInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> StashToTactEquipped = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> StashToTactSafetyBelt = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> StashToCharInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData> StashToCharEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> TactInvToTactEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> TactInvToTactBelt = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> TactBeltToTactEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> TactBeltToTactInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> TactEquipToTactInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> TactEquipToTactEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> TactEquipToTactBelt = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> TactEquipToCharInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> TactInvToCharInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData> TactInvToCharEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> TactSafetyBeltToCharInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData> TactSafetyBeltToCharEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> TactInvToStash = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> TactEquipToStash = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> TactBeltToStash = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> CharInvToTactInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> CharEquipToTactInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> CharInvToTactBelt = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> CharEquipToTactBelt = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string> ServerDestroyItem = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData> CharInvToTactEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> CharInvToStash = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string> CharEquipToStash = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string, string>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData> CharInvToCharInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData> CharInvToCharEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData> CharEquipToCharInv = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData> CharEquipToCharEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData> CharEquipToInvSame = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData> CharInvToEquipSame = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData> CharEquipToEquipSame = new UnityEvent<NetworkConnectionToClient,ItemSelectable, string ,EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, ItemSelectable, EquippingData> CharOneUnequipToCharEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, ItemSelectable, EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,ItemSelectable, ItemSelectable, EquippingData> CharTwoUnequipToCharEquip = new UnityEvent<NetworkConnectionToClient,ItemSelectable, ItemSelectable, EquippingData>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient,string, string, EquippingData> CharUnequipTactToCharEquip = new UnityEvent<NetworkConnectionToClient,string, string, EquippingData>();
    
    [SerializeField] public static UnityEvent<NewStackCreated> BuildStackableItem = new UnityEvent<NewStackCreated>();
    [SerializeField] public static UnityEvent<NetworkConnectionToClient, string> SendParty = new UnityEvent<NetworkConnectionToClient, string>();
    [SerializeField] public static UnityEvent GetCharacters = new UnityEvent();
    [SerializeField] public static UnityEvent<string> BuildItems = new UnityEvent<string>();
    [SerializeField] public static UnityEvent PartySpawned = new UnityEvent();
    [SerializeField] public static UnityEvent PartyRefresh = new UnityEvent();
    [SerializeField] public static UnityEvent ResetSpring = new UnityEvent();
    [SerializeField] public static UnityEvent<ScenePlayer, string> RefreshSheets = new UnityEvent<ScenePlayer, string>();
    [SerializeField] public static UnityEvent<ScenePlayer, string> RefreshEXP = new UnityEvent<ScenePlayer, string>();
    [SerializeField] public static UnityEvent<ScenePlayer, string> Ressurected = new UnityEvent<ScenePlayer, string>();

    [SerializeField] public static UnityEvent<string, string> RefreshAbilityPage = new UnityEvent<string, string>();
    [SerializeField] public static UnityEvent<ItemSelectable> ResetItemSelectable = new UnityEvent<ItemSelectable>();
    [SerializeField] public static UnityEvent<ScenePlayer, string> DeathBroadcast = new UnityEvent<ScenePlayer, string>();


    //[SerializeField] public static UnityEvent RefreshTactician = new UnityEvent();
    [SerializeField] public static UnityEvent<ScenePlayer, ItemSelectable> Refreshitem = new UnityEvent<ScenePlayer, ItemSelectable>();

    [SerializeField] public static UnityEvent<ScenePlayer, ItemSelectable> DestroyInventoryItem = new UnityEvent<ScenePlayer, ItemSelectable>();

    //[SerializeField] public static UnityEvent<string, string> RebuildItems = new UnityEvent<string, string>();
    [SerializeField] public static UnityEvent<string> ImproperCheckText = new UnityEvent<string>();
    //Swap spells
    [SerializeField] public static UnityEvent<string, ScenePlayer, SendSpellList> ChangingMOSpellsMatch = new UnityEvent<string, ScenePlayer, SendSpellList>();
    
    [SyncVar] public int TokenCount;
    //[SyncVar] public string TacticianName;
    public static UnityEvent ResetTokens = new UnityEvent();
    //****************************
    //***********Characters*******
    //****************************
    [SyncVar] [SerializeField] public long Gold;
    [SyncVar] 
    [SerializeField] public string matchID;
    [SyncVar] 
    [SerializeField] public int playerIndex;
    [SyncVar]
    [SerializeField] public Match currentMatch;
    [SyncVar]
    [SerializeField] public bool inMatch = false;
    [SyncVar]
    [SerializeField] public bool inLobby = false;
    [SyncVar]
    [SerializeField] public string playerName;
    [SerializeField] public string loadSprite;
    Transform cameraController;
    [SyncVar]
    public bool LerpInProgress = false;
    [SyncVar]
    public bool justSpawned = true;
    [SyncVar]
    public string currentScene;
    [SyncVar]
    public string currentNode;
    [SyncVar]
    public string travelNode;
    public SceneNode OurNode;
    //event for opening combat UI
    [SerializeField] public static UnityEvent<string> OpenCharSheetOneCombat = new UnityEvent<string>();
    [SerializeField] public static UnityEvent<string> OpenCharSheetTwoCombat = new UnityEvent<string>();
    public static event Action<Vector2, List<MovingObject>, Match> OnCharactersMoved;
    public static event Action<MovingObject, List<MovingObject>, Match> OnCharactersFollow;
    public static event Action<MovingObject, List<MovingObject>, Match> OnCharactersAttack;
    public static event Action<GameObject> selectedCharacterHighlight;

    
    public float timerTime;
    Vector2 travelTarget;
    [SerializeField] GameObject playerLobbyUI;
    string tempName;
    bool rejected = false;
    bool waitingAcceptance = true;
    bool acceptedPayment = false;
    public bool ableToClick = true;
    bool noWasClicked = false;
    public GameObject tilePrefab;
    //Movement
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private bool Loading = true;
    [SerializeField] GameObject castBarPrefab;
    [SerializeField] GameObject spellPrefab;
    GameObject SpellQ;
    GameObject SpellE;
    GameObject SpellR;
    GameObject SpellF;
    private const string CastingQ = "CastingQ";
    private const string CastingE = "CastingE";
    private const string CastingR = "CastingR";
    private const string CastingF = "CastingF";
    private const string Selected = "Selected";
    private const string Unselected = "Unselected";
    string MODE = Unselected;
    Coroutine movementCoroutine;
    Coroutine spellCoroutine;
    //Controlling characters
	//[SerializeField] public PlayerCharacter selectedPlayer;
	public Mob selectedMob;
    [SerializeField] public static UnityEvent<Mob>  selectedMobSend = new UnityEvent<Mob>();
    //client receiving characterData
    [SerializeField] public TacticianFullDataMessage TacticianInformationSheet;
    [SerializeField] public List<CharacterFullDataMessage> InformationSheets = new List<CharacterFullDataMessage>();
    [SerializeField] public List<string> ActivePartyList = new List<string>();
    [SerializeField] public List<string> MatchPartyList = new List<string>();
    public Dictionary<string, string> InspectParty = new Dictionary<string, string>();
    private Dictionary<string, SceneNode> sceneNodesDictionary = new Dictionary<string, SceneNode>();
    string spritePath = "Player";
    private Coroutine spriteSwap;

   void AddAllSceneNodesToDictionary()
    {
        SceneNode[] allNodes = FindObjectsOfType<SceneNode>();

        // clear dictionary before adding new elements
        sceneNodesDictionary.Clear();

        foreach (SceneNode node in allNodes)
        {
            // use the node's name as the key
            sceneNodesDictionary[node.nodeName] = node;
        }
    }
    public void LogoutClient(ScenePlayer sPlayer){
        if(sPlayer == ScenePlayer.localPlayer){
            print("Logging Cleint out now");
            CmdLogoutClient();
        }
    }
    [Command]
    void CmdLogoutClient(){
        LogoutPlayer.Invoke(this.connectionToClient);
    }
    public List<CharacterFullDataMessage> GetInformationSheets(){
        return InformationSheets;
    }
    public List<string> GetParty(){
        return ActivePartyList;
    }
    public List<string> GetMatchParty(){
        return MatchPartyList;
    }
    public TacticianFullDataMessage GetTacticianSheet(){
        return TacticianInformationSheet;
    }

    [ClientRpc]
    public void RpcBuildPartyInspector(ClientPartyInformation savedList)
    {
        //if(ScenePlayer.localPlayer.currentMatch == null){
        //    return;
        //}
        foreach(var key in savedList.Party){
            // Only add if the key does not exist in InspectParty
            if (!InspectParty.ContainsKey(key.Key))//make the class to look at these in inspector
            {
                InspectParty.Add(key.Key, key.Value);
                //print($"added {key.Key} to inspectorlist and they have sprite {key.Value}");
            }
        }
    }
    [Server]
    public void AddPartyServer(string ID){
         if(!ActivePartyList.Contains(ID)){
            ActivePartyList.Add(ID);
            TargetAddParty(ID);
        }
    }
    [TargetRpc]
    void TargetAddParty(string ID){
        ActivePartyList.Add(ID);
    }
    [Server]
    public void AddMatchPartyListServer(string ID){
        MatchPartyList.Add(ID);
        TargetAddMatchPartyList(ID);
    }
    [TargetRpc]
    void TargetAddMatchPartyList(string ID){
        MatchPartyList.Add(ID);
    }
    [Server]
    public void ServerRemovingPartymember(string ID){
        if(ActivePartyList.Contains(ID)){
            ActivePartyList.Remove(ID);
            TargetRemoveParty(ID);
        }
    }
    [TargetRpc]
    void TargetRemoveParty(string ID){
        if(ActivePartyList.Contains(ID)){
            ActivePartyList.Remove(ID);
        }
    }
    [TargetRpc]
    public void TargetSendMobList(List<Mob> mobs){
        SendFogMobs.Invoke(mobs);
    }
    //Inventory/Stat/Spell manipulation
    [Server]
    public void ServerPurchasedItemResult(CharacterInventoryListItem DATA){
        TacticianInformationSheet.StashInventoryData.Add(DATA);
        TargetPurchasedItemResult(DATA);
    }
    [TargetRpc]
    public void TargetPurchasedItemResult(CharacterInventoryListItem DATA){
        // Add the new CharacterInventoryListItem to the CharInventoryData list
        TacticianInformationSheet.StashInventoryData.Add(DATA);
        PurchasedItem.Invoke(DATA);
        ContentSizer.Invoke();
    }
     [Server]
    public void ServerTacticianItemResult(CharacterInventoryListItem DATA){
        TacticianInformationSheet.TacticianInventoryData.Add(DATA);
        TargetTacticianItemResult(DATA);
    }
    [TargetRpc]
    public void TargetTacticianItemResult(CharacterInventoryListItem DATA){
        // Add the new CharacterInventoryListItem to the CharInventoryData list
        TacticianInformationSheet.TacticianInventoryData.Add(DATA);
        PickedUpItemTactician.Invoke(DATA);
    }
    [Server]
    public void GetFullTacticianData(TacticianFullDataMessage DATA){
        TacticianInformationSheet = DATA;
        TargetTactInfoSheetAdd(DATA);
    }
    [TargetRpc]
    void TargetTactInfoSheetAdd(TacticianFullDataMessage DATA){
        SetTactSheetClient(DATA);
    }
    
    void SetTactSheetClient(TacticianFullDataMessage DATA){
        TacticianInformationSheet = DATA;
        //RefreshTactician.Invoke();
        //RebuildItems.Invoke("Tactician", null);
    }
    [Server]
    public void ServerSendDKPCD(string cd, string XRP, string DKP){
        TacticianInformationSheet.DKPCooldown = cd;
        TacticianInformationSheet.DKPBalance = DKP;
        TacticianInformationSheet.XRPBalance = XRP;
        TargetSendDKPCD(cd, XRP, DKP);
    }
    [TargetRpc]
    void TargetSendDKPCD(string cd, string XRP, string DKP){
        TacticianInformationSheet.DKPBalance = DKP;
        TacticianInformationSheet.XRPBalance = XRP;
        TacticianInformationSheet.DKPCooldown = cd;
        StartCoroutine(WalletWakeup());
    }
    IEnumerator WalletWakeup(){
        yield return new WaitForSeconds(2f);
        WalletAwake.Invoke();
        walletTransmute.Invoke();
    }
    [Server]
    public void ServerSpawnItems(){
        TargetSpawnItems();
    }
    [TargetRpc]
    void TargetSpawnItems(){
        BuildInventories.Invoke();
    }
    
    [Server]
    public void GetTacticianNewItem(CharacterInventoryListItem DATA){
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.TacticianInventoryData.Count; j++){
            if (TacticianInformationSheet.TacticianInventoryData[j].Value.customID == DATA.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.TacticianInventoryData.RemoveAt(itemIndex);
        }
        // Add the new CharacterInventoryListItem to the CharInventoryData list
        TacticianInformationSheet.TacticianInventoryData.Add(DATA);
        TargetGetTacticianNewItem(DATA);
    }
    [TargetRpc]
    public void TargetGetTacticianNewItem(CharacterInventoryListItem DATA)
    {
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.TacticianInventoryData.Count; j++){
            if (TacticianInformationSheet.TacticianInventoryData[j].Value.customID == DATA.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.TacticianInventoryData.RemoveAt(itemIndex);
        }
        // Add the new CharacterInventoryListItem to the CharInventoryData list
        TacticianInformationSheet.TacticianInventoryData.Add(DATA);
        Refreshitem.Invoke(this, DATA.Value);
        foreach(var sheet in ScenePlayer.localPlayer.GetInformationSheets()){
            RefreshSheets.Invoke(this, sheet.CharacterID);
        }
        ContentSizer.Invoke();

        //RebuildItems.Invoke("Tactician", null);
        //refresh items just sent back
    }
    [Server]
    public void GetTacticianRemoveItem(CharacterInventoryListItem itemKey){
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.TacticianInventoryData.Count; j++){
            if (TacticianInformationSheet.TacticianInventoryData[j].Value.customID == itemKey.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.TacticianInventoryData.RemoveAt(itemIndex);
        }
        TargetGetTacticianRemoveItem(itemKey);
    }
    [TargetRpc]
    public void TargetGetTacticianRemoveItem(CharacterInventoryListItem itemKey){
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.TacticianInventoryData.Count; j++){
            if (TacticianInformationSheet.TacticianInventoryData[j].Value.customID == itemKey.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.TacticianInventoryData.RemoveAt(itemIndex);
        }
        ContentSizer.Invoke();

        //RebuildItems.Invoke("Tactician", null);

    }
    [Server]
    public void GetStashNewItems(List<CharacterInventoryListItem> DATAList){
        foreach(var DATA in DATAList){
            TacticianInformationSheet.StashInventoryData.Add(DATA);
        }
        // Add the new CharacterInventoryListItem to the CharInventoryData list
        TargetGetStashNewItems(DATAList);
    }
    [TargetRpc]
    public void TargetGetStashNewItems(List<CharacterInventoryListItem> DATAList)
    {
        foreach(var DATA in DATAList){
            TacticianInformationSheet.StashInventoryData.Add(DATA);
        }
        ContentSizer.Invoke();

    }
    void SharedPingMessage(){
        #if UNITY_SERVER
        //print("Server message from scene player called building tactician");
        #endif

        #if !UNITY_SERVER
        //print("Client message from scene player called building tactician");
        #endif
    }
    [TargetRpc]
    public void TargetSendInventoryItemSelectable(CharacterInventoryListItem DATA){
        ResetItemSelectable.Invoke(DATA.Value);
    }
    [Server]
    public void GetStashNewItem(CharacterInventoryListItem DATA){
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.StashInventoryData.Count; j++){
            if (TacticianInformationSheet.StashInventoryData[j].Value.customID == DATA.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.StashInventoryData.RemoveAt(itemIndex);
        }
        // Add the new CharacterInventoryListItem to the CharInventoryData list
        TacticianInformationSheet.StashInventoryData.Add(DATA);
        TargetGetStashNewItem(DATA);
    }
    [TargetRpc]
    public void TargetGetStashNewItem(CharacterInventoryListItem DATA)
    {
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.StashInventoryData.Count; j++){
            if (TacticianInformationSheet.StashInventoryData[j].Value.customID == DATA.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.StashInventoryData.RemoveAt(itemIndex);
        }
        // Add the new CharacterInventoryListItem to the CharInventoryData list
        TacticianInformationSheet.StashInventoryData.Add(DATA);
        Refreshitem.Invoke(this, DATA.Value);
        ContentSizer.Invoke();

        //WRONG
    }
    [Server]
    public void GetStashRemoveItem(CharacterInventoryListItem itemKey){
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.StashInventoryData.Count; j++){
            if (TacticianInformationSheet.StashInventoryData[j].Value.customID == itemKey.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.StashInventoryData.RemoveAt(itemIndex);
        }
        TargetGetStashRemoveItem(itemKey);
    }
    [TargetRpc]
    public void TargetGetStashRemoveItem(CharacterInventoryListItem itemKey){
        int itemIndex = -1;
        for (int j = 0; j < TacticianInformationSheet.StashInventoryData.Count; j++){
            if (TacticianInformationSheet.StashInventoryData[j].Value.customID == itemKey.Value.customID){
                itemIndex = j;
                break;
            }
        }
        // If the item is found, remove it
        if (itemIndex != -1){
            TacticianInformationSheet.StashInventoryData.RemoveAt(itemIndex);
        }
        ContentSizer.Invoke();

    }
    [Server]
    public void GetCharacterPickedUpItem(string charID, string _class, CharacterInventoryListItem DATA)
    {
        int sheetIndex = -1;
        int itemIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharInventoryData.Count; j++)
                {
                    if (InformationSheets[i].CharInventoryData[j].Value.customID == DATA.Value.customID)
                    {
                        itemIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (itemIndex != -1)
                {
                    InformationSheets[i].CharInventoryData.RemoveAt(itemIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharInventoryData.Add(DATA);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetGetCharacterPickedUpItem(charID, _class, DATA);
                break;
            }
        }
    }
    [TargetRpc]
    public void TargetGetCharacterPickedUpItem(string charID, string _class, CharacterInventoryListItem DATA)
    {
        int sheetIndex = -1;
        int itemIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharInventoryData.Count; j++)
                {
                    if (InformationSheets[i].CharInventoryData[j].Value.customID == DATA.Value.customID)
                    {
                        itemIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (itemIndex != -1)
                {
                    InformationSheets[i].CharInventoryData.RemoveAt(itemIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharInventoryData.Add(DATA);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                PickedUpItemCharacter.Invoke(DATA, charID, _class);
                break;
            }
        }
    }
    [Server]
    public void GetCharacterNewItem(string charID, CharacterInventoryListItem DATA)
    {
        int sheetIndex = -1;
        int itemIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharInventoryData.Count; j++)
                {
                    if (InformationSheets[i].CharInventoryData[j].Value.customID == DATA.Value.customID)
                    {
                        itemIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (itemIndex != -1)
                {
                    InformationSheets[i].CharInventoryData.RemoveAt(itemIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharInventoryData.Add(DATA);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetGetCharacterNewItem(charID, DATA);
                break;
            }
        }
    }
    [TargetRpc]
    public void TargetGetCharacterNewItem(string charID, CharacterInventoryListItem DATA)
    {
        int sheetIndex = -1;
        int itemIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharInventoryData.Count; j++)
                {
                    if (InformationSheets[i].CharInventoryData[j].Value.customID == DATA.Value.customID)
                    {
                        itemIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (itemIndex != -1)
                {
                    InformationSheets[i].CharInventoryData.RemoveAt(itemIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharInventoryData.Add(DATA);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                Refreshitem.Invoke(this, DATA.Value);
                RefreshSheets.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]
public void RemoveCharacterItem(string charID, CharacterInventoryListItem itemKey)
{
    int sheetIndex = -1;
    int itemIndex = -1;

    // Find the character sheet with the matching CharacterID
    for (int i = 0; i < InformationSheets.Count; i++)
    {
        if (InformationSheets[i].CharacterID == charID)
        {
            sheetIndex = i;

            // Find the index of the CharacterInventoryListItem with the matching key
            for (int j = 0; j < InformationSheets[i].CharInventoryData.Count; j++)
            {
                if (InformationSheets[i].CharInventoryData[j].Value.customID == itemKey.Value.customID)
                {
                    itemIndex = j;
                    break;
                }
            }

            // If the item is found, remove it
            if (itemIndex != -1)
            {
                InformationSheets[i].CharInventoryData.RemoveAt(itemIndex);
            }

            // Update the character sheet in the InformationSheets
            InformationSheets[i] = InformationSheets[sheetIndex];
            TargetRemoveCharacterItem(charID, itemKey);
            break;
        }
    }
}

[TargetRpc]
public void TargetRemoveCharacterItem(string charID, CharacterInventoryListItem itemKey)
{
    int sheetIndex = -1;
    int itemIndex = -1;

    // Find the character sheet with the matching CharacterID
    for (int i = 0; i < InformationSheets.Count; i++)
    {
        if (InformationSheets[i].CharacterID == charID)
        {
            sheetIndex = i;

            // Find the index of the CharacterInventoryListItem with the matching key
            for (int j = 0; j < InformationSheets[i].CharInventoryData.Count; j++)
            {
                if (InformationSheets[i].CharInventoryData[j].Value.customID == itemKey.Value.customID)
                {
                    itemIndex = j;
                    break;
                }
            }
            // If the item is found, remove it
            if (itemIndex != -1)
            {
                InformationSheets[i].CharInventoryData.RemoveAt(itemIndex);
            }
            // Update the character sheet in the InformationSheets
            InformationSheets[i] = InformationSheets[sheetIndex];
            RefreshSheets.Invoke(this, charID);
            break;
        }
    }
}
    [Server]
    public void GetCharacterSpellItemPurchase(string charID, CharacterSpellListItem SpellDATA, CharacterStatListItem StatDATA)
    {
        int sheetIndex = -1;
        int spellIndex = -1;
        int statIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharSpellData.Count; j++)
                {
                    if (InformationSheets[i].CharSpellData[j].Key == SpellDATA.Key)
                    {
                        spellIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (spellIndex != -1)
                {
                    InformationSheets[i].CharSpellData.RemoveAt(spellIndex);
                }
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == SpellDATA.Key)
                    {
                        statIndex = s;
                        break;
                    }
                }
                if (statIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(statIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharSpellData.Add(SpellDATA);
                InformationSheets[i].CharStatData.Add(StatDATA);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetGetCharacterSpellItemPurchase(charID, SpellDATA, StatDATA);
                break;
            }
        }
    }
    [TargetRpc]
    public void TargetGetCharacterSpellItemPurchase(string charID, CharacterSpellListItem SpellDATA, CharacterStatListItem StatDATA)
    {
        int sheetIndex = -1;
        int spellIndex = -1;
        int statIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharSpellData.Count; j++)
                {
                    if (InformationSheets[i].CharSpellData[j].Key == SpellDATA.Key)
                    {
                        spellIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (spellIndex != -1)
                {
                    InformationSheets[i].CharSpellData.RemoveAt(spellIndex);
                }
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == StatDATA.Key)
                    {
                        statIndex = s;
                    }

                }
                if (statIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(statIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharSpellData.Add(SpellDATA);
                InformationSheets[i].CharStatData.Add(StatDATA);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                ResetSpring.Invoke();
                break;
            }
        }
    }
    [Server]
    public void GetCharacterSpellChange(string charID, CharacterSpellListItem SpellUpdatedDATA, CharacterSpellListItem possibleChangeDATA, SendSpellList spellList)
    {
        int sheetIndex = -1;
        int spellNewIndex = -1;
        int spellOldIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharSpellData.Count; j++)
                {
                    if (InformationSheets[i].CharSpellData[j].Key == SpellUpdatedDATA.Key)
                    {
                        spellNewIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (spellNewIndex != -1)
                {
                    InformationSheets[i].CharSpellData.RemoveAt(spellNewIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharSpellData.Add(SpellUpdatedDATA);
                if(possibleChangeDATA.Key != "Empty"){
                    for (int X = 0; X < InformationSheets[i].CharSpellData.Count; X++)
                    {
                        if (InformationSheets[i].CharSpellData[X].Key == possibleChangeDATA.Key)
                        {
                            spellOldIndex = X;
                            break;
                        }
                    }
                    // If the item is found, remove it
                    if (spellOldIndex != -1)
                    {
                        InformationSheets[i].CharSpellData.RemoveAt(spellOldIndex);
                        InformationSheets[i].CharSpellData.Add(possibleChangeDATA);
                    }
                }
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                break;
            }
        }
        if(currentScene != "OVM" && currentScene != "TOWNOFARUDINE"){
            ChangingMOSpellsMatch.Invoke(charID, this, spellList);
        }
        TargetGetCharacterSpellChange(charID, SpellUpdatedDATA, possibleChangeDATA);
    }
    [TargetRpc]
    public void TargetGetCharacterSpellChange(string charID, CharacterSpellListItem SpellUpdatedDATA, CharacterSpellListItem possibleChangeDATA)
    {
        int sheetIndex = -1;
        int spellNewIndex = -1;
        int spellOldIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                // Find the index of the CharacterInventoryListItem with the matching key
                for (int j = 0; j < InformationSheets[i].CharSpellData.Count; j++)
                {
                    if (InformationSheets[i].CharSpellData[j].Key == SpellUpdatedDATA.Key)
                    {
                        spellNewIndex = j;
                        break;
                    }
                }
                // If the item is found, remove it
                if (spellNewIndex != -1)
                {
                    InformationSheets[i].CharSpellData.RemoveAt(spellNewIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharSpellData.Add(SpellUpdatedDATA);
                if(possibleChangeDATA.Key != "Empty"){
                    for (int X = 0; X < InformationSheets[i].CharSpellData.Count; X++)
                    {
                        if (InformationSheets[i].CharSpellData[X].Key == possibleChangeDATA.Key)
                        {
                            spellOldIndex = X;
                            break;
                        }
                    }
                    // If the item is found, remove it
                    if (spellOldIndex != -1)
                    {
                        InformationSheets[i].CharSpellData.RemoveAt(spellOldIndex);
                        InformationSheets[i].CharSpellData.Add(possibleChangeDATA);
                    }
                }
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                break;
            }
        }
        RefreshSheets.Invoke(this, charID);
    }
    //Save Game
    [Server]

    public void GetSavedGame(CharacterSaveData savedData){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;
        int ClassPointsIndex = -1;
        int expIndex = -1;
        CharacterStatListItem HP = (new CharacterStatListItem{
            Key = "currentHP",
            Value = savedData.CharHealth.ToString()
        });
        CharacterStatListItem MP = (new CharacterStatListItem{
            Key = "currentMP",
            Value =  savedData.CharMana.ToString()
        });
        CharacterStatListItem EXP = (new CharacterStatListItem{
            Key = "EXP",
            Value =  savedData.CharExperience.ToString("F2")
        });
        CharacterStatListItem CLASSPOINTS = (new CharacterStatListItem{
            Key = "ClassPoints",
            Value =  savedData.CharClassPoints.ToString("F2")
        });


        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == savedData.CharID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);

                for (int m = 0; m < InformationSheets[i].CharStatData.Count; m++)
                {
                    if (InformationSheets[i].CharStatData[m].Key == MP.Key)
                    {
                        MPIndex = m;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == CLASSPOINTS.Key)
                    {
                        ClassPointsIndex = X;
                        break;
                    }
                }
                if (ClassPointsIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(ClassPointsIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(CLASSPOINTS);

                for (int R = 0; R < InformationSheets[i].CharStatData.Count; R++)
                {
                    if (InformationSheets[i].CharStatData[R].Key == EXP.Key)
                    {
                        expIndex = R;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(EXP);

                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetSavedGameR(savedData);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetSavedGameR(CharacterSaveData savedData){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;
        int ClassPointsIndex = -1;
        int expIndex = -1;
        CharacterStatListItem HP = (new CharacterStatListItem{
            Key = "currentHP",
            Value = savedData.CharHealth.ToString()
        });
        CharacterStatListItem MP = (new CharacterStatListItem{
            Key = "currentMP",
            Value =  savedData.CharMana.ToString()
        });
        CharacterStatListItem EXP = (new CharacterStatListItem{
            Key = "EXP",
            Value =  savedData.CharExperience.ToString("F2")
        });
        CharacterStatListItem CLASSPOINTS = (new CharacterStatListItem{
            Key = "ClassPoints",
            Value =  savedData.CharClassPoints.ToString("F2")
        });


        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == savedData.CharID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);

                for (int m = 0; m < InformationSheets[i].CharStatData.Count; m++)
                {
                    if (InformationSheets[i].CharStatData[m].Key == MP.Key)
                    {
                        MPIndex = m;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == CLASSPOINTS.Key)
                    {
                        ClassPointsIndex = X;
                        break;
                    }
                }
                if (ClassPointsIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(ClassPointsIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(CLASSPOINTS);

                for (int R = 0; R < InformationSheets[i].CharStatData.Count; R++)
                {
                    if (InformationSheets[i].CharStatData[R].Key == EXP.Key)
                    {
                        expIndex = R;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(EXP);

                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, savedData.CharID);
                break;
            }
        }
    }
    [Server]

    public void GetDEATHCHARACTER(string charID, CharacterStatListItem DEATH, CharacterStatListItem EXP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;
        int DEATHIndex = -1;
        int expIndex = -1;
        CharacterStatListItem HP = (new CharacterStatListItem{
            Key = "currentHP",
            Value = "0"
        });
        CharacterStatListItem MP = (new CharacterStatListItem{
            Key = "currentMP",
            Value = "0"
        });

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);
                for (int m = 0; m < InformationSheets[i].CharStatData.Count; m++)
                {
                    if (InformationSheets[i].CharStatData[m].Key == MP.Key)
                    {
                        MPIndex = m;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == DEATH.Key)
                    {
                        DEATHIndex = X;
                        break;
                    }
                }
                if (DEATHIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(DEATHIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(DEATH);

                for (int R = 0; R < InformationSheets[i].CharStatData.Count; R++)
                {
                    if (InformationSheets[i].CharStatData[R].Key == EXP.Key)
                    {
                        expIndex = R;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(EXP);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetDEATHCHARACTER(charID, DEATH, EXP);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetDEATHCHARACTER(string charID, CharacterStatListItem DEATH, CharacterStatListItem EXP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;
        int DEATHIndex = -1;
        int expIndex = -1;

        CharacterStatListItem HP = (new CharacterStatListItem{
            Key = "currentHP",
            Value = "0"
        });
        CharacterStatListItem MP = (new CharacterStatListItem{
            Key = "currentMP",
            Value = "0"
        });
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);
                for (int m = 0; m < InformationSheets[i].CharStatData.Count; m++)
                {
                    if (InformationSheets[i].CharStatData[m].Key == MP.Key)
                    {
                        MPIndex = m;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == DEATH.Key)
                    {
                        DEATHIndex = X;
                        break;
                    }
                }
                if (DEATHIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(DEATHIndex);
                }
                
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(DEATH);
                for (int R = 0; R < InformationSheets[i].CharStatData.Count; R++)
                {
                    if (InformationSheets[i].CharStatData[R].Key == EXP.Key)
                    {
                        expIndex = R;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                InformationSheets[i].CharStatData.Add(EXP);

                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                DeathBroadcast.Invoke(this, charID);
                break;
            }
        }
    }
     [Server]

    public void ServerResurrectCharacter(string charID, CharacterStatListItem HP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int DeathIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);
                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == "DEATH")
                    {
                        DeathIndex = X;
                        break;
                    }
                }
                if (DeathIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(DeathIndex);
                }
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetResurrectCharacter(charID, HP);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetResurrectCharacter(string charID, CharacterStatListItem HP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int DeathIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);
                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == "DEATH")
                    {
                        DeathIndex = X;
                        break;
                    }
                }
                if (DeathIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(DeathIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                Ressurected.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]

    public void GetCharacterUpdateHPDurability(string charID, CharacterStatListItem HP, CharacterInventoryListItem damagedItem){
        int sheetIndex = -1;
        int HPIndex = -1;
        int DurabilityIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);
                if(damagedItem.Key != "LuckyRoll"){
                    for (int X = 0; X < InformationSheets[i].CharInventoryData.Count; X++)
                    {
                        if (InformationSheets[i].CharInventoryData[X].Value.InstanceID == damagedItem.Value.InstanceID)
                        {
                            DurabilityIndex = X;
                            break;
                        }
                    }
                    if (DurabilityIndex != -1)
                    {
                        InformationSheets[i].CharInventoryData.RemoveAt(DurabilityIndex);
                    }
                    // Add the new CharacterInventoryListItem to the CharInventoryData list
                    InformationSheets[i].CharInventoryData.Add(damagedItem);
                }
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetCharacterUpdateHPDurability(charID, HP, damagedItem);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetCharacterUpdateHPDurability(string charID, CharacterStatListItem HP, CharacterInventoryListItem damagedItem){
        int sheetIndex = -1;
        int HPIndex = -1;
        int DurabilityIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);
                if(damagedItem.Key != "LuckyRoll"){
                    for (int X = 0; X < InformationSheets[i].CharInventoryData.Count; X++)
                    {
                        if (InformationSheets[i].CharInventoryData[X].Value.InstanceID == damagedItem.Value.InstanceID)
                        {
                            DurabilityIndex = X;
                            break;
                        }
                    }
                    if (DurabilityIndex != -1)
                    {
                        InformationSheets[i].CharInventoryData.RemoveAt(DurabilityIndex);
                    }
                    // Add the new CharacterInventoryListItem to the CharInventoryData list
                    InformationSheets[i].CharInventoryData.Add(damagedItem);
                    ResetItemSelectable.Invoke(damagedItem.Value);
                }
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]

    public void ServerCombatHPUpdate(string charID, CharacterStatListItem HP){
        int sheetIndex = -1;
        int HPIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);

                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetCombatHPUpdate(charID, HP);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetCombatHPUpdate(string charID, CharacterStatListItem HP){
        int sheetIndex = -1;
        int HPIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]

    public void GetCharacterUpdateHPMP(string charID, CharacterStatListItem HP, CharacterStatListItem MP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == MP.Key)
                    {
                        MPIndex = X;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetCharacterUpdateHPMP(charID, HP, MP);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetCharacterUpdateHPMP(string charID, CharacterStatListItem HP, CharacterStatListItem MP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == MP.Key)
                    {
                        MPIndex = X;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]

    public void GetCharacterUpdateEXPLVL(string charID, CharacterStatListItem LVL, CharacterStatListItem exp){
        int sheetIndex = -1;
        int LVLIndex = -1;
        int expIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == LVL.Key)
                    {
                        LVLIndex = s;
                        break;
                    }
                }
                if (LVLIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(LVLIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(LVL);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == exp.Key)
                    {
                        expIndex = X;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(exp);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetCharacterUpdateEXPLVL(charID, LVL, exp);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetCharacterUpdateEXPLVL(string charID, CharacterStatListItem LVL, CharacterStatListItem exp){
        int sheetIndex = -1;
        int LVLIndex = -1;
        int expIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == LVL.Key)
                    {
                        LVLIndex = s;
                        break;
                    }
                }
                if (LVLIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(LVLIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(LVL);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == exp.Key)
                    {
                        expIndex = X;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(exp);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]
    public void GetCharacterUpdateLVLINGEXP(string charID, CharacterStatListItem LVLING, CharacterStatListItem EXP){
        int sheetIndex = -1;
        int lvlIndex = -1;
        int expIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == LVLING.Key)
                    {
                        lvlIndex = s;
                        break;
                    }
                }
                if (lvlIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(lvlIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(LVLING);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == EXP.Key)
                    {
                        expIndex = X;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(EXP);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetCharacterUpdateLVLEXP(charID, LVLING, EXP);
                break;
            }
        }
    }
    [TargetRpc]
    public void TargetCharacterUpdateLVLEXP(string charID, CharacterStatListItem LVLING, CharacterStatListItem EXP){
        int sheetIndex = -1;
        int lvlIndex = -1;
        int expIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == LVLING.Key)
                    {
                        lvlIndex = s;
                        break;
                    }
                }
                if (lvlIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(lvlIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(LVLING);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == EXP.Key)
                    {
                        expIndex = X;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }

                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(EXP);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                LevelUpStartedSound.Invoke();
                //Add the event here to play music for level up start
                break;
            }
        }
    }
    [Server]
    public void GetCharacterUpdateLVL(string charID, CharacterStatListItem LVL, string LVLING){
        int sheetIndex = -1;
        int LVLIndex = -1;
        int LevelingIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == LVL.Key)
                    {
                        LVLIndex = s;
                        break;
                    }
                }
                if (LVLIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(LVLIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(LVL);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == LVLING)
                    {
                        LevelingIndex = X;
                        break;
                    }
                }
                if (LevelingIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(LevelingIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetGetCharacterUpdateLVL(charID, LVL, LVLING);
                break;
            }
        }
    }
    [TargetRpc]
    public void TargetGetCharacterUpdateLVL(string charID, CharacterStatListItem LVL, string LVLING){
        int sheetIndex = -1;
        int LVLIndex = -1;
        int LevelingIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == LVL.Key)
                    {
                        LVLIndex = s;
                        break;
                    }
                }
                if (LVLIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(LVLIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(LVL);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == LVLING)
                    {
                        LevelingIndex = X;
                        break;
                    }
                }
                if (LevelingIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(LevelingIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                LevelUpEndedSound.Invoke();
                //Add the event here to play music for level up start
                break;
            }
        }
    }
    [Server]
    public void GetINNServer(string charID, CharacterStatListItem HP, CharacterStatListItem MP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == MP.Key)
                    {
                        MPIndex = X;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetGetINNClient(charID, HP, MP);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetGetINNClient(string charID, CharacterStatListItem HP, CharacterStatListItem MP){
        int sheetIndex = -1;
        int HPIndex = -1;
        int MPIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == HP.Key)
                    {
                        HPIndex = s;
                        break;
                    }
                }
                if (HPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(HPIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(HP);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == MP.Key)
                    {
                        MPIndex = X;
                        break;
                    }
                }
                if (MPIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(MPIndex);
                }

                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(MP);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]
    public void GetCharacterUpdateEXPCP(string charID, CharacterStatListItem classpoints, CharacterStatListItem exp){
        int sheetIndex = -1;
        int classPointsIndex = -1;
        int expIndex = -1;

        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == classpoints.Key)
                    {
                        classPointsIndex = s;
                        break;
                    }
                }
                if (classPointsIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(classPointsIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(classpoints);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == exp.Key)
                    {
                        expIndex = X;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(exp);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                TargetCharacterUpdateEXPCP(charID, classpoints, exp);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetCharacterUpdateEXPCP(string charID, CharacterStatListItem classpoints, CharacterStatListItem exp){
        int sheetIndex = -1;
        int classPointsIndex = -1;
        int expIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                sheetIndex = i;
                for (int s = 0; s < InformationSheets[i].CharStatData.Count; s++)
                {
                    if (InformationSheets[i].CharStatData[s].Key == classpoints.Key)
                    {
                        classPointsIndex = s;
                        break;
                    }
                }
                if (classPointsIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(classPointsIndex);
                }
                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(classpoints);

                for (int X = 0; X < InformationSheets[i].CharStatData.Count; X++)
                {
                    if (InformationSheets[i].CharStatData[X].Key == exp.Key)
                    {
                        expIndex = X;
                        break;
                    }
                }
                if (expIndex != -1)
                {
                    InformationSheets[i].CharStatData.RemoveAt(expIndex);
                }

                // Add the new CharacterInventoryListItem to the CharInventoryData list
                InformationSheets[i].CharStatData.Add(exp);
                // Update the character sheet in the InformationSheets
                InformationSheets[i] = InformationSheets[sheetIndex];
                RefreshSheets.Invoke(this, charID);
                RefreshEXP.Invoke(this, charID);
                break;
            }
        }
    }
    [Server]
    public void GetFullCharacterData(CharacterFullDataMessage DATA){
        // If the character's data is already in the InformationSheets dictionary, update the CharStatData field
        CharacterFullDataMessage SheetRemoving = new CharacterFullDataMessage();
        foreach (var sheet in InformationSheets){
            if (sheet.CharacterID == DATA.CharacterID){
                SheetRemoving = sheet;
            }
        }
        if(InformationSheets.Contains(SheetRemoving)){
            InformationSheets.Remove(SheetRemoving);
        }
        InformationSheets.Add(DATA);
        // If the character's data was not already in the InformationSheets dictionary, add it now
        TargetGiveFullCharacterData(DATA);
    }
    [TargetRpc]
    void TargetGiveFullCharacterData(CharacterFullDataMessage DATA){
        ClientUpdateCharacter(DATA);
    }
    void ClientUpdateCharacter(CharacterFullDataMessage DATA){
        CharacterFullDataMessage SheetRemoving = new CharacterFullDataMessage();
        foreach (var sheet in InformationSheets){
            if (sheet.CharacterID == DATA.CharacterID){
                SheetRemoving = sheet;
            }
        }
        if(InformationSheets.Contains(SheetRemoving)){
            InformationSheets.Remove(SheetRemoving);
        }
        InformationSheets.Add(DATA);
        string _Class = string.Empty;
        foreach(var stat in DATA.CharStatData){
            if(stat.Key == "Class"){
                _Class = stat.Value;
            }
        }
        GetCharacters.Invoke();
        //RebuildItems.Invoke(DATA.CharacterID, _Class);
        RefreshSheets.Invoke(this, DATA.CharacterID);
    }
    [Server]
    public void ServerBuildCharacters(){
        TargetBuildCharacters();
    }
    [TargetRpc]
    void TargetBuildCharacters(){
        GetCharacters.Invoke();
    }
    [Server]
    public void GetFullCharacterDataNew(CharacterFullDataMessage DATA){
        // If the character's data is already in the InformationSheets dictionary, update the CharStatData field
        CharacterFullDataMessage SheetRemoving = new CharacterFullDataMessage();
        foreach (var sheet in InformationSheets){
            if (sheet.CharacterID == DATA.CharacterID){
                SheetRemoving = sheet;
            }
        }
        if(InformationSheets.Contains(SheetRemoving)){
            InformationSheets.Remove(SheetRemoving);
        }
        InformationSheets.Add(DATA);
        // If the character's data was not already in the InformationSheets dictionary, add it now
        TargetGetFullCharacterDataNew(DATA);
    }
    [TargetRpc]
    void TargetGetFullCharacterDataNew(CharacterFullDataMessage DATA){
        ClientGetFullCharacterDataNew(DATA);
    }
    void ClientGetFullCharacterDataNew(CharacterFullDataMessage DATA){
        CharacterFullDataMessage SheetRemoving = new CharacterFullDataMessage();
        foreach (var sheet in InformationSheets){
            if (sheet.CharacterID == DATA.CharacterID){
                SheetRemoving = sheet;
            }
        }
        if(InformationSheets.Contains(SheetRemoving)){
            InformationSheets.Remove(SheetRemoving);
        }
        InformationSheets.Add(DATA);
        string _Class = string.Empty;
        foreach(var stat in DATA.CharStatData){
            if(stat.Key == "Class"){
                _Class = stat.Value;
            }
        }
        GetCharacters.Invoke();
        BuildItems.Invoke(DATA.CharacterID);
    }
    [Server]

    public void ServerCooldownRemove(string charID, CharacterCooldownListItem coolie){
        int sheetIndex = -1;
        int coolieIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                var sheet = InformationSheets[i];
                sheetIndex = i;
                if (sheet.CharCooldownData == null){
                    print("Nothing to delete so we skipped this part");
                } else {
                    for (int s = 0; s < InformationSheets[i].CharCooldownData.Count; s++)
                    {
                        if (InformationSheets[i].CharCooldownData[s].PKey == coolie.PKey)
                        {
                            coolieIndex = s;
                            break;
                        }
                    }
                    if (coolieIndex != -1)
                    {
                        InformationSheets[i].CharCooldownData.RemoveAt(coolieIndex);
                    }
                    // Add the new CharacterInventoryListItem to the CharInventoryData list
                    // Update the character sheet in the InformationSheets
                    InformationSheets[i] = InformationSheets[sheetIndex];
                    print($"{coolie.SpellnameFull} is the spell we are removing from cooldown");
                    TargetCooldownRemove(charID, coolie);
                }
                break;
            }
        }
    }
    [TargetRpc]
    public void TargetCooldownRemove(string charID, CharacterCooldownListItem coolie){
        int sheetIndex = -1;
        int coolieIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                var sheet = InformationSheets[i];
                sheetIndex = i;
                if (sheet.CharCooldownData == null){
                    print("Nothing to delete so we skipped this part");
                } else {
                    for (int s = 0; s < InformationSheets[i].CharCooldownData.Count; s++)
                    {
                        if (InformationSheets[i].CharCooldownData[s].PKey == coolie.PKey)
                        {
                            coolieIndex = s;
                            break;
                        }
                    }
                    if (coolieIndex != -1)
                    {
                        InformationSheets[i].CharCooldownData.RemoveAt(coolieIndex);
                    }
                    // Add the new CharacterInventoryListItem to the CharInventoryData list
                    // Update the character sheet in the InformationSheets
                    InformationSheets[i] = InformationSheets[sheetIndex];
                    print($"{coolie.SpellnameFull} is the spell we are removing from cooldown");
                    RefreshSheets.Invoke(this, charID);
                    break;
                }
            }
        }
    }
    [Server]

    public void ServerCooldownSave(string charID, CharacterCooldownListItem coolie){
        int sheetIndex = -1;
        int coolieIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                var sheet = InformationSheets[i];
                sheetIndex = i;
                if (sheet.CharCooldownData == null){
                        List<CharacterCooldownListItem> cdList = new List<CharacterCooldownListItem>();
                        sheet.CharCooldownData = cdList; // Assign the newly created list
                        sheet.CharCooldownData.Add(coolie);
                        InformationSheets[i] = sheet;
                    } else {
                    for (int s = 0; s < InformationSheets[i].CharCooldownData.Count; s++)
                    {
                        if (InformationSheets[i].CharCooldownData[s].PKey == coolie.PKey)
                        {
                            coolieIndex = s;
                            break;
                        }
                    }
                    if (coolieIndex != -1)
                    {
                        InformationSheets[i].CharCooldownData.RemoveAt(coolieIndex);
                    }
                    // Add the new CharacterInventoryListItem to the CharInventoryData list
                    InformationSheets[i].CharCooldownData.Add(coolie);
                    // Update the character sheet in the InformationSheets
                    InformationSheets[i] = InformationSheets[sheetIndex];
                }
                print($"{coolie.SpellnameFull} is the spell we are putting on cooldown");
                TargetCooldownSave(charID, coolie);
                break;
            }
        }
    }
    [TargetRpc]

    public void TargetCooldownSave(string charID, CharacterCooldownListItem coolie){
        int sheetIndex = -1;
        int coolieIndex = -1;
        // Find the character sheet with the matching CharacterID
        for (int i = 0; i < InformationSheets.Count; i++)
        {
            if (InformationSheets[i].CharacterID == charID)
            {
                var sheet = InformationSheets[i];
                sheetIndex = i;
                if (sheet.CharCooldownData == null){
                    List<CharacterCooldownListItem> cdList = new List<CharacterCooldownListItem>();
                    sheet.CharCooldownData = cdList; // Assign the newly created list
                    sheet.CharCooldownData.Add(coolie);
                    InformationSheets[i] = sheet;
                    print($"{coolie.SpellnameFull} is the spell we are putting on cooldown");
                    RefreshSheets.Invoke(this, charID);
                    break;
                } else {
                    for (int s = 0; s < InformationSheets[i].CharCooldownData.Count; s++)
                    {
                        if (InformationSheets[i].CharCooldownData[s].PKey == coolie.PKey)
                        {
                            coolieIndex = s;
                            break;
                        }
                    }
                    if (coolieIndex != -1)
                    {
                        InformationSheets[i].CharCooldownData.RemoveAt(coolieIndex);
                    }
                    // Add the new CharacterInventoryListItem to the CharInventoryData list
                    InformationSheets[i].CharCooldownData.Add(coolie);
                    // Update the character sheet in the InformationSheets
                    InformationSheets[i] = InformationSheets[sheetIndex];
                    print($"{coolie.SpellnameFull} is the spell we are putting on cooldown");
                    RefreshSheets.Invoke(this, charID);
                    break;
                }
            }
        }
    }
    [TargetRpc]
    public void TargetInnReset(){
        ResetInnBools();
    }
    void ResetInnBools(){
        innReset.Invoke();
    }
    [TargetRpc]
    public void TargetUIOpenNodeOVM(){
        StartCoroutine(OpenNodeUIOVMPause());
    }
    IEnumerator OpenNodeUIOVMPause(){
        yield return new WaitForSeconds(5f);
        LeftTownOpenNode.Invoke("100");
    }
    //Items
    [TargetRpc]
    public void TargetItemRoll(string itemName, int amount, string ID){
        BuildingItemDrop.Invoke(itemName, amount, ID);
         //print($"TargetItemRoll on client for {itemName}");
    }
    [TargetRpc]
    public void TargetCloseEndGameCanvas(){
        DeselectedCharacter();
        if(movementCoroutine != null){
            StopCoroutine(movementCoroutine);
        }
        if(spellCoroutine != null){
            StopCoroutine(spellCoroutine);
        }
        ScenePlayer.localPlayer.ClearSelected();
        CombatPartyView.instance.TurnOffCanvas();
        ClosingWindow();
    }
    void ClosingWindow(){
        ToggleCloseEndMatch.Invoke();
    }
    [TargetRpc]
    public void TargetGatherSeekers(PlayerCharacter player){
        FindSeekers(player);
    }
    IEnumerator CastFindSeekers(PlayerCharacter player){
        yield return new WaitForSeconds(2f);
        //print("Casting find seekrs!");
        BuildCombatPlayerUI.Invoke(player);
    }
    void FindSeekers(PlayerCharacter player){
        BuildCombatPlayerUI.Invoke(player);

        //StartCoroutine(CastFindSeekers(player));
    }
    [TargetRpc]
    public void TargetRefreshItems(ItemSelectable item){
        Refreshitem.Invoke(this, item);
    }
    [Server]
    public void GoldAmountSet(long newGold){
        SetGold(newGold);
    }
    [Server]
    public void SetGold(long newGold){
        Gold = newGold;
        TargetWalletAwake();
    }
    public long GoldAmount(){
        return Gold;
    }
    
    [TargetRpc]
    public void TargetStaffBuild(ItemSelectable item){
        StaffBuild.Invoke(item);
    }
    //TradingItemsInInventory***************************************************************************************************************
    //Stacks
    void StackItemSend(StackingMessage message){
        CmdStackItemSend(message);
    }
    [Command]
    void CmdStackItemSend(StackingMessage message){
        ServerStackItemSend(message);
    }
    [Server]
    void ServerStackItemSend(StackingMessage message){
        StackingItem.Invoke(this.connectionToClient, message);
    }
    //Stash
    void StashToTactInventory(ItemSelectable item){
        CmdStashToTactInventory(item);
    }
    [Command]
    void CmdStashToTactInventory(ItemSelectable item){
        ServerStashToTactInventory(item);
    }
    [Server]
    void ServerStashToTactInventory(ItemSelectable item){
        string request = "StashToTactInventory";
        StashToTactInv.Invoke(this.connectionToClient, item, request);
    }
    void StashToTactEquipment(ItemSelectable item, string SlotName){
        CmdStashToTactEquip(item, SlotName);
    }
    [Command]
    void CmdStashToTactEquip(ItemSelectable item, string SlotName){
        ServerStashToTactEquip(item, SlotName);
    }
    [Server]
    void ServerStashToTactEquip(ItemSelectable item, string SlotName){
        string request = "StashToTactEquip";
        StashToTactEquipped.Invoke(this.connectionToClient, item, request, SlotName);
    }
    void StashToTactbelt(ItemSelectable item){
        CmdStashToTactbelt(item);
    }
    [Command]
    void CmdStashToTactbelt(ItemSelectable item){
        ServerStashToTactbelt(item);
    }
    [Server]
    void ServerStashToTactbelt(ItemSelectable item){
        string request = "StashToTactBelt";
        StashToTactSafetyBelt.Invoke(this.connectionToClient, item, request);
    }
    void StashToCharInventory(string character, ItemSelectable item){
        CmdStashToCharInventory(character, item);
    }
    [Command]
    void CmdStashToCharInventory(string character, ItemSelectable item){
        ServerStashToCharInventory(character, item);
    }
    [Server]
    void ServerStashToCharInventory(string character, ItemSelectable item){
        string request = "StashToCharInv";
        StashToCharInv.Invoke(this.connectionToClient, item, request, character);
    }
    void StashToCharEquipment(string character, ItemSelectable item, string SlotName){
        CmdStashToCharEquipment(character, item, SlotName);
    }
    [Command]
    void CmdStashToCharEquipment(string character, ItemSelectable item, string SlotName){
        ServerStashToCharEquipment(character, item, SlotName);
    }
    [Server]
    void ServerStashToCharEquipment(string character, ItemSelectable item, string SlotName){
        string request = "StashToCharEquip";
        EquippingData BuiltEquipData = new EquippingData { Slot = SlotName, CharacterSlot = character };
        StashToCharEquip.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    void TacticianInvToStash(ItemSelectable item){
        CmdTacticianInvToStash(item);
    }
    [Command]
    void CmdTacticianInvToStash(ItemSelectable item){
        ServerTacticianInvToStash(item);
    }
    [Server]
    void ServerTacticianInvToStash(ItemSelectable item){
        string request = "TactInventoryToStash";
        TactInvToStash.Invoke(this.connectionToClient, item, request);
    }
    void TacticianInvToTactEquip(ItemSelectable item, string SlotName){
        CmdTacticianInvToTactEquip(item, SlotName);
    }
    [Command]
    void CmdTacticianInvToTactEquip(ItemSelectable item, string SlotName){
        ServerTacticianInvToTactEquip(item, SlotName);
    }
    [Server]
    void ServerTacticianInvToTactEquip(ItemSelectable item, string SlotName){
        string request = "TacticianInvToTacticianEquip";
        TactInvToTactEquip.Invoke(this.connectionToClient, item, request, SlotName);
    }
    void TacticianInvToTactBelt(ItemSelectable item){
        CmdTacticianInvToTactBelt(item);
    }
    [Command]
    void CmdTacticianInvToTactBelt(ItemSelectable item){
        ServerTacticianInvToTactBelt(item);
    }
    [Server]
    void ServerTacticianInvToTactBelt(ItemSelectable item){
        string request = "TactInvToTactBelt";
        TactInvToTactBelt.Invoke(this.connectionToClient, item, request);
    }
    
    void TacticianInvToCharInv(string character, ItemSelectable item){
        CmdTactInvToCharInv(character, item);
    }
    [Command]
    void CmdTactInvToCharInv(string character, ItemSelectable item){
        ServerInvTactToCharInv(character, item);
    }
    [Server]
    void ServerInvTactToCharInv(string character, ItemSelectable item){
        string request = "TactInvCharInv";
        TactInvToCharInv.Invoke(this.connectionToClient, item, request, character);
    }
    void TactInvToCharEquipment(string character, ItemSelectable item, string SlotName){
        CmdTactInvToCharEquipment(character, item, SlotName);
    }
    [Command]
    void CmdTactInvToCharEquipment(string character, ItemSelectable item, string SlotName){
        ServerInvTactToCharEquipment(character, item, SlotName);
    }
    [Server]
    void ServerInvTactToCharEquipment(string character, ItemSelectable item, string SlotName){
        string request = "TactInvToCharEquip";
        EquippingData BuiltEquipData = new EquippingData { Slot = SlotName, CharacterSlot = character };
        TactInvToCharEquip.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    
    void TacticianEquipToStash(ItemSelectable item){
        CmdTacticianEquipToStash(item);
    }
    [Command]
    void CmdTacticianEquipToStash(ItemSelectable item){
        ServerTacticianEquipToStash(item);
    }
    [Server]
    void ServerTacticianEquipToStash(ItemSelectable item){
        string request = "TactEquipToStash";
        TactEquipToStash.Invoke(this.connectionToClient, item, request);
    }
    void TacticianEquipToTactInv(ItemSelectable item){
        CmdTacticianEquipToTactInv(item);
    }
    [Command]
    void CmdTacticianEquipToTactInv(ItemSelectable item){
        ServerTacticianEquipToTactInv(item);
    }
    [Server]
    void ServerTacticianEquipToTactInv(ItemSelectable item){
        string request = "TactEquipToTactInv";
        TactEquipToTactInv.Invoke(this.connectionToClient, item, request);
    }
    void TacticianEquipToTactEquip(ItemSelectable item, string SlotName){
        CmdTacticianEquipToTactEquip(item, SlotName);
    }
    [Command]
    void CmdTacticianEquipToTactEquip(ItemSelectable item, string SlotName){
        ServerTacticianEquipToTactEquip(item, SlotName);
    }
    [Server]
    void ServerTacticianEquipToTactEquip(ItemSelectable item, string SlotName){
        string request = "TacticianInvToTacticianEquip";
        TactEquipToTactEquip.Invoke(this.connectionToClient, item, request, SlotName);
    }
    void TacticianEquipToTactBelt(ItemSelectable item){
        CmdTacticianEquipToTactBelt(item);
    }
    [Command]
    void CmdTacticianEquipToTactBelt(ItemSelectable item){
        ServerTacticianEquipToTactBelt(item);
    }
    [Server]
    void ServerTacticianEquipToTactBelt(ItemSelectable item){
        string request = "TactEquipToTactBelt";
        TactEquipToTactBelt.Invoke(this.connectionToClient, item, request);
    }
    void TacticianEquipToCharInv(string character, ItemSelectable item){
        CmdTacticianEquipToCharInv(character, item);
    }
    [Command]
    void CmdTacticianEquipToCharInv(string character, ItemSelectable item){
        ServerTacticianEquipToCharInv(character, item);
    }
    [Server]
    void ServerTacticianEquipToCharInv(string character, ItemSelectable item){
        string request = "TactEquipCharInv";
        TactEquipToCharInv.Invoke(this.connectionToClient, item, request, character);
    }
    void TacticianBeltToStash(ItemSelectable item){
        CmdTacticianBeltToStash(item);
    }
    [Command]
    void CmdTacticianBeltToStash(ItemSelectable item){
        ServerTacticianBeltToStash(item);
    }
    [Server]
    void ServerTacticianBeltToStash(ItemSelectable item){
        string request = "TactBeltToStash";
        TactBeltToStash.Invoke(this.connectionToClient, item, request);
    }
    void TacticianBeltToTactInv(ItemSelectable item){
        CmdTacticianBeltToTactInv(item);
    }
    [Command]
    void CmdTacticianBeltToTactInv(ItemSelectable item){
        ServerTacticianBeltToTactInv(item);
    }
    [Server]
    void ServerTacticianBeltToTactInv(ItemSelectable item){
        string request = "TactBeltToTactInv";
        TactBeltToTactInv.Invoke(this.connectionToClient, item, request);
    }
    void TacticianBeltToTactEquip(ItemSelectable item, string SlotName){
        CmdTacticianBeltToTactEquip(item, SlotName);
    }
    [Command]
    void CmdTacticianBeltToTactEquip(ItemSelectable item, string SlotName){
        ServerTacticianBeltToTactEquip(item, SlotName);
    }
    [Server]
    void ServerTacticianBeltToTactEquip(ItemSelectable item, string SlotName){
        string request = "TactBeltToTactEquip";
        TactBeltToTactEquip.Invoke(this.connectionToClient, item, request, SlotName);
    }
    void TacticianBeltToCharInv(string character, ItemSelectable item){
        CmdTacticianBeltToCharInv(character, item);
    }
    [Command]
    void CmdTacticianBeltToCharInv(string character, ItemSelectable item){
        ServerTacticianBeltToCharInv(character, item);
    }
    [Server]
    void ServerTacticianBeltToCharInv(string character, ItemSelectable item){
        string request = "TactBeltCharInv";
        TactSafetyBeltToCharInv.Invoke(this.connectionToClient, item, request, character);
    }
    void TacticianBeltToCharEquip(string character, ItemSelectable item, string SlotName){
        CmdTacticianBeltToCharEquip(character, item, SlotName);
    }
    [Command]
    void CmdTacticianBeltToCharEquip(string character, ItemSelectable item, string SlotName){
        ServerTacticianBeltToCharEquip(character, item, SlotName);
    }
    [Server]
    void ServerTacticianBeltToCharEquip(string character, ItemSelectable item, string SlotName){
        string request = "TactBeltToCharEquip";
        EquippingData BuiltEquipData = new EquippingData { Slot = SlotName, CharacterSlot = character };
        TactSafetyBeltToCharEquip.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    void CharacterInvToStash(string character, ItemSelectable item){
        CmdCharacterInvToStash(character, item);
    }
    [Command]
    void CmdCharacterInvToStash(string character, ItemSelectable item){
        ServerCharacterInvToStash(character, item);
    }
    [Server]
    void ServerCharacterInvToStash(string character, ItemSelectable item){
        string request = "CharInvStash";
        CharInvToStash.Invoke(this.connectionToClient, item, request, character);
    }
    void CharacterInvToTactInventory(string character, ItemSelectable item){
        CmdCharacterInvToTactInventory(character, item);
    }
    [Command]
    void CmdCharacterInvToTactInventory(string character, ItemSelectable item){
        ServerCharacterInvToTactInventory(character, item);
    }
    [Server]
    void ServerCharacterInvToTactInventory(string character, ItemSelectable item){
        string request = "CharInvTactInv";
        CharInvToTactInv.Invoke(this.connectionToClient, item, request, character);
    }
    void CharacterInvToTactBelt(string character, ItemSelectable item){
        CmdCharacterInvToTactBelt(character, item);
    }
    [Command]
    void CmdCharacterInvToTactBelt(string character, ItemSelectable item){
        ServerCharacterInvToTactBelt(character, item);
    }
    [Server]
    void ServerCharacterInvToTactBelt(string character, ItemSelectable item){
        string request = "CharInvTactBelt";
        CharInvToTactBelt.Invoke(this.connectionToClient, item, request, character);
    }
    void CharacterInvToTactEquip(string character, ItemSelectable item, string SlotName){
        CmdCharacterInvToTactEquip(character, item, SlotName);
    }
    [Command]
    void CmdCharacterInvToTactEquip(string character, ItemSelectable item, string SlotName){
        ServerCharacterInvToTactEquip(character, item, SlotName);
    }
    [Server]
    void ServerCharacterInvToTactEquip(string character, ItemSelectable item, string SlotName){
        string request = "CharInvTactEquip";
        EquippingData BuiltEquipData = new EquippingData { Slot = SlotName, CharacterSlot = character };
        CharInvToTactEquip.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    
    void CharacterEquipToStash(string character, ItemSelectable item){
        CmdCharacterEquipToStash(character, item);
    }
    [Command]
    void CmdCharacterEquipToStash(string character, ItemSelectable item){
        ServerCharacterEquipToStash(character, item);
    }
    [Server]
    void ServerCharacterEquipToStash(string character, ItemSelectable item){
        string request = "CharEquipStash";
        CharEquipToStash.Invoke(this.connectionToClient, item, request, character);
    }
    void CharacterEquipToTacticianInv(string character, ItemSelectable item){
        CmdCharacterEquipToTacticianInv(character, item);
    }
    [Command]
    void CmdCharacterEquipToTacticianInv(string character, ItemSelectable item){
        ServerCharacterEquipToTacticianInv(character, item);
    }
    [Server]
    void ServerCharacterEquipToTacticianInv(string character, ItemSelectable item){
        string request = "CharEquipTactInv";
        CharEquipToTactInv.Invoke(this.connectionToClient, item, request, character);
    }
    void CharacterEquipToTacticianBelt(string character, ItemSelectable item){
        CmdCharacterEquipToTacticianBelt(character, item);
    }
    [Command]
    void CmdCharacterEquipToTacticianBelt(string character, ItemSelectable item){
        ServerCharacterEquipToTacticianBelt(character, item);
    }
    [Server]
    void ServerCharacterEquipToTacticianBelt(string character, ItemSelectable item){
        string request = "CharEquipTactBelt";
        CharEquipToTactBelt.Invoke(this.connectionToClient, item, request, character);
    }
     void DestroyingItem(string character, ItemSelectable item){
        CmdDestroyingItem(character, item);
    }
    [Command]
    void CmdDestroyingItem(string character, ItemSelectable item){
        ServerDestroyingItem(character, item);
    }
    [Server]
    void ServerDestroyingItem(string character, ItemSelectable item){
        ServerDestroyItem.Invoke(this.connectionToClient, item, character);
    }
    void CharacterInvToCharInv(string characterOne, string characterTwo, ItemSelectable item){
        CmdCharacterInvToCharInv(characterOne, characterTwo, item);
    }
    [Command]
    void CmdCharacterInvToCharInv(string characterOne, string characterTwo,  ItemSelectable item){
        ServerCharacterInvToCharInv(characterOne, characterTwo, item);
    }
    [Server]
    void ServerCharacterInvToCharInv(string characterOne, string characterTwo,  ItemSelectable item){
        string request = "CharInvCharInv";
        EquippingData BuiltEquipData = new EquippingData { CharacterSlotOne = characterOne, CharacterSlotTwo = characterTwo};
        CharInvToCharInv.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    void CharacterInvToCharEquip(string characterOne, string characterTwo, ItemSelectable item, string SlotName){
        CmdCharacterInvToCharEquip(characterOne, characterTwo, item, SlotName);
    }
    [Command]
    void CmdCharacterInvToCharEquip(string characterOne, string characterTwo, ItemSelectable item, string SlotName){
        ServerCharacterInvToCharEquip(characterOne, characterTwo, item, SlotName);
    }
    [Server]
    void ServerCharacterInvToCharEquip(string characterOne, string characterTwo, ItemSelectable item, string SlotName){
        string request = "CharInvCharEquip";
        EquippingData BuiltEquipData = new EquippingData { CharacterSlotOne = characterOne, CharacterSlotTwo = characterTwo, Slot = SlotName};
        CharInvToCharEquip.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    void CharacterEquipToCharInv(string characterOne, string characterTwo, ItemSelectable item){
        CmdCharacterEquipToCharInv(characterOne, characterTwo, item);
    }
    [Command]
    void CmdCharacterEquipToCharInv(string characterOne, string characterTwo, ItemSelectable item){
        ServerCharacterEquipToCharInv(characterOne, characterTwo, item);
    }
    [Server]
    void ServerCharacterEquipToCharInv(string characterOne, string characterTwo, ItemSelectable item){
        string request = "CharEquipCharInv";
        EquippingData BuiltEquipData = new EquippingData { CharacterSlotOne = characterOne, CharacterSlotTwo = characterTwo};

        CharEquipToCharInv.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    void CharacterEquipToCharEquip(string characterOne, string characterTwo, ItemSelectable item, string SlotName){
        CmdCharacterEquipToCharEquip(characterOne, characterTwo, item, SlotName);
    }
    [Command]
    void CmdCharacterEquipToCharEquip(string characterOne, string characterTwo, ItemSelectable item, string SlotName){
        ServerCharacterEquipToCharEquip(characterOne, characterTwo, item, SlotName);
    }
    [Server]
    void ServerCharacterEquipToCharEquip(string characterOne, string characterTwo, ItemSelectable item, string SlotName){
        string request = "CharEquipCharEquip";
        EquippingData BuiltEquipData = new EquippingData { Slot = SlotName,  CharacterSlotOne = characterOne, CharacterSlotTwo = characterTwo};
        CharEquipToCharEquip.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    
    void CharacterEquiptoInvSame(string character, ItemSelectable item){
        CmdCharacterEquiptoInvSame(character, item);
    }
    [Command]
    void CmdCharacterEquiptoInvSame(string character, ItemSelectable item){
        ServerCharacterEquiptoInvSame(character, item);
    }
    [Server]
    void ServerCharacterEquiptoInvSame(string character, ItemSelectable item){
        string request = "CharEquipInvSame";
        EquippingData BuiltEquipData = new EquippingData { CharacterSlot = character };
        CharEquipToInvSame.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    void CharacterInvtoEquipSame(string character, ItemSelectable item, string SlotName){
        CmdCharacterInvtoEquipSame(character, item, SlotName);
    }
    [Command]
    void CmdCharacterInvtoEquipSame(string character, ItemSelectable item, string SlotName){
        ServerCharacterInvtoEquipSame(character, item, SlotName);
    }
    [Server]
    void ServerCharacterInvtoEquipSame(string character, ItemSelectable item, string SlotName){
        string request = "CharInvEquipSame";
        EquippingData BuiltEquipData = new EquippingData { Slot = SlotName, CharacterSlot = character };
        CharInvToEquipSame.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }
    void CharacterEquiptoEquipSame(string character, ItemSelectable item, string SlotName){
        CmdCharacterEquiptoEquipSame(character, item, SlotName);
    }
    [Command]
    void CmdCharacterEquiptoEquipSame(string character, ItemSelectable item, string SlotName){
        ServerCharacterEquiptoEquipSame(character, item, SlotName);
    }
    [Server]
    void ServerCharacterEquiptoEquipSame(string character, ItemSelectable item, string SlotName){
        string request = "CharEquipEquipSame";
        EquippingData BuiltEquipData = new EquippingData { Slot = SlotName, CharacterSlot = character };
        CharEquipToEquipSame.Invoke(this.connectionToClient, item, request, BuiltEquipData);
    }

    //UnequipEquipMethods
    void CharacterOneUnequipToCharEquip(string character, ItemSelectable itemOne, ItemSelectable itemTwo, string slot){
        CmdCharacterOneUnequipToCharEquip(character, slot, itemOne, itemTwo);
    }
    [Command]
    void CmdCharacterOneUnequipToCharEquip(string character, string slot, ItemSelectable itemOne, ItemSelectable itemTwo){
        ServerCharacterOneUnequipToCharEquip(character, slot, itemOne, itemTwo);
    }
    [Server]
    void ServerCharacterOneUnequipToCharEquip(string character, string slot, ItemSelectable itemOne, ItemSelectable itemTwo){
        string request = "CharUnequipCharEquip";
        EquippingData BuiltEquipData = new EquippingData { Request = request, CharacterSlot = character, Slot =  slot};

        CharOneUnequipToCharEquip.Invoke(this.connectionToClient, itemOne, itemTwo, BuiltEquipData);
    }
    void CharacterTwoUnequipToCharEquip(string character, ItemSelectable itemOne, ItemSelectable itemTwo, string slot){
        CmdCharacterTwoUnequipToCharEquip(character, slot, itemOne, itemTwo);
    }
    [Command]
    void CmdCharacterTwoUnequipToCharEquip(string character, string slot, ItemSelectable itemOne, ItemSelectable itemTwo){
        ServerCharacterTwoUnequipToCharEquip(character, slot, itemOne, itemTwo);
    }
    [Server]
    void ServerCharacterTwoUnequipToCharEquip(string character, string slot, ItemSelectable itemOne, ItemSelectable itemTwo){
        string request = "CharUnequipCharEquip";
        EquippingData BuiltEquipData = new EquippingData { Request = request, CharacterSlot = character , Slot = slot};

        CharTwoUnequipToCharEquip.Invoke(this.connectionToClient, itemOne, itemTwo, BuiltEquipData);
    }
     void CharacterUnequipToTactStash(string character, InventoryItem itemOne, InventoryItem itemTwo, string slot){
        string ItemOneID = itemOne.SeeSelectable().customID;
        string ItemTwoID = itemTwo.SeeSelectable().customID;
        if(ItemOneID == ItemTwoID){
            print("Rejected this call! ************************************ means we have an issue still");
            return;
        }
        bool inv = itemOne.GetTactInventory();
        bool belt = itemOne.GetTactBelt();
        bool stash = itemOne.GetStashSheet();
        bool NFT = itemOne.SeeSelectable().NFT;
        if(stash){
            inv = false;
        }
        print($"{inv} inv, {belt} belt, {stash} stash, {NFT} NFT, {ItemOneID} ItemOneID, {ItemTwoID} ItemTwoID, {itemOne.GetTacticianSheet()} itemOne.GetTacticianSheet, {itemTwo.GetTacticianSheet()} itemTwo.GetTacticianSheet, {itemOne.GetStashSheet()} itemOne.GetStashSheet, {itemTwo.GetStashSheet()} itemTwo.GetStashSheet,");
        CmdCharacterUnequipToTactStash(character, slot, ItemOneID, ItemTwoID, inv, belt, stash, NFT);
    }
    [Command]
    void CmdCharacterUnequipToTactStash(string character, string slot, string itemOne, string itemTwo, bool inv, bool belt, bool stash, bool NFT){
        ServerCharacterUnequipToTactStash(character, slot, itemOne, itemTwo, inv, belt, stash, NFT);
    }
    [Server]
    void ServerCharacterUnequipToTactStash(string character, string slot, string itemOne, string itemTwo, bool inv, bool belt, bool stash, bool NFT){
        //itemOne is the one going to tact, itemTwo is coming from stash tact inv or belt lets find out
        bool tactInv = inv;
        bool tactBelt = belt;
        bool NFTStash = NFT;
        bool Stash = stash;
        string tactOrStashID = string.Empty;
        if(tactInv || tactBelt){
            tactOrStashID = "Tactician";
        } else {
            tactOrStashID = "Stash";
        }
        string request = "CharacterUnequipToTactStash";
        print($"{tactOrStashID} is the tact or stash id ");
        EquippingData BuiltEquipData = new EquippingData { Request = request, CharacterSlot = character , Slot = slot, CharacterSlotOne = tactOrStashID , TactBelt = tactBelt, Stash = Stash, TactInv = tactInv };

        CharUnequipTactToCharEquip.Invoke(this.connectionToClient, itemOne, itemTwo, BuiltEquipData);
    }
    void CreateNewStackItem(NewStackCreated buildingStack){
        CmdCreateNewStackItem(buildingStack);
    }
    [Command]
    void CmdCreateNewStackItem(NewStackCreated buildingStack){
        ServerCreateNewStackItem(buildingStack);
    }
    [Server]
    void ServerCreateNewStackItem(NewStackCreated buildingStack){

        BuildStackableItem.Invoke(buildingStack);
    }
    //****************************
    //***********Characters*******
    //**************************** 
    [TargetRpc]
    public void TargetTokenUpdate(){
        ResetTokens.Invoke();
    }
    void TokenUpdate(){
        ResetTokens.Invoke();
    }
    
    [Command]
    void CmdAddToParty(string charID){
        PartySelected(charID);
    }
    [Server]
    void PartySelected(string charID){
        if(ActivePartyList.Count < 6){
            SendParty.Invoke(this.connectionToClient, charID);
        }
    }
    [TargetRpc]
    public void TargetPopulateSelected(){
        PartySpawned.Invoke();
    }
    void AddPartyMember(string Id){
        if(ActivePartyList.Count > 5){
            return;
        }
        foreach(var sheet in InformationSheets){
            if(sheet.CharacterID == Id){
                if(!ActivePartyList.Contains(Id)){
                    CmdAddToParty(Id);
                }
            }
        }
    }
    void RemovePartyMember(string ID){
        if(ActivePartyList.Contains(ID)){
            ActivePartyList.Remove(ID);
            CmdRemovePartyMember(ID);
        }
    }
    [Command]
    void CmdRemovePartyMember(string ID){
        if(ActivePartyList.Contains(ID)){
            ServerRemovePartyMember(ID);
        }
    }
    [Server]
    void ServerRemovePartyMember(string ID){
        if(ActivePartyList.Contains(ID)){
            PartyRemoval.Invoke(this.connectionToClient, ID);
        }
        
    }
    void RollNeed(string itemName, string ID, string choice){
        CmdPassVoteNeed(itemName, ID, choice);
    }
    [Command]
    void CmdPassVoteNeed(string itemName, string ID, string choice){
        VoteNeed.Invoke(itemName, ID, choice, this);
    }
    void RollGreed(string itemName, string ID, string choice){
        CmdPassVoteGreed(itemName, ID, choice);
    }
    [Command]
    void CmdPassVoteGreed(string itemName, string ID, string choice){
        VoteGreed.Invoke(itemName, ID, choice, this);
    }
    void RollPass(string itemName, string ID){
        CmdPassVotePass(itemName, ID);
    }
    [Command]
    void CmdPassVotePass(string itemName, string ID){
        VotePass.Invoke(itemName, ID, this);
    }
    void HealPartyRequest(){
        CmdHealPartyRequest();
    }
    [Command]
    void CmdHealPartyRequest(){
        HealPartyServer.Invoke(this.connectionToClient);
    }
    void RequestToBuild(string _nameRequest, string _Type, string spri){
        //do check to make sure this is a valid use of a token and we hav eone
        //print("Ready to ping server");
        CMDRecruitCharacter(_nameRequest, _Type, spri);
    }
    [Command]
    void CMDRecruitCharacter(string _nameRequest, string _Type, string spri){
        //if character token is in inventory process
        RecruitCharacter(_nameRequest, _Type, spri);

    }
    [Server]
    void RecruitCharacter(string _nameRequest, string _Type, string spri){
        //print("got to recruit character");

        ServerCharacterBuildRequest.Invoke(this.connectionToClient, _nameRequest, _Type, spri);
    }
    [Server]
    public void TokenCounted(int tokens){
        TokenCount = tokens;
    }
    public bool nodeWindow(){
        return ableToClick;
    }
    [Server]
    public void ServerAbletoClick(){
        TargetAbleToClick();
    }
    [TargetRpc]
    void TargetAbleToClick(){
        ableToClick = true;
    }
    Sprite Load( string imageName, string spriteName)
    {
        Sprite[] all = Resources.LoadAll<Sprite>( imageName);
 
        foreach( var s in all)
        {
            if (s.name == spriteName)
            {
                return s;
            }
        }
        return null;
    }
    public override void OnStartAuthority(){
        
    }
    [Server]
    public void ServerMapSelect(bool on){
        if(on){
            TargetOVMapOn();
        } else {
            TargetOVMapOff();
        }
    }
    [TargetRpc]
    public void TargetOVMapOn(){
        MapOn.Invoke();
    }
    [TargetRpc]
    public void TargetOVMapOff(){
        MapOff.Invoke();
    }
    [Server]
    public void SetPlayerData(PlayerInfo playerData){
        //print("printing at set player data");
        playerName = playerData.PlayerName;
        Energy = playerData.Energy;
        currentScene = playerData.CurrentScene;
        currentNode = playerData.SavedNode;
        loadSprite = playerData.PlayerSprite;
        charliesTicket = playerData.SessionTicket;
        if(playerData.newPlayer){
            currentScene = "TOWNOFARUDINE";
            //Gold = 1000000;
        } 
        //print("printing at end set player data");
        //print($"printing {currentNode}");

    }
    [Server]
    public void SetOurNode(Dictionary<string, SceneNode> nodes, string current){
        StartCoroutine(PauseThenSetNode(current));
        sceneNodesDictionary = nodes;
    }
    IEnumerator PauseThenSetNode(string current){
        yield return new WaitForSeconds(2f);
        OurNode = FindNodeByName(current);
        //print($"printing {current}");
        //TargetSetOurNode(current);
    }
    
    void FindOurNode()
    {
        if(currentScene == "OVM"){
            OurNode = FindNodeByName(currentNode);
            //print($"Finding node and setting ournode to {currentNode}");
        }
    }
    [TargetRpc]
    public void TargetCharge(float _energy)
    {
        Charging.Invoke(_energy);
    }
    [Server]
    public void EnergyTick(float energy){
        Energy = energy;
        //print($"Testing Energy: {Energy}");
        TargetUpdateEnergyDisplay(Energy);
    }
    [TargetRpc]
    public void TargetWalletAwake(){
        WalletAwake.Invoke();
    }
    [TargetRpc]
    public void TargetOpenUI(string currentScene){
        OnUI.Invoke(currentScene);
        //WalletAwake.Invoke();
    }
    //[Server]
    //public void ServerTurnOffSprite(){
    //    TargetTurnOffSprite();
    //}
    //[TargetRpc]
    //void TargetTurnOffSprite(){
    //    TurnOffSprite();
    //}
    //void TurnOffSprite(){
    //    SpriteRenderer sRend = GetComponent<SpriteRenderer>();
    //    sRend.enabled = false;
    //}
    
    [TargetRpc]
    public void TargetToggleSprite(bool show, string loadSprite){
        SceneChangeSprite(show, loadSprite);
    }
    [ClientRpc]
    public void RpcToggleSprite(bool SpriteSet, string sprite){
        //if(currentMatch != null){
            //if(currentMatch == ScenePlayer.localPlayer.currentMatch){
                SceneChangeSprite(SpriteSet, sprite);
            //}
        //}
    }
    [TargetRpc]
    public void TargetShowPartyCombatView(){
        CombatPartyView.instance.TurnOnCanvas();
    }
   
   
    void SceneChangeSprite(bool show, string playerSprite){
        print($"Turn sprite on is {show}, {playerSprite} is our player sprite");
        if(spriteSwap != null){
            StopCoroutine(spriteSwap);
        }
        if(show)
        {
            spriteSwap = StartCoroutine(SpriteSwapPlayer(playerSprite));

        }else{
            SpriteRenderer sRend = GetComponent<SpriteRenderer>();
            sRend.enabled = false;
        }
    }
    IEnumerator SpriteSwapPlayer(string playerSprite){
        SpriteRenderer sRend = GetComponent<SpriteRenderer>();
        sRend.enabled = true;
        string playerSpriteAlt = playerSprite.Replace("Player0", "Player1");
    
        Sprite spriteOne = Load("Player0", playerSprite);
        Sprite spriteTwo = Load("Player1", playerSpriteAlt);
        while(true){
            sRend.sprite = spriteOne;
            yield return new WaitForSeconds(.5f);
            sRend.sprite = spriteTwo;
            yield return new WaitForSeconds(.5f);
        }
    }
    [TargetRpc]
    public void TargetToggleLoadBarOff(){
        LoadbarOffToggle.Invoke();
    }
    void RequestPurchase(uint price, string itemName){
        //print($"Item: {itemName} was purchased at price: {price}");
        if(price > 0 && price < 30001 && itemName != null){
            CmdPayForItem(price, itemName);
        }
    }
    [Command]
    void CmdPayForItem(uint price, string itemName){
        if(price > 0 && price < 30001 && itemName != null){
        ServerTransmitPayment(price, itemName);
        }

    }
    [Server]
    void ServerTransmitPayment(uint price, string itemName){
        ServerTransmitTX.Invoke(this.connectionToClient, price, itemName);
    }
    [TargetRpc]
    public void TargetEnablePurchaseBtn(){
        EnablePurchaseBtn();
    }
    void EnablePurchaseBtn(){
        PurchaseButtonAvailable.Invoke();
    }
    void RequestOVM(){
        
        if(!isLocalPlayer){ return; }
        if(currentScene == "TOWNOFARUDINE"){
            CmdRequestOVM();
        }
        
    }
    [Command]
    void CmdRequestOVM(){
        OurNode = FindNodeByName("TOWNOFARUDINE");
        RequestOVMCheck();
        TargetSetTownNode();
        //add validation 
    }
    [TargetRpc]
    void TargetSetTownNode(){
        OurNode = FindNodeByName("TOWNOFARUDINE");
    }
    [Server]
    void RequestOVMCheck(){
        ResetOVM.Invoke(this.connectionToClient, charliesTicket);
        
    }
    [TargetRpc]
    public void TargetSendOVMRequest(){
        SendingOVMRequest();
    }
    void SendingOVMRequest(){
        OVMRequest.Invoke(currentScene);
    }
    void CaptainSaysGo(){
        
        noWasClicked = false;
        acceptedPayment = true;
    }
    void CaptainSaysNo(){
        noWasClicked = true;
        acceptedPayment = false;
        TurnClickerOn();
        
    }
    void TurnClickerOn(){
        ableToClick = true;
    }
    [Command]
    void CmdProcessSpellPurchase(LearnSpell spell, string spellBook){
        SpellPurchase.Invoke(this.connectionToClient, spell, spellBook); 
    }
    [Command]
    void CmdProcessSpellChange(string spellName, string spellBook, string slot){
        SpellChange.Invoke(this.connectionToClient, spellName, spellBook, slot); 
    }
    [Command]
    void CmdPermissionToEnd(){
        ServerPermissionToEnd();
        
    }
    [Server]
    void ServerPermissionToEnd(){
        PermissionToFinish.Invoke(currentMatch);
    }
    void BlockClicker(){
        playerOVMReady = false;
    }
    void ClearClicker(){
        playerOVMReady = true;
        //GetCharacters.Invoke();
        //foreach(var character in InformationSheets){
        //    GetCharacters.Invoke(character.CharacterID);
        //}
        if(IsOVMSceneLoaded()){
            StartCoroutine(SendDelayedTileSweep());
        }
    }
    IEnumerator SendDelayedTileSweep(){
        yield return new WaitForSeconds(.75f);
        NewWave.Invoke();
    }
    public bool IsOVMSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "OVM")
            {
                return true;
            }
        }
        return false;
    }
    private void ProcessResRequest(string ID){
        CmdProcessResRequest(ID);
    }
    [Command]
    void CmdProcessResRequest(string ID){
        ResCharacter.Invoke(connectionToClient, ID);
    }
    private void LevelUpStartHandler(DateTime timestamp, string charID){
        Debug.Log("Level Up Started at: " + timestamp);
        CmdSendServerLevelUpStart(timestamp.ToString(), charID);
    }
    
    [Command]
    void CmdSendServerLevelUpStart(string timestampString, string charID){
        LevelUpStarted.Invoke(connectionToClient, timestampString, charID);
    }
    private void LevelUpCompleteHandler(DateTime timestamp, string charID){
        Debug.Log("Level Up Completed!");
        CmdSendServerLevelUpComplete(timestamp.ToString(), charID);
    }
    [Command]
    void CmdSendServerLevelUpComplete(string timestampString, string charID){
        LevelUpEnded.Invoke(connectionToClient,timestampString, charID);
    }
    public void ClearTargetSpellChange(MovingObject mo){
        print("Clearing selected and unselecting");
        foreach(var character in selectedCharacters){
            PlayerCharacter pc = character.GetComponent<PlayerCharacter>();
            pc.UnselectedMO();
            pc.UnTargettedMO();
        }
        //if(SelectedMob != null){
        //    SelectedMob.UnselectedMO();
        //    SelectedMob = null;
        //}
        //if(TargetMob != null){
        //    TargetMob.UnTargettedMO();
        //    TargetMob = null;
        //}
        selectedCharacters.Clear();
        CombatPartyView.instance.TurnOffSelectedWindow();
        ToggleSpellsOff.Invoke();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(isLocalPlayer){
            localPlayer = this;
            PlayerCharacter.ResetSpells.AddListener(ClearTargetSpellChange);
            MovingObject.MovedToCast.AddListener(CharacterCastingTargetSpell);
            PlayFabClient.CenterCamera.AddListener(ClearClicker);
            PlayFabClient.BlockCLicker.AddListener(BlockClicker);
            LevelUpPrefab.OnLevelUpStart.AddListener(LevelUpStartHandler);
            LevelUpPrefab.OnLevelUpComplete.AddListener(LevelUpCompleteHandler);
            UILobby.RemoveAdventurer.AddListener(RemoveCharacterFromAdventureList);
            UILobby.PickingAdventurer.AddListener(AddCharacterToAdventureList);
            ContainerPlayerUI.FinishedMatch.AddListener(CmdPermissionToEnd);
            AbilityRankController.PurchaseSkillLevel.AddListener(CmdProcessSpellPurchase);
            CharacterSheet.ChangingSpellRequest.AddListener(CmdProcessSpellChange);
            CharacterTwoSheet.ChangingSpellRequest.AddListener(CmdProcessSpellChange);
            Castbar.CastingSpell.AddListener(CastSpell);
            ContainerUIButtons.RequestOVM.AddListener(RequestOVM);
            ContainerUIButtons.PurchaseRequest.AddListener(RequestPurchase);
            ContainerUIButtons.StackingItem.AddListener(StackItemSend);
            ContainerUIButtons.StashToTactInv.AddListener(StashToTactInventory);
            ContainerUIButtons.StashToTactEquip.AddListener(StashToTactEquipment);
            ContainerUIButtons.StashToTactBelt.AddListener(StashToTactbelt);
            ContainerUIButtons.StashToCharInv.AddListener(StashToCharInventory);
            ContainerUIButtons.StashToCharEquip.AddListener(StashToCharEquipment);
            ContainerUIButtons.TactInvStash.AddListener(TacticianInvToStash);
            ContainerUIButtons.TactInvToTactEquip.AddListener(TacticianInvToTactEquip);
            ContainerUIButtons.TactInvToTactBelt.AddListener(TacticianInvToTactBelt);
            ContainerUIButtons.TactInvToCharInv.AddListener(TacticianInvToCharInv);
            ContainerUIButtons.TactInvToCharEquip.AddListener(TactInvToCharEquipment);
            ContainerUIButtons.TactEquipStash.AddListener(TacticianEquipToStash);
            ContainerUIButtons.TactEquipToTactInv.AddListener(TacticianEquipToTactInv);
            ContainerUIButtons.TactEquipToTactEquip.AddListener(TacticianEquipToTactEquip);
            ContainerUIButtons.TactEquipToTactBelt.AddListener(TacticianEquipToTactBelt);
            ContainerUIButtons.TactEquipToCharInv.AddListener(TacticianEquipToCharInv);
            ContainerUIButtons.TactBeltStash.AddListener(TacticianBeltToStash);
            ContainerUIButtons.TactBeltToTactInv.AddListener(TacticianBeltToTactInv);
            ContainerUIButtons.TactBeltToTactEquip.AddListener(TacticianBeltToTactEquip);
            ContainerUIButtons.TactBeltToCharInv.AddListener(TacticianBeltToCharInv);
            ContainerUIButtons.TactBeltToCharEquip.AddListener(TacticianBeltToCharEquip);

            ContainerUIButtons.CharInvToStash.AddListener(CharacterInvToStash);
            ContainerUIButtons.CharInvToTactInv.AddListener(CharacterInvToTactInventory);
            ContainerUIButtons.CharInvToTactBelt.AddListener(CharacterInvToTactBelt);
            ContainerUIButtons.CharInvToTactEquip.AddListener(CharacterInvToTactEquip);
            ContainerUIButtons.CharEquipToStash.AddListener(CharacterEquipToStash);
            ContainerUIButtons.CharEquipToTactInv.AddListener(CharacterEquipToTacticianInv);
            ContainerUIButtons.CharEquipToTactBelt.AddListener(CharacterEquipToTacticianBelt);
            ContainerUIButtons.DestroyThisItem.AddListener(DestroyingItem);
            ContainerUIButtons.CharInvToCharInv.AddListener(CharacterInvToCharInv);
            ContainerUIButtons.CharInvToCharEquip.AddListener(CharacterInvToCharEquip);
            ContainerUIButtons.CharEquipToCharEquip.AddListener(CharacterEquipToCharEquip);
            ContainerUIButtons.CharEquipToCharInv.AddListener(CharacterEquipToCharInv);
            ContainerUIButtons.CharEquipToCharEquipSame.AddListener(CharacterEquiptoEquipSame);
            ContainerUIButtons.CharEquipToCharInvSame.AddListener(CharacterEquiptoInvSame);
            ContainerUIButtons.CharInvToCharEquipSame.AddListener(CharacterInvtoEquipSame);
            ContainerUIButtons.CharOneSWAPUnequipEquip.AddListener(CharacterOneUnequipToCharEquip);
            ContainerUIButtons.CharTwoSWAPUnequipEquip.AddListener(CharacterTwoUnequipToCharEquip);
            ContainerUIButtons.CharOneSWAPTactStashUnequipEquip.AddListener(CharacterUnequipToTactStash);
            ContainerUIButtons.CharTwoSWAPTactStashUnequipEquip.AddListener(CharacterUnequipToTactStash);
            ContainerUIButtons.BuildingNewStackItem.AddListener(CreateNewStackItem);
            //ContainerUIButtons.CharToTact.AddListener(StashToTactInventory);
            //ContainerUIButtons.TactToChar.AddListener(StashToTactInventory);
           // ContainerUIButtons.CharToChar.AddListener(StashToTactInventory);
            //ContainerPlayerUI.PartyRemove.AddListener(RemovePartyMember);
            ContainerPlayerUI.CharacterFinalize.AddListener(RequestToBuild);
            ContainerPlayerUI.SendingNeedRoll.AddListener(RollNeed);
            ContainerPlayerUI.SendingGreedRoll.AddListener(RollGreed);
            ContainerPlayerUI.SendingPassRoll.AddListener(RollPass);
            ContainerUIButtons.HealParty.AddListener(HealPartyRequest);
            UILobby.PermissionGranted.AddListener(CaptainSaysGo);
            UILobby.PermissionDenied.AddListener(CaptainSaysNo);
            UILobby.CancelMenu.AddListener(TurnClickerOn);
            CharacterSelectedScript.PartyMemberAdd.AddListener(AddPartyMember);
            CharacterSelectedScript.RemoveFromParty.AddListener(RemovePartyMember);
            DeathPrefab.ResFromDragon.AddListener(ProcessResRequest);
            PlayFabClient.OurNodeSet.AddListener(FindOurNode);
            //UIEnergyToggle();
            //inventory = new InventoryManager();
            //uI_Inventory = GameObject.FindGameObjectWithTag("TacticianInventory").GetComponent<UI_Inventory>();
            //uI_Inventory.SetInventory(inventory);
            //LeavingTown.Invoke();
            Loading = false;
            if(currentScene == null || currentScene == string.Empty)
            {
                return;
            }
            //if(currentScene == "OVM"){
            //    //LeavingTown.Invoke();
            //    StartCoroutine(LocalSprite());
            //}
            //TacticianInventory.Callback += OnEquipmentChange;
            // Process initial SyncDictionary payload
            //foreach (KeyValuePair<string, ItemSelectable> kvp in TacticianInventory)
           //OnEquipmentChange(SyncDictionary<string, ItemSelectable>.Operation.OP_ADD, kvp.Key, kvp.Value);
        }
    }
    public override void OnStopClient(){
        Debug.Log($"Client Stopped");
        //ClientDisconnect();
    }
    public override void OnStopServer(){
        //NetworkServer.OnDisconnectedEvent.Invoke(connectionToClient);
        Debug.Log($"Client Stopped on Server");
        //ServerDisconnect();
        //Logout();
    }
    //IEnumerator LocalSprite(){
    //    SpriteRenderer sRend = GetComponent<SpriteRenderer>();
    //    if(loadSprite == null || loadSprite == string.Empty)
    //    {
    //        while(loadSprite == null || loadSprite == string.Empty)
    //        {
    //            yield return null;
    //        }
    //    }
    //    sRend.sprite = Load("Player0", loadSprite);
    //    if(sRend.sprite == null)
    //    {
    //        while(sRend.sprite == null)
    //        {
    //            yield return null;
    //        }
    //    }
    //    sRend.enabled = true;
    //}
    
    public void SpawnCharacterEnterUI(){
        ClientCharacterEnterSpawn();
    }
    [ClientRpc]
    void ClientCharacterEnterSpawn(){
        SpawnClientCharacterEnterUI();
    }
    void SpawnClientCharacterEnterUI(){
        // figure out which character this is supposed to be
    }
    [ClientRpc]
    public void RpcSpawnPlayerUI(string TacticianSprite){
        SpawnClientUI(TacticianSprite);
    }
    void SpawnClientUI(string TacticianSprite){
        //use match to get all the players but be sure to clear them first in the instance of ui lobby on the player parent
            UILobby.instance.SpawnPlayerUIAlltogether(this, TacticianSprite);
    }
    void OnTriggerEnter2D(Collider2D other){
        if(isServer){
            if(other.GetComponent<SceneNode>() != null)
            {
                //print($"Player object landed on node {other.gameObject.name}");
                currentNode = other.gameObject.name;
            }
        }
        if(isLocalPlayer){
            if(other.GetComponent<SceneNode>() != null)
            {
                //print($"Landed on Node: {other.gameObject.name}");
                if(justSpawned)
                {
                    ableToClick = true;
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D collider){
        if(isServer)
        {
            return;
        }
        if(isLocalPlayer){
            // maninipulate menu now 
        //    //print($"Left Node: {collider.gameObject.name}");
            //StartCoroutine(EagleHasLanded(other.gameObject.name));
        }
    }
    public void TownRequest(){
        MoveRequest.Invoke("TOWNOFARUDINE", currentScene);
    }
    // we need a mode to decipher if move grid should be shown or spell grid
    bool SkillShot(string spell){
        if(spell == "Fireball")
            return true;
        if(spell == "Ice Blast")
            return true;
        if(spell == "Gravity Stun")
            return true;
        if(spell == "Group Heal")
            return true;
        if(spell == "Teleport")
            return true;
        if(spell == "Meteor Shower")
            return true;
        if(spell == "Blizzard")
            return true;
        if(spell == "Tornado")
            return true;
        if(spell == "Dash")
            return true;
        if (spell == "Solar Flare")
            return true;
        return false;
    }

    bool AnyTarget(string spell){
        if(spell == "Purge")
            return true;
        if(spell == "Dispel")
            return true;
        return false;
    }
    float SelfCasted(string spell)
    {
        float result = 0f;
        if(spell == "Divine Armor"){
            result = 1f;
        }
        if(spell == "Ice Block"){
            result = 1f;
        }
        if(spell == "Light"){
            result = 1f;
        }
        if(spell == "Solar Flare"){
            result = 1f;
        }
        if(spell == "Nature's Precision"){
            result = 1f;
        }
        if(spell == "Block"){
            result = 1f;
        }
        if(spell == "Tank Stance"){
            result = 1f;
        }
        if(spell == "Offensive Stance"){
            result = 1f;
        }
        if(spell == "Intimidating Roar"){
            result = 1f;
        }
        if(spell == "Protect"){
            result = 1f;
        }
        if(spell == "Sneak"){
            result = 1f;
        }
        if(spell == "Cover"){
            result = 1f;
        }
        if(spell == "Angelic Shield"){
            result = 1f;
        }
        if(spell == "Celestial Wave"){
            result = 1f;
        }
        if(spell == "Shapeshift"){
            result = 1f;
        }
        if(spell == "Rush"){
            result = 1f;
        }
        if(spell == "Track"){
            result = 1f;
        }
        if(spell == "Undead Protection"){
            result = 1f;
        }
        if(spell == "Turn Undead"){
            result = 1f;
        }
        if(spell == "Magic Burst"){
            result = 1f;
        }
        if(spell == "Consecrated Ground"){
            result = 1f;
        }
        if(spell == "Detect Traps"){
            result = 1f;
        }
        if(spell == "Hide"){
            result = 1f;
        }
        return result;
    }
    [Command]
    void CmdCancelSpell(MovingObject pc){
        ServerCancelSpell(pc);
    }
    [Server]
    void ServerCancelSpell(MovingObject pc){
        pc.Casting = false;
        foreach(var player in currentMatch.players){
            TargetCancelSpell(pc);
        }
    }
    [TargetRpc]
    public void TargetCancelSpell(MovingObject pc){
        CancelCast.Invoke(pc);
        //foreach(var key in castingBars){
        //    if(pc == key.Key){
        //        key.Value.CancelingCast(pc);
        //        castingBars.Remove(pc);
        //    }
        //}
    }
    bool isDragging = false;
    Vector2 startPoint;
    public float clickThreshold = 0.1f;
    private Vector2 boxStart;
    private Vector2 boxEnd;
    List<GameObject> selectedCharacters = new List<GameObject>();
    void Update()
    {   
        if(!isLocalPlayer){ return; }
        if(Loading){
            return;
        }
        if(!playerOVMReady){
            return;
        }
        if(ScenePlayer.localPlayer == null){
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q) && currentScene != "OVM" && currentScene != "TOWNOFARUDINE")
		{
            //use combatpartywindow
            MovingObject selectedObject = CombatPartyView.instance.GetSelected();
            
            if(selectedObject == null){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(!selectedObject.GetComponent<NetworkIdentity>().hasAuthority){
                return;
            }
            GameObject spellQGO = SpellManager.instance.GetSpellQ();
            if(spellQGO == null){
                return;
            }
            Spell spellq = spellQGO.GetComponent<Spell>();
            if(spellq == null){
                return;
            }
            PlayerCharacter selectedPlayer = selectedObject.GetComponent<PlayerCharacter>();
            if(!selectedPlayer){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(selectedPlayer.SpellQ == "None"){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have a spell selected for that key");
                return;
            }
            if(selectedPlayer.stamina > 0)
            {
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough stamina to cast {selectedPlayer.SpellQ}");
                return;
            }
            var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellQ, @"^\D*");
            string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
            int _spellRank = 1;
            // Extract spell rank
            var rankMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellQ, @"\d+$");
            if (rankMatch.Success) {
                _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
            }
            if(selectedPlayer.cur_mp < GetSpellCost(spell)){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough MP for {selectedPlayer.SpellQ}");
                return;
            }
            if(!spellq.GetCastable()){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName}'s spell {selectedPlayer.SpellQ} is on cooldown");
                return;
            }
            if(selectedObject.moving){
                //tell server to stop moving
                selectedObject.CmdStopMoving();
            }
            //var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellQ, @"^\D*");
            //string spellNameShaved = nameMatch.Value.Trim(); // Trim any trailing spaces
            float targetable = SelfCasted(spell);
            if(targetable != 0f){
                //cast the spell its a self target

                // add cast time here

                if(spell != "Divine Armor"){
                    selectedPlayer.CmdSelfTarget(selectedPlayer.SpellQ, CastingQ);
                } else {
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer, CastingQ, Input.mousePosition);
                }
                //possibly clear Mode to deselect character?
                return;
            }
            if(SkillShot(spell)){
                CharacterCastingTargetSpell(selectedPlayer, null, CastingQ, Input.mousePosition);
                return;
            }
            if(AnyTarget(spell)){
                CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingQ, Input.mousePosition);
            }
            bool friendlySpell = DetermineFriendly(spell);
            //check if target required
            if(selectedPlayer.Target != null){
                if(selectedPlayer.Target.friendly && friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellQ} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingQ, Input.mousePosition);
                    return;
                } else if(selectedPlayer.Target.friendly && !friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellQ} cannot be cast on a friendly target");
                    return;
                }
                
                if(!selectedPlayer.Target.friendly && !friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellQ} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingQ, Input.mousePosition);
                } else if(!selectedPlayer.Target.friendly && friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellQ} cannot be cast on a hostile target");
                    return;
                }
                //clean this up and make it so that we can see if we need to cast immediately or later
            } else {
                //try to cast aoe on the mouse position?
            }
		}
        if (Input.GetKeyDown(KeyCode.E) && currentScene != "OVM" && currentScene != "TOWNOFARUDINE")
		{
            //use combatpartywindow
            MovingObject selectedObject = CombatPartyView.instance.GetSelected();
            
            if(selectedObject == null){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(!selectedObject.GetComponent<NetworkIdentity>().hasAuthority){
                return;
            }
            GameObject spellEGO = SpellManager.instance.GetSpellE();
            if(spellEGO == null){
                return;
            }
            Spell spellE = spellEGO.GetComponent<Spell>();
            if(spellE == null){
                return;
            }
            PlayerCharacter selectedPlayer = selectedObject.GetComponent<PlayerCharacter>();
            if(!selectedPlayer){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(selectedPlayer.SpellE == "None"){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have a spell selected for that key");
                return;
            }
            if(selectedPlayer.stamina > 0)
            {
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough stamina to cast {selectedPlayer.SpellE}");
                return;
            }
            var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellE, @"^\D*");
            string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
            int _spellRank = 1;
            // Extract spell rank
            var rankMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellE, @"\d+$");
            if (rankMatch.Success) {
                _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
            }
            if(selectedPlayer.cur_mp < GetSpellCost(spell)){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough MP for {selectedPlayer.SpellE}");
                return;
            }
            if(!spellE.GetCastable()){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName}'s spell {selectedPlayer.SpellE} is on cooldown");
                return;
            }
            if(selectedObject.moving){
                //tell server to stop moving
                selectedObject.CmdStopMoving();
            }
            //var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellQ, @"^\D*");
            //string spellNameShaved = nameMatch.Value.Trim(); // Trim any trailing spaces
            float targetable = SelfCasted(spell);
            if(targetable != 0f){
                //cast the spell its a self target
                selectedPlayer.CmdSelfTarget(selectedPlayer.SpellE, CastingE);
                //possibly clear Mode to deselect character?
                return;
            }
            if(SkillShot(spell)){
                CharacterCastingTargetSpell(selectedPlayer, null, CastingE, Input.mousePosition);
                return;
            }
            if(AnyTarget(spell)){
                CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingE, Input.mousePosition);
            }
            bool friendlySpell = DetermineFriendly(spell);
            //check if target required
            if(selectedPlayer.Target != null){
                if(selectedPlayer.Target.friendly && friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellQ} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingE, Input.mousePosition);
                    return;
                } else if(selectedPlayer.Target.friendly && !friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellE} cannot be cast on a friendly target");
                    return;
                }
                
                if(!selectedPlayer.Target.friendly && !friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellE} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingE, Input.mousePosition);
                } else if(!selectedPlayer.Target.friendly && friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellE} cannot be cast on a hostile target");
                    return;
                }
                //clean this up and make it so that we can see if we need to cast immediately or later
            } else {
                //try to cast aoe on the mouse position?
            }
		}
        if (Input.GetKeyDown(KeyCode.R) && currentScene != "OVM" && currentScene != "TOWNOFARUDINE")
		{
            //use combatpartywindow
            MovingObject selectedObject = CombatPartyView.instance.GetSelected();
            
            if(selectedObject == null){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(!selectedObject.GetComponent<NetworkIdentity>().hasAuthority){
                return;
            }
            GameObject spellRGO = SpellManager.instance.GetSpellR();
            if(spellRGO == null){
                return;
            }
            Spell spellR = spellRGO.GetComponent<Spell>();
            if(spellR == null){
                return;
            }
            PlayerCharacter selectedPlayer = selectedObject.GetComponent<PlayerCharacter>();
            if(!selectedPlayer){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(selectedPlayer.SpellR == "None"){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have a spell selected for that key");
                return;
            }
            if(selectedPlayer.stamina > 0)
            {
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough stamina to cast {selectedPlayer.SpellR}");
                return;
            }
            var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellR, @"^\D*");
            string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
            int _spellRank = 1;
            // Extract spell rank
            var rankMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellR, @"\d+$");
            if (rankMatch.Success) {
                _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
            }
            if(selectedPlayer.cur_mp < GetSpellCost(spell)){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough MP for {selectedPlayer.SpellR}");
                return;
            }
            if(!spellR.GetCastable()){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName}'s spell {selectedPlayer.SpellR} is on cooldown");
                return;
            }
            if(selectedObject.moving){
                //tell server to stop moving
                selectedObject.CmdStopMoving();
            }
            //var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellR, @"^\D*");
            //string spellNameShaved = nameMatch.Value.Trim(); // Trim any trailing spaces
            float targetable = SelfCasted(spell);
            if(targetable != 0f){
                //cast the spell its a self target
                selectedPlayer.CmdSelfTarget(selectedPlayer.SpellR, CastingR);
                //possibly clear Mode to deselect character?
                return;
            }
            if(SkillShot(spell)){
                CharacterCastingTargetSpell(selectedPlayer, null, CastingQ, Input.mousePosition);
                return;
            }
            if(AnyTarget(spell)){
                CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingR, Input.mousePosition);
            }
            bool friendlySpell = DetermineFriendly(spell);
            //check if target required
            if(selectedPlayer.Target != null){
                if(selectedPlayer.Target.friendly && friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellR} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingR, Input.mousePosition);
                    return;
                } else if(selectedPlayer.Target.friendly && !friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellR} cannot be cast on a friendly target");
                    return;
                }
                
                if(!selectedPlayer.Target.friendly && !friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellR} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingR, Input.mousePosition);
                } else if(!selectedPlayer.Target.friendly && friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellR} cannot be cast on a hostile target");
                    return;
                }
                //clean this up and make it so that we can see if we need to cast immediately or later
            } else {
                //try to cast aoe on the mouse position?
            }
		}
        if (Input.GetKeyDown(KeyCode.F) && currentScene != "OVM" && currentScene != "TOWNOFARUDINE")
		{
            //use combatpartywindow
            MovingObject selectedObject = CombatPartyView.instance.GetSelected();
            
            if(selectedObject == null){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(!selectedObject.GetComponent<NetworkIdentity>().hasAuthority){
                return;
            }
            GameObject spellFGO = SpellManager.instance.GetSpellF();
            if(spellFGO == null){
                return;
            }
            Spell spellq = spellFGO.GetComponent<Spell>();
            if(spellq == null){
                return;
            }
            PlayerCharacter selectedPlayer = selectedObject.GetComponent<PlayerCharacter>();
            if(!selectedPlayer){
                ImproperCheckText.Invoke($"No target set");
                return;
            }
            if(selectedPlayer.SpellF == "None"){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have a spell selected for that key");
                return;
            }
            if(selectedPlayer.stamina > 0)
            {
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough stamina to cast {selectedPlayer.SpellF}");
                return;
            }
            var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellF, @"^\D*");
            string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
            int _spellRank = 1;
            // Extract spell rank
            var rankMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellF, @"\d+$");
            if (rankMatch.Success) {
                _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
            }
            if(selectedPlayer.cur_mp < GetSpellCost(spell)){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName} does not have enough MP for {selectedPlayer.SpellF}");
                return;
            }
            if(!spellq.GetCastable()){
                ImproperCheckText.Invoke($"{selectedPlayer.CharacterName}'s spell {selectedPlayer.SpellF} is on cooldown");
                return;
            }
            if(selectedObject.moving){
                //tell server to stop moving
                selectedObject.CmdStopMoving();
            }
            //var nameMatch = System.Text.RegularExpressions.Regex.Match(selectedPlayer.SpellF, @"^\D*");
            //string spellNameShaved = nameMatch.Value.Trim(); // Trim any trailing spaces
            float targetable = SelfCasted(spell);
            if(targetable != 0f){
                //cast the spell its a self target
                selectedPlayer.CmdSelfTarget(selectedPlayer.SpellF, CastingF);
                //possibly clear Mode to deselect character?
                return;
            }
            if(SkillShot(spell)){
                CharacterCastingTargetSpell(selectedPlayer, null, CastingQ, Input.mousePosition);
                return;
            }
            if(AnyTarget(spell)){
                CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingF, Input.mousePosition);
            }
            bool friendlySpell = DetermineFriendly(spell);
            //check if target required
            if(selectedPlayer.Target != null){
                if(selectedPlayer.Target.friendly && friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellF} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingF, Input.mousePosition);
                    return;
                } else if(selectedPlayer.Target.friendly && !friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellF} cannot be cast on a friendly target");
                    return;
                }
                
                if(!selectedPlayer.Target.friendly && !friendlySpell){
                    if(selectedPlayer.Target.Dying){
                        ImproperCheckText.Invoke($"{selectedPlayer.SpellF} cannot be cast on a dead target");
                        return;
                    }
                    CharacterCastingTargetSpell(selectedPlayer, selectedPlayer.Target, CastingF, Input.mousePosition);
                } else if(!selectedPlayer.Target.friendly && friendlySpell){
                    ImproperCheckText.Invoke($"{selectedPlayer.SpellF} cannot be cast on a hostile target");
                    return;
                }
                //clean this up and make it so that we can see if we need to cast immediately or later
            } else {
                //try to cast aoe on the mouse position?
            }
		}
        if(currentScene != "OVM" && currentScene != "TOWNOFARUDINE"){
            if(Input.GetMouseButtonDown(1)){
                //print("Right clicked!");
                bool Green = true;
                Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if(selectedCharacters.Count > 0){
                    RaycastHit2D hit = Physics2D.Raycast(targetPos, Vector2.zero, 0f, LayerMask.GetMask("movingObjects"));
                    if (hit.collider == null){
                        // The target position is not blocked
                        RaycastHit2D Floor = Physics2D.Raycast(targetPos, Vector2.zero, 0f, LayerMask.GetMask("Floor"));
                        if(Floor.collider != null){
                            RaycastHit2D Wall = Physics2D.Raycast(targetPos, Vector2.zero, 0f, LayerMask.GetMask("blockingLayer"));
                            if(Wall.collider == null){
                                List<MovingObject> MOs = new List<MovingObject>();
                                Dictionary<MovingObject, int> siblingIndices = new Dictionary<MovingObject, int>();
                                foreach(var selectedChar in selectedCharacters){
                                    MovingObject pc = selectedChar.GetComponent<MovingObject>();
                                    MOs.Add(pc);
                                }
                                foreach(Transform child in CombatPartyView.instance.transform){
                                    CharacterCombatUI _char = child.GetComponent<CharacterCombatUI>();
                                    MovingObject charOwner = _char.owner;
                                    if (MOs.Contains(charOwner)) {
                                        siblingIndices[charOwner] = child.GetSiblingIndex();
                                    }
                                }
                                // Sort MOs based on sibling indices
                                MOs = MOs.OrderBy(mo => siblingIndices[mo]).ToList();
                                CmdMoveUnits(MOs, targetPos);
                            }
                        }
                    } else {
                        MovingObject  selectedTarget = hit.collider.gameObject.GetComponent<MovingObject>();
                        if(!selectedTarget){
                            return;
                        }
                        if(selectedTarget.Dying){
                            return;
                        }
                        List<MovingObject> MOs = new List<MovingObject>();
                        foreach(var selectedChar in selectedCharacters){
                            MovingObject pc = selectedChar.GetComponent<MovingObject>();
                            pc.CmdSetTarget(selectedTarget);
                            MOs.Add(pc);
                        }
                        if(hit.collider.gameObject.tag == "Character"){
                            CmdMoveUnits(MOs, targetPos);//change to follow
                        } else if(hit.collider.gameObject.tag == "Enemy"){
                            SpriteRenderer sRend = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                            if(sRend){
                                if(sRend.enabled){
                                    Green = false;
                                }
                            }
                            CmdAttackUnit(MOs, selectedTarget);//change to autoattack
                        }
                        //selectedTarget.TargettedMO();
                        //CombatPartyView.instance.Retargetter(selectedTarget);
                    }
                }
                Vector3 target3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target3D.z += 10;
                GameObject rightclick = Instantiate(rightClickPrefab, target3D, Quaternion.identity);
                RightClickAnimation RCA = rightclick.GetComponent<RightClickAnimation>();
                if(Green){
                    RCA.StartGreenSequence();
                } else {
                    RCA.StartRedSequence();
                }
                NewTarget.Invoke();
            }
            if(Input.GetKeyDown(KeyCode.Z)){
                CancelLastSelectedCast();
            }
            if (Input.GetKeyDown(KeyCode.Space)){
                ClearTarget();
                TargetHighlightReset.Invoke();
                NewTarget.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                RemoveTarget();
                NewTarget.Invoke();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // If the mouse is over a UI object, don't proceed with the rest of the code
                    return;
                }
                print("Dragging started");
                startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); // changed to Input.mousePosition
                boxStart = Input.mousePosition;
                if(selectedCharacters.Count > 0){
                    RaycastHit2D MovingObjectCheck = Physics2D.Raycast(startPoint, Vector2.zero, 0f, LayerMask.GetMask("movingObjects"));
                    if(MovingObjectCheck.collider != null){
                        MovingObject selectedTarget = MovingObjectCheck.collider.gameObject.GetComponent<MovingObject>();
                        List<MovingObject> MOs = new List<MovingObject>();
                        foreach(var selectedChar in selectedCharacters){
                            MovingObject pc = selectedChar.GetComponent<MovingObject>();
                            pc.CmdSetTarget(selectedTarget);
                            MOs.Add(pc);
                        }
                        Mob mob = selectedTarget.GetComponent<Mob>();
                        if(mob){
                            ClickedTargetMob(mob);
                        }
                        CombatPartyView.instance.Retargetter(selectedTarget);
                    }
                } else {
                    ClearTarget();
                }
                isDragging = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // If the mouse is over a UI object, don't proceed with the rest of the code
                    isDragging = false;
                    return;
                }
                Vector2 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // If mouse movement is smaller than the threshold, consider it a click.
                if (Vector2.Distance(startPoint, endPoint) < clickThreshold)
                {
                    print("thinks we werent dragging");
                    if(selectedCharacters.Count > 0){
                    } else {
                        RaycastHit2D MovingObjectCheck = Physics2D.Raycast(startPoint, Vector2.zero, 0f, LayerMask.GetMask("movingObjects"));
                        if(MovingObjectCheck.collider != null){
                            List<MovingObject> selected = new List<MovingObject>();
                            //if(MovingObjectCheck.collider.GetComponent<MovingObject>().Dying){
                            //    return;
                            //}
                            selected.Add(MovingObjectCheck.collider.GetComponent<MovingObject>());
                            if (MovingObjectCheck.collider.gameObject.tag == "Character" && MovingObjectCheck.collider.GetComponent<NetworkIdentity>().hasAuthority){
                                PlayerCharacter pc = MovingObjectCheck.collider.gameObject.GetComponent<PlayerCharacter>();
                                ClickedCharacter(pc);
                                if(pc.Target != null){
                                    SpriteRenderer sRend = pc.Target.GetComponent<SpriteRenderer>();
                                    CombatPartyView.instance.TurnOnSelectedWindow(selected);
                                    NewTarget.Invoke();
                                    if(!sRend.enabled){
                                        pc.CmdRemoveTarget();
                                    } 
                                } else {
                                    CombatPartyView.instance.TurnOnSelectedWindow(selected);
                                    NewTarget.Invoke();
                                }
                            }
                            if(MovingObjectCheck.collider.gameObject.tag == "Enemy"){
                                Mob mob = MovingObjectCheck.collider.gameObject.GetComponent<Mob>();
                                ClickedMob(mob);
                                CombatPartyView.instance.TurnOnSelectedWindow(selected);
                                NewTarget.Invoke();
                            }
                        }
                    }
                }
                else
                {
                    CheckCharactersInSelectionArea(startPoint, endPoint);
                    print("Dragging ended");

                }

                isDragging = false;
            }
            if (isDragging)
            {
                // Convert the mouse position to viewport coordinates,
                // which are used by the GUI system
                boxEnd = Input.mousePosition;
            }
        }
        if(currentScene != "TOWNOFARUDINE"){
            LocalCamera();
        } else {
            MovePlayer();
        }
        if(currentScene == "OVM")
        {
            if(UILobby.instance == null){
                return;
            }
            if(UILobby.instance.MenuOpen()){
                return;
            }
            if(ContainerUIButtons.Instance.CheckContainerButtonsMenu()){
                return;
            }
            ClientClickedMap();
        }
    }
    void CancelLastSelectedCast(){
        MovingObject lastSelected = CombatPartyView.instance.GetSelected();
        if(lastSelected != null){
            if(lastSelected.GetComponent<NetworkIdentity>().hasAuthority){
                lastSelected.CmdCancelSpell();
            }
        }
    }
    public bool DetermineFriendly(string spell){
        bool friendly = false;
        // Archer Skills
        if (spell == "Aimed Shot")
            friendly = false;
        if (spell == "Bandage Wound")
            friendly = true;
        if (spell == "Fire Arrow")
            friendly = false;
        if (spell == "Penetrating Shot")
            friendly = false;
        if (spell == "Sleep")
            friendly = false;
        if (spell == "Perception")
            friendly = true;
        if (spell == "Head Shot")
            friendly = false;
        if (spell == "Silence Shot")
            friendly = false;
        if (spell == "Crippling Shot")
            friendly = false;
        if (spell == "Dash")
            friendly = true;
        if (spell == "Identify Enemy")
            friendly = false;
        if (spell == "Track")
            friendly = true;
        if (spell == "Double Shot")
            friendly = false;
        if (spell == "Natures Precision")
            friendly = true;

        // Enchanter Skills
        if (spell == "Mesmerize")
            friendly = false;
        if (spell == "Haste")
            friendly = true;
        if (spell == "Root")
            friendly = false;
        if (spell == "Invisibility")
            friendly = true;
        if (spell == "Rune")
            friendly = true;
        if (spell == "Slow")
            friendly = false;
        if (spell == "Magic Sieve")
            friendly = false;
        if (spell == "Aneurysm")
            friendly = false;
        if (spell == "Gravity Stun")
            friendly = false;
        if (spell == "Weaken")
            friendly = false;
        if (spell == "Resist Magic")
            friendly = true;
        if (spell == "Charm")
            friendly = false;
        if (spell == "Mp Transfer")
            friendly = true;

        // Fighter Skills
        if (spell == "Charge")
            friendly = false;
        if (spell == "Bash")
            friendly = false;
        if (spell == "Intimidating Roar")
            friendly = false;
        if (spell == "Protect")
            friendly = true;
        if (spell == "Knockback")
            friendly = false;
        if (spell == "Throw Stone")
            friendly = false;
        if (spell == "Heavy Swing")
            friendly = false;
        if (spell == "Taunt")
            friendly = false;
        // Priest Skills
        if (spell == "Holy Bolt")
            friendly = false;
        if (spell == "Heal")
            friendly = true;
        if (spell == "Cure Poison")
            friendly = true;
        if (spell == "Fortitude")
            friendly = true;
        if (spell == "Turn Undead")
            friendly = false;
        if (spell == "Undead Protection")
            friendly = true;
        if (spell == "Smite")
            friendly = false;
        if (spell == "Shield Bash")
            friendly = false;
        if (spell == "Greater Heal")
            friendly = true;
        if (spell == "Regeneration")
            friendly = true;
        if (spell == "Resurrect")
            friendly = true;

        // Rogue Skills
        if (spell == "Shuriken")
            friendly = false;
        if (spell == "Steal")
            friendly = false;
        if (spell == "Tendon Slice")
            friendly = false;
        if (spell == "Backstab")
            friendly = false;
        if (spell == "Blind")
            friendly = false;
        if (spell == "Poison")
            friendly = false;
        // Wizard Skills
        if (spell == "Ice")
            friendly = false;
        if (spell == "Fire")
            friendly = false;
        if (spell == "Blizzard")
            friendly = false;
        if (spell == "Meteor Shower")
            friendly = false;
        if (spell == "Incinerate")
            friendly = false;
        if (spell == "Brain Freeze")
            friendly = false;
        if (spell == "Magic Missile")
            friendly = false;

        // Druid Skills
        if (spell == "Rejuvination")
            friendly = true;
        if (spell == "Swarm Of Insects")
            friendly = false;
        if (spell == "Thorns")
            friendly = true;
        if (spell == "Nature's Protection")
            friendly = true;
        if (spell == "Strength")
            friendly = true;
        if (spell == "Snare")
            friendly = false;
        if (spell == "Engulfing Roots")
            friendly = false;
        if (spell == "Chain Lightning")
            friendly = false;
        if (spell == "Greater Rejuvination")
            friendly = true;
        

        // Paladin Skills
        if (spell == "Holy Swing")
            friendly = false;
        if (spell == "Divine Armor")
            friendly = true;
        if (spell == "Flash Of Light")
            friendly = true;
        if (spell == "Undead Slayer")
            friendly = false;
        if (spell == "Stun")
            friendly = false;
        if (spell == "Celestial Wave")
            friendly = true;
        if (spell == "Cleanse")
            friendly = true;
        if (spell == "Divine Wrath")
            friendly = false;
        if (spell == "Cover")
            friendly = true;
        if (spell == "Shackle")
            friendly = false;
        if (spell == "Lay On Hands")
            friendly = true;
        if (spell == "Double Attack")
            friendly = true;
        return friendly;
    }
    public void RemoveSelectedCharacter(GameObject deadChar){
        if(selectedCharacters.Contains(deadChar)){
            selectedCharacters.Remove(deadChar);
        }
    }
    public void CheckAuthorityOfCharacter(MovingObject character, MovingObject MobInstance){
        if(character.GetComponent<NetworkIdentity>().hasAuthority){
            foreach(var selected in selectedCharacters){
                MovingObject charCheck = selected.GetComponent<MovingObject>();
                if(charCheck.Target != null){
                    if(charCheck.Target == MobInstance){
                        charCheck.CmdRemoveTarget();
                    }
                }
            }
        }
    }
    void RemoveTarget(){
        foreach(var character in selectedCharacters){
            MovingObject pc = character.GetComponent<MovingObject>();
            pc.UnTargettedMO();
            pc.CmdClearAllStopMoving();
        }
        //if(TargetMob != null){
        //    TargetMob.UnTargettedMO();
        //    TargetMob = null;
        //}
    }
    public void ClearTarget(){
        print("Clearing selected and unselecting");
        foreach(var character in selectedCharacters){
            PlayerCharacter pc = character.GetComponent<PlayerCharacter>();
            pc.UnselectedMO();
            pc.UnTargettedMO();
        }
        //if(SelectedMob != null){
        //    SelectedMob.UnselectedMO();
        //    SelectedMob = null;
        //}
        //if(TargetMob != null){
        //    TargetMob.UnTargettedMO();
        //    TargetMob = null;
        //}
        selectedCharacters.Clear();
        CombatPartyView.instance.TurnOffSelectedWindow();
        ToggleSpellsOff.Invoke();
    }
    [Command]
    void CmdMoveUnits(List<MovingObject> characters, Vector2 targetPos){
        
        OnCharactersMoved.Invoke(targetPos, characters, currentMatch);
    }
    [Command]
    void CmdFollowFriendlyUnit(List<MovingObject> characters, MovingObject target){
        OnCharactersFollow.Invoke(target, characters, currentMatch);
    }
    [Command]
    void CmdAttackUnit(List<MovingObject> characters, MovingObject target){
        OnCharactersAttack.Invoke(target, characters, currentMatch);
    }
    void OnGUI()
{
    if (isDragging)
    {
        // Create a rect object
        Rect rect = GetScreenRect(boxStart, boxEnd); // use GetScreenRect

        // Draw the rect
        GUI.color = new Color(1, 1, 1, 0.5f);
        GUI.Box(rect, "", new GUIStyle { normal = new GUIStyleState { background = Texture2D.whiteTexture } });
        GUI.color = Color.white;
    }
}

private Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
{
    // Move origin from bottom-left to top-left
    screenPosition1.y = Screen.height - screenPosition1.y;
    screenPosition2.y = Screen.height - screenPosition2.y;
    // Calculate corners
    Vector2 topLeft = Vector2.Min(screenPosition1, screenPosition2);
    Vector2 bottomRight = Vector2.Max(screenPosition1, screenPosition2);
    // Create Rect
    return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
}

    void CheckCharactersInSelectionArea(Vector2 startPoint, Vector2 endPoint)
    {
        // Ensure the start point is the bottom-left and the end point is the top-right
        Vector2 min = Vector2.Min(startPoint, endPoint);
        Vector2 max = Vector2.Max(startPoint, endPoint);

        // Get all characters within the selection area
        Collider2D[] charactersInArea = Physics2D.OverlapAreaAll(min, max, LayerMask.GetMask("movingObjects"));
        bool triggered = false;
        // Loop over all characters in the area and add those owned by the player to the selected characters list
        foreach (Collider2D characterCollider in charactersInArea)
        {
            GameObject character = characterCollider.gameObject;
            if (character.CompareTag("Character") && character.GetComponent<NetworkIdentity>().hasAuthority)
            {
                if(!triggered){
                    print("In triggered for checking so it thinks we were dragging to select");
                    ClearTarget();
                    triggered = true;
                }
                PlayerCharacter pc = character.GetComponent<PlayerCharacter>();
                if(pc.Dying){
                    continue;
                }
                selectedCharacters.Add(character);
                pc.SelectedMO();
                selectedCharacterHighlight.Invoke(character);
                print($"Pc {pc.CharacterName} was selected in our drag method!");
            }
        }
        if(selectedCharacters.Count > 0){
            List<MovingObject> selected = new List<MovingObject>();
            foreach(var sChar in selectedCharacters){
                MovingObject mo = sChar.GetComponent<MovingObject>();
                if(mo){
                    selected.Add(mo);
                    //foreach(Transform child in CombatPartyView.instance.transform){
                    //    CharacterCombatUI combatCharUI = child.GetComponent<CharacterCombatUI>();
                    //    if(combatCharUI.owner == mo){
                    //        combatCharUI.Selected();
                    //        break;
                    //    }
                    //}
                }
            }
            CombatPartyView.instance.TurnOnSelectedWindow(selected);
            NewTarget.Invoke();
        }
    }
    float finalRange;
    public bool InSpellRange(MovingObject caster, MovingObject target, string mode, Vector2 mousePosition){
        int lvl = caster.GetComponent<PlayerCharacter>().Level;
        float range = 0f;
        float baseRange = 0f;
        bool inRange = false;
        string _spellname = string.Empty;
        if(mode == CastingQ){
            _spellname = caster.SpellQ;
        }
        if(mode == CastingE){
            _spellname = caster.SpellE;
        }
        if(mode == CastingR){
            _spellname = caster.SpellR;
        }
        if(mode == CastingF){
            _spellname = caster.SpellF;
        }
        var nameMatch = System.Text.RegularExpressions.Regex.Match(_spellname, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(_spellname, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
        // Archer Skills
        if (spell == "Aimed Shot")
            baseRange = 6f;
        if (spell == "Bandage Wound")
            baseRange = 1.5f;
        if (spell == "Head Shot")
            baseRange = 5f;
        if (spell == "Silence Shot")
            baseRange = 5f;
        if (spell == "Crippling Shot")
            baseRange = 5f;
        if (spell == "Dash")
            baseRange = 3f;
        if (spell == "Identify Enemy")
            baseRange = 5f;
        if (spell == "Double Shot")
            baseRange = 6f;
        if (spell == "Fire Arrow")
            baseRange = 6f;
        if (spell == "Penetrating Shot")
            baseRange = 6f;
        if (spell == "Sleep")
            baseRange = 5f;

        // Enchanter Skills
        if (spell == "Mesmerize")
            baseRange = 5f;
        if (spell == "Haste")
            baseRange = 5f;
        if (spell == "Root")
            baseRange = 5f;
        if (spell == "Invisibility")
            baseRange = 5f;
        if (spell == "Rune")
            baseRange = 5f;
        if (spell == "Slow")
            baseRange = 5f;
        if (spell == "Magic Sieve")
            baseRange = 5f;
        if (spell == "Aneurysm")
            baseRange = 5f;
        if (spell == "Gravity Stun")
            baseRange = 5f;
        if (spell == "Weaken")
            baseRange = 5f;
        if (spell == "Resist Magic")
            baseRange = 5f;
        if (spell == "Purge")
            baseRange = 5f;
        if (spell == "Charm")
            baseRange = 5f;
        if (spell == "Mp Transfer")
            baseRange = 1.5f;

        // Fighter Skills
        if (spell == "Charge")
            baseRange = 5f;
        if (spell == "Bash")
            baseRange = 1.5f;
        if (spell == "Intimidating Roar")
            baseRange = 1.5f;
        if (spell == "Protect")
            baseRange = 1.5f;
        if (spell == "Knockback")
            baseRange = 1.5f;
        if (spell == "Throw Stone")
            baseRange = 5f;
        if (spell == "Heavy Swing")
            baseRange = 1.5f;
        if (spell == "Taunt")
            baseRange = 1.5f;
        // Priest Skills
        if (spell == "Holy Bolt")
            baseRange = 5f;
        if (spell == "Heal")
            baseRange = 5f;
        if (spell == "Cure Poison")
            baseRange = 5f;
        if (spell == "Dispel")
            baseRange = 5f;
        if (spell == "Fortitude")
            baseRange = 5f;
        if (spell == "Turn Undead")
            baseRange = 5f;
        if (spell == "Undead Protection")
            baseRange = 3f;
        if (spell == "Smite")
            baseRange = 5f;
        if (spell == "Shield Bash")
            baseRange = 1.5f;
        if (spell == "Greater Heal")
            baseRange = 3f;
        if (spell == "Group Heal")
            baseRange = 4f;
        if (spell == "Resurrect")
            baseRange = 5f;

        // Rogue Skills
        if (spell == "Shuriken")
            baseRange = 5f;
        if (spell == "Picklock")
            baseRange = 1.5f;
        if (spell == "Steal")
            baseRange = 1.5f;
        if (spell == "Tendon Slice")
            baseRange = 1.5f;
        if (spell == "Backstab")
            baseRange = 1.5f;
        if (spell == "Blind")
            baseRange = 5f;
        if (spell == "Poison")
            baseRange = 1.5f;
        // Wizard Skills
        if (spell == "Ice")
            baseRange = 5f;
        if (spell == "Fire")
            baseRange = 5f;
        if (spell == "Blizzard")
            baseRange = 6f;
        if (spell == "Magic Burst")
            baseRange = 1.5f;
        if (spell == "Teleport")
            baseRange = 7f;
        if (spell == "Meteor Shower")
            baseRange = 7f;
        if (spell == "Ice Block")
            baseRange = 1.5f;
        if (spell == "Ice Blast")
            baseRange = 5f;
        if (spell == "Incinerate")
            baseRange = 5f;
        if (spell == "Brain Freeze")
            baseRange = 5f;
        if (spell == "Light")
            baseRange = 5f;
        if (spell == "Magic Missile")
            baseRange = 5f;

        // Druid Skills
        if (spell == "Rejuvination")
            baseRange = 5f;
        if (spell == "Swarm Of Insects")
            baseRange = 5f;
        if (spell == "Thorns")
            baseRange = 5f;
        if (spell == "Nature's Protection")
            baseRange = 5f;
        if (spell == "Strength")
            baseRange = 5f;
        if (spell == "Snare")
            baseRange = 5f;
        if (spell == "Engulfing Roots")
            baseRange = 5f;
        if (spell == "Shapeshift")
            baseRange = 1.5f;
        if (spell == "Tornado")
            baseRange = 5f;
        if (spell == "Chain Lightning")
            baseRange = 5f;
        if (spell == "Greater Rejuvination")
            baseRange = 5f;
        if (spell == "Solar Flare")
            baseRange = 7f;
        if (spell == "Evacuate")
            baseRange = 4f;

        // Paladin Skills
        if (spell == "Holy Swing")
            baseRange = 1.5f;
        if (spell == "Divine Armor")
            baseRange = 5f;
        if (spell == "Flash Of Light")
            baseRange = 5f;
        if (spell == "Undead Slayer")
            baseRange = 0f;
        if (spell == "Stun")
            baseRange = 5f;
        if (spell == "Celestial Wave")
            baseRange = 5f;
        if (spell == "Angelic Shield")
            baseRange = 1.5f;
        if (spell == "Cleanse")
            baseRange = 5f;
        if (spell == "Consecrated Ground")
            baseRange = 1.5f;
        if (spell == "Divine Wrath")
            baseRange = 1.5f;
        if (spell == "Cover")
            baseRange = 1.5f;
        if (spell == "Shackle")
            baseRange = 5f;
        if (spell == "Lay On Hands")
            baseRange = 1.5f;
        range = (baseRange * ((_spellRank - 1) * .004f) + baseRange);
        if(target){
            float distance = Vector2.Distance(caster.transform.position, target.transform.position);
            if(distance <= range)
            {
                inRange = true;

            }
            else
            {
                inRange = false;
            }
        } else {
            float distance = Vector2.Distance(caster.transform.position, mousePosition);
            if(distance <= range)
            {
                inRange = true;

            }
            else
            {
                inRange = false;
            }
        }
        
        print($"{baseRange} is the base range for the spell {spell}");
        finalRange = range;
        return inRange;
    }
    public float GetCastTime(string spell, int charLevel, int _spellRank){
        //var nameMatch = System.Text.RegularExpressions.Regex.Match(_spellName, @"^\D*");
        //string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        //int _spellRank = 1;
        //// Extract spell rank
        //var rankMatch = System.Text.RegularExpressions.Regex.Match(_spellName, @"\d+$");
        //if (rankMatch.Success) {
        //    _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        //}
        float castTimeBase = 0f;
        float castTime = 0f;
        // Archer Skills
        if (spell == "Aimed Shot")
            castTimeBase = 8f;
        if (spell == "Bandage Wound")
            castTimeBase = 0f;
        if (spell == "Head Shot")
            castTimeBase = 4f;
        if (spell == "Silence Shot")
            castTimeBase = 4f;
        if (spell == "Crippling Shot")
            castTimeBase = 4f;
        if (spell == "Dash")
            castTimeBase = 0f;
        if (spell == "Identify Enemy")
            castTimeBase = 10f;
        if (spell == "Track")
            castTimeBase = 0f;
        if (spell == "Double Shot")
            castTimeBase = 4f;
        if (spell == "Natures Precision")
            castTimeBase = 4f;
        if (spell == "Fire Arrow")
            castTimeBase = 4f;
        if (spell == "Penetrating Shot")
            castTimeBase = 4f;
        if (spell == "Sleep")
            castTimeBase = 4f;
        if (spell == "Perception")
            castTimeBase = 0f;

        // Enchanter Skills
        if (spell == "Mesmerize")
            castTimeBase = 4f;
        if (spell == "Haste")
            castTimeBase = 5f;
        if (spell == "Root")
            castTimeBase = 4f;
        if (spell == "Invisibility")
            castTimeBase = 8f;
        if (spell == "Rune")
            castTimeBase = 5f;
        if (spell == "Slow")
            castTimeBase = 6f;
        if (spell == "Magic Sieve")
            castTimeBase = 2f;
        if (spell == "Aneurysm")
            castTimeBase = 2f;
        if (spell == "Gravity Stun")
            castTimeBase = 6f;
        if (spell == "Weaken")
            castTimeBase = 7f;
        if (spell == "Resist Magic")
            castTimeBase = 4f;
        if (spell == "Purge")
            castTimeBase = 4f;
        if (spell == "Charm")
            castTimeBase = 4f;
        if (spell == "Mp Transfer")
            castTimeBase = 4f;

        // Fighter Skills
        if (spell == "Charge")
            castTimeBase = 0f;
        if (spell == "Bash")
            castTimeBase = 0f;
        if (spell == "Intimidating Roar")
            castTimeBase = 1f;
        if (spell == "Protect")
            castTimeBase = 0f;
        if (spell == "Knockback")
            castTimeBase = 2f;
        if (spell == "Throw Stone")
            castTimeBase = 2f;
        if (spell == "Heavy Swing")
            castTimeBase = 4f;
        if (spell == "Taunt")
            castTimeBase = 1f;
        if (spell == "Block")
            castTimeBase = 2f;
        if (spell == "Tank Stance")
            castTimeBase = 4f;
        if (spell == "Offensive Stance")
            castTimeBase = 4f;
        if (spell == "Critical Strike")
            castTimeBase = 0f;
        if (spell == "Riposte")
            castTimeBase = 0f;
        if (spell == "Double Attack")
            castTimeBase = 0f;

        // Priest Skills
        if (spell == "Holy Bolt")
            castTimeBase = 5f;
        if (spell == "Heal")
            castTimeBase = 5f;
        if (spell == "Cure Poison")
            castTimeBase = 2f;
        if (spell == "Dispel")
            castTimeBase = 4f;
        if (spell == "Fortitude")
            castTimeBase = 4f;
        if (spell == "Turn Undead")
            castTimeBase = 2f;
        if (spell == "Critical Heal")
            castTimeBase = 0f;
        if (spell == "Undead Protection")
            castTimeBase = 2f;
        if (spell == "Smite")
            castTimeBase = 4f;
        if (spell == "Shield Bash")
            castTimeBase = 0f;
        if (spell == "Greater Heal")
            castTimeBase = 6f;
        if (spell == "Group Heal")
            castTimeBase = 8f;
        if (spell == "Regeneration")
            castTimeBase = 3f;
        if (spell == "Resurrect")
            castTimeBase = 10f;

        // Rogue Skills
        if (spell == "Shuriken")
            castTimeBase = 4f;
        if (spell == "Hide")
            castTimeBase = 0f;
        if (spell == "Picklock")
            castTimeBase = 10f;
        if (spell == "Steal")
            castTimeBase = 2f;
        if (spell == "Detect Traps")
            castTimeBase = 6f;
        if (spell == "Tendon Slice")
            castTimeBase = 2f;
        if (spell == "Backstab")
            castTimeBase = 1f;
        if (spell == "Rush")
            castTimeBase = 0f;
        if (spell == "Blind")
            castTimeBase = 2f;
        if (spell == "Treasure Finding")
            castTimeBase = 0f;
        if (spell == "Ambidexterity")
            castTimeBase = 0f;
        if (spell == "Poison")
            castTimeBase = 1f;

        // Wizard Skills
        if (spell == "Ice")
            castTimeBase = 4f;
        if (spell == "Fire")
            castTimeBase = 4f;
        if (spell == "Blizzard")
            castTimeBase = 6f;
        if (spell == "Magic Burst")
            castTimeBase = 8f;
        if (spell == "Teleport")
            castTimeBase = 6f;
        if (spell == "Meteor Shower")
            castTimeBase = 6f;
        if (spell == "Spell Crit")
            castTimeBase = 0f;
        if (spell == "Ice Block")
            castTimeBase = 2f;
        if (spell == "Ice Blast")
            castTimeBase = 6f;
        if (spell == "Incinerate")
            castTimeBase = 8f;
        if (spell == "Brain Freeze")
            castTimeBase = 8f;
        if (spell == "Light")
            castTimeBase = 2f;
        if (spell == "Magic Missile")
            castTimeBase = 4f;
        //Druid skills
        if (spell == "Rejuvination")
            castTimeBase = 2f;
        if (spell == "Swarm Of Insects")
            castTimeBase = 3f;
        if (spell == "Thorns")
            castTimeBase = 3f;
        if (spell == "Nature's Protection")
            castTimeBase = 5f;
        if (spell == "Strength")
            castTimeBase = 3f;
        if (spell == "Snare")
            castTimeBase = 2f;
        if (spell == "Engulfing Roots")
            castTimeBase = 2f;
        if (spell == "Shapeshift")
            castTimeBase = 4f;
        if (spell == "Tornado")
            castTimeBase = 4f;
        if (spell == "Chain Lightning")
            castTimeBase = 3f;
        if (spell == "Greater Rejuvenation")
            castTimeBase = 3f;
        //Paladin skills
         if (spell == "Holy Swing")
            castTimeBase = 2f;
        if (spell == "Divine Armor")
            castTimeBase = 4f;
        if (spell == "Flash Of Light")
            castTimeBase = 3f;
        if (spell == "Undead Slayer")
            castTimeBase = 0f;
        if (spell == "Stun")
            castTimeBase = 2f;
        if (spell == "Celestial Wave")
            castTimeBase = 4f;
        if (spell == "Angelic Shield")
            castTimeBase = 3f;
        if (spell == "Cleanse")
            castTimeBase = 3f;
        if (spell == "Consecrated Ground")
            castTimeBase = 2f;
        if (spell == "Divine Wrath")
            castTimeBase = 2f;
        if (spell == "Cover")
            castTimeBase = 0f;
        if (spell == "Shackle")
            castTimeBase = 4f;

        castTime = castTimeBase * (1 - _spellRank * 0.009f);
        return castTime;

    }
    int GetSpellCost(string spell){
        int cost = 0;
        //Tier  **************

        // Archer Skills
        if (spell == "Aimed Shot")
            return 1;
        if (spell == "Bandage Wound")
            return 1;
        // Enchanter Skills
        if (spell == "Mesmerize")
            return 1;
        if (spell == "Haste")
            return 1;
        // Fighter Skills
        if (spell == "Charge")
            return 1;
        if (spell == "Bash")
            return 1;
        // Priest Skills
        if (spell == "Holy Bolt")
            return 1;
        if (spell == "Heal")
            return 1;
        // Rogue Skills
        if (spell == "Shuriken")
            return 1;
        if (spell == "Hide")
            return 1;
        // Wizard Skills
        if (spell == "Ice")
            return 1;
        if (spell == "Fire")
            return 1;
        // Druid Skills
        if (spell == "Rejuvination")
            return 1;
        if (spell == "Swarm Of Insects")
            return 1;
        // Paladin Skills
        if (spell == "Holy Swing")
            return 1;
        if (spell == "Divine Armor")
            return 1;
        //Tier 2 **************
        // Archer Skills
        if (spell == "Head Shot")
            return 1;
        if (spell == "Silence Shot")
            return 1;
        if (spell == "Crippling Shot")
            return 1;
        if (spell == "Dash")
            return 1;
        if (spell == "Identify Enemy")
            return 1;
        if (spell == "Track")
            return 1;
        // Enchanter Skills
        if (spell == "Root")
            return 1;
        if (spell == "Invisibility")
            return 1;
        if (spell == "Rune")
            return 1;
        if (spell == "Slow")
            return 2;
        if (spell == "Magic Sieve")
            return 1;
        if (spell == "Aneurysm")
            return 2;
        // Fighter Skills
        if (spell == "Intimidating Roar")
            return 1;
        if (spell == "Protect")
            return 1;
        if (spell == "Knockback")
            return 1;
        if (spell == "Throw Stone")
            return 1;
        if (spell == "Heavy Swing")
            return 1;
        if (spell == "Taunt")
            return 1;
        // Priest Skills
        if (spell == "Cure Poison")
            return 1;
        if (spell == "Dispel")
            return 1;
        if (spell == "Fortitude")
            return 1;
        if (spell == "Turn Undead")
            return 2;
        if (spell == "Critical Heal")
            return 0;
        if (spell == "Undead Protection")
            return 1;
        // Rogue Skills
        if (spell == "Picklock")
            return 1;
        if (spell == "Steal")
            return 1;
        if (spell == "Detect Traps")
            return 1;
        if (spell == "Tendon Slice")
            return 1;
        if (spell == "Backstab")
            return 1;
        if (spell == "Rush")
            return 1;
        // Wizard Skills
        if (spell == "Fireball")
            return 2;
        if (spell == "Light")
            return 1;
        if (spell == "Magic Missile")
            return 1;
        if (spell == "Spell Crit")
            return 0;
        if (spell == "Ice Block")
            return 2;
        if (spell == "Ice Blast")
            return 2;
        // Druid Skills
        if (spell == "Thorns")
            return 1;
        if (spell == "Nature's Protection")
            return 2;
        if (spell == "Strength")
            return 1;
        if (spell == "Snare")
            return 1;
        if (spell == "Engulfing Roots")
            return 1;
        if (spell == "Shapeshift")
            return 2;
        // Paladin Skills
        if (spell == "Flash Of Light")
            return 1;
        if (spell == "Undead Slayer")
            return 0;
        if (spell == "Stun")
            return 1;
        if (spell == "Celestial Wave")
            return 2;
        if (spell == "Angelic Shield")
            return 1;
        if (spell == "Cleanse")
            return 1;
        //Tier 3 **************
        // Archer Skills
        if (spell == "Fire Arrow")
            return 2;
        if (spell == "Penetrating Shot")
            return 2;
        if (spell == "Sleep")
            return 2;
        if (spell == "Perception")
            return 0;
        // Enchanter Skills
        if (spell == "Gravity Stun")
            return 4;
        if (spell == "Weaken")
            return 2;
        if (spell == "Resist Magic")
            return 2;
        if (spell == "Purge")
            return 1;
        // Fighter Skills
        if (spell == "Block")
            return 2;
        if (spell == "Tank Stance")
            return 1;
        if (spell == "Offensive Stance")
            return 1;
        if (spell == "Critical Strike")
            return 0;
        // Priest Skills
        if (spell == "Smite")
            return 2;
        if (spell == "Shield Bash")
            return 1;
        if (spell == "Greater Heal")
            return 2;
        if (spell == "Group Heal")
            return 4;
        // Rogue Skills
        if (spell == "Blind")
            return 1;
        if (spell == "Treasure Finding")
            return 0;
        if (spell == "Ambidexterity")
            return 0;
        if (spell == "Poison")
            return 1;
        // Wizard Skills
        if (spell == "Blizzard")
            return 6;
        if (spell == "Magic Burst")
            return 1;
        if (spell == "Teleport")
            return 1;
        if (spell == "Meteor Shower")
            return 8;
        // Druid Skills
        if (spell == "Staff Specialization")
            return 0;
        if (spell == "Tornado")
            return 5;
        if (spell == "Chain Lightning")
            return 4;
        if (spell == "Greater Rejuvination")
            return 2;
        // Paladin Skills
        if (spell == "Consecrated Ground")
            return 1;
        if (spell == "Divine Wrath")
            return 2;
        if (spell == "Cover")
            return 1;
        if (spell == "Shackle")
            return 1;
        //Tier 4 ****************
        // Archer Skills
        if (spell == "Double Shot")
            return 2;
        if (spell == "Natures Precision")
            return 1;
        // Enchanter Skills
        if (spell == "Charm")
            return 4;
        if (spell == "Mp Transfer")
            return 6;
        // Fighter Skills
        if (spell == "Riposte")
            return 0;
        if (spell == "Double Attack")
            return 0;
        // Priest Skills
        if (spell == "Regeneration")
            return 2;
        if (spell == "Resurrect")
            return 10;
        // Rogue Skills
        if (spell == "Double attack")
            return 0;
        if (spell == "Sneak")
            return 1;
        // Wizard Skills
        if (spell == "Incinerate")
            return 4;
        if (spell == "Brain Freeze")
            return 4;
        // Druid Skills
        if (spell == "Solar Flare")
            return 3;
        if (spell == "Evacuate")
            return 5;
        // Paladin Skills
        if (spell == "Lay On Hands")
            return 3;
        if (spell == "Double Attack")
            return 0;
        return cost;
    }
    void CancelMenu(){
        ableToClick = true;
    }
    void ClientClickedMap(){
        //SetSprite(true);
        if (Input.GetMouseButtonDown(0) && LerpInProgress == false) //&& menuOpened == false)
        {
            if(ableToClick){
                StartCoroutine(FindTiles());
            } else {
                //print("Cant click right now!");
            }
        }
            
    }
private SceneNode FindNodeByName(string nodeName)
{
    if(!isServer)
    {
        AddAllSceneNodesToDictionary();
    }
    if (sceneNodesDictionary.TryGetValue(nodeName, out SceneNode node))
    {
        return node;
    }
    
    // if no matching node is found, return null
    return null;
}
    IEnumerator FindTiles() {
    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

    Collider2D nodeHit = null;

    foreach (RaycastHit2D hit in hits)
    {
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Node"))
        {
            nodeHit = hit.collider;
            break;
        }
    }

    if(nodeHit == null || !nodeHit.CompareTag("Node") || LerpInProgress != false) {
        //not a valid route
        yield break;
    }

    if (nodeHit.CompareTag("Node") && LerpInProgress == false && nodeHit.gameObject.name == currentNode){
        SceneNode targetNode = nodeHit.GetComponent<SceneNode>();
        if(nodeHit.gameObject.name == "TOWNOFARUDINE"){
            LeftTownOpenNode.Invoke("100");
            ableToClick = false;
            yield break;
        } else {
            if(Energy >= targetNode.EnergyEnterNodeCost){
                LeftTownOpenNode.Invoke($"{targetNode.EnergyEnterNodeCost}");
                ableToClick = false;
            } else {
                ImproperCheckText.Invoke($"Entering {targetNode.nodeName} requires {targetNode.EnergyEnterNodeCost.ToString()} energy");
            }
            yield break;
        }
    } else if(nodeHit.CompareTag("Node") && LerpInProgress == false) {
        SceneNode targetNode = nodeHit.GetComponent<SceneNode>();
        if(targetNode != null) {
            //Check if targetNode is a neighbor of OurNode
            NodeWaypointsPair neighbor = OurNode.GetNeighborNodesFromNode().Find(pair => pair.Node == targetNode);
            if(neighbor != null && Energy >= targetNode.EnergyTravelCost) {
                TriggerAsk.Invoke(targetNode.gameObject.name, targetNode.EnergyTravelCost, targetNode.TimeTravelDuration);
                ableToClick = false;
                acceptedPayment = false;
                noWasClicked = false;
                while(!acceptedPayment) {
                    if(noWasClicked){
                        noWasClicked = false;
                        ableToClick = true;
                        acceptedPayment = false;
                        yield break;
                    }
                    yield return null;
                }
                float tempEnergy = Energy - targetNode.EnergyTravelCost;
                UIToggle.Invoke(tempEnergy);
                OurNode = targetNode;
                TimerOVM.instance.StartTimer(targetNode.TimeTravelDuration);
                //Send timer the node count which is
                //OurNode.TimeTravelDuration        
                CmdMoveToNode(nodeHit.gameObject.GetComponent<NetworkIdentity>()); //pass the NetworkIdentity of the target node
            } else {
                if(targetNode.nodeName == "TOWNOFARUDINE"){
                    ImproperCheckText.Invoke($"Traveling to Town of Arudine requires {targetNode.EnergyTravelCost.ToString()} energy");
                } else {
                    ImproperCheckText.Invoke($"Traveling to {targetNode.nodeName} requires {targetNode.EnergyTravelCost.ToString()} energy");
                }
            }
        }
    }
}
[Command]
void CmdMoveToNode(NetworkIdentity targetNodeId) {
    SceneNode targetNode = targetNodeId.GetComponent<SceneNode>();
    //print($"made it to the cmdMovetoNode {targetNode.nodeName} is node we clicked on");
    if(targetNode != null && Energy >= targetNode.EnergyTravelCost){
        //OurNode = targetNode;
        //print("startingServerTravelToNode");        
        StartCoroutine(ServerTravelToNode(targetNode, targetNode.EnergyTravelCost));
    }
}
[Server]
IEnumerator ServerTravelToNode(SceneNode targetNode, float energyCost) {
    // Save current node as last node
    string lastNode = currentNode;
    // Get the waypoints for the target node
    if (!sceneNodesDictionary.ContainsKey(currentNode)) {
        Debug.LogError("Current node not found in the sceneNodesDictionary");
        yield break;  // Exit the coroutine
    }
    // Get SceneNode from the sceneNodesDictionary
    SceneNode OurNode = sceneNodesDictionary[currentNode];
    // Get the waypoints for the target node
    NodeWaypointsPair nodeWaypointsPair = null;
    foreach (NodeWaypointsPair pair in OurNode.GetNeighborNodesFromNode()) {
        if (pair.Node == targetNode) {
            nodeWaypointsPair = pair;
            break;
        }
    }
    List<Transform> waypoints = nodeWaypointsPair?.Waypoints ?? new List<Transform>();
    float totalTravelTime = targetNode.TimeTravelDuration;
    float waypointTravelTime = totalTravelTime / waypoints.Count;

    int waypointCount = waypoints.Count;
    float timeSinceLastRoll = 0f;

    for (int i = 0; i < waypointCount; i++) {
        Transform waypoint = waypoints[i];
        Vector2 startPosition = transform.position;
        Vector2 endPosition = waypoint.position;

        LerpInProgress = true;
        float elapsedTime = 0;

        while (elapsedTime < waypointTravelTime) {
            transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / waypointTravelTime);
            elapsedTime += Time.deltaTime;
            timeSinceLastRoll += Time.deltaTime;

            if (i != waypointCount - 1 && timeSinceLastRoll >= 1f) {
                int roll = UnityEngine.Random.Range(1, 101);
                print($"Got a {roll} roll for a random match");
                if (roll <= 6) {
                    print($"Got a random match to spawn at this time {DateTime.Now}");
                }
                timeSinceLastRoll = 0f;
            }
            yield return null;
        }
        justSpawned = false;
        // Snap to final position
        transform.position = endPosition;
    }
    // Once all waypoints (including final node) have been reached...
    OurNode = targetNode;
    currentNode = targetNode.name;
    Energy -= energyCost;
    if (OnPlayerDataUpdateRequest != null) {
        OnPlayerDataUpdateRequest.Invoke(connectionToClient, Energy.ToString(), "OVM", currentNode);
    }
    LerpInProgress = false;
    TargetOpenNodeMenu(targetNode.EnergyEnterNodeCost.ToString());
    //Send the open signal
    }
    [TargetRpc]
    void TargetOpenNodeMenu(string entry){

        LeftTownOpenNode.Invoke($"{entry}");
    }
/*
[Server]
IEnumerator ServerTravelToNode(Transform nodePosition, float energyCost){
    // Save current node as last node
    string lastNode = currentNode;

    // Start node travel
    float travelTime = 2.0f; // or whatever your travel time calculation is
    Vector2 startPosition = transform.position;
    Vector2 endPosition = nodePosition.position;

    // Flag that lerp is in progress
    LerpInProgress = true;

    float elapsedTime = 0;
    while(elapsedTime < travelTime)
    {
        transform.position = Vector2.Lerp(startPosition, endPosition, (elapsedTime / travelTime));
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Snap to final position
    transform.position = endPosition;

    // Update current node
    currentNode = nodePosition.gameObject.name;

    // Reduce energy
    Energy -= energyCost;

    // Save this data to PlayFab
    if (OnPlayerDataUpdateRequest != null)
    {
        OnPlayerDataUpdateRequest.Invoke(connectionToClient, Energy.ToString(), "OVM", lastNode);
    }

    // Flag that lerp is complete
    LerpInProgress = false;
}
*/
    //void FindTile(){
    //    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //            //valueHolder = worldPoint;
    //    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
    //    if( hit == false ||hit.collider == null && !hit.collider.CompareTag("Node") && LerpInProgress != false){
    //        return;
    //    }
    //    if (hit.collider != null && hit.collider.CompareTag("Node") && LerpInProgress == false && hit.collider.gameObject.name == currentNode){
    //        if(hit.collider.gameObject.name == "TOWNOFARUDINE"){
    //            ReopenTown.Invoke();
    //            return;
    //        }else{
    //            ReopenConnect.Invoke();
    //            return;
    //        }
    //    }
    //    float distX = this.transform.position.x - hit.transform.position.x;
    //    float distY = this.transform.position.y - hit.transform.position.y;
    //    if(distY == 7f && distX == 0.0f|| distY == -7f && distX == 0.0f || distY == 0.0f && distX == 7f || distY == 0.0f && distX == -7f || distY < 7.0f && distY > -7.0f && distX > -7f && distX < -7f  )
    //    { 
    //        if(hit.collider != null && hit.collider.CompareTag("Node") && LerpInProgress == false){
    //            //add energy pop up
    //            //StartCoroutine(ProcessEnergyPaymentRequest(hit.collider.transform));
    //           // if(hit.collider.name != "TOWNOFARUDINE"){
    //            float tempEnergy = Energy - 100;
    //            UIToggle.Invoke(tempEnergy, true);
    //            CmdMoveToNode(hit.collider.transform);
    //        } 
    //    }
    //}
    //IEnumerator ProcessEnergyPaymentRequest(Transform nodePosition){
    //    //invoke event to trigger the waiting acceptenace, dont forget to reset
    //    waitingAcceptance = false;
    //    rejected = false;
    //    while (waitingAcceptance == true)
    //    {
    //        yield return null;
    //    }
    //    if(rejected == true){
    //        waitingAcceptance = true;
    //        rejected = false;
    //        yield break;
    //    }
    //    CmdMoveToNode(nodePosition);
    //}
    //[Command]
    //void CmdMoveToNode(Transform nodePosition){
    //    if(Energy >= requirement){
    //        ServerMoveToNode(nodePosition);
    //    }
    //}
    //[Server]
    //void ServerMoveToNode(Transform nodePosition){
    //    tempName = nodePosition.gameObject.name;
    //    travelNode = tempName;
    //    //print($"{tempName} is temp, {currentNode} is currentNOde");
    //    if(Energy >= requirement){
    //        transformProcessing = nodePosition;
    //        //print($"Sending final request to {this.connectionToClient.connectionId}");
    //        FinalRequest.Invoke(this.connectionToClient, charliesTicket);
    //        // create bool that turns on while waiting for response from server saying it saved the data and then turn travelToNode on
    //        // have animation of footprints going to the node?
    //        //do energy call and save new x and y position
    //        //turn off minimap !!
    //    }
    //}
    [TargetRpc]
    public void TargetUpdateEnergyDisplay(float _energy){
        //print($"{_energy} is our energy for player {playerName} at time: {Time.time}");
        UIToggle.Invoke(_energy);
        
    }
    //[Server]
    //public void TravelToTheNode(){
    //    currentNode = tempName;
    //    //print($"{tempName} is temp, {currentNode} is currentNOde");
    //    StartCoroutine(ServerTravelToNode(transformProcessing));
    //}
    //
    //[Server]
    //// runs on server
    //IEnumerator ServerTravelToNode(Transform nodePosition){
    //    if(justSpawned){
    //        justSpawned = false;
    //    }
    //    //print($"New Energy = {Energy}");
    //    travelTarget = nodePosition.position;    
    //    float duration = 2.0f;
    //    LerpInProgress = true;
    //    float time = 0;
    //    Vector2 startPosition = transform.position;
    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        timerTime = time; // duration;
    //        yield return null;
    //    }
    //    transform.position = travelTarget;
	//	LerpInProgress = false;
    //}
    
    void LocalCamera(){
        cameraController = GameObject.Find("Camera Controller").transform;
        float speed = 14f;
		if (Input.GetKey(KeyCode.W))
			cameraController.Translate(new Vector3(0,speed * Time.deltaTime,0));
		if (Input.GetKey(KeyCode.A))
			cameraController.Translate(new Vector3(-speed * Time.deltaTime,0,0));
		if (Input.GetKey(KeyCode.S))
			cameraController.Translate(new Vector3(0,-speed * Time.deltaTime,0));
		if (Input.GetKey(KeyCode.D))
			cameraController.Translate(new Vector3(speed * Time.deltaTime,0,0));
    }
    void MovePlayer()
{
    Transform playerTransform = gameObject.transform; // Assuming this script is attached to the player GameObject
    cameraController = GameObject.Find("Camera Controller").transform; // Find Camera Controller

    float speed = 5f; // Speed is set to 14 to match LocalCamera
    if (Input.GetKey(KeyCode.W))
        cameraController.Translate(new Vector3(0, 0, speed * Time.deltaTime));
    if (Input.GetKey(KeyCode.A))
        cameraController.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
    if (Input.GetKey(KeyCode.S))
        cameraController.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
    if (Input.GetKey(KeyCode.D))
        cameraController.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    
    //cameraController.position = playerTransform.position + new Vector3(0, 5, -5); // Set this offset as per your needs
    Vector3 moveDirection = Vector3.zero;

    if (Input.GetKey(KeyCode.W))
        moveDirection += Vector3.forward * speed * Time.deltaTime;
    if (Input.GetKey(KeyCode.A))
        moveDirection += Vector3.left * speed * Time.deltaTime;
    if (Input.GetKey(KeyCode.S))
        moveDirection += Vector3.back * speed * Time.deltaTime;
    if (Input.GetKey(KeyCode.D))
        moveDirection += Vector3.right * speed * Time.deltaTime;

    CmdMovePlayer(moveDirection);

}
    [Command]

void CmdMovePlayer(Vector3 moveDirection){
    Transform playerTransform = gameObject.transform;
    playerTransform.Translate(moveDirection, Space.World);
}
    //*******************************************
	//*********NODE LOBBY CONTROLS***************
	//*******************************************

     /*
        HOST MATCH
    */
    void RemoveCharacterFromAdventureList(string charSlot){
        CmdRemoveCharacterFromAdventureList(charSlot);
    }
    [Command]
    void CmdRemoveCharacterFromAdventureList(string charSlot){
        ServerRemoveCharacterFromAdventureList(charSlot);
    }
    [Server]
    void ServerRemoveCharacterFromAdventureList(string charSlot){
        //print("Got to server remove character");
        MatchMaker.instance.RemoveCharacterVote(this, charSlot, currentMatch);

    }
    [TargetRpc]
    public void TargetRemoveAdventurer(ScenePlayer sPlayer, string serial){
        //UILobby.instance.SpawnAdventurer(sPlayer,selectedCharacter, spriteName, serial);
        //if(sPlayer.currentMatch == ScenePlayer.localPlayer.currentMatch){
            RemoveAdventurer.Invoke(sPlayer, serial);
        //}
    }
    void AddCharacterToAdventureList(string charName, string spriteName, string charSlot){
        CmdAddCharacterToAdventureList(charName, spriteName, charSlot);
    }
    [Command]
    void CmdAddCharacterToAdventureList(string charName, string spriteName, string charSlot){
        ServerAddCharacterToAdventureList(charName, spriteName, charSlot);
    }
    [Server]
    void ServerAddCharacterToAdventureList(string charName, string spriteName, string charSlot){
        MatchMaker.instance.CastCharacterVote(this, charSlot, currentMatch, charName, spriteName);
    }
    [TargetRpc]
    public void TargetSpawnAdventurer(ScenePlayer sPlayer, string selectedCharacter, string spriteName, string serial){
        UILobby.instance.SpawnAdventurer(sPlayer,selectedCharacter, spriteName, serial);
    }
    public void HostGame(bool publicMatch){
        CmdHostGame(publicMatch);
    }

    [Command]
    void CmdHostGame(bool publicMatch){
        matchID = MatchMaker.GetRandomMatchID();
        
        if (MatchMaker.instance.HostGame(matchID, this, publicMatch, out int _playerIndex)) {
            Debug.Log($"Game {matchID} hosted successfully, Public game?: {publicMatch}");
             playerIndex = _playerIndex;
            //networkMatchChecker.matchId = _matchID.ToGuid();
            List<SavedPartyList> savedParty = new List<SavedPartyList>();
            foreach(var member in GetParty()){
                    foreach(var sheet in GetInformationSheets()){
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
                    owner = this
                });
            TargetHostGame(true, matchID, playerIndex, loadSprite, partyList);
           // StartCoroutine(SendPartyInfoToClient(partyList));
        } else {
            Debug.Log($"<color=red>Game hosted failed:</color>");
            TargetHostGame(false, matchID, playerIndex, loadSprite, new ClientPartyInformation());

        }

    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID, int _playerindex, string tactSprite, ClientPartyInformation savedList){
        if(!success){
            return;
        }
        foreach(var key in savedList.Party){
            // Only add if the key does not exist in InspectParty
            if (!InspectParty.ContainsKey(key.Key))//make the class to look at these in inspector
            {
                InspectParty.Add(key.Key, key.Value);
                //print($"added {key.Key} to inspectorlist and they have sprite {key.Value}");
            }
        }
        playerIndex = _playerindex;
        matchID = _matchID;
        //isMatchLeader = true;
        Debug.Log($"Match ID: {matchID} == {_matchID}");
        UILobby.instance.HostSuccess (success, _matchID, tactSprite);

    }
    [Server]
    IEnumerator SendPartyInfoToClient(ClientPartyInformation partyList){
        yield return new WaitForSeconds(2f);
        RpcBuildPartyInspector(partyList);
    }
    /*
        JOIN MATCH
    */
    public void JoinGame (string _inputID) {
        CmdJoinGame (_inputID);
    }
    [Command]
    void CmdJoinGame (string _matchID) {
        matchID = _matchID;
        ScenePlayer Host;
        if (MatchMaker.instance.JoinGame(_matchID, this, out int _playerIndex, out Host)) {
            Debug.Log($"Game hosted successfully CMDJOINGAME");
            playerIndex = _playerIndex;
            matchID = _matchID;
            //networkMatchChecker.matchId = _matchID.ToGuid();
            TargetJoinGame(true, _matchID, _playerIndex);
            /*
            foreach(var partyMember in currentMatch.players){
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
                    owner = this
                });
                StartCoroutine(SendPartyInfoToClient(partyList));
            }
                */

        } else {
            Debug.Log($"<color=red>Game hosted failed:</color>");
            TargetJoinGame(false, _matchID, playerIndex);
        }
    }
    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, int _playerIndex){
        Debug.Log($"Match ID: {matchID} == {_matchID}");
        UILobby.instance.JoiningSucces (success, _matchID);
    }
    /*
        SEARCH MATCH
    */

    public void SearchGame() {
        CmdSearchGame();
    }
    [Command]
    void CmdSearchGame () {
        if (MatchMaker.instance.SearchGame(this, out int _playerIndex, out string _matchID)) {
            Debug.Log($"Game found CMDSEARCHGAME");
            playerIndex = _playerIndex;
            matchID = _matchID;
            //networkMatchChecker.matchId = matchID.ToGuid();
            TargetSearchGame(true, matchID, playerIndex);
            //Host
            if (isServer && playerLobbyUI != null) {
                playerLobbyUI.SetActive(true);
            }
        } else {
            Debug.Log($"<color=red>Game not found</color>");
            TargetSearchGame(false, matchID, playerIndex);
        }
    }
    [TargetRpc]
    void TargetSearchGame(bool success, string _matchID, int _playerIndex){
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"Match ID: {matchID} == {_matchID}");
        UILobby.instance.SearchSuccess (success, _matchID);
    }
    
    
    /*
        BEGIN MATCH
    */
    public void BeginGame() {
        //print("Beginning game from start game");
        CmdBeginGame(ScenePlayer.localPlayer);
    }
    

    [Command]
    void CmdBeginGame(ScenePlayer player) {
        ServerBeginGame(player);
    }
    [Server]
    void ServerBeginGame(ScenePlayer player){
        Debug.Log($"Game Beginning CMDBEGINGAME");
        if (matchID == string.Empty)
        { 
            //print("Bugged out from match maker disconnect");
        }
        foreach(var sPlayer in player.currentMatch.players){
            TargetRemoveLobby();
        }
        MatchMaker.instance.CreateGame (player, matchID, currentMatch);
    }
    [TargetRpc]
    void TargetRemoveLobby(){
        BeginGameClearLobby.Invoke();
    }
    
    [TargetRpc]
    public void TargetGetReadyForStart(){
        //lets try to turn off screen here with a call to load bar
        UILobby.instance.RemoveLobby();
        LoadbarOnToggle.Invoke();
    }
    public void SoloGame(){
        CmdSoloGame();
    }
    [Command]
    void CmdSoloGame(){
        matchID = MatchMaker.GetRandomMatchID();
        //print("called CMDSOLOGAME ************* on server");
        if (MatchMaker.instance.HostSoloGame(matchID, this, false, out playerIndex)) {
            Debug.Log($"Game {matchID} Solo created successfully");
        } 
    }
    /*
        DISCONNECT MATCH
    */
    [TargetRpc]
    public void TargetPassLeader(bool leader){
        Debug.Log($"Server passed our client leadership of the match");
        UILobby.instance.LeaderToggle(leader);
    }
    

    public void DisconnectMatchLobby(){
        CmdDisconnectFromMatchLobby();
    }
    [Command]
    void CmdDisconnectFromMatchLobby(){
        MatchMaker.instance.PlayerDisconnectedFromLobby(this, matchID);
        
        //ServerDisconnectFromMatchLobby();
    }
    [Server]
    void ServerDisconnectFromMatchLobby(){
        //matchID = string.Empty;
        //playerIndex = 0;
        //RpcDisconnectFromMatchLobby();
    }
    [TargetRpc]
    public void TargetDisconnectFromMatchLobby(ScenePlayer sPlayer){
        DisconnectFromMatchLobby(sPlayer);
    }
    [ClientRpc]
    public void rpcDisconnectFromMatchLobby(){
        DisconnectFromMatchLobby(this);
    }
    void DisconnectFromMatchLobby(ScenePlayer sPlayer){
        RemoveLobby.Invoke(sPlayer);
        PlayerDisconnectedFromMatchLobby.Invoke();
    }
    [Server]
    void Logout(){
        DateTime LogoutTime = DateTime.Now;
        string time = LogoutTime.ToString();
        float x = this.gameObject.transform.position.x;
        float y = this.gameObject.transform.position.y;
    }
    [TargetRpc]
    public void TargetCloseGameCompletely(){
        Application.Quit();
    }
    [ClientRpc]
    void RpcDisconnectGame(){
        ClientDisconnect();
    }
    void ClientDisconnect(){
        if(playerLobbyUI != null){
            if (!isServer){
                Destroy(playerLobbyUI);
            } else {
                playerLobbyUI.SetActive(false);
            }
        }
    }
    

    [TargetRpc]
    public void TargetShowCastTile(Vector3 mousePosition, float duration){
        GameObject tileObject = Instantiate(tilePrefab, mousePosition, Quaternion.identity);
        MoveableTile castTile = tileObject.GetComponent<MoveableTile>();
        castTile.spawnDie = castTile.StartCoroutine(castTile.SelectedAbilityTile(duration));
    }
    [ClientRpc]
    public void RpcCastBar(MovingObject pc, float duration, string mode, MovingObject target, ScenePlayer owner){
        //print($"Clicked cast location is {mousePosition}");
        GameObject tileObject = Instantiate(tilePrefab, new Vector3(target.transform.position.x, target.transform.position.y, 0f), Quaternion.identity);
        MoveableTile castTile = tileObject.GetComponent<MoveableTile>();
        GameObject castBarObject = Instantiate(castBarPrefab, new Vector3(pc.transform.position.x, pc.transform.position.y - .8f, 0f) , Quaternion.identity);
        Castbar castbar = castBarObject.GetComponent<Castbar>();
        castTile.StartCoroutine(castTile.SelectedAbilityTile(duration));
        castbar.SetMob(pc, duration, mode, target, castTile, owner);
        //pc.PlayCastSound();
    }
    [ClientRpc]
    public void RpcCastBarAOESPELL(MovingObject pc, float duration, string mode, Vector2 target, ScenePlayer owner){
        //print($"Clicked cast location is {mousePosition}");
        GameObject tileObject = Instantiate(tilePrefab, target, Quaternion.identity);
        MoveableTile castTile = tileObject.GetComponent<MoveableTile>();
        GameObject castBarObject = Instantiate(castBarPrefab, new Vector3(pc.transform.position.x, pc.transform.position.y - .8f, 0f) , Quaternion.identity);
        Castbar castbar = castBarObject.GetComponent<Castbar>();
        castTile.StartCoroutine(castTile.SelectedAbilityTile(duration));
        castbar.SetTargetPositionAOESPELL(pc, duration, mode, target, castTile, owner);
        //pc.PlayCastSound();
    }
    [Command]
    void CmdBuildCastBar(MovingObject pc, string mode, MovingObject target, ScenePlayer owner, Vector2 MousePosition){
        string _spell = string.Empty;
        if(mode == CastingQ){
            _spell = pc.SpellQ;
        }
        if(mode == CastingE){
            _spell = pc.SpellE;
        }
        if(mode == CastingR){
            _spell = pc.SpellR;
        }
        if(mode == CastingF){
            _spell = pc.SpellF;
        }
        pc.Casting = true;
        var nameMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"^\D*");
        string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
        int _spellRank = 1;
        // Extract spell rank
        var rankMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"\d+$");
        if (rankMatch.Success) {
            _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
        }
        float castTime = GetCastTime(spell, pc.GetComponent<PlayerCharacter>().Level, _spellRank);
        //print($"building cast bar for {pc.CharacterName} who is casting spell on keybind {mode}");
        if(mode == CastingQ){
            if(pc.cur_mp >= GetSpellCost(spell)){
                pc.Casting = true;
                //if aoe do this 
                if(SkillShot(spell)){
                    RpcCastBarAOESPELL(pc, castTime, mode, MousePosition, owner);
                    return;
                }
                RpcCastBar(pc, castTime, mode, target, owner);
                return;
            }
        }
        if(mode == CastingE){
            if(pc.cur_mp >= GetSpellCost(spell)){
                pc.Casting = true;
                //if aoe do this 
                if(SkillShot(spell)){
                    RpcCastBarAOESPELL(pc, castTime, mode, MousePosition, owner);
                    return;
                }
                RpcCastBar(pc, castTime, mode, target, owner);
                return;
            }
        }
        if(mode == CastingR){
            if(pc.cur_mp >= GetSpellCost(spell)){
                pc.Casting = true;
                //if aoe do this 
                if(SkillShot(spell)){
                    RpcCastBarAOESPELL(pc, castTime, mode, MousePosition, owner);
                    return;
                }
                RpcCastBar(pc, castTime, mode, target, owner);
                return;
            }
        }
        if(mode == CastingF){
            if(pc.cur_mp >= GetSpellCost(spell)){
                pc.Casting = true;
                //if aoe do this 
                if(SkillShot(spell)){
                    RpcCastBarAOESPELL(pc, castTime, mode, MousePosition, owner);
                    return;
                }
                RpcCastBar(pc, castTime, mode, target, owner);
                return;
            }
        }
    }
    void CastSpell(CastingSpellTargetsOrMouse casting){
        if(casting.character.GetComponent<NetworkIdentity>().hasAuthority){
            if(casting.owner == this){
                if(casting.AOE){
                    casting.character.CmdCastAOESpell(casting.mode, casting.Clicked);
                } else {
                    if(InSpellRange(casting.character, casting.target, casting.mode, new Vector2())){
                        casting.character.CmdCastSpell(casting.mode, casting.target);
                    }
                }
                
            }
        }
    }
    public (string, int) SeparateSpell(string spell)
{
    var match = Regex.Match(spell, @"(.*\D)(\d+)$");

    string spellName = spell;  // default values, in case the regex doesn't match
    int level = 1;

    if (match.Success) 
    {
        spellName = match.Groups[1].Value.Trim();
        level = int.Parse(match.Groups[2].Value);
    }

    return (spellName, level);
}
public void AutoCastForCharacter(MovingObject castingCharacter, MovingObject target, string mode, Vector2 mousePosition){
    StartCoroutine(AutoCastingRangeFinder(castingCharacter, target, mode, mousePosition));
}
IEnumerator AutoCastingRangeFinder(MovingObject castingCharacter, MovingObject target, string mode, Vector2 mousePosition){
    yield return new WaitForSeconds(.1f);
print($"Auto casting for character with {mode} as the mode for {castingCharacter.gameObject.name}");
    if(castingCharacter.Casting){
        castingCharacter.CmdCancelSpell();
    }
    PlayerCharacter pc = castingCharacter.GetComponent<PlayerCharacter>();
    string _spell = string.Empty;
    
    if(mode == CastingQ){
        _spell = castingCharacter.SpellQ;
    }
    if(mode == CastingE){
        _spell = castingCharacter.SpellE;
    }
    if(mode == CastingR){
        _spell = castingCharacter.SpellR;
    }
    if(mode == CastingF){
        _spell = castingCharacter.SpellF;
    }
    var nameMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"^\D*");
    string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
    int _spellRank = 1;
    // Extract spell rank
    var rankMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"\d+$");
    if (rankMatch.Success) {
        _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
    }
    if(castingCharacter.cur_mp < castingCharacter.GetSpellCost(spell)){
        ImproperCheckText.Invoke($"Not enough magic points to cast {spell}");
        yield break;

    }
    if(target){
       if(!HasLineOfSight(castingCharacter.transform.position, target.transform.position)){
            ImproperCheckText.Invoke($"{spell} location is not in line of sight");
            yield break;
        }
        if(!InSpellRange(castingCharacter, target, mode, mousePosition)){
            print($"{finalRange} is max range of this spell");
            castingCharacter.CmdMoveToCast(target, mode, finalRange, mousePosition);
            yield break;
        } 
    } else {
        if(!HasLineOfSight(castingCharacter.transform.position, mousePosition)){
            ImproperCheckText.Invoke($"{spell} location is not in line of sight");
            yield break;
        }
        if(!InSpellRange(castingCharacter, null, mode, mousePosition)){
            print($"{finalRange} is max range of this spell");
            castingCharacter.CmdMoveToCast(target, mode, finalRange, mousePosition);
            yield break;
        }
    }
    //friendly spell
    float castTime = GetCastTime(spell, pc.Level, _spellRank);
    if(castTime == 0f){
        if(target){
            pc.CmdCastSpell(mode, target);
        } else {
            pc.CmdCastAOESpell(mode, mousePosition);
        }
    } else {
        CmdBuildCastBar(castingCharacter, mode, target, this, mousePosition);
    }
}
void CharacterCastingTargetSpell(MovingObject castingCharacter, MovingObject target, string mode, Vector2 mousePosition){
    if(castingCharacter.Casting){
        castingCharacter.CmdCancelSpell();
    }
    PlayerCharacter pc = castingCharacter.GetComponent<PlayerCharacter>();
    string _spell = string.Empty;
    
    if(mode == CastingQ){
        _spell = castingCharacter.SpellQ;
    }
    if(mode == CastingE){
        _spell = castingCharacter.SpellE;
    }
    if(mode == CastingR){
        _spell = castingCharacter.SpellR;
    }
    if(mode == CastingF){
        _spell = castingCharacter.SpellF;
    }
    var nameMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"^\D*");
    string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
    int _spellRank = 1;
    // Extract spell rank
    var rankMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"\d+$");
    if (rankMatch.Success) {
        _spellRank = int.Parse(rankMatch.Value); // Parse the rank number
    }
    if(castingCharacter.cur_mp < castingCharacter.GetSpellCost(spell)){
        ImproperCheckText.Invoke($"Not enough magic points to cast {spell}");
        return;
    }
    if(target){
       if(!HasLineOfSight(castingCharacter.transform.position, target.transform.position)){
            ImproperCheckText.Invoke($"{spell} location is not in line of sight");
            return;
        }
        if(!InSpellRange(castingCharacter, target, mode, mousePosition)){
            print($"{finalRange} is max range of this spell");
            castingCharacter.CmdMoveToCast(target, mode, finalRange, mousePosition);
            return;
        } 
    } else { 
        if(!HasLineOfSight(castingCharacter.transform.position, mousePosition)){
            ImproperCheckText.Invoke($"{spell} location is not in line of sight");
            return;
        }
        if(!InSpellRange(castingCharacter, null, mode, mousePosition)){
            print($"{finalRange} is max range of this spell");
            castingCharacter.CmdMoveToCast(target, mode, finalRange, mousePosition);
            return;
        }
    }
    //friendly spell
    float castTime = GetCastTime(spell, pc.Level, _spellRank);
    if(castTime == 0f){
        pc.CmdCastSpell(mode, target);
    } else {
        CmdBuildCastBar(castingCharacter, mode, target, this, mousePosition);
    }
}
List<Vector2> reservationWalls;
public bool HasLineOfSight(Vector2 start, Vector2 end)
    {
        int x0 = Mathf.FloorToInt(start.x);
        int y0 = Mathf.FloorToInt(start.y);
        int x1 = Mathf.FloorToInt(end.x);
        int y1 = Mathf.FloorToInt(end.y);
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            Vector2 current = new Vector2(x0 + .5f, y0 + .5f);
            if (reservationWalls.Contains(current)){
                return false;
            }
            if (x0 == x1 && y0 == y1){
                return true;
            }
            int e2 = 2 * err;
            if (e2 > -dy){
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx){
                err += dx;
                y0 += sy;
            }
        }
    }
    [TargetRpc]
    public void TargetWalls(List<Vector2> targetList){
        reservationWalls = targetList;
    }
    public void OwnerClickedCharacterUI(PlayerCharacter pcClicked, bool ClearSelected){
        if(ClearSelected){
            ClearTarget();
        }
        ClickedCharacter(pcClicked);
    }
    void ClickedCharacter(PlayerCharacter SelectedChar){
        print($"Owned character was clicked, {SelectedChar.CharacterName} is ready to be used");
        if(!selectedCharacters.Contains(SelectedChar.gameObject)){
            selectedCharacters.Add(SelectedChar.gameObject);
        }
        SelectedChar.SelectedMO();
    }
    void ClickedMob(Mob _SelectedMob){
        _SelectedMob.SelectedMO();
    }
    void ClickedTargetMob(Mob _TargetMob){
        _TargetMob.TargettedMO();
        //TargetMob = _TargetMob;
    }
   
    
    void DeselectedCharacter(){
        //selectedPlayer = null;
        selectedCharacters.Clear();
        CombatPartyView.instance.TurnOffSelectedWindow();
        if(SpellQ != null){
            Destroy(SpellQ);
        }
        if(SpellE != null){
            Destroy(SpellE);
        }
        if(SpellR != null){
            Destroy(SpellR);
        }
        if(SpellF != null){
            Destroy(SpellF);
        }
        ToggleSpellsOff.Invoke();
    }
    public void ClearSelected(){
        //if(SelectedMob != null){
        //    SelectedMob = null;
        //}
        //if(TargetMob != null){
        //    TargetMob = null;
        //}
        selectedCharacters.Clear();
        CombatPartyView.instance.TurnOffSelectedWindow();
        ToggleSpellsOff.Invoke();
    }
    [TargetRpc]
    public void TargetEndMatchCanvas(){
        ToggleEndMatch.Invoke();
    }
    public void PortalOVM(){
        ToggleEndMatch.Invoke();
    }
    public int GetCharacterResCost(int _level){
        if(_level > 20 || _level < 0){
            return 0;
        }
        int[] resCosts = {
            2000,  3000,  6000, 10000, 13000,
            17000, 20000, 25000, 30000, 35000,
            40000, 50000, 60000, 70000, 80000,
            90000, 100000, 115000, 150000, 200000
        };
        return resCosts[_level - 1];
    }
    public (int, int, float, int) GetCharacterLevelUp(int _level, string CORE){
            int ExpCost = 0;
            int EnergyCost = 0;
            float TimeCost = 0f;
            int GoldCost = 0;
            switch (CORE) {
                case "STANDARD":
                    switch (_level) {
                        case 1:
                            ExpCost = 4000;
                            EnergyCost = 100;
                            TimeCost = 60f; // 1 minute in seconds
                            GoldCost = 500;
                            break;
                        case 2:
                            ExpCost = 8000;
                            EnergyCost = 200;
                            TimeCost = 300f; // 5 minutes in seconds
                            GoldCost = 2000;
                            break;
                        case 3:
                            ExpCost = 12000;
                            EnergyCost = 300;
                            TimeCost = 900f; // 15 minutes in seconds
                            GoldCost = 4000;
                            break;
                        case 4:
                            ExpCost = 20000;
                            EnergyCost = 400;
                            TimeCost = 3600f; // 1 hour in seconds
                            GoldCost = 8000;
                            break;
                        case 5:
                            ExpCost = 30000;
                            EnergyCost = 500;
                            TimeCost = 10800f; // 3 hours in seconds
                            GoldCost = 12000;
                            break;
                        case 6:
                            ExpCost = 55000;
                            EnergyCost = 600;
                            TimeCost = 18000f; // 5 hours in seconds
                            GoldCost = 16000;
                            break;
                        case 7:
                            ExpCost = 75000;
                            EnergyCost = 700;
                            TimeCost = 21600f; // 6 hours in seconds
                            GoldCost = 20000;
                            break;
                        case 8:
                            ExpCost = 100000;
                            EnergyCost = 800;
                            TimeCost = 25200f; // 7 hours in seconds
                            GoldCost = 24000;
                            break;
                        case 9:
                            ExpCost = 145000;
                            EnergyCost = 900;
                            TimeCost = 28800f; // 8 hours in seconds
                            GoldCost = 28000;
                            break;
                        case 10:
                            ExpCost = 180000;
                            EnergyCost = 1000;
                            TimeCost = 32400f; // 9 hours in seconds
                            GoldCost = 32000;
                            break;
                        case 11:
                            ExpCost = 220000;
                            EnergyCost = 1200;
                            TimeCost = 39600f; // 11 hours in seconds
                            GoldCost = 36000;
                            break;
                        case 12:
                            ExpCost = 265000;
                            EnergyCost = 1400;
                            TimeCost = 46800f; // 13 hours in seconds
                            GoldCost = 40000;
                            break;
                        case 13:
                            ExpCost = 300000;
                            EnergyCost = 1600;
                            TimeCost = 54000f; // 15 hours in seconds
                            GoldCost = 45000;
                            break;
                        case 14:
                            ExpCost = 350000;
                            EnergyCost = 1800;
                            TimeCost = 61200f; // 17 hours in seconds
                            GoldCost = 50000;
                            break;
                        case 15:
                            ExpCost = 425000;
                            EnergyCost = 2000;
                            TimeCost = 68400f; // 19 hours in seconds
                            GoldCost = 55000;
                            break;
                        case 16:
                            ExpCost = 500000;
                            EnergyCost = 2200;
                            TimeCost = 75600f; // 21 hours in seconds
                            GoldCost = 60000;
                            break;
                        case 17:
                            ExpCost = 600000;
                            EnergyCost = 2400;
                            TimeCost = 82800f; // 23 hours in seconds
                            GoldCost = 65000;
                            break;
                        case 18:
                            ExpCost = 700000;
                            EnergyCost = 2600;
                            TimeCost = 90000f; // 25 hours in seconds
                            GoldCost = 70000;
                            break;
                        case 19:
                            ExpCost = 800000;
                            EnergyCost = 2800;
                            TimeCost = 97200f; // 27 hours in seconds
                            GoldCost = 75000;
                            break;
                        case 20:
                            ExpCost = 1000000;
                            EnergyCost = 3000;
                            TimeCost = 104400f; // 29 hours in seconds
                            GoldCost = 80000;
                            break;
                        default:
                            break;
                    }
                    break;
                case "HARDCORE":
                    switch (_level) {
                        case 1:
                            ExpCost = 8000;
                            EnergyCost = 200;
                            TimeCost = 3600f;
                            GoldCost = 800;
                            break;
                        case 2:
                            ExpCost = 16000;
                            EnergyCost = 400;
                            TimeCost = 7200f;
                            GoldCost = 2500;
                            break;
                        case 3:
                            ExpCost = 24000;
                            EnergyCost = 600;
                            TimeCost = 14400f;
                            GoldCost = 5000;
                            break;
                        case 4:
                            ExpCost = 40000;
                            EnergyCost = 800;
                            TimeCost = 21600f;
                            GoldCost = 10000;
                            break;
                        case 5:
                            ExpCost = 60000;
                            EnergyCost = 1000;
                            TimeCost = 28800f;
                            GoldCost = 15000;
                            break;
                        case 6:
                            ExpCost = 110000;
                            EnergyCost = 1200;
                            TimeCost = 36000f;
                            GoldCost = 20000;
                            break;
                        case 7:
                            ExpCost = 150000;
                            EnergyCost = 1400;
                            TimeCost = 43200f;
                            GoldCost = 25000;
                            break;
                        case 8:
                            ExpCost = 200000;
                            EnergyCost = 1600;
                            TimeCost = 50400f;
                            GoldCost = 30000;
                            break;
                        case 9:
                            ExpCost = 290000;
                            EnergyCost = 1800;
                            TimeCost = 57600f;
                            GoldCost = 35000;
                            break;
                        case 10:
                            ExpCost = 360000;
                            EnergyCost = 2000;
                            TimeCost = 64800f;
                            GoldCost = 40000;
                            break;
                        case 11:
                            ExpCost = 440000;
                            EnergyCost = 2200;
                            TimeCost = 72000f;
                            GoldCost = 45000;
                            break;
                        case 12:
                            ExpCost = 530000;
                            EnergyCost = 2400;
                            TimeCost = 79200f;
                            GoldCost = 50000;
                            break;
                        case 13:
                            ExpCost = 600000;
                            EnergyCost = 2600;
                            TimeCost = 86400f;
                            GoldCost = 55000;
                            break;
                        case 14:
                            ExpCost = 700000;
                            EnergyCost = 2800;
                            TimeCost = 93600f;
                            GoldCost = 60000;
                            break;
                        case 15:
                            ExpCost = 850000;
                            EnergyCost = 3000;
                            TimeCost = 100800f;
                            GoldCost = 70000;
                            break;
                        case 16:
                            ExpCost = 1000000;
                            EnergyCost = 3200;
                            TimeCost = 108000f;
                            GoldCost = 80000;
                            break;
                        case 17:
                            ExpCost = 1200000;
                            EnergyCost = 3400;
                            TimeCost = 115200f;
                            GoldCost = 90000;
                            break;
                        case 18:
                            ExpCost = 1400000;
                            EnergyCost = 3600;
                            TimeCost = 122400f;
                            GoldCost = 100000;
                            break;
                        case 19:
                            ExpCost = 1600000;
                            EnergyCost = 3800;
                            TimeCost = 129600f;
                            GoldCost = 125000;
                            break;
                        case 20:
                            ExpCost = 2000000;
                            EnergyCost = 4000;
                            TimeCost = 136800f;
                            GoldCost = 150000;
                            break;
                        default:
                            break;
                    }
                    break;
                case "HERO":
                    switch (_level) {
                        case 1:
                            ExpCost = 16000;
                            EnergyCost = 300;
                            TimeCost = 7200f;
                            GoldCost = 1500;
                            break;
                        case 2:
                            ExpCost = 32000;
                            EnergyCost = 600;
                            TimeCost = 18000f;
                            GoldCost = 4000;
                            break;
                        case 3:
                            ExpCost = 48000;
                            EnergyCost = 900;
                            TimeCost = 28800f;
                            GoldCost = 10000;
                            break;
                        case 4:
                            ExpCost = 80000;
                            EnergyCost = 1200;
                            TimeCost = 39600f;
                            GoldCost = 16000;
                            break;
                        case 5:
                            ExpCost = 120000;
                            EnergyCost = 1500;
                            TimeCost = 50400f;
                            GoldCost = 25000;
                            break;
                        case 6:
                            ExpCost = 220000;
                            EnergyCost = 1800;
                            TimeCost = 61200f;
                            GoldCost = 35000;
                            break;
                        case 7:
                            ExpCost = 300000;
                            EnergyCost = 2100;
                            TimeCost = 72000f;
                            GoldCost = 45000;
                            break;
                        case 8:
                            ExpCost = 400000;
                            EnergyCost = 2400;
                            TimeCost = 82800f;
                            GoldCost = 55000;
                            break;
                        case 9:
                            ExpCost = 580000;
                            EnergyCost = 2700;
                            TimeCost = 93600f;
                            GoldCost = 65000;
                            break;
                        case 10:
                            ExpCost = 720000;
                            EnergyCost = 3000;
                            TimeCost = 104400f;
                            GoldCost = 75000;
                            break;
                        case 11:
                            ExpCost = 880000;
                            EnergyCost = 3300;
                            TimeCost = 115200f;
                            GoldCost = 100000;
                            break;
                        case 12:
                            ExpCost = 1060000;
                            EnergyCost = 3600;
                            TimeCost = 126000f;
                            GoldCost = 125000;
                            break;
                        case 13:
                            ExpCost = 1200000;
                            EnergyCost = 3900;
                            TimeCost = 136800f;
                            GoldCost = 150000;
                            break;
                        case 14:
                            ExpCost = 1400000;
                            EnergyCost = 4200;
                            TimeCost = 147600f;
                            GoldCost = 175000;
                            break;
                        case 15:
                            ExpCost = 1800000;
                            EnergyCost = 4500;
                            TimeCost = 158400f;
                            GoldCost = 200000;
                            break;
                        case 16:
                            ExpCost = 2100000;
                            EnergyCost = 4800;
                            TimeCost = 169200f;
                            GoldCost = 225000;
                            break;
                        case 17:
                            ExpCost = 2400000;
                            EnergyCost = 5100;
                            TimeCost = 180000f;
                            GoldCost = 250000;
                            break;
                        case 18:
                            ExpCost = 2800000;
                            EnergyCost = 5400;
                            TimeCost = 190800f;
                            GoldCost = 275000;
                            break;
                        case 19:
                            ExpCost = 3200000;
                            EnergyCost = 5700;
                            TimeCost = 201600f;
                            GoldCost = 350000;
                            break;
                        case 20:
                            ExpCost = 4000000;
                            EnergyCost = 6000;
                            TimeCost = 212400f;
                            GoldCost = 500000;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        return (ExpCost, EnergyCost, TimeCost, GoldCost);
    }
    public (int, int, int, int) GetCharacterStats(string _class, int _level, string CORE){
        int strength = 0;
        int agility = 0;
        int fortitude = 0;
        int arcana = 0;
        if (_class == "Fighter"){
            switch (CORE){
                case "STANDARD": strength = 90 + ((_level - 1) * 3); agility = 70 + ((_level - 1) * 2); fortitude = 90 + ((_level - 1) * 3); arcana = 50 + ((_level - 1) * 2); break;
                case "HARDCORE": strength = 105 + ((_level - 1) * 3); agility = 85 + ((_level - 1) * 2); fortitude = 105 + ((_level - 1) * 3); arcana = 60 + ((_level - 1) * 3); break;
                case "HERO": strength = 130 + ((_level - 1) * 3); agility = 110 + ((_level - 1) * 2); fortitude = 130 + ((_level - 1) * 3); arcana = 75 + ((_level - 1) * 4); break;
            }
        }
        if (_class == "Rogue"){
            switch (CORE){
                case "STANDARD": strength = 70 + ((_level - 1) * 2); agility = 110 + ((_level - 1) * 4); fortitude = 70 + ((_level - 1) * 2); arcana = 60 + ((_level - 1) * 2); break;
                case "HARDCORE": strength = 85 + ((_level - 1) * 2); agility = 125 + ((_level - 1) * 4); fortitude = 85 + ((_level - 1) * 2); arcana = 70 + ((_level - 1) * 3); break;
                case "HERO": strength = 110 + ((_level - 1) * 2); agility = 140 + ((_level - 1) * 4); fortitude = 105 + ((_level - 1) * 2); arcana = 85 + ((_level - 1) * 4); break;
            }
        }
        if (_class == "Priest"){
            switch (CORE){
                case "STANDARD": strength = 80 + ((_level - 1) * 2); agility = 60 + ((_level - 1) * 2); fortitude = 90 + ((_level - 1) * 2); arcana = 90 + ((_level - 1) * 4); break;
                case "HARDCORE": strength = 95 + ((_level - 1) * 2); agility = 75 + ((_level - 1) * 2); fortitude = 105 + ((_level - 1) * 2); arcana = 105 + ((_level - 1) * 5); break;
                case "HERO": strength = 110 + ((_level - 1) * 2); agility = 90 + ((_level - 1) * 2); fortitude = 130 + ((_level - 1) * 2); arcana = 120 + ((_level - 1) * 6); break;
            }
        }
        if (_class == "Archer"){
            switch (CORE){
                case "STANDARD": strength = 50 + ((_level - 1) * 1); agility = 120 + ((_level - 1) * 4); fortitude = 70 + ((_level - 1) * 2); arcana = 50 + ((_level - 1) * 2); break;
                case "HARDCORE": strength = 60 + ((_level - 1) * 1); agility = 130 + ((_level - 1) * 4); fortitude = 80 + ((_level - 1) * 2); arcana = 60 + ((_level - 1) * 3); break;
                case "HERO": strength = 90 + ((_level - 1) * 1); agility = 140 + ((_level - 1) * 4); fortitude = 90 + ((_level - 1) * 2); arcana = 70 + ((_level - 1) * 4); break;
            }
        }
        if (_class == "Wizard"){
            switch (CORE){
                case "STANDARD": strength = 50 + ((_level - 1) * 1); agility = 90 + ((_level - 1) * 2); fortitude = 50 + ((_level - 1) * 1); arcana = 120 + ((_level - 1) * 5); break;
                case "HARDCORE": strength = 60 + ((_level - 1) * 1); agility = 100 + ((_level - 1) * 2); fortitude = 60 + ((_level - 1) * 1); arcana = 130 + ((_level - 1) * 6); break;
                case "HERO": strength = 70 + ((_level - 1) * 1); agility = 110 + ((_level - 1) * 2); fortitude = 75 + ((_level - 1) * 1); arcana = 150 + ((_level - 1) * 7); break;
            }
        }
        if (_class == "Enchanter"){
            switch (CORE){
                case "STANDARD": strength = 50 + ((_level - 1) * 1); agility = 110 + ((_level - 1) * 2); fortitude = 40 + ((_level - 1) * 1); arcana = 140 + ((_level - 1) * 6); break;
                case "HARDCORE": strength = 60 + ((_level - 1) * 1); agility = 120 + ((_level - 1) * 2); fortitude = 50 + ((_level - 1) * 1); arcana = 160 + ((_level - 1) * 7); break;
                case "HERO": strength = 70 + ((_level - 1) * 1); agility = 130 + ((_level - 1) * 2); fortitude = 60 + ((_level - 1) * 1); arcana = 180 + ((_level - 1) * 8); break;
            }
        }
        if (_class == "Druid"){
            switch (CORE){
                case "STANDARD": strength = 65 + ((_level - 1) * 1); agility = 90 + ((_level - 1) * 2); fortitude = 75 + ((_level - 1) * 2); arcana = 100 + ((_level - 1) * 4); break;
                case "HARDCORE": strength = 75 + ((_level - 1) * 1); agility = 100 + ((_level - 1) * 2); fortitude = 90 + ((_level - 1) * 2); arcana = 115 + ((_level - 1) * 5); break;
                case "HERO": strength = 85 + ((_level - 1) * 1); agility = 110 + ((_level - 1) * 2); fortitude = 105 + ((_level - 1) * 2); arcana = 130 + ((_level - 1) * 6); break;
            }
        }
        if (_class == "Paladin"){
            switch (CORE){
                case "STANDARD": strength = 80 + ((_level - 1) * 1); agility = 70 + ((_level - 1) * 2); fortitude = 85 + ((_level - 1) * 2); arcana = 70 + ((_level - 1) * 4); break;
                case "HARDCORE": strength = 95 + ((_level - 1) * 1); agility = 85 + ((_level - 1) * 2); fortitude = 100 + ((_level - 1) * 2); arcana = 85 + ((_level - 1) * 5); break;
                case "HERO": strength = 110 + ((_level - 1) * 1); agility = 110 + ((_level - 1) * 2); fortitude = 125 + ((_level - 1) * 2); arcana = 100 + ((_level - 1) * 6); break;
            }
        }
        return (strength, agility, fortitude, arcana);
    }
    //public bool HasLineOfSight(Vector2 start, Vector2 end, PlayerCharacter pc)
    //{
    //    int x0 = Mathf.FloorToInt(start.x);
    //    int y0 = Mathf.FloorToInt(start.y);
    //    int x1 = Mathf.FloorToInt(end.x);
    //    int y1 = Mathf.FloorToInt(end.y);
    //    int dx = Mathf.Abs(x1 - x0);
    //    int dy = Mathf.Abs(y1 - y0);
    //    int sx = x0 < x1 ? 1 : -1;
    //    int sy = y0 < y1 ? 1 : -1;
    //    int err = dx - dy;
    //    while (true)
    //    {
    //        Vector2 current = new Vector2(x0 + .5f, y0 + .5f);
    //        RaycastHit2D floorCheck = Physics2D.Raycast(current, Vector2.zero, 0f, LayerMask.GetMask("Floor"));
    //        if(floorCheck.collider != null){
    //            RaycastHit2D obstructionCheck = Physics2D.Raycast(current, Vector2.zero, 0f, LayerMask.GetMask("blockingLayer"));
    //            if(obstructionCheck.collider != null){
    //                return false;
    //            }
    //        } else {
    //            return false;
    //        }
    //        if (x0 == x1 && y0 == y1){
    //            return true;
    //        }
    //        int e2 = 2 * err;
    //        if (e2 > -dy){
    //            err -= dy;
    //            x0 += sx;
    //        }
    //        if (e2 < dx){
    //            err += dx;
    //            y0 += sy;
    //        }
    //    }
    //}
}
}

