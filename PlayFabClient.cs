using System.Drawing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Mirror;
using UnityEngine.Events;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using TMPro;
namespace dragon.mirror
{

public class PlayFabClient : NetworkManager
{
    public static PlayFabClient Instance { get; private set; }
    public static UnityEvent<bool> TacticianBegin = new UnityEvent<bool>();
    public static UnityEvent<string, bool> XummMSG = new UnityEvent<string, bool>();
    public static UnityEvent CenterCamera = new UnityEvent();
    public static UnityEvent BlockCLicker = new UnityEvent();
    public static UnityEvent NameDone = new UnityEvent();
    public static UnityEvent CharacterBegin = new UnityEvent();
    public static UnityEvent QRMusic = new UnityEvent();
    public static UnityEvent QRStopMusic = new UnityEvent();
    public static UnityEvent<int> StoreFrontMessage = new UnityEvent<int>();

    public DisconnectedEvent OnDisconnected = new DisconnectedEvent();
    public class DisconnectedEvent : UnityEvent<int?> {}
    [SerializeField] public static UnityEvent OnConnected = new UnityEvent();
    public string userName { get; private set; }
    public string PlayFabId;
    public string SessionTicket;
    public static string tactician;
    public string scene;
    GameObject settings;
    [SerializeField] GameObject LoadScreenGameObject;
    [SerializeField] Canvas LoadScreenCanvas;
    [SerializeField] Slider LoadScreenSlider;
    [SerializeField] GameObject LoadScreenSliderGameObject;
    List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] GameObject XummQRCodePayment;
    [SerializeField] GameObject QRCODEGENERATOR;
    [SerializeField] GameObject ErrorQRCODE;
    [SerializeField] GameObject ConfirmQRCODE;
    [SerializeField] GameObject ExitButton;
    [SerializeField] TextMeshProUGUI qrCodeDisplayText;
    [SerializeField] Canvas XummQRCodePaymentCanvas;
    public static UnityEvent OurNodeSet = new UnityEvent();


    //Coroutine ServerChecker;
    public bool showPing = true;
    public override void Awake()
    {
        LoadNetworkPrefabs();
        base.Awake();
        Instance = this;
    }
    private void LoadNetworkPrefabs()
    {
        // Clear the list to not have duplicates
        // Load all prefabs in the 'Resources/Prefabs/Monsters' directory
        //prefabs = Resources.LoadAll<GameObject>("Prefabs/Monsters");
        prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Monsters"));
        prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Droppables"));

        foreach (GameObject prefab in prefabs)
        {
            if (prefab.GetComponent<NetworkIdentity>() != null)
            {
                spawnPrefabs.Add(prefab);
            }
            else
            {
                Debug.LogWarning($"{prefab.name} does not have a NetworkIdentity and was not added to the list of spawnable prefabs.");
            }
        }
    }
    void SetUserName(string _userName){
        userName = _userName;
    }
    private const string NAME_REGEX = @"^(?!.*([A-Za-z0-9])\1{2})(?=.*[a-z])(?=.{3,13})[A-Za-z0-9]+$";
    void SetNamePermanently(string DesiredName){
        if(!Regex.IsMatch(DesiredName, NAME_REGEX)){
           
        }else{
            
            PlayFabClientAPI.UpdateUserTitleDisplayName( new UpdateUserTitleDisplayNameRequest(){
                DisplayName = DesiredName
            },
            response => {
                userName = DesiredName;
                NameDone.Invoke();
            },
            error => {
                Debug.Log(error.Error);
            }
            );
        }
    }
    void OnEnable(){
        ScenePlayer.MoveRequest.AddListener(MoveRequest);
        ContainerPlayerUI.TacticianReady.AddListener(NoobReady);
        ScenePlayer.OVMRequest.AddListener(OVMRequest);
        ScenePlayer.LoadbarOffToggle.AddListener(TurnOffLoadbar);
        ScenePlayer.LoadbarOnToggle.AddListener(TurnOnLoadbar);
        NetworkClient.RegisterHandler<SendQRCodeUrlMessage>(RegisterPlayer);
        NetworkClient.RegisterHandler<XummMessage>(XummCode);
        NetworkClient.RegisterHandler<XummTransmute>(XUMMTRANSMUTERES);
        NetworkClient.RegisterHandler<XRPLTransmute>(XRPLTRANSMUTERES);
        ContainerPlayerUI.TacticianNameSet.AddListener(SetUserName);
        StoreFront.ReadyToTransmuteXUMMDKP.AddListener(PrepareXumm);
        StoreFront.ReadyToTransmuteXRPLGOLD.AddListener(PrepareXRPL);

        //NetworkClient.RegisterHandler<XUMMQRCODE>(ACTIVATEACCOUNT);
    }
    public void ExitPressedOnQR(){
        StoreFrontMessage.Invoke(10);
        XummQRCodePayment.SetActive(false);
        XummQRCodePaymentCanvas.enabled = false;
        ExitButton.SetActive(false);
    }
    void XRPLTRANSMUTERES(XRPLTransmute msg){
            //QRCODEGENERATOR.SetActive(false);
        int signal = 0;
        if(msg.code == "1"){
            signal = 1;
        }
        StoreFrontMessage.Invoke(signal);
    }
    void XUMMTRANSMUTERES(XummTransmute msg){
        if(msg.qrCodeUrl != null){
            XummQRCodePayment.SetActive(true);
            XummQRCodePaymentCanvas.enabled = true;
            ErrorQRCODE.SetActive(false);
            ConfirmQRCODE.SetActive(false);
            //QRCODEGENERATOR.SetActive(true);
            qrCodeDisplayText.text = "Scan the QR Code to transmute the dkp xls-20 token into in-game gold currency";
            ExitButton.SetActive(true);
            QRCodeGenerator qrCode = QRCODEGENERATOR.GetComponent<QRCodeGenerator>();
            if(qrCode){
                qrCode.StartMe(msg.qrCodeUrl);
            }
        }
        if(msg.code != null){
            int signal = 2;
                //cancelled
                

            if(msg.code == "3"){
                signal = 3;
                //expired
            }
            if(msg.code == "4"){
                signal = 4;
                //validating
            }
            if(msg.code == "5"){
                signal = 5;
                //success!
            }
            if(msg.code == "6"){
                signal = 6;
                //failed!
            }
            if(msg.code == "15"){
                signal = 15;
                //failed!
            }
            if(msg.code == "25"){
                signal = 25;
                //failed!
            }
            XummQRCodePayment.SetActive(false);
            XummQRCodePaymentCanvas.enabled = false;
            StoreFrontMessage.Invoke(signal);
        }
        
    }
    void XummCode(XummMessage msg){
        bool quitting = false;
        if(msg.quit){
            quitting = true;
            OnClientDisconnect();
        }
        if(msg.code  == "Checking status of our market buy for DKP"){
            qrCodeDisplayText.text = "After trading XRP for DKP we can register our account, 50000 dkp is the requirement";
        }
        if(msg.code == "Trust set was sent to XRPL from Xumm"){
            qrCodeDisplayText.text = "Trust set tx was sent to the XRP Ledger awaiting a validator node response to confirm";
        }
        if(msg.code == "The trust set QR code was cancelled, trying again in a few moments."){
            qrCodeDisplayText.text = "Generating a new QR code, trying again in a few moments";
        }
        if(msg.code == "The trust set QR code has expired, trying again in a few seconds."){
            qrCodeDisplayText.text = "Generating a new QR code, trying again in a few moments";
        }
        if(msg.code == "No trust line detected, sending a trust set QR code in a few moments"){
            qrCodeDisplayText.text = "Processing a new QR code for setting a trustline with DKP, 2 XRP reserve is required for this, the XRP will not be consumed";
        }
        if(msg.code == "The registration QR code has expired, trying again in a few seconds."){
            qrCodeDisplayText.text = "Your registration code was expired, checking trust line, preparing another QR code";
        }
        if(msg.code == "The registration QR code was cancelled, trying again in a few seconds."){
            qrCodeDisplayText.text = "Your registration code was cancelled, checking trust line, preparing another QR code";
        }
        if(msg.code == "Registration request sent from XUMM to XRPL awaiting a validator node reponse"){
            qrCodeDisplayText.text = "Pending response from a validator node on the XRP Ledger, one moment please";
        }
        if(msg.code == "Trading XRP for DKP via the XRP Ledger's decentralized exchange one moment please"){
            qrCodeDisplayText.text = "Your trade will be completed shortly";
        }
        if(msg.code == "The wallet you have tried to sign with has already been registered to another DragonKill account, please try another."){
            qrCodeDisplayText.text = "Try another wallet, it seems that one has already been used before";
        }
        if(msg.code == "Error on payload, we will generate you a new QR code in just a few moments."){
            qrCodeDisplayText.text = "Generating another QR code";
        }
        if(msg.code == "Registration tx successfully validated on the XRP Ledger. Welcome to DragonKill!"){
            qrCodeDisplayText.text = "Welcome to DragonKill, congraduations on registering.";
        }
        if(msg.code == "Checking status one moment please"){
            qrCodeDisplayText.text = "Scan the QR Code to continue linking your Xumm wallet with your DragonKill game account. If you have zero DKP and have a DKP trust line, you can remove your trustline for DKP via the Xummm app, once removed you can scan the QR code, and when prompted accept the payload even though you have no trustline. This allows our system to process your trustline request for a seamless experience. If you have some DKP but under the 50,000 DKP registration fee you can also purchase more on the dex via Xumm app.";
        }
        if(msg.code == "Checking status trust set status one moment please"){
            qrCodeDisplayText.text = "We are in the trust set part of the registration";
        }
        if(msg.code == "Trust line set for DKP but not enough liquidity, fetching the market order to accomplish registration please double check"){
            qrCodeDisplayText.text = "Please double check the QR code to verify it is an acceptable trade for XRP to DKP, this is based on the current market";
        }
        if(msg.code == "The registration QR code was cancelled. If you have zero DKP and a trust line for DKP you can remove your trustline for DKP on your xumm app and then rescan the next QR code and follow the process to register seamlessly, otherwise you can purchase more DKP on the xumm app via the decentralized exchange on the XRP ledger"){
            qrCodeDisplayText.text = "If you have zero DKP and just a DKP trust line, you can remove your trustline for DKP via the Xummm app for a seamless experience, if you have some DKP but under the 50,000 DKP registration fee you can also purchase more on the dex via Xumm app.";
        }
        
        
        if(!quitting && !msg.pending){
            //QRCODEGENERATOR.SetActive(false);
            QRCodeGenerator qrCode = QRCODEGENERATOR.GetComponent<QRCodeGenerator>();
            if(qrCode){
                qrCode.ToggleCanvas(false);
            }
            if(msg.error){
                ErrorQRCODE.SetActive(true);
                
                ConfirmQRCODE.SetActive(false);
            } else {
                ErrorQRCODE.SetActive(false);
                ConfirmQRCODE.SetActive(true);
            }
            XummMSG.Invoke(msg.code, msg.error);
        }
        if(msg.pending){
            XummMSG.Invoke(msg.code, msg.error);
        }
    }
    
    void NoobReady(BuildingTacticianHelper tactBuild){
        QRStopMusic.Invoke();
        LoadScreenGameObject.SetActive(true);
        LoadScreenCanvas.enabled = true;
        LoadScreenSliderGameObject.SetActive(true);
        NetworkClient.connection.Send<NoobToPlayer>(new NoobToPlayer
        {
            Sprite = tactBuild.Sprite,
            bonusStatStrength = tactBuild.bonusStatStrength,
            bonusStatAgility = tactBuild.bonusStatAgility,
            bonusStatFortitude = tactBuild.bonusStatFortitude,
            bonusStatArcana = tactBuild.bonusStatArcana,
            BirthDate = tactBuild.BirthDate,
            BodyStyle = tactBuild.BodyStyle,
            EyeColor = tactBuild.EyeColor,
            finished = true
        });
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) {
        if (customHandling)
        {
            StartCoroutine(LoadScreenEngaged(newSceneName));
        }
    }
    IEnumerator LoadScreenEngaged(string newSceneName){
            BoxCollider2D boxCollider = ScenePlayer.localPlayer.GetComponent<BoxCollider2D>();

        if(newSceneName != "OVM" && newSceneName != "TOWNOFARUDINE"){
            boxCollider.enabled = false;
        } else {
            boxCollider.enabled = true;
        }
        BlockCLicker.Invoke();
        //print($" {newSceneName} was scene name we are supposed to be at");
        LoadScreenGameObject.SetActive(true);
        LoadScreenCanvas.enabled = true;
        LoadScreenSliderGameObject.SetActive(true);
        loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        loadingSceneAsync.allowSceneActivation = false;
        float lerpValue = 0.0f; 
        float startTime = Time.time;
        float duration = 4f;
        while(lerpValue < 1f){
            lerpValue = Mathf.Lerp(0f,1f, (Time.time - startTime) / duration);
            LoadScreenSlider.value = lerpValue;
            yield return null;
        }
        loadingSceneAsync.allowSceneActivation = true;
        

    }
    void TurnOnLoadbar(){
        LoadScreenGameObject.SetActive(true);
        LoadScreenCanvas.enabled = true;
        LoadScreenSliderGameObject.SetActive(true);
    }
    void TurnOffLoadbar(){
        
        StartCoroutine(WaitForLoading());
    }
    IEnumerator WaitForLoading()
    {
        while(!SceneManager.GetSceneByName(ScenePlayer.localPlayer.currentScene).isLoaded)
        {
           // Do something here
            yield return null;
        }
        TurnOffLoadbarEXT();
    }
    void TurnOffLoadbarEXT(){
        CenterCamera.Invoke();
        OurNodeSet.Invoke();
        LoadScreenCanvas.enabled = false;
        LoadScreenGameObject.SetActive(false);
        LoadScreenSliderGameObject.SetActive(false);
    }
    void ResetAfterFailedConnect(){
        LoadScreenCanvas.enabled = false;
        LoadScreenGameObject.SetActive(false);
        LoadScreenSliderGameObject.SetActive(false);
        UISignIn.instance.ResetLogin();
        //unfreeze the button
    }
    
    public string GetCurrentScene(){
            int countLoaded = SceneManager.sceneCount;
            print($"*******{countLoaded} is how many sceens are loaded on the server*******");
            Scene[] loadedScenes = new Scene[countLoaded];
            for (int i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
                string sceneName = loadedScenes[i].name;
                if(sceneName == "Container")
                {
                    //change scene name
                    scene = sceneName;
                    break;
                }
                if(sceneName == "TOWNOFARUDINE")
                {
                    //change scene name
                    scene = sceneName;
                    break;
                }
                if(sceneName == "OVM")
                {
                    //change scene name
                    scene = sceneName;
                } 

            }
            return scene;
         
        }

    void MoveRequest(string newScene, string oldScene){
        LoadScreenGameObject.SetActive(true);
        LoadScreenCanvas.enabled = true;
        LoadScreenSliderGameObject.SetActive(true);
        string unloadScene = string.Empty;
        if (oldScene == null)
        {
            unloadScene = "Container";
        }else {
            unloadScene = oldScene;
        }
        NetworkClient.connection.Send<ClientRequestLoadScene>(new ClientRequestLoadScene {
            newScene = newScene,
            oldScene = unloadScene,
            login = false
        });
    }
    void OVMRequest(string currentScene)
    {
        LoadScreenGameObject.SetActive(true);
        LoadScreenCanvas.enabled = true;
        LoadScreenSliderGameObject.SetActive(true);
        //NetworkClient.connection.Send<ClientRequestLoadScene>(new ClientRequestLoadScene {
        //    newScene = "OVM",
        //    oldScene = "TOWNOFARUDINE",
        //    login = false
        //});
    }
    public override void Start()
    {
        Startup.Reset.AddListener(ResetAfterFailedConnect);
        settings = GameObject.Find("SETTINGS");
    }
    private void RequestMultiplayerServer()
    {
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
        
        requestData.BuildId = settings.GetComponent<Settings>().buildId;
        requestData.PreferredRegions = new List<string>() { "EastUs" };
        requestData.SessionId = settings.GetComponent<Settings>().sessionId;
        if (requestData.SessionId.Equals(""))
        {
            requestData.SessionId = System.Guid.NewGuid().ToString();
        }
        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }
    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        NetworkClient.RegisterHandler<Noob>(NoobPlayer, false);
        NetworkClient.RegisterHandler<PlayerInfo>(OnReceivePlayerInfo, false);
        print($"response address was {response.IPV4Address}");
        this.networkAddress = response.IPV4Address;
        this.GetComponent<TelepathyTransport>().port = (ushort)response.Ports[0].Num;
        this.StartClient();
    }

    private void OnRequestMultiplayerServerError(PlayFabError error)
    {
        ResetAfterFailedConnect();

        Debug.Log(error.ErrorMessage);
    }
    public void ChoCho(string sessionId, bool newAccount){
        LoadScreenGameObject.SetActive(true);
        LoadScreenCanvas.enabled = true;
        if(!newAccount)
        {
            LoadScreenSliderGameObject.SetActive(true);
        }
        this.SessionTicket = sessionId;
    }
    public void ClientStarter(bool newPlayer){
        if (!settings.GetComponent<Settings>().isLocalServer)
        {
            RequestMultiplayerServer();
        }
        else
        {
            NetworkClient.RegisterHandler<Noob>(NoobPlayer);
            NetworkClient.RegisterHandler<PlayerInfo>(OnReceivePlayerInfo);
            this.StartClient();
        }
        if(newPlayer){
            LoadScreenGameObject.SetActive(false);
        }
    }
    public string checkAddress;
    void NoobPlayer(Noob netMsg){
        checkAddress = netMsg.Address;
        LoadScreenCanvas.enabled = false;
        LoadScreenGameObject.SetActive(false);
        XummQRCodePayment.SetActive(false);
        XummQRCodePaymentCanvas.enabled = false;
        //LoadScreenSliderGameObject.SetActive(false);
        QRMusic.Invoke();
        if(netMsg.finished == false){
            bool NameNotSet = true;
            if(netMsg.tactician == "NOTACTICIANNAMEPHASE"){
                NameNotSet = false;
            } else {
                userName = netMsg.tactician;
            }
            TacticianBegin.Invoke(NameNotSet);
        } 
    }
    void RegisterPlayer(SendQRCodeUrlMessage netMsg){
        LoadScreenCanvas.enabled = false;
        LoadScreenGameObject.SetActive(false);
        XummQRCodePayment.SetActive(true);
        XummQRCodePaymentCanvas.enabled = true;
        ErrorQRCODE.SetActive(false);
        ConfirmQRCODE.SetActive(false);
        //QRCODEGENERATOR.SetActive(true);
        QRCodeGenerator qrCode = QRCODEGENERATOR.GetComponent<QRCodeGenerator>();
        if(qrCode){
            qrCode.StartMe(netMsg.qrCodeUrl);
        }
        QRMusic.Invoke();
        //LoadScreenSliderGameObject.SetActive(false);
    }
    void PrepareXumm(string amount){
        XummQRCodePayment.SetActive(true);
        XummQRCodePaymentCanvas.enabled = true;
        ErrorQRCODE.SetActive(false);
        ConfirmQRCODE.SetActive(false);
        qrCodeDisplayText.text = $"Preparing your QR code to transmute {amount} dkp into gold";
        NetworkClient.connection.Send<XummTransmute>(new XummTransmute
        {
            amount = amount
        });
        
    }
    void PrepareXRPL(string amount){
        NetworkClient.connection.Send<XRPLTransmute>(new XRPLTransmute
        {
            amount = amount
        });
        
    }
    public void CancelQRCode(){
        XummQRCodePayment.SetActive(false);
        XummQRCodePaymentCanvas.enabled = false;
        ErrorQRCODE.SetActive(false);
        ConfirmQRCODE.SetActive(false);
    }
    public void LoadScreenOpen(){
        LoadScreenGameObject.SetActive(true);
        LoadScreenCanvas.enabled = true;
        LoadScreenSliderGameObject.SetActive(true);
    }
    void CharacterBuild(){
        CharacterBegin.Invoke();
    }
   
    public void OnReceivePlayerInfo(PlayerInfo netMsg)
    {
        Debug.Log("client connected");
        OnConnected.Invoke();
        print($"Received PlayerInfo, sending playfabid: {this.PlayFabId} back to Server");
        NetworkClient.connection.Send<PlayerInfo>(new PlayerInfo
        {
            ConnectionId = netMsg.ConnectionId,
            SessionTicket = this.SessionTicket
        });

    }
    private void OnLoginFailure(PlayFabError obj)
    {
        Debug.Log("Login with CustomID failed.");
    }
    public override void OnStopClient(){
        if (mode == NetworkManagerMode.ClientOnly)
                StartCoroutine(ClientUnloadSubScenes());

    }
    IEnumerator ClientUnloadSubScenes()
        {
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                if (SceneManager.GetSceneAt(index) != SceneManager.GetActiveScene())
                    yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
            }
        }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ResetAfterFailedConnect();
        Debug.Log("client disconnected");
        OnDisconnected.Invoke(null);
    }
    public void Disagreed(){
          #if UNITY_EDITOR
         // Application.Quit() does not work in the editor so
         // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
         UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
        
    }
}
}

