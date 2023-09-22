using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PlayFab;
#if UNITY_SERVER || UNITY_EDITOR
using PlayFab.MultiplayerAgent.Model;
#endif
using UnityEngine.Events;
#if UNITY_SERVER || UNITY_EDITOR
using PlayFab.ServerModels;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine.AI;

namespace dragon.mirror
{
    public class TacticianExtractor
{
    public string BirthDate { get; set; }
    public string ZodiacSign { get; set; } // New field
    public string EyeColor { get; set; }
    public string BodyStyle { get; set; }
    public string BonusStatStrength { get; set; }
    public string BonusStatAgility { get; set; }
    public string BonusStatFortitude { get; set; }
    public string BonusStatArcana { get; set; }
    public string BonusStatArmor { get; set; }
    public string GiantRep { get; set; } // New field
    public string DragonRep { get; set; } // New field
    public string LizardRep { get; set; } // New field
    public string OrcRep { get; set; } // New field
    public string FaerieRep { get; set; } // New field
    public string ElvesRep { get; set; } // New field
    public string DwarvesRep { get; set; } // New field
    public string GnomesRep { get; set; } // New field
    public string DKPCooldown { get; set; } // New field

}

[System.Serializable]
public class Details
{
    public string userId;
    public string goldAmount;
    public string walletAddress;
}

[System.Serializable]
public class ResponseObj
{
    public Details details;
    public string success;
    public string message;
}

[System.Serializable]
public class XrplAccountInfoResponse
{
    public Result result { get; set; }

    public class Result
    {
        public string status { get; set; }
        public AccountData account_data { get; set; }
    }

    public class AccountData
    {
        public long Sequence { get; set; }

        // ... other fields
    }
}

    [System.Serializable]

    public class Amount
{
    public string currency { get; set; }
    public string issuer { get; set; }
    public string value { get; set; }
}
[System.Serializable]
public class TxJson
{
    public string TransactionType { get; set; }
    public string Destination { get; set; }
    public Amount Amount { get; set; }
    //public LimitAmount LimitAmount { get; set; }
    //public TakerPays TakerPays { get; set; }
    //public TakerGets TakerGets { get; set; }
}
[System.Serializable]
public class TrustSet
{
    public string TransactionType { get; set; }
    public string Account  { get; set; }
    public Amount Amount { get; set; }
    public LimitAmount LimitAmount { get; set; }
}
[System.Serializable]
public class MarketOrderXRPL
{
    public string TransactionType { get; set; }
    public TakerPays TakerPays { get; set; }
    public TakerGets TakerGets { get; set; }
}
public class TakerPays{
    public Amount Amount { get; set; }

}
public class TakerGets{
    public Amount Amount { get; set; }

}
[System.Serializable]
public class TransactionPayload
{
    public TxJson txjson { get; set; }
    public CustomMeta custom_meta { get; set; }
    public Options options { get; set; }
}
[System.Serializable]
public class TransactionPayloadTrust
{
    public TrustSet txjson { get; set; }
    public CustomMeta custom_meta { get; set; }
    public Options options { get; set; }
}
[System.Serializable]
public class TransactionPayloadMarketOrder
{
    public MarketOrderXRPL txjson { get; set; }
    public CustomMeta custom_meta { get; set; }
    public Options options { get; set; }
}
public class Tx
{
    public string Account { get; set; }
    public string Destination { get; set; }
    // ... Other fields
}
public class Transaction
{
    public Tx tx { get; set; }
    public bool validated { get; set; }
}
public class LimitAmount
{
    public string currency  { get; set; }
    public string issuer { get; set; }
    public string value { get; set; }


}

public class TransactionsResponse
{
    public Result result { get; set; }
}
public class Result
{
    public List<Transaction> transactions { get; set; }
}
[System.Serializable]
public class CustomMeta
{
    public string identifier { get; set; }
    public string blob { get; set; }
    public string instruction { get; set; }

}
[System.Serializable]
public class Options
{
    public bool submit { get; set; }
    public bool multisign { get; set; }
    public int expire { get; set; }
    public string force_network { get; set; }

}
[System.Serializable]

public class XummStatusResponse
{
    public string status { get; set; }
}
[System.Serializable]
    public class Meta
    {
        public Amount delivered_amount { get; set; }

        public string TransactionResult { get; set; }
        public int TransactionIndex { get; set; }
    }
//[System.Serializable]
//    public class TakerPays
//    {
//        public string currency { get; set; }
//        public string issuer { get; set; }
//        public string value { get; set; }
//    }
    public class Request {
            public string command { get; set; }
            public string id { get; set; }
            public string tx_blob { get; set; }
        }
    [System.Serializable]
public class XrplErrorResponse
{
    public Result result { get; set; }

    public class Result
    {
        public string error { get; set; }
        public string error_exception { get; set; }
        public string id { get; set; }
        public Request request { get; set; }
        public string status { get; set; }
        public string type { get; set; }

        // Add more fields as needed
    }
}
[System.Serializable]
public class XrplTransactionResponse
{
    public Result result { get; set; }

    public class Result
    {

        public string status { get; set; }
        public string Account { get; set; }
        public string Fee { get; set; }
        public long LastLedgerSequence { get; set; }
        public long OfferSequence { get; set; }
        public long Sequence { get; set; }
        public string SigningPubKey { get; set; }
        public string TakerGets { get; set; }
        public TakerPays TakerPays { get; set; }
        public string TransactionType { get; set; }
        public string TxnSignature { get; set; }
        public long date { get; set; }
        public string hash { get; set; }
        public long inLedger { get; set; }
        public long ledger_index { get; set; }
        public Meta meta { get; set; }
        public bool validated { get; set; }
        public string error { get; set; }
        public int? error_code { get; set; }
        public string error_message { get; set; }
        public LimitAmount LimitAmount { get; set; }

    }
}
[System.Serializable]
public class XummResponse
{
    public string uuid { get; set; }
    public string transactionId { get; set; } // Replace with actual property name if different
    public Refs refs { get; set; }
    public Next next { get; set; }
    public bool pushed { get; set; }

    public class Next
    {
        public string always { get; set; }
    }

    public class Refs
    {
        
        public string qr_png { get; set; }
        public string qr_matrix { get; set; }
        public string[] qr_uri_quality_opts { get; set; }
        public string websocket_status { get; set; }
    }
}
[System.Serializable]
public class XummDetailedResponse
{
    public Meta meta { get; set; }
    public CustomMeta custom_meta { get; set; }
    public Response response { get; set; }

    [System.Serializable]
    public class Meta
    {
        public string bestMarketPrice;
        public bool TrustLineNotSet;
        public bool NoAddressSendBackToMain;
        public bool NotEnoughLiquidityButHasTrustLine;
        
        public string RequiredAmount;
        public bool wrongSigner;
        public bool exists;
        public string uuid;
        public bool signed; // assuming this field exists in your actual API response
        public bool submit; // assuming this field exists in your actual API response
        public bool resolved; // assuming this field exists in your actual API response
        public bool expired; // assuming this field exists in your actual API response
    }
    [System.Serializable]
    public class Response
    {
        public string hex;
        public string txid;
        public string account; // Add this to capture the sender's XRP public address
    }
}
[System.Serializable]
    public class CurrencyBalance
    {
        public string currency;
        public string value;
    }

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        // Add a wrapper to handle an array at the top level
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
    public class PlayFabServer : NetworkManager
    {
        [System.Serializable]
        public class NFTMetadata
        {
            public string name;
            public string description;
            public string external_url;
            public string category;
            public string md5hash;
            public bool is_explicit;
            public string content_type;
            public string image_url;
            public string animation_url;
        }
        [System.Serializable]
        public class NFTData
        {
            public int Flags;
            public string Issuer;
            public string NFTokenID;
            public int NFTokenTaxon;
            public int TransferFee;
            public string URI;
            public int nft_serial;
        }
        public static PlayFabServer instance;
        #if UNITY_SERVER || UNITY_EDITOR
        private string _GOLD_WALLET_ADDRESS = ""; //"rUMrzjvDEeH2zQ49TMEn62r69tMa8MWi65";
        private string _GOLD_WALLET_SIGN = ""; //"rUMrzjvDEeH2zQ49TMEn62r69tMa8MWi65";
        private string _Heroku_URL = ""; //"rUMrzjvDEeH2zQ49TMEn62r69tMa8MWi65";

        private string _REGISTRATION_WALLET_ADDRESS = "rhB7i1DDJAmw1A3sVi6nR89GWcUkNPo6KJ"; //"rUMrzjvDEeH2zQ49TMEn62r69tMa8MWi65";
        private string _GAME_WALLET_ADDRESS = ""; //"rUMrzjvDEeH2zQ49TMEn62r69tMa8MWi65";
        private string _DKP_DESTINATION_ADDRESS = ""; //"rhB7i1DDJAmw1A3sVi6nR89GWcUkNPo6KJ";
        private string _DKP_ISSUER = "rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J";
        private string _DKP_NFT_ISSUER = "rhB7i1DDJAmw1A3sVi6nR89GWcUkNPo6KJ";
        private string _DKP_XAPP_FIRST =  ""; //"38b9f3fa-d4d1-4666-bf10-bd9931c1031d";
        private string _DKP_XAPP_SECOND = ""; // "660fe58b-83d3-46fc-a3fa-9e8a86e5ef13";
        private string xrpscanAPI = "https://api.xrpscan.com/api/v1/account/";
        private static string sharedSecret = "";
        private static string salt = "";

        #endif
        private List<PlayerConnection> playerConnections = new List<PlayerConnection>();
        #if UNITY_SERVER || UNITY_EDITOR
        private List<ConnectedPlayer> connectedPlayers = new List<ConnectedPlayer>();
        #endif
        public static UnityEvent<Match> ENDMATCHFULLY = new UnityEvent<Match>();
        public static UnityEvent<Match, float , float> SendEXPCP = new UnityEvent<Match, float , float>();

        public static UnityEvent<Match> ENDMATCHMAKER = new UnityEvent<Match>();
        public static UnityEvent<Match, TurnManager, float> SendingGoldAmount = new UnityEvent<Match, TurnManager, float>();
        public static UnityEvent<Match, TurnManager, string> SendingItem = new UnityEvent<Match, TurnManager, string>();
        public static UnityEvent<ScenePlayer> playerToBeAdded = new UnityEvent<ScenePlayer>();
        //[SerializeField] public static UnityEvent<MovingObject>  Energize = new UnityEvent<MovingObject>();
        [SerializeField] GameObject turnManagerPrefab;
        private const string TOWNOFARUDINE = "TOWNOFARUDINE";
        private const string TACTICIAN = "Tactician";
        //Prefabs
        [SerializeField] GameObject Player18PrefabModel;
        [SerializeField] GameObject Player37PrefabModel;
        [SerializeField] GameObject Player7PrefabModel;
        [SerializeField] GameObject Player23PrefabModel;
        [SerializeField] GameObject Player17PrefabModel;
        [SerializeField] GameObject Player36PrefabModel;
        [SerializeField] GameObject Player33PrefabModel;
        [SerializeField] GameObject Player43PrefabModel;
        [SerializeField] GameObject Player26PrefabModel;
        [SerializeField] GameObject Player45PrefabModel;
        [SerializeField] GameObject Player6PrefabModel;
        [SerializeField] GameObject Player22PrefabModel;
        [SerializeField] GameObject Player35PrefabModel;

        // Mobs
        List<GameObject> prefabs = new List<GameObject>();
        Dictionary<string, List<string>> prefabGroups = new Dictionary<string, List<string>>
{
    // caves
    { "RandomCaveTier1", new List<string> { "Bat", "Rat", "Snake", "Ghoul", "Spider", "Crocodile" } },
    { "RandomCaveTier2", new List<string> { "DragonWhelp", "SkeletonKnight", "GiantRat", "FireGhoul", "Mummy", "Ghost", "DiseasedGhoul", "PoisonGhoul", "Dog", "SpiderMature" } },
    { "RandomCaveTier3", new List<string> { "Cyclops", "Ghast", "FireElemental", "GuardDog", "GhoulNoble", "GhoulWarlock", "Spectre", "GhoulTreasurer", "GreaterMummy", "GiantSpider" } },
    { "RandomCaveTier4", new List<string> { "MummyLord", "ZombieKing", "GreaterSpectre", "DemonElite", "GreaterGhast" } },
    { "RandomCaveTier5", new List<string> { "BlackDragon", "Vampire", "Draco" } },

    // forests
    { "RandomForestTier1", new List<string> { "Bat", "Rat", "Snake", "Spider", "Crocodile" } },
    { "RandomForestTier2", new List<string> { "DragonWhelp", "GiantRat", "Dog", "SpiderMature" } },
    { "RandomForestTier3", new List<string> { "Cyclops", "FireElemental", "GuardDog", "GiantSpider", "LizardGladiator", "LizardWarlock" } },
    { "RandomForestTier4", new List<string> { "Sphinx" } },
    { "RandomForestTier5", new List<string> { "BlackDragon", "Draco" } },

    // swamps
    { "RandomSwampTier1", new List<string> { "Bat", "Rat", "Snake", "Ghoul", "Spider", "Crocodile" } },
    { "RandomSwampTier2", new List<string> { "DragonWhelp", "SkeletonKnight", "GiantRat", "FireGhoul", "Mummy", "Ghost", "DiseasedGhoul", "PoisonGhoul", "Dog", "SpiderMature" } },
    { "RandomSwampTier3", new List<string> { "Cyclops", "Ghast", "FireElemental", "GuardDog", "GhoulWarlock", "GhoulNoble", "Spectre", "GhoulTreasurer", "GreaterMummy", "GiantSpider" } },
    { "RandomSwampTier4", new List<string> { "MummyLord", "ZombieKing", "GreaterSpectre", "DemonElite", "GreaterGhast", "SeaSerpent" } },
    { "RandomSwampTier5", new List<string> { "BlackDragon", "Vampire", "Draco" } },

    // deserts
    { "RandomDesertTier1", new List<string> { "Bat", "Rat", "Snake" } },
    { "RandomDesertTier2", new List<string> { "DragonWhelp", "GiantRat", "Dog", "SpiderMature" } },
    { "RandomDesertTier3", new List<string> { "Cyclops", "FireElemental", "GuardDog", "GiantSpider", "LizardGladiator", "LizardWarlock", "Sphinx" } },
    { "RandomDesertTier4", new List<string> { "MummyLord", "ZombieKing", "GreaterSpectre", "DemonElite", "Sphinx" } },
    { "RandomDesertTier5", new List<string> { "BlackDragon", "Draco" } },

    // water
    { "RandomWaterTier1", new List<string> { "Jellyfish", "LizardChild", "Lobster" } },
    { "RandomWaterTier2", new List<string> { "LizardArcher", "LizardKnight", "LizardMage", "LizardPriest", "SpiderMature" } },
    { "RandomWaterTier3", new List<string> { "GuardDog", "SeaSerpent", "LizardGladiator", "LizardWarlock" } },
    { "RandomWaterTier4", new List<string> { "SeaSerpent", "Sphinx", "Octopus" } },
    { "RandomWaterTier5", new List<string> { "Whale" } },

    // tundra
    { "RandomTundraTier1", new List<string> { "Bat", "Rat", "Snake", "Spider" } },
    { "RandomTundraTier2", new List<string> { "DragonWhelp", "SkeletonKnight", "GiantRat", "Dog", "SpiderMature" } },
    { "RandomTundraTier3", new List<string> { "Cyclops", "FireElemental", "GuardDog", "GiantSpider" } },
    { "RandomTundraTier4", new List<string> { "MummyLord", "ZombieKing", "GreaterSpectre", "DemonElite", "GreaterGhast", "Sphinx" } },
    { "RandomTundraTier5", new List<string> { "BlackDragon", "Vampire", "Draco" } }
};
        #if UNITY_SERVER || UNITY_EDITOR

    public async Task<bool> VerifyPlayerPayment(string playerWalletAddress)
    {
        // Step 1: Fetch registration wallet transaction history
        var (success, transactionsJson) = await GetTransactionHistory();
        if (!success)
        {
            Debug.LogError("Failed to retrieve transaction history");
            return false;
        }
        // Step 2: Parse the transactions from JSON into a list
        var transactionsList = ParseTransactionHistory(transactionsJson);
        // Step 3: Loop through the transactions to look for the player's wallet as sender
        foreach (var transaction in transactionsList)
        {
            if(transaction.tx.Account != null){
                if (transaction.tx.Account == playerWalletAddress)
                {
                    print($"Found players wallet they are trying to double sign a wallet no go");
                    // Player's wallet found as sender in one of the transactions
                    return true;
                }
            }
        }
        // Player's wallet was not found as a sender in any transactions
        return false;
    }

    private async Task<(bool, string)> GetTransactionHistory()
    {
        using (var httpClient = new HttpClient())
        {
            var postData = new
            {
                method = "account_tx",
                @params = new object[]
                {
                    new
                    {
                        account = _REGISTRATION_WALLET_ADDRESS,
                        ledger_index_max = -1
                    }
                }
            };
            var jsonString = JsonConvert.SerializeObject(postData);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://s1.ripple.com:51234/", httpContent); // Adjust the endpoint URL accordingly
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return (true, responseContent); // Return the JSON response which contains transaction history
            }
            else
            {
                Debug.LogError("Error fetching transaction history from XRPL: " + response.StatusCode);
                return (false, "Failed to retrieve transaction history");
            }
        }
    }

    private List<Transaction> ParseTransactionHistory(string transactionsJson)
    {
        var parsedJson = JsonConvert.DeserializeObject<TransactionsResponse>(transactionsJson);
        return parsedJson.result.transactions;
    }
    public static string EncryptString(string plainText)
{
    byte[] encryptedByteArray;  // Declare this variable at a scope where you can access it later

    using (Aes aesAlg = Aes.Create())
    {
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, Encoding.ASCII.GetBytes(salt));
        aesAlg.Key = key.GetBytes(16);  // AES key size is 16 bytes
        aesAlg.IV = key.GetBytes(16);   // AES IV size is 16 bytes

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msEncrypt = new MemoryStream())
        {
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
            }
            encryptedByteArray = msEncrypt.ToArray();  // Populate the byte array here
        }
    }

    return Convert.ToBase64String(encryptedByteArray); // Return or use the byte array as needed
}

    public static string DecryptString(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, Encoding.ASCII.GetBytes(salt));

            aesAlg.Key = key.GetBytes(16);
            aesAlg.IV = key.GetBytes(16);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
        async void RegisterTrustSet(NetworkConnectionToClient conn, string playfabID, int tries, string address){
            Debug.LogError($"calling registertrust set the address is {address} playfab is {playfabID} ");
            print($"RegisterTrustSet!!!!!!!!! *********************************!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            PlayerInfo playerinfo = (PlayerInfo)conn.authenticationData;

            print($"Tries: {tries} are how many times we have tried to register");
            if(conn == null){
                print("No connection found when trying to register account");
                return;
            }
            //if(tries > 2){
            //    var msg = new XummMessage { code = "Too many attempts to register a wallet to this account, please try again later, for now we need to rest", error = true, quit = true };
            //    conn.Send(msg);
            //    return;
            //}
            tries++;
            if(string.IsNullOrEmpty(playfabID)){
                playfabID = playerinfo.PlayFabId;
            }
            string id = EncryptString(playfabID);
            string xummIdentifier = Guid.NewGuid().ToString();
            // Prepare your payload object here.
            if(address == null || address == "Undefined"){
                var _msg = new XummMessage { code = "The registration QR code has expired, trying again in a few seconds." , error = true };
                conn.Send(_msg);
                await Task.Delay(10000);
                print("we cannot go any further because we will bug out, we need to go back to get an account number");
                RegisterAccount(conn, playfabID, tries --, address);
                return;
            }
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"{_Heroku_URL}/xummqueue?id={id}", null);  // No payload
                if (response.IsSuccessStatusCode)
                {
                    // Successfully joined the queue
                    print("queue is ready lets go!");
                }
                else
                {
                    // Log the failure or take other actions
                    Debug.LogError("Failed to join the queue, retrying...");
                    
                    RegisterTrustSet(conn, playfabID, tries --, address);
                    return;
                    // Wait before the next retry
                }
            }
            string jsonPayload = @"{
    ""txjson"": {
        ""TransactionType"": ""TrustSet"",
        ""Account"": """ + address + @""",
        ""LimitAmount"": {
            ""currency"": ""DKP"",
            ""issuer"": """ + _DKP_ISSUER + @""",
            ""value"": ""500000000000""
        },
        ""Flags"": 131072
    },
    ""options"": {
        ""submit"": false,
        ""multisign"": false,
        ""expire"": 5,
        ""force_network"": ""MAINNET""
    },
    ""custom_meta"": {
        ""identifier"": """ + xummIdentifier + @""",
        ""blob"": """ + id + @""",
        ""instruction"": ""Please proceed to authenticate the trust set request, this is required for a DragonKill account 🔒.""
    }
}";
            /*
            string jsonPayload = @"{
    ""txjson"": {
        ""TransactionType"": ""TrustSet"",
        ""Account"": """ + address + @""",
        ""LimitAmount"": {
            ""currency"": ""DKP"",
            ""issuer"": """ + _DKP_ISSUER + @""",
            ""value"": ""500000000000""
        }
    },
    ""options"": {
        ""submit"": false,
        ""multisign"": false,
        ""expire"": 5,
        ""force_network"": ""MAINNET""
    },
    ""custom_meta"": {
        ""identifier"": """ + xummIdentifier + @""",
        ""blob"": """ + id + @""",
        ""instruction"": ""Please proceed to authenticate the trust set request, this is required for a DragonKill account 🔒.""
    }
}";*/
/*
            //send these over to heroku
            var payload = new TransactionPayloadTrust
            {
                txjson = new TrustSet
                {
                    TransactionType = "TrustSet",
                    Account = address,
                    LimitAmount = new LimitAmount
                    {
                        currency = "DKP",
                        issuer = _DKP_ISSUER,
                        value = "500000000000" // New limit of 10 billion dkp
                    }
                },
                options = new Options
                {
                    submit = false,
                    multisign = false,
                    expire = 5,
                    force_network = "MAINNET"
                },
                custom_meta = new CustomMeta
                {
                    identifier = xummIdentifier,
                    blob = id,
                    instruction = "Please proceed to authenticate the trust set request, this is required for a DragonKill account 🔒."
                }
            };
            */
            //string jsonPayload = JsonConvert.SerializeObject(payload);
            // Use the HttpClient to make a POST request
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", _DKP_XAPP_FIRST);
                httpClient.DefaultRequestHeaders.Add("x-api-secret", _DKP_XAPP_SECOND);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://xumm.app/api/v1/platform/payload", content);
                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response Content: " + responseContent);
                    var xummResponse = JsonConvert.DeserializeObject<XummResponse>(responseContent);
                    string qrUrl = xummResponse.refs.qr_png; // Access the qr_png field under refs
                    print($"{qrUrl} was the provided qrURL!");
                    var msg = new SendQRCodeUrlMessage { qrCodeUrl = qrUrl };
                    conn.Send(msg);
                    Debug.LogError($"Sending payload from XUMM to {msg.qrCodeUrl} playfabAccount");
                    string payloadId = xummResponse.uuid; // Retrieve the payload ID from the xummResponse
                    print(payloadId + " is payload id!");
                    int pollingInterval = 10000; // Poll every 10 seconds
                    await MonitorTrustlineRegister(payloadId, pollingInterval, conn, tries, address, "50000");
                    //Stop here we need to reconfigure this so that it can get the proper stuff it needs for xrpl
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error sending payload to XUMM: " + response.StatusCode);
                    Debug.LogError("Error Details: " + errorContent);
                    var msg = new XummMessage { code = "Error on payload, we will generate you a new QR code in just a few moments.", error = true };
                    conn.Send(msg);
                    await Task.Delay(5000);
                    print($"{tries} is our trust set tries");
                    RegisterTrustSet(conn, playfabID, tries --, address);

                }

            }
        }
        private async Task MonitorTrustlineRegister(string payloadId, int pollingInterval, NetworkConnectionToClient conn, int registerTries, string address, string requiredAmount){
            bool validatorNodeVerified = false;
            XummDetailedResponse savedResponse = null;
            string decodedID = string.Empty;
            PlayerInfo playerinfo = (PlayerInfo)conn.authenticationData;

            while (true){
                if(!activeConnectionIds.Contains(conn.connectionId)){
                    print("connection was disconnected");
                    return;
                }
                print("Polling MonitorTrustlineRegister");
                print(payloadId + " is payload id!");
                if(string.IsNullOrEmpty(address)){
                    address = "Undefined";
                }
                var xummstatus = await CheckXummStatusAPP(payloadId, address);
                if(xummstatus != null){
                    savedResponse = xummstatus;
                    if (xummstatus.meta.signed && !xummstatus.meta.submit && xummstatus.meta.resolved){
                        print("right before decodeID");
                        if(!string.IsNullOrEmpty(xummstatus.custom_meta.blob)){
                            decodedID = DecryptString(xummstatus.custom_meta.blob);
                        } else {
                            decodedID = playerinfo.PlayFabId;
                        }
                        if(string.IsNullOrEmpty(decodedID)){
                            print("no decode for some reason, we changed it out to make sure we are good to go");
                            decodedID = playerinfo.PlayFabId;
                        }
                        //decodedID = DecryptString(xummstatus.custom_meta.blob);
                        print($"the player who requested to MonitorTrustlineRegister had a playfab of {decodedID} and their public xrp wallet was {xummstatus.response.account}, {xummstatus.response.txid} was our txid");
                        //lets check xrpl now 
                        print($"Polling rippleD here is the hex {xummstatus.response.hex}");
                        var msg = new XummMessage { code = "Trust set was sent to XRPL from Xumm", error = false };
                        conn.Send(msg);
                        var (xrplStatus, status) = await SubmitBlobToXRPL(xummstatus.response.hex);
                        if(xrplStatus){
                            break; // Exit the loop
                        }
                    } else {
                        if(!xummstatus.meta.signed){

                            var msg = new XummMessage { code = "The trust set QR code was cancelled, trying again in a few moments." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            print($"{registerTries} is our register tries amount, running register on cancel payload");
                            RegisterTrustSet(conn, decodedID, registerTries, address);
                            return;
                        }
                        if(xummstatus.meta.expired){
                            var msg = new XummMessage { code = "The trust set QR code has expired, trying again in a few seconds." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            print($"{registerTries} is our register tries amount, running register on expired payload payload");
                            RegisterTrustSet(conn, decodedID, registerTries, address);
                            return;
                        }
                    }
                } else {
                    var msg = new XummMessage { code = "Checking status trust set status one moment please" , error = true, pending = true };
                    conn.Send(msg);
                }
                await Task.Delay(pollingInterval);
            }
            print("Transaction was signed lets move on to looking for the validated payload");
            await Task.Delay(10000);
            int tries = 0;
            while(!validatorNodeVerified && savedResponse != null && tries < 3){
                tries ++;
                print($"Polling rippleD try # {tries}");
                var msg = new XummMessage { code = "Asking the XRP ledger to verify your trust set for DKP", error = false };
                conn.Send(msg);
                var validated = await ValidateTrustlineTransaction(savedResponse.response.txid);
                if (validated){
                    PurchaseDKPMarketPriceRegistration(conn, decodedID, registerTries, savedResponse.response.account, requiredAmount);
                    return; // Exit the loop
                }
                await Task.Delay(pollingInterval);
            }
            RegisterTrustSet(conn, decodedID, registerTries, address);

        }
    async void ReassignTrustSet(NetworkConnectionToClient conn, string playfabID, int tries, string amount){
        tries ++;
        string id = EncryptString(playfabID);
            string xummIdentifier = Guid.NewGuid().ToString();
            // Prepare your payload object here.
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"{_Heroku_URL}/xummqueue?id={id}", null);  // No payload
                if (response.IsSuccessStatusCode)
                {
                    // Successfully joined the queue
                    print("queue is ready lets go!");
                }
                else
                {
                    // Log the failure or take other actions
                    Debug.LogError("Failed to join the queue, retrying...");
                    ReassignTrustSet(conn, playfabID, tries --, amount);
                    return;
                    // Wait before the next retry
                }
            }
            ScenePlayer stash = conn.identity.gameObject.GetComponent<ScenePlayer>();

            //send these over to heroku
            var payload = new TransactionPayloadTrust
            {
                txjson = new TrustSet
                {
                    TransactionType = "TrustSet",
                    Account = stash.GetTacticianSheet().Address,
                    LimitAmount = new LimitAmount
                    {
                        currency = "DKP",
                        issuer = _DKP_ISSUER,
                        value = "500000000000" // New limit of 10 billion dkp
                    }
                },
                options = new Options
                {
                    submit = false,
                    multisign = false,
                    expire = 5,
                    force_network = "MAINNET"
                },
                custom_meta = new CustomMeta
                {
                    identifier = xummIdentifier,
                    blob = id,
                    instruction = "Please proceed to authenticate the trust set request, this is required for a DragonKill account 🔒."
                }
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            // Use the HttpClient to make a POST request
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", _DKP_XAPP_FIRST);
                httpClient.DefaultRequestHeaders.Add("x-api-secret", _DKP_XAPP_SECOND);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://xumm.app/api/v1/platform/payload", content);
                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response Content: " + responseContent);
                    var xummResponse = JsonConvert.DeserializeObject<XummResponse>(responseContent);
                    string qrUrl = xummResponse.refs.qr_png; // Access the qr_png field under refs
                    print($"{qrUrl} was the provided qrURL!");
                    var msg = new XummTransmute { qrCodeUrl = qrUrl, error = false};
                    conn.Send(msg);
                    Debug.LogError($"Sending payload from XUMM to {msg.qrCodeUrl} playfabAccount");
                    string payloadId = xummResponse.uuid; // Retrieve the payload ID from the xummResponse
                    int pollingInterval = 10000; // Poll every 10 seconds
                    await MonitorTrustlineReassigned(payloadId, pollingInterval, conn, tries, amount);
                    //Stop here we need to reconfigure this so that it can get the proper stuff it needs for xrpl
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error sending payload to XUMM: " + response.StatusCode);
                    Debug.LogError("Error Details: " + errorContent);
                    var msg = new XummTransmute { code = "Error on payload, we will generate you a new QR code in just a few moments.", error = true };
                    conn.Send(msg);
                    await Task.Delay(5000);
                    print($"{tries} is our register tries amount, we had issue on making payload for xumm trying again");
                    ReassignTrustSet(conn, playfabID, tries --, amount);

                }

            }
    }
    private async Task MonitorTrustlineReassigned(string payloadId, int pollingInterval, NetworkConnectionToClient conn, int registerTries, string amount){
            bool validatorNodeVerified = false;
            XummDetailedResponse savedResponse = null;
            string decodedID = string.Empty;
            while (true){
                if(!activeConnectionIds.Contains(conn.connectionId)){
                    print("connection was disconnected");
                    return;
                }
                print("Polling XUMM 1");
                print(payloadId + " is payload id!");
                var xummstatus = await CheckXummStatusAPP(payloadId, "Undefined");
                if(xummstatus != null){
                    savedResponse = xummstatus;
                    if (xummstatus.meta.signed && !xummstatus.meta.submit && xummstatus.meta.resolved){
                        print("right before decodeID");
                        decodedID = DecryptString(xummstatus.custom_meta.blob);
                        print($"the player who requested to register had a playfab of {decodedID} and their public xrp wallet was {xummstatus.response.account}, {xummstatus.response.txid} was our txid");
                        //lets check xrpl now 
                        print($"Polling rippleD here is the hex {xummstatus.response.hex}");
                        var msg = new XummTransmute { code = "Trust set was sent to XRPL from Xumm", error = false };
                        conn.Send(msg);
                        var (xrplStatus, status) = await SubmitBlobToXRPL(xummstatus.response.hex);
                        if(xrplStatus){
                            break; // Exit the loop
                        }
                    } else {
                        if(!xummstatus.meta.signed && xummstatus.meta.resolved){

                            var msg = new XummTransmute { code = "The trust set QR code was cancelled, trying again in a few moments." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            print($"{registerTries} is our register tries amount, running register on cancel payload");
                            ReassignTrustSet(conn, decodedID, registerTries, amount);
                            return;
                        }
                        if(xummstatus.meta.expired){
                            var msg = new XummTransmute { code = "The trust set QR code has expired, trying again in a few seconds." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            print($"{registerTries} is our register tries amount, running register on expired payload payload");
                            ReassignTrustSet(conn, decodedID, registerTries, amount);
                            return;
                        }
                    }
                } else {
                    var msg = new XummTransmute { code = "Checking status one moment please" , error = true };
                    conn.Send(msg);
                }
                await Task.Delay(pollingInterval);
            }
            print("Transaction was signed lets move on to looking for the validated payload");
            await Task.Delay(10000);
            int tries = 0;
            while(!validatorNodeVerified && savedResponse != null && tries < 3){
                tries ++;
                print($"Polling rippleD try # {tries}");
                var msg = new XummTransmute { code = "Asking the XRP ledger to verify your trust set for DKP", error = false };
                conn.Send(msg);
                var validated = await ValidateTrustlineTransaction(savedResponse.response.txid);
                if (validated){
                    // Success, credit the account
                    DKPTOGOLDTRANSMUTE(conn, decodedID, amount);
                    return; // Exit the loop
                }
                await Task.Delay(pollingInterval);
            }
            ReassignTrustSet(conn, decodedID, registerTries, amount);
        }
        async void PurchaseDKPMarketPriceRegistration(NetworkConnectionToClient conn, string playfabID, int tries, string addressPUB, string amountRequired){
            if(conn == null){
                print("connection was disconnected");
                return;
            }
            if(!activeConnectionIds.Contains(conn.connectionId)){
                    print("connection was disconnected");
                    return;
                }
                
            var marketPrice = await GetFairMarketValueRegisterPrice(amountRequired);
            print($"market price is {marketPrice.meta.bestMarketPrice}");
            //float xrpDrops = float.Parse(marketPrice.meta.bestMarketPrice) * 1000000f;
            string id = EncryptString(playfabID);
            string xummIdentifier = Guid.NewGuid().ToString();

            /*
            string jsonPayload = @"{
    ""txjson"": {
        ""TransactionType"": ""OfferCreate"",
        ""TakerPays"": {
            ""currency"": ""DKP"",
            ""issuer"": """ + _DKP_ISSUER + @""",
            ""value"": ""50000""
        },
        ""TakerGets"": {
            ""currency"": ""XRP"",
            ""value"": """ + xrpDrops.ToString() + @"""
        }
    },
    ""options"": {
        ""submit"": false,
        ""multisign"": false,
        ""expire"": 5,
        ""force_network"": ""MAINNET""
    },
    ""custom_meta"": {
        ""identifier"": """ + xummIdentifier + @""",
        ""blob"": """ + id + @""",
        ""instruction"": ""Please proceed to authenticate the market order request.""
    }
}";
*/
double calculatedMarketPrice = double.Parse(marketPrice.meta.bestMarketPrice);
long xrpDrops = (long) Math.Round(calculatedMarketPrice * 1000000); // Convert to drops and round to nearest integer

string jsonPayload = @"{
    ""txjson"": {
        ""TransactionType"": ""OfferCreate"",
        ""Account"": """ + addressPUB + @""",
        ""TakerPays"": {
            ""currency"": ""DKP"",
            ""issuer"": """ + _DKP_ISSUER + @""",
            ""value"": ""50000""
        },
        ""TakerGets"": """ + xrpDrops.ToString() + @"""
    },
    ""options"": {
        ""submit"": false,
        ""multisign"": false,
        ""expire"": 5,
        ""force_network"": ""MAINNET""
    },
    ""custom_meta"": {
        ""identifier"": """ + xummIdentifier + @""",
        ""blob"": """ + id + @""",
        ""instruction"": ""Please proceed to authenticate the market order request.""
    }
}";
            // Use the HttpClient to make a POST request
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", _DKP_XAPP_FIRST);
                httpClient.DefaultRequestHeaders.Add("x-api-secret", _DKP_XAPP_SECOND);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://xumm.app/api/v1/platform/payload", content);
                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response Content: " + responseContent);
                    var xummResponse = JsonConvert.DeserializeObject<XummResponse>(responseContent);
                    string qrUrl = xummResponse.refs.qr_png; // Access the qr_png field under refs
                    print($"{qrUrl} was the provided qrURL!");
                    var msg = new SendQRCodeUrlMessage { qrCodeUrl = qrUrl };
                    conn.Send(msg);
                    Debug.LogError($"Sending payload from XUMM to {msg.qrCodeUrl} playfabAccount");
                    string payloadId = xummResponse.uuid; // Retrieve the payload ID from the xummResponse
                    print(payloadId + " is payload id!");
                    string expectedAmount = "50000"; // The expected amount in your token
                    int pollingInterval = 10000; // Poll every 10 seconds
                    await MonitorTransactionStatusMARKET(payloadId, expectedAmount, pollingInterval, conn, tries, addressPUB, amountRequired);
                    //Stop here we need to reconfigure this so that it can get the proper stuff it needs for xrpl
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error sending payload to XUMM: " + response.StatusCode);
                    Debug.LogError("Error Details: " + errorContent);
                    var msg = new XummMessage { code = "Error on payload, we will generate you a new QR code in just a few moments.", error = true };
                    conn.Send(msg);
                    await Task.Delay(5000);
                    print($"{tries} is our register tries amount, we had issue on making payload for xumm trying again");
                    PurchaseDKPMarketPriceRegistration(conn, playfabID, tries, addressPUB, amountRequired);
                }

            }
        }
         private async Task<XummDetailedResponse> GetFairMarketValueRegisterPrice(string amountRequired){
        using (var httpClient = new HttpClient())
    {
        // Replace with your Heroku app's check endpoint
        //var response = await httpClient.GetAsync($"{_Heroku_URL}/check-payload/{payloadId}");
        var payload = new { amount = amountRequired };
        var json = JsonConvert.SerializeObject(payload);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        // Replace with your Heroku app's check endpoint
        var response = await httpClient.PostAsync($"{_Heroku_URL}/GetMarketPrice", data);
        //var response = await httpClient.GetAsync($"{_Heroku_URL}/GetMarketPrice");
        if (response.IsSuccessStatusCode)
        {
            print("Success on getting Heroku status code");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Deserialize the response to XummDetailedResponse.
            // Make sure your Heroku app sends the response in this format.
            var xummDetailedResponse = JsonConvert.DeserializeObject<XummDetailedResponse>(responseContent);
            
            return xummDetailedResponse;
        }
        else
        {
            Debug.LogError("Error checking Heroku status: " + response.StatusCode);
            return null;
        }
    }
    }
        private async Task MonitorTransactionStatusMARKET(string payloadId, string expectedAmount, int pollingInterval, NetworkConnectionToClient conn, int registerTries, string addressPUB, string amountRequired){
            bool validatorNodeVerified = false;
            XummDetailedResponse savedResponse = null;
            string decodedID = string.Empty;
            while (true){
                if(!activeConnectionIds.Contains(conn.connectionId)){
                    print("connection was disconnected");
                    return;
                }
                print("Polling MonitorTransactionStatusMARKET");
                print(payloadId + " is payload id!");
                var xummstatus = await CheckXummStatusAPP(payloadId, addressPUB);
                if(xummstatus != null){
                    savedResponse = xummstatus;
                    if (xummstatus.meta.signed && !xummstatus.meta.submit && xummstatus.meta.resolved && !xummstatus.meta.TrustLineNotSet){
                        print("right before decodeID");
                        decodedID = DecryptString(xummstatus.custom_meta.blob);
                        print($"the player who requested to register had a playfab of {decodedID} and their public xrp wallet was {xummstatus.response.account}, {xummstatus.response.txid} was our txid");
                        //lets check xrpl now 
                        //so previously we were just sending after it was signed properly, but we need to verify this signing account has never been used before so how can we look through a wallets transactions to see if there are any from {xummstatus.response.account}?
                        print($"Polling rippleD here is the hex {xummstatus.response.hex}");
                        print("True paying customer lets get the the product!");
                        var msg = new XummMessage { code = "Trading XRP for DKP via the XRP Ledger's decentralized exchange one moment please", error = false };
                        conn.Send(msg);
                        var (xrplStatus, status) = await SubmitBlobToXRPL(xummstatus.response.hex);
                        if(xrplStatus){
                            break; // Exit the loop
                        }
                    } else {
                        if(!xummstatus.meta.signed && !xummstatus.meta.TrustLineNotSet){
                            var msg = new XummMessage { code = "The market purchase QR code was cancelled, trying again in a few moments." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            PurchaseDKPMarketPriceRegistration(conn, decodedID, registerTries, addressPUB, amountRequired);
                            return;
                        }
                        if(!xummstatus.meta.signed && xummstatus.meta.TrustLineNotSet){
                            var msg = new XummMessage { code = "No trust line detected, sending a trust set QR code in a few moments" , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterTrustSet(conn, decodedID, registerTries, xummstatus.response.account);
                            //create trustline setter now
                            return;
                        }
                        if(xummstatus.meta.signed && xummstatus.meta.TrustLineNotSet){
                            var msg = new XummMessage { code = "No trust line detected, sending a trust set QR code in a few moments" , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterTrustSet(conn, decodedID, registerTries, xummstatus.response.account);
                            //create trustline setter now
                            return;
                        }
                        if(!xummstatus.meta.TrustLineNotSet && xummstatus.meta.expired){
                            var msg = new XummMessage { code = "The registration QR code has expired, trying again in a few seconds." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            PurchaseDKPMarketPriceRegistration(conn, decodedID, registerTries, addressPUB, amountRequired);
                            return;
                        }
                         if(xummstatus.meta.TrustLineNotSet && xummstatus.meta.expired){
                            var msg = new XummMessage { code = "No trust line detected, sending a trust set QR code in a few moments" , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterTrustSet(conn, decodedID, registerTries, xummstatus.response.account);
                            //create trustline setter now
                            return;
                        }
                    }
                } else {
                    var msg = new XummMessage { code = "Checking status of our market buy for DKP" , error = true, pending = true };
                    conn.Send(msg);
                }
                await Task.Delay(pollingInterval);
            }
            print("Transaction was signed lets move on to looking for the validated payload");
            await Task.Delay(10000);
            int tries = 0;
            while(!validatorNodeVerified && savedResponse != null && tries < 3){
                tries ++;
                print($"Polling rippleD try # {tries}");
                var msg = new XummMessage { code = "Asking the XRP ledger to verify your registration", error = false };
                conn.Send(msg);
                var validated = await ValidateMARKETORDER(savedResponse.response.txid);
                if (validated){
                    // Success, credit the account
                    RegisterAccount(conn, decodedID, registerTries, addressPUB);
                    return;
                }
                await Task.Delay(pollingInterval);
            }
            PurchaseDKPMarketPriceRegistration(conn, decodedID, registerTries, addressPUB, amountRequired);
        }
        async void RegisterAccount(NetworkConnectionToClient conn, string playfabID, int tries, string pubAddress){
            print($"Tries: {tries} are how many times we have tried to register");
            if(conn == null){
                print("No connection found when trying to register account");
                return;
            }
            //if(tries > 2){
            //    var msg = new XummMessage { code = "Too many attempts to register a wallet to this account, please try again later, for now we need to rest", error = true, quit = true };
            //    conn.Send(msg);
            //    return;
            //}
            tries++;
            string id = EncryptString(playfabID);
            string xummIdentifier = Guid.NewGuid().ToString();
            // Prepare your payload object here.
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"{_Heroku_URL}/xummqueue?id={id}", null);  // No payload
                if (response.IsSuccessStatusCode)
                {
                    // Successfully joined the queue
                    print("queue is ready lets go!");
                }
                else
                {
                    // Log the failure or take other actions
                    Debug.LogError("Failed to join the queue, retrying...");
                    RegisterAccount(conn, playfabID, tries --, pubAddress);
                    return;
                    // Wait before the next retry
                }
            }

            //send these over to heroku
            var payload = new TransactionPayload
            {
                txjson = new TxJson
                {
                    TransactionType = "Payment",
                    Destination = _DKP_DESTINATION_ADDRESS,
                    Amount = new Amount
                    {
                        currency = "DKP",
                        issuer = _DKP_ISSUER,
                        value = "50000" // 50,000 DKP
                    }
                },
                options = new Options
                {
                    submit = false,
                    multisign = false,
                    expire = 5,
                    force_network = "MAINNET"
                },
                custom_meta = new CustomMeta
                {
                    identifier = xummIdentifier,
                    blob = id,
                    instruction = "Please proceed to authenticate the payment request to link your XUMM wallet with your DragonKill account 🐉. Warning, once you link this wallet it cannot be linked to another DragonKill account 🔒."
                }
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            // Use the HttpClient to make a POST request
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", _DKP_XAPP_FIRST);
                httpClient.DefaultRequestHeaders.Add("x-api-secret", _DKP_XAPP_SECOND);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://xumm.app/api/v1/platform/payload", content);
                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response Content: " + responseContent);
                    var xummResponse = JsonConvert.DeserializeObject<XummResponse>(responseContent);
                    string qrUrl = xummResponse.refs.qr_png; // Access the qr_png field under refs
                    print($"{qrUrl} was the provided qrURL!");
                    var msg = new SendQRCodeUrlMessage { qrCodeUrl = qrUrl };
                    conn.Send(msg);
                    Debug.LogError($"Sending payload from XUMM to {msg.qrCodeUrl} playfabAccount");
                    string payloadId = xummResponse.uuid; // Retrieve the payload ID from the xummResponse
                    print(payloadId + " is payload id!");
                    string expectedAmount = "50000"; // The expected amount in your token
                    int pollingInterval = 10000; // Poll every 10 seconds
                    await MonitorTransactionStatus(payloadId, expectedAmount, pollingInterval, conn, tries, pubAddress);
                    //Stop here we need to reconfigure this so that it can get the proper stuff it needs for xrpl
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error sending payload to XUMM: " + response.StatusCode);
                    Debug.LogError("Error Details: " + errorContent);
                    var msg = new XummMessage { code = "Error on payload, we will generate you a new QR code in just a few moments.", error = true };
                    conn.Send(msg);
                    await Task.Delay(5000);
                    print($"{tries} is our register tries amount, we had issue on making payload for xumm trying again");
                    RegisterAccount(conn, playfabID, tries ++, pubAddress);
                }

            }
        }
        private async Task MonitorTransactionStatus(string payloadId, string expectedAmount, int pollingInterval, NetworkConnectionToClient conn, int registerTries, string pubAddress){
            bool validatorNodeVerified = false;
            XummDetailedResponse savedResponse = null;
            string decodedID = string.Empty;
            PlayerInfo playerinfo = (PlayerInfo)conn.authenticationData;

            while (true){
                if(!activeConnectionIds.Contains(conn.connectionId)){
                    print("connection was disconnected");
                    return;
                }
                print("Polling MonitorTransactionStatus");
                print(payloadId + " is payload id!");
                if(pubAddress == null){
                    pubAddress = "Undefined";
                }
                var xummstatus = await CheckXummStatusAPP(payloadId, pubAddress);
                if(xummstatus != null){
                    savedResponse = xummstatus;
                    if(!string.IsNullOrEmpty(xummstatus.custom_meta.blob)){
                        decodedID = DecryptString(xummstatus.custom_meta.blob);
                    } else {
                        decodedID = playerinfo.PlayFabId;
                    }
                    if(string.IsNullOrEmpty(decodedID)){
                        print("no decode for some reason, we changed it out to make sure we are good to go");
                        decodedID = playerinfo.PlayFabId;
                    }
                    if (xummstatus.meta.signed && !xummstatus.meta.submit && xummstatus.meta.resolved && !xummstatus.meta.TrustLineNotSet && !xummstatus.meta.NotEnoughLiquidityButHasTrustLine){
                        print("right before decodeID");
                        decodedID = DecryptString(xummstatus.custom_meta.blob);
                        if(string.IsNullOrEmpty(decodedID)){
                            print("no decode for some reason, we changed it out to make sure we are good to go");
                            decodedID = playerinfo.PlayFabId;
                        }
                        print($"the player who requested to register had a playfab of {decodedID} and their public xrp wallet was {xummstatus.response.account}, {xummstatus.response.txid} was our txid");
                        //lets check xrpl now 
                        //so previously we were just sending after it was signed properly, but we need to verify this signing account has never been used before so how can we look through a wallets transactions to see if there are any from {xummstatus.response.account}?
                        print($"Polling rippleD here is the hex {xummstatus.response.hex}");
                        var checkPlayersWallet = await VerifyPlayerPayment(xummstatus.response.account);
                        if(checkPlayersWallet){
                            print("Warning Double registration!!");
                            //return;
                            //first echo warning, lets send them a new QR code? lets make it so there is a counter though they only get one chance after and then lock um out
                            var msg = new XummMessage { code = "The wallet you have tried to sign with has already been registered to another DragonKill account, please try another." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            //print($"{registerTries} is our register tries amount, running register on double registration");
                            //RegisterAccount(conn, decodedID, registerTries);
                            //return;
                        } else {
                            print("True paying customer lets get the the product!");
                            var msg = new XummMessage { code = "Registration request sent from XUMM to XRPL awaiting a validator node reponse", error = false };
                            conn.Send(msg);
                        }
                        var (xrplStatus, status) = await SubmitBlobToXRPL(xummstatus.response.hex);
                        if(xrplStatus){
                            break; // Exit the loop
                        }
                    } else {
                        if(!string.IsNullOrEmpty(xummstatus.response.account)){
                            pubAddress = xummstatus.response.account;
                            print($"{xummstatus.response.account} is our account address we are about to call something that isnt the accept");
                        }
                        if(xummstatus.meta.NotEnoughLiquidityButHasTrustLine){
                             var msg = new XummMessage { code = "Trust line set for DKP but not enough liquidity, fetching the market order to accomplish registration please double check" , error = true };
                            conn.Send(msg);
                            PurchaseDKPMarketPriceRegistration(conn, decodedID, registerTries, pubAddress, xummstatus.meta.RequiredAmount);
                            return;
                        }
                        
                        if(!xummstatus.meta.signed && !xummstatus.meta.TrustLineNotSet && !xummstatus.meta.NoAddressSendBackToMain){
                            var msg = new XummMessage { code = "No trust line detected, sending a trust set QR code in a few moments" , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterTrustSet(conn, decodedID, registerTries --, pubAddress);
                            print("DETECTED NO TRUSTLINE!! STARTING REGISTER TRUST SET");
                            return;
                        }
                        if(!xummstatus.meta.signed && !xummstatus.meta.TrustLineNotSet && xummstatus.meta.NoAddressSendBackToMain){
                            var msg = new XummMessage { code = "The registration QR code was cancelled. If you have zero DKP and a trust line for DKP you can remove your trustline for DKP on your xumm app and then rescan the next QR code and follow the process to register seamlessly, otherwise you can purchase more DKP on the xumm app via the decentralized exchange on the XRP ledger" , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterAccount(conn, decodedID, registerTries, pubAddress);
                            return;
                        }
                        if(xummstatus.meta.signed && xummstatus.meta.TrustLineNotSet){
                            var msg = new XummMessage { code = "No trust line detected, sending a trust set QR code in a few moments" , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterTrustSet(conn, decodedID, registerTries --, pubAddress);
                            print("DETECTED NO TRUSTLINE!! STARTING REGISTER TRUST SET");
                            return;
                        }
                        if(!xummstatus.meta.signed && xummstatus.meta.TrustLineNotSet){
                            var msg = new XummMessage { code = "No trust line detected, sending a trust set QR code in a few moments" , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterTrustSet(conn, decodedID, registerTries --, pubAddress);
                            print("DETECTED NO TRUSTLINE!! STARTING REGISTER TRUST SET");
                            //create trustline setter now
                            return;
                        }
                        if(xummstatus.meta.expired && !xummstatus.meta.TrustLineNotSet){
                            var msg = new XummMessage { code = "The registration QR code has expired, trying again in a few seconds." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterAccount(conn, decodedID, registerTries, pubAddress);
                            return;
                        }
                        if(xummstatus.meta.expired && xummstatus.meta.TrustLineNotSet){
                            var msg = new XummMessage { code = "The registration QR code has expired, trying again in a few seconds." , error = true };
                            conn.Send(msg);
                            await Task.Delay(10000);
                            RegisterTrustSet(conn, decodedID, registerTries --, pubAddress);
                            print("DETECTED NO TRUSTLINE!! STARTING REGISTER TRUST SET");
                            return;
                        }
                        
                    }
                } else {
                    var msg = new XummMessage { code = "Checking status one moment please" , error = true, pending = true };
                    conn.Send(msg);
                }
                await Task.Delay(pollingInterval);
            }
            print("Transaction was signed lets move on to looking for the validated payload");
            await Task.Delay(10000);
            int tries = 0;
            print("validatorNodeVerified && savedResponse != null && tries");
            while(!validatorNodeVerified && savedResponse != null && tries < 3){
                tries ++;
                print($"Polling rippleD try # {tries}");
                var msg = new XummMessage { code = "Asking the XRP ledger to verify your registration", error = false };
                conn.Send(msg);
                    print("RIGHT BEFORE VALIDATE");
            
                var (validated, validatedAmount) = await ValidateXrplTransaction(savedResponse.response.txid);
                if (validated && validatedAmount == expectedAmount){
                    // Success, credit the account
                    print($"paid {validatedAmount} amount");
                    Dictionary<string, string> newPlayerData = new Dictionary<string, string>();
                    print("marker 1");
                    if(pubAddress == "Undefined"){
                        newPlayerData.Add("PUBLICADDRESS", savedResponse.response.account);
                    print("marker 2");

                    } else {
                        newPlayerData.Add("PUBLICADDRESS", pubAddress);
                    print("marker 2.5");

                    }
                    print("marker 3");

                    PlayFabServerAPI.UpdateUserData(new UpdateUserDataRequest
                    {
                        PlayFabId = decodedID,
                        Data = newPlayerData
                    }, result =>
                    {
                    print("marker 4");

                    print("marker 5");

                        if(pubAddress == "Undefined"){
                            playerinfo.XRPLPUBLIC = savedResponse.response.account;
                    print("marker 6");
                        } else {
                            playerinfo.XRPLPUBLIC = pubAddress;
                    print("marker 6.5");
                        }
                        conn.authenticationData = playerinfo;
                        //print($"Successfully registered XUMM account {savedResponse.response.account} to playfab account {decodedID}");
                    print("marker 7");
                        var msg = new XummMessage { code = "Registration tx successfully validated on the XRP Ledger. Welcome to DragonKill!", error = false };
                        if(pubAddress == "Undefined"){
                    print("marker 8");
                            conn.Send(msg);
                        conn.Send<Noob>(new Noob {
                            finished = false,
                            Address = savedResponse.response.account
                        });
                        } else {
                    print("marker 8.5");
                            conn.Send(msg);
                        conn.Send<Noob>(new Noob {
                            finished = false,
                            Address = pubAddress
                        });
                        }
                        //conn.Send(msg);
                        //conn.Send<Noob>(new Noob {
                        //    finished = false,
                        //    Address = savedResponse.response.account
                        //});
                    }, error => {
                    
                    });
                    return; // Exit the loop
                }
                else
                {
                    // Handle partial payment
                    print($"unpaid because {validatedAmount} was amount");
                     print($"{registerTries} is our register tries amount, running register on cancel payload");
                }
                await Task.Delay(pollingInterval);
            }
            print("RegisterAccount final road was casted not looking good!!!!!!!!!!!!!!!!!!!!!!!!!");

            RegisterAccount(conn, decodedID, registerTries, pubAddress);

        }
        
        private async Task<(bool, string)> SubmitBlobToXRPL(string transactionBlob){
        using (var httpClient = new HttpClient())
        {
            var postData = new
            {
                method = "submit",
                @params = new object[]
                {
                    new
                    {
                        tx_blob = transactionBlob
                    }
                }
            };
            var jsonString = JsonConvert.SerializeObject(postData);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://s1.ripple.com:51234/", httpContent); // Adjust the endpoint URL accordingly
            if (response.IsSuccessStatusCode){
                // Successfully submitted to XRPL
                var responseContent = await response.Content.ReadAsStringAsync();
                print("Success on submitting to xrpl!");
                // Further logic
                return (true, "PASS");;
            } else {
                // Handle error
                Debug.LogError("Error submitting transaction blob to XRPL: " + response.StatusCode);
                return (false, "Xrpl error");
            }
        }
    }
    private async Task<bool> ValidateTrustlineTransaction(string transactionId)
{
    using (var httpClient = new HttpClient())
    {
        var requestPayload = new
        {
            method = "tx",
            @params = new[]
            {
                new
                {
                    transaction = transactionId,
                    binary = false
                }
            }
        };

        var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://s1.ripple.com:51234/", content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var xrplTransactionResponse = JsonConvert.DeserializeObject<XrplTransactionResponse>(responseContent);

            if (xrplTransactionResponse.result.status == "success" &&
                xrplTransactionResponse.result.validated &&
                xrplTransactionResponse.result.TransactionType == "TrustSet" &&
                xrplTransactionResponse.result.LimitAmount != null && // Additional check for LimitAmount or other trustline specific fields
                xrplTransactionResponse.result.LimitAmount.currency == "DKP" && // Ensure the currency is as expected
                xrplTransactionResponse.result.LimitAmount.issuer == _DKP_ISSUER) // Ensure the issuer is as expected
            {
                return true;
            }
            else
            {
                Debug.LogError("Trustline is not validated or was not successful.");
            }
        }
        else
        {
            Debug.LogError("Error validating XRPL Trustline: " + response.StatusCode);
        }
    }

    return false;
}
    private async Task<(bool, string)> ValidateXrplTransaction(string transactionId){
        using (var httpClient = new HttpClient()){
            var requestPayload = new
            {
                method = "tx",
                @params = new[]
                {
                    new
                    {
                        transaction = transactionId,
                        binary = false
                    }
                }
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://s1.ripple.com:51234/", content);
            if (response.IsSuccessStatusCode){
                var responseContent = await response.Content.ReadAsStringAsync();
                print($"GOT OUR VALIDATED XRPL TRANSACTION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                var xrplTransactionResponse = JsonConvert.DeserializeObject<XrplTransactionResponse>(responseContent);
                print($"CONVERTED JSON TO XRPL TRANSACTION");

                if (xrplTransactionResponse.result.status == "success" && xrplTransactionResponse.result.validated && xrplTransactionResponse.result.meta.TransactionResult == "tesSUCCESS")            {
                    print($"CONVERTED JSON TO XRPL TRANSACTION");
                    return (true, xrplTransactionResponse.result.meta.delivered_amount.value);
                } else {
                    Debug.LogError("Transaction is not validated or was not successful.");
                }
            } else {
                Debug.LogError("Error validating XRPL transaction: " + response.StatusCode);
            }
        }
        return (false, "Transaction failed or could not be validated.");
    }
    private async Task <bool> ValidateMARKETORDER(string transactionId){
        using (var httpClient = new HttpClient()){
            var requestPayload = new
            {
                method = "tx",
                @params = new[]
                {
                    new
                    {
                        transaction = transactionId,
                        binary = false
                    }
                }
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://s1.ripple.com:51234/", content);
            if (response.IsSuccessStatusCode){
                var responseContent = await response.Content.ReadAsStringAsync();
                print($"GOT OUR VALIDATED XRPL TRANSACTION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                var xrplTransactionResponse = JsonConvert.DeserializeObject<XrplTransactionResponse>(responseContent);
                print($"CONVERTED JSON TO XRPL TRANSACTION");

                if (xrplTransactionResponse.result.status == "success" && xrplTransactionResponse.result.validated && xrplTransactionResponse.result.meta.TransactionResult == "tesSUCCESS")            {
                    print($"CONVERTED JSON TO XRPL TRANSACTION");
                    return true;
                } else {
                    Debug.LogError("Transaction is not validated or was not successful.");
                }
            } else {
                Debug.LogError("Error validating XRPL transaction: " + response.StatusCode);
            }
        }
        return false;
    }
    private async Task<string> CheckXummStatus(string payloadId){
        using (var httpClient = new HttpClient()){
            // Adding headers
            httpClient.DefaultRequestHeaders.Add("x-api-key", _DKP_XAPP_FIRST);
            httpClient.DefaultRequestHeaders.Add("x-api-secret", _DKP_XAPP_SECOND);
            var response = await httpClient.GetAsync($"https://xumm.app/api/v1/platform/payload/{payloadId}");
            if (response.IsSuccessStatusCode){
                var responseContent = await response.Content.ReadAsStringAsync();
                var xummStatusResponse = JsonConvert.DeserializeObject<XummStatusResponse>(responseContent);
                return xummStatusResponse.status;
            } else {
                Debug.LogError("Error checking XUMM status: " + response.StatusCode);
                return null;
            }
        }
    }
    private async Task<XummDetailedResponse> CheckXummStatusFull(string payloadId){
        using (var httpClient = new HttpClient()){
            // Add headers here as before...
            httpClient.DefaultRequestHeaders.Add("x-api-key", _DKP_XAPP_FIRST);
            httpClient.DefaultRequestHeaders.Add("x-api-secret", _DKP_XAPP_SECOND);
            var response = await httpClient.GetAsync($"https://xumm.app/api/v1/platform/payload/{payloadId}");
            if (response.IsSuccessStatusCode){
                // Read rate limit headers
                if (response.Headers.TryGetValues("X-RateLimit-Limit", out var rateLimitLimit)){
                    print($"Rate Limit (Max allowed): {string.Join(", ", rateLimitLimit)}");
                }
                if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var rateLimitRemaining)){
                    print($"Rate Limit (Remaining): {string.Join(", ", rateLimitRemaining)}");
                }
                if (response.Headers.TryGetValues("X-RateLimit-Reset", out var rateLimitReset)){
                    print($"Rate Limit (Reset time): {string.Join(", ", rateLimitReset)}");
                }
                print("Success on getting Xumm statuscode");
                var responseContent = await response.Content.ReadAsStringAsync();
                var xummDetailedResponse = JsonConvert.DeserializeObject<XummDetailedResponse>(responseContent);
                return xummDetailedResponse;
            } else {
                Debug.LogError("Error checking XUMM status: " + response.StatusCode);
                return null;
            }
        }
    }
    private async Task<XummDetailedResponse> CheckXummStatusAPP(string payloadId, string address){
        using (var httpClient = new HttpClient())
    {
        // Replace with your Heroku app's check endpoint
        //var response = await httpClient.GetAsync($"{_Heroku_URL}/check-payload/{payloadId}");
        var response = await httpClient.GetAsync($"{_Heroku_URL}/check-payload/{payloadId}/{address}");
        if (response.IsSuccessStatusCode)
        {
            print("Success on getting Heroku status code");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Deserialize the response to XummDetailedResponse.
            // Make sure your Heroku app sends the response in this format.
            var xummDetailedResponse = JsonConvert.DeserializeObject<XummDetailedResponse>(responseContent);
            
            return xummDetailedResponse;
        }
        else
        {
            Debug.LogError("Error checking Heroku status: " + response.StatusCode);
            return null;
        }
    }
    }
    async void DKPTOGOLDTRANSMUTE(NetworkConnectionToClient conn, string playfabID, string amount){
            if(conn == null){
                print("No connection found when trying to transmute");
                return;
            }
            ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>();

            string publicAddress = p.GetTacticianSheet().Address;
            string url = $"https://data.ripple.com/v2/accounts/{publicAddress}/trustlines";
            using (var httpClient = new HttpClient())
            {
                var trustlineResponse = await httpClient.GetAsync(url);
                if (trustlineResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await trustlineResponse.Content.ReadAsStringAsync();
                    Debug.Log("Received JSON: " + jsonResponse);
                    //XRPLTrustlineResponse trustlineData = JsonUtility.FromJson<XRPLTrustlineResponse>(jsonResponse);
//
                    //bool hasDKPTrustline = false;
                    //foreach (Trustline t in trustlineData.trustlines)
                    //{
                    //    if (t.currency == "DKP" && t.issuer == _DKP_ISSUER)
                    //    {
                    //        hasDKPTrustline = true;
                    //        break;
                    //    }
                    //}

                    //if (!hasDKPTrustline)
                    //{
                    //    Debug.LogError("No DKP trustline found for this account.");
                    //    // Send error message to client or take other actions
                    //    return;
                    //}
                }
                else
                {
                    Debug.LogError("Failed to fetch trustlines.");
                    // Handle error
                    return;
                }
            }

            string id = EncryptString(playfabID);
            string xummIdentifier = Guid.NewGuid().ToString();
            // Prepare your payload object here.
            int httpTries = 0;

            while(true && httpTries < 5){
                httpTries ++;
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync($"{_Heroku_URL}/xummqueue?id={id}", null);  // No payload
                    if (response.IsSuccessStatusCode)
                    {
                        // Successfully joined the queue
                        print("queue is ready lets go!");
                        break;
                    }
                    else
                    {
                        // Log the failure or take other actions
                        Debug.LogError("Failed to join the queue, retrying...");
                        await Task.Delay(10000);
                    }
                }
            }
            if(httpTries == 5){
                DKPTOGOLDTRANSMUTE(conn, playfabID, amount);
                return;
            }
            var payload = new TransactionPayload
            {
                txjson = new TxJson
                {
                    TransactionType = "Payment",
                    Destination = _DKP_DESTINATION_ADDRESS,
                    Amount = new Amount
                    {
                        currency = "DKP",
                        issuer = _DKP_ISSUER,
                        value = amount
                    }
                },
                options = new Options
                {
                    submit = false,
                    multisign = false,
                    expire = 5,
                    force_network = "MAINNET"
                },
                custom_meta = new CustomMeta
                {
                    identifier = xummIdentifier,
                    blob = id,
                    instruction = $"Transmuting {amount} DKP to Gold. Gold will appear in your DragonKill account 🐉."
                }
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            // Use the HttpClient to make a POST request
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", _DKP_XAPP_FIRST);
                httpClient.DefaultRequestHeaders.Add("x-api-secret", _DKP_XAPP_SECOND);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://xumm.app/api/v1/platform/payload", content);
                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response Content: " + responseContent);
                    var xummResponse = JsonConvert.DeserializeObject<XummResponse>(responseContent);
                    string qrUrl = xummResponse.refs.qr_png; // Access the qr_png field under refs
                    print($"{qrUrl} was the provided qrURL!");
                    var msg = new XummTransmute { qrCodeUrl = qrUrl, error = false };
                    conn.Send(msg);
                    Debug.LogError($"Sending payload from XUMM to {msg.qrCodeUrl} playfabAccount");
                    string payloadId = xummResponse.uuid; // Retrieve the payload ID from the xummResponse
                    int pollingInterval = 10000; // Poll every 10 seconds
                    await MonitorXUMMTRANSMUTEStatus(payloadId, amount, pollingInterval, conn);
                    //Stop here we need to reconfigure this so that it can get the proper stuff it needs for xrpl
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error sending payload to XUMM: " + response.StatusCode);
                    Debug.LogError("Error Details: " + errorContent);
                    //var msg = new XummMessage { code = "Error on payload, we will generate you a new QR code in just a few moments.", error = true };
                    //conn.Send(msg);
                    await Task.Delay(10000);
                }

            }
        }
        private async Task MonitorXUMMTRANSMUTEStatus(string payloadId, string expectedAmount, int pollingInterval, NetworkConnectionToClient conn){
            ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>();
            bool validatorNodeVerified = false;
            XummDetailedResponse savedResponse = null;
            string decodedID = string.Empty;
            while (true){
                if(!activeConnectionIds.Contains(conn.connectionId)){
                    print("connection was disconnected");
                    return;
                }
                print("Polling XUMM");
                var xummstatus = await CheckXummStatusAPP(payloadId, p.GetTacticianSheet().Address);
                if(xummstatus != null){
                    savedResponse = xummstatus;
                    if (xummstatus.meta.signed && !xummstatus.meta.submit && xummstatus.meta.resolved){
                        print("right before decodeID");
                        decodedID = DecryptString(xummstatus.custom_meta.blob);
                        print($"the player who requested to register had a playfab of {decodedID} and their public xrp wallet was {xummstatus.response.account}, {xummstatus.response.txid} was our txid");
                        //lets check xrpl now 
                        //so previously we were just sending after it was signed properly, but we need to verify this signing account has never been used before so how can we look through a wallets transactions to see if there are any from {xummstatus.response.account}?
                        print($"Polling rippleD here is the hex {xummstatus.response.hex}");
                        var (xrplStatus, status) = await SubmitBlobToXRPL(xummstatus.response.hex);
                        if(xrplStatus){
                            break; // Exit the loop
                        }
                    }
                    if(xummstatus.meta.wrongSigner){
                        var _message = new XummTransmute { code = "15", error = true };
                        conn.Send(_message);
                        if (!(conn.authenticationData is PlayerInfo playerData)) {
                            Debug.LogWarning("Authentication data is missing or not of type PlayerInfo.");
                            return;
                        }
                        DKPTOGOLDTRANSMUTE(conn, playerData.PlayFabId, expectedAmount);
                        return;
                    }
                    if(xummstatus.meta.TrustLineNotSet){
                        var _message = new XummTransmute { code = "25", error = true };
                        conn.Send(_message);
                        if (!(conn.authenticationData is PlayerInfo playerData)) {
                            Debug.LogWarning("Authentication data is missing or not of type PlayerInfo.");
                            return;
                        }
                        ReassignTrustSet(conn, playerData.PlayFabId, 0, expectedAmount);
                        return;
                    } 
                }
                await Task.Delay(pollingInterval);
            }
            print("Transaction was signed lets move on to looking for the validated payload");
            await Task.Delay(10000);
            int tries = 0;
            while(!validatorNodeVerified && savedResponse != null && tries < 3){
                tries ++;
                print($"Polling rippleD try # {tries}");
                var msg = new XummTransmute { code = "4", error = false };
                conn.Send(msg);
                var (validated, validatedAmount) = await ValidateXrplTransaction(savedResponse.response.txid);
                if (validated && validatedAmount == expectedAmount){
                    // Success, credit the account
                    print($"Successfully paid with XUMM account {savedResponse.response.account} to playfab account {decodedID}");
                    var _msg = new XummTransmute { code = "5", error = false };
                    conn.Send(_msg);
                    p.Gold += long.Parse(validatedAmount);
                    //long dkpBalance = long.Parse(p.GetTacticianSheet().DKPBalance);
                    //dkpBalance -= long.Parse(validatedAmount);
                    float dkpBalanceFloat = float.Parse(p.GetTacticianSheet().DKPBalance) - float.Parse(validatedAmount);
                    //p.ServerSendDKPCD(p.GetTacticianSheet().DKPCooldown, p.GetTacticianSheet().XRPBalance, dkpBalance.ToString());
                    //p.TargetWalletAwake();
                    StartCoroutine(NewGoldAmountTRANSMUTE(p, dkpBalanceFloat));
                    return; // Exit the loop
                }
                else
                {
                    // Handle partial payment
                    print($"unpaid because {validatedAmount} was amount");
                }
                await Task.Delay(pollingInterval);
            }
            var _Msg = new XummTransmute { code = "6", error = true };
            conn.Send(_Msg);
        }
        IEnumerator NewGoldAmountTRANSMUTE(ScenePlayer p, float dkpBalance){
            yield return new WaitForSeconds(.5f);
            p.ServerSendDKPCD(p.GetTacticianSheet().DKPCooldown, p.GetTacticianSheet().XRPBalance, dkpBalance.ToString("F2"));
            //p.TargetWalletAwake();
        }
    private async Task<bool> SignedXRPLBlobSubmission(string transactionBlob){
        using (var httpClient = new HttpClient())
        {
            var postData = new
            {
                method = "submit",
                @params = new object[]
                {
                    new
                    {
                        tx_blob = transactionBlob
                    }
                }
            };
            var jsonString = JsonConvert.SerializeObject(postData);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://s1.ripple.com:51234/", httpContent); // Adjust the endpoint URL accordingly
            if (response.IsSuccessStatusCode){
                // Successfully submitted to XRPL
                var responseContent = await response.Content.ReadAsStringAsync();
                print("Success on submitting to xrpl!");
                // Further logic
                return true;
            } else {
                // Handle error
                Debug.LogError("Error submitting transaction blob to XRPL: " + response.StatusCode);
                return false;
            }
        }
    }
    
/*
    private async Task<XrplAccountInfoResponse> FetchNextSequenceAsync(string accountAddress)
{
    using (var httpClient = new HttpClient())
    {
        var requestPayload = new
        {
            method = "account_info",
            @params = new[]
            {
                new
                {
                    account = accountAddress,
                    ledger_index = "current",
                    queue = true
                }
            }
        };

        var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://s1.ripple.com:51234/", content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var xrplAccountInfoResponse = JsonConvert.DeserializeObject<XrplAccountInfoResponse>(responseContent);
            if (xrplAccountInfoResponse.result.status == "success")
            {
                Debug.LogError($"{xrplAccountInfoResponse.result.account_data.Sequence} is our sequence for this account");
                return xrplAccountInfoResponse;
            }
            else
            {
                Debug.LogError("Error fetching account info.");
            }
        }
        else
        {
            Debug.LogError("Error fetching XRPL account info: " + response.StatusCode);
        }
    }
    return null;
}
*/
/*
public static string Base58Decode(byte[] data)
{
    const string alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    BigInteger intData = new BigInteger(1, data); // Initialize as a BigInteger
    
    StringBuilder base58String = new StringBuilder();

    while (intData.CompareTo(BigInteger.Zero) > 0)
    {
        BigInteger[] divmod = intData.DivideAndRemainder(BigInteger.ValueOf(58));
        intData = divmod[0];
        int remainder = divmod[1].IntValue;
        base58String.Insert(0, alphabet[remainder]);
    }

    // Handle leading zeros
    foreach (byte b in data)
    {
        if (b == 0)
        {
            base58String.Insert(0, alphabet[0]);
        }
        else
        {
            break;
        }
    }

    return base58String.ToString();
}
public static byte[] Base58Encode(string base58String)
{
    const string alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    BigInteger intData = new BigInteger("0"); // Initialize as a BigInteger

    // Convert base58String to BigInteger
    for (int i = 0; i < base58String.Length; i++)
    {
        int alphabetIndex = alphabet.IndexOf(base58String[i]);
        if (alphabetIndex == -1)
        {
            throw new ArgumentException("Invalid character in Base58 string");
        }
        intData = intData.Multiply(BigInteger.ValueOf(58)).Add(BigInteger.ValueOf(alphabetIndex));
    }

    List<byte> byteList = new List<byte>();
    while (intData.CompareTo(BigInteger.Zero) > 0)
    {
        BigInteger[] divmod = intData.DivideAndRemainder(BigInteger.ValueOf(256));
        intData = divmod[0];
        byte remainder = (byte)divmod[1].IntValue;
        byteList.Insert(0, remainder);
    }

    return byteList.ToArray();
}
private string GetPublicKeyFromSeed(byte[] seedBytes)
{
    // Decode the base58 seed
    //byte[] seedBytes = Base58Encode(seedBase58);  // Make sure this function returns the byte array correctly
    
    // Create the private key parameter
    BigInteger privateKeyValue = new BigInteger(1, seedBytes);
    X9ECParameters ecp = SecNamedCurves.GetByName("secp256k1");
    ECDomainParameters domainParams = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
    
    // Generate the public key point
    Org.BouncyCastle.Math.EC.ECPoint q = domainParams.G.Multiply(privateKeyValue);
    
    // Convert the point to its byte representation
    byte[] pubKeyBytes = q.GetEncoded();  // This will give you the compressed form of the public key
    
    // Optionally, encode this byte array to base58 or base64 as per your requirements
    string pubKeyBase58 = Base58Decode(pubKeyBytes);  // Make sure this function works as expected
    
    return pubKeyBase58;
}
    private async Task EncryptLedgerTX(string sequence, string address){
        string amount = "10000";
        string destination = "rhHSo1fpGymnF9uB43kwpLxpe3ko6zr1Av";
        // Step 1: Create your transaction in JSON format (simplified)
        //string transactionJson = $"{{\"Account\": \"{address}\", \"Fee\": \"10\", \"Sequence\": {sequence}, \"TransactionType\": \"Payment\"}}";
        string transactionJson = $"{{\"Account\": \"{address}\", \"Amount\": \"{amount}\", \"Destination\": \"{destination}\", \"Fee\": \"10\", \"Sequence\": {sequence}, \"TransactionType\": \"Payment\"}}";
        print($"{transactionJson} was our unsigned json object");
        // Step 2: Serialize the transaction to binary format (simplified, not adhering to XRPL specifics)
        //byte[] serializedTransaction = Encoding.ASCII.GetBytes(transactionJson); //wrong
        byte[] serializedTransaction = Encoding.UTF8.GetBytes(transactionJson);

        // Step 3: Add the signing prefix
        byte[] signingPrefix = new byte[] { 0x53, 0x54, 0x58, 0x00 };
        byte[] dataToSign = new byte[signingPrefix.Length + serializedTransaction.Length];
        Buffer.BlockCopy(signingPrefix, 0, dataToSign, 0, signingPrefix.Length);
        Buffer.BlockCopy(serializedTransaction, 0, dataToSign, signingPrefix.Length, serializedTransaction.Length);
        // Replace this with your actual private key in big integer format
        byte[] encodedBytes = Base58Encode("");
        // Initialize BigInteger with the decoded bytes
        BigInteger privateKeyValue = new BigInteger(1, encodedBytes);
        // Setup EC domain parameters
        X9ECParameters ecp = SecNamedCurves.GetByName("secp256k1");
        ECDomainParameters domainParams = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
        ECPrivateKeyParameters privateKeyParameters = new ECPrivateKeyParameters(privateKeyValue, domainParams);
        // Step 5: Sign the transaction
        ISigner signer = SignerUtilities.GetSigner("ECDSA");
        signer.Init(true, privateKeyParameters);
        signer.BlockUpdate(dataToSign, 0, dataToSign.Length); //wrong 
        byte[] signature = signer.GenerateSignature();
        // Step 6: Add the signature to the transaction and re-serialize (simplified)
        string signatureBase64 = Convert.ToBase64String(signature);
        //string signedTransactionJson = "{\"signature\": \"" + signatureBase64 + "\", \"transaction\": " + transactionJson + "}";
        print($"{signatureBase64} is our signature ");
        // Your transaction JSON object (as a string)
        string your_account = "rPj3zg7vP3c39dELGFSbTdVfagqgzmRDQm";
        string destination_account = "rhHSo1fpGymnF9uB43kwpLxpe3ko6zr1Av";
        string value = "10000"; // Or any dynamic value
        string fee = "10"; // Transaction fee
        //string payloadJson = $"{{\"TransactionType\":\"Payment\",\"Account\":\"{your_account}\",\"Amount\":{{\"currency\":\"DKP\",\"value\":\"{value}\",\"issuer\":\"rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J\"}},\"Destination\":\"{destination_account}\",\"Sequence\":{sequence},\"Fee\":\"{fee}\",\"Signature\":\"{signatureBase64}\"}}";
        string signingPubKey = GetPublicKeyFromSeed(encodedBytes);
        //string payloadJson = $"{{\"TransactionType\":\"Payment\",\"Account\":\"{your_account}\",\"Amount\":\"{value}\",\"Destination\":\"{destination_account}\",\"Sequence\":{sequence},\"Fee\":\"{fee}\",\"SigningPubKey\":\"{signingPubKey}\"\"TxnSignature\":\"{signatureBase64}\"}}";
        string payloadJson = $"{{\"Account\":\"{your_account}\",\"Amount\":\"{value}\",\"Destination\":\"{destination_account}\",\"Fee\":\"{fee}\",\"Sequence\":{sequence},\"TransactionType\":\"Payment\",\"SigningPubKey\":\"{signingPubKey}\",\"TxnSignature\":\"{signatureBase64}\"}}";

        // Serialize the transaction to binary according to XRPL's specifications.
        // This is a simplified placeholder. Actual serialization is more involved.
        //byte[] transactionBlobBytes = Encoding.ASCII.GetBytes(payloadJson);
        byte[] transactionBlobBytes = Encoding.UTF8.GetBytes(payloadJson);
        // Convert the binary data to hexadecimal (this is your tx_blob)
        string transactionBlobHex = BitConverter.ToString(transactionBlobBytes).Replace("-", "");
        print($"{transactionBlobHex} is our new transaction blob hex we generated and this is what gets sent to ledger! HAUDKEN!");
        while(true){
            try{
            //var account_info_XRPL = await SignedXRPLBlobSubmission(transactionBlobHex);
            var account_info_XRPL = await SignedXRPLBlobSubmissionTwo(transactionBlobHex);
            if(account_info_XRPL){
                print($"Successfully submitted and tested");
                break;
            } else {
                print($"FAILED********SUBMISSION TO XRPL");
                break;
            }
            } catch (Exception e){
               print($"An exception occurred while fetching the sequence: {e.Message}");
            }
            await Task.Delay(5000);
        }
    }
    */
        #endif
    
    public override void Awake(){
        LoadNetworkPrefabs();
        base.Awake();
        instance = this;   
    }
    private void LoadNetworkPrefabs(){
        // Clear the list to not have duplicates
        // Load all prefabs in the 'Resources/Prefabs/Monsters' directory
        //prefabs = Resources.LoadAll<GameObject>("Prefabs/Monsters");
        prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Monsters"));
        prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Droppables"));
        foreach (GameObject prefab in prefabs){
            if (prefab.GetComponent<NetworkIdentity>() != null){
                spawnPrefabs.Add(prefab);
            } else {
                Debug.LogWarning($"{prefab.name} does not have a NetworkIdentity and was not added to the list of spawnable prefabs.");
            }
        }
    }
        #if UNITY_SERVER || UNITY_EDITOR

        public override void Start()
        {
            StartPlayFabAPI();
            /*
            var request = new PlayFab.MultiplayerModels.ListMultiplayerServersRequest
            {
                BuildId = "", // Leave it empty initially
                PageSize = 1 // Only request one server, assuming you want the latest build ID
            };
            PlayFabMultiplayerAPI.ListMultiplayerServers(request, result =>
            {
                if (result.MultiplayerServerSummaries.Count > 0)
                {
                    var serverSummary = result.MultiplayerServerSummaries[0];
                    string buildId = serverSummary.VmId;
                    string SessionId = serverSummary.SessionId;

                    // Set the build ID for this game server
                    var updateRequest = new SetTitleDataRequest
                    {
                        Key = "latestBuildId",
                        Value = buildId + ";" + SessionId 
                    };
                    PlayFabServerAPI.SetTitleData(updateRequest, updateResult =>
                    {
                        Debug.Log("latestBuildId updated in title data.");

                        // Proceed with starting the game server
                        this.StartServer();
                    }, updateError =>
                    {
                        Debug.LogError("Failed to update latestBuildId in title data: " + updateError.ErrorMessage);
                    });
                }
                else
                {
                    Debug.LogError("No multiplayer servers found.");
                    return;
                }
            }, error =>
            {
                Debug.LogError("Failed to retrieve multiplayer servers: " + error.ErrorMessage);
            });
            */
            this.StartServer();
        }
        
        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<NoobToPlayer>(CreatePlayerProcess);
            NetworkServer.RegisterHandler<ClientRequestLoadScene>(GetCleanedSceneName);
            NetworkServer.RegisterHandler<Noob>(NoobPlayer);
            NetworkServer.RegisterHandler<PlayerInfo>(OnReceivePlayerInfo);
            NetworkServer.RegisterHandler<XummTransmute>(OnDKPTransmuteRequest);
            NetworkServer.RegisterHandler<XRPLTransmute>(OnGOLDTransmuteRequest);
            AzureLoaded = false;
            FetchPlayFabIngredients();
            StartCoroutine(LoadSubScenes());
            StartCoroutine(WaitFiveSeconds());
            base.OnStartServer();
        }
        public void FetchPlayFabIngredients() {

            var request = new GetTitleDataRequest { Keys = null }; // Fetch all, or specify which keys you want
            PlayFabServerAPI.GetTitleInternalData(request, OnDataReceivedStartTask, OnError);
           

        }
        #endif
        private long counter = 0;
        #if UNITY_SERVER || UNITY_EDITOR
        void OnDataReceivedStartTask(GetTitleDataResult result){
            salt = result.Data["salt"];
            sharedSecret = result.Data["sharedSecret"];
            _DKP_DESTINATION_ADDRESS = result.Data["RegisterCollectionsKey"];
            _DKP_XAPP_FIRST = result.Data["XappPublicKey"];
            _DKP_XAPP_SECOND = result.Data["XappPrivateKey"];
            _GAME_WALLET_ADDRESS = result.Data["GameWalletPublicKey"];
            _GOLD_WALLET_ADDRESS = result.Data["GoldPublicKey"];
            _GOLD_WALLET_SIGN = result.Data["GoldPrivateKey"];
            _Heroku_URL = result.Data["HerokuURL"];
            //StartCoroutine(PostToNodeServer("8F6EE26B13D3248A","rhHSo1fpGymnF9uB43kwpLxpe3ko6zr1Av", 100));

           //Task.Run(async () => await OnDataReceived(result));
        }
        #endif
        #if UNITY_SERVER || UNITY_EDITOR

        private void OnError(PlayFabError error) {
            Debug.LogError("Could not retrieve Title Data: " + error.GenerateErrorReport());
        }
        #endif
        List<int> activeConnectionIds = new List<int>();
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {   
            Debug.Log("Connected client to server, ConnectionId: " + conn.connectionId);
            playerConnections.Add(new PlayerConnection
            {
                ConnectionId = conn.connectionId,
                conn = conn
            });
            conn.Send<PlayerInfo>(new PlayerInfo
            {
                ConnectionId = conn.connectionId
            });
            activeConnectionIds.Add(conn.connectionId);
        }
        IEnumerator WaitFiveSeconds(){
            yield return new WaitForSeconds(5);
            AzureLoaded = true;
        }
        #if UNITY_SERVER || UNITY_EDITOR
        IEnumerator CheckIfPlayerIsConnected(string playerId, NetworkConnectionToClient conn, GetUserDataResult result, PlayerInfo netMsg, string tacticianNamePlayFab, string ticket) {
            string xrplPublic = "Empty";
            string tactBuildString = "Empty";
            bool hasXUMM = false;
            bool DKPTrustLineSet = false;
            if (result.Data != null && result.Data.ContainsKey("PUBLICADDRESS")){ 
                if (result.Data.TryGetValue("PUBLICADDRESS", out UserDataRecord userDataRecord)){
                    xrplPublic = userDataRecord.Value;
                    hasXUMM = true;
                } 
            }
            if (result.Data != null && result.Data.ContainsKey("TACTBUILDSTRING")){ 
                if (result.Data.TryGetValue("TACTBUILDSTRING", out UserDataRecord userDataRecord)){
                    tactBuildString = userDataRecord.Value;
                } 
            }
            if(hasXUMM){
                //check for trustline if not make sure they do this same step on register
            }
            var playerConnection = playerConnections.Find(c => c.ConnectedPlayer != null && c.ConnectedPlayer.PlayerId == playerId && c.ConnectionId != netMsg.ConnectionId);
            if (playerConnection != null) {
                Debug.Log($"Player with ID {playerId} is already connected. Disconnecting existing connection. ConnectionId: {playerConnection.ConnectionId}");
                //processing list for disonncet
                if(ProcessingLogoutList.Contains(playerId)){
                    bool onList = true;
                    Debug.Log($"Player with ID {playerId} is being disconnected waiting");
                    while(onList){
                        if(!ProcessingLogoutList.Contains(playerId)){
                            onList = false;
                            Debug.Log($"Player with ID {playerId} is finally disconnected going to connect this account in 10 seconds");
                        }
                        yield return null;
                    }
                    yield return new WaitForSeconds(10f);
                    Debug.Log($"Player with ID {playerId} is ready to connect now");
                    var _playerConnection = playerConnections.Find(c => c.ConnectionId == netMsg.ConnectionId);
                    _playerConnection.ConnectedPlayer = new ConnectedPlayer(playerId);
                    connectedPlayers.Add(_playerConnection.ConnectedPlayer);
                    PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(connectedPlayers);
                    if (result.Data != null && result.Data.ContainsKey("NewAccount89")){ 
                        //Generate the Xumm QRCode
                        bool newPlayer = true;
                        PlayerInfo playerinfo = new PlayerInfo { 
                            newPlayer = newPlayer,
                            PlayFabId = playerId,
                            SessionTicket = ticket,
                            SavedNode = "TOWNOFARUDINE",
                            CurrentScene = TOWNOFARUDINE,
                            PlayerName = tacticianNamePlayFab,
                            XRPLPUBLIC = xrplPublic
                        };
                        bool registered = false;
                        if(xrplPublic != "Empty"){
                            registered = true;
                        }
                        conn.authenticationData = playerinfo;
                        base.OnServerConnect(conn);
                        if(!registered){
                            RegisterAccount(conn, playerId, 0, null);
                        } else {
                            conn.Send<Noob>(new Noob {
                                tactician = tacticianNamePlayFab,
                                finished = false,
                                Address = xrplPublic
                            });
                        }
                    }
                    if(result.Data != null && !result.Data.ContainsKey("NewAccount89")){
                        StartCoroutine(GetPlayerData(playerId, conn, tacticianNamePlayFab, ticket, netMsg, xrplPublic, tactBuildString));
                    }
                } else {
                    Debug.Log($"Player with ID {playerId} is being forced to disconnect");
                    NetworkIdentity iden = playerConnection.conn.identity;
                    GameObject idenGO = null;//playerConnection.conn.GetComponent<NetworkIdentity>();
                    ScenePlayer removeplayer = null;//playerConnection.conn.identity.gameObject.GetComponent<ScenePlayer>();
                    bool HasIdentity = false;
                    bool HasObject = false;
                    bool HasScenePlayer = false;
                    if(iden != null){
                        idenGO = playerConnection.conn.identity.gameObject;
                        if(idenGO != null){
                            removeplayer = playerConnection.conn.identity.gameObject.GetComponent<ScenePlayer>();
                        }
                    }

                    if(removeplayer != null){
                        removeplayer.TargetCloseGameCompletely();
                        yield return new WaitForSeconds(10f);
                        if(ProcessingLogoutList.Contains(playerId)){
                            bool onList = true;
                            while(onList){
                                if(!ProcessingLogoutList.Contains(playerId)){
                                    onList = false;
                                }
                                yield return null;
                            }
                            Debug.Log($"Player with ID {playerId} is finally disconnected going to connect this account in 10 seconds");
                            yield return new WaitForSeconds(10f);
                        }
                    } else {
                        if(activeConnectionIds.Contains(playerConnection.conn.connectionId)){
                            activeConnectionIds.Remove(playerConnection.conn.connectionId);
                        }
                        base.OnServerDisconnect(playerConnection.conn);
                        if(connectedPlayers.Contains(playerConnection.ConnectedPlayer)){
                            connectedPlayers.Remove(playerConnection.ConnectedPlayer);
                        }
                        if(playerConnections.Contains(playerConnection)){
                            playerConnections.Remove(playerConnection);
                        }
                        
                    }
                    Debug.Log($"Player with ID {playerId} is ready to connect now");
                    var _playerConnection = playerConnections.Find(c => c.ConnectionId == netMsg.ConnectionId);
                    _playerConnection.ConnectedPlayer = new ConnectedPlayer(playerId);
                    connectedPlayers.Add(_playerConnection.ConnectedPlayer);
                    PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(connectedPlayers);
                    if (result.Data != null && result.Data.ContainsKey("NewAccount89")){ 
                        //Generate the Xumm QRCode
                        bool newPlayer = true;
                        PlayerInfo playerinfo = new PlayerInfo { 
                            newPlayer = newPlayer,
                            PlayFabId = playerId,
                            SessionTicket = ticket,
                            SavedNode = "TOWNOFARUDINE",
                            PlayerName = tacticianNamePlayFab,
                            CurrentScene = TOWNOFARUDINE,
                            XRPLPUBLIC = xrplPublic
                        };
                        bool registered = false;
                        if(xrplPublic != "Empty"){
                            registered = true;
                        }
                        conn.authenticationData = playerinfo;
                        base.OnServerConnect(conn);
                        if(!registered){
                            RegisterAccount(conn, playerId, 0, null);
                        } else {
                            conn.Send<Noob>(new Noob {
                                tactician = tacticianNamePlayFab,
                                finished = false,
                                Address = xrplPublic
                            });
                        }
                    }
                    if(result.Data != null && !result.Data.ContainsKey("NewAccount89")){
                        StartCoroutine(GetPlayerData(playerId, conn, tacticianNamePlayFab, ticket, netMsg, xrplPublic, tactBuildString));
                    }
                }
                //begin the reconnect process
            } else {
                if(ProcessingLogoutList.Contains(playerId)){
                    bool onList = true;
                    while(onList){
                        if(!ProcessingLogoutList.Contains(playerId)){
                            onList = false;
                        }
                        yield return null;
                    }
                    yield return new WaitForSeconds(10f);
                }
                Debug.Log($"Player with ID {playerId} is ready to connect now");
                var _playerConnection = playerConnections.Find(c => c.ConnectionId == netMsg.ConnectionId);
                _playerConnection.ConnectedPlayer = new ConnectedPlayer(playerId);
                connectedPlayers.Add(_playerConnection.ConnectedPlayer);
                PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(connectedPlayers);
                if (result.Data != null && result.Data.ContainsKey("NewAccount89")){ 
                    //Generate the Xumm QRCode
                    bool newPlayer = true;
                    PlayerInfo playerinfo = new PlayerInfo { 
                        newPlayer = newPlayer,
                        PlayFabId = playerId,
                        SessionTicket = ticket,
                        PlayerName = tacticianNamePlayFab,
                        SavedNode = "TOWNOFARUDINE",
                        CurrentScene = TOWNOFARUDINE,
                        XRPLPUBLIC = xrplPublic
                    };
                    bool registered = false;
                    if(xrplPublic != "Empty"){
                        registered = true;
                    }
                    conn.authenticationData = playerinfo;
                    base.OnServerConnect(conn);
                    if(!registered){
                        RegisterAccount(conn, playerId, 0, null);
                    } else {
                        conn.Send<Noob>(new Noob {
                            tactician = tacticianNamePlayFab,
                            finished = false,
                            Address = xrplPublic
                        });
                    }
                }
                if(result.Data != null && !result.Data.ContainsKey("NewAccount89")){
                    StartCoroutine(GetPlayerData(playerId, conn, tacticianNamePlayFab, ticket, netMsg, xrplPublic, tactBuildString));
                }
            }
            //StartCoroutine(DelayXRPLRegistration(conn, playerId));
        }
#endif
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {   
            StartCoroutine(AddingPlayerToServer(conn));   
        }
        IEnumerator AddingPlayerToServer(NetworkConnectionToClient conn){
            // start energy recharge here for character
            PlayerInfo playerData = (PlayerInfo)conn.authenticationData;
            if (playerData.newPlayer == true){                
                GameObject Playernoob = Instantiate(playerPrefab);
                Playernoob.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
                NetworkServer.AddPlayerForConnection(conn, Playernoob);
                //SceneManager.MoveGameObjectToScene(conn.identity.gameObject, TownScene);
                float newX = 2.5f;
                float newY = 0.0f;
                float newEnergy = 1500f;
                playerData.Energy = newEnergy;
                playerData.CurrentScene = TOWNOFARUDINE;
                Vector3 newPlayerPos = new Vector3 (newX, newY, 0f);
                Playernoob.transform.position = newPlayerPos;
                conn.authenticationData = playerData;
                VerifyNewPlayerData(conn, playerData);
                yield break;
            }
            //needed this
            conn.authenticationData = playerData;
            LoginInventoryCheck(conn, playerData);
        }//Assets/Resources/Sprites/Characters/Player0.png
        //inserting NFT code for spawning in the items for demo.
        #if UNITY_SERVER || UNITY_EDITOR
       
    public override void OnServerDisconnect(NetworkConnectionToClient conn){
        if(activeConnectionIds.Contains(conn.connectionId)){
            activeConnectionIds.Remove(conn.connectionId);
        }
        if (conn?.identity?.gameObject == null) {
            var playerConnection = playerConnections.Find(c => c.ConnectionId == conn.connectionId);
            if(playerConnection != null){
                if(connectedPlayers.Contains(playerConnection.ConnectedPlayer)){
                    connectedPlayers.Remove(playerConnection.ConnectedPlayer);
                }
                if(playerConnections.Contains(playerConnection)){
                    playerConnections.Remove(playerConnection);
                }
                PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(connectedPlayers);
            }
            base.OnServerDisconnect(conn);
            //StartCoroutine(ServerDisconnectPlayerPause(conn, null));
            Debug.LogWarning("Connection or identity or game object is null.");
            return;
        }
        ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>();
        // Check if the ScenePlayer component exists
        if (p == null) {
            var playerConnection = playerConnections.Find(c => c.ConnectionId == conn.connectionId);
            if(playerConnection != null){
                if(connectedPlayers.Contains(playerConnection.ConnectedPlayer)){
                    connectedPlayers.Remove(playerConnection.ConnectedPlayer);
                }
                if(playerConnections.Contains(playerConnection)){
                    playerConnections.Remove(playerConnection);
                }
                PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(connectedPlayers);
            }
            base.OnServerDisconnect(conn);
            Debug.LogWarning("ScenePlayer component is missing.");
            return;
        }
        DateTime LogoutTime = DateTime.Now;
        string time = LogoutTime.ToString();
        string scene = p.currentScene;
        // Check if authentication data exists and is of type PlayerInfo
        if (!(conn.authenticationData is PlayerInfo playerData)) {
            Debug.LogWarning("Authentication data is missing or not of type PlayerInfo.");
            return;
        }
        DisconnectedList.Add(playerData.PlayFabId);

        StartCoroutine(ProcessingDisconnecting(conn, playerData, time, p.Energy, scene, p.currentNode));   
        
    }

    private IEnumerator ServerDisconnectPlayerPause(NetworkConnectionToClient conn, string ID){
        yield return new WaitForSeconds(.1f);
        var playerConnection = playerConnections.Find(c => c.ConnectionId == conn.connectionId);
        if(playerConnection != null){
            if(connectedPlayers.Contains(playerConnection.ConnectedPlayer)){
                connectedPlayers.Remove(playerConnection.ConnectedPlayer);
            }
            if(playerConnections.Contains(playerConnection)){
                playerConnections.Remove(playerConnection);
            }
        }
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(connectedPlayers);
        if(ID != null){
            if(DisconnectedList.Contains(ID)){
                DisconnectedList.Remove(ID);
            }
            if(ProcessingLogoutList.Contains(ID)){
                ProcessingLogoutList.Remove(ID);
            }
        }
        
        base.OnServerDisconnect(conn);
        if (playerConnections.Count == 0) {
            StartCoroutine(Shutdown());
        }
    }
        private IEnumerator ServerDisconnectPlayer(NetworkConnectionToClient conn, string ID){
            ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>();
            if(p.inMatch){
                MatchMaker.instance.PlayerDisconnected (p, p.matchID);
            } else {
                if(p.inLobby){
                    MatchMaker.instance.PlayerDisconnectedFromLobby (p, p.matchID);
                }
            }
            yield return new WaitForSeconds(1f);

            StartCoroutine(ServerDisconnectPlayerPause(conn, ID));
        }
        private IEnumerator Shutdown()
        {
            
            yield return new WaitForSeconds(.5f);
            if (playerConnections.Count == 0)
            {
                //print("No players left closing server");
                //Application.Quit();
            }
        }
        #endif

        void StartPlayFabAPI()
        {
#if UNITY_SERVER || UNITY_EDITOR
            
            PlayFabMultiplayerAgentAPI.Start();
            StartCoroutine(ReadyForPlayers());
            #endif
        }
        #if UNITY_SERVER || UNITY_EDITOR

                IEnumerator ReadyForPlayers()
                {
                    yield return new WaitForSeconds(.5f);
                    PlayFabMultiplayerAgentAPI.ReadyForPlayers();
                    
                }
            #endif
              
        void OnEnable(){
            ScenePlayer.BuildStackableItem.AddListener(BuildNewStack);
            TurnManager.ProcessMainChest.AddListener(GetMainChestDropTable);
            TurnManager.ProcessMiniChest.AddListener(GetMiniChestDropTable);
            TurnManager.ProcessArmorRack.AddListener(GetArmorRackDropTable);
            TurnManager.ProcessWeaponRack.AddListener(GetWeaponRackDropTable);
            VoteManager.BuildItemServer.AddListener(BuildItemDropped);
            MatchMaker.EnterNodeCost.AddListener(AuthorizeEnergyUpdate);
            MatchMaker.SendCuratorTMToPlayer.AddListener(SendTurnManagerToPlayerCharacter);
            //MatchMaker.MakeCharacters.AddListener(SendPCToMatch);
            MatchMaker.MoveChatNodeOVM.AddListener(ChatManagerNodeOVMTransport);
            MatchMaker.CLEARTHEMATCH.AddListener(ClearMatch);
            MatchMaker.moveTurnManager.AddListener(SendTurnManagerToMatch);
            MatchMaker.moveChatManagerNode.AddListener(SendChatManagerNodeToMatch);
            MatchMaker.moveMob.AddListener(SendMobToMatch);
            //Finished Match
            ScenePlayer.PermissionToFinish.AddListener(FinishingMatch);
            //Drop calls
            TurnManager.RetrievingDroppedItem.AddListener(GetDropTable);
            TurnManager.ProcessEXPandCP.AddListener(GiveExpClassPoints);
            //Spell manipulation
            ScenePlayer.SpellPurchase.AddListener(PlayerRequestedLearnSpell);
            ScenePlayer.SpellChange.AddListener(PlayerRequestedSpellChange);
            //Movement request
            //ScenePlayer.FinalRequest.AddListener(AuthorizeMovementRequest);
            ScenePlayer.ResetOVM.AddListener(UpdatePlayerOVMPosition);
            //BuildCharacter request
            ScenePlayer.ServerCharacterBuildRequest.AddListener(CharacterCreation);
            //Party Requests
            ScenePlayer.SendParty.AddListener(SavePartyListAdding);
            ScenePlayer.PartyRemoval.AddListener(SavePartyListRemoving);
            ScenePlayer.ServerTransmitTX.AddListener(PurchasedVendorItem);
            //StashToTactician
            ScenePlayer.StashToTactInv.AddListener(AuthenticateThenSavePF);//completed
            ScenePlayer.StashToTactSafetyBelt.AddListener(AuthenticateThenSavePF);//completed
            ScenePlayer.StashToTactEquipped.AddListener(AuthenticateThenSavePFEquip);//completed
            //StashToCharacter
            ScenePlayer.StashToCharInv.AddListener(AuthenticateThenSavePFCharacters);//completed
            ScenePlayer.StashToCharEquip.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            //TacticianToStash
            ScenePlayer.TactInvToStash.AddListener(AuthenticateThenSavePF);//completed
            ScenePlayer.TactEquipToStash.AddListener(AuthenticateThenSavePF);//completed
            ScenePlayer.TactBeltToStash.AddListener(AuthenticateThenSavePF);//completed
            //TacticianToTactician
            ScenePlayer.TactEquipToTactInv.AddListener(AuthenticateThenSavePF);//completed
            ScenePlayer.TactEquipToTactBelt.AddListener(AuthenticateThenSavePF);//completed
            ScenePlayer.TactInvToTactEquip.AddListener(AuthenticateThenSavePFEquip);//completed
            ScenePlayer.TactInvToTactBelt.AddListener(AuthenticateThenSavePF);//completed
            ScenePlayer.TactBeltToTactEquip.AddListener(AuthenticateThenSavePFEquip);//completed
            ScenePlayer.TactBeltToTactInv.AddListener(AuthenticateThenSavePF);//completed
            //TacticianToChars
            ScenePlayer.TactInvToCharInv.AddListener(AuthenticateThenSavePFCharacters);//completed
            ScenePlayer.TactInvToCharEquip.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.TactSafetyBeltToCharEquip.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.TactSafetyBeltToCharInv.AddListener(AuthenticateThenSavePFCharacters);//completed 
            ScenePlayer.TactEquipToCharInv.AddListener(AuthenticateThenSavePFCharacters);//completed
            //CharacterToTactician
            ScenePlayer.CharInvToTactInv.AddListener(AuthenticateThenSavePFCharacters);//completed
            ScenePlayer.CharInvToTactBelt.AddListener(AuthenticateThenSavePFCharacters);//completed
            ScenePlayer.CharInvToTactEquip.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharEquipToTactInv.AddListener(AuthenticateThenSavePFCharacters);//completed
            ScenePlayer.CharEquipToTactBelt.AddListener(AuthenticateThenSavePFCharacters);//completed
            //CharacterToCharacter
            //These require a swap to user then back to the character
            ScenePlayer.CharInvToCharInv.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharInvToCharEquip.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharEquipToCharInv.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharEquipToCharEquip.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharEquipToEquipSame.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharEquipToInvSame.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharInvToEquipSame.AddListener(AuthenticateThenSavePFCharactersEquip);//completed
            ScenePlayer.CharOneUnequipToCharEquip.AddListener(AuthenticateThenSavePFCharactersSwap);
            ScenePlayer.CharTwoUnequipToCharEquip.AddListener(AuthenticateThenSavePFCharactersSwap);
            ScenePlayer.CharUnequipTactToCharEquip.AddListener(AuthenticateDropBox);
            //CharacterToStash
            ScenePlayer.CharInvToStash.AddListener(AuthenticateThenSavePFCharacters);//completed
            ScenePlayer.CharEquipToStash.AddListener(AuthenticateThenSavePFCharacters);//completed
            ScenePlayer.OnPlayerDataUpdateRequest.AddListener(UpdatePlayerData);
            ScenePlayer.LevelUpEnded.AddListener(LevelUpEnding);
            ScenePlayer.LevelUpStarted.AddListener(LevelUpStarting);
            ScenePlayer.StackingItem.AddListener(StackingItem);
            ScenePlayer.ServerDestroyItem.AddListener(RemoveThisItem);
            ScenePlayer.HealPartyServer.AddListener(ServerINNRoomRest);

            ScenePlayer.ResCharacter.AddListener(ResCharacterServer);
            ScenePlayer.LogoutPlayer.AddListener(OnServerDisconnect);
            MovingObject.TakeDamageCharacter.AddListener(CharacterTakingDamage);
            MovingObject.DeathCharacter.AddListener(CharacterDied);
            PlayerCharacter.SaveCharacter.AddListener(SaveGame);
            PlayerCharacter.TrapMP.AddListener(CharacterCastedSpell);
            TurnManager.HealingCharacter.AddListener(CharacterHealed);
            TurnManager.SpendingMP.AddListener(CharacterCastedSpell);
            TurnManager.FullWipe.AddListener(WipeoutMatch);
	//public static event Action<NetworkConnectionToClient, int, string> SpendingMP;
            
        }
        void OnDisable(){
            
            MatchMaker.CLEARTHEMATCH.RemoveListener(ClearMatch);
            MatchMaker.moveTurnManager.RemoveListener(SendTurnManagerToMatch);
            MatchMaker.moveMob.RemoveListener(SendMobToMatch);
            //ScenePlayer.FinalRequest.RemoveListener(AuthorizeMovementRequest);
        }
        
        void UpdatePlayerData(NetworkConnectionToClient connectionToClient, string energy, string lastScene, string lastNode) 
        {
            #if UNITY_SERVER || UNITY_EDITOR
            string playFabId = ((PlayerInfo)connectionToClient.authenticationData).PlayFabId;

            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = playFabId,
                Data = new Dictionary<string, string>
                {
                    {"energy", energy},
                    {"LastScene", lastScene},
                    {"savedNode", lastNode}
                }
            }, result =>
            {
                Debug.Log("Successfully updated player data for player " + playFabId);
            }, error =>
            {
                Debug.LogError("Failed to update player data: " + error.ErrorMessage);
            });
            #endif

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
        void LevelUpStarting(NetworkConnectionToClient connectionToClient, string timeStamp, string charID) 
{
    //print("Starting LevelUP Start");

    float charEXP = 0;
    int _level = 0;
    string _CORE = string.Empty;

    ScenePlayer sPlayer = connectionToClient.identity.gameObject.GetComponent<ScenePlayer>();

    // Get necessary character data
    foreach(var sheet in sPlayer.GetInformationSheets())
    {
        if(sheet.CharacterID == charID)
        {
            foreach(var stat in sheet.CharStatData)
            {
                switch(stat.Key)
                {
                    case "LVL":
                        _level = int.Parse(stat.Value);
                        break;
                    case "EXP":
                        charEXP = float.Parse(stat.Value);
                        break;
                    case "CORE":
                        _CORE = stat.Value;
                        break;
                }
            }
        }
    }

    PlayerInfo playerData = ((PlayerInfo)connectionToClient.authenticationData);

    (int ExpCost, int EnergyCost, float TimeCost, int GoldCost) = GetCharacterLevelUp(_level, _CORE);
    
    // Validate if player has enough resources to level up
    if(sPlayer.Energy < EnergyCost || sPlayer.Gold < GoldCost || charEXP < ExpCost)
        return;

    // Deduct the resources
    sPlayer.Energy -= EnergyCost;
    //print($"Paid {EnergyCost}");
    sPlayer.Gold -= (long)GoldCost;

    int amount = GoldCost;
    SubtractVirtualCurrency(playerData, amount);

    UpdatePlayerEnergy(sPlayer, playerData);
    
    // Deduct EXP cost and start leveling process
    charEXP -= ExpCost;
    StartLevelingProcess(charEXP, playerData, charID, timeStamp, sPlayer);
}

private void SubtractVirtualCurrency(PlayerInfo playerData, int amount)
{
    #if UNITY_SERVER || UNITY_EDITOR

    PlayFabServerAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest
    {
        PlayFabId = playerData.PlayFabId,
        Amount = amount,
        VirtualCurrency = "DK"
    }, result =>
    {
        //print($"Paid {amount}");
    }, error =>
    {
        Debug.Log(error.ErrorMessage);
    });
    #endif
}

private void UpdatePlayerEnergy(ScenePlayer sPlayer, PlayerInfo playerData)
{
    #if UNITY_SERVER || UNITY_EDITOR
    PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
    {
        PlayFabId = playerData.PlayFabId,
        Data = new Dictionary<string, string>
        {
            {"energy", sPlayer.Energy.ToString()}
        }
    }, result => {}, 
    error =>
    {
        Debug.LogError("Failed to update player data: " + error.ErrorMessage);
    });
    #endif
}

private void StartLevelingProcess(float charEXP, PlayerInfo playerData, string charID, string timeStamp, ScenePlayer sPlayer)
{
    #if UNITY_SERVER || UNITY_EDITOR
    PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
    {
        PlayFabId = playerData.PlayFabId,
        CharacterId = charID,
        Data = new Dictionary<string, string>
        {
            {"LEVELING", timeStamp},
            {"EXP", charEXP.ToString()}
        }
    }, result =>
    {
        CharacterStatListItem Leveling = new CharacterStatListItem
        {
            Key = "LEVELING",
            Value = timeStamp
        };
        CharacterStatListItem Exp = new CharacterStatListItem
        {
            Key = "EXP",
            Value = charEXP.ToString()
        };
        sPlayer.GetCharacterUpdateLVLINGEXP(charID, Leveling, Exp);
        Debug.Log("Level up in progress");
    }, error =>
    {
        Debug.Log(error.ErrorMessage);
        Debug.Log(error.ErrorDetails);
        Debug.Log(error.Error);
    });
    #endif
}

        void LevelUpEnding(NetworkConnectionToClient connectionToClient, string timeStamp, string charID)
{
    //print("Starting LevelUP Ending");

    int _level = 0;
    string _CORE = string.Empty;
    DateTime serverTimeStamp = DateTime.Now;
    DateTime endingTimeStamp = DateTime.Parse(timeStamp);

    PlayerInfo playerData = ((PlayerInfo)connectionToClient.authenticationData);
    ScenePlayer sPlayer = connectionToClient.identity.gameObject.GetComponent<ScenePlayer>();

    // Get necessary character data
    foreach(var sheet in sPlayer.GetInformationSheets())
    {
        if(sheet.CharacterID == charID)
        {
            foreach(var stat in sheet.CharStatData)
            {
                switch(stat.Key)
                {
                    case "LVL":
                        _level = int.Parse(stat.Value);
                        break;
                    case "LEVELING":
                        serverTimeStamp = DateTime.Parse(stat.Value);
                        break;
                    case "CORE":
                        _CORE = stat.Value;
                        break;
                }
            }
        }
    }

    (int ExpCost, int EnergyCost, float TimeCost, int GoldCost) = GetCharacterLevelUp(_level, _CORE);

    TimeSpan timeDifference = endingTimeStamp - serverTimeStamp;
    TimeSpan timeCostSpan = TimeSpan.FromSeconds(TimeCost);

    // Validate if leveling time is up
    if(timeDifference < timeCostSpan)
    {
        Debug.Log("Error on level up - Server side");
        return;
    }

    // Level up
    _level++;
    
    UpdateCharacterData(_level, playerData, charID, sPlayer);
}

private void UpdateCharacterData(int _level, PlayerInfo playerData, string charID, ScenePlayer sPlayer)
{
    #if UNITY_SERVER || UNITY_EDITOR
    CharacterStatListItem LEVEL = new CharacterStatListItem
    {
        Key = "LVL",
        Value = _level.ToString()
    };

    PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
    {
        PlayFabId = playerData.PlayFabId,
        CharacterId = charID,
        Data = new Dictionary<string, string>
        {
            {"LEVELING", null},
            {"LVL", _level.ToString()}
        }
    }, result =>
    {
        sPlayer.GetCharacterUpdateLVL(charID, LEVEL, "LEVELING");
    }, error =>
    {
        Debug.Log(error.ErrorMessage); 
        Debug.Log(error.ErrorDetails);
        Debug.Log(error.Error);
    });
    #endif
}

        //Login Process
        
        void OnReceivePlayerInfo(NetworkConnectionToClient nconn, PlayerInfo netMsg)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = netMsg.SessionTicket
            }, result =>
            {
                /*
                PlayFabServerAPI.GetPlayerProfile(new GetPlayerProfileRequest
                {
                    PlayFabId = result.UserInfo.PlayFabId,
                    ProfileConstraints = new PlayerProfileViewConstraints(){
                        ShowContactEmailAddresses = true
                    }
                }, accountInfoResult =>
                {
                    ContactEmailInfoModel contactEmail = null;
                    List<ContactEmailInfoModel> contactEmails = new List<ContactEmailInfoModel>();
                    if(accountInfoResult.PlayerProfile.ContactEmailAddresses != null){
                            print($"not null for the contact emails");
                        contactEmails = accountInfoResult.PlayerProfile.ContactEmailAddresses;
                    } else {
                            print($"we are def null for the contact emails  not good");
                    }
                    if(contactEmails.Count > 0){
                        foreach(var email in contactEmails){
                            contactEmail = email;
                        }
                        EmailVerificationStatus? emailStatus = contactEmail.VerificationStatus;

                        // Check if the email is confirmed
                        if (emailStatus == EmailVerificationStatus.Confirmed)
                        {
                            // Proceed to CheckDisplayName
                            print($"confirmed on {contactEmail.EmailAddress}");
                            CheckDisplayName(result.UserInfo.PlayFabId, nconn, netMsg.SessionTicket, netMsg);
                            return;
                        }
                        else
                        {
                            print($"not confirmed on {contactEmail.EmailAddress}");
                            OnServerDisconnect(nconn);
                            // Handle unconfirmed or pending email verification status
                            // You might want to disconnect the user or send them a warning message, etc.
                        }
                    }
                    print($"not confirmed on {accountInfoResult.PlayerProfile.DisplayName}");
                    OnServerDisconnect(nconn);
                }, accountInfoError =>
                {
                    // Handle any errors in fetching the account info
                    Debug.Log(accountInfoError.ErrorMessage);
                });
                */
                CheckDisplayName(result.UserInfo.PlayFabId, nconn, netMsg.SessionTicket, netMsg);
                //StartCoroutine(CheckPlayerData(result.UserInfo.PlayFabId, nconn, netMsg.SessionTicket, netMsg, result.UserInfo.TitleInfo.DisplayName));
                
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
       
        void CheckDisplayName(string playFabID, NetworkConnectionToClient nconn, string ticket, PlayerInfo netMsg){
            #if UNITY_SERVER || UNITY_EDITOR
            
            PlayFabServerAPI.GetUserAccountInfo(new GetUserAccountInfoRequest
            {
                PlayFabId = playFabID
            }, result =>
            {
                string display = result.UserInfo.TitleInfo.DisplayName;
                if(string.IsNullOrEmpty(display)){
                    display = "NOTACTICIANNAMEPHASE";
                }
                StartCoroutine(CheckPlayerData(playFabID, nconn, ticket, netMsg, result.UserInfo.TitleInfo.DisplayName));

            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        IEnumerator CheckPlayerData(string playFabID, NetworkConnectionToClient nconn, string ticket, PlayerInfo netMsg, string tacticianNamePlayFab){
            bool UserData = false;
            #if UNITY_SERVER || UNITY_EDITOR

            PlayFabServerAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = playFabID
            }, result =>
            {
                print($"Starting Check if player is connected with CheckIfPlayerIsConnected");
                StartCoroutine(CheckIfPlayerIsConnected(playFabID, nconn, result, netMsg, tacticianNamePlayFab, ticket));
                /*
                var playerConnection = playerConnections.Find(c => c.ConnectionId == netMsg.ConnectionId);
                playerConnection.ConnectedPlayer = new ConnectedPlayer(playFabID);
                connectedPlayers.Add(playerConnection.ConnectedPlayer);
                PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(connectedPlayers);
                if (result.Data != null && result.Data.ContainsKey("NewAccount89"))
                    { 
                        //Generate the Xumm QRCode
                        bool newPlayer = true;
                        PlayerInfo playerinfo = new PlayerInfo { 
                            newPlayer = newPlayer,
                            PlayerName = tacticianNamePlayFab,
                            PlayFabId = playFabID,
                            SessionTicket = ticket,
                            SavedNode = "TOWNOFARUDINE",
                            CurrentScene = TOWNOFARUDINE
                        };
                        nconn.authenticationData = playerinfo;
                        base.OnServerConnect(nconn);
                        nconn.Send<Noob>(new Noob
                        {
                            finished = false,
                            tactician = tacticianNamePlayFab,
                        });
                    }
                    if(result.Data != null && !result.Data.ContainsKey("NewAccount89"))
                    {
                        GetPlayerData(playFabID, nconn, tacticianNamePlayFab, ticket, netMsg);
                    }
                */
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            #endif
            
            while(!UserData){
                yield return new WaitForSeconds(.1f);
            }
            
        }
        //New Account login
        void NoobPlayer(NetworkConnectionToClient nconn, Noob clientMsg){
            OnServerAddPlayer(nconn);
        }
        public (int, int) GetDate(string input){
    StringBuilder dayString = new StringBuilder();
        StringBuilder monthString = new StringBuilder();
        
        foreach (char c in input)
        {
            if (Char.IsDigit(c))
            {
                dayString.Append(c);
            }
            else
            {
                monthString.Append(c);
            }
        }

        int selectedDay = int.Parse(dayString.ToString());

        // Convert month string to month number
        int selectedMonth = 0;
        switch (monthString.ToString())
        {
            case "January":
                selectedMonth = 1;
                break;
            case "February":
                selectedMonth = 2;
                break;
            case "March":
                selectedMonth = 3;
                break;
            case "April":
                selectedMonth = 4;
                break;
            case "May":
                selectedMonth = 5;
                break;
            case "June":
                selectedMonth = 6;
                break;
            case "July":
                selectedMonth = 7;
                break;
            case "August":
                selectedMonth = 8;
                break;
            case "September":
                selectedMonth = 9;
                break;
            case "October":
                selectedMonth = 10;
                break;
            case "November":
                selectedMonth = 11;
                break;
            case "December":
                selectedMonth = 12;
                break;
            default:
                Console.WriteLine("Invalid month");
                return (selectedDay, selectedMonth);
        }
        return (selectedDay, selectedMonth);

}
            #if UNITY_SERVER || UNITY_EDITOR

        void CreatePlayerProcess(NetworkConnectionToClient nconn, NoobToPlayer clientMsg){
            PlayerInfo playerinfo = (PlayerInfo)nconn.authenticationData;
            int GiantRep = 1000;
            int DragonRep = 1000;
            int LizardRep = 3500;
            int OrcRep = 3500;
            int FaerieRep = 4500;
            int ElvesRep = 4500;
            int DwarvesRep = 4500;
            int GnomesRep = 4500;
            int Arcana = 0;
            int Str = 0;
            int Fort = 0;
            int Agi = 0;
            string type = "";
            string zodiacDescription = string.Empty;
            (int selectedDay, int selectedMonth) = GetDate(clientMsg.BirthDate);
            string zodiacSign = "";
            if ((selectedMonth == 3 && selectedDay >= 21) || (selectedMonth == 4 && selectedDay <= 19))
                zodiacSign = "Aries";
            else if ((selectedMonth == 4 && selectedDay >= 20) || (selectedMonth == 5 && selectedDay <= 20))
                zodiacSign = "Taurus";
            else if ((selectedMonth == 5 && selectedDay >= 21) || (selectedMonth == 6 && selectedDay <= 21))
                zodiacSign = "Gemini";
            else if ((selectedMonth == 6 && selectedDay >= 22) || (selectedMonth == 7 && selectedDay <= 22))
                zodiacSign = "Cancer";
            else if ((selectedMonth == 7 && selectedDay >= 23) || (selectedMonth == 8 && selectedDay <= 22))
                zodiacSign = "Leo";
            else if ((selectedMonth == 8 && selectedDay >= 23) || (selectedMonth == 9 && selectedDay <= 22))
                zodiacSign = "Virgo";
            else if ((selectedMonth == 9 && selectedDay >= 23) || (selectedMonth == 10 && selectedDay <= 23))
                zodiacSign = "Libra";
            else if ((selectedMonth == 10 && selectedDay >= 24) || (selectedMonth == 11 && selectedDay <= 21))
                zodiacSign = "Scorpio";
            else if ((selectedMonth == 11 && selectedDay >= 22) || (selectedMonth == 12 && selectedDay <= 21))
                zodiacSign = "Sagittarius";
            else if ((selectedMonth == 12 && selectedDay >= 22) || (selectedMonth == 1 && selectedDay <= 19))
                zodiacSign = "Capricorn";
            else if ((selectedMonth == 1 && selectedDay >= 20) || (selectedMonth == 2 && selectedDay <= 18))
                zodiacSign = "Aquarius";
            else if ((selectedMonth == 2 && selectedDay >= 19) || (selectedMonth == 3 && selectedDay <= 20))
            zodiacSign = "Pisces";
            if (zodiacSign == "Aries") {
                GiantRep += 0; DragonRep += 0; LizardRep += 600; OrcRep += -600; FaerieRep += -600; ElvesRep += -600; DwarvesRep += -600; GnomesRep += -600;
                Arcana = 2; Str = 1; Fort = 1; Agi = 2; type = "Fire"; zodiacDescription = "Courageous, determined, confident, enthusiastic, optimistic, and honest the Aries are worshipped by the Lizard folk.";
            } else if (zodiacSign == "Taurus") {
                GiantRep += 0; DragonRep += 0; LizardRep += -600; OrcRep += 600; FaerieRep += -600; ElvesRep += -600; DwarvesRep += -600; GnomesRep += -600;
                Arcana = 1; Str = 2; Fort = 1; Agi = 2; type = "Earth"; zodiacDescription = "Reliable, patient, practical, devoted, responsible, and stable the Taurus are worshipped by the Orcs";
            } else if (zodiacSign == "Gemini") {
                GiantRep += 0; DragonRep += 0; LizardRep += 0; OrcRep += 0; FaerieRep += 1000; ElvesRep += -250; DwarvesRep += -250; GnomesRep += -250;
                Arcana = 3; Str = 1; Fort = 1; Agi = 1; type = "Air"; zodiacDescription = "Gentle, affectionate, curious, adaptable, with the ability to learn quickly Geimini are worshipped by the Faeries";
            } else if (zodiacSign == "Cancer") {
                GiantRep += 0; DragonRep += 0; LizardRep += 0; OrcRep += 0; FaerieRep += -250; ElvesRep += 1000; DwarvesRep += -250; GnomesRep += -250;
                Arcana = 1; Str = 3; Fort = 2; Agi = 1;type = "Water"; zodiacDescription = "Tenacious, highly imaginative, loyal, emotional, sympathetic, and persuasive Cancer is favored by the elves";
            } else if (zodiacSign == "Leo") {
                GiantRep += 0; DragonRep += 0; LizardRep += 0; OrcRep += 0; FaerieRep += 0; ElvesRep += 0; DwarvesRep += 0; GnomesRep += 0;
                Arcana = 3; Str = 1; Fort = 1; Agi = 1;type = "Earth"; zodiacDescription = "Creative, passionate, generous, warm-hearted, cheerful, humorous Leo are favored by the gnomes";
            } else if (zodiacSign == "Virgo") {
                GiantRep += 0; DragonRep += 200; LizardRep += -500; OrcRep += -500; FaerieRep += -500; ElvesRep += -500; DwarvesRep += -500; GnomesRep += -500;
                Arcana = 1; Str = 2; Fort = 2; Agi = 1;type = "Earth"; zodiacDescription = "Loyal, analytical, kind, hardworking, practical, Virgo are revered by the Dragons";
            } else if (zodiacSign == "Libra") {
                GiantRep += 200; DragonRep += 0; LizardRep += -500; OrcRep += -500; FaerieRep += -500; ElvesRep += -500; DwarvesRep += -500; GnomesRep += -500;
                Arcana = 2; Str = 1; Fort = 2; Agi = 1;type = "Air"; zodiacDescription = "Cooperative,diplomatic, gracious, fair-minded, social, Libra are revered by the Giants";
            } else if (zodiacSign == "Scorpio") {
                GiantRep += -200; DragonRep += -200; LizardRep += 600; OrcRep += 600; FaerieRep += 600; ElvesRep += 600; DwarvesRep += 600; GnomesRep += 600;
                Arcana = 0; Str = 2; Fort = 2; Agi = 2;type = "Water"; zodiacDescription = "Resourceful, powerful, brave, passionate, trustworthy, the scorpio are hated by dragons and giants but favored by all others";
            } else if (zodiacSign == "Sagittarius") {
                GiantRep += 0; DragonRep += 0; LizardRep += 0; OrcRep += 0; FaerieRep += -250; ElvesRep += -250; DwarvesRep += 1000; GnomesRep += -250;
                Arcana = 0; Str = 1; Fort = 4; Agi = 1; type = "Fire"; zodiacDescription = "Generous, idealistic, has a great sense of humor Sagittarius are favored by the Dwarves";
            } else if (zodiacSign == "Capricorn") {
                GiantRep += 100; DragonRep += 100; LizardRep += -500; OrcRep += -500; FaerieRep += -500; ElvesRep += -500; DwarvesRep += -500; GnomesRep += -500;
                Arcana = 1; Str = 1; Fort = 1; Agi = 3; type = "Earth"; zodiacDescription = "Responsible, disciplined, strong self-control, and strong leaders capricorns are revered by giants and dragons and uneasy with others.";
            } else if (zodiacSign == "Aquarius") {
                GiantRep += 0; DragonRep += 0; LizardRep += 0; OrcRep += 0; FaerieRep += -250; ElvesRep += -250; DwarvesRep += -250; GnomesRep += 1000;
                Arcana = 4; Str = 0; Fort = 1; Agi = 1; type = "Air"; zodiacDescription = "Progressive, original, independent, humanitarian, the Aquarius are viewed equally by all";
            } else if (zodiacSign == "Pisces") {
                GiantRep += 0; DragonRep += 0; LizardRep += 0; OrcRep += 0; FaerieRep += 1000; ElvesRep += 1000; DwarvesRep += 1000; GnomesRep += 1000;
                Arcana = 0; Str = 0; Fort = 1; Agi = 4; type = "Water"; zodiacDescription = "Compassionate, artistic, intuitive, gentle, wise, musical, the pisces are viewed neutrally";
            }
            clientMsg.bonusStatStrength = (int.Parse(clientMsg.bonusStatStrength) + Str).ToString();
            clientMsg.bonusStatAgility = (int.Parse(clientMsg.bonusStatAgility) + Agi).ToString();
            clientMsg.bonusStatFortitude = (int.Parse(clientMsg.bonusStatFortitude) + Fort).ToString();
            clientMsg.bonusStatArcana = (int.Parse(clientMsg.bonusStatArcana) + Arcana).ToString();
            if(clientMsg.BodyStyle == "Small"){
                clientMsg.bonusStatAgility = (int.Parse(clientMsg.bonusStatAgility) + 2).ToString();
                clientMsg.bonusStatArcana = (int.Parse(clientMsg.bonusStatArcana) + 8).ToString();
            }
            if(clientMsg.BodyStyle == "Average"){
                clientMsg.bonusStatFortitude = (int.Parse(clientMsg.bonusStatFortitude) + 1).ToString();
                clientMsg.bonusStatArcana = (int.Parse(clientMsg.bonusStatArcana) + 2).ToString();
                clientMsg.bonusStatStrength = (int.Parse(clientMsg.bonusStatStrength) + 2).ToString();
                clientMsg.bonusStatAgility = (int.Parse(clientMsg.bonusStatAgility) + 2).ToString();
            }
            if(clientMsg.BodyStyle == "Large"){
                clientMsg.bonusStatStrength = (int.Parse(clientMsg.bonusStatStrength) + 3).ToString();
                clientMsg.bonusStatFortitude = (int.Parse(clientMsg.bonusStatFortitude) + 3).ToString();
            }
            string armorBonus = "0";
            if(type == "Earth"){
                armorBonus = "1";
            }
            if(type == "Fire"){
                clientMsg.bonusStatArcana = (int.Parse(clientMsg.bonusStatArcana) + 5).ToString();
            }
            if(type == "Water"){
                clientMsg.bonusStatFortitude = (int.Parse(clientMsg.bonusStatFortitude) + 3).ToString();
            }
            if(type == "Air"){
                clientMsg.bonusStatAgility = (int.Parse(clientMsg.bonusStatAgility) + 5).ToString();
            }
            if(clientMsg.EyeColor == "Brown"){
                clientMsg.bonusStatStrength = (int.Parse(clientMsg.bonusStatStrength) + 1).ToString();
            }
            if(clientMsg.EyeColor == "Hazel"){
                clientMsg.bonusStatArcana = (int.Parse(clientMsg.bonusStatArcana) + 1).ToString();
            }
            if(clientMsg.EyeColor == "Blue"){
                clientMsg.bonusStatFortitude = (int.Parse(clientMsg.bonusStatFortitude) + 1).ToString();
            }
            if(clientMsg.EyeColor == "Green"){
                clientMsg.bonusStatAgility = (int.Parse(clientMsg.bonusStatAgility) + 1).ToString();
            }
            //add in all their info including the bonus and stuff to playfab so we can access on each login and add in to our stat page

            Dictionary<string, string> newPlayerData = new Dictionary<string, string>();
            newPlayerData.Add("TACTBUILDSTRING", clientMsg.BirthDate + "_" + zodiacSign + "_" +  clientMsg.EyeColor + "_" + clientMsg.BodyStyle + "_" + clientMsg.bonusStatStrength + "_" + clientMsg.bonusStatAgility + "_" + clientMsg.bonusStatFortitude + "_" + clientMsg.bonusStatArcana + "_" + armorBonus + 
            "_" + GiantRep.ToString() + "_" +  DragonRep.ToString() + "_" +  LizardRep.ToString() + "_" +  OrcRep.ToString() + "_" +  FaerieRep.ToString() + "_" +  ElvesRep.ToString() + "_" +  DwarvesRep.ToString() + "_" +  GnomesRep.ToString() + "_" + "0");
            PlayFabServerAPI.UpdateUserData(new UpdateUserDataRequest
            {
                PlayFabId = playerinfo.PlayFabId,
                Data = newPlayerData
            }, result =>
            {
                playerinfo.TactBuildString = clientMsg.BirthDate + "_" + zodiacSign + "_" +  clientMsg.EyeColor + "_" + clientMsg.BodyStyle + "_" + clientMsg.bonusStatStrength + "_" + clientMsg.bonusStatAgility + "_" + clientMsg.bonusStatFortitude + "_" + clientMsg.bonusStatArcana + "_" + armorBonus + 
            "_" + GiantRep.ToString() + "_" +  DragonRep.ToString() + "_" +  LizardRep.ToString() + "_" +  OrcRep.ToString() + "_" +  FaerieRep.ToString() + "_" +  ElvesRep.ToString() + "_" +  DwarvesRep.ToString() + "_" +  GnomesRep.ToString() + "_" + "0";
                playerinfo.PlayerSprite = clientMsg.Sprite;
                playerinfo.Energy = float.Parse("1500");
                nconn.authenticationData = playerinfo;
                OnServerAddPlayer(nconn);    
            }, error => {
            });
                         
        }
        #endif
        void VerifyNewPlayerData(NetworkConnectionToClient nconn, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR

            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {   
                StartCoroutine(SetNewPlayerData(nconn, playerData));
                //GrantPlayerTokens(nconn, playerData, result.UserInfo.PlayFabId); 
            }, error =>{
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void OnDKPTransmuteRequest(NetworkConnectionToClient nconn, XummTransmute request){
#if UNITY_SERVER || UNITY_EDITOR

            //validate them now and dont send unless we can if we cant reject it send it back to the client and close their window give them a 10 min wait timer
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            DKPTOGOLDTRANSMUTE(nconn, playerData.PlayFabId, request.amount);
            #endif
        }
        void OnGOLDTransmuteRequest(NetworkConnectionToClient nconn, XRPLTransmute request){
#if UNITY_SERVER || UNITY_EDITOR

            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = sPlayer.GetTacticianSheet();
            if(tactSheet.DKPCooldown == "0"){   
                long parsedAmount = long.Parse(request.amount);
                if(parsedAmount > 0 && parsedAmount <= sPlayer.Gold){
                    StartCoroutine(SubmitTransmuteRequest(nconn, parsedAmount));
                }
            }
            #endif
        }
#if UNITY_SERVER || UNITY_EDITOR

        IEnumerator SubmitTransmuteRequest(NetworkConnectionToClient nconn, long goldAmount){
        ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
        PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
        string userId = playerData.PlayFabId;
        string walletAddress = sPlayer.GetTacticianSheet().Address;
        WWWForm form = new WWWForm();
        string id = EncryptString(userId);
        form.AddField("userId", id);
        form.AddField("walletAddress", walletAddress);
        form.AddField("goldAmount", goldAmount.ToString());
        using (UnityWebRequest www = UnityWebRequest.Post(_Heroku_URL + "/dkpsend", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                ResponseObj response = JsonUtility.FromJson<ResponseObj>(www.downloadHandler.text);
                Debug.Log("Received JSON: " + www.downloadHandler.text);
                //print($"{response.success} was our success bool, {response.message} was our message and {response.details.userId} was our User ID, {response.details.goldAmount} was amount and {response.details.walletAddress} is our address");
                var jsonNode = JSON.Parse(www.downloadHandler.text);
                string _success = jsonNode["success"];
                string _message = jsonNode["message"];
                string _userId = jsonNode["details"]["userId"];
                string _id = DecryptString(_userId);
                string _goldAmount = jsonNode["details"]["goldAmount"];
                string _walletAddress = jsonNode["details"]["walletAddress"];
                string _XRPAMOUNT = jsonNode["details"]["xrpBalance"];
                string _DKPAMOUNT = jsonNode["details"]["dkpBalance"];

                
                Debug.Log("Success: " + _success);
                Debug.Log("Message: " + _message);
                Debug.Log("User ID: " + _id + " the encrypted id is " + _userId);
                Debug.Log("Gold Amount: " + _goldAmount);
                Debug.Log("Wallet Address: " + _walletAddress);
                Debug.Log("DKP Balance: " + _DKPAMOUNT);
                Debug.Log("XRP Balance: " + _XRPAMOUNT);

                if(_success == "true"){
                    sPlayer.Gold -= goldAmount;
                    Debug.Log("New Gold TOTAL: " + sPlayer.Gold);
                    float dkpBalancer = float.Parse(_DKPAMOUNT);
                    string dateTimeWithZone = DateTime.UtcNow.ToString("o");
                    //sPlayer.ServerSendDKPCD(dateTimeWithZone, _XRPAMOUNT, dkpBalancer.ToString("F2"));
                    StartCoroutine(NewDKPAmountTRANSMUTE(sPlayer, dkpBalancer, dateTimeWithZone));
                    var msg = new XRPLTransmute { code = "1", error = false };
                    nconn.Send(msg);
                    
                } else {
                    //its false
                    var msg = new XRPLTransmute { code = "0", error = true };
                    nconn.Send(msg);
                    print("Failed to make the swap");
                    
                }
                //if (response.success == "true")
                //{
                //    Debug.Log("Successfully completed transaction and removed gold");
                //    Debug.Log("User ID: " + response.details.userId);
                //    Debug.Log("Gold Amount: " + response.details.goldAmount);
                //    Debug.Log("Wallet Address: " + response.details.walletAddress);
                //}
                //else
                //{
                //    Debug.Log("Failed to complete transaction");
                //}
            }
        }
    }
     IEnumerator NewDKPAmountTRANSMUTE(ScenePlayer p, float dkpBalance, string CD){
            yield return new WaitForSeconds(.5f);
            p.ServerSendDKPCD(CD, p.GetTacticianSheet().XRPBalance, dkpBalance.ToString("F2"));
            //p.TargetWalletAwake();
        }
    #endif
        /*
        private IEnumerator GetBalancesNew(NetworkConnectionToClient nconn, PlayerInfo playerData, bool Dev, string wallet)
        {
            if(string.IsNullOrEmpty(wallet)){
                List<CharacterStatListItem> tacticianStats = new List<CharacterStatListItem>();
                CharacterStatListItem newLevelStat = (new CharacterStatListItem { Key = "LVL", Value = "1" });
                tacticianStats.Add(newLevelStat);
                CharacterStatListItem newEXPStat = (new CharacterStatListItem { Key = "EXP", Value = "0" });
                tacticianStats.Add(newEXPStat);
                CharacterStatListItem spriteStat = (new CharacterStatListItem { Key = "spriteTactician", Value = playerData.PlayerSprite });
                tacticianStats.Add(spriteStat);
                CharacterStatListItem newNameStat = (new CharacterStatListItem { Key = "TacticianName", Value = playerData.PlayerName });
                tacticianStats.Add(newNameStat);
                TacticianFullDataMessage fullTactMessage = (new TacticianFullDataMessage
                {
                    XRPBalance = "Not connected",
                    DKPBalance = "Not connected",
                    StashInventoryData = new List<CharacterInventoryListItem>(),
                    TacticianStatData = tacticianStats,
                    TacticianInventoryData = new List<CharacterInventoryListItem>()
                });
                StartCoroutone(SetNewPlayerInternalData(nconn, playerData, fullTactMessage, Dev, ( new NFTEQUIPMENT {NFTRINGOFTACTICIAN = true})));
                yield break;
            }
            string url = $"https://data.ripple.com/v2/accounts/{wallet}/balances";
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);

            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                XRPLResponse response = JsonUtility.FromJson<XRPLResponse>(jsonResponse);

                string XRP = "";
                string DKP = "";

                foreach (Balance balance in response.balances)
                {
                    if (balance.currency == "XRP")
                    {
                        XRP = balance.value;
                    }
                    else if (balance.currency == "DKP")
                    {
                        DKP = balance.value;
                    }
                }
                List<CharacterStatListItem> tacticianStats = new List<CharacterStatListItem>();
                CharacterStatListItem newLevelStat = (new CharacterStatListItem { Key = "LVL", Value = "1" });
                tacticianStats.Add(newLevelStat);
                CharacterStatListItem newEXPStat = (new CharacterStatListItem { Key = "EXP", Value = "0" });
                tacticianStats.Add(newEXPStat);
                CharacterStatListItem spriteStat = (new CharacterStatListItem { Key = "spriteTactician", Value = playerData.PlayerSprite });
                tacticianStats.Add(spriteStat);
                CharacterStatListItem newNameStat = (new CharacterStatListItem { Key = "TacticianName", Value = playerData.PlayerName });
                tacticianStats.Add(newNameStat);
                TacticianFullDataMessage fullTactMessage = (new TacticianFullDataMessage
                {
                    XRPBalance = XRP,
                    DKPBalance = DKP,
                    StashInventoryData = new List<CharacterInventoryListItem>(),
                    TacticianStatData = tacticianStats,
                    TacticianInventoryData = new List<CharacterInventoryListItem>()
                });
                Dictionary<string, string> newPlayerData = new Dictionary<string, string>();
                newPlayerData.Add("Reset", "R");
            #if UNITY_SERVER || UNITY_EDITOR

                PlayFabServerAPI.UpdateUserData(new UpdateUserDataRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    Data = newPlayerData
                }, result =>
                {   
                    StartCoroutine(SetNewPlayerInternalData(nconn, playerData, fullTactMessage, Dev, ( new NFTEQUIPMENT {NFTRINGOFTACTICIAN = true})));
                }, error =>
                {
                    Debug.Log(error.ErrorMessage);
                });
            #endif
                // You can now parse the response JSON to get the balances.
                // Consider using a JSON library like SimpleJSON or JsonUtility to parse the response.
            }
        }
        */
        IEnumerator SetNewPlayerData(NetworkConnectionToClient nconn, PlayerInfo playerData){
            yield return new WaitForSeconds(.1f);
            #if UNITY_SERVER || UNITY_EDITOR
            bool Dev = false;
            string wallet = string.Empty;
            bool ResetItems = false;
            Dictionary<string, string> newPlayerData = new Dictionary<string, string>();
            newPlayerData.Add("NewAccount89", null);
            PlayFabServerAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = playerData.PlayFabId,
            }, result =>
            {
                string ledgerAddress = string.Empty;
                if(result.Data.ContainsKey("PUBLICADDRESS")){
                    ledgerAddress = result.Data["PUBLICADDRESS"].Value;
                }
               
                    if(result.Data.ContainsKey("Reset") && result.Data["Reset"].Value == "R"){
                        ResetItems = true;
                        newPlayerData.Add("Reset", null);
                        PlayFabServerAPI.GetUserInternalData(new GetUserDataRequest
                        {
                            PlayFabId = playerData.PlayFabId,
                        }, result =>
                        {
                        // New dictionaries for the internal data update
                        Dictionary<string, string> internalDataOne = new Dictionary<string, string>();
                        Dictionary<string, string> internalDataTwo = new Dictionary<string, string>();
                        Dictionary<string, string> internalDataThree = new Dictionary<string, string>();
                        // Check for additional keys and add to the corresponding dictionary if found
                        string[] keysOne = { "LVL", "LastScene", "lastLogin", "energy", "EXP" };
                        string[] keysTwo = { "PartyMemberOne", "PartyMemberTwo", "PartyMemberThree", "PartyMemberZero", "PartyMemberFour" };
                        string[] keysThree = { "savedNode", "spriteTactician", "PartyMemberFive" };
                        bool keyoneEmpty = true;
                        bool keytwoEmpty = true;
                        bool keythreeEmpty = true;
                        foreach (string key in keysOne)
                        {
                            if (result.Data.ContainsKey(key))
                            {
                                internalDataOne.Add(key, null);
                                keyoneEmpty = false;
                            }
                        }
                        foreach (string key in keysTwo)
                        {
                            if (result.Data.ContainsKey(key))
                            {
                                internalDataTwo.Add(key, null);
                                keytwoEmpty = false;

                            }
                        }
                        foreach (string key in keysThree)
                        {
                            if (result.Data.ContainsKey(key))
                            {
                                internalDataThree.Add(key, null);
                                keythreeEmpty = false;
                            }
                        }

                        // Perform the internal data update
                        if(!keyoneEmpty){
                            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
                            {
                                PlayFabId = playerData.PlayFabId,
                                Data = internalDataOne
                            }, result => { /* Handle success */ }, error => { Debug.Log(error.ErrorMessage); });
                        }
                        
                        if(!keytwoEmpty){
                    
                        PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
                        {
                            PlayFabId = playerData.PlayFabId,
                            Data = internalDataTwo
                        }, result => { /* Handle success */ }, error => { Debug.Log(error.ErrorMessage); });
                        }
                        if(!keythreeEmpty){
                    
                        PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
                        {
                            PlayFabId = playerData.PlayFabId,
                            Data = internalDataThree
                        }, result => { /* Handle success */ }, error => { Debug.Log(error.ErrorMessage); });
                        }
                        }, error => { Debug.Log(error.ErrorMessage); });
                        PlayFabServerAPI.GetAllUsersCharacters(new ListUsersCharactersRequest
                        {
                            PlayFabId = playerData.PlayFabId
                        }, result =>
                        {
                            for(int j = 0; j < result.Characters.Count; j++){
                                PlayFabServerAPI.DeleteCharacterFromUser(new DeleteCharacterFromUserRequest
                                {
                                    PlayFabId = playerData.PlayFabId,
                                    CharacterId = result.Characters[j].CharacterId,
                                    SaveCharacterInventory = false // Set this to false to not save character's inventory
                                }, result =>
                                {
                                }, error =>
                                {
                                    Debug.Log(error.ErrorMessage);
                                });
                            }
                        }, error =>
                        {
                            Debug.Log(error.ErrorMessage);
                        });
                        
                    }

                    if(result.Data.ContainsKey("DEV") && result.Data["DEV"].Value == "KEY"){
                        Dev = true;
                    }
                    PlayFabServerAPI.UpdateUserData(new UpdateUserDataRequest
                    {
                        PlayFabId = playerData.PlayFabId,
                        Data = newPlayerData
                    }, result =>
                    { 
                        string address = null;
                        if(ResetItems){
                            PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest
                            {
                                PlayFabId = playerData.PlayFabId
                            }, result =>
                            {
                                //print("Made call LoginInventoryCheck");
                                // build tactician data we have everything we need now
                                
                                List<ItemInstance> inventory = result.Inventory;
                                List<ItemSelectable> TacticianItems = new List<ItemSelectable>();
                                if(inventory.Count > 0 && inventory != null){
                                    for (int i = 0; i < inventory.Count; i++)
                                    {
                                        ServerRemoveItemOnUser(nconn, inventory[i].ItemInstanceId);
                                    }
                                }
                                GrantItemsToUserRequest request = new GrantItemsToUserRequest();
                                request.CatalogVersion = "DragonKill_Characters_Bundles_Items";
                                List<string> UToken = new List<string>();
                                string token = "UniversalToken";
                                UToken.Add(token);
                                UToken.Add(token);
                                UToken.Add(token);
                                UToken.Add(token);
                                UToken.Add(token);
                                UToken.Add(token);
                                UToken.Add(token);
                                UToken.Add(token);
                                request.ItemIds = UToken;
                                request.PlayFabId = playerData.PlayFabId;
                                PlayFabServerAPI.GrantItemsToUser(request, result => {
                                    //StartCoroutine(GetBalancesNew(nconn, playerData, Dev, address));
                                    List<CharacterStatListItem> tacticianStats = new List<CharacterStatListItem>();
                                    CharacterStatListItem newLevelStat = (new CharacterStatListItem { Key = "LVL", Value = "1" });
                                    tacticianStats.Add(newLevelStat);
                                    CharacterStatListItem newEXPStat = (new CharacterStatListItem { Key = "EXP", Value = "0" });
                                    tacticianStats.Add(newEXPStat);
                                    CharacterStatListItem spriteStat = (new CharacterStatListItem { Key = "spriteTactician", Value = playerData.PlayerSprite });
                                    tacticianStats.Add(spriteStat);
                                    CharacterStatListItem newNameStat = (new CharacterStatListItem { Key = "TacticianName", Value = playerData.PlayerName });
                                    tacticianStats.Add(newNameStat);
                                    TacticianFullDataMessage fullTactMessage = (new TacticianFullDataMessage
                                    {
                                        XRPBalance = "Not connected",
                                        DKPBalance = "Not connected",
                                        StashInventoryData = new List<CharacterInventoryListItem>(),
                                        TacticianStatData = tacticianStats,
                                        TacticianInventoryData = new List<CharacterInventoryListItem>()
                                    });
                                    StartCoroutine(SetNewPlayerInternalData(nconn, playerData, fullTactMessage, Dev, true, ledgerAddress));
                                    //SetPlayerVirtualCurrency(request.PlayFabId);
                                }, error => {
                                    Debug.Log(error.ErrorMessage);
                                    Debug.Log(error.ErrorDetails);
                                    Debug.Log(error.Error);
                                });
                             }, error =>{
                                Debug.Log(error.ErrorMessage);
                            });
                            
                            //ServerRemoveItemOnUser
                        } else {
                            GrantItemsToUserRequest request = new GrantItemsToUserRequest();
                            request.CatalogVersion = "DragonKill_Characters_Bundles_Items";
                            List<string> UToken = new List<string>();
                            string token = "UniversalToken";
                            UToken.Add(token);
                            UToken.Add(token);
                            UToken.Add(token);
                            UToken.Add(token);
                            UToken.Add(token);
                            UToken.Add(token);
                            UToken.Add(token);
                            UToken.Add(token);
                            request.ItemIds = UToken;
                            request.PlayFabId = playerData.PlayFabId;
                            PlayFabServerAPI.GrantItemsToUser(request, result => {
                                //StartCoroutine(GetBalancesNew(nconn, playerData, Dev, address));
                                List<CharacterStatListItem> tacticianStats = new List<CharacterStatListItem>();
                                CharacterStatListItem newLevelStat = (new CharacterStatListItem { Key = "LVL", Value = "1" });
                                tacticianStats.Add(newLevelStat);
                                CharacterStatListItem newEXPStat = (new CharacterStatListItem { Key = "EXP", Value = "0" });
                                tacticianStats.Add(newEXPStat);
                                CharacterStatListItem spriteStat = (new CharacterStatListItem { Key = "spriteTactician", Value = playerData.PlayerSprite });
                                tacticianStats.Add(spriteStat);
                                CharacterStatListItem newNameStat = (new CharacterStatListItem { Key = "TacticianName", Value = playerData.PlayerName });
                                tacticianStats.Add(newNameStat);
                                TacticianFullDataMessage fullTactMessage = (new TacticianFullDataMessage
                                {
                                    XRPBalance = "Not connected",
                                    DKPBalance = "Not connected",
                                    StashInventoryData = new List<CharacterInventoryListItem>(),
                                    TacticianStatData = tacticianStats,
                                    TacticianInventoryData = new List<CharacterInventoryListItem>()
                                });
                                StartCoroutine(SetNewPlayerInternalData(nconn, playerData, fullTactMessage, Dev, false, ledgerAddress));
                            }, error => {
                                Debug.Log(error.ErrorMessage);
                                Debug.Log(error.ErrorDetails);
                                Debug.Log(error.Error);
                            });
                        }
                        
                    }, error =>{
                        Debug.Log(error.ErrorMessage);
                    });
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            #endif
        }
        #if UNITY_SERVER || UNITY_EDITOR
    public void GetPlayerVirtualCurrency(string playFabId){
        PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest(){
            PlayFabId = playFabId,
        }, result => {
            int currentBalance = result.VirtualCurrency.ContainsKey("DK") ? result.VirtualCurrency["DK"] : 0;
        }, error => Debug.LogError("Could not get player inventory: " + error.GenerateErrorReport()));
    }
    public void SetPlayerVirtualCurrency(string playFabId)
{
PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest()
{
    PlayFabId = playFabId,
},
result => {
    int currentBalance = result.VirtualCurrency.ContainsKey("DK") ? result.VirtualCurrency["DK"] : 0;
    int difference = 1000000 - currentBalance;

    if (difference > 0) {
        // Add the difference
        PlayFabServerAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest()
        {
            PlayFabId = playFabId,
            VirtualCurrency = "DK",
            Amount = difference
        },
        addResult => Debug.Log("Successfully set player's DK virtual currency to 1000000."),
        error => Debug.LogError("Could not add to player's DK virtual currency: " + error.GenerateErrorReport()));
    } else if (difference < 0) {
        // Subtract the absolute value of the difference
        PlayFabServerAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest()
        {
            PlayFabId = playFabId,
            VirtualCurrency = "DK",
            Amount = Math.Abs(difference)
        },
        subtractResult => Debug.Log("Successfully set player's DK virtual currency to 1000000."),
        error => Debug.LogError("Could not subtract from player's DK virtual currency: " + error.GenerateErrorReport()));
    }
},
error => Debug.LogError("Could not get player inventory: " + error.GenerateErrorReport()));
}
        #endif
    //SetNewPlayerData Here  
        #if UNITY_SERVER || UNITY_EDITOR

    IEnumerator SetNewPlayerInternalData(NetworkConnectionToClient nconn, PlayerInfo playerData, TacticianFullDataMessage fullTactMessage, bool Dev, bool reset, string ledgerAddress){
        string XRP = "";
        string DKP = "";
        string url = $"https://data.ripple.com/v2/accounts/{ledgerAddress}/balances";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            XRPLResponse response = JsonUtility.FromJson<XRPLResponse>(jsonResponse);
            
            foreach (Balance balance in response.balances)
            {
                if (balance.currency == "XRP")
                {
                    XRP = balance.value;
                }
                else if (balance.currency == "DKP")
                {
                    DKP = balance.value;
                }
            }
            //we have their balances now
        }
        bool nftList = false;
        Dictionary<string, string> NFTIDs = new Dictionary<string, string>();
        if(!string.IsNullOrEmpty(ledgerAddress)){
            UnityWebRequest nftListRequest = UnityWebRequest.Get(xrpscanAPI + ledgerAddress + "/nfts");
            yield return nftListRequest.SendWebRequest();
            if (nftListRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(nftListRequest.error);
            }
            else
            {
                string nftJson = nftListRequest.downloadHandler.text;
                NFTData[] nftDataArray = JsonHelper.FromJson<NFTData>(nftJson);
                if(nftDataArray != null){
                    foreach (NFTData nftData in nftDataArray)
                    {
                        bool nftInfo = false;
                        if (nftData.Issuer == "rhB7i1DDJAmw1A3sVi6nR89GWcUkNPo6KJ")  
                        {
                            string metadataURI = nftData.URI;  // This URI can now be used in your subsequent requests
                            string metadataNFTID = nftData.NFTokenID;
                            if(!string.IsNullOrEmpty(metadataNFTID)){
                                print($"NFT ID = {metadataNFTID}");
                            }
                            UnityWebRequest NFTURI = UnityWebRequest.Get(metadataURI);
                            yield return NFTURI.SendWebRequest();
                            if (NFTURI.result != UnityWebRequest.Result.Success)
                            {
                                Debug.Log(NFTURI.error);
                            }
                            else
                            {
                                string metadataJson = NFTURI.downloadHandler.text;
                                NFTMetadata metadata = JsonUtility.FromJson<NFTMetadata>(metadataJson);
                                string md5hash = metadata.md5hash;
                                NFTIDs.Add(md5hash, metadataNFTID);//md5hash is how we ciper which nft type it is, ID is singularity
                                string NFTName = metadata.name;
                                Debug.Log("MD5 Hash: " + md5hash);
                                Debug.Log("NFT: " + NFTName);
                                Debug.Log("Issuer: " + nftData.Issuer);
                                nftInfo = true;
                                // Perform your checks or operations here
                            }
                        } else {
                            nftInfo = true;
                        }
                        while(!nftInfo){
                            yield return new WaitForSeconds(.1f);
                        }
                    }
                }
                nftList = true;
            }
        }
        while(!nftList){
            yield return new WaitForSeconds(.1f);
        }   
        
        string startEnergy = "1500";
        float xrpfloat = float.Parse(XRP);
        float dkpfloat = float.Parse(DKP);

        PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string>
                {
                    {"spriteTactician", playerData.PlayerSprite},
                    {"energy", startEnergy}, {"EXP", "0"}, {"LVL", "1"}
                }
            }, result =>
            { 

                playerData.Energy = float.Parse(startEnergy);
                playerData.XRPLPUBLIC = ledgerAddress;
                nconn.authenticationData = playerData;
                ScenePlayer q = nconn.identity.gameObject.GetComponent<ScenePlayer>();
                TacticianExtractor tactData = ParseClientData(playerData.TactBuildString);
                fullTactMessage.DKPBalance = dkpfloat.ToString("F2");
                fullTactMessage.XRPBalance = xrpfloat.ToString("F2");
                fullTactMessage.Address = playerData.XRPLPUBLIC;
                fullTactMessage.tactBuild = playerData.TactBuildString;
                fullTactMessage.StrengthBonus = tactData.BonusStatStrength;
                fullTactMessage.AgilityBonus = tactData.BonusStatAgility;
                fullTactMessage.ArcanaBonus = tactData.BonusStatArcana;
                fullTactMessage.FortitudeBonus = tactData.BonusStatFortitude;
                fullTactMessage.ArmorBonus = tactData.BonusStatArmor;
                fullTactMessage.EyeColor = tactData.EyeColor;
                fullTactMessage.BodyStyle = tactData.BodyStyle;
                fullTactMessage.Birthdate = tactData.BirthDate;
                fullTactMessage.DKPCooldown = tactData.DKPCooldown;
                CharacterStatListItem GiantRepStat = (new CharacterStatListItem { Key = "GiantRep", Value = tactData.GiantRep});
                fullTactMessage.TacticianStatData.Add(GiantRepStat);
                CharacterStatListItem DragonRepStat = (new CharacterStatListItem { Key = "DragonRep", Value = tactData.DragonRep });
                fullTactMessage.TacticianStatData.Add(DragonRepStat);
                CharacterStatListItem LizardRepStat = (new CharacterStatListItem { Key = "LizardRep", Value = tactData.LizardRep });
                fullTactMessage.TacticianStatData.Add(LizardRepStat);
                CharacterStatListItem OrcRepStat = (new CharacterStatListItem { Key = "OrcRep", Value = tactData.OrcRep });
                fullTactMessage.TacticianStatData.Add(OrcRepStat);
                CharacterStatListItem FaerieStat = (new CharacterStatListItem { Key = "FaerieRep", Value = tactData.FaerieRep });
                fullTactMessage.TacticianStatData.Add(FaerieStat);
                CharacterStatListItem ElfRepStat = (new CharacterStatListItem { Key = "ElfRep", Value = tactData.ElvesRep });
                fullTactMessage.TacticianStatData.Add(ElfRepStat);
                CharacterStatListItem DwarfRepStat = (new CharacterStatListItem { Key = "DwarfRep", Value = tactData.DwarvesRep });
                fullTactMessage.TacticianStatData.Add(DwarfRepStat);
                CharacterStatListItem GnomeRepStat = (new CharacterStatListItem { Key = "GnomeRep", Value = tactData.GnomesRep });
                fullTactMessage.TacticianStatData.Add(GnomeRepStat);
                fullTactMessage.tactString = playerData.PlayerSprite;
                StartCoroutine(StartNewPlayerClient(nconn, playerData, fullTactMessage, Dev, reset, NFTIDs));
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
    }
    IEnumerator StartNewPlayerClient(NetworkConnectionToClient nconn, PlayerInfo playerData, TacticianFullDataMessage fullTactMessage, bool Dev, bool reset, Dictionary<string, string> NFTIDs){
        ScenePlayer q = nconn.identity.gameObject.GetComponent<ScenePlayer>();
        bool readyWGold = false;
        int currentBalance = 0;
                if(reset){
                    PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest(){
                        PlayFabId = playerData.PlayFabId,
                    }, result => {
                        currentBalance = result.VirtualCurrency.ContainsKey("DK") ? result.VirtualCurrency["DK"] : 0;
                        readyWGold = true;
                    }, error =>{
                        readyWGold = true;
                        Debug.LogError("Could not get player inventory: " + error.GenerateErrorReport());
                    });
                    
                } else {
                    readyWGold = true;
                }
                while(!readyWGold){
                    yield return new WaitForSeconds(.1f);
                }
                if(!reset){

                }
                q.GetFullTacticianData(fullTactMessage);
                q.SetPlayerData(playerData);
                //q.TargetToggleSprite(false, q.loadSprite);
                q.TargetOpenUI("TOWNOFARUDINE");
                q.TargetCharge(1500);
                
                if (NFTIDs.Count > 0)
                {
                    StartCoroutine(SpawnNewNFTS(NFTIDs, playerData, nconn));
                }
                if(Dev){
                    StartCoroutine(GetAllItemsInDragonKill(nconn, playerData));
                }
                q.Gold = (long)currentBalance;
                q.TargetWalletAwake();
                ClientRequestLoadScene dummy = new ClientRequestLoadScene {
                    oldScene = "Container",
                    newScene = TOWNOFARUDINE,
                    login = true
                };
                GetCleanedSceneName(nconn, dummy);
                StartCoroutine(ReChargeEnergy(nconn));
                q.TokenCounted(8);
                q.TargetTokenUpdate();
    }
    IEnumerator SpawnNewNFTS(Dictionary<string, string> NFTIDs, PlayerInfo playerData, NetworkConnectionToClient nconn){
        if (NFTIDs.Count > 0){
            Dictionary<string, string> nfts = GetNFTList(NFTIDs);
            foreach(var nft in nfts){
                print($"NFT md5Hash = {nft.Key} and its NFT ID is {nft.Value}");
            }
            int itemsPerRequest = 20; // Set to 20 NFTs at a time
            if(nfts.Count < itemsPerRequest){
                itemsPerRequest = nfts.Count;
            }
            int maxBatchBeforeCooldown = 5; // After 5 batches, cooldown
            int currentBatch = 0;
            for (int i = 0; i < nfts.Count; )
            {
                bool finishedOne = false; // Reset the flag for the new batch
                List<string> itemsIDForThisRequest = nfts.Values.Skip(i).Take(itemsPerRequest).ToList();
                List<string> itemsForThisRequest = nfts.Keys.Skip(i).Take(itemsPerRequest).ToList();
                Dictionary<string, string> batchNFTs = itemsForThisRequest.Zip(itemsIDForThisRequest, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
                if(batchNFTs == null){
                    print("WARNING NFT BATCH IS NULL FOR SOME REASON***********************");
                }
                PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    ItemIds = itemsForThisRequest
                },
                result =>
                {
                    StartCoroutine(ProcessItems(nconn, playerData, result.ItemGrantResults, batchNFTs, () =>
                    {
                        finishedOne = true; // Signal that this batch is finished
                    }));
                },
                error =>
                {
                    // Handle error
                });
                // Wait for the batch to finish before continuing
                while (!finishedOne)
                {
                    yield return new WaitForSeconds(.1f);
                }
                // Increment your loop counter
                i += itemsPerRequest;
                // Increment the batch count
                currentBatch++;
                if (currentBatch >= maxBatchBeforeCooldown)
                {
                    // Cooldown for 15 seconds
                    yield return new WaitForSeconds(15f);
                    currentBatch = 0; // Reset the batch counter
                }
                else
                {
                    // Wait 1 second before next batch
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
    Dictionary<string, string> GetNFTList(Dictionary <string, string> nftDictionary){
        //the key is the md5hash we need to figure out which, the value is going to be the nftID\
        string SpearOfDragonslaying = "NFT_SpearOfDragonslaying";
        string SwordOfFire = "NFT_SwordOfFire";
        string AcidicAxe = "NFT_AcidicAxe";
        string MaceOfHealing = "NFT_MaceOfHealing";
        string BowOfPower = "NFT_BowOfPower";
        string FrozenGreatsword = "NFT_FrozenGreatsword";
        string StaffOfProtection = "NFT_StaffOfProtection";
        string GreatspearOfDragonSlaying = "NFT_GreatspearOfDragonslaying";
        string ThunderInfusedGreathammer = "NFT_ThunderInfusedGreathammer";
        string VampiricDagger = "NFT_VampiricDagger";
        string VenomousGreataxe = "NFT_VenomousGreataxe";
        string RingOfTheTactician = "TACTNFT_RingOfTheTactician";
        Dictionary<string, string> OwnedNFTs = new Dictionary<string, string>();
        foreach(var kvp in nftDictionary){
            if(kvp.Key == "0b887ab70ddba6465dbfd0fe57ffbcde"){//ring of tactician
                string nftID = kvp.Value;
                OwnedNFTs.Add(RingOfTheTactician, nftID);
            }
            if(kvp.Key == "90a9da7e8426a118883902d856d69cb5"){//Mace of Healing
                string nftID = kvp.Value;
                OwnedNFTs.Add(MaceOfHealing, nftID);
            }
            //if(kvp.Key == "8abc8b74239115f2d8daf6de3ebc71a1"){//Arudine guild plot
            //    string nftID = kvp.Value;
            //    OwnedNFTs.Add(GuildPlot, nftID);
            //}
            //if(kvp.Key == "523819ad5decced3d0d47a973ae921c6"){//Housing plot
            //    string nftID = kvp.Value;
            //    OwnedNFTs.Add(HousingPlot, nftID);
            //}
            if(kvp.Key == "aba2af3c46490ef57e6499cc6bb5a14a"){//Sword of fire
                string nftID = kvp.Value;
                OwnedNFTs.Add(SwordOfFire, nftID);
            }
        }
        return OwnedNFTs;
    }
    IEnumerator DelayXRPLRegistration(NetworkConnectionToClient nconn, string playFabID){
        yield return new WaitForSeconds(15f);
        RegisterAccount(nconn, playFabID, 0, null);
    }
        #endif

        //Login w/ existing account
            #if UNITY_SERVER || UNITY_EDITOR

        IEnumerator GetPlayerData(string playFabID, NetworkConnectionToClient nconn, string tacticianNamePlayFab, string ticket, PlayerInfo netMsg, string publicAddress, string tactbuildString){
            string XRP = "";
            string DKP = "";
            string url = $"https://data.ripple.com/v2/accounts/{publicAddress}/balances";
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                XRPLResponse response = JsonUtility.FromJson<XRPLResponse>(jsonResponse);

                foreach (Balance balance in response.balances)
                {
                    if (balance.currency == "XRP")
                    {
                        XRP = balance.value;
                    }
                    else if (balance.currency == "DKP")
                    {
                        DKP = balance.value;
                    }
                }
                //we have their balances now
            }
            bool nftList = false;
            //Dictionary<string, string> NFTIDs = new Dictionary<string, string>();
            List<string> md5HashList = new List<string>();
            List<string> nftIDs = new List<string>();
            if(!string.IsNullOrEmpty(publicAddress)){
                UnityWebRequest nftListRequest = UnityWebRequest.Get(xrpscanAPI + publicAddress + "/nfts");
                yield return nftListRequest.SendWebRequest();
                if (nftListRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(nftListRequest.error);
                }
                else
                {
                    string nftJson = nftListRequest.downloadHandler.text;
                    NFTData[] nftDataArray = JsonHelper.FromJson<NFTData>(nftJson);
                    if(nftDataArray != null){
                        foreach (NFTData nftData in nftDataArray)
                        {
                            bool nftInfo = false;
                            if (nftData.Issuer == "rhB7i1DDJAmw1A3sVi6nR89GWcUkNPo6KJ")  
                            {
                                string metadataURI = nftData.URI;  // This URI can now be used in your subsequent requests
                                string metadataNFTID = nftData.NFTokenID;
                                if(!string.IsNullOrEmpty(metadataNFTID)){
                                    print($"NFT ID = {metadataNFTID}");
                                }
                                UnityWebRequest NFTURI = UnityWebRequest.Get(metadataURI);
                                yield return NFTURI.SendWebRequest();
                                if (NFTURI.result != UnityWebRequest.Result.Success)
                                {
                                    Debug.Log(NFTURI.error);
                                }
                                else
                                {
                                    string metadataJson = NFTURI.downloadHandler.text;
                                    NFTMetadata metadata = JsonUtility.FromJson<NFTMetadata>(metadataJson);
                                    string md5hash = metadata.md5hash;
                                    //NFTIDs.Add(md5hash, metadataNFTID);//md5hash is how we ciper which nft type it is, ID is singularity
                                    md5HashList.Add(md5hash);
                                    nftIDs.Add(metadataNFTID);
                                    string NFTName = metadata.name;
                                    Debug.Log("MD5 Hash: " + md5hash);
                                    Debug.Log("NFT: " + NFTName);
                                    Debug.Log("Issuer: " + nftData.Issuer);
                                    nftInfo = true;
                                    // Perform your checks or operations here
                                }
                            } else {
                                nftInfo = true;
                            }
                            while(!nftInfo){
                                yield return new WaitForSeconds(.1f);
                            }
                        }
                    }
                    nftList = true;
                }
            }
            while(!nftList){
                yield return new WaitForSeconds(.1f);
            }
            print("Starting GetPlayerData from prior logout");
            PlayFabServerAPI.GetUserInternalData(new GetUserDataRequest
            {
                PlayFabId = playFabID,
                
            }, result =>
            {
                //print("Made call in getPlayer Data");
                List<string> Party = new List<string>();
                string PartyMemberZero = null;
                string PartyMemberOne = null;
                string PartyMemberTwo = null;
                string PartyMemberThree = null;
                string PartyMemberFour = null;
                string PartyMemberFive = null;
                if(result.Data.ContainsKey("PartyMemberZero")){
                    PartyMemberZero = result.Data["PartyMemberZero"].Value;
                    Party.Add(PartyMemberZero);
                }
                if(result.Data.ContainsKey("PartyMemberOne")){
                    PartyMemberOne = result.Data["PartyMemberOne"].Value;
                    Party.Add(PartyMemberOne);
                }
                if(result.Data.ContainsKey("PartyMemberTwo")){
                    PartyMemberTwo = result.Data["PartyMemberTwo"].Value;
                    Party.Add(PartyMemberTwo);
                }
                if(result.Data.ContainsKey("PartyMemberThree")){
                    PartyMemberThree = result.Data["PartyMemberThree"].Value;
                    Party.Add(PartyMemberThree);
                }
                if(result.Data.ContainsKey("PartyMemberFour")){
                    PartyMemberFour = result.Data["PartyMemberFour"].Value;
                    Party.Add(PartyMemberFour);
                }
                if(result.Data.ContainsKey("PartyMemberFive")){
                    PartyMemberFive = result.Data["PartyMemberFive"].Value;
                    Party.Add(PartyMemberFive);
                }
                // save value as character ID!! will ensure its the proper character in battle
                float currentEnergy = 1500f;
                string LastLogin = null;
                if(result.Data.ContainsKey("lastLogin")){
                    LastLogin = result.Data["lastLogin"].Value;
                    currentEnergy = 0f;
                    DateTime datePrior = DateTime.Parse(LastLogin);
                    DateTime dateCurrent = DateTime.Now;
                    float fastChargeRate = 3000f / (24f * 60f); // 3000 energy per day, converted to per minute
                    float normalChargeRate = 1000f / (24f * 60f); // 1000 energy per day, converted to per minute
                    float maxEnergy = 10000f;
                    float lowEnergy = 3000f;
                    float oldEnergy = float.Parse(result.Data["energy"].Value);
                    double elapsedMinutes = (dateCurrent - datePrior).TotalMinutes;

                    if(oldEnergy < lowEnergy){
                        double minutesToReachLowEnergy = (lowEnergy - oldEnergy) / fastChargeRate;

                        if(elapsedMinutes <= minutesToReachLowEnergy){
                            currentEnergy = oldEnergy + (float)(elapsedMinutes * fastChargeRate);
                        }else{
                            double fastChargedEnergy = (minutesToReachLowEnergy * fastChargeRate) + oldEnergy;
                            double remainingMinutes = elapsedMinutes - minutesToReachLowEnergy;
                            double finalEnergy = fastChargedEnergy + (remainingMinutes * normalChargeRate);
                            currentEnergy = (float)(finalEnergy > maxEnergy ? maxEnergy : finalEnergy);
                        }
                    }else{
                        double additionalEnergy = elapsedMinutes * normalChargeRate;
                        currentEnergy = (float)((oldEnergy + additionalEnergy > maxEnergy) ? maxEnergy : oldEnergy + additionalEnergy);
                    }
                }
                string LastScene = "TOWNOFARUDINE";
                if(result.Data.ContainsKey("LastScene")){
                    LastScene = result.Data["LastScene"].Value;
                }
                string savedNode = string.Empty;
                if(result.Data.ContainsKey("savedNode")){
                    savedNode = result.Data["savedNode"].Value;
                } else {
                    savedNode = "TOWNOFARUDINE";
                }
                LastScene = (savedNode == "TOWNOFARUDINE") ? "TOWNOFARUDINE" : "OVM";
                PlayerInfo playerinfo = new PlayerInfo {
                    Energy = currentEnergy,
                    newPlayer = false, 
                    PlayFabId = playFabID,
                    PlayerName = tacticianNamePlayFab,
                    SavedNode = savedNode,
                    SessionTicket = ticket,
                    PlayerSprite = result.Data["spriteTactician"].Value,
                    CurrentScene = LastScene,
                    PartyIDs = Party,
                    PlayerLevel = result.Data["LVL"].Value,
                    PlayerExperience = result.Data["EXP"].Value,
                    XRPLPUBLIC = publicAddress,
                    NFTIDS = nftIDs,
                    NFTMD5Hash = md5HashList,
                    XRP = XRP,
                    DKP = DKP,
                    lastLogin = LastLogin,
                    TactBuildString = tactbuildString
                };
                nconn.authenticationData = playerinfo;
                //print($"{playerinfo.Energy} is energy, {playerinfo.PlayFabId} is PlayFabId, {playerinfo.PlayerName} is PlayerName, {playerinfo.PlayerSprite} is PlayerSprite, ");
                base.OnServerConnect(nconn);   
                StartCoroutine(WaitSeconds(nconn));
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
        }
            #endif

        IEnumerator WaitSeconds(NetworkConnectionToClient nconn){
            yield return new WaitForSeconds(3f);
            OnServerAddPlayer(nconn);
        }
        void LoginInventoryCheck(NetworkConnectionToClient nconn, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            
            PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest
            {
                PlayFabId = playerData.PlayFabId
            }, result =>
            {
                //print("Made call LoginInventoryCheck");
                // build tactician data we have everything we need now
                int gold = 0;
                Dictionary<string, int> currencies = result.VirtualCurrency;
                foreach(var key in currencies){
                    if(key.Key == "DK"){
                        gold = key.Value;
                        //print(gold);
                    }
                }
                nconn.authenticationData = playerData;
                List<ItemInstance> inventory = result.Inventory;
                string id = string.Empty;
                int tokens = 0;
                List<ItemSelectable> TacticianItems = new List<ItemSelectable>();
                Dictionary<string, string> NFTDictionary = new Dictionary<string, string>();
                if(inventory.Count > 0 && inventory != null){
                    for (int i = 0; i < inventory.Count; i++)
                    {
                        if(inventory[i].ItemId == "UniversalToken"){
                            id = inventory[i].ItemInstanceId; 
                            int? TokenQuant = inventory[i].RemainingUses;
                            if(TokenQuant.HasValue){
                                tokens = TokenQuant.Value;
                            }
                            continue;
                        }
                        bool tInv = false;
                        bool tEquip = false;
                        bool tStash = false;
                        bool tBelt = false;
                        bool nonFungibleToken = false;
                        string Weight = "0";
                        string STRENGTH_item = "0";
                        string AGILITY_item = "0";
                        string FORTITUDE_item = "0";
                        string ARCANA_item = "0";
                        string Rarity_item = "0";
                        string MagicResist_item = "0";
                        string FireResist_item = "0";
                        string ColdResist_item = "0";
                        string PoisonResist_item = "0";
                        string DiseaseResist_item = "0";
                        string Armor = "0";
                        string Durability_item = "100";
                        string DurabilityMax_item = "100";
                        string DamageMin_item = "0";
                        string DamageMax_item = "0";
                        string Parry_item = "0";
                        string Penetration_item= "0";
                        string AttackDelay_item = "0";
                        string BlockChance_item = "0";
                        string Quality_item = "Plain";
                        string BlockValue_item = "0";
                        string TypeOfItem = "0";
                        string Item_Slot = "0";
                        string Item_Name = "0";
                        string equippedslot = "0";
                        int quantity = 1;
                        string OWNERID = "Tactician";
                        string NFTID = "NotNFT";

                        //int? quant = inventory[i].RemainingUses;
                        foreach(var stat in inventory[i].CustomData){
                            if(stat.Key == "Amount"){
                                quantity = int.Parse(stat.Value);
                            }
                            if(stat.Key == "Weight"){
                                Weight = stat.Value;
                            }
                            if(stat.Key == "STRENGTH_item"){
                                STRENGTH_item = stat.Value;
                            }
                            if(stat.Key == "AGILITY_item"){
                                AGILITY_item = stat.Value;
                            }
                            if(stat.Key == "FORTITUDE_item"){
                                FORTITUDE_item = stat.Value;
                            }
                            if(stat.Key == "ARCANA_item"){
                                ARCANA_item = stat.Value;
                            }
                            if(stat.Key == "NFTID"){
                                NFTID = stat.Value;
                            }
                            if(stat.Key == "Rarity_item"){
                                Rarity_item = stat.Value;
                                if(stat.Value == "NFT"){
                                    nonFungibleToken = true;
                                }
                            }
                            if(stat.Key == "MagicResist_item"){
                                MagicResist_item = stat.Value;
                            }
                            if(stat.Key == "FireResist_item"){
                                FireResist_item = stat.Value;
                            }
                            if(stat.Key == "ColdResist_item"){
                                ColdResist_item = stat.Value;
                            }
                            if(stat.Key == "PoisonResist_item"){
                                PoisonResist_item = stat.Value;
                            }
                            if(stat.Key == "DiseaseResist_item"){
                                DiseaseResist_item = stat.Value;
                            }
                            if(stat.Key == "Armor_item"){
                                Armor = stat.Value;
                            }
                            if(stat.Key == "Durability_item"){
                                Durability_item = stat.Value;
                            }
                            if(stat.Key == "DurabilityMax_item"){
                                DurabilityMax_item = stat.Value;
                            }
                            if(stat.Key == "DamageMin_item"){
                                DamageMin_item = stat.Value;
                            }
                            if(stat.Key == "DamageMax_item"){
                                DamageMax_item = stat.Value;
                            }
                            if(stat.Key == "Parry_item"){
                                Parry_item = stat.Value;
                            }
                            if(stat.Key == "AttackDelay_item"){
                                AttackDelay_item = stat.Value;
                            }
                            if(stat.Key == "Penetration_item"){
                                Penetration_item = stat.Value;
                            }
                            if(stat.Key == "BlockValue_item"){
                                BlockValue_item = stat.Value;
                            }
                            if(stat.Key == "BlockChance_item"){
                                BlockChance_item = stat.Value;
                            }
                            if(stat.Key == "Quality_item"){
                                Quality_item = stat.Value;
                            }
                            if(stat.Key == "TypeOfItem"){
                                TypeOfItem = stat.Value;
                            }
                            if(stat.Key == "Item_Slot"){
                                Item_Slot = stat.Value;
                            }
                            if(stat.Key == "Item_Name"){
                                Item_Name = stat.Value;
                            }
                            if(stat.Key == "EquippedSlot"){
                                equippedslot = stat.Value;
                                if(stat.Value != "0" || stat.Value != "Unequipped"){
                                    tEquip = true;
                                }
                            }
                            if(stat.Key == "TactInventory"){
                                string tactInv = null;
                                tactInv = stat.Value;
                                if(tactInv != null && tactInv == "InventoryItem" ){
                                    tInv = true;
                                }
                            }
                            if(stat.Key == "TactStash"){
                                string tactStash = null;
                                tactStash = stat.Value;
                                if(tactStash != null && tactStash == "Stashed" ){
                                    tStash = true;
                                    OWNERID = "Stash";
                                }
                            }
                            if(stat.Key == "TactBelt"){
                                string tactBelt = null;
                                tactBelt = stat.Value;
                                if(tactBelt != null && tactBelt == "Belted" ){
                                    tBelt = true;
                                }
                            }
                            //if(stat.Key == "TactEquipped"){
                            //    string tactEquip = null;
                            //    tactEquip = stat.Value;
                            //    if(tactEquip != null && tactEquip == "Equipment" ){
                            //        tEquip = true;
                            //    }
                            //}
                            if(stat.Key == "NFT"){
                                string NFTtext = null;
                                NFTtext = stat.Value;
                                if(NFTtext != null && NFTtext == "NFT" ){
                                    nonFungibleToken = true;
                                }
                            }
                        }
                        
                        ItemSelectable.ItemType type = ItemSelectable.ItemType.None;
                        string itemClass = inventory[i].ItemClass;
                        string itemID = inventory[i].ItemId;
                        if(NFTID != "NotNFT"){
                            NFTDictionary.Add(itemID, NFTID);
                        }
                        //print($"{Item_Name} was added to {playerData.PlayerName}");
                        if(itemClass == "Misc"){
                            type = ItemSelectable.ItemType.Misc;
                        }
                        if(itemClass == "GemstoneT2"){
                            type = ItemSelectable.ItemType.GemstoneT2;
                        }
                        if(itemClass == "GemstoneT3"){
                            type = ItemSelectable.ItemType.GemstoneT3;
                        }
                        if(itemClass == "GemstoneT4"){
                            type = ItemSelectable.ItemType.GemstoneT4;
                        }
                        if(itemClass == "GemstoneT5"){
                            type = ItemSelectable.ItemType.GemstoneT5;
                        }
                        if(itemClass == "RefinedMaterialT1"){
                            type = ItemSelectable.ItemType.RefinedMaterialT1;
                        }
                        if(itemClass == "MaterialT1"){
                            type = ItemSelectable.ItemType.MaterialT1;
                        }
                        if(itemClass == "ResourceT1"){
                            type = ItemSelectable.ItemType.ResourceT1;
                        }
                        if(itemClass == "FoodT1"){
                            type = ItemSelectable.ItemType.FoodT1;
                        }
                        if(itemClass == "GrownT1"){
                            type = ItemSelectable.ItemType.GrownT1;
                        }
                        if(itemClass == "PotionT1"){
                            type = ItemSelectable.ItemType.PotionT1;
                        }
                        if(itemClass == "PatternT2"){
                            type = ItemSelectable.ItemType.PatternT2;
                        }
                        if(itemClass == "PatternT3"){
                            type = ItemSelectable.ItemType.PatternT3;
                        }
                        if(itemClass == "PatternT4"){
                            type = ItemSelectable.ItemType.PatternT4;
                        }
                        if(itemClass == "PatternT5"){
                            type = ItemSelectable.ItemType.PatternT5;
                        }
                        if(itemClass == "Tool"){
                            type = ItemSelectable.ItemType.Tool;
                        }
                        if(itemClass == "Ammo"){
                            type = ItemSelectable.ItemType.Ammo;
                        }
                        if(itemClass == "Head"){
                            type = ItemSelectable.ItemType.Head;
                        }
                        if(itemClass == "Earring"){
                            type = ItemSelectable.ItemType.Earring;
                        }
                        if(itemClass == "Necklace"){
                            type = ItemSelectable.ItemType.Necklace;
                        }
                        if(itemClass == "Waist"){
                            type = ItemSelectable.ItemType.Waist;
                        }
                        if(itemClass == "Chest"){
                            type = ItemSelectable.ItemType.Chest;
                        }
                        if(itemClass == "Shoulders"){
                            type = ItemSelectable.ItemType.Shoulders;
                        }
                        if(itemClass == "Arms"){
                            type = ItemSelectable.ItemType.Arms;
                        }
                        if(itemClass == "Wrists"){
                            type = ItemSelectable.ItemType.Wrists;
                        }
                        if(itemClass == "Leggings"){
                            type = ItemSelectable.ItemType.Leggings;
                        }
                        if(itemClass == "Hands"){
                            type = ItemSelectable.ItemType.Hands;
                        }
                        if(itemClass == "Feet"){
                            type = ItemSelectable.ItemType.Feet;
                        }
                        if(itemClass == "Ring"){
                            type = ItemSelectable.ItemType.Ring;
                        }
                        if(itemClass == "SingleHandedWeapon"){
                            type = ItemSelectable.ItemType.SingleHandedWeapon;
                        }
                        if(itemClass == "TwoHandedWeapon"){
                            type = ItemSelectable.ItemType.TwoHandedWeapon;
                        }
                        if(itemClass == "OffHand"){
                            type = ItemSelectable.ItemType.OffHand;
                        }
                        if(itemClass == "Shield"){
                            type = ItemSelectable.ItemType.Shield;
                        }
                        if(itemClass == "NFTHead"){
                            type = ItemSelectable.ItemType.NFTHead;
                        }
                        if(itemClass == "NFTEarring"){
                            type = ItemSelectable.ItemType.NFTEarring;
                        }
                        if(itemClass == "NFTNecklace"){
                            type = ItemSelectable.ItemType.NFTNecklace;
                        }
                        if(itemClass == "NFTWaist"){
                            type = ItemSelectable.ItemType.NFTWaist;
                        }
                        if(itemClass == "NFTChest"){
                            type = ItemSelectable.ItemType.NFTChest;
                        }
                        if(itemClass == "NFTShoulders"){
                            type = ItemSelectable.ItemType.NFTShoulders;
                        }
                        if(itemClass == "NFTArms"){
                            type = ItemSelectable.ItemType.NFTArms;
                        }
                        if(itemClass == "NFTWrists"){
                            type = ItemSelectable.ItemType.NFTWrists;
                        }
                        if(itemClass == "NFTLeggings"){
                            type = ItemSelectable.ItemType.NFTLeggings;
                        }
                        if(itemClass == "NFTHands"){
                            type = ItemSelectable.ItemType.NFTHands;
                        }
                        if(itemClass == "NFTFeet"){
                            type = ItemSelectable.ItemType.NFTFeet;
                        }
                        if(itemClass == "NFTRing"){
                            type = ItemSelectable.ItemType.NFTRing;
                        }
                        if(itemClass == "NFTSingleHandedWeapon"){
                            type = ItemSelectable.ItemType.NFTSingleHandedWeapon;
                        }
                        if(itemClass == "NFTTwoHandedWeapon"){
                            type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
                        }
                        if(itemClass == "NFTOffHand"){
                            type = ItemSelectable.ItemType.NFTOffHand;
                        }
                        if(itemClass == "NFTShield"){
                            type = ItemSelectable.ItemType.NFTShield;
                        }
                        if(itemClass == "TacticianHead"){
                            type = ItemSelectable.ItemType.TacticianHead;
                        }
                        if(itemClass == "TacticianEarring"){
                            type = ItemSelectable.ItemType.TacticianEarring;
                        }
                        if(itemClass == "TacticianNecklace"){
                            type = ItemSelectable.ItemType.TacticianNecklace;
                        }
                        if(itemClass == "TacticianWaist"){
                            type = ItemSelectable.ItemType.TacticianWaist;
                        }
                        if(itemClass == "TacticianChest"){
                            type = ItemSelectable.ItemType.TacticianChest;
                        }
                        if(itemClass == "TacticianShoulders"){
                            type = ItemSelectable.ItemType.TacticianShoulders;
                        }
                        if(itemClass == "TacticianArms"){
                            type = ItemSelectable.ItemType.TacticianArms;
                        }
                        if(itemClass == "TacticianWrists"){
                            type = ItemSelectable.ItemType.TacticianWrists;
                        }
                        if(itemClass == "TacticianLeggings"){
                            type = ItemSelectable.ItemType.TacticianLeggings;
                        }
                        if(itemClass == "TacticianHands"){
                            type = ItemSelectable.ItemType.TacticianHands;
                        }
                        if(itemClass == "TacticianFeet"){
                            type = ItemSelectable.ItemType.TacticianFeet;
                        }
                        if(itemClass == "TacticianRing"){
                            type = ItemSelectable.ItemType.TacticianRing;
                        }
                        if(itemClass == "TacticianSingleHandedWeapon"){
                            type = ItemSelectable.ItemType.TacticianSingleHandedWeapon;
                        }
                        if(itemClass == "TacticianTwoHandedWeapon"){
                            type = ItemSelectable.ItemType.TacticianTwoHandedWeapon;
                        }
                        if(itemClass == "TacticianOffHand"){
                            type = ItemSelectable.ItemType.TacticianOffHand;
                        }
                        if(itemClass == "TacticianShield"){
                            type = ItemSelectable.ItemType.TacticianShield;
                        }
                        if(itemClass == "NFTTacticianHead"){
                            type = ItemSelectable.ItemType.NFTTacticianHead;
                        }
                        if(itemClass == "NFTTacticianEarring"){
                            type = ItemSelectable.ItemType.NFTTacticianEarring;
                        }
                        if(itemClass == "NFTTacticianNecklace"){
                            type = ItemSelectable.ItemType.NFTTacticianNecklace;
                        }
                        if(itemClass == "NFTTacticianWaist"){
                            type = ItemSelectable.ItemType.NFTTacticianWaist;
                        }
                        if(itemClass == "NFTTacticianChest"){
                            type = ItemSelectable.ItemType.NFTTacticianChest;
                        }
                        if(itemClass == "NFTTacticianShoulders"){
                            type = ItemSelectable.ItemType.NFTTacticianShoulders;
                        }
                        if(itemClass == "NFTTacticianArms"){
                            type = ItemSelectable.ItemType.NFTTacticianArms;
                        }
                        if(itemClass == "NFTTacticianWrists"){
                            type = ItemSelectable.ItemType.NFTTacticianWrists;
                        }
                        if(itemClass == "NFTTacticianLeggings"){
                            type = ItemSelectable.ItemType.NFTTacticianLeggings;
                        }
                        if(itemClass == "NFTTacticianHands"){
                            type = ItemSelectable.ItemType.NFTTacticianHands;
                        }
                        if(itemClass == "NFTTacticianFeet"){
                            type = ItemSelectable.ItemType.NFTTacticianFeet;
                        }
                        if(itemClass == "NFTTacticianRing"){
                            type = ItemSelectable.ItemType.NFTTacticianRing;
                        }
                        if(itemClass == "NFTTacticianSingleHandedWeapon"){
                            type = ItemSelectable.ItemType.NFTTacticianSingleHandedWeapon;
                        }
                        if(itemClass == "NFTTacticianTwoHandedWeapon"){
                            type = ItemSelectable.ItemType.NFTTacticianTwoHandedWeapon;
                        }
                        if(itemClass == "NFTTacticianOffHand"){
                            type = ItemSelectable.ItemType.NFTTacticianOffHand;
                        }
                        if(itemClass == "NFTTacticianShield"){
                            type = ItemSelectable.ItemType.NFTTacticianShield;
                        }
                        //int quantity = 1;
                        //if(quant.HasValue && quant != null){
                        //    quantity = quant.Value;
                        //}
                        bool magiceffect = false;
                        if(nonFungibleToken){
                            magiceffect = true;
                        }
                        string ItemDescript = GetItemDescription(Item_Name);
                        //float weightCal = float.Parse(Weight) * quantity;
                        //Weight = weightCal.ToString();
                        ItemSelectable itemAdded = new ItemSelectable{
                            itemType = type, amount = quantity,  Weight = Weight, 
                            STRENGTH_item = STRENGTH_item, AGILITY_item = AGILITY_item,
                            FORTITUDE_item = FORTITUDE_item, ARCANA_item = ARCANA_item,
                            MagicResist_item = MagicResist_item, FireResist_item = FireResist_item, 
                            ColdResist_item = ColdResist_item, PoisonResist_item = PoisonResist_item,
                            DiseaseResist_item = DiseaseResist_item, Rarity_item = Rarity_item,
                            Item_Name = Item_Name, Durability = Durability_item, DurabilityMax = DurabilityMax_item,
                            Parry = Parry_item, Penetration = Penetration_item, AttackDelay = AttackDelay_item,
                            BlockChance = BlockChance_item, BlockValue = BlockValue_item, 
                            ItemSpecificClass = TypeOfItem, itemSlot = Item_Slot, Armor_item = Armor,
                            OwnerID = OWNERID, InstanceID = inventory[i].ItemInstanceId,
                            DamageMin = DamageMin_item, DamageMax = DamageMax_item, itemID = itemID,
                            TacticianBelt = tBelt, OGTacticianBelt = tBelt, TacticianInventory = tInv, OGTacticianInventory = tInv, TacticianEquip = tEquip,
                            TacticianStash = tStash, OGTacticianStash = tStash, NFT = nonFungibleToken, MagicalEffectActive = magiceffect,
                            ItemDescription = ItemDescript, EQUIPPEDSLOT = equippedslot, Quality_item = Quality_item, customID = Guid.NewGuid().ToString(), NFTID = NFTID
                        };
                        TacticianItems.Add(itemAdded);
                    }  
                }
                //print("Building tacticianFullDataMessage");
                TacticianExtractor tactData = ParseClientData(playerData.TactBuildString);

                List<CharacterInventoryListItem> stash = new List<CharacterInventoryListItem>();
                List<CharacterInventoryListItem> tactInventory = new List<CharacterInventoryListItem>();
                foreach(var item in TacticianItems){
                    CharacterInventoryListItem newListItem = (new CharacterInventoryListItem { Key = item.GetInstanceID(), Value = item });
                    if(item.GetTacticianStash()){
                        stash.Add(newListItem);
                    } else {
                        tactInventory.Add(newListItem);
                    }
                }
                List<CharacterStatListItem> tacticianStats = new List<CharacterStatListItem>();
                CharacterStatListItem newLevelStat = (new CharacterStatListItem { Key = "LVL", Value = playerData.PlayerLevel });
                tacticianStats.Add(newLevelStat);
                CharacterStatListItem newEXPStat = (new CharacterStatListItem { Key = "EXP", Value = playerData.PlayerExperience });
                tacticianStats.Add(newEXPStat);
                CharacterStatListItem spriteStat = (new CharacterStatListItem { Key = "spriteTactician", Value = playerData.PlayerSprite });
                tacticianStats.Add(spriteStat);
                CharacterStatListItem newNameStat = (new CharacterStatListItem { Key = "TacticianName", Value = playerData.PlayerName });
                tacticianStats.Add(newNameStat);
                CharacterStatListItem GiantRepStat = (new CharacterStatListItem { Key = "GiantRep", Value = tactData.GiantRep});
                tacticianStats.Add(GiantRepStat);
                CharacterStatListItem DragonRepStat = (new CharacterStatListItem { Key = "DragonRep", Value = tactData.DragonRep });
                tacticianStats.Add(DragonRepStat);
                CharacterStatListItem LizardRepStat = (new CharacterStatListItem { Key = "LizardRep", Value = tactData.LizardRep });
                tacticianStats.Add(LizardRepStat);
                CharacterStatListItem OrcRepStat = (new CharacterStatListItem { Key = "OrcRep", Value = tactData.OrcRep });
                tacticianStats.Add(OrcRepStat);
                CharacterStatListItem FaerieStat = (new CharacterStatListItem { Key = "FaerieRep", Value = tactData.FaerieRep });
                tacticianStats.Add(FaerieStat);
                CharacterStatListItem ElfRepStat = (new CharacterStatListItem { Key = "ElfRep", Value = tactData.ElvesRep });
                tacticianStats.Add(ElfRepStat);
                CharacterStatListItem DwarfRepStat = (new CharacterStatListItem { Key = "DwarfRep", Value = tactData.DwarvesRep });
                tacticianStats.Add(DwarfRepStat);
                CharacterStatListItem GnomeRepStat = (new CharacterStatListItem { Key = "GnomeRep", Value = tactData.GnomesRep });
                tacticianStats.Add(GnomeRepStat);
                string dkpcd = tactData.DKPCooldown;
                if(tactData.DKPCooldown != "0"){
                    DateTime cdTime = DateTime.Parse(tactData.DKPCooldown);
                    DateTime timeNow = DateTime.Now;
                    TimeSpan timeSpan = timeNow - cdTime;
                    if(timeSpan.TotalHours >= 24){
                        dkpcd = "0";
                    }
                }
                float xrpfloat = float.Parse(playerData.XRP);
                float dkpfloat = float.Parse(playerData.DKP);

                TacticianFullDataMessage TacticianInfoSheet = (new TacticianFullDataMessage{
                    XRPBalance = xrpfloat.ToString("F2"),
                    DKPBalance = dkpfloat.ToString("F2"),
                    Address = playerData.XRPLPUBLIC,
                    StashInventoryData = stash,
                    TacticianInventoryData = tactInventory,
                    TacticianStatData = tacticianStats,
                    StrengthBonus = tactData.BonusStatStrength,
                    AgilityBonus = tactData.BonusStatAgility,
                    ArcanaBonus = tactData.BonusStatArcana,
                    FortitudeBonus = tactData.BonusStatFortitude,
                    ArmorBonus = tactData.BonusStatArmor,
                    EyeColor = tactData.EyeColor,
                    BodyStyle = tactData.BodyStyle,
                    Birthdate = tactData.BirthDate,
                    DKPCooldown = dkpcd,
                    tactString = playerData.PlayerSprite
                });
                ////print("Calling GetBalances");
                //StartCoroutine(GetBalances(nconn, playerData, tokens, TacticianInfoSheet, gold));
                StartCoroutine(GetCharacterList(nconn, playerData, tokens, TacticianInfoSheet, gold, NFTDictionary)); 

                
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        public TacticianExtractor ParseClientData(string rawData)
        {
            string[] dataElements = rawData.Split('_');

            if (dataElements.Length != 18)
            {
                // Handle error - the rawData format is incorrect
                return null;
            }

            TacticianExtractor clientData = new TacticianExtractor
            {
                BirthDate = dataElements[0],
                ZodiacSign = dataElements[1],
                EyeColor = dataElements[2],
                BodyStyle = dataElements[3],
                BonusStatStrength = dataElements[4],
                BonusStatAgility = dataElements[5],
                BonusStatFortitude = dataElements[6],
                BonusStatArcana = dataElements[7],
                BonusStatArmor = dataElements[8],
                GiantRep = dataElements[9],
                DragonRep = dataElements[10],
                LizardRep = dataElements[11],
                OrcRep = dataElements[12],
                FaerieRep = dataElements[13],
                ElvesRep = dataElements[14],
                DwarvesRep = dataElements[15],
                GnomesRep = dataElements[16],
                DKPCooldown = dataElements[17]
            };

            return clientData;
        }
/*

        #if UNITY_SERVER || UNITY_EDITOR

    private IEnumerator GetBalances(NetworkConnectionToClient nconn, PlayerInfo playerData, int tokenCount, TacticianFullDataMessage TacticianInfoSheet, int gold)
    {
        //use PlayfabID to fetch our wallet from playfab
        string address = "";
        string url = $"https://data.ripple.com/v2/accounts/{address}/balances";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            XRPLResponse response = JsonUtility.FromJson<XRPLResponse>(jsonResponse);

            string XRP = "";
            string DKP = "";

            foreach (Balance balance in response.balances)
            {
                if (balance.currency == "XRP")
                {
                    XRP = balance.value;
                }
                else if (balance.currency == "DKP")
                {
                    DKP = balance.value;
                }
            }
            TacticianFullDataMessage updatedTacticianInfoSheet = TacticianInfoSheet;
            updatedTacticianInfoSheet.XRPBalance = XRP;
            updatedTacticianInfoSheet.DKPBalance = DKP;
            StartCoroutine(GetCharacterList(nconn, playerData, tokenCount, updatedTacticianInfoSheet, gold)); 
            // You can now parse the response JSON to get the balances.
            // Consider using a JSON library like SimpleJSON or JsonUtility to parse the response.
        }
    }
        #endif
     */
        #if UNITY_SERVER || UNITY_EDITOR


    IEnumerator GetCharacterList(NetworkConnectionToClient nconn, PlayerInfo playerData, int tokenCount, TacticianFullDataMessage TacticianInfoSheet, int gold, Dictionary<string, string> NFTsInWallet){
        //print("Calling GetCharacterList");
        var charactersReady = false;
        List<CharacterResult> characters = null;
        PlayFabServerAPI.GetAllUsersCharacters(new ListUsersCharactersRequest
        {
            PlayFabId = playerData.PlayFabId
        }, result =>
        {
            characters = result.Characters;
            charactersReady = true;
        }, error =>
        {
            Debug.Log(error.ErrorMessage);
        });
        yield return new WaitUntil(() => charactersReady);
        GameObject player = Instantiate(playerPrefab);
        player.name = $"{playerPrefab.name} [connId={nconn.connectionId}]";
        NetworkServer.AddPlayerForConnection(nconn, player);
        ScenePlayer p = player.GetComponent<ScenePlayer>();
       // p.TargetToggleSprite(false, p.loadSprite);
        nconn.authenticationData = playerData;
        bool townBound = false;
        List<string> CharacterList = new List<string>();
        bool deletingCharacters = false;
        var DeletionReady = false;
        List<string> DeletionList = new List<string>();
        if(characters == null || characters.Count < 1){
            townBound = true;
        } else {
            //print("Building charList ");

            for (int i = 0; i < characters.Count; i++)
            {
                var characterDataReady = false;
                Dictionary<string, UserDataRecord> characterData = null;
                PlayFabServerAPI.GetCharacterInternalData(new GetCharacterDataRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    CharacterId = characters[i].CharacterId
                }, result =>
                {
                    characterData = result.Data;
                    characterDataReady = true;
                }, error =>
                {
                    Debug.Log(error.ErrorMessage);
                });
                yield return new WaitUntil(() => characterDataReady);
                yield return new WaitForSeconds(.1f);
                // Check if the character is inactive
                if (characterData.ContainsKey("ResetCharacter") && characterData["ResetCharacter"].Value == "R")
                {
                    deletingCharacters = true;
                    //print($"Skipping {characters[i].CharacterName} because it is inactive");
                    DeletionList.Add(characters[i].CharacterId);
                }
                else
                {
                    // The character is not inactive, so add it to the list
                    if (!CharacterList.Contains(characters[i].CharacterId))
                    {
                        CharacterList.Add(characters[i].CharacterId);
                        //print($"Adding {characters[i].CharacterName} to the list");
                    }
                }
            }
            if(playerData.PartyIDs == null || playerData.PartyIDs.Count < 1){
                townBound = true;
            }
        }
        //print("Calling ProcessCharacterList");
        if(deletingCharacters){
            for(int j = 0; j < DeletionList.Count; j++){
                var characterDeletionReady = false;

                PlayFabServerAPI.DeleteCharacterFromUser(new DeleteCharacterFromUserRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    CharacterId = DeletionList[j],
                    SaveCharacterInventory = false // Set this to false to not save character's inventory
                }, result =>
                {
                    //print($"Character {DeletionList[j]} has been deleted successfully.");
                    characterDeletionReady = true;
                }, error =>
                {
                    Debug.Log(error.ErrorMessage);
                });

                yield return new WaitUntil(() => characterDeletionReady);
                yield return new WaitForSeconds(.1f);

            }
            DeletionReady = true;
        } else {
            DeletionReady = true;
        }
        yield return new WaitUntil(() => DeletionReady);
        yield return new WaitForSeconds(.1f);

        StartCoroutine(ProcessCharacterList(nconn, playerData, CharacterList, townBound, tokenCount, TacticianInfoSheet, gold, NFTsInWallet));
    }
#endif

    //void SetUserWeight(NetworkConnectionToClient nconn, PlayerInfo playerData, string ID, string weight, int Tokens, Dictionary<string, ItemSelectable> TacticianItems, int gold, string BeltWeight){
    //    PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
    //        {
    //            PlayFabId = playerData.PlayFabId,
    //            Data = new Dictionary<string, string>
    //            {
    //                {"currentWeight", weight}
    //            }
    //        }, result =>{
    //            GetCharacterList(nconn, playerData, Tokens, TacticianItems, gold, weight, BeltWeight); 
    //        }, error =>{
    //            Debug.Log(error.ErrorMessage); 
    //            Debug.Log(error.ErrorDetails);
    //            Debug.Log(error.Error);
    //        });  
    //}
   /*
   void GetCharacterList(NetworkConnectionToClient nconn, PlayerInfo playerData, int tokenCount, TacticianFullDataMessage TacticianInfoSheet, int gold)
    {   
        //print("Calling GetCharacterList");

        PlayFabServerAPI.GetAllUsersCharacters(new ListUsersCharactersRequest
        {
            PlayFabId = playerData.PlayFabId
        }, result =>
        {
            GameObject player = Instantiate(playerPrefab);
            player.name = $"{playerPrefab.name} [connId={nconn.connectionId}]";
            NetworkServer.AddPlayerForConnection(nconn, player);
            ScenePlayer p = player.GetComponent<ScenePlayer>();
            p.TargetToggleSprite(false, p.loadSprite);
            //p.TacticianStatList.Add("currentWeight", weight);
            //p.TacticianStatList.Add("currentBelt", beltWeight);
            //p.TacticianStatList.Add("LVL", "3");
            //p.TacticianStatList.Add("EXP", "8800");
            //p.TacticianName = playerData.PlayerName;
            //foreach(var key in TacticianItems){
            //    PassLoginItemToStash(nconn, key.Value);
            //}
            nconn.authenticationData = playerData;
                
            bool townBound = false;
            List<string> CharacterList = new List<string>();
            List<CharacterResult> characters = result.Characters;
            if(result.Characters == null){
                townBound = true;
            } else {
                if(characters.Count < 1){
                    townBound = true;
                }
            //print("Building charList ");

            for (int i = 0; i < characters.Count; i++)
                {
                    if(!CharacterList.Contains(characters[i].CharacterId)){
                        CharacterList.Add(characters[i].CharacterId);
                        //print($"Adding {characters[i].CharacterName} to the list");
                    }
                }
                if(playerData.PartyIDs == null ){
                    townBound = true;
                } else {
                    if(playerData.PartyIDs.Count < 1 || result.Characters.Count < 1){
                        townBound = true;
                    }
                }
            }
            //print("Calling ProcessCharacterList");
                
            StartCoroutine(ProcessCharacterList(nconn, playerData, CharacterList, townBound, tokenCount, TacticianInfoSheet, gold));
        }, error =>{
            Debug.Log(error.ErrorMessage);
        });
    }
    */
        #if UNITY_SERVER || UNITY_EDITOR

    IEnumerator ProcessCharacterList(NetworkConnectionToClient nconn, PlayerInfo playerData, List<string> CharacterList, bool townRidden, int tokenCount, TacticianFullDataMessage TacticianInfoSheet, int gold, Dictionary<string, string> NFTsInWallet){
            
        ScenePlayer p = nconn.identity.gameObject.GetComponent<ScenePlayer>();
        List<string> AddToPartyList = playerData.PartyIDs;
        Dictionary<string, CharacterFullDataMessage> Sheets = new Dictionary<string, CharacterFullDataMessage>();
        if(CharacterList.Count >= 1){
        foreach(var _key in CharacterList)
        {
            //print("Calling GetCharacterInternalData");
            var charactersReady = false;
            GetCharacterDataRequest request = new GetCharacterDataRequest();
            request.PlayFabId = playerData.PlayFabId;
            request.CharacterId = _key;
            PlayFabServerAPI.GetCharacterInternalData(request,
            result =>
            {   
                List<CharacterStatListItem> charStats = new List<CharacterStatListItem>();
                List<CharacterSpellListItem> charSpells = new List<CharacterSpellListItem>();
                List<CharacterCooldownListItem> charCooldowns = new List<CharacterCooldownListItem>();

                //bool leveledUpOverTime = false;
                //int lvl = 0;
                string charID = string.Empty;
                foreach(var data in result.Data){
                    //if(data.Key == "LEVELING"){
                    //    // Parse the end time from the string
                    //    DateTime endTime;
                    //    if(DateTime.TryParse(data.Value.Value, out endTime)){
                    //        // Compare the end time to the current time
                    //        if(endTime <= DateTime.UtcNow){
                    //            // Enough time has passed
                    //            leveledUpOverTime = true;
                    //        } else {
                    //            CharacterStatListItem LEVELING = (new CharacterStatListItem{
                    //                Key = data.Key,
                    //                Value = data.Value.Value
                    //            });
                    //            // Not enough time has passed
                    //        }
                    //    } else {
                    //        // Failed to parse the string as a DateTime object
                    //    }
                    //}
                    if(data.Key == "COOLDOWNQ"){
                        //data.Value.Value
                        SplitCombinedValue(data.Value.Value, out string value, out string spellnamefull);
                        CharacterCooldownListItem CooldownQ = (new CharacterCooldownListItem{
                            PKey = "COOLDOWNQ",
                            Value = value,
                            SpellnameFull = spellnamefull,
                            Position = "SPELLQ"
                        });
                        print($"{CooldownQ.PKey} is the Pkey and {CooldownQ.SpellnameFull} is the spellname");
                        charCooldowns.Add(CooldownQ);
                    }
                    if(data.Key == "COOLDOWNE"){
                        //data.Value.Value
                        SplitCombinedValue(data.Value.Value, out string value, out string spellnamefull);
                        CharacterCooldownListItem CooldownE = (new CharacterCooldownListItem{
                            PKey = "COOLDOWNE",
                            Value = value,
                            SpellnameFull = spellnamefull,
                            Position = "SPELLE"
                        });
                        print($"{CooldownE.PKey} is the Pkey and {CooldownE.SpellnameFull} is the spellname");
                        charCooldowns.Add(CooldownE);
                    }
                    if(data.Key == "COOLDOWNR"){
                        //data.Value.Value
                        SplitCombinedValue(data.Value.Value, out string value, out string spellnamefull);
                        CharacterCooldownListItem CooldownR = (new CharacterCooldownListItem{
                            PKey = "COOLDOWNR",
                            Value = value,
                            SpellnameFull = spellnamefull,
                            Position = "SPELLR"
                        });
                        print($"{CooldownR.PKey} is the Pkey and {CooldownR.SpellnameFull} is the spellname");
                        charCooldowns.Add(CooldownR);
                    }
                    if(data.Key == "COOLDOWNF"){
                        //data.Value.Value
                        SplitCombinedValue(data.Value.Value, out string value, out string spellnamefull);
                        CharacterCooldownListItem CooldownF = (new CharacterCooldownListItem{
                            PKey = "COOLDOWNF",
                            Value = value,
                            SpellnameFull = spellnamefull,
                            Position = "SPELLF"
                        });
                        print($"{CooldownF.PKey} is the Pkey and {CooldownF.SpellnameFull} is the spellname");
                        charCooldowns.Add(CooldownF);
                    }
                    if(data.Key == "DEATH"){
                        CharacterStatListItem DEATHStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(DEATHStat);
                    }
                    if(data.Key == "LVL"){
                        //lvl = int.Parse(data.Value.Value);
                        CharacterStatListItem LVLStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(LVLStat);
                    }
                    if(data.Key == "EXP"){
                        CharacterStatListItem EXPStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(EXPStat);
                    }
                    if(data.Key == "currentHP"){
                        CharacterStatListItem currentHPStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(currentHPStat);
                    }
                    if(data.Key == "currentMP"){
                        CharacterStatListItem currentMPStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(currentMPStat);
                    }
                    if(data.Key == "LEVELING"){
                        CharacterStatListItem LEVELINGStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(LEVELINGStat);
                    }
                    if(data.Key == "Class"){
                        CharacterStatListItem ClassStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(ClassStat);
                    }
                    if(data.Key == "CharName"){
                        CharacterStatListItem CharNameStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(CharNameStat);
                    }
                    if(data.Key == "CharacterID"){
                        CharacterStatListItem CharacterIDStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charID = data.Value.Value;
                        charStats.Add(CharacterIDStat);
                    }
                    if(data.Key == "CharacterSprite"){
                        CharacterStatListItem CharacterSpriteStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(CharacterSpriteStat);
                    }
                    if(data.Key == "CORE"){
                        CharacterStatListItem COREStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(COREStat);
                    }
                    if(data.Key == "ClassPoints"){
                        CharacterStatListItem ClassPointsStat = (new CharacterStatListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charStats.Add(ClassPointsStat);
                    }
                    if(data.Key == "SPELLQ"){
                        CharacterSpellListItem SPELLQSpell = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SPELLQSpell);
                    }
                    if(data.Key == "SPELLE"){
                        CharacterSpellListItem SPELLESpell = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SPELLESpell);
                    }
                    if(data.Key == "SPELLR"){
                        CharacterSpellListItem SPELLRSpell = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SPELLRSpell);
                    }
                    if(data.Key == "SPELLF"){
                        CharacterSpellListItem SPELLFSpell = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SPELLFSpell);
                    }
                    if(data.Key == "StartClassSkill"){
                        CharacterSpellListItem StartClassSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(StartClassSkill);
                    }
                    if(data.Key == "WestT1Skill"){
                        CharacterSpellListItem WestT1Skill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(WestT1Skill);
                    }
                    if(data.Key == "WestT2TopSkill"){
                        CharacterSpellListItem WestT2TopSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(WestT2TopSkill);
                    }
                    if(data.Key == "WestT2MiddleSkill"){
                        CharacterSpellListItem WestT2Middle = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(WestT2Middle);
                    }
                    if(data.Key == "WestT2BottomSkill"){
                        CharacterSpellListItem WestT2BottomSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(WestT2BottomSkill);
                    }
                    if(data.Key == "WestT3TopSkill"){
                        CharacterSpellListItem WestT3TopSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(WestT3TopSkill);
                    }
                    if(data.Key == "WestT3BottomSkill"){
                        CharacterSpellListItem WestT3BottomSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(WestT3BottomSkill);
                    }
                    if(data.Key == "WestT3EndSkill"){
                        CharacterSpellListItem WestT3EndSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(WestT3EndSkill);
                    }
                    if(data.Key == "EastT1Skill"){
                        CharacterSpellListItem EastT1Skill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(EastT1Skill);
                    }
                    if(data.Key == "EastT2TopSkill"){
                        CharacterSpellListItem EastT2TopSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(EastT2TopSkill);
                    }
                    if(data.Key == "EastT2MiddleSkill"){
                        CharacterSpellListItem EastT2MiddleSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(EastT2MiddleSkill);
                    }
                    if(data.Key == "EastT2BottomSkill"){
                        CharacterSpellListItem EastT2BottomSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(EastT2BottomSkill);
                    }
                    if(data.Key == "EastT3TopSkill"){
                        CharacterSpellListItem EastT3TopSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(EastT3TopSkill);
                    }
                    if(data.Key == "EastT3BottomSkill"){
                        CharacterSpellListItem EastT3BottomSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(EastT3BottomSkill);
                    }
                    if(data.Key == "EastT3EndSkill"){
                        CharacterSpellListItem EastT3EndSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(EastT3EndSkill);
                    }
                    if(data.Key == "SouthT1Skill"){
                        CharacterSpellListItem SouthT1Skill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SouthT1Skill);
                    }
                    if(data.Key == "SouthT2LeftSkill"){
                        CharacterSpellListItem SouthT2LeftSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SouthT2LeftSkill);
                    }
                    if(data.Key == "SouthT2MiddleSkill"){
                        CharacterSpellListItem SouthT2MiddleSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SouthT2MiddleSkill);
                    }
                    if(data.Key == "SouthT2RightSkill"){
                        CharacterSpellListItem SouthT2RightSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SouthT2RightSkill);
                    }
                    if(data.Key == "SouthT3LeftSkill"){
                        CharacterSpellListItem SouthT3LeftSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SouthT3LeftSkill);
                    }
                    if(data.Key == "SouthT3RightSkill"){
                        CharacterSpellListItem SouthT3RightSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SouthT3RightSkill);
                    }
                    if(data.Key == "SouthT3EndSkill"){
                        CharacterSpellListItem SouthT3EndSkill = (new CharacterSpellListItem{
                            Key = data.Key,
                            Value = data.Value.Value
                        });
                        charSpells.Add(SouthT3EndSkill);
                    }
                }
                if(charCooldowns.Count > 0){
                    List<CharacterCooldownListItem> keysToRemove = new List<CharacterCooldownListItem>();
                    Dictionary<string, string> cooldownData = new Dictionary<string, string>();
                    foreach(var cd in charCooldowns){
                        DateTime endTime;
                        if (DateTime.TryParse(cd.Value, out endTime))
                        {
                            if (DateTime.Now >= endTime)
                            {
                                // Cooldown has expired
                                // Add the key to the list of keys to remove
                                keysToRemove.Add(cd);
                                cooldownData.Add(cd.PKey, null); // If you want to add null to the cooldownData
                            }
                            // else, Cooldown has not expired, continue with the old date
                        }
                        else
                        {
                            // Handle error in parsing the date
                        }
                    }
                    if(cooldownData.Count > 0){
                        PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest {
                            PlayFabId = playerData.PlayFabId,
                            CharacterId = charID,
                            Data = cooldownData
                        }, result =>{
                        }, error =>{
                            Debug.Log(error.ErrorMessage); 
                            Debug.Log(error.ErrorDetails);
                            Debug.Log(error.Error);
                        });
                        foreach (var key in keysToRemove)
                        {
                            charCooldowns.Remove(key);
                        }
                    }
                }
                //if(leveledUpOverTime){
                //    CharacterStatListItem OldLevel = new CharacterStatListItem();
                //    foreach(var stat in charStats){
                //        if(stat.Key == "LVL"){
                //            OldLevel = stat; 
                //        }
                //    }
                //    charStats.Remove(OldLevel);
                //    lvl ++;
                //    CharacterStatListItem LVLStat = (new CharacterStatListItem{
                //        Key = "LVL",
                //        Value = lvl.ToString()
                //    });
                //    charStats.Add(LVLStat);
                //    PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
                //    {
                //        PlayFabId = playerData.PlayFabId,
                //        CharacterId = charID,
                //        Data = new Dictionary<string, string>
                //        {
                //            {"LVL", lvl.ToString()},
                //            {"TimeStampLVL", null}
                //        }
                //    }, result =>
                //    {
                //    }, error =>{
                //        Debug.Log(error.ErrorMessage); 
                //        Debug.Log(error.ErrorDetails);
                //        Debug.Log(error.Error);
                //    });
                //}
                CharacterFullDataMessage charPage = (new CharacterFullDataMessage{
                    CharacterID = _key,
                    CharSpellData = charSpells,
                    CharStatData = charStats,
                    CharCooldownData = charCooldowns
                });

                if (!Sheets.ContainsKey(_key)){
                    Sheets.Add(_key, charPage);
                } else {
                    Debug.LogWarning("Key already exists in Sheets dictionary: " + _key);
                }
                charactersReady = true;
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            yield return new WaitUntil(() => charactersReady);
        }
        }


        string sceneSelect = playerData.CurrentScene;
        if(townRidden){
            sceneSelect = TOWNOFARUDINE;
        }
        p.SetPlayerData(playerData);
        p.TokenCounted(tokenCount);
            //print("Calling GetAllCharacterInventory");
        //update their cooldown stuff in playfab from here
        StartCoroutine(GetAllCharacterInventory(nconn, playerData, townRidden, tokenCount, gold, TacticianInfoSheet, Sheets, NFTsInWallet));
    }
        #endif
        #if UNITY_SERVER || UNITY_EDITOR

    IEnumerator GetAllCharacterInventory(NetworkConnectionToClient nconn, PlayerInfo playerData, bool townRidden, int tokenCount, int gold, TacticianFullDataMessage TacticianInfoSheet, Dictionary<string, CharacterFullDataMessage> Sheets, Dictionary<string, string> NFTsInWallet){
        ScenePlayer p = nconn.identity.gameObject.GetComponent<ScenePlayer>();
        //print($"{gold} is the amount of gold pulled in from user inventory");
        Dictionary<string, List<CharacterInventoryListItem>> Inventories = new Dictionary<string, List<CharacterInventoryListItem>>();
        int inventoriesRetrieved = 0;
        if(Sheets.Count > 0){
            foreach(var key in Sheets){
                List<CharacterInventoryListItem> chararacterInventory = new List<CharacterInventoryListItem>();
                GetCharacterInventoryRequest request = new GetCharacterInventoryRequest();
                request.PlayFabId = playerData.PlayFabId;
                request.CharacterId = key.Key;
                bool charInventoryComp = false;
                PlayFabServerAPI.GetCharacterInventory(request, result => {
                    if(result.Inventory != null){
                        List<ItemInstance> inventory = result.Inventory;
                        if(inventory.Count > 0 && inventory != null){
                            // make the stackable list here to spawn them in one big item to move then we can add to it later
                            for (int e = 0; e < inventory.Count; e++)
                            {
                                string Weight = "0";
                                string STRENGTH_item = "0";
                                string AGILITY_item = "0";
                                string FORTITUDE_item = "0";
                                string ARCANA_item = "0";
                                string Rarity_item = "0";
                                string MagicResist_item = "0";
                                string FireResist_item = "0";
                                string ColdResist_item = "0";
                                string PoisonResist_item = "0";
                                string DiseaseResist_item = "0";
                                string Armor = "0";
                                string Durability_item = "100";
                                string DurabilityMax_item = "100";
                                string DamageMin_item = "0";
                                string DamageMax_item = "0";
                                string Parry_item = "0";
                                string Penetration_item= "0";
                                string AttackDelay_item = "0";
                                string BlockChance_item = "0";
                                string BlockValue_item = "0";
                                string Quality_item = "Plain";
                                string TypeOfItem = null;
                                string Item_Slot = "Not equippable";
                                string Item_Name = null;
                                string equippedslot = "0";
                                bool equipped = false;
                                string Charinven = "0";
                                bool charInventory = true;
                                bool NFT = false;
                                string nftID = "NotNFT";
                                int quantity = 1;
                                //int? quant = inventory[e].RemainingUses;
                                foreach(var stat in inventory[e].CustomData){
                                    if(stat.Key == "NFT"){
                                        string NFTTag = null;
                                        NFTTag = stat.Value;
                                        if(NFTTag == "NFT"){
                                            NFT = true;
                                        }
                                    }
                                    if(stat.Key == "Weight"){
                                        Weight = stat.Value;
                                    }
                                    if(stat.Key == "STRENGTH_item"){
                                        STRENGTH_item = stat.Value;
                                    }
                                    if(stat.Key == "AGILITY_item"){
                                        AGILITY_item = stat.Value;
                                    }
                                    if(stat.Key == "FORTITUDE_item"){
                                        FORTITUDE_item = stat.Value;
                                    }
                                    if(stat.Key == "ARCANA_item"){
                                        ARCANA_item = stat.Value;
                                    }
                                    if(stat.Key == "NFTID"){
                                        nftID = stat.Value;
                                    }
                                    if(stat.Key == "Rarity_item"){
                                        Rarity_item = stat.Value;
                                    }
                                    if(stat.Key == "MagicResist_item"){
                                        MagicResist_item = stat.Value;
                                    }
                                    if(stat.Key == "FireResist_item"){
                                        FireResist_item = stat.Value;
                                    }
                                    if(stat.Key == "ColdResist_item"){
                                        ColdResist_item = stat.Value;
                                    }
                                    if(stat.Key == "PoisonResist_item"){
                                        PoisonResist_item = stat.Value;
                                    }
                                    if(stat.Key == "DiseaseResist_item"){
                                        DiseaseResist_item = stat.Value;
                                    }
                                    if(stat.Key == "Armor_item"){
                                        Armor = stat.Value;
                                    }
                                    if(stat.Key == "Durability_item"){
                                        Durability_item = stat.Value;
                                    }
                                    if(stat.Key == "DamageMin_item"){
                                        DamageMin_item = stat.Value;
                                    }
                                    if(stat.Key == "DamageMax_item"){
                                        DamageMax_item = stat.Value;
                                    }
                                    if(stat.Key == "Parry_item"){
                                        Parry_item = stat.Value;
                                    }
                                    if(stat.Key == "AttackDelay_item"){
                                        AttackDelay_item = stat.Value;
                                    }
                                    if(stat.Key == "Penetration_item"){
                                        Penetration_item = stat.Value;
                                    }
                                    if(stat.Key == "BlockValue_item"){
                                        BlockValue_item = stat.Value;
                                    }
                                    if(stat.Key == "BlockChance_item"){
                                        BlockChance_item = stat.Value;
                                    }
                                    if(stat.Key == "Quality_item"){
                                        Quality_item = stat.Value;
                                    }
                                    if(stat.Key == "TypeOfItem"){
                                        TypeOfItem = stat.Value;
                                    }
                                    if(stat.Key == "Item_Slot"){
                                        Item_Slot = stat.Value;
                                    }
                                    if(stat.Key == "Item_Name"){
                                        Item_Name = stat.Value;
                                    }
                                    if(stat.Key == "Amount"){
                                        quantity = int.Parse(stat.Value);
                                    }
                                    if(stat.Key == "EquippedSlot"){
                                        equippedslot = stat.Value;
                                        equipped = true;
                                        charInventory = false;
                                    }
                                }
                                //print($"character {key.Key} had an inventory for sure, building item {Item_Name}");
                                ItemSelectable.ItemType type = ItemSelectable.ItemType.None;
                                string itemClass = inventory[e].ItemClass;
                                string itemID = inventory[e].ItemId;
                                if(nftID != "NotNFT"){
                                    NFTsInWallet.Add(itemID, nftID);
                                }
                                //print($"{itemName} was added to {result.CharacterId}");
                                if(itemClass == "Misc"){
                                    type = ItemSelectable.ItemType.Misc;
                                }
                                if(itemClass == "GemstoneT2"){
                                    type = ItemSelectable.ItemType.GemstoneT2;
                                }
                                if(itemClass == "GemstoneT3"){
                                    type = ItemSelectable.ItemType.GemstoneT3;
                                }
                                if(itemClass == "GemstoneT4"){
                                    type = ItemSelectable.ItemType.GemstoneT4;
                                }
                                if(itemClass == "GemstoneT5"){
                                    type = ItemSelectable.ItemType.GemstoneT5;
                                }
                                if(itemClass == "RefinedMaterialT1"){
                                    type = ItemSelectable.ItemType.RefinedMaterialT1;
                                }
                                if(itemClass == "MaterialT1"){
                                    type = ItemSelectable.ItemType.MaterialT1;
                                }
                                if(itemClass == "ResourceT1"){
                                    type = ItemSelectable.ItemType.ResourceT1;
                                }
                                if(itemClass == "FoodT1"){
                                    type = ItemSelectable.ItemType.FoodT1;
                                }
                                if(itemClass == "GrownT1"){
                                    type = ItemSelectable.ItemType.GrownT1;
                                }
                                if(itemClass == "PotionT1"){
                                    type = ItemSelectable.ItemType.PotionT1;
                                }
                                if(itemClass == "PatternT2"){
                                    type = ItemSelectable.ItemType.PatternT2;
                                }
                                if(itemClass == "PatternT3"){
                                    type = ItemSelectable.ItemType.PatternT3;
                                }
                                if(itemClass == "PatternT4"){
                                    type = ItemSelectable.ItemType.PatternT4;
                                }
                                if(itemClass == "PatternT5"){
                                    type = ItemSelectable.ItemType.PatternT5;
                                }
                                if(itemClass == "Tool"){
                                    type = ItemSelectable.ItemType.Tool;
                                }
                                if(itemClass == "Ammo"){
                                    type = ItemSelectable.ItemType.Ammo;
                                }
                                if(itemClass == "Head"){
                                    type = ItemSelectable.ItemType.Head;
                                }
                                if(itemClass == "Earring"){
                                    type = ItemSelectable.ItemType.Earring;
                                }
                                if(itemClass == "Necklace"){
                                    type = ItemSelectable.ItemType.Necklace;
                                }
                                if(itemClass == "Waist"){
                                    type = ItemSelectable.ItemType.Waist;
                                }
                                if(itemClass == "Chest"){
                                    type = ItemSelectable.ItemType.Chest;
                                }
                                if(itemClass == "Shoulders"){
                                    type = ItemSelectable.ItemType.Shoulders;
                                }
                                if(itemClass == "Arms"){
                                    type = ItemSelectable.ItemType.Arms;
                                }
                                if(itemClass == "Wrists"){
                                    type = ItemSelectable.ItemType.Wrists;
                                }
                                if(itemClass == "Leggings"){
                                    type = ItemSelectable.ItemType.Leggings;
                                }
                                if(itemClass == "Hands"){
                                    type = ItemSelectable.ItemType.Hands;
                                }
                                if(itemClass == "Feet"){
                                    type = ItemSelectable.ItemType.Feet;
                                }
                                if(itemClass == "Ring"){
                                    type = ItemSelectable.ItemType.Ring;
                                }
                                if(itemClass == "SingleHandedWeapon"){
                                    type = ItemSelectable.ItemType.SingleHandedWeapon;
                                }
                                if(itemClass == "TwoHandedWeapon"){
                                    type = ItemSelectable.ItemType.TwoHandedWeapon;
                                }
                                if(itemClass == "OffHand"){
                                    type = ItemSelectable.ItemType.OffHand;
                                }
                                if(itemClass == "Shield"){
                                    type = ItemSelectable.ItemType.Shield;
                                }
                                if(itemClass == "NFTHead"){
                                    type = ItemSelectable.ItemType.NFTHead;
                                }
                                if(itemClass == "NFTEarring"){
                                    type = ItemSelectable.ItemType.NFTEarring;
                                }
                                if(itemClass == "NFTNecklace"){
                                    type = ItemSelectable.ItemType.NFTNecklace;
                                }
                                if(itemClass == "NFTWaist"){
                                    type = ItemSelectable.ItemType.NFTWaist;
                                }
                                if(itemClass == "NFTChest"){
                                    type = ItemSelectable.ItemType.NFTChest;
                                }
                                if(itemClass == "NFTShoulders"){
                                    type = ItemSelectable.ItemType.NFTShoulders;
                                }
                                if(itemClass == "NFTArms"){
                                    type = ItemSelectable.ItemType.NFTArms;
                                }
                                if(itemClass == "NFTWrists"){
                                    type = ItemSelectable.ItemType.NFTWrists;
                                }
                                if(itemClass == "NFTLeggings"){
                                    type = ItemSelectable.ItemType.NFTLeggings;
                                }
                                if(itemClass == "NFTHands"){
                                    type = ItemSelectable.ItemType.NFTHands;
                                }
                                if(itemClass == "NFTFeet"){
                                    type = ItemSelectable.ItemType.NFTFeet;
                                }
                                if(itemClass == "NFTRing"){
                                    type = ItemSelectable.ItemType.NFTRing;
                                }
                                if(itemClass == "NFTSingleHandedWeapon"){
                                    type = ItemSelectable.ItemType.NFTSingleHandedWeapon;
                                }
                                if(itemClass == "NFTTwoHandedWeapon"){
                                    type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
                                }
                                if(itemClass == "NFTOffHand"){
                                    type = ItemSelectable.ItemType.NFTOffHand;
                                }
                                if(itemClass == "NFTShield"){
                                    type = ItemSelectable.ItemType.NFTShield;
                                }
                                if(itemClass == "TacticianHead"){
                                    type = ItemSelectable.ItemType.TacticianHead;
                                }
                                if(itemClass == "TacticianEarring"){
                                    type = ItemSelectable.ItemType.TacticianEarring;
                                }
                                if(itemClass == "TacticianNecklace"){
                                    type = ItemSelectable.ItemType.TacticianNecklace;
                                }
                                if(itemClass == "TacticianWaist"){
                                    type = ItemSelectable.ItemType.TacticianWaist;
                                }
                                if(itemClass == "TacticianChest"){
                                    type = ItemSelectable.ItemType.TacticianChest;
                                }
                                if(itemClass == "TacticianShoulders"){
                                    type = ItemSelectable.ItemType.TacticianShoulders;
                                }
                                if(itemClass == "TacticianArms"){
                                    type = ItemSelectable.ItemType.TacticianArms;
                                }
                                if(itemClass == "TacticianWrists"){
                                    type = ItemSelectable.ItemType.TacticianWrists;
                                }
                                if(itemClass == "TacticianLeggings"){
                                    type = ItemSelectable.ItemType.TacticianLeggings;
                                }
                                if(itemClass == "TacticianHands"){
                                    type = ItemSelectable.ItemType.TacticianHands;
                                }
                                if(itemClass == "TacticianFeet"){
                                    type = ItemSelectable.ItemType.TacticianFeet;
                                }
                                if(itemClass == "TacticianRing"){
                                    type = ItemSelectable.ItemType.TacticianRing;
                                }
                                if(itemClass == "TacticianSingleHandedWeapon"){
                                    type = ItemSelectable.ItemType.TacticianSingleHandedWeapon;
                                }
                                if(itemClass == "TacticianTwoHandedWeapon"){
                                    type = ItemSelectable.ItemType.TacticianTwoHandedWeapon;
                                }
                                if(itemClass == "TacticianOffHand"){
                                    type = ItemSelectable.ItemType.TacticianOffHand;
                                }
                                if(itemClass == "TacticianShield"){
                                    type = ItemSelectable.ItemType.TacticianShield;
                                }
                                if(itemClass == "NFTTacticianHead"){
                                    type = ItemSelectable.ItemType.NFTTacticianHead;
                                }
                                if(itemClass == "NFTTacticianEarring"){
                                    type = ItemSelectable.ItemType.NFTTacticianEarring;
                                }
                                if(itemClass == "NFTTacticianNecklace"){
                                    type = ItemSelectable.ItemType.NFTTacticianNecklace;
                                }
                                if(itemClass == "NFTTacticianWaist"){
                                    type = ItemSelectable.ItemType.NFTTacticianWaist;
                                }
                                if(itemClass == "NFTTacticianChest"){
                                    type = ItemSelectable.ItemType.NFTTacticianChest;
                                }
                                if(itemClass == "NFTTacticianShoulders"){
                                    type = ItemSelectable.ItemType.NFTTacticianShoulders;
                                }
                                if(itemClass == "NFTTacticianArms"){
                                    type = ItemSelectable.ItemType.NFTTacticianArms;
                                }
                                if(itemClass == "NFTTacticianWrists"){
                                    type = ItemSelectable.ItemType.NFTTacticianWrists;
                                }
                                if(itemClass == "NFTTacticianLeggings"){
                                    type = ItemSelectable.ItemType.NFTTacticianLeggings;
                                }
                                if(itemClass == "NFTTacticianHands"){
                                    type = ItemSelectable.ItemType.NFTTacticianHands;
                                }
                                if(itemClass == "NFTTacticianFeet"){
                                    type = ItemSelectable.ItemType.NFTTacticianFeet;
                                }
                                if(itemClass == "NFTTacticianRing"){
                                    type = ItemSelectable.ItemType.NFTTacticianRing;
                                }
                                if(itemClass == "NFTTacticianSingleHandedWeapon"){
                                    type = ItemSelectable.ItemType.NFTTacticianSingleHandedWeapon;
                                }
                                if(itemClass == "NFTTacticianTwoHandedWeapon"){
                                    type = ItemSelectable.ItemType.NFTTacticianTwoHandedWeapon;
                                }
                                if(itemClass == "NFTTacticianOffHand"){
                                    type = ItemSelectable.ItemType.NFTTacticianOffHand;
                                }
                                if(itemClass == "NFTTacticianShield"){
                                    type = ItemSelectable.ItemType.NFTTacticianShield;
                                }
                                bool magiceffect = false;
                                if(NFT){
                                    magiceffect = true;
                                }
                                string ItemDescript = GetItemDescription(Item_Name);
                                //float weightCal = float.Parse(Weight) * quantity;
                                // Weight = weightCal.ToString();
                                ItemSelectable itemAdded = new ItemSelectable{
                                    itemType = type, amount = quantity,  Weight = Weight, 
                                    STRENGTH_item = STRENGTH_item, AGILITY_item = AGILITY_item,
                                    FORTITUDE_item = FORTITUDE_item, ARCANA_item = ARCANA_item,
                                    MagicResist_item = MagicResist_item, FireResist_item = FireResist_item, 
                                    ColdResist_item = ColdResist_item, PoisonResist_item = PoisonResist_item,
                                    DiseaseResist_item = DiseaseResist_item, Rarity_item = Rarity_item,
                                    Item_Name = Item_Name, Durability = Durability_item, DurabilityMax = DurabilityMax_item,
                                    Parry = Parry_item, Penetration = Penetration_item, AttackDelay = AttackDelay_item,
                                    BlockChance = BlockChance_item, BlockValue = BlockValue_item, 
                                    ItemSpecificClass = TypeOfItem, itemSlot = Item_Slot, Armor_item = Armor,
                                    OwnerID = key.Key, InstanceID = inventory[e].ItemInstanceId, EQUIPPED = equipped,
                                    DamageMin = DamageMin_item, DamageMax = DamageMax_item, EQUIPPEDSLOT = equippedslot,
                                    INVENTORY = charInventory, NFT = NFT, MagicalEffectActive = magiceffect, ItemDescription = ItemDescript,
                                    Quality_item = Quality_item, customID = Guid.NewGuid().ToString(), itemID = itemID, NFTID = nftID
                                };
                                /*

                                ItemSelectable itemAdded = new ItemSelectable();
                                itemAdded.SetItemType(type);
                                itemAdded.SetAmount(quantity);
                                itemAdded.SetWeight(Weight);
                                itemAdded.SetSTRENGTH_item(STRENGTH_item);
                                itemAdded.SetAGILITY_item(AGILITY_item);
                                itemAdded.SetFORTITUDE_item(FORTITUDE_item);
                                itemAdded.SetARCANA_item(ARCANA_item);
                                itemAdded.SetMagicResist_item(MagicResist_item);
                                itemAdded.SetFireResist_item(FireResist_item);
                                itemAdded.SetColdResist_item(ColdResist_item);
                                itemAdded.SetPoisonResist_item(PoisonResist_item);
                                itemAdded.SetDiseaseResist_item(DiseaseResist_item);
                                itemAdded.SetRarity_item(Rarity_item);
                                itemAdded.SetItemName(Item_Name);
                                itemAdded.SetDurability(Durability_item);
                                itemAdded.SetDurabilityMax(DurabilityMax_item);
                                itemAdded.SetParry(Parry_item);
                                itemAdded.SetPenetration(Penetration_item);
                                itemAdded.SetAttackDelay(AttackDelay_item);
                                itemAdded.SetBlockChance(BlockChance_item);
                                itemAdded.SetBlockValue(BlockValue_item);
                                itemAdded.SetItemSpecificClass(TypeOfItem);
                                itemAdded.SetItemSlot(Item_Slot);
                                itemAdded.SetArmor_item(Armor);
                                itemAdded.SetOwnerID(key.Key);
                                itemAdded.SetInstanceID(inventory[e].ItemInstanceId);
                                itemAdded.SetEQUIPPED(equipped);
                                itemAdded.SetDamageMin(DamageMin_item);
                                itemAdded.SetDamageMax(DamageMax_item);
                                itemAdded.SetEQUIPPEDSLOT(equippedslot);
                                itemAdded.SetINVENTORY(charInventory);
                                itemAdded.SetNFT(NFT);
                                itemAdded.SetMagicalEffectActive(magiceffect);
                                itemAdded.SetItemDescription(ItemDescript);
                                */

                                CharacterInventoryListItem itemCreated = (new CharacterInventoryListItem{
                                    Key = itemAdded.GetInstanceID(),
                                    Value = itemAdded
                                });
                                chararacterInventory.Add(itemCreated);
                            } 
                        }
                    }
                    inventoriesRetrieved++;
                    charInventoryComp = true;
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
                while(!charInventoryComp){
                    yield return new WaitForSeconds(.1f);
                }
                Inventories.Add(key.Key, chararacterInventory);
            }
        }
            
        //print($"{playerData.CurrentScene} is playerData CurrentScene, {playerData.Energy} is playerData Energy");
        long goldcoins = 0;
        int coinAmount = checked((int)gold);
        if(coinAmount > 0 ){
          // goldcoins = unchecked((uint)coinAmount);
           goldcoins = (long)coinAmount;

        }
        if(goldcoins > 0){
            p.Gold = goldcoins;
            p.TargetWalletAwake();
            //print($"Goldcoins = {goldcoins}");
        }
        if(goldcoins <= 0){
            p.GoldAmountSet(0);
            //print($"Goldcoins = {goldcoins}");
        }
        List<string> keys = playerData.PartyIDs;
        foreach(var key in keys){
            p.AddPartyServer(key);
        }
        /*
        p.TargetOpenUI(playerData.CurrentScene);
        p.TargetCharge(playerData.Energy);
        //print("still good in Get all character inventory");
        ClientRequestLoadScene dummy = new ClientRequestLoadScene {
            oldScene = "Container",
            newScene = sceneSelect,
            node = playerData.SavedNode,
            login = true
        };
        GetCleanedSceneName(nconn, dummy);
        StartCoroutine(ReChargeEnergy(nconn));
        */
        while (inventoriesRetrieved < Sheets.Count)
        {
            yield return new WaitForSeconds(0.1f);
        }
        //print("AboutToSendSheets with inventories!!");
        List<CharacterFullDataMessage> characterSheets = new List<CharacterFullDataMessage>();
        foreach (var sheetKey in Sheets)
        {
            foreach (var inventoryKey in Inventories.Keys)
            {
                if (sheetKey.Value.CharacterID == inventoryKey)
                {
                    // Get the CharacterFullDataMessage object
                    CharacterFullDataMessage charData = (new CharacterFullDataMessage{
                        CharacterID = sheetKey.Value.CharacterID,
                        CharInventoryData = Inventories[inventoryKey],
                        CharSpellData = sheetKey.Value.CharSpellData,
                        CharStatData = sheetKey.Value.CharStatData
                    });
                    characterSheets.Add(charData);
                    // Update the Sheets dictionary with the modified CharacterFullDataMessage
                }
            }
        }
            
        StartCoroutine(CreateInformationSheetsOnPlayer(nconn, p, TacticianInfoSheet, characterSheets, playerData, townRidden, NFTsInWallet));
        //we have all the char inventories, char spells, and char stats and we have tactician items as well, lets bring it all in!
            
        //yield return new WaitForSeconds(2f);
            
       // p.SetPlayerData(playerData);
            
        //StartCoroutine(LoginSetup(nconn, dummy));   
    }
        #endif
    string GetItemDescription(string itemName){
        string description = "Under construction, will be adding this in very soon";
        return description;
    }

    /*
    IEnumerator CreateInfomrationSheetsOnPlayer(ScenePlayer sPlayer, TacticianFullDataMessage tactSheet, List<CharacterFullDataMessage> characterSheets){
        yield return new WaitForSeconds(2f);
        if(tactSheet.StashInventoryData.Count > 40){
            //Send in batches.

            List<CharacterInventoryListItem> stashItems = tactSheet.StashInventoryData;
            tactSheet.StashInventoryData = new List<CharacterInventoryListItem>();
            if(stashItems.Count > 40){
                for(int i = 0; i < 40; i++) {
                    tactSheet.StashInventoryData.Add(stashItems[i]);
                }
                stashItems.RemoveRange(0, 40); // Remove the first 40 items.
            }
            sPlayer.GetFullTacticianData(tactSheet);
            foreach(var item in stashItems){
                sPlayer.GetStashNewItem(item);
            }
        } else {
            sPlayer.GetFullTacticianData(tactSheet);
        }
        foreach(var sheet in characterSheets){
            sPlayer.GetFullCharacterData(sheet);
        }
    }
    */
            #if UNITY_SERVER || UNITY_EDITOR

    IEnumerator CreateInformationSheetsOnPlayer(NetworkConnectionToClient nconn, ScenePlayer sPlayer, TacticianFullDataMessage tactSheet, List<CharacterFullDataMessage> characterSheets, PlayerInfo playerData, bool townRidden, Dictionary<string, string> NFTsInWallet){
        Dictionary<string, string> currentNFTS = playerData.NFTMD5Hash.Zip(playerData.NFTIDS, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        //playerData.NFTIDS;
        if(currentNFTS.Count > 0){
            foreach(var nft in currentNFTS){
                print($"Detected nft key {nft.Key} // nft value {nft.Value} was the nft from detected nfts");
            }
        }
        if(NFTsInWallet != null){
            foreach(var nft in NFTsInWallet){
                print($"Playfab nft key {nft.Key} // nft value {nft.Value} was the nft from Playfab nfts");
            }
        }
        List<CharacterInventoryListItem> stashItems = tactSheet.StashInventoryData;
        int numBatches = (int) Math.Ceiling((double) stashItems.Count / 40); // Calculate the number of batches.
        // Split the stashItems into batches and send each batch separately.
        if(numBatches > 1){
            for (int i = 0; i < numBatches; i++) {
                int batchSize = Math.Min(40, stashItems.Count - i * 40); // The size of this batch will be 40 or the remaining number of items.
                List<CharacterInventoryListItem> batchItems = stashItems.GetRange(i * 40, batchSize);
                if(i == 0) {
                    // For the first batch, send it via GetFullTacticianData.
                    tactSheet.StashInventoryData = batchItems;
                    sPlayer.GetFullTacticianData(tactSheet);
                } else {
                    // For subsequent batches, send each item individually via GetStashNewItem.
                    sPlayer.GetStashNewItems(batchItems);
                    yield return new WaitForSeconds(.1f);
                    //foreach (var item in batchItems) {
                    //    sPlayer.GetStashNewItem(item);
                    //    yield return new WaitForSeconds(.1f);
                    //}
                }
            }
        } else {
            sPlayer.GetFullTacticianData(tactSheet);
        }
        yield return new WaitForSeconds(.1f);
        List<CharacterInventoryListItem> tactNFTsDeleting = new List<CharacterInventoryListItem>();
        foreach(var TactItem in sPlayer.GetTacticianSheet().TacticianInventoryData){
            if(TactItem.Value.NFT){
                string nftID = TactItem.Value.NFTID;
                if(!currentNFTS.ContainsValue(nftID)){
                    TactItem.Value.Deleted = true;
                    TactItem.Value.Changed = true;
                    tactNFTsDeleting.Add(TactItem);
                } else {
                    string keyToRemove = null;
                    foreach (var kvp in currentNFTS){
                        if (kvp.Value == nftID){
                            keyToRemove = kvp.Key;
                            break;
                        }
                    }
                    if (keyToRemove != null){
                        currentNFTS.Remove(keyToRemove);
                    }
                }
            }
        }
        if(tactNFTsDeleting.Count > 0){
            foreach(var TactNft in tactNFTsDeleting){
                sPlayer.GetTacticianNewItem(TactNft);
                yield return new WaitForSeconds(.1f);
            }
        }
        List<CharacterInventoryListItem> stashNFTsDeleting = new List<CharacterInventoryListItem>();
        foreach(var StashItem in sPlayer.GetTacticianSheet().StashInventoryData){
            if(StashItem.Value.NFT){
                string nftID = StashItem.Value.NFTID;
                if(!currentNFTS.ContainsValue(nftID)){
                    StashItem.Value.Deleted = true;
                    StashItem.Value.Changed = true;
                    stashNFTsDeleting.Add(StashItem);
                }  else {
                    string keyToRemove = null;
                    foreach (var kvp in currentNFTS){
                        if (kvp.Value == nftID){
                            keyToRemove = kvp.Key;
                            break;
                        }
                    }
                    if (keyToRemove != null){
                        currentNFTS.Remove(keyToRemove);
                    }
                }
            }
        }
        if(stashNFTsDeleting.Count > 0){
            foreach(var stashNFtDeleted in stashNFTsDeleting){
                sPlayer.GetStashNewItem(stashNFtDeleted);
                yield return new WaitForSeconds(.1f);
            }
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(ReChargeEnergy(nconn));
        sPlayer.TargetOpenUI(playerData.CurrentScene);
        sPlayer.TargetCharge(playerData.Energy);
        foreach(var sheet in characterSheets){
            sPlayer.GetFullCharacterData(sheet);
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(.1f);
        foreach(var charSheet in sPlayer.GetInformationSheets()){
            List<CharacterInventoryListItem> charNFTsDeleting = new List<CharacterInventoryListItem>();
            foreach(var charItem in charSheet.CharInventoryData){
                if(charItem.Value.NFT){
                    string nftID = charItem.Value.NFTID;
                    if(!currentNFTS.ContainsValue(nftID)){
                        charItem.Value.Deleted = true;
                        charItem.Value.Changed = true;
                        charNFTsDeleting.Add(charItem);
                    }  else {
                    string keyToRemove = null;
                    foreach (var kvp in currentNFTS)
                    {
                        if (kvp.Value == nftID)
                        {
                            keyToRemove = kvp.Key;
                            break;
                        }
                    }
                    
                    if (keyToRemove != null)
                    {
                        currentNFTS.Remove(keyToRemove);
                    }
                }
                }
            }
            if(charNFTsDeleting.Count > 0){
                foreach(var charNftDeleted in charNFTsDeleting){
                    sPlayer.GetCharacterNewItem(charSheet.CharacterID, charNftDeleted);
                    yield return new WaitForSeconds(.1f);
                }
            }
        }
        yield return new WaitForSeconds(.25f);
        sPlayer.ServerSpawnItems();
        print($"player {playerData.PlayerName} has connected and is logging in");
        string sceneSelect = playerData.CurrentScene;
        if(townRidden){
            sceneSelect = TOWNOFARUDINE;
        }
        ClientRequestLoadScene dummy = new ClientRequestLoadScene {
            oldScene = "Container",
            newScene = sceneSelect,
            node = playerData.SavedNode,
            login = true
        };
        GetCleanedSceneName(nconn, dummy);
        sPlayer.ServerBuildCharacters();
        if (currentNFTS.Count > 0)
        {
            StartCoroutine(SpawnNewNFTS(currentNFTS, playerData, nconn));
        }
        //StartCoroutine(DelayXRPLRegistration(nconn));
    }
    #endif
    public void SplitCombinedValue(string combinedValue, out string value, out string spellNameFull)
    {
        string[] parts = combinedValue.Split(';'); // Replace with the delimiter you used

        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid combined value");
        }

        value = parts[0];
        spellNameFull = parts[1];
    }

        List<string> DisconnectedList = new List<string>();
        List<string> ProcessingLogoutList = new List<string>();

        //Logout process
            #if UNITY_SERVER || UNITY_EDITOR

        IEnumerator ProcessingDisconnecting(NetworkConnectionToClient conn, PlayerInfo playerData, string _time, float energy, string scene, string node){
            yield return new WaitForSeconds(.1f); //set this much higher for disconnection time
            if(DisconnectedList.Contains(playerData.PlayFabId)){
                //process the logout now
                ProcessingLogoutList.Add(playerData.PlayFabId);
                StartCoroutine(LogThemOut(conn, playerData, _time, energy, scene, node));
            }
        }
         IEnumerator LogThemOut(NetworkConnectionToClient conn, PlayerInfo playerData, string _time, float energy, string scene, string node){
            
            ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>();
            bool loggedInBefore = false;
            bool SetNFTCooldowns = false;
            TimeSpan timeLeftForDay  = TimeSpan.Zero; // Initialize with zero time
            if(!string.IsNullOrEmpty(playerData.lastLogin)){
                loggedInBefore = true;
                DateTime datePrior = DateTime.Parse(playerData.lastLogin);
                DateTime dateCurrent = DateTime.Now;
                TimeSpan timeDifference = dateCurrent - datePrior;
                if (timeDifference.TotalDays >= 1)
                {
                    SetNFTCooldowns = false;
                } else {    
                    // Calculate how much time is left to complete a week    
                    timeLeftForDay  = TimeSpan.FromDays(1) - timeDifference;
                    print($"{timeLeftForDay } is how much time is left to reset our NFT COOLDOWN");
                }
            }
            string NewEnergy = energy.ToString();
            bool LimitHit = false;
            int LimitCount = 0;
            LimitCount ++;
            int currentBalance = 0;
            bool currencySave = false;
            PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest(){
                PlayFabId = playerData.PlayFabId,
            }, result => {
                currentBalance = result.VirtualCurrency.ContainsKey("DK") ? result.VirtualCurrency["DK"] : 0;
                currencySave = true;
            }, error => {
                Debug.LogError("Could not get player inventory: " + error.GenerateErrorReport());
                currencySave = true;
            });
            while(!currencySave){
                yield return new WaitForSeconds(.1f);
            }
            bool balancingGold = false;
            if((long)currentBalance != p.Gold){
                LimitCount ++;
                long difference = p.Gold - currentBalance;  // Find the difference between p.Gold and currentBalance
                int absDifference = Mathf.Abs((int)difference); // Convert difference to positive if it's negative
                // Choose either AddUserVirtualCurrency or SubtractUserVirtualCurrency based on the sign of the difference
                if (difference > 0){
                    PlayFabServerAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest{
                        PlayFabId = playerData.PlayFabId,
                        Amount = absDifference,
                        VirtualCurrency = "DK"
                    }, result =>
                    {
                        balancingGold = true;
                    }, error =>
                    {
                        balancingGold = true;
                        Debug.Log(error.ErrorMessage);
                    });
                } else {
                    PlayFabServerAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest
                    {
                        PlayFabId = playerData.PlayFabId,
                        Amount = absDifference,
                        VirtualCurrency = "DK"
                    }, result =>
                    {
                        balancingGold = true;
                    }, error =>
                    {
                        balancingGold = true;
                        Debug.Log(error.ErrorMessage);
                    });
                }
            } else {
                balancingGold = true;
            }
            while(!balancingGold){
                yield return new WaitForSeconds(.1f);
            }
            bool internalSave = false;
            //sPlayer.Gold -= (long)GoldCost;
            //int amount = GoldCost;
            //SubtractVirtualCurrency(playerData, amount);
            LimitCount ++;
            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string>
                {
                    {"savedNode", node},
                    {"LastScene", scene},
                    {"energy", NewEnergy},
                    {"lastLogin", _time}
                }
            }, result =>
            {
                internalSave = true;
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            while(!internalSave){
                yield return new WaitForSeconds(.1f);
            }
            LimitCount ++;
            //build string 
            string armorBonus = "0"; // not completed
            string GiantRep = string.Empty; // not completed
            string DragonRep = string.Empty; // not completed
            string LizardRep = string.Empty; // not completed
            string OrcRep = string.Empty; // not completed
            string FaerieRep = string.Empty; // not completed
            string ElvesRep = string.Empty; // not completed
            string DwarvesRep = string.Empty; // not completed
            string GnomesRep = string.Empty; // not completed
            (int selectedDay, int selectedMonth) = GetDate(p.GetTacticianSheet().Birthdate);
            string zodiacSign = "";
            if ((selectedMonth == 3 && selectedDay >= 21) || (selectedMonth == 4 && selectedDay <= 19))
                zodiacSign = "Aries";
            else if ((selectedMonth == 4 && selectedDay >= 20) || (selectedMonth == 5 && selectedDay <= 20))
                zodiacSign = "Taurus";
            else if ((selectedMonth == 5 && selectedDay >= 21) || (selectedMonth == 6 && selectedDay <= 21))
                zodiacSign = "Gemini";
            else if ((selectedMonth == 6 && selectedDay >= 22) || (selectedMonth == 7 && selectedDay <= 22))
                zodiacSign = "Cancer";
            else if ((selectedMonth == 7 && selectedDay >= 23) || (selectedMonth == 8 && selectedDay <= 22))
                zodiacSign = "Leo";
            else if ((selectedMonth == 8 && selectedDay >= 23) || (selectedMonth == 9 && selectedDay <= 22))
                zodiacSign = "Virgo";
            else if ((selectedMonth == 9 && selectedDay >= 23) || (selectedMonth == 10 && selectedDay <= 23))
                zodiacSign = "Libra";
            else if ((selectedMonth == 10 && selectedDay >= 24) || (selectedMonth == 11 && selectedDay <= 21))
                zodiacSign = "Scorpio";
            else if ((selectedMonth == 11 && selectedDay >= 22) || (selectedMonth == 12 && selectedDay <= 21))
                zodiacSign = "Sagittarius";
            else if ((selectedMonth == 12 && selectedDay >= 22) || (selectedMonth == 1 && selectedDay <= 19))
                zodiacSign = "Capricorn"; 
            else if ((selectedMonth == 1 && selectedDay >= 20) || (selectedMonth == 2 && selectedDay <= 18))
                zodiacSign = "Aquarius";
            else if ((selectedMonth == 2 && selectedDay >= 19) || (selectedMonth == 3 && selectedDay <= 20))
                zodiacSign = "Pisces";

            if(zodiacSign == "Capricorn" || zodiacSign == "Virgo" || zodiacSign == "Taurus" || zodiacSign == "Leo"){
                armorBonus = "1";
            }
            foreach(var stat in p.GetTacticianSheet().TacticianStatData){
                if(stat.Key == "GiantRep"){
                    GiantRep = stat.Value;
                }
                if(stat.Key == "DragonRep"){
                    DragonRep = stat.Value;
                }
                if(stat.Key == "LizardRep"){
                    LizardRep = stat.Value;
                }
                if(stat.Key == "OrcRep"){
                    OrcRep = stat.Value;
                }
                if(stat.Key == "FaerieRep"){
                    FaerieRep = stat.Value;
                }
                if(stat.Key == "ElfRep"){
                    ElvesRep = stat.Value;
                }
                if(stat.Key == "DwarfRep"){
                    DwarvesRep = stat.Value;
                }
                if(stat.Key == "GnomeRep"){
                    GnomesRep = stat.Value;
                }
            }
            string dkpCooldown = p.GetTacticianSheet().DKPCooldown;
            if(dkpCooldown != "0"){
                DateTime cdTime = DateTime.Parse(dkpCooldown);
                DateTime timeNow = DateTime.Now;
                TimeSpan timeSpan = timeNow - cdTime;
                if(timeSpan.TotalHours >= 24){
                    dkpCooldown = "0";
                }
            }
            string tactBuild = p.GetTacticianSheet().Birthdate + "_" + zodiacSign + "_" +  
                p.GetTacticianSheet().EyeColor + "_" + p.GetTacticianSheet().BodyStyle + "_" + 
                p.GetTacticianSheet().StrengthBonus + "_" + p.GetTacticianSheet().AgilityBonus + "_" + 
                p.GetTacticianSheet().FortitudeBonus + "_" + p.GetTacticianSheet().ArcanaBonus + "_" + 
                armorBonus + "_" + GiantRep.ToString() + "_" +  DragonRep.ToString() + "_" +  
                LizardRep.ToString() + "_" +  OrcRep.ToString() + "_" +  FaerieRep.ToString() + "_" +  
                ElvesRep.ToString() + "_" +  DwarvesRep.ToString() + "_" +  GnomesRep.ToString() + "_" + 
                dkpCooldown;
            PlayFabServerAPI.UpdateUserData(new UpdateUserDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string>
                {
                    {"TACTBUILDSTRING", tactBuild}
                }
            }, result =>
            {
                internalSave = true;
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            foreach(var item in p.GetTacticianSheet().TacticianInventoryData){
                if(LimitCount >= 10){
                    LimitHit = true;
                }
                if(LimitHit){
                    yield return new WaitForSeconds(15f);
                    LimitCount = 0;
                    LimitHit = false;
                }
                if(item.Value.Changed){
                    print($"{item.Value.Item_Name} is being worked on in tact inventory");
                    EquipmentSaveData saveData = (new EquipmentSaveData{
                        intialized = "Intialized"
                    });
                    saveData.Stash = false;
                    if(item.Value.OGTacticianBelt){
                        saveData.OGTactBelt = true;
                    }
                    if(item.Value.OGTacticianInventory){
                        saveData.OGTactInv= true;
                    }
                    if(item.Value.OGTacticianStash){
                        saveData.OGStash = true;
                    }
                    if(item.Value.EQUIPPEDSLOT == "Unequipped" || item.Value.EQUIPPEDSLOT == "0"){
                        saveData.Slot = null;
                    } else {
                        saveData.Slot = item.Value.EQUIPPEDSLOT;
                    }
                    if(item.Value.TacticianInventory){
                        saveData.TactInv = true;
                        saveData.Stash = false;
                        saveData.TactBelt = false;
                        saveData.TactEquipped = false;
                    }
                    if(item.Value.TacticianEquip){
                        saveData.TactEquipped = true;
                        saveData.Stash = false;
                        saveData.TactBelt = false;
                        saveData.TactInv = false;
                    }
                    if(item.Value.TacticianBelt){
                        saveData.TactBelt = true;
                        saveData.TactEquipped = false;
                        saveData.Stash = false;
                        saveData.TactInv = false;
                    }
                    if(!item.Value.Deleted){
                        if(item.Value.amount != 0){
                            if(item.Value.InstanceID == "NewItemGenerated"){
                                //Generate an item and move to tact position in 
                                float weightCal = float.Parse(item.Value.Weight);
                                weightCal *= item.Value.amount;
                                string weightCalculation = weightCal.ToString();
                                ServerAddItemLogout(conn, item.Value.Item_Name, string.Empty, true, false, item.Value.amount, weightCalculation, item.Value.Rarity_item, item.Value.itemID);
                                LimitCount ++;
                            } else {
                                if(item.Value.OwnerID == "Tactician" || item.Value.OwnerID == "Stash"){
                                    SaveTacticianChange(conn, item.Value, saveData);
                                    LimitCount ++;
                                } else {
                                    saveData.CharacterSlotOne = item.Value.OwnerID;
                                    SendItemFromCharacterToTactician(conn, item.Value, saveData);
                                    LimitCount ++;
                                }
                            }
                        } else {
                            if(item.Value.InstanceID == "NewItemGenerated"){
                                //no need to trash do nothing
                            } else {
                                ServerRemoveItemOnUser(conn, item.Value.InstanceID);
                                LimitCount ++;
                            }
                        }
                    } else {
                        //this is a deleted NFT lets kill it
                        ServerRemoveItemOnUser(conn, item.Value.InstanceID);
                    }
                    
                }
            }
            foreach(var item in p.GetTacticianSheet().StashInventoryData){
                if(LimitCount >= 10){
                    LimitHit = true;
                }
                if(LimitHit){
                    yield return new WaitForSeconds(15f);
                    LimitCount = 0;
                    LimitHit = false;
                }
                if(item.Value.Changed){
                    print($"{item.Value.Item_Name} is being worked on in stash inventory");
                    EquipmentSaveData saveData = (new EquipmentSaveData{
                        intialized = "Intialized"
                    });
                    saveData.Stash = true;
                    if(item.Value.OGTacticianBelt){
                        saveData.OGTactBelt = true;
                    }
                    if(item.Value.OGTacticianInventory){
                        saveData.OGTactInv= true;
                    }
                    if(item.Value.OGTacticianStash){
                        saveData.OGStash = true;
                    }
                    if(item.Value.EQUIPPEDSLOT == "Unequipped" || item.Value.EQUIPPEDSLOT == "0"){
                        saveData.Slot = null;
                    } else {
                        saveData.Slot = item.Value.EQUIPPEDSLOT;
                    }
                    if(item.Value.TacticianStash){
                        saveData.TactInv = false;
                        saveData.TactBelt = false;
                    }
                    if(!item.Value.Deleted){
                        if(item.Value.amount != 0){
                            if(item.Value.InstanceID == "NewItemGenerated"){
                                //Generate an item and move to tact position in 
                                float weightCal = float.Parse(item.Value.Weight);
                                weightCal *= item.Value.amount;
                                string weightCalculation = weightCal.ToString();
                                ServerAddItemLogout(conn, item.Value.Item_Name, string.Empty, false, true, item.Value.amount, weightCalculation, item.Value.Rarity_item, item.Value.itemID);
                                LimitCount ++;
                            } else {
                                if(item.Value.OwnerID == "Tactician" || item.Value.OwnerID == "Stash"){
                                    SaveTacticianChange(conn, item.Value, saveData);
                                    LimitCount ++;
                                } else {
                                    saveData.CharacterSlotOne = item.Value.OwnerID;
                                    SendItemFromCharacterToTactician(conn, item.Value, saveData);
                                    LimitCount ++;
                                }
                            }   
                        } else {
                            if(item.Value.InstanceID == "NewItemGenerated"){
                                //no need to trash do nothing
                            } else {
                                if(item.Value.OwnerID == "Tactician" || item.Value.OwnerID == "Stash"){
                                    ServerRemoveItemOnUser(conn, item.Value.InstanceID);
                                    LimitCount ++;
                                } else {
                                    ServerRemoveItemOnUserFromCharacter(conn, item.Value.InstanceID, item.Value.OwnerID);
                                    LimitCount ++;
                                }
                            }
                        }
                    } else {
                        //this is a deleted NFT lets kill it
                        ServerRemoveItemOnUser(conn, item.Value.InstanceID);
                    }
                }
            }
            foreach(var sheet in p.GetInformationSheets()){
                string curHealth = "1";
                string curMana = "1";
                bool manafound = false;
                bool hpfound = false;
                foreach(var stat in sheet.CharStatData){
                    if(hpfound && manafound){
                        break;
                    }
                    if(stat.Key == "currentHP"){
                        curHealth = stat.Value;
                        hpfound = true;
                    }
                    if(stat.Key == "currentMP"){
                        curMana = stat.Value;
                        manafound = true;
                    }
                }
                PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    CharacterId = sheet.CharacterID,
                    Data = new Dictionary<string, string>
                    {
                        {"currentHP", curHealth}, {"currentMP", curMana}
                    }
                }, result =>
                {
                }, error =>{
                    Debug.Log(error.ErrorMessage); 
                    Debug.Log(error.ErrorDetails);
                    Debug.Log(error.Error);
                });
                LimitCount ++;
                if(LimitCount >= 10){
                    LimitHit = true;
                }
                if(LimitHit){
                    yield return new WaitForSeconds(15f);
                    LimitCount = 0;
                    LimitHit = false;                                                 
                }
                foreach(var item in sheet.CharInventoryData){
                    if(LimitCount >= 10){
                        LimitHit = true;
                    }
                    if(LimitHit){
                        yield return new WaitForSeconds(15f);
                        LimitCount = 0;
                        LimitHit = false;
                    }
                    if(item.Value.Changed){
                        print($"{item.Value.Item_Name} is being worked on in char sheets");
                        EquipmentSaveData saveData = (new EquipmentSaveData{
                            intialized = "Intialized"
                        });
                        if(item.Value.OGTacticianBelt){
                            saveData.OGTactBelt = true;
                        }
                        if(item.Value.OGTacticianInventory){
                            saveData.OGTactInv= true;
                        }
                        if(item.Value.OGTacticianStash){
                            saveData.OGStash = true;
                        }
                        saveData.Stash = false;
                        saveData.TactInv = false;
                        saveData.TactBelt = false;
                        if(item.Value.EQUIPPEDSLOT == "Unequipped" || item.Value.EQUIPPEDSLOT == "0"){
                            saveData.Slot = null;
                        } else {
                            saveData.Slot = item.Value.EQUIPPEDSLOT;
                        }
                        if(!item.Value.Deleted){
                            if(item.Value.amount != 0){
                                if(item.Value.InstanceID == "NewItemGenerated"){
                                    //Generate an item and move to tact position in 
                                    float weightCal = float.Parse(item.Value.Weight);
                                    weightCal *= item.Value.amount;
                                    string weightCalculation = weightCal.ToString();
                                    ServerAddItemLogout(conn, item.Value.Item_Name, sheet.CharacterID, false, false, item.Value.amount, weightCalculation, item.Value.Rarity_item, item.Value.itemID);
                                    LimitCount ++;
                                } else {
                                    if(item.Value.OwnerID == "Tactician" || item.Value.OwnerID == "Stash"){
                                        saveData.CharacterSlotOne = sheet.CharacterID;
                                        SendItemFromTacticianToCharacter(conn, item.Value, saveData);
                                        LimitCount ++;
                                    } else {
                                        if(item.Value.OwnerID == sheet.CharacterID){
                                            saveData.CharacterSlotOne = sheet.CharacterID;
                                            saveData.CharacterSlotTwo = sheet.CharacterID;
                                            SendItemToUserForUpdatingThenBackToDesiredSerial(conn, item.Value, saveData);
                                            LimitCount ++;
                                        }
                                        if(item.Value.OwnerID != sheet.CharacterID && item.Value.OwnerID != "Tactician" && item.Value.OwnerID != "Stash"){
                                            saveData.CharacterSlotOne = item.Value.OwnerID;
                                            saveData.CharacterSlotTwo = sheet.CharacterID;
                                            SendItemToUserForUpdatingThenBackToDesiredSerial(conn, item.Value, saveData);
                                            LimitCount ++;
                                        }
                                    }
                                }
                            } else {
                                if(item.Value.InstanceID == "NewItemGenerated"){
                                    //no need to trash do nothing
                                } else {
                                    if(item.Value.OwnerID == "Tactician" || item.Value.OwnerID == "Stash"){
                                        ServerRemoveItemOnUser(conn, item.Value.InstanceID);
                                        LimitCount ++;
                                    } else {
                                        ServerRemoveItemOnUserFromCharacter(conn, item.Value.InstanceID, item.Value.OwnerID);
                                        LimitCount ++;
                                    }
                                }
                            }
                        } else {
                            if(item.Value.OwnerID == "Tactician" || item.Value.OwnerID == "Stash"){
                                ServerRemoveItemOnUser(conn, item.Value.InstanceID);
                                LimitCount ++;
                            } else {
                                ServerRemoveItemOnUserFromCharacter(conn, item.Value.InstanceID, item.Value.OwnerID);
                                LimitCount ++;
                            }
                        }
                    }
                    if(LimitCount >= 9){
                        LimitHit = true;
                    }
                    if(LimitHit){
                        yield return new WaitForSeconds(15f);
                        LimitCount = 0;
                        LimitHit = false;
                    }
                    Dictionary<string, string> cooldownData = new Dictionary<string, string>
                    {
                        { "COOLDOWNQ", null },
                        { "COOLDOWNE", null },
                        { "COOLDOWNR", null },
                        { "COOLDOWNF", null }
                    };
                    if (sheet.CharCooldownData != null){
                        foreach (var coolies in sheet.CharCooldownData){
                            DateTime endTime;
                            if (DateTime.TryParse(coolies.Value, out endTime)){
                                if (DateTime.Now <= endTime){
                                    // Update the value for the corresponding key with the unexpired time
                                    cooldownData[coolies.PKey] = endTime.ToString() + ";" + coolies.SpellnameFull;
                                }
                            }
                        }
                        PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest{
                            PlayFabId = playerData.PlayFabId,
                            CharacterId = sheet.CharacterID,
                            Data = cooldownData
                        }, result =>
                        {}, error =>
                        {
                            Debug.Log(error.ErrorMessage);
                        });
                        LimitCount ++;
                    }
                }
            }
            StartCoroutine(FinishLogout(conn, playerData.PlayFabId));
        }
            #endif

        private IEnumerator FinishLogout(NetworkConnectionToClient conn, string ID) {
            yield return new WaitForSeconds(2f);
            #if UNITY_SERVER || UNITY_EDITOR

            ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>();
            bool KillmatchNeeded = p.currentMatch != null;
            StopCoroutine(ReChargeEnergy(conn));
            Debug.Log("Client disconnected from server, ConnectionId: " + conn.connectionId);
            if(KillmatchNeeded){
                StartCoroutine(ServerDisconnectPlayer(conn, ID));
            } else {
                var playerConnection = playerConnections.Find(c => c.ConnectionId == conn.connectionId);
                // Check if playerConnection exists
                if (playerConnection == null) {
                    Debug.LogWarning("Player connection not found.");
                    yield break;
                }
                StartCoroutine(ServerDisconnectPlayerPause(conn, ID));
            }
            #endif
        }

        //Energy recharge
        private IEnumerator ReChargeEnergy(NetworkConnectionToClient conn) {
            ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>();  
             
            while (true) {
                if(p == null){
                    yield break;
                }
                PlayerInfo player = (PlayerInfo)conn.authenticationData;
                yield return new WaitForSeconds(60);
                if(p == null){
                    yield break;
                }
                AdjustCurrentEnergy (conn, player);
            }
        }
        void AuthorizeEnergyUpdate(ScenePlayer player, float cost){
            #if UNITY_SERVER || UNITY_EDITOR

            NetworkConnectionToClient nconn = player.connectionToClient;
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                UpdateEnergy(nconn, playerData, cost);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void UpdateEnergy(NetworkConnectionToClient nconn, PlayerInfo playerData, float cost){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer player = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            string CurrentNode = player.currentNode;
            Vector2 nodeCoordinates = GetNodeCoordinates(CurrentNode);
            player.Energy -= cost;
            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string>
                {
                    {"energy", player.Energy.ToString()},
                    {"LastScene", player.currentScene},
                    {"savedNode", player.currentNode}
                }
                
            }, result =>
            {

                player.TargetUpdateEnergyDisplay(player.Energy);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void AuthorizeMovementRequest(NetworkConnectionToClient nconn, string charliesTicket){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            if(playerData.SessionTicket != charliesTicket)
            {
                //print($"{nconn.identity.gameObject} has been compromised, log information");
                return;
            }
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                MovePlayerOVM(nconn, playerData);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void MovePlayerOVM(NetworkConnectionToClient nconn, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR

            ScenePlayer player = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            float payment = 100f;
            string saveNode = player.currentNode;
            float original = playerData.Energy;
            float newEnergy = original - payment;
            string NewEnergy = newEnergy.ToString();
            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string>
                {
                    {"energy", NewEnergy},
                    {"LastScene", "OVM"},
                    {"savedNode", saveNode}
                }
            }, result =>
            {
                player.Energy = newEnergy;
                //player.TravelToTheNode();
                
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        //void SetupLogin(NetworkConnectionToClient conn, ClientRequestLoadScene dummy)
        //{   
        //    //print("made it to setup login method before GetCleanedSceneName");
        //    GetCleanedSceneName(conn, dummy);
        //    StartCoroutine(ReChargeEnergy(conn));
        //}
        //IEnumerator LoginSetup(NetworkConnectionToClient conn, ClientRequestLoadScene dummy){
        //    yield return new WaitForSeconds(1f);
        //    //print("made it to login Setup");
        //    SetupLogin(conn, dummy);
        //}
        void AdjustCurrentEnergy(NetworkConnectionToClient conn, PlayerInfo playerData) {
        float _oldEnergy = conn.identity.gameObject.GetComponent<ScenePlayer>().Energy;
        ScenePlayer p = conn.identity.gameObject.GetComponent<ScenePlayer>(); 
        //float _oldEnergy = playerData.Energy;
        //print($"_oldEnergy is {_oldEnergy}");
        float fastRechargeRate = 2.08f;
        float normalRechargeRate = 0.694444f;
        float newEnergy = 0.0f;
        if(_oldEnergy >= 3000){
            newEnergy = normalRechargeRate + _oldEnergy;
            if(newEnergy >= 10000.0f)
            {
               playerData.Energy = 10000.0f; 
            }else{
                playerData.Energy = newEnergy;
                //print($"New energy is {playerData.Energy}");
            }
        }else{ 
            if(_oldEnergy < 3000)
            {
                var FastChargeableAmount = 3000.0f - _oldEnergy;
                if(FastChargeableAmount >= fastRechargeRate)
                {
                    newEnergy = fastRechargeRate + _oldEnergy;
                    playerData.Energy = newEnergy;
                    //print($"New energy is {playerData.Energy}");
                }else{
                    float SlowSecondRate = 0.0115740666666667f;
                    float FastSecondRate = 0.0346666666666667f;
                    var firstTimeHalf = FastChargeableAmount/FastSecondRate;
                    var SecondTimeHalf = 60.0f - firstTimeHalf;
                    var slowCharge = SecondTimeHalf * SlowSecondRate;
                    newEnergy = slowCharge + firstTimeHalf + _oldEnergy;
                    playerData.Energy = newEnergy;
                    //print($"New energy is {playerData.Energy}");
                }   
            }            
        }
        if(p != null)
        {
            conn.authenticationData = playerData;
              
            p.EnergyTick(newEnergy);
        }else{
            //print("Solution worked");
            return;
        }
    }
        //CHARACTER FUNCTIONS /// OTHER FUNCTIONS
        void CharacterCreation(NetworkConnectionToClient nconn, string _nameRequest, string _Type, string spri){
            #if UNITY_SERVER || UNITY_EDITOR

            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            //print($"{_nameRequest} is name, {_Type} is type, {spri} is sprite name. ");
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                //print("got to CheckInventoryUniversal");
                CheckInventoryUniversal(nconn, playerData, _nameRequest, _Type, spri);
                
                
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void CheckInventoryUniversal(NetworkConnectionToClient nconn, PlayerInfo playerData, string _nameRequest, string _Type, string spri){
            #if UNITY_SERVER || UNITY_EDITOR

        PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest
            {
                PlayFabId = playerData.PlayFabId,


            }, result =>
            {
                //print("got to right before SpendUniversalToken");
                ScenePlayer player = nconn.identity.gameObject.GetComponent<ScenePlayer>();
                //List<string> strings = new List<string>();
                List<ItemInstance> inventory = result.Inventory;

                string id = string.Empty;

                for (int i = 0; i < inventory.Count; i++)
                {
                    if(inventory[i].ItemId == "UniversalToken"){
                        int? tokenz = inventory[i].RemainingUses;
                        int TokensLeft = 0;
                        id = inventory[i].ItemInstanceId;
                        if(tokenz.HasValue){
                            TokensLeft = tokenz.Value;
                            SpendUniversalToken(nconn, playerData, id, _nameRequest, _Type, spri);
                            player.TokenCounted(TokensLeft);
                        }
                    }
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void SpendUniversalToken(NetworkConnectionToClient nconn, PlayerInfo playerData, string ID, string _nameRequest, string _Type, string spri){
            #if UNITY_SERVER || UNITY_EDITOR

            int tokens = 0;
            PlayFabServerAPI.ConsumeItem(new ConsumeItemRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemInstanceId = ID,
                ConsumeCount = 1
            }, result =>
            {
                BuildCharacter(nconn, playerData, _nameRequest, _Type, spri);
                tokens = result.RemainingUses;
                ScenePlayer player = nconn.identity.GetComponent<ScenePlayer>();
                player.TokenCounted(tokens);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void BuildCharacter(NetworkConnectionToClient nconn, PlayerInfo playerData, string _nameRequest, string _Type, string spri){
            #if UNITY_SERVER || UNITY_EDITOR

            PlayFabServerAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                CharacterName = _nameRequest,
                CharacterType = _Type
            }, result =>
            {
                string id = result.CharacterId;
                UpdateCharacterSprite(nconn, playerData, _nameRequest, _Type, spri, id);
            }, error =>{
                    Debug.Log(error.ErrorMessage);
                    Debug.Log(error.ErrorDetails);
                    Debug.Log(error.Error);
                });
            #endif
        }
        void UpdateCharacterSprite(NetworkConnectionToClient nconn, PlayerInfo playerData, string _nameRequest, string _Type, string spri, string ID){
            #if UNITY_SERVER || UNITY_EDITOR

            int FORTITUDE = 0;
            int ARCANA = 0;
            if(_Type == "Fighter"){
                FORTITUDE = 90;
                ARCANA = 50;
            }
            if(_Type == "Rogue"){
                FORTITUDE = 70;
                ARCANA = 60;
            }
            if(_Type == "Priest"){
                FORTITUDE = 90;
                ARCANA = 90;
            }
            if(_Type == "Archer"){
                FORTITUDE = 70;
                ARCANA = 50;
            }
            if(_Type == "Wizard"){
                FORTITUDE = 50;
                ARCANA = 120;
            }
            if(_Type == "Enchanter"){
                FORTITUDE = 40;
                ARCANA = 140;
            }
            if(_Type == "Druid"){
                FORTITUDE = 75;
                ARCANA = 100;
            }
            if(_Type == "Paladin"){
                FORTITUDE = 85;
                ARCANA = 70;
            }
            ScenePlayer player = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int tacticianBonusFortitude = 0;
            int tacticianBonusArcana = 0;
            TacticianFullDataMessage tacticianSheet = player.GetTacticianSheet();
            tacticianBonusFortitude += int.Parse(tacticianSheet.FortitudeBonus);
            tacticianBonusArcana += int.Parse(tacticianSheet.ArcanaBonus);
            foreach(var tacticianEquipped in tacticianSheet.TacticianInventoryData){
                if (tacticianEquipped.Value.GetTacticianEquip())
                {
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFORTITUDE_item()))
                    {
                        tacticianBonusFortitude +=  int.Parse(tacticianEquipped.Value.GetFORTITUDE_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetARCANA_item()))
                    {
                        tacticianBonusArcana += int.Parse(tacticianEquipped.Value.GetARCANA_item());
                    }
                }
            }
            int MaxHP = tacticianBonusFortitude + FORTITUDE;
            int MaxMP = (tacticianBonusArcana + ARCANA) / 7;
            //print($"{_Type} is class and {_nameRequest} is name requested");
            PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    CharacterId = ID,
                    Data = new Dictionary<string, string>
                    {
                        {"CharacterSprite", spri}, {"LVL", "1"}, {"EXP", "0"}, 
                        {"currentMP", MaxMP.ToString()}, {"Class", _Type}, {"CharName", _nameRequest}, 
                        {"currentHP", MaxHP.ToString()}, {"ClassPoints", "55"}
                    }
                }, result =>
                {
                    Dictionary<string, string> stats = new Dictionary<string, string>();
                    stats.Add("CharacterSprite", spri);
                    stats.Add("LVL", "1");
                    stats.Add("EXP", "0");
                    stats.Add("currentMP", MaxMP.ToString());
                    stats.Add("Class", _Type);
                    stats.Add("CharName", _nameRequest);
                    stats.Add("currentHP", MaxHP.ToString());
                    stats.Add("ClassPoints", "55");
                    FinishUpdateCharacterSprite(nconn, playerData, ID, stats);
                }, error =>{
                    Debug.Log(error.ErrorMessage); 
                    Debug.Log(error.ErrorDetails);
                    Debug.Log(error.Error);
                });
            #endif
        }
        void FinishUpdateCharacterSprite(NetworkConnectionToClient nconn, PlayerInfo playerData, string ID, Dictionary<string, string> stats){
            #if UNITY_SERVER || UNITY_EDITOR

            string none = "None";
            PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    CharacterId = ID,
                    Data = new Dictionary<string, string>
                    {
                        {"CharacterID", ID}, {"SPELLQ", none}, {"SPELLE", none}, 
                        {"SPELLR", none}, {"SPELLF", none}, {"CORE", "STANDARD"}
                    }
                }, result =>
                {
                    stats.Add("CharacterID", ID);
                    stats.Add("SPELLQ", none);
                    stats.Add("SPELLE", none);
                    stats.Add("SPELLR", none);
                    stats.Add("SPELLF", none);
                    stats.Add("CORE", "STANDARD");
                    GetNewCharacterList(nconn, playerData, ID, stats);  
                }, error =>{
                    Debug.Log(error.ErrorMessage); 
                    Debug.Log(error.ErrorDetails);
                    Debug.Log(error.Error);
                });    
            #endif
        }
        void GetNewCharacterList(NetworkConnectionToClient nconn, PlayerInfo playerData, string ID, Dictionary<string, string> stats)
        {   
            #if UNITY_SERVER || UNITY_EDITOR

            GetCharacterDataRequest request = new GetCharacterDataRequest();
            request.PlayFabId = playerData.PlayFabId;
            request.CharacterId = ID;
            PlayFabServerAPI.GetCharacterInternalData(request,
            result =>
            {
                ScenePlayer p = nconn.identity.GetComponent<ScenePlayer>();
                p.TokenCount = p.TokenCount - 1;
                p.TargetTokenUpdate();
                List<string> items = new List<string>();
                string staff = "Tier1Staff";
                items.Add(staff);
                CreatingStaff(nconn, items, playerData, ID, stats);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif   
        }
        void CreatingStaff(NetworkConnectionToClient nconn,List<string> items, PlayerInfo playerData, string charID, Dictionary<string, string> stats){
            #if UNITY_SERVER || UNITY_EDITOR

            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = items
            }, result =>
            {
                foreach(var item in result.ItemGrantResults){
                    BuildStaffItemForNewChar(nconn, playerData, item, charID, stats);
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        #if UNITY_SERVER || UNITY_EDITOR

    void BuildStaffItemForNewChar(NetworkConnectionToClient nconn, PlayerInfo playerData, GrantedItemInstance item, string charID, Dictionary<string, string> stats){

        string Weight = "3.5";
        string STRENGTH_item = "0";
        string AGILITY_item = "0";
        string FORTITUDE_item = "0";
        string ARCANA_item = "0";
        string Rarity_item = "Common";
        string MagicResist_item = "0";
        string FireResist_item = "0";
        string ColdResist_item = "0";
        string PoisonResist_item = "0";
        string DiseaseResist_item = "0";
        string Armor = "0";
        string DamageMin_item = "20";
        string DamageMax_item = "35";
        string Parry_item = "15";
        string Penetration_item = "1";
        string AttackDelay_item = "75";
        string BlockChance_item = null;
        string BlockValue_item = null;
        string TypeOfItem = "Staff";
        string Itemname = "Staff";
        string Item_Slot = "Two Handed";
        int quantity = 1;
        string ItemDescript = "Starting Staff";
            
        Dictionary<string,string> statBookOne = new Dictionary<string, string>();
        Dictionary<string,string> statBookTwo = new Dictionary<string, string>();
        Dictionary<string,string> statBookThree = new Dictionary<string, string>();
        Dictionary<string,string> statBookFour = new Dictionary<string, string>();
        Dictionary<string,string> statBookFive = new Dictionary<string, string>();
        Dictionary<string,string> statBookSix = new Dictionary<string, string>();

        ItemSelectable.ItemType type = ItemSelectable.ItemType.TwoHandedWeapon;
            
            
        statBookOne.Add("Weight", Weight);
        statBookOne.Add("STRENGTH_item", STRENGTH_item);
        statBookOne.Add("AGILITY_item", AGILITY_item);
        statBookOne.Add("FORTITUDE_item", FORTITUDE_item);
        statBookOne.Add("ARCANA_item", ARCANA_item);
            
        statBookTwo.Add("Rarity_item", Rarity_item);
        statBookTwo.Add("MagicResist_item", MagicResist_item);
        statBookTwo.Add("FireResist_item", FireResist_item);
        statBookTwo.Add("ColdResist_item", ColdResist_item);
        statBookTwo.Add("PoisonResist_item", PoisonResist_item);
            
        statBookThree.Add("DiseaseResist_item", DiseaseResist_item);
        statBookThree.Add("Durability_item", "100");
        statBookThree.Add("DurabilityMax_item", "100");
        statBookThree.Add("Item_Slot", Item_Slot);
        statBookThree.Add("TypeOfItem", TypeOfItem);
            
        statBookFour.Add("Item_Name", Itemname);
        statBookFour.Add("Armor_item", Armor);
        statBookFour.Add("BlockChance_item", BlockChance_item);
        statBookFour.Add("BlockValue_item", BlockValue_item);
            
        statBookFive.Add("DamageMin_item", DamageMin_item);
        statBookFive.Add("DamageMax_item", DamageMax_item);
        statBookFive.Add("Parry_item", Parry_item);
        statBookFive.Add("Penetration_item", Penetration_item);
        statBookFive.Add("AttackDelay_item", AttackDelay_item);

        statBookSix.Add("EquippedSlot", "Main-Hand");
        statBookSix.Add("Quality_item", "Plain");
        statBookSix.Add("ItemDescription", ItemDescript);
        ItemSelectable itemAdded = new ItemSelectable{
            itemType = type, amount = quantity,  Weight = Weight, 
            STRENGTH_item = STRENGTH_item, AGILITY_item = AGILITY_item,
            FORTITUDE_item = FORTITUDE_item, ARCANA_item = ARCANA_item,
            MagicResist_item = MagicResist_item, FireResist_item = FireResist_item, 
            ColdResist_item = ColdResist_item, PoisonResist_item = PoisonResist_item,
            DiseaseResist_item = DiseaseResist_item, Rarity_item = Rarity_item,
            Item_Name = Itemname, Durability = "100", DurabilityMax = "100",
            Parry = Parry_item, Penetration = Penetration_item, AttackDelay = AttackDelay_item,
            BlockChance = BlockChance_item, BlockValue = BlockValue_item, 
            ItemSpecificClass = TypeOfItem, itemSlot = Item_Slot, Armor_item = Armor,
            OwnerID = charID, InstanceID = item.ItemInstanceId, EQUIPPED = true, itemID = item.ItemId,
            DamageMin = DamageMin_item, DamageMax = DamageMax_item, EQUIPPEDSLOT = "Main-Hand",
            INVENTORY = false, NFT = false, MagicalEffectActive = false, ItemDescription = ItemDescript, Quality_item = "Plain", customID = Guid.NewGuid().ToString()
        };
        /*

        ItemSelectable itemAdded = new ItemSelectable();
        itemAdded.SetItemType(type);
        itemAdded.SetAmount(quantity);
        itemAdded.SetWeight(Weight);
        itemAdded.SetSTRENGTH_item(STRENGTH_item);
        itemAdded.SetAGILITY_item(AGILITY_item);
        itemAdded.SetFORTITUDE_item(FORTITUDE_item);
        itemAdded.SetARCANA_item(ARCANA_item);
        itemAdded.SetMagicResist_item(MagicResist_item);
        itemAdded.SetFireResist_item(FireResist_item);
        itemAdded.SetColdResist_item(ColdResist_item);
        itemAdded.SetPoisonResist_item(PoisonResist_item);
        itemAdded.SetDiseaseResist_item(DiseaseResist_item);
        itemAdded.SetRarity_item(Rarity_item);
        itemAdded.SetItemName(Itemname);
        itemAdded.SetDurability("100");
        itemAdded.SetDurabilityMax("100");
        itemAdded.SetParry(Parry_item);
        itemAdded.SetPenetration(Penetration_item);
        itemAdded.SetAttackDelay(AttackDelay_item);
        itemAdded.SetBlockChance(BlockChance_item);
        itemAdded.SetBlockValue(BlockValue_item);
        itemAdded.SetItemSpecificClass(TypeOfItem);
        itemAdded.SetItemSlot(Item_Slot);
        itemAdded.SetArmor_item(Armor);
        itemAdded.SetOwnerID(charID);
        itemAdded.SetInstanceID(item.ItemInstanceId);
        itemAdded.EQUIPPED = true;
        itemAdded.SetDamageMin(DamageMin_item);
        itemAdded.SetDamageMax(DamageMax_item);
        itemAdded.SetEQUIPPEDSLOT("Main-Hand");
        itemAdded.INVENTORY = false;
        itemAdded.SetNFT(false);
        itemAdded.SetMagicalEffectActive(false);
        itemAdded.SetItemDescription(ItemDescript);
        */
        //print($"Building item {itemAdded.GetItemName()}***************");
        StaffBookOne(nconn, item.ItemInstanceId, playerData, itemAdded, statBookOne, statBookTwo, statBookThree, statBookFour, statBookFive, charID, statBookSix, stats);
            
    }
    #endif
    void StaffBookOne(NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookOne, Dictionary<string,string> statBookTwo, Dictionary<string,string> statBookThree, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, string ID, Dictionary<string,string> statBookSix, Dictionary<string, string> stats){
        #if UNITY_SERVER || UNITY_EDITOR
            
        //print("made it to first part");
        Dictionary<string,string> statBook = statBookOne; 
            
        PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
        {
            ItemInstanceId = InstanceID,
            PlayFabId = playerData.PlayFabId,
            Data = statBook
        }, result =>{
            //PassItemToStash(nconn, item);
            StaffBookTwo(nconn, InstanceID, playerData, item, statBookTwo, statBookThree, statBookFour, statBookFive, ID, statBookSix, stats);
        }, error =>{
            //print("Failed second part of first");
            Debug.Log(error.ErrorMessage);
            Debug.Log(error.ErrorDetails);
            Debug.Log(error.Error);
        });
        #endif
    }
        void StaffBookTwo(NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookTwo, Dictionary<string,string> statBookThree, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, string ID, Dictionary<string,string> statBookSix, Dictionary<string, string> stats){
            #if UNITY_SERVER || UNITY_EDITOR
            
            Dictionary<string,string> statBook = statBookTwo; 
            //print("made it to second part");

            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
                StaffBookThree(nconn, InstanceID, playerData, item, statBookThree, statBookFour, statBookFive, ID, statBookSix, stats);
            }, error =>{
                //print("Failed second part of second");

                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void StaffBookThree(NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookThree, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, string ID, Dictionary<string,string> statBookSix, Dictionary<string, string> stats){
            #if UNITY_SERVER || UNITY_EDITOR
            
            Dictionary<string,string> statBook = statBookThree; 
            //print("made it to third part");

            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{

                StaffBookFour(nconn, InstanceID, playerData, item, statBookFour, statBookFive, ID, statBookSix, stats);
            }, error =>{
                //print("Failed second part of third");

                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void StaffBookFour(NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, string ID, Dictionary<string,string> statBookSix, Dictionary<string, string> stats){
            #if UNITY_SERVER || UNITY_EDITOR
            
            Dictionary<string,string> statBook = statBookFour; 
            //print("made it to fourth part");
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
           
                StaffBookFive(nconn, InstanceID, playerData, item, statBookFive, ID, statBookSix, stats);
             
            }, error =>{
                //print("Failed second part of fourth");

                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void StaffBookFive(NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookFive, string ID, Dictionary<string,string> statBookSix, Dictionary<string, string> stats){
            #if UNITY_SERVER || UNITY_EDITOR
            
            Dictionary<string,string> statBook = statBookFive; 
            //print("made it to Fifth part");

            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
                StaffBookSix(nconn, InstanceID, playerData, item, ID, statBookSix, stats);
            }, error =>{
                //print("Failed second part of Fifth");

                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void StaffBookSix(NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, string ID, Dictionary<string,string> statBookSix, Dictionary<string, string> stats){
            #if UNITY_SERVER || UNITY_EDITOR
            
            Dictionary<string,string> statBook = statBookSix; 
            //print("made it to Sixth part");
            //print($"{ID} is the ID and {InstanceID} is the item instance ID, {item.GetInstanceID()} is also the item instance ID");
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
                 PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                        {
                            CharacterId = ID,
                            ItemInstanceId = item.GetInstanceID(),
                            PlayFabId = playerData.PlayFabId,

                        }, result =>
                        {
                            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
                            List<CharacterStatListItem> charStatData = new List<CharacterStatListItem>();
                            List<CharacterSpellListItem> charSpellData = new List<CharacterSpellListItem>();

                            foreach (var statKey in stats) {
                                if (statKey.Key == "SPELLQ" || statKey.Key == "SPELLE" || statKey.Key == "SPELLR" || statKey.Key == "SPELLF") {
                                    // Create a CharacterSpellListItem and add it to charSpellData
                                    CharacterSpellListItem spell = new CharacterSpellListItem {
                                        Key = statKey.Key,
                                        Value = statKey.Value
                                    };
                                    charSpellData.Add(spell);
                                } else {
                                    // Create a CharacterStatListItem and add it to charStatData
                                    CharacterStatListItem stat = new CharacterStatListItem {
                                        Key = statKey.Key,
                                        Value = statKey.Value
                                    };
                                    charStatData.Add(stat);
                                }
                            }
                            List<CharacterInventoryListItem> charInventoryData = new List<CharacterInventoryListItem>();
                            CharacterInventoryListItem staff = (new CharacterInventoryListItem{
                                Key = item.GetInstanceID(),
                                Value = item
                            });
                            charInventoryData.Add(staff);
                            CharacterFullDataMessage newChar = (new CharacterFullDataMessage{
                                CharacterID = ID,
                                CharStatData = charStatData,
                                CharInventoryData = charInventoryData,
                                CharSpellData = charSpellData
                            });
                            stash.GetFullCharacterDataNew(newChar);
                            //print($"{item.GetItemName()} belongs to {item.GetOwnerID()}");
                        }, error =>{
                            Debug.Log(error.ErrorMessage);
                        });
                
            }, error =>{
                //print("Failed second part of Sixth");
                
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        //PartySelectedSave
        void SavePartyListAdding(NetworkConnectionToClient nconn, string member){
        #if UNITY_SERVER || UNITY_EDITOR
        
        PlayerInfo netMsg = (PlayerInfo)nconn.authenticationData;
        ScenePlayer p = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            p.AddPartyServer(member);
            string ZeroName = null;
            string OneName = null;
            string TwoName = null;
            string ThreeName = null;
            string FourName = null;
            string FiveName = null;
            List<string> IDs = new List<string>();
            foreach(var entry in p.GetParty())
            {
                IDs.Add(entry);
                if(ZeroName == null){
                    ZeroName = entry;
                    continue;
                }
                if(OneName == null){
                    OneName = entry;
                    continue;
                }
                if(TwoName == null){
                    TwoName = entry;
                    continue;
                }
                if(ThreeName == null){
                    ThreeName = entry;
                    continue;
                }
                if(FourName == null){
                    FourName = entry;
                    continue;
                }
                if(FiveName == null){
                    FiveName = entry;
                    continue;
                }
            }
            
            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = netMsg.PlayFabId,
                Data = new Dictionary<string, string>
                    {
                        {"PartyMemberZero", ZeroName}, {"PartyMemberOne", OneName},
                        {"PartyMemberTwo", TwoName}, {"PartyMemberThree", ThreeName},
                        {"PartyMemberFour", FourName}, {"PartyMemberFive", FiveName}
                    }
            }, result =>
            {

                netMsg.PartyIDs = IDs;
                nconn.authenticationData = netMsg;
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void SavePartyListRemoving(NetworkConnectionToClient nconn, string member){
            #if UNITY_SERVER || UNITY_EDITOR

            PlayerInfo netMsg = (PlayerInfo)nconn.authenticationData;
            ScenePlayer p = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            p.ServerRemovingPartymember(member);
            string ZeroName = null;
            string OneName = null;
            string TwoName = null;
            string ThreeName = null;
            string FourName = null;
            string FiveName = null;
            List<string> IDs = new List<string>();
            foreach(var entry in p.GetParty())
            {
                IDs.Add(entry);
                if(ZeroName == null){
                    ZeroName = entry;
                    continue;
                }
                if(OneName == null){
                    OneName = entry;
                    continue;
                }
                if(TwoName == null){
                    TwoName = entry;
                    continue;
                }
                if(ThreeName == null){
                    ThreeName = entry;
                    continue;
                }
                if(FourName == null){
                    FourName = entry;
                    continue;
                }
                if(FiveName == null){
                    FiveName = entry;
                    continue;
                }
            }
            
            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = netMsg.PlayFabId,
                Data = new Dictionary<string, string>
                    {
                        {"PartyMemberZero", ZeroName}, {"PartyMemberOne", OneName},
                        {"PartyMemberTwo", TwoName}, {"PartyMemberThree", ThreeName},
                        {"PartyMemberFour", FourName}, {"PartyMemberFive", FiveName}
                    }
            }, result =>
            {
                netMsg.PartyIDs = IDs;
                nconn.authenticationData = netMsg;
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        //Granting items from vendor
        void PurchasedVendorItem(NetworkConnectionToClient nconn, uint Price, string ID){
            #if UNITY_SERVER || UNITY_EDITOR

            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int StashItemCount = 0;
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();

            foreach(var itemKey in tactSheet.StashInventoryData){
                if(itemKey.Value.GetTacticianStash() && !itemKey.Value.GetNFT()){
                    StashItemCount ++;
                }
            }
            if(StashItemCount == 60){
                //print($"inventory is maxed at {StashItemCount} items!! should be 60");
                return;
            }
            List<string> items = new List<string>();
            items.Add(ID);
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                GetUserDKPCoins(nconn, items, playerData);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void GetUserDKPCoins(NetworkConnectionToClient nconn,List<string> items, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR

            PlayFabServerAPI.GetUserInventory(new GetUserInventoryRequest
            {
                PlayFabId = playerData.PlayFabId
            }, result =>
            {
                int DragonKillCoins = 0;
                Dictionary<string, int> findDKP = result.VirtualCurrency;
                foreach(var key in findDKP){
                    if(key.Key == "DK"){
                        DragonKillCoins = key.Value;
                        //print(DragonKillCoins);
                    }
                }
                GetItemPrices(nconn, items, playerData, DragonKillCoins);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void GetItemPrices(NetworkConnectionToClient nconn,List<string> items, PlayerInfo playerData, int DKCoins){
            #if UNITY_SERVER || UNITY_EDITOR

             PlayFabServerAPI.GetCatalogItems(new GetCatalogItemsRequest
            {
                CatalogVersion = "DragonKill_Characters_Bundles_Items"
            }, result =>
            {
                uint totalAmountDue = 0;
                foreach(var ID in items){
                    List<CatalogItem> ItemCatalog = result.Catalog;
                    foreach(CatalogItem i in ItemCatalog){
                        if(i.ItemId == ID){
                            Dictionary<string, uint> priceBook = i.VirtualCurrencyPrices;

                            foreach(var currencyCode in priceBook){
                                if(currencyCode.Key == "DK"){
                                    totalAmountDue = currencyCode.Value;
                                }
                            }
                        }
                    }
                }
                if(DKCoins < totalAmountDue){
                    //Pass not enough DKCoins message
                    ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
                    stash.TargetEnablePurchaseBtn();
                    return;
                }
                //print($"Total gold due = {totalAmountDue}");
                MakePayment(nconn, items, playerData, totalAmountDue);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void MakePayment(NetworkConnectionToClient nconn,List<string> items, PlayerInfo playerData, uint totalAmountDue){
            #if UNITY_SERVER || UNITY_EDITOR

            if(totalAmountDue > Int32.MaxValue){
                return;
            }
            int amountDueNow = checked((int)totalAmountDue);
            //print($"Total gold due right now = {amountDueNow}");
            PlayFabServerAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest
            {
                PlayFabId = playerData.PlayFabId,
                Amount = amountDueNow,
                VirtualCurrency = "DK"

            }, result =>
            {
                ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
                long cAmount = stash.Gold;
                long nAmount = cAmount - (long)totalAmountDue;
                if(nAmount > 0){
                    stash.Gold = nAmount;
                    //stash.GoldAmountSet(nAmount);
                    stash.TargetWalletAwake();
                }
                if(nAmount <= 0){
                    stash.GoldAmountSet(0);
                    stash.TargetWalletAwake();
                }
                //print($"Client paid for item in the amount of DK coins:{amountDueNow}");
                ReceivingPayment(nconn, items, playerData);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void ReceivingPayment(NetworkConnectionToClient nconn,List<string> items, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR

            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = items
            }, result =>
            {
                foreach(var item in result.ItemGrantResults){
                    TransformItemIntoDragonKill(nconn,playerData, item, "Stash", "Plain", 1, false, false, null);
                }
                ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
                stash.TargetEnablePurchaseBtn();
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        #if UNITY_SERVER || UNITY_EDITOR

    IEnumerator GetAllItemsInDragonKill(NetworkConnectionToClient nconn, PlayerInfo playerData)
    {

        List<string> ItemsOne = GetItemsListOne(); // Assuming this is a method that returns your 380 items.
        int itemsPerRequestOne = 5;
        bool finishedOne = false;
        for (int i = 0; i < ItemsOne.Count; i += itemsPerRequestOne)
        {
            List<string> itemsForThisRequest = ItemsOne.GetRange(i, Math.Min(itemsPerRequestOne, ItemsOne.Count - i));
            finishedOne = false; // Reset the flag for the new batch
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = itemsForThisRequest
            }, 
            result => 
            {
                StartCoroutine(ProcessItems(nconn, playerData, result.ItemGrantResults, null, () => 
                {
                    finishedOne = true; // Signal that this batch is finished
                }));
            }, 
            error => 
            {
                // Handle error
            });

            // Wait for the batch to finish before continuing
            while (!finishedOne)
            {
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(15f);
        List<string> ItemsTwo = GetItemsListTwo(); // Assuming this is a method that returns your 380 items.
        int itemsPerRequestTwo = 5;
        bool finishedTwo = false;
        for (int X = 0; X < ItemsTwo.Count; X += itemsPerRequestTwo)
        {
            List<string> itemsForThisRequest = ItemsTwo.GetRange(X, Math.Min(itemsPerRequestTwo, ItemsTwo.Count - X));
            finishedTwo = false; // Reset the flag for the new batch
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = itemsForThisRequest
            }, 
            result => 
            {
                StartCoroutine(ProcessItems(nconn, playerData, result.ItemGrantResults, null, () => 
                {
                    finishedTwo = true; // Signal that this batch is finishedTwo
                }));
            }, 
            error => 
            {
                // Handle error
            });

            // Wait for the batch to finish before continuing
            while (!finishedTwo)
            {
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(15f);
        List<string> ItemsThree = GetItemsListThree(); // Assuming this is a method that returns your 380 items.
        int itemsPerRequestThree = 5;
        bool finishedThree = false;
        for (int R = 0; R < ItemsThree.Count; R += itemsPerRequestThree)
        {
            List<string> itemsForThisRequest = ItemsThree.GetRange(R, Math.Min(itemsPerRequestThree, ItemsThree.Count - R));
            finishedThree = false; // Reset the flag for the new batch
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = itemsForThisRequest
            }, 
            result => 
            {
                StartCoroutine(ProcessItems(nconn, playerData, result.ItemGrantResults, null, () => 
                {
                    finishedThree = true; // Signal that this batch is finishedThree
                }));
            }, 
            error => 
            {
                // Handle error
            });

            // Wait for the batch to finish before continuing
            while (!finishedThree)
            {
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(15f);
        List<string> ItemsFour = GetItemsListFour(); // Assuming this is a method that returns your 380 items.
        int itemsPerRequestFour = 5;
        bool finishedFour = false;
        for (int P = 0; P < ItemsFour.Count; P += itemsPerRequestFour)
        {
            List<string> itemsForThisRequest = ItemsFour.GetRange(P, Math.Min(itemsPerRequestFour, ItemsFour.Count - P));
            finishedFour = false; // Reset the flag for the new batch
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = itemsForThisRequest
            }, 
            result => 
            {
                StartCoroutine(ProcessItems(nconn, playerData, result.ItemGrantResults, null, () => 
                {
                    finishedFour = true; // Signal that this batch is finishedFour
                }));
            }, 
            error => 
            {
                // Handle error
            });

            // Wait for the batch to finish before continuing
            while (!finishedFour)
            {
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(15f);
        List<string> ItemsFive = GetItemsListFive(); // Assuming this is a method that returns your 380 items.
        int itemsPerRequestFive = 5;
        bool finishedFive = false;
        for (int Z = 0; Z < ItemsFive.Count; Z += itemsPerRequestFive)
        {
            List<string> itemsForThisRequest = ItemsFive.GetRange(Z, Math.Min(itemsPerRequestFive, ItemsFive.Count - Z));
            finishedFive = false; // Reset the flag for the new batch
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = itemsForThisRequest
            }, 
            result => 
            {
                StartCoroutine(ProcessItems(nconn, playerData, result.ItemGrantResults, null, () => 
                {
                    finishedFive = true; // Signal that this batch is finishedFive
                }));
            }, 
            error => 
            {
                // Handle error
            });

            // Wait for the batch to finish before continuing
            while (!finishedFive)
            {
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(15f);
        List<string> ItemsSix = GetItemsListSix(); // Assuming this is a method that returns your 380 items.
        int itemsPerRequestSix = 5;
        bool finishedSix = false;
        for (int S = 0; S < ItemsSix.Count; S += itemsPerRequestSix)
        {
            List<string> itemsForThisRequest = ItemsSix.GetRange(S, Math.Min(itemsPerRequestSix, ItemsSix.Count - S));
            finishedSix = false; // Reset the flag for the new batch
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = itemsForThisRequest
            }, 
            result => 
            {
                StartCoroutine(ProcessItems(nconn, playerData, result.ItemGrantResults, null, () => 
                {
                    finishedSix = true; // Signal that this batch is finishedSix
                }));
            }, 
            error => 
            {
                // Handle error
            });

            // Wait for the batch to finish before continuing
            while (!finishedSix)
            {
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ProcessItems(NetworkConnectionToClient nconn, PlayerInfo playerData, List<GrantedItemInstance> items, Dictionary<string,string> nftDictionary, Action onComplete)
    {
        Dictionary<string, string> newNftDictionary = null;
        if(nftDictionary != null){
            newNftDictionary = nftDictionary;
            
        }
        foreach (var item in items)
        {
            print($"{item.ItemId} is item ID we are processing");
            string NFTID = "NotNFT";
            if(nftDictionary != null){
                string RemovingKey = string.Empty;
                foreach(var kvp in newNftDictionary){
                    if(kvp.Key == item.ItemId){
                        NFTID = kvp.Value;
                        RemovingKey = kvp.Key;
                        break;
                    }
                }
                if(newNftDictionary.ContainsKey(RemovingKey)){
                    newNftDictionary.Remove(RemovingKey);
                }
            }
            TransformItemIntoDragonKill(nconn, playerData, item, "Stash", "Plain", 1, true, false, NFTID);
            yield return new WaitForSeconds(1f);
        }
        onComplete();
    }
        #endif

    List<string> GetItemsListOne(){
            List<string> allItems = new List<string>
            {
                "Ammo01",
                "Ammo02",
                "Ammo03",
                "AxeT2",
                "AxeT3",
                "AxeT4",
                "AxeT5",
                "BowT2",
                "BowT3",
                "BowT4",
                "BowT5",
                "ClothArmsT2",
                "ClothArmsT3",
                "ClothArmsT4",
                "ClothArmsT5",
                "ClothChestT2",
                "ClothChestT3",
                "ClothChestT4",
                "ClothChestT5",
                "ClothFeetT2",
                "ClothFeetT3",
                "ClothFeetT4",
                "ClothFeetT5",
                "ClothHandsT2",
                "ClothHandsT3",
                "ClothHandsT4",
                "ClothHandsT5",
                "ClothHeadT2",
                "ClothHeadT3",
                "ClothHeadT4",
                "ClothHeadT5",
                "ClothLeggingsT2",
                "ClothLeggingsT3",
                "ClothLeggingsT4",
                "ClothLeggingsT5",
                "ClothShouldersT2",
                "ClothShouldersT3",
                "ClothShouldersT4",
                "ClothShouldersT5",
                "ClothWaistT2",
                "ClothWaistT3"
            };
            return allItems;
    }
        List<string> GetItemsListTwo(){
            List<string> allItems = new List<string>
            {
                "ClothWaistT4",
                "ClothWaistT5",
                "ClothWristsT2",
                "ClothWristsT3",
                "ClothWristsT4",
                "ClothWristsT5",
                "DaggerT2",
                "DaggerT3",
                "DaggerT4",
                "DaggerT5",
                "Earring01",
                "Earring02",
                "Earring03",
                "Earring04",
                "Earring05",
                "Earring06",
                "Earring07",
                "Earring08",
                "Earring09",
                "Earring10",
                "Earring11",
                "Earring12",
                "Earring13",
                "Earring14",
                "Earring15",
                "GreatAxeT2",
                "GreatAxeT3",
                "GreatAxeT4",
                "GreatAxeT5",
                "GreatHammerT2",
                "GreatHammerT3",
                "GreatHammerT4",
                "GreatHammerT5",
                "GreatSpearT2",
                "GreatSpearT3",
                "GreatSpearT4",
                "GreatSpearT5",
                "GreatSwordT2",
                "GreatSwordT3",
                "GreatSwordT4",
                "GreatSwordT5"
            };
            return allItems;
        }
        List<string> GetItemsListThree(){
            List<string> allItems = new List<string>
            {
                "LeatherArmsT2",
                "LeatherArmsT3",
                "LeatherArmsT4",
                "LeatherArmsT5",
                "LeatherChestT2",
                "LeatherChestT3",
                "LeatherChestT4",
                "LeatherChestT5",
                "LeatherFeetT2",
                "LeatherFeetT3",
                "LeatherFeetT4",
                "LeatherFeetT5",
                "LeatherHandsT2",
                "LeatherHandsT3",
                "LeatherHandsT4",
                "LeatherHandsT5",
                "LeatherHeadT2",
                "LeatherHeadT3",
                "LeatherHeadT4",
                "LeatherHeadT5",
                "LeatherLeggingsT2",
                "LeatherLeggingsT3",
                "LeatherLeggingsT4",
                "LeatherLeggingsT5",
                "LeatherShouldersT2",
                "LeatherShouldersT3",
                "LeatherShouldersT4",
                "LeatherShouldersT5",
                "LeatherWaistT2",
                "LeatherWaistT3",
                "LeatherWaistT4",
                "LeatherWaistT5",
                "LeatherWristsT2",
                "LeatherWristsT3",
                "LeatherWristsT4",
                "LeatherWristsT5",
                "MaceT2",
                "MaceT3",
                "MaceT4",
                "MaceT5",
                "Necklace01"
            };
            return allItems;
        }
        List<string> GetItemsListFour(){
            List<string> allItems = new List<string>
            {
                "Necklace02",
                "Necklace03",
                "Necklace04",
                "Necklace05",
                "Necklace06",
                "Necklace07",
                "Necklace08",
                "Necklace09",
                "Necklace10",
                "Necklace11",
                "Necklace12",
                "Necklace13",
                "Necklace14",
                "Necklace15",
                "PlateArmsT2",
                "PlateArmsT3",
                "PlateArmsT4",
                "PlateArmsT5",
                "PlateChestT2",
                "PlateChestT3",
                "PlateChestT4",
                "PlateChestT5",
                "PlateFeetT2",
                "PlateFeetT3",
                "PlateFeetT4",
                "PlateFeetT5",
                "PlateHandsT2",
                "PlateHandsT3",
                "PlateHandsT4",
                "PlateHandsT5",
                "PlateHeadT2",
                "PlateHeadT3",
                "PlateHeadT4",
                "PlateHeadT5",
                "PlateLeggingsT2",
                "PlateLeggingsT3",
                "PlateLeggingsT4",
                "PlateLeggingsT5",
                "PlateShouldersT2",
                "PlateShouldersT3",
                "PlateShouldersT4"
            };
            return allItems;
        }
        List<string> GetItemsListFive(){
            List<string> allItems = new List<string>();
            allItems.Add("PlateShouldersT5");
            allItems.Add("PlateWaistT2");
            allItems.Add("PlateWaistT3");
            allItems.Add("PlateWaistT4");
            allItems.Add("PlateWaistT5");
            allItems.Add("PlateWristsT2");
            allItems.Add("PlateWristsT3");
            allItems.Add("PlateWristsT4");
            allItems.Add("PlateWristsT5");
            allItems.Add("Quiver01");
            allItems.Add("Ring01");
            allItems.Add("Ring02");
            allItems.Add("Ring03");
            allItems.Add("Ring04");
            allItems.Add("Ring05");
            allItems.Add("Ring06");
            allItems.Add("Ring07");
            allItems.Add("Ring08");
            allItems.Add("Ring09");
            allItems.Add("Ring10");
            allItems.Add("Ring11");
            allItems.Add("Ring12");
            allItems.Add("Ring13");
            allItems.Add("Ring14");
            allItems.Add("Ring15");
            allItems.Add("ShieldT2");
            allItems.Add("ShieldT3");
            allItems.Add("ShieldT4");
            allItems.Add("ShieldT5");
            allItems.Add("ShieldT5-2");
            allItems.Add("SpearT2");
            allItems.Add("SpearT3");
            allItems.Add("SpearT4");
            allItems.Add("SpearT5");
            allItems.Add("StaffT2");
            allItems.Add("StaffT3");
            allItems.Add("StaffT4");
            allItems.Add("StaffT5");
            allItems.Add("SwordT2");
            allItems.Add("SwordT3");
            allItems.Add("SwordT4");
            allItems.Add("SwordT5");
            allItems.Add("TomeT2");
            allItems.Add("TomeT3");
            allItems.Add("TomeT4");
            allItems.Add("TomeT5");
            allItems.Add("TacticianArmsT1");
            allItems.Add("TacticianArmsT2");
            allItems.Add("TacticianChestT1");
            allItems.Add("TacticianChestT2");
            allItems.Add("TacticianFeetT1");
            return allItems;
        }
        List<string> GetItemsListSix(){
            //string SpearOfDragonslaying = "NFT_SpearOfDragonslaying";
            //string SwordOfFire = "NFT_SwordOfFire";
            //string AcidicAxe = "NFT_AcidicAxe";
            //string MaceOfHealing = "NFT_MaceOfHealing";
            //string BowOfPower = "NFT_BowOfPower";
            //string FrozenGreatsword = "NFT_FrozenGreatsword";
            //string StaffOfProtection = "NFT_StaffOfProtection";
            //string GreatspearOfDragonSlaying = "NFT_GreatspearOfDragonslaying";
            //string ThunderInfusedGreathammer = "NFT_ThunderInfusedGreathammer";
            //string VampiricDagger = "NFT_VampiricDagger";
            //string VenomousGreataxe = "NFT_VenomousGreataxe";
            //string RingOfTheTactician = "TACTNFT_RingOfTheTactician";
            List<string> allItems = new List<string>();
            allItems.Add("Ammo01");
            allItems.Add("TomeT5-2");
            allItems.Add("TacticianFeetT2");
            allItems.Add("TacticianHandsT1");
            allItems.Add("TacticianHandsT2");
            allItems.Add("TacticianHeadT1");
            allItems.Add("TacticianHeadT2");
            allItems.Add("TacticianLeggingsT1");
            allItems.Add("TacticianLeggingsT2");
            allItems.Add("TacticianShouldersT1");
            allItems.Add("TacticianShouldersT2");
            //allItems.Add(RingOfTheTactician);
            //allItems.Add(SpearOfDragonslaying);
            //allItems.Add(StaffOfProtection);
            //allItems.Add(SwordOfFire);
            //allItems.Add(AcidicAxe);
            //allItems.Add(MaceOfHealing);
            //allItems.Add(BowOfPower);
            //allItems.Add(FrozenGreatsword);
            //allItems.Add(GreatspearOfDragonSlaying);
            //allItems.Add(ThunderInfusedGreathammer);
            //allItems.Add(VampiricDagger);
            //allItems.Add(VenomousGreataxe);
            return allItems;
        }
        #if UNITY_SERVER || UNITY_EDITOR
    
    void TransformItemIntoDragonKill(NetworkConnectionToClient nconn, PlayerInfo playerData, GrantedItemInstance item, string selector, string Quality, int quant, bool login, bool Unstacking, string nftID){
        //print($"{item.DisplayName} is the item that was setn to transform, {selector} is who we want to own this item");
        string Weight = "0";
        string STRENGTH_item = "0";
        string AGILITY_item = "0";
        string FORTITUDE_item = "0";
        string ARCANA_item = "0";
        string Rarity_item = "0";
        string MagicResist_item = "0";
        string FireResist_item = "0";
        string ColdResist_item = "0";
        string PoisonResist_item = "0";
        string DiseaseResist_item = "0";
        string Armor = "0";
        string DamageMin_item = "0";
        string DamageMax_item = "0";
        string Parry_item = "0";
        string Penetration_item= "0";
        string AttackDelay_item = "0";
        string BlockChance_item = "0";
        string BlockValue_item = "0";
        string TypeOfItem = "0";
        string Itemname = "0";
        string Item_Slot = "0";
        //string ItemDescript = "0";
        Dictionary<string,string> statBookOne = new Dictionary<string, string>();
        Dictionary<string,string> statBookTwo = new Dictionary<string, string>();
        Dictionary<string,string> statBookThree = new Dictionary<string, string>();
        Dictionary<string,string> statBookFour = new Dictionary<string, string>();
        Dictionary<string,string> statBookFive = new Dictionary<string, string>();
        Dictionary<string,string> statBookSix = new Dictionary<string, string>();
        ItemSelectable.ItemType type = ItemSelectable.ItemType.None;
        //NFT
        if(item.DisplayName == "RingOfTheTactician"){
            type = ItemSelectable.ItemType.NFTTacticianRing;
            Item_Slot = "Ring";
            Itemname = "Ring Of The Tactician";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "NFT";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "The Ring Of A True Tactician";
            TypeOfItem = "NFT Tactician Ring";
			}
        if(item.DisplayName == "SwordOfFire"){
            type = ItemSelectable.ItemType.NFTSingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Sword Of Fire";
            Weight = "2";
            STRENGTH_item = "2";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "NFT";
            MagicResist_item = "0";
            FireResist_item = "5";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "1";
            DamageMax_item = "60";
            Parry_item = "15";
            Penetration_item = "2";
            AttackDelay_item = "90";
            TypeOfItem = "NFT Sword";
        }
        if(item.DisplayName == "AcidicAxe"){
            type = ItemSelectable.ItemType.NFTSingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Acidic Axe";
            Weight = "3";
            STRENGTH_item = "2";
            AGILITY_item = "3"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "NFT";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "5";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "15";
            DamageMax_item = "50";
            Parry_item = "2";
            Penetration_item = "17";
            AttackDelay_item = "95";
            TypeOfItem = "NFT Axe";
        }
        if(item.DisplayName == "SpearOfDragonslaying"){
            type = ItemSelectable.ItemType.NFTSingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Spear Of Dragonslaying";
            Weight = "2";
            STRENGTH_item = "2";
            AGILITY_item = "2"; 
            FORTITUDE_item = "2";
            ARCANA_item = "1";
            Rarity_item = "NFT";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            DamageMin_item = "25";
            DamageMax_item = "45";
            Parry_item = "7";
            Penetration_item = "6";
            AttackDelay_item = "105";
            TypeOfItem = "NFT Spear";
        }
        if(item.DisplayName == "MaceOfHealing"){
            type = ItemSelectable.ItemType.NFTSingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Mace Of Healing";
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "NFT";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "35";
            DamageMax_item = "40";
            Parry_item = "5";
            Penetration_item = "4";
            AttackDelay_item = "110";
            TypeOfItem = "NFT Mace";
        }
        if(item.DisplayName == "VampiricDagger"){
            type = ItemSelectable.ItemType.NFTSingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Vampiric Dagger";
            Weight = "0.5";
            STRENGTH_item = "1";
            AGILITY_item = "3"; 
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "NFT";
            MagicResist_item = "10";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "10";
            DamageMax_item = "40";
            Parry_item = "10";
            Penetration_item = "5";
            AttackDelay_item = "70";
            TypeOfItem = "NFT Dagger";
        }
        if(item.DisplayName == "StaffOfProtection"){
            type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Staff Of Protection";
                
            Weight = "3";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "NFT";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "0";
            DamageMin_item = "20";
            DamageMax_item = "35";
            Parry_item = "15";
            Penetration_item = "1";
            AttackDelay_item = "75";
            TypeOfItem = "NFT Staff";
        }
        if(item.DisplayName == "BowOfPower"){
            type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Bow Of Power";
                
            Weight = "1";
            STRENGTH_item = "1";
            AGILITY_item = "3"; 
            FORTITUDE_item = "2";
            ARCANA_item = "2";
            Rarity_item = "NFT";
            MagicResist_item = "10";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "5";
            DamageMax_item = "45";
            Parry_item = "0";
            Penetration_item = "0";
            AttackDelay_item = "100";
            TypeOfItem = "NFT Bow";
        }
        if(item.DisplayName == "ThunderInfusedGreathammer"){
            type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Thunder Infused Greathammer";
                
            Weight = "8";
            STRENGTH_item = "5";
            AGILITY_item = "3"; 
            FORTITUDE_item = "2";
            ARCANA_item = "2";
            Rarity_item = "NFT";
            MagicResist_item = "10";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "75";
            DamageMax_item = "95";
            Parry_item = "5";
            Penetration_item = "14";
            AttackDelay_item = "190";
            TypeOfItem = "NFT Greathammer";
        }
        if(item.DisplayName == "VenomousGreataxe"){
            type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Venomous Greataxe";
                
            Weight = "7";
            STRENGTH_item = "5";
            AGILITY_item = "3"; 
            FORTITUDE_item = "2";
            ARCANA_item = "2";
            Rarity_item = "NFT";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "10";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "40";
            DamageMax_item = "110";
            Parry_item = "2";
            Penetration_item = "32";
            AttackDelay_item = "175";
            TypeOfItem = "NFT Greataxe";
        }
        if(item.DisplayName == "FrozenGreatsword"){
            type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Frozen Greatsword";
                
            Weight = "6";
            STRENGTH_item = "5";
            AGILITY_item = "5"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "NFT";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "10";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "5";
            DamageMax_item = "130";
            Parry_item = "10";
            Penetration_item = "8";
            AttackDelay_item = "170";
            TypeOfItem = "NFT Greatsword";
        }
        if(item.DisplayName == "GreatspearOfDragonslaying"){
            type = ItemSelectable.ItemType.NFTTwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Greatspear Of Dragonslaying";
                
            Weight = "3.5";
            STRENGTH_item = "5";
            AGILITY_item = "5"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "NFT";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "0";
            DamageMin_item = "55";
            DamageMax_item = "105";
            Parry_item = "7";
            Penetration_item = "20";
            AttackDelay_item = "185";
            TypeOfItem = "NFT Greatspear";
        }
        //regular items
        if(item.DisplayName == "Arrow"){
            type = ItemSelectable.ItemType.Ammo;
            Item_Slot = "Cannot Equip";
            Itemname = "Arrow";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Component for the archers attack";
            TypeOfItem = "Ability Item";
        }
        if(item.DisplayName == "Bandage"){
            type = ItemSelectable.ItemType.Misc;
            Item_Slot = "Cannot Equip";
            Itemname = "Bandage";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Component for the archers bandage ability";
            TypeOfItem = "Ability Item";
        }
        if(item.DisplayName == "Shuriken"){
            type = ItemSelectable.ItemType.Ammo;
            Item_Slot = "Cannot Equip";
            Itemname = "Shuriken";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Component for the rogue shuriken ability";
            TypeOfItem = "Ability Item";
        }
        if(item.DisplayName == "LockPicks"){
            type = ItemSelectable.ItemType.LockPick;
            Item_Slot = "Cannot Equip";
            Itemname = "Lock Picks";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Required to pick locks";
            TypeOfItem = "Ability Item";
        }
        if(item.DisplayName == "BlindPowder"){
            type = ItemSelectable.ItemType.Misc;
            Item_Slot = "Cannot Equip";
            Itemname = "Blind Powder";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Component for the rogues blind ability";
            TypeOfItem = "Ability Item";
        }
        if(item.DisplayName == "ThrowingStone"){
            type = ItemSelectable.ItemType.Ammo;
            Item_Slot = "Cannot Equip";
            Itemname = "Throwing Stone";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Component for the fighters throw stone ability";
            TypeOfItem = "Ability Item";
        }
        if(item.DisplayName == "Torch"){
            type = ItemSelectable.ItemType.OffHand;
            Item_Slot = "OffHand";
            Itemname = "Torch";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Helps you see farther";
            TypeOfItem = "Offhand";
        }
        if(item.DisplayName == "Agate"){
            type = ItemSelectable.ItemType.GemstoneT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Agate";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 4 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Carnelian"){
            type = ItemSelectable.ItemType.GemstoneT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Carnelian";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 4 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Jade"){
            type = ItemSelectable.ItemType.GemstoneT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Jade";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 3 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Moonstone"){
            type = ItemSelectable.ItemType.GemstoneT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Moonstone";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 3 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Mythril"){
            type = ItemSelectable.ItemType.GemstoneT5;
            Item_Slot = "Cannot Equip";
            Itemname = "Mythril";
                
            Weight = "10";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 5 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Onyx"){
            type = ItemSelectable.ItemType.GemstoneT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Onyx";
                
            Weight = "5";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 4 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Opal"){
            type = ItemSelectable.ItemType.GemstoneT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Opal";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 3 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Titanite"){
            type = ItemSelectable.ItemType.GemstoneT5;
            Item_Slot = "Cannot Equip";
            Itemname = "Titanite";
                
            Weight = "10";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 5 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Topaz"){
            type = ItemSelectable.ItemType.GemstoneT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Topaz";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 3 gemstone used to make jewelry";
            TypeOfItem = "Gemstone";
        }
        if(item.DisplayName == "Amber"){
            type = ItemSelectable.ItemType.GemstoneT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Amber";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 2 gemstone used to make jewelry";
            TypeOfItem = "GemStone";
        }
        if(item.DisplayName == "LapisLazuli"){
            type = ItemSelectable.ItemType.GemstoneT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Lapis Lazuli";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 2 gemstone used to make jewelry";
            TypeOfItem = "GemStone";
        }
        if(item.DisplayName == "RoseQuartz"){
            type = ItemSelectable.ItemType.GemstoneT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Rose Quartz";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 2 gemstone used to make jewelry";
            TypeOfItem = "GemStone";
        }
        if(item.DisplayName == "Sapphire"){
            type = ItemSelectable.ItemType.GemstoneT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Sapphire";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 2 gemstone used to make jewelry";
            TypeOfItem = "GemStone";
        }
        if(item.DisplayName == "Ruby"){
            type = ItemSelectable.ItemType.GemstoneT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Ruby";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 2 gemstone used to make jewelry";
            TypeOfItem = "GemStone";
        }
        if(item.DisplayName == "Ore"){
            type = ItemSelectable.ItemType.MaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Ore";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 1 Unrefined resource";
            TypeOfItem = "Resource";
        }
        if(item.DisplayName == "Stone"){
            type = ItemSelectable.ItemType.MaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Stone";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 1 Unrefined resource";
            TypeOfItem = "Resource";
        }
        if(item.DisplayName == "Ingot"){
            type = ItemSelectable.ItemType.RefinedMaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Ingot";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 1 Refined resource";
            TypeOfItem = "Resource";
        }
        if(item.DisplayName == "Block"){
            type = ItemSelectable.ItemType.RefinedMaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Block";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 1 Refined resource";
            TypeOfItem = "Resource";
        }
        if(item.DisplayName == "BottleOfPoison"){
            type = ItemSelectable.ItemType.Misc;
            Item_Slot = "Cannot Equip";
            Itemname = "Bottle Of Poison";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Bottle of poison";
            TypeOfItem = "Ability Item";
        }
        if(item.DisplayName == "Plank"){
            type = ItemSelectable.ItemType.RefinedMaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Plank";
                
            Weight = "0.8";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 1 Refined resource";
            TypeOfItem = "Resource";
        }
        if(item.DisplayName == "Wood"){
            type = ItemSelectable.ItemType.MaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Wood";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tier 1 Unrefined resource";
            TypeOfItem = "Resource";
        }
            
        if(item.DisplayName == "StoneOfLife"){
            type = ItemSelectable.ItemType.Misc;
            Item_Slot = "Cannot Equip";
            Itemname = "Stone Of Life";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Bring back to life";
            TypeOfItem = "GemStone";
        }
        //PlateStart
        if(item.DisplayName == "PlateBreastplate"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Plate Breastplate";
                
            Weight = "18";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "5";
            //ItemDescript = "";
            TypeOfItem = "Plate Chest";
        }
            
        if(item.DisplayName == "PlateLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Plate Leggings";
                
            Weight = "12";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "";
            TypeOfItem = "Plate Leggings";
        }
        if(item.DisplayName == "PlateHelmet"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Plate Helmet";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Plate Head";
        }
        if(item.DisplayName == "PlateBoots"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Plate Boots";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Plate Feet";
        }
        if(item.DisplayName == "PlateBracer"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Plate Bracers";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Plate Wrists";
        }
        if(item.DisplayName == "PlateSpaulders"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Plate Spaulders";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Plate Shoulders";
        }
        if(item.DisplayName == "PlateVambraces"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Plate Vambraces";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Plate Arms";
        }
        if(item.DisplayName == "PlateGauntlets"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Plate Gauntlets";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Plate Hands";
        }
        if(item.DisplayName == "PlateBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Plate Belt";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Plate Waist";
        }
        //LeatherStart
        if(item.DisplayName == "LeatherTunic"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Leather Tunic";
                
            Weight = "12";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "";
            TypeOfItem = "Leather Chest";
        }
        if(item.DisplayName == "LeatherLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Leather Leggings";
                
            Weight = "8";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Leather Leggings";
        }
        if(item.DisplayName == "LeatherCap"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Leather Cap";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Leather Head";
        }
        if(item.DisplayName == "LeatherShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Leather Shoes";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Leather Feet";
        }
        if(item.DisplayName == "LeatherBracers"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Leather Bracers";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Leather Wrists";
        }
        if(item.DisplayName == "LeatherShoulderpads"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Leather Shoulderpads";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Leather Shoulders";
        }
        if(item.DisplayName == "LeatherSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Leather Sleeves";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Leather Arms";
        }
        if(item.DisplayName == "LeatherGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Leather Gloves";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Leather Hands";
        }
        if(item.DisplayName == "LeatherBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Leather Belt";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Leather Waist";
        }
        //ClothStart
        if(item.DisplayName == "ClothRobe"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Cloth Robe";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "2";
            //ItemDescript = "";
            TypeOfItem = "Cloth Chest";
        }
        if(item.DisplayName == "ClothPants"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Cloth Pants";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Cloth Leggings";
        }
        if(item.DisplayName == "ClothHood"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Cloth Hood";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "";
            TypeOfItem = "Cloth Head";
        }
        if(item.DisplayName == "ClothShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Cloth Shoes";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "";
            TypeOfItem = "Cloth Feet";
        }
        if(item.DisplayName == "ClothCuffs"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Cloth Cuffs";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Cloth Wrists";
        }
        if(item.DisplayName == "ClothMantle"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Cloth Mantle";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "";
            TypeOfItem = "Cloth Shoulders";
        }
        if(item.DisplayName == "ClothSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Cloth Sleeves";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "";
            TypeOfItem = "Cloth Arms";
        }
        if(item.DisplayName == "ClothGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Cloth Gloves";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "";
            TypeOfItem = "Cloth Hands";
        }
        if(item.DisplayName == "ClothCord"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Cloth Cord";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "";
            TypeOfItem = "Cloth Waist";
        }
        //Shields
        if(item.DisplayName == "SmallShield"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Small Shield";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "5"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            BlockChance_item = "25";
            BlockValue_item = "10";
            //ItemDescript = "";
            TypeOfItem = "Shield";
                
        }
        if(item.DisplayName == "MediumShield"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Medium Shield";
                
            Weight = "5";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            BlockChance_item = "20";
            BlockValue_item = "13";
            //ItemDescript = "";
            TypeOfItem = "Shield";
        }
        if(item.DisplayName == "NoviceTome"){
            type = ItemSelectable.ItemType.OffHand;
            Item_Slot = "OffHand";
            Itemname = "Novice's Tome";
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "Common";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "0";
            //ItemDescript = "";
            TypeOfItem = "OffHand";
        }
        if(item.DisplayName == "Sword"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Sword";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "1";
            DamageMax_item = "60";
            Parry_item = "15";
            Penetration_item = "2";
            AttackDelay_item = "90";
            //ItemDescript = "";
            TypeOfItem = "Sword";
                
        }
        if(item.DisplayName == "Axe"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Axe";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "15";
            DamageMax_item = "50";
            Parry_item = "2";
            Penetration_item = "12";
            AttackDelay_item = "95";
            //ItemDescript = "";
            TypeOfItem = "Axe";
                
        }
        if(item.DisplayName == "Spear"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Spear";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "25";
            DamageMax_item = "45";
            Parry_item = "7";
            Penetration_item = "6";
            AttackDelay_item = "105";
            //ItemDescript = "";
            TypeOfItem = "Spear";
                
        }
        if(item.DisplayName == "Mace"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Mace";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "35";
            DamageMax_item = "40";
            Parry_item = "5";
            Penetration_item = "4";
            AttackDelay_item = "110";
            //ItemDescript = "";
            TypeOfItem = "Mace";
                
        }
        if(item.DisplayName == "Dagger"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Dagger";
                
            Weight = "0.5";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "10";
            DamageMax_item = "40";
            Parry_item = "10";
            Penetration_item = "5";
            AttackDelay_item = "70";
            //ItemDescript = "";
            TypeOfItem = "Dagger";
                
        }
        if(item.DisplayName == "Bow"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Bow";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "5";
            DamageMax_item = "45";
            Parry_item = "5";
            Penetration_item = "0";
            AttackDelay_item = "100";
            //ItemDescript = "";
            TypeOfItem = "Bow";
        }
        if(item.DisplayName == "Greathammer"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Greathammer";
                
            Weight = "8";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "75";
            DamageMax_item = "95";
            Parry_item = "5";
            Penetration_item = "14";
            AttackDelay_item = "190";
            //ItemDescript = "";
            TypeOfItem = "Greathammer";
        }
        if(item.DisplayName == "Greataxe"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Greataxe";
                
            Weight = "7";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "40";
            DamageMax_item = "110";
            Parry_item = "5";
            Penetration_item = "26";
            AttackDelay_item = "175";
            //ItemDescript = "";
            TypeOfItem = "Greataxe";
        }
        if(item.DisplayName == "Greatsword"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Greatsword";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "5";
            DamageMax_item = "130";
            Parry_item = "10";
            Penetration_item = "8";
            AttackDelay_item = "170";
            //ItemDescript = "";
            TypeOfItem = "Greatsword";
        }
        if(item.DisplayName == "Greatspear"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Greatspear";
                
            Weight = "3.5";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "55";
            DamageMax_item = "105";
            Parry_item = "7";
            Penetration_item = "20";
            AttackDelay_item = "185";
            //ItemDescript = "";
            TypeOfItem = "Greatspear";
        }
        if(item.DisplayName == "Squire'sSword"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Squire's Sword";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "2";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "3";
            DamageMax_item = "65";
            Parry_item = "15";
            Penetration_item = "2";
            AttackDelay_item = "90";
            //ItemDescript = "";
            TypeOfItem = "Sword";
			}
			if(item.DisplayName == "Squire'sAxe"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Squire's Axe";
                
            Weight = "3";
            STRENGTH_item = "2";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "2";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "18";
            DamageMax_item = "53";
            Parry_item = "2";
            Penetration_item = "13";
            AttackDelay_item = "95";
            //ItemDescript = "";
            TypeOfItem = "Axe";
			}
			if(item.DisplayName == "Squire'sSpear"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Squire's Spear";
                
            Weight = "2";
            STRENGTH_item = "10";
            AGILITY_item = "5";
            FORTITUDE_item = "0";
            ARCANA_item = "10";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "28";
            DamageMax_item = "48";
            Parry_item = "7";
            Penetration_item = "5";
            AttackDelay_item = "105";
            //ItemDescript = "";
            TypeOfItem = "Spear";
			}
			if(item.DisplayName == "Squire'sMace"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Squire's Mace";
                
            Weight = "4";
            STRENGTH_item = "3";
            AGILITY_item = "2";
            FORTITUDE_item = "2";
            ARCANA_item = "1";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "38";
            DamageMax_item = "43";
            Parry_item = "5";
            Penetration_item = "7";
            AttackDelay_item = "110";
            //ItemDescript = "";
            TypeOfItem = "Mace";
			}
			if(item.DisplayName == "Squire'sDagger"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Squire's Dagger";
                
            Weight = "0.5";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "13";
            DamageMax_item = "43";
            Parry_item = "10";
            Penetration_item = "5";
            AttackDelay_item = "70";
            //ItemDescript = "";
            TypeOfItem = "Dagger";
			}
			if(item.DisplayName == "Squire'sStaff"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Squire's Staff";
                
            Weight = "3";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "3";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "23";
            DamageMax_item = "38";
            Parry_item = "15";
            Penetration_item = "1";
            AttackDelay_item = "75";
            //ItemDescript = "";
            TypeOfItem = "Staff";
			}
			if(item.DisplayName == "Squire'sBow"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Squire's Bow";
                
            Weight = "1";
            STRENGTH_item = "2";
            AGILITY_item = "4";
            FORTITUDE_item = "3";
            ARCANA_item = "2";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "8";
            DamageMax_item = "48";
            Parry_item = "0";
            Penetration_item = "1";
            AttackDelay_item = "100";
            //ItemDescript = "";
            TypeOfItem = "Bow";
			}
			if(item.DisplayName == "Squire'sGreathammer"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Squire's Greathammer";
                
            Weight = "8";
            STRENGTH_item = "4";
            AGILITY_item = "3";
            FORTITUDE_item = "3";
            ARCANA_item = "2";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "81";
            DamageMax_item = "101";
            Parry_item = "5";
            Penetration_item = "14";
            AttackDelay_item = "190";
            //ItemDescript = "";
            TypeOfItem = "Greathammer";
			}
			if(item.DisplayName == "Squire'sGreataxe"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Squire's Greataxe";
                
            Weight = "7";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "46";
            DamageMax_item = "116";
            Parry_item = "2";
            Penetration_item = "27";
            AttackDelay_item = "175";
            //ItemDescript = "";
            TypeOfItem = "Greataxe";
			}
			if(item.DisplayName == "Squire'sGreatsword"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Squire's Greatsword";
                
            Weight = "6";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "2";
            ARCANA_item = "1";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "11";
            DamageMax_item = "136";
            Parry_item = "10";
            Penetration_item = "8";
            AttackDelay_item = "170";
            //ItemDescript = "";
            TypeOfItem = "Greatsword";
			}
			if(item.DisplayName == "Squire'sGreatspear"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Squire's Greatspear";
                
            Weight = "3.5";
            STRENGTH_item = "15";
            AGILITY_item = "10";
            FORTITUDE_item = "5";
            ARCANA_item = "15";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "75";
            DamageMax_item = "95";
            Parry_item = "7";
            Penetration_item = "20";
            AttackDelay_item = "185";
            //ItemDescript = "";
            TypeOfItem = "Greatspear";
			}
			if(item.DisplayName == "Knight'sSword"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Knight's Sword";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "5";
            ARCANA_item = "3";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "6";
            DamageMax_item = "70";
            Parry_item = "15";
            Penetration_item = "3";
            AttackDelay_item = "90";
            //ItemDescript = "";
            TypeOfItem = "Sword";
			}
			if(item.DisplayName == "Knight'sAxe"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Knight's Axe";
                
            Weight = "3";
            STRENGTH_item = "5";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "3";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "20";
            DamageMax_item = "55";
            Parry_item = "2";
            Penetration_item = "14";
            AttackDelay_item = "95";
            //ItemDescript = "";
            TypeOfItem = "Axe";
			}
			if(item.DisplayName == "Knight'sSpear"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Knight's Spear";
                
            Weight = "2";
            STRENGTH_item = "15";
            AGILITY_item = "10";
            FORTITUDE_item = "5";
            ARCANA_item = "15";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "29";
            DamageMax_item = "51";
            Parry_item = "7";
            Penetration_item = "5";
            AttackDelay_item = "105";
            //ItemDescript = "";
            TypeOfItem = "Spear";
			}
			if(item.DisplayName == "Knight'sMace"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Knight's Mace";
                
            Weight = "4";
            STRENGTH_item = "4";
            AGILITY_item = "2";
            FORTITUDE_item = "2";
            ARCANA_item = "2";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "39";
            DamageMax_item = "45";
            Parry_item = "5";
            Penetration_item = "7";
            AttackDelay_item = "110";
            //ItemDescript = "";
            TypeOfItem = "Mace";
			}
			if(item.DisplayName == "Rogue'sDagger"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Rogue's Dagger";
                
            Weight = "0.5";
            STRENGTH_item = "6";
            AGILITY_item = "6";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "14";
            DamageMax_item = "45";
            Parry_item = "10";
            Penetration_item = "6";
            AttackDelay_item = "70";
            //ItemDescript = "";
            TypeOfItem = "Dagger";
			}
			if(item.DisplayName == "Scholar'sStaff"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Scholar's Staff";
                
            Weight = "3";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "25";
            DamageMax_item = "39";
            Parry_item = "15";
            Penetration_item = "2";
            AttackDelay_item = "75";
            //ItemDescript = "";
            TypeOfItem = "Staff";
			}
			if(item.DisplayName == "Archer'sBow"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Archer's Bow";
                
            Weight = "1";
            STRENGTH_item = "2";
            AGILITY_item = "11";
            FORTITUDE_item = "3";
            ARCANA_item = "6";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "12";
            DamageMax_item = "55";
            Parry_item = "0";
            Penetration_item = "1";
            AttackDelay_item = "100";
            //ItemDescript = "";
            TypeOfItem = "Bow";
			}
			if(item.DisplayName == "Knight'sGreathammer"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Knight's Greathammer";
                
            Weight = "8";
            STRENGTH_item = "7";
            AGILITY_item = "3";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "86";
            DamageMax_item = "107";
            Parry_item = "5";
            Penetration_item = "14";
            AttackDelay_item = "190";
            //ItemDescript = "";
            TypeOfItem = "Greathammer";
			}
			if(item.DisplayName == "Knight'sGreataxe"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Knight's Greataxe";
                
            Weight = "7";
            STRENGTH_item = "10";
            AGILITY_item = "0";
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "51";
            DamageMax_item = "122";
            Parry_item = "2";
            Penetration_item = "28";
            AttackDelay_item = "175";
            //ItemDescript = "";
            TypeOfItem = "Greataxe";
			}
			if(item.DisplayName == "Knight'sGreatsword"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Knight's Greatsword";
                
            Weight = "6";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "2";
            ARCANA_item = "2";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "16";
            DamageMax_item = "142";
            Parry_item = "10";
            Penetration_item = "9";
            AttackDelay_item = "170";
            //ItemDescript = "";
            TypeOfItem = "Greatsword";
			}
			if(item.DisplayName == "Knight'sGreatspear"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Knight's Greatspear";
                
            Weight = "3.5";
            STRENGTH_item = "20";
            AGILITY_item = "15";
            FORTITUDE_item = "10";
            ARCANA_item = "20";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "66";
            DamageMax_item = "115";
            Parry_item = "7";
            Penetration_item = "20";
            AttackDelay_item = "185";
            //ItemDescript = "";
            TypeOfItem = "Greatspear";
			}
			if(item.DisplayName == "Myrmidon'sSword"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Myrmidon's Sword";
                
            Weight = "2";
            STRENGTH_item = "2";
            AGILITY_item = "2";
            FORTITUDE_item = "10";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "9";
            DamageMax_item = "75";
            Parry_item = "15";
            Penetration_item = "3";
            AttackDelay_item = "90";
            //ItemDescript = "";
            TypeOfItem = "Sword";
			}
			if(item.DisplayName == "Myrmidon'sAxe"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Myrmidon's Axe";
                
            Weight = "3";
            STRENGTH_item = "10";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "25";
            DamageMax_item = "60";
            Parry_item = "2";
            Penetration_item = "15";
            AttackDelay_item = "95";
            //ItemDescript = "";
            TypeOfItem = "Axe";
			}
			if(item.DisplayName == "Myrmidon'sSpear"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Myrmidon's Spear";
                
            Weight = "2";
            STRENGTH_item = "15";
            AGILITY_item = "10";
            FORTITUDE_item = "10";
            ARCANA_item = "20";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "31";
            DamageMax_item = "55";
            Parry_item = "7";
            Penetration_item = "5";
            AttackDelay_item = "105";
            //ItemDescript = "";
            TypeOfItem = "Spear";
			}
			if(item.DisplayName == "Cleric'sMace"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Cleric's Mace";
                
            Weight = "4";
            STRENGTH_item = "5";
            AGILITY_item = "3";
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "43";
            DamageMax_item = "48";
            Parry_item = "5";
            Penetration_item = "8";
            AttackDelay_item = "110";
            //ItemDescript = "";
            TypeOfItem = "Mace";
			}
			if(item.DisplayName == "Assassin'sDagger"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Assassin's Dagger";
                
            Weight = "0.5";
            STRENGTH_item = "8";
            AGILITY_item = "8";
            FORTITUDE_item = "3";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "13";
            DamageMax_item = "43";
            Parry_item = "10";
            Penetration_item = "6";
            AttackDelay_item = "70";
            //ItemDescript = "";
            TypeOfItem = "Dagger";
			}
			if(item.DisplayName == "Arcanist'sStaff"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Arcanist's Staff";
                
            Weight = "3";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "3";
            ARCANA_item = "10";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "25";
            DamageMax_item = "41";
            Parry_item = "15";
            Penetration_item = "2";
            AttackDelay_item = "75";
            //ItemDescript = "";
            TypeOfItem = "Staff";
			}
			if(item.DisplayName == "Marksman'sBow"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Marksman's Bow";
                
            Weight = "1";
            STRENGTH_item = "3";
            AGILITY_item = "15";
            FORTITUDE_item = "3";
            ARCANA_item = "8";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "16";
            DamageMax_item = "60";
            Parry_item = "0";
            Penetration_item = "1";
            AttackDelay_item = "100";
            //ItemDescript = "";
            TypeOfItem = "Bow";
			}
			if(item.DisplayName == "Myrmidon'sGreathammer"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Myrmidon's Greathammer";
                
            Weight = "8";
            STRENGTH_item = "11";
            AGILITY_item = "4";
            FORTITUDE_item = "7";
            ARCANA_item = "7";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "91";
            DamageMax_item = "115";
            Parry_item = "5";
            Penetration_item = "14";
            AttackDelay_item = "190";
            //ItemDescript = "";
            TypeOfItem = "Greathammer";
			}
			if(item.DisplayName == "Myrmidon'sGreataxe"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Myrmidon's Greataxe";
                
            Weight = "7";
            STRENGTH_item = "15";
            AGILITY_item = "0";
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "55";
            DamageMax_item = "127";
            Parry_item = "2";
            Penetration_item = "29";
            AttackDelay_item = "175";
            //ItemDescript = "";
            TypeOfItem = "Greataxe";
			}
			if(item.DisplayName == "Myrmidon'sGreatsword"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Myrmidon's Greatsword";
                
            Weight = "6";
            STRENGTH_item = "6";
            AGILITY_item = "0";
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "19";
            DamageMax_item = "150";
            Parry_item = "10";
            Penetration_item = "9";
            AttackDelay_item = "170";
            //ItemDescript = "";
            TypeOfItem = "Greatsword";
			}
			if(item.DisplayName == "Myrmidon'sGreatspear"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Myrmidon's Greatspear";
                
            Weight = "3.5";
            STRENGTH_item = "20";
            AGILITY_item = "20";
            FORTITUDE_item = "15";
            ARCANA_item = "30";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "71";
            DamageMax_item = "122";
            Parry_item = "7";
            Penetration_item = "20";
            AttackDelay_item = "185";
            //ItemDescript = "";
            TypeOfItem = "Greatspear";
			}
			if(item.DisplayName == "BladeOfProtection"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Blade Of Protection";
                
            Weight = "2";
            STRENGTH_item = "5";
            AGILITY_item = "5";
            FORTITUDE_item = "20";
            ARCANA_item = "10";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "11";
            DamageMax_item = "85";
            Parry_item = "15";
            Penetration_item = "4";
            AttackDelay_item = "90";
            //ItemDescript = "";
            TypeOfItem = "Sword";
			}
			if(item.DisplayName == "CorrosiveAxe"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Corrosive Axe";
                
            Weight = "3";
            STRENGTH_item = "20";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "29";
            DamageMax_item = "70";
            Parry_item = "2";
            Penetration_item = "16";
            AttackDelay_item = "95";
            //ItemDescript = "";
            TypeOfItem = "Axe";
			}
			if(item.DisplayName == "DreadedSpear"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Dreaded Spear";
                
            Weight = "2";
            STRENGTH_item = "25";
            AGILITY_item = "20";
            FORTITUDE_item = "15";
            ARCANA_item = "30";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "36";
            DamageMax_item = "60";
            Parry_item = "7";
            Penetration_item = "6";
            AttackDelay_item = "105";
            //ItemDescript = "";
            TypeOfItem = "Spear";
			}
			if(item.DisplayName == "MaceOfCrushing"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Mace Of Crushing";
                
            Weight = "4";
            STRENGTH_item = "10";
            AGILITY_item = "5";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "48";
            DamageMax_item = "54";
            Parry_item = "5";
            Penetration_item = "8";
            AttackDelay_item = "110";
            //ItemDescript = "";
            TypeOfItem = "Mace";
			}
			if(item.DisplayName == "DaggerOfDeath"){
            type = ItemSelectable.ItemType.SingleHandedWeapon;
            Item_Slot = "One Handed";
            Itemname = "Dagger Of Death";
                
            Weight = "0.5";
            STRENGTH_item = "15";
            AGILITY_item = "15";
            FORTITUDE_item = "5";
            ARCANA_item = "10";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
			DamageMin_item = "22";
            DamageMax_item = "61";
            Parry_item = "10";
            Penetration_item = "6";
            AttackDelay_item = "70";
            //ItemDescript = "";
            TypeOfItem = "Dagger";
			}
			if(item.DisplayName == "PeridotStaff"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Peridot Staff";
                
            Weight = "3";
            STRENGTH_item = "5";
            AGILITY_item = "0";
            FORTITUDE_item = "5";
            ARCANA_item = "20";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "29";
            DamageMax_item = "46";
            Parry_item = "15";
            Penetration_item = "2";
            AttackDelay_item = "75";
            //ItemDescript = "";
            TypeOfItem = "Staff";
			}
			if(item.DisplayName == "BowOfDoom"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Bow Of Doom";
                
            Weight = "1";
            STRENGTH_item = "5";
            AGILITY_item = "25";
            FORTITUDE_item = "5";
            ARCANA_item = "10";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "19";
            DamageMax_item = "68";
            Parry_item = "0";
            Penetration_item = "2";
            AttackDelay_item = "100";
            //ItemDescript = "";
            TypeOfItem = "Bow";
			}
			if(item.DisplayName == "FrozenGreathammer"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Frozen Greathammer";
                
            Weight = "8";
            STRENGTH_item = "15";
            AGILITY_item = "5";
            FORTITUDE_item = "10";
            ARCANA_item = "10";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "99";
            DamageMax_item = "125";
            Parry_item = "5";
            Penetration_item = "15";
            AttackDelay_item = "190";
            //ItemDescript = "";
            TypeOfItem = "Greathammer";
			}
			if(item.DisplayName == "GreataxeOfFire"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Greataxe Of Fire";
                
            Weight = "7";
            STRENGTH_item = "20";
            AGILITY_item = "0";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "65";
            DamageMax_item = "137";
            Parry_item = "2";
            Penetration_item = "30";
            AttackDelay_item = "175";
            //ItemDescript = "";
            TypeOfItem = "Greataxe";
			}
			if(item.DisplayName == "FlambergeOfHatred"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Flamberge Of Hatred";
                
            Weight = "6";
            STRENGTH_item = "10";
            AGILITY_item = "0";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "25";
            DamageMax_item = "160";
            Parry_item = "10";
            Penetration_item = "10";
            AttackDelay_item = "170";
            //ItemDescript = "";
            TypeOfItem = "Greatsword";
			}
			if(item.DisplayName == "MythrilGreatspear"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Mythril Greatspear";
                
            Weight = "3.5";
            STRENGTH_item = "30";
            AGILITY_item = "25";
            FORTITUDE_item = "20";
            ARCANA_item = "45";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            DamageMin_item = "77";
            DamageMax_item = "132";
            Parry_item = "7";
            Penetration_item = "21";
            AttackDelay_item = "185";
            //ItemDescript = "";
            TypeOfItem = "Greatspear";
			}
            if(item.DisplayName == "Tactician'sTunic"){
            type = ItemSelectable.ItemType.TacticianChest;
            Item_Slot = "Chest";
            Itemname = "Tactician's Tunic";
                
            Weight = "12";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Chest";
			}
			if(item.DisplayName == "Tactician'sCap"){
            type = ItemSelectable.ItemType.TacticianHead;
            Item_Slot = "Head";
            Itemname = "Tactician's Cap";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Head";
			}
			if(item.DisplayName == "Tactician'sShoes"){
            type = ItemSelectable.ItemType.TacticianFeet;
            Item_Slot = "Feet";
            Itemname = "Tactician's Shoes";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "2";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "TacticianFeet";
			}
			if(item.DisplayName == "Tactician'sLeggings"){
            type = ItemSelectable.ItemType.TacticianLeggings;
            Item_Slot = "Leggings";
            Itemname = "Tactician's Leggings";
                
            Weight = "8";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Leggings";
			}
			if(item.DisplayName == "Tactician'sBracer"){
            type = ItemSelectable.ItemType.TacticianWrists;
            Item_Slot = "Wrists";
            Itemname = "Tactician's Bracer";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "1";
            FORTITUDE_item = "0";
            ARCANA_item = "1";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Wrists";
			}
			if(item.DisplayName == "Tactician'sShoulderpads"){
            type = ItemSelectable.ItemType.TacticianShoulders;
            Item_Slot = "Shoulders";
            Itemname = "Tactician's Shoulderpads";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Shoulders";
			}
			if(item.DisplayName == "Tactician'sSleeves"){
            type = ItemSelectable.ItemType.TacticianArms;
            Item_Slot = "Arms";
            Itemname = "Tactician's Sleeves";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Arms";
			}
			if(item.DisplayName == "Tactician'sBelt"){
            type = ItemSelectable.ItemType.TacticianWaist;
            Item_Slot = "Waist";
            Itemname = "Tactician's Belt";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "1";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Waist";
			}
			if(item.DisplayName == "Tactician'sGloves"){
            type = ItemSelectable.ItemType.TacticianHands;
            Item_Slot = "Hands";
            Itemname = "Tactician's Gloves";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "10% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Hands";
			}
			if(item.DisplayName == "Tactician'sImbuedTunic"){
            type = ItemSelectable.ItemType.TacticianChest;
            Item_Slot = "Chest";
            Itemname = "Tactician's Imbued Tunic";
                
            Weight = "12";
            STRENGTH_item = "3";
            AGILITY_item = "3";
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Chest";
			}
			if(item.DisplayName == "Tactician'sImbuedCap"){
            type = ItemSelectable.ItemType.TacticianHead;
            Item_Slot = "Head";
            Itemname = "Tactician's Imbued Cap";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Head";
			}
			if(item.DisplayName == "Tactician'sImbuedShoes"){
            type = ItemSelectable.ItemType.TacticianFeet;
            Item_Slot = "Feet";
            Itemname = "Tactician's Imbued Shoes";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "2";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "TacticianFeet";
			}
			if(item.DisplayName == "Tactician'sImbuedLeggings"){
            type = ItemSelectable.ItemType.TacticianLeggings;
            Item_Slot = "Leggings";
            Itemname = "Tactician's Imbued Leggings";
                
            Weight = "8";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "2";
            ARCANA_item = "2";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Leggings";
			}
			if(item.DisplayName == "Tactician'sImbuedBracer"){
            type = ItemSelectable.ItemType.TacticianWrists;
            Item_Slot = "Wrists";
            Itemname = "Tactician's Imbued Bracer";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "2";
            FORTITUDE_item = "1";
            ARCANA_item = "2";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Wrists";
			}
			if(item.DisplayName == "Tactician'sImbuedShoulderpads"){
            type = ItemSelectable.ItemType.TacticianShoulders;
            Item_Slot = "Shoulders";
            Itemname = "Tactician's Imbued Shoulderpads";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "2";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Shoulders";
			}
			if(item.DisplayName == "Tactician'sImbuedSleeves"){
            type = ItemSelectable.ItemType.TacticianArms;
            Item_Slot = "Arms";
            Itemname = "Tactician's Imbued Sleeves";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Arms";
			}
			if(item.DisplayName == "Tactician'sImbuedBelt"){
            type = ItemSelectable.ItemType.TacticianWaist;
            Item_Slot = "Waist";
            Itemname = "Tactician's Imbued Belt";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Waist";
			}
			if(item.DisplayName == "Tactician'sImbuedGloves"){
            type = ItemSelectable.ItemType.TacticianHands;
            Item_Slot = "Hands";
            Itemname = "Tactician's Imbued Gloves";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "20% Bonus To Crafting Exp";
            TypeOfItem = "Tactician Hands";
			}
				
            if(item.DisplayName == "Squire'sShield"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Squire's Shield";
                
            Weight = "5";
            STRENGTH_item = "1";
            AGILITY_item = "5"; 
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "3";
            BlockChance_item = "25";
            BlockValue_item = "15";
            //ItemDescript = "Heavy But Durable";
            TypeOfItem = "Shield";
			}
			if(item.DisplayName == "Knight'sShield"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Knight's Shield";
                
            Weight = "8";
            STRENGTH_item = "2";
            AGILITY_item = "5"; 
            FORTITUDE_item = "8";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "4";
            BlockChance_item = "28";
            BlockValue_item = "18";
            //ItemDescript = "Anyone Would Be Proud To Wear This";
            TypeOfItem = "Shield";
			}
			if(item.DisplayName == "Guardian'sShield"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Guardian's Shield";
                
            Weight = "9";
            STRENGTH_item = "3";
            AGILITY_item = "5"; 
            FORTITUDE_item = "12";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "7";
            BlockChance_item = "30";
            BlockValue_item = "22";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Shield";
			}
			if(item.DisplayName == "DragonShield"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Dragon Shield";
                
            Weight = "5";
            STRENGTH_item = "3";
            AGILITY_item = "5"; 
            FORTITUDE_item = "12";
            ARCANA_item = "10";
            Rarity_item = "Exotic";
            MagicResist_item = "6";
            FireResist_item = "6";
            ColdResist_item = "6";
            PoisonResist_item = "6";
            DiseaseResist_item = "6";
            Armor = "6";
            BlockChance_item = "35";
            BlockValue_item = "25";
            //ItemDescript = "A Shield Encased With Dragon Scales";
            TypeOfItem = "Shield";
			}
			if(item.DisplayName == "ShieldOfProtection"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Shield Of Protection";
                
            Weight = "5";
            STRENGTH_item = "5";
            AGILITY_item = "5"; 
            FORTITUDE_item = "15";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "6";
            FireResist_item = "6";
            ColdResist_item = "6";
            PoisonResist_item = "6";
            DiseaseResist_item = "6";
            Armor = "7";
            BlockChance_item = "50";
            BlockValue_item = "15";
            //ItemDescript = "A Shield Emitting A powerful Glow";
            TypeOfItem = "Shield";
			}
			if(item.DisplayName == "ShellShield"){
            type = ItemSelectable.ItemType.Shield;
            Item_Slot = "Shield";
            Itemname = "Shell Shield";
                
            Weight = "3";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "3";
            BlockChance_item = "25";
            BlockValue_item = "11";
            //ItemDescript = "This Shell Seems More Sturdy Than Most";
            TypeOfItem = "Shield";
			}
			if(item.DisplayName == "Apprentice'sTome"){
            type = ItemSelectable.ItemType.OffHand;
            Item_Slot = "OffHand";
            Itemname = "Apprentice's Tome";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "10";
            Rarity_item = "Uncommon";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "0";
            //ItemDescript = "A Bit Weathered But You Can Easily Make Out The Text";
            TypeOfItem = "OffHand";
			}
			if(item.DisplayName == "Scholar'sTome"){
            type = ItemSelectable.ItemType.OffHand;
            Item_Slot = "OffHand";
            Itemname = "Scholar's Tome";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "15";
            Rarity_item = "Rare";
            MagicResist_item = "7";
            FireResist_item = "7";
            ColdResist_item = "7";
            PoisonResist_item = "7";
            DiseaseResist_item = "7";
            Armor = "0";
            //ItemDescript = "Such a Pristine And Well Designed Book";
            TypeOfItem = "OffHand";
			}
			if(item.DisplayName == "Arcanist'sTome"){
            type = ItemSelectable.ItemType.OffHand;
            Item_Slot = "OffHand";
            Itemname = "Arcanist's Tome";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "1";
            ARCANA_item = "20";
            Rarity_item = "UltraRare";
            MagicResist_item = "9";
            FireResist_item = "9";
            ColdResist_item = "9";
            PoisonResist_item = "9";
            DiseaseResist_item = "9";
            Armor = "1";
            //ItemDescript = "Any Young Wizard Would Be Proud To Show This Book Off";
            TypeOfItem = "OffHand";
			}
			if(item.DisplayName == "Wizard'sTome"){
            type = ItemSelectable.ItemType.OffHand;
            Item_Slot = "OffHand";
            Itemname = "Wizard's Tome";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "5";
            ARCANA_item = "20";
            Rarity_item = "Exotic";
            MagicResist_item = "15";
            FireResist_item = "15";
            ColdResist_item = "15";
            PoisonResist_item = "15";
            DiseaseResist_item = "15";
            Armor = "1";
            //ItemDescript = "Any Wizard Would Be Proud To Show This Book Off";
            TypeOfItem = "OffHand";
			}
			if(item.DisplayName == "TomeOfKnowledge"){
            type = ItemSelectable.ItemType.OffHand;
            Item_Slot = "OffHand";
            Itemname = "Tome Of Knowledge";
                
            Weight = "2";
            STRENGTH_item = "5";
            AGILITY_item = "5"; 
            FORTITUDE_item = "5";
            ARCANA_item = "40";
            Rarity_item = "Exotic";
            MagicResist_item = "10";
            FireResist_item = "10";
            ColdResist_item = "10";
            PoisonResist_item = "10";
            DiseaseResist_item = "10";
            Armor = "0";
            //ItemDescript = "You Feel Your Mind Expand When You Put This On";
            TypeOfItem = "OffHand";
			}
			if(item.DisplayName == "Quiver"){
            type = ItemSelectable.ItemType.Quiver;
            Item_Slot = "Quiver";
            Itemname = "Quiver";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "5"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Used To Store Arrows";
            TypeOfItem = "Quiver";
			}
            if(item.DisplayName == "Squire'sBreastplate"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Squire's Breastplate";
                
            Weight = "18";
            STRENGTH_item = "2";
            AGILITY_item = "2";
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "1";
            Armor = "7";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Chest";
			}
	        if(item.DisplayName == "Squire'sHelmet"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Squire's Helmet";
                
            Weight = "6";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "1";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Head";
			}
			if(item.DisplayName == "Squire'sBoots"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Squire's Boots";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "1";
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Feet";
			}
			if(item.DisplayName == "Squire'sLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Squire's Leggings";
                
            Weight = "12";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "1";
            Armor = "5";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Leggings";
			}
			if(item.DisplayName == "Squire'sBracer"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Squire's Bracer";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "1";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Wrists";
			}
			if(item.DisplayName == "Squire'sSpaulders"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Squire's Spaulders";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "2";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "1";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Shoulders";
			}
			if(item.DisplayName == "Squire'sVambraces"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Squire's Vambraces";
                
            Weight = "6";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Arms";
			}
			if(item.DisplayName == "Squire'sBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Squire's Belt";
                
            Weight = "3";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "1";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Waist";
			}
			if(item.DisplayName == "Squire'sGauntlets"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Squire's Gauntlets";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "1";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "3";
            //ItemDescript = "Heavy but durable";
            TypeOfItem = "Plate Hands";
			}		
			if(item.DisplayName == "Knight'sBreastplate"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Knight's Breastplate";
                
            Weight = "18";
            STRENGTH_item = "3";
            AGILITY_item = "3";
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "8";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Chest";
			}
			if(item.DisplayName == "Knight'sHelmet"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Knight's Helmet";
                
            Weight = "6";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Head";
			}
			if(item.DisplayName == "Knight'sBoots"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Knight's Boots";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "2";
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Feet";
			}
			if(item.DisplayName == "Knight'sLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Knight's Leggings";
                
            Weight = "12";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "6";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Leggings";
			}
			if(item.DisplayName == "Knight'sBracer"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Knight's Bracer";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Wrists";
			}
			if(item.DisplayName == "Knight'sSpaulders"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Knight's Spaulders";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "2";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Shoulders";
			}
			if(item.DisplayName == "Knight'sVambraces"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Knight's Vambraces";
                
            Weight = "6";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Arms";
			}
			if(item.DisplayName == "Knight'sBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Knight's Belt";
                
            Weight = "3";
            STRENGTH_item = "2";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Waist";
			}
			if(item.DisplayName == "Knight'sGauntlets"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Knight's Gauntlets";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Plate Hands";
			}
			if(item.DisplayName == "Guardian'sBreastplate"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Guardian's Breastplate";
                
            Weight = "18";
            STRENGTH_item = "5";
            AGILITY_item = "5";
            FORTITUDE_item = "10";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "8";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Chest";
			}
			if(item.DisplayName == "Guardian'sHelmet"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Guardian's Helmet";
                
            Weight = "6";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Head";
			}
			if(item.DisplayName == "Guardian'sBoots"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Guardian's Boots";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "2";
            FORTITUDE_item = "3";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Feet";
			}
			if(item.DisplayName == "Guardian'sLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Guardian's Leggings";
                
            Weight = "12";
            STRENGTH_item = "5";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "6";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Leggings";
			}
			if(item.DisplayName == "Guardian'sBracer"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Guardian's Bracer";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Wrists";
			}
			if(item.DisplayName == "Guardian'sSpaulders"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Guardian's Spaulders";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "5";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Shoulders";
			}
			if(item.DisplayName == "Guardian'sVambraces"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Guardian's Vambraces";
                
            Weight = "6";
            STRENGTH_item = "2";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Arms";
			}
			if(item.DisplayName == "Guardian'sBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Guardian's Belt";
                
            Weight = "3";
            STRENGTH_item = "2";
            AGILITY_item = "0";
            FORTITUDE_item = "3";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Waist";
			}
			if(item.DisplayName == "Guardian'sGauntlets"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Guardian's Gauntlets";
                
            Weight = "6";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Plate Hands";
			}
			if(item.DisplayName == "BreastplateOfAnguish"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Breastplate Of Anguish";
                
            Weight = "18";
            STRENGTH_item = "15";
            AGILITY_item = "10";
            FORTITUDE_item = "20";
            ARCANA_item = "10";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "9";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Chest";
			}
			if(item.DisplayName == "MythrilHelmet"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Mythril Helmet";
                
            Weight = "6";
            STRENGTH_item = "5";
            AGILITY_item = "2";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Head";
			}
			if(item.DisplayName == "BootsOfPower"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Boots Of Power";
                
            Weight = "6";
            STRENGTH_item = "10";
            AGILITY_item = "5";
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Feet";
			}
			if(item.DisplayName == "BloodInfusedLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Blood Infused Leggings";
                
            Weight = "12";
            STRENGTH_item = "10";
            AGILITY_item = "5";
            FORTITUDE_item = "5";
            ARCANA_item = "10";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "7";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Leggings";
			}
			if(item.DisplayName == "BracersOfKnowledge"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Bracers Of Knowledge";
                
            Weight = "6";
            STRENGTH_item = "2";
            AGILITY_item = "5";
            FORTITUDE_item = "5";
            ARCANA_item = "2";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Wrists";
			}
			if(item.DisplayName == "BloodstoneSpaulders"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Bloodstone Spaulders";
                
            Weight = "6";
            STRENGTH_item = "5";
            AGILITY_item = "5";
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Shoulders";
			}
			if(item.DisplayName == "VambracesOfWar"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Vambraces Of War";
                
            Weight = "6";
            STRENGTH_item = "2";
            AGILITY_item = "4";
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Arms";
			}
			if(item.DisplayName == "Abishi'sBeltOfPower"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Abishi's Belt Of Power";
                
            Weight = "3";
            STRENGTH_item = "3";
            AGILITY_item = "3";
            FORTITUDE_item = "3";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Waist";
			}
			if(item.DisplayName == "GauntletsOfMight"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Gauntlets Of Might";
                
            Weight = "6";
            STRENGTH_item = "10";
            AGILITY_item = "3";
            FORTITUDE_item = "5";
            ARCANA_item = "3";
            Rarity_item = "Exotic";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Plate Hands";
			}
			if(item.DisplayName == "OozeCoveredHelmet"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Ooze Covered Helmet";
                
            Weight = "6";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "2";
            //ItemDescript = "A gross slimy helmet";
            TypeOfItem = "Plate Head";
			}
            if(item.DisplayName == "SilkyBoots"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Silky Boots";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "These boots are extra soft";
            TypeOfItem = "Cloth Feet";
			}
            if(item.DisplayName == "Scout'sTunic"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Scout's Tunic";
                
            Weight = "12";
            STRENGTH_item = "2";
            AGILITY_item = "2";
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "4";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Chest";
			}
			if(item.DisplayName == "Scout'sCap"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Scout's Cap";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "5";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "2";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Head";
			}
			if(item.DisplayName == "Scout'sShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Scout's Shoes";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "3";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Feet";
			}
			if(item.DisplayName == "Scout'sLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Scout's Leggings";
                
            Weight = "8";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "3";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Leggings";
			}
			if(item.DisplayName == "Scout'sBracers"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Scout's Bracers";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "3";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Wrists";
			}
			if(item.DisplayName == "Scout'sShoulderpads"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Scout's Shoulderpads";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "2";
            FORTITUDE_item = "1";
            ARCANA_item = "2";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Shoulders";
			}
			if(item.DisplayName == "Scout'sSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Scout's Sleeves";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "2";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Arms";
			}
			if(item.DisplayName == "Scout'sBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Scout's Belt";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "2";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Waist";
			}
			if(item.DisplayName == "Scout'sGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Scout's Gloves";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "4";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Leather Hands";
			}
			if(item.DisplayName == "Hunter'sTunic"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Hunter's Tunic";
                
            Weight = "12";
            STRENGTH_item = "3";
            AGILITY_item = "3";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "5";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Chest";
			}
			if(item.DisplayName == "Hunter'sCap"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Hunter's Cap";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "5";
            FORTITUDE_item = "3";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Head";
			}
			if(item.DisplayName == "Hunter'sShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Hunter's Shoes";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "3";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Feet";
			}
			if(item.DisplayName == "Hunter'sLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Hunter's Leggings";
                
            Weight = "8";
            STRENGTH_item = "3";
            AGILITY_item = "5";
            FORTITUDE_item = "2";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "3";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Leggings";
			}
			if(item.DisplayName == "Hunter'sBracer"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Hunter's Bracer";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "3";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Wrists";
			}
			if(item.DisplayName == "Hunter'sShoulderpads"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Hunter's Shoulderpads";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "2";
            FORTITUDE_item = "1";
            ARCANA_item = "2";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Shoulders";
			}
			if(item.DisplayName == "Hunter'sSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Hunter's Sleeves";
                
            Weight = "4";
            STRENGTH_item = "3";
            AGILITY_item = "3";
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Arms";
			}
			if(item.DisplayName == "Hunter'sBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Hunter's Belt";
                
            Weight = "2";
            STRENGTH_item = "3";
            AGILITY_item = "3";
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Waist";
			}
			if(item.DisplayName == "Hunter'sGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Hunter's Gloves";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "4";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Leather Hands";
			}
			if(item.DisplayName == "Assassin'sTunic"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Assassin's Tunic";
                
            Weight = "12";
            STRENGTH_item = "3";
            AGILITY_item = "7";
            FORTITUDE_item = "5";
            ARCANA_item = "7";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "5";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Chest";
			}
			if(item.DisplayName == "Assassin'sCap"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Assassin's Cap";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "5";
            FORTITUDE_item = "3";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Head";
			}
			if(item.DisplayName == "Assassin'sShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Assassin's Shoes";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "5";
            FORTITUDE_item = "2";
            ARCANA_item = "4";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "2";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Feet";
			}
			if(item.DisplayName == "Assassin'sLeggings"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Assassin's Leggings";
                
            Weight = "8";
            STRENGTH_item = "3";
            AGILITY_item = "5";
            FORTITUDE_item = "2";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "4";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Leggings";
			}
			if(item.DisplayName == "Assassin'sBracer"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Assassin's Bracer";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "4";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Wrists";
			}
			if(item.DisplayName == "Assassin'sPads"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Assassin's Shoulderpads";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "3";
            FORTITUDE_item = "1";
            ARCANA_item = "4";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Shoulders";
			}
			if(item.DisplayName == "Assassin'sSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Assassin's Sleeves";
                
            Weight = "4";
            STRENGTH_item = "3";
            AGILITY_item = "4";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Arms";
			}
			if(item.DisplayName == "Assassin'sBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Assassin's Belt";
                
            Weight = "2";
            STRENGTH_item = "3";
            AGILITY_item = "5";
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Waist";
			}
			if(item.DisplayName == "Assassin'sGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Assassin's Gloves";
                
            Weight = "4";
            STRENGTH_item = "1";
            AGILITY_item = "4";
            FORTITUDE_item = "2";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Leather Hands";
			}
			if(item.DisplayName == "Blood-SoakedTunic"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Blood-Soaked Tunic";
                
            Weight = "12";
            STRENGTH_item = "10";
            AGILITY_item = "20";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "6";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Chest";
			}
			if(item.DisplayName == "HoodOfSlaughter"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Hood Of Slaughter";
                
            Weight = "4";
            STRENGTH_item = "3";
            AGILITY_item = "8";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "4";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Head";
			}
			if(item.DisplayName == "ShoesOfPower"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Shoes Of Power";
                
            Weight = "4";
            STRENGTH_item = "10";
            AGILITY_item = "5";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "3";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Feet";
			}
			if(item.DisplayName == "LeggingsOfVigor"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Leggings Of Vigor";
                
            Weight = "8";
            STRENGTH_item = "5";
            AGILITY_item = "5";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "5";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Leggings";
			}
			if(item.DisplayName == "BracerOfKnowledge"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Bracer Of Knowledge";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "5";
            FORTITUDE_item = "5";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "4";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Wrists";
			}
			if(item.DisplayName == "SpauldersOfVelocity"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Spaulders Of Velocity";
                
            Weight = "4";
            STRENGTH_item = "5";
            AGILITY_item = "8";
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "3";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Shoulders";
			}
			if(item.DisplayName == "SleevesOfSpeed"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Sleeves Of Speed";
                
            Weight = "4";
            STRENGTH_item = "3";
            AGILITY_item = "12";
            FORTITUDE_item = "5";
            ARCANA_item = "6";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "3";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Arms";
			}
			if(item.DisplayName == "CordOfInsight"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Cord Of Insight";
                
            Weight = "2";
            STRENGTH_item = "3";
            AGILITY_item = "5";
            FORTITUDE_item = "3";
            ARCANA_item = "20";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "3";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Waist";
			}
			if(item.DisplayName == "GauntletsOfFortitude"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Gauntlets Of Fortitude";
                
            Weight = "4";
            STRENGTH_item = "2";
            AGILITY_item = "3";
            FORTITUDE_item = "12";
            ARCANA_item = "2";
            Rarity_item = "Exotic";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "4";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Leather Hands";
			}
			if(item.DisplayName == "SlimyBelt"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Slimy Belt";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "0";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "This disgusting belt is covered in slime";
            TypeOfItem = "Leather Waist";
			}
            if(item.DisplayName == "Ring"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Basic Ring";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "Earring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Basic Earring";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "Amulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Basic Amulet";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "LapisLazuliRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Lapis Lazuli Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Lapis Lazuli";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "LapisLazuliEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Lapis Lazuli Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Lapis Lazuli";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "LapisLazuliAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Lapis Lazuli Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Lapis Lazuli";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "AmberRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Amber Ring";
                
            Weight = "0.1";
            STRENGTH_item = "2";
            AGILITY_item = "2"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Amber";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "AmberEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Amber Earring";
                
            Weight = "0.1";
            STRENGTH_item = "2";
            AGILITY_item = "2"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Amber";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "AmberAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Amber Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "2";
            AGILITY_item = "2"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Amber";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "RubyRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Ruby Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "A Ring Infused With Ruby";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "RubyEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Ruby Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "An Earring Infused With Ruby";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "RubyAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Ruby Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "2";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "An Amulet Infused With Ruby";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "SapphireRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Sapphire Ring";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Sapphire";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "SapphireEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Sapphire Earring";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Sapphire";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "SapphireAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Sapphire Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Sapphire";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "RoseQuartzRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Rose Quartz Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Rose Quartz";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "RoseQuartzEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Rose Quartz Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Rose Quartz";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "RoseQuartzAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Rose Quartz Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Rose Quartz";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "OpalRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Opal Ring";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "0";
            ARCANA_item = "1";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Opal";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "OpalEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Opal Earring";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "0";
            ARCANA_item = "1";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Opal";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "OpalAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Opal Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "0";
            ARCANA_item = "1";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Opal";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "TopazRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Topaz Ring";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Topaz";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "TopazEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Topaz Earring";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Topaz";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "TopazAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Topaz Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Rare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Topaz";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "MoonstoneRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Moonstone Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Moonstone";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "MoonstoneEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Moonstone Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Moonstone";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "MoonstoneAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Moonstone Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Moonstone";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "JadeRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Jade Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "2";
            ARCANA_item = "10";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Jade";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "JadeEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Jade Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "2";
            ARCANA_item = "10";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Jade";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "JadeAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Jade Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "2";
            ARCANA_item = "10";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Jade";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "CarnelianRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Carnelian Ring";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Carnelian";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "CarnelianEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Carnelian Earring";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Carnelian";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "CarnelianAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Carnelian Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "UltraRare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Carnelian";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "AgateRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Agate Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "0";
            //ItemDescript = "A Ring Infused With Agate";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "AgateEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Agate Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "0";
            //ItemDescript = "An Earring Infused With Agate";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "AgateAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Agate Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "1";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "0";
            //ItemDescript = "An Amulet Infused With Agate";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "OnyxRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Onyx Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "A Ring Infused With Onyx";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "OnyxEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Onyx Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "An Earring Infused With Onyx";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "OnyxAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Onyx Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "5";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "1";
            //ItemDescript = "An Amulet Infused With Onyx";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "TitaniteRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Titanite Ring";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "A Ring Infused With Titanite";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "TitaniteEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Titanite Earring";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "An Earring Infused With Titanite";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "TitaniteAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Titanite Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "3";
            AGILITY_item = "3"; 
            FORTITUDE_item = "3";
            ARCANA_item = "3";
            Rarity_item = "Exotic";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "1";
            //ItemDescript = "An Amulet Infused With Titanite";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "MythrilRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Mythril Ring";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "A Ring Infused With Mythril";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "MythrilEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Mythril Earring";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "An Earring Infused With Mythril";
            TypeOfItem = "Earring";
			}
			if(item.DisplayName == "MythrilAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Mythril Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "1"; 
            FORTITUDE_item = "1";
            ARCANA_item = "1";
            Rarity_item = "Exotic";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "An Amulet Infused With Mythril";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "BatFangAmulet"){
            type = ItemSelectable.ItemType.Necklace;
            Item_Slot = "Necklace";
            Itemname = "Bat Fang Amulet";
                
            Weight = "0.1";
            STRENGTH_item = "1";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Strange Amulet Smells Like A Dead Bat";
            TypeOfItem = "Necklace";
			}
			if(item.DisplayName == "RatShapedRing"){
            type = ItemSelectable.ItemType.Ring;
            Item_Slot = "Ring";
            Itemname = "Rat Shaped Ring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "1"; 
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Strange Ring In The Shape Of A Tiny Rat";
            TypeOfItem = "Ring";
			}
			if(item.DisplayName == "DirtyEarring"){
            type = ItemSelectable.ItemType.Earring;
            Item_Slot = "Earring";
            Itemname = "Dirty Earring";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0"; 
            FORTITUDE_item = "0";
            ARCANA_item = "2";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "This Earring Smells Terrible";
            TypeOfItem = "Earring";
			}
            if(item.DisplayName == "RatMeat"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Rat Meat";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Cooking Component";
            TypeOfItem = "ResourceT1";
			}
			if(item.DisplayName == "BatWing"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Bat Wing";
                
            Weight = "0.5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
			}
            if(item.DisplayName == "SpiderSilk"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Spider Silk";
                
            Weight = "0.5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
            }
				
			if(item.DisplayName == "SnakeVenom"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Snake Venom";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
			}
			if(item.DisplayName == "WhelpScale"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Whelp Scale";
                
            Weight = "0.5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
			}
			if(item.DisplayName == "LizardScale"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Lizard Scale";
                
            Weight = "0.5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
			}
			if(item.DisplayName == "JellyResidue"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Jelly Residue";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
			}
			if(item.DisplayName == "LobsterMeat"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Lobster Meat";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Cooking Component";
            TypeOfItem = "ResourceT1";
			}
            if(item.DisplayName == "VialOfArcana"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Vial Of Arcana";
                
            Weight = "0.5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
			}
            if(item.DisplayName == "Blood"){
            type = ItemSelectable.ItemType.ResourceT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Blood";
                
            Weight = "0.5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "ResourceT1";
			}
			if(item.DisplayName == "Mushroom"){
            type = ItemSelectable.ItemType.GrownT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Mushroom";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "GrownT1";
			}
			if(item.DisplayName == "Cabbage"){
            type = ItemSelectable.ItemType.GrownT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Cabbage";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Cooking Component";
            TypeOfItem = "GrownT1";
			}
			if(item.DisplayName == "MushroomSoup"){
            type = ItemSelectable.ItemType.FoodT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Mushroom Soup";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Adds 10 Arcana 5 Fort For 30 Minutes";
            TypeOfItem = "FoodT1";
			}
			if(item.DisplayName == "SnakeStew"){
            type = ItemSelectable.ItemType.FoodT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Snake Stew";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Adds 5 Agility 20 Poison Resist For 30 Minutes";
            TypeOfItem = "FoodT1";
			}
			if(item.DisplayName == "RatSkewer"){
            type = ItemSelectable.ItemType.FoodT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Rat Skewer";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Adds 10 Strength For 30 Minutes";
            TypeOfItem = "FoodT1";
			}
			if(item.DisplayName == "Sauerkraut"){
            type = ItemSelectable.ItemType.FoodT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Sauerkraut";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Adds 5 Fortitude 20 Disease Resist For 30 Minutes";
            TypeOfItem = "FoodT1";
			}
            if(item.DisplayName == "LobsterJerky"){
            type = ItemSelectable.ItemType.FoodT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Lobster Jerky";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Adds 7 Agility and 7 Strength for 30 minutes";
            TypeOfItem = "FoodT1";
			}
			if(item.DisplayName == "HealingPotion"){
            type = ItemSelectable.ItemType.PotionT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Healing Potion";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Heals 25-30 Hit Points";
            TypeOfItem = "PotionT1";
			}
			if(item.DisplayName == "MagicPotion"){
            type = ItemSelectable.ItemType.PotionT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Magic Potion";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Heals 2-4 MP";
            TypeOfItem = "PotionT1";
			}
			if(item.DisplayName == "HastePotion"){
            type = ItemSelectable.ItemType.PotionT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Haste Potion";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Increases Agility By 10 For 30 Minutes";
            TypeOfItem = "PotionT1";
			}
			if(item.DisplayName == "DefensePotion"){
            type = ItemSelectable.ItemType.PotionT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Defense Potion";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Increases Armor By 2 For 30 Minutes";
            TypeOfItem = "PotionT1";
			}
			if(item.DisplayName == "Antidote"){
            type = ItemSelectable.ItemType.PotionT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Antidote";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Cure's Poison";
            TypeOfItem = "PotionT1";
			}
            if(item.DisplayName == "EnergyPotion"){
            type = ItemSelectable.ItemType.PotionT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Energy Potion";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Restores 200 energy";
            TypeOfItem = "PotionT2";
			}
            if(item.DisplayName == "BloodPotion"){
            type = ItemSelectable.ItemType.PotionT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Blood Potion";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Attacks leech an additional 1 damage per hit for 5 minutes";
            TypeOfItem = "PotionT2";
			}
			if(item.DisplayName == "Squire'sWeaponPattern"){
            type = ItemSelectable.ItemType.PatternT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Squire's Weapon Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT2";
			}
			if(item.DisplayName == "Squire'sPlatePattern"){
            type = ItemSelectable.ItemType.PatternT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Squire's Plate Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT2";
			}
			if(item.DisplayName == "Apprentice'sClothPattern"){
            type = ItemSelectable.ItemType.PatternT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Apprentice's Cloth Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT2";
			}
			if(item.DisplayName == "Scout'sLeatherPattern"){
            type = ItemSelectable.ItemType.PatternT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Scout's Leather Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT2";
			}
			if(item.DisplayName == "Squire'sShieldPattern"){
            type = ItemSelectable.ItemType.PatternT2;
            Item_Slot = "Cannot Equip";
            Itemname = "Squire's Shield Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Uncommon";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT2";
			}
			if(item.DisplayName == "Knight'sWeaponPattern"){
            type = ItemSelectable.ItemType.PatternT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Knight's Weapon Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT3";
			}
			if(item.DisplayName == "Knight'sPlatePattern"){
            type = ItemSelectable.ItemType.PatternT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Knight's Plate Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT3";
			}
			if(item.DisplayName == "Scholar'sClothPattern"){
            type = ItemSelectable.ItemType.PatternT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Scholar's Cloth Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT3";
			}
			if(item.DisplayName == "Hunter'sLeatherPattern"){
            type = ItemSelectable.ItemType.PatternT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Hunter's Leather Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT3";
			}
			if(item.DisplayName == "Knight'sShieldPattern"){
            type = ItemSelectable.ItemType.PatternT3;
            Item_Slot = "Cannot Equip";
            Itemname = "Knight's Shield Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Rare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT3";
			}
			if(item.DisplayName == "Myrmidon'sWeaponPattern"){
            type = ItemSelectable.ItemType.PatternT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Myrmidon's Weapon Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT4";
			}
			if(item.DisplayName == "Guardian'sPlatePattern"){
            type = ItemSelectable.ItemType.PatternT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Guardian's Plate Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT4";
			}
			if(item.DisplayName == "Arcanist'sClothPattern"){
            type = ItemSelectable.ItemType.PatternT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Arcanist's Cloth Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT4";
			}
			if(item.DisplayName == "Assassin'sLeatherPattern"){
            type = ItemSelectable.ItemType.PatternT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Assassin's Leather Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT4";
			}
			if(item.DisplayName == "Guardian'sShieldPattern"){
            type = ItemSelectable.ItemType.PatternT4;
            Item_Slot = "Cannot Equip";
            Itemname = "Guardian's Shield Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "UltraRare";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT4";
			}
			if(item.DisplayName == "GlowingWeaponPattern"){
            type = ItemSelectable.ItemType.PatternT5;
            Item_Slot = "Cannot Equip";
            Itemname = "Glowing Weapon Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT5";
			}
			if(item.DisplayName == "GlowingPlatePattern"){
            type = ItemSelectable.ItemType.PatternT5;
            Item_Slot = "Cannot Equip";
            Itemname = "Glowing Plate Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT5";
			}
			if(item.DisplayName == "GlowingClothPattern"){
            type = ItemSelectable.ItemType.PatternT5;
            Item_Slot = "Cannot Equip";
            Itemname = "Glowing Cloth Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT5";
			}
			if(item.DisplayName == "GlowingLeatherPattern"){
            type = ItemSelectable.ItemType.PatternT5;
            Item_Slot = "Cannot Equip";
            Itemname = "Glowing Leather Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT5";
			}
			if(item.DisplayName == "GlowingShieldPattern"){
            type = ItemSelectable.ItemType.PatternT5;
            Item_Slot = "Cannot Equip";
            Itemname = "Glowing Shield Pattern";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Exotic";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "An Ancient Design";
            TypeOfItem = "PatternT5";
			}
			if(item.DisplayName == "Pickaxe"){
            type = ItemSelectable.ItemType.Tool;
            Item_Slot = "Cannot Equip";
            Itemname = "Pickaxe";
                
            Weight = "5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tool For Harvesting Ore";
            TypeOfItem = "Tool";
			}
			if(item.DisplayName == "SkinningKnife"){
            type = ItemSelectable.ItemType.Tool;
            Item_Slot = "Cannot Equip";
            Itemname = "Skinning Knife";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tool For Harvesting Leather";
            TypeOfItem = "Tool";
			}
			if(item.DisplayName == "Scythe"){
            type = ItemSelectable.ItemType.Tool;
            Item_Slot = "Cannot Equip";
            Itemname = "Scythe";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tool For Harvesting Cloth";
            TypeOfItem = "Tool";
			}
			if(item.DisplayName == "LumberjackAxe"){
            type = ItemSelectable.ItemType.Tool;
            Item_Slot = "Cannot Equip";
            Itemname = "Lumberjack Axe";
                
            Weight = "5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tool For Harvesting Wood";
            TypeOfItem = "Tool";
			}
			if(item.DisplayName == "StoneHammer"){
            type = ItemSelectable.ItemType.Tool;
            Item_Slot = "Cannot Equip";
            Itemname = "StoneHammer";
                
            Weight = "5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Tool For Harvesting Stone";
            TypeOfItem = "Tool";
			}
			if(item.DisplayName == "Hide"){
            type = ItemSelectable.ItemType.MaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Hide";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Unrefined Resource";
            TypeOfItem = "MaterialT1";
			}
			if(item.DisplayName == "Leather"){
            type = ItemSelectable.ItemType.RefinedMaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Leather";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Refined Resource";
            TypeOfItem = "RefinedMaterialT1";
			}
			if(item.DisplayName == "Fiber"){
            type = ItemSelectable.ItemType.MaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Fiber";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Unrefined Resource";
            TypeOfItem = "MaterialT1";
			}
			if(item.DisplayName == "Cloth"){
            type = ItemSelectable.ItemType.RefinedMaterialT1;
            Item_Slot = "Cannot Equip";
            Itemname = "Cloth";
                
            Weight = "1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Refined Resource";
            TypeOfItem = "RefinedMaterialT1";
			}
			if(item.DisplayName == "BoneChip"){
            type = ItemSelectable.ItemType.Misc;
            Item_Slot = "Cannot Equip";
            Itemname = "Bone Chip";
                
            Weight = "0.1";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Alchemy Component";
            TypeOfItem = "Misc";
			}
            if(item.DisplayName == "Apprentice'sRobe"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Apprentice's Robe";
                
            Weight = "6";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "3";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Chest";
			}
			if(item.DisplayName == "Apprentice'sHood"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Apprentice's Hood";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Head";
			}
			if(item.DisplayName == "Apprentice'sShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Apprentice's Shoes";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Feet";
			}
			if(item.DisplayName == "Apprentice'sPants"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Apprentice's Pants";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "6";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Leggings";
			}
			if(item.DisplayName == "Apprentice'sCuff's"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Apprentice's Cuff's";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "0";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Wrists";
			}
			if(item.DisplayName == "Apprentice'sMantle"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Apprentice's Mantle";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Shoulders";
			}
			if(item.DisplayName == "Apprentice'sSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Apprentice's Sleeves";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Arms";
			}
			if(item.DisplayName == "Apprentice'sCord"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Apprentice's Cord";
                
            Weight = "1";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Waist";
			}
			if(item.DisplayName == "Apprentice'sGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Apprentice's Gloves";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "4";
            Rarity_item = "Uncommon";
            MagicResist_item = "2";
            FireResist_item = "2";
            ColdResist_item = "2";
            PoisonResist_item = "2";
            DiseaseResist_item = "2";
            Armor = "1";
            //ItemDescript = "Light yet durable";
            TypeOfItem = "Cloth Hands";
			}
			if(item.DisplayName == "Scholar'sRobe"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Scholar's Robe";
                
            Weight = "6";
            STRENGTH_item = "2";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "8";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "3";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Chest";
			}
			if(item.DisplayName == "Scholar'sHood"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Scholar's Hood";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "7";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Head";
			}
			if(item.DisplayName == "Scholar'sShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Scholar's Shoes";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "1";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Feet";
			}
			if(item.DisplayName == "Scholar'sPants"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Scholar's Pants";
                
            Weight = "4";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "8";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Leggings";
			}
			if(item.DisplayName == "Scholar'sCuff's"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Scholar's Cuff's";
                
            Weight = "2";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "2";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "1";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Wrists";
			}
			if(item.DisplayName == "Scholar'sMantle"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Scholar's Mantle";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "8";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "1";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Shoulders";
			}
			if(item.DisplayName == "Scholar'sSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Scholar's Sleeves";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "2";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Arms";
			}
			if(item.DisplayName == "Scholar'sCord"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Scholar's Cord";
                
            Weight = "1";
            STRENGTH_item = "2";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "9";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "1";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Waist";
			}
            if(item.DisplayName == "Scholar'sGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Scholar's Gloves";
                
            Weight = "2";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "Rare";
            MagicResist_item = "3";
            FireResist_item = "3";
            ColdResist_item = "3";
            PoisonResist_item = "3";
            DiseaseResist_item = "3";
            Armor = "1";
            //ItemDescript = "Anyone would be proud to wear this";
            TypeOfItem = "Cloth Hands";
			}
							if(item.DisplayName == "Arcanist'sRobe"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Arcanist's Robe";
                
            Weight = "6";
            STRENGTH_item = "3";
            AGILITY_item = "0";
            FORTITUDE_item = "10";
            ARCANA_item = "15";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Chest";
			}
			if(item.DisplayName == "Arcanist'sHood"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Arcanist's Hood";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "1";
            FORTITUDE_item = "2";
            ARCANA_item = "10";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "2";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Head";
			}
			if(item.DisplayName == "Arcanist'sShoes"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Arcanist's Shoes";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "8";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "2";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Feet";
			}
			if(item.DisplayName == "Arcanist'sPants"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Arcanist's Pants";
                
            Weight = "4";
            STRENGTH_item = "5";
            AGILITY_item = "0";
            FORTITUDE_item = "4";
            ARCANA_item = "15";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "3";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Leggings";
			}
			if(item.DisplayName == "Arcanist'sCuff's"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Arcanist's Cuff's";
                
            Weight = "2";
            STRENGTH_item = "3";
            AGILITY_item = "2";
            FORTITUDE_item = "2";
            ARCANA_item = "9";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "2";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Wrists";
			}
			if(item.DisplayName == "Arcanist'sMantle"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Arcanist's Mantle";
                
            Weight = "2";
            STRENGTH_item = "2";
            AGILITY_item = "0";
            FORTITUDE_item = "1";
            ARCANA_item = "8";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "2";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Shoulders";
			}
			if(item.DisplayName == "Arcanist'sSleeves"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Arcanist's Sleeves";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "2";
            FORTITUDE_item = "3";
            ARCANA_item = "8";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "2";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Arms";
			}
			if(item.DisplayName == "Arcanist'sCord"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Arcanist's Cord";
                
            Weight = "1";
            STRENGTH_item = "2";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "9";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "2";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Waist";
			}
			if(item.DisplayName == "Arcanist'sGloves"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Arcanist's Gloves";
                
            Weight = "2";
            STRENGTH_item = "1";
            AGILITY_item = "3";
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "4";
            FireResist_item = "4";
            ColdResist_item = "4";
            PoisonResist_item = "4";
            DiseaseResist_item = "4";
            Armor = "1";
            //ItemDescript = "Only a seasoned fighter would be seen in this";
            TypeOfItem = "Cloth Hands";
			}
			if(item.DisplayName == "RobeOfTerror"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Robe Of Terror";
                
            Weight = "6";
            STRENGTH_item = "5";
            AGILITY_item = "1";
            FORTITUDE_item = "10";
            ARCANA_item = "20";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "4";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Chest";
			}
			if(item.DisplayName == "CoifOfKnowledge"){
            type = ItemSelectable.ItemType.Head;
            Item_Slot = "Head";
            Itemname = "Coif Of Knowledge";
                
            Weight = "2";
            STRENGTH_item = "3";
            AGILITY_item = "1";
            FORTITUDE_item = "2";
            ARCANA_item = "12";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "3";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Head";
			}
			if(item.DisplayName == "ShoesOfTenacity"){
            type = ItemSelectable.ItemType.Feet;
            Item_Slot = "Feet";
            Itemname = "Shoes Of Tenacity";
                
            Weight = "2";
            STRENGTH_item = "4";
            AGILITY_item = "3";
            FORTITUDE_item = "4";
            ARCANA_item = "11";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "2";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Feet";
			}
			if(item.DisplayName == "ArcanaInfusedPants"){
            type = ItemSelectable.ItemType.Leggings;
            Item_Slot = "Leggings";
            Itemname = "Arcana Infused Pants";
                
            Weight = "4";
            STRENGTH_item = "5";
            AGILITY_item = "5";
            FORTITUDE_item = "10";
            ARCANA_item = "15";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "3";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Leggings";
			}
			if(item.DisplayName == "CuffsOfNature"){
            type = ItemSelectable.ItemType.Wrists;
            Item_Slot = "Wrists";
            Itemname = "Cuffs Of Nature";
                
            Weight = "2";
            STRENGTH_item = "3";
            AGILITY_item = "2";
            FORTITUDE_item = "2";
            ARCANA_item = "9";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "2";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Wrists";
			}
			if(item.DisplayName == "Gold-InlaidMantle"){
            type = ItemSelectable.ItemType.Shoulders;
            Item_Slot = "Shoulders";
            Itemname = "Gold-Inlaid Mantle";
                
            Weight = "2";
            STRENGTH_item = "2";
            AGILITY_item = "2";
            FORTITUDE_item = "4";
            ARCANA_item = "11";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "2";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Shoulders";
			}
			if(item.DisplayName == "SleevesOfInsight"){
            type = ItemSelectable.ItemType.Arms;
            Item_Slot = "Arms";
            Itemname = "Sleeves Of Insight";
                
            Weight = "2";
            STRENGTH_item = "2";
            AGILITY_item = "2";
            FORTITUDE_item = "3";
            ARCANA_item = "12";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "2";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Arms";
			}
			if(item.DisplayName == "CordOfTemperence"){
            type = ItemSelectable.ItemType.Waist;
            Item_Slot = "Waist";
            Itemname = "Cord Of Temperence";
                
            Weight = "1";
            STRENGTH_item = "2";
            AGILITY_item = "1";
            FORTITUDE_item = "1";
            ARCANA_item = "9";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "2";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Waist";
			}
			if(item.DisplayName == "GlovesOfTheInvoker"){
            type = ItemSelectable.ItemType.Hands;
            Item_Slot = "Hands";
            Itemname = "Gloves Of The Invoker";
                
            Weight = "2";
            STRENGTH_item = "5";
            AGILITY_item = "3";
            FORTITUDE_item = "2";
            ARCANA_item = "15";
            Rarity_item = "Exotic";
            MagicResist_item = "5";
            FireResist_item = "5";
            ColdResist_item = "5";
            PoisonResist_item = "5";
            DiseaseResist_item = "5";
            Armor = "2";
            //ItemDescript = "Mortal men couldnt understand this power";
            TypeOfItem = "Cloth Hands";
			}
			if(item.DisplayName == "SalivaStainedRobe"){
            type = ItemSelectable.ItemType.Chest;
            Item_Slot = "Chest";
            Itemname = "Saliva Stained Robe";
                
            Weight = "6";
            STRENGTH_item = "1";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "5";
            Rarity_item = "UltraRare";
            MagicResist_item = "1";
            FireResist_item = "1";
            ColdResist_item = "1";
            PoisonResist_item = "1";
            DiseaseResist_item = "1";
            Armor = "2";
            //ItemDescript = "Sticky And Wet With Saliva";
            TypeOfItem = "Cloth Chest";
			}
            if(item.DisplayName == "Staff"){
            type = ItemSelectable.ItemType.TwoHandedWeapon;
            Item_Slot = "Two Handed";
            Itemname = "Staff";
            DamageMin_item = "20";
            DamageMax_item = "35";
            Parry_item = "15";
            Penetration_item = "1";
            AttackDelay_item = "75";
            Weight = "3.5";
            STRENGTH_item = "0";
            AGILITY_item = "0";
            FORTITUDE_item = "0";
            ARCANA_item = "0";
            Rarity_item = "Common";
            MagicResist_item = "0";
            FireResist_item = "0";
            ColdResist_item = "0";
            PoisonResist_item = "0";
            DiseaseResist_item = "0";
            Armor = "0";
            //ItemDescript = "Starting Staff";
            TypeOfItem = "Staff";
			}
        bool tacticianPurchase =  false;
        bool charInv = false;
        bool TactInv = false;
        string ownerID = string.Empty;
        if(selector == TACTICIAN){
            TactInv = true;
            ownerID = selector;
        }
        if(selector == "Stash"){
            tacticianPurchase = true;
            ownerID = selector;
        }
        if(selector != TACTICIAN && selector != "Stash"){
            charInv = true;
            ownerID = selector;
        } 
        string duramin = "100";
        string duramax = "100";
        if(type == ItemSelectable.ItemType.Ammo || type == ItemSelectable.ItemType.Misc || type == ItemSelectable.ItemType.RefinedMaterialT1 || type == ItemSelectable.ItemType.MaterialT1
        || type == ItemSelectable.ItemType.LockPick || type == ItemSelectable.ItemType.GemstoneT2 || type == ItemSelectable.ItemType.GemstoneT3 || type == ItemSelectable.ItemType.GemstoneT4 
        || type == ItemSelectable.ItemType.GemstoneT5){
            duramin = "None";
            duramax = "None";
        }
        bool NFT = false;
        if(Rarity_item == "NFT"){
            NFT = true;
        }
        string ItemDescript = GetItemDescription(Itemname);
        ItemSelectable itemAdded = new ItemSelectable{
            itemType = type, amount = quant, Weight = Weight, 
            STRENGTH_item = STRENGTH_item, AGILITY_item = AGILITY_item,
            FORTITUDE_item = FORTITUDE_item, ARCANA_item = ARCANA_item,
            MagicResist_item = MagicResist_item, FireResist_item = FireResist_item, 
            ColdResist_item = ColdResist_item, PoisonResist_item = PoisonResist_item,
            DiseaseResist_item = DiseaseResist_item, Rarity_item = Rarity_item,
            Item_Name = Itemname, Durability = duramin, DurabilityMax = duramax,
            Parry = Parry_item, Penetration = Penetration_item, AttackDelay = AttackDelay_item,
            BlockChance = BlockChance_item, BlockValue = BlockValue_item, 
            ItemSpecificClass = TypeOfItem, itemSlot = Item_Slot, Armor_item = Armor,
            InstanceID = item.ItemInstanceId, OwnerID = ownerID, DamageMin = DamageMin_item, DamageMax = DamageMax_item,
            TacticianInventory = TactInv, OGTacticianInventory = TactInv, TacticianStash = tacticianPurchase, OGTacticianStash = tacticianPurchase, NFT = NFT, ItemDescription = ItemDescript,
            INVENTORY = charInv, Quality_item = Quality, customID = Guid.NewGuid().ToString(), itemID = item.ItemId, NFTID = nftID
        };
        statBookOne.Add("Weight", Weight);
        statBookOne.Add("STRENGTH_item", STRENGTH_item);
        statBookOne.Add("AGILITY_item", AGILITY_item);
        statBookOne.Add("FORTITUDE_item", FORTITUDE_item);
        statBookOne.Add("ARCANA_item", ARCANA_item);
            
        statBookTwo.Add("Rarity_item", Rarity_item);
        statBookTwo.Add("MagicResist_item", MagicResist_item);
        statBookTwo.Add("FireResist_item", FireResist_item);
        statBookTwo.Add("ColdResist_item", ColdResist_item);
        statBookTwo.Add("PoisonResist_item", PoisonResist_item);
            
        statBookThree.Add("DiseaseResist_item", DiseaseResist_item);
        if(type == ItemSelectable.ItemType.Ammo || type == ItemSelectable.ItemType.Misc || type == ItemSelectable.ItemType.RefinedMaterialT1 || type == ItemSelectable.ItemType.MaterialT1
        || type == ItemSelectable.ItemType.LockPick || type == ItemSelectable.ItemType.GemstoneT2 || type == ItemSelectable.ItemType.GemstoneT3 || type == ItemSelectable.ItemType.GemstoneT4 
        || type == ItemSelectable.ItemType.GemstoneT5){
            statBookThree.Add("Durability_item", "None");
            statBookThree.Add("DurabilityMax_item", "None");
        } else {
            if(!NFT){
                statBookThree.Add("Durability_item", "100");
                statBookThree.Add("DurabilityMax_item", "100");
            } else {
                statBookThree.Add("Durability_item", "Indestructible");
                statBookThree.Add("DurabilityMax_item", "Indestructible");
            }
                
        }
        statBookThree.Add("Item_Slot", Item_Slot);
        statBookThree.Add("TypeOfItem", TypeOfItem);
        if(tacticianPurchase){
            statBookFour.Add("TactStash", "Stashed");
        }
        if(TactInv){
            statBookFour.Add("TactInventory", "InventoryItem");
        }
        statBookFour.Add("Item_Name", Itemname);
        statBookFour.Add("Armor_item", Armor);
        statBookFour.Add("BlockChance_item", BlockChance_item);
        statBookFour.Add("BlockValue_item", BlockValue_item);
            
        statBookFive.Add("DamageMin_item", DamageMin_item);
        statBookFive.Add("DamageMax_item", DamageMax_item);
        statBookFive.Add("Parry_item", Parry_item);
        statBookFive.Add("Penetration_item", Penetration_item);
        statBookFive.Add("AttackDelay_item", AttackDelay_item);
        statBookSix.Add("Amount", quant.ToString());
        statBookSix.Add("Quality_item", Quality);
        if(NFT){
            statBookSix.Add("NFT", "NFT");
            statBookSix.Add("NFTID", nftID);
        } else {
            statBookSix.Add("ItemDescription", ItemDescript);
        }
        //print($"Building item {itemAdded.GetItemName()}***************");
            
        SignBookOnePurchase(selector, nconn, item.ItemInstanceId, playerData, itemAdded, statBookOne, statBookTwo, statBookThree, statBookFour, statBookFive, statBookSix, login, Unstacking);
    }
    #endif
    void SignBookOnePurchase(string select, NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookOne, Dictionary<string,string> statBookTwo, Dictionary<string,string> statBookThree, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, Dictionary<string,string> statBookSix, bool login, bool Unstacking){
        #if UNITY_SERVER || UNITY_EDITOR
        //print("made it to first part");
        Dictionary<string,string> statBook = statBookOne; 
            
        PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
        {
            ItemInstanceId = InstanceID,
            PlayFabId = playerData.PlayFabId,
            Data = statBook
        }, result =>{
            //PassItemToStash(nconn, item);
            SignBookTwoPurchase(select, nconn, InstanceID, playerData, item, statBookTwo, statBookThree, statBookFour, statBookFive, statBookSix, login, Unstacking);
        }, error =>{
            //print("Failed second part of first");
            Debug.Log(error.ErrorMessage);
            Debug.Log(error.ErrorDetails);
            Debug.Log(error.Error);
        });
        #endif
    }
        void SignBookTwoPurchase(string select, NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookTwo, Dictionary<string,string> statBookThree, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, Dictionary<string,string> statBookSix, bool login, bool Unstacking){
            #if UNITY_SERVER || UNITY_EDITOR
            Dictionary<string,string> statBook = statBookTwo; 

            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
                SignBookThreePurchase(select, nconn, InstanceID, playerData, item, statBookThree, statBookFour, statBookFive, statBookSix, login, Unstacking);
            }, error =>{
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void SignBookThreePurchase(string select, NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookThree, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, Dictionary<string,string> statBookSix, bool login, bool Unstacking){
            #if UNITY_SERVER || UNITY_EDITOR
            Dictionary<string,string> statBook = statBookThree; 

            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
                SignBookFourPurchase(select, nconn, InstanceID, playerData, item, statBookFour, statBookFive, statBookSix, login, Unstacking);
            }, error =>{
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void SignBookFourPurchase(string select, NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookFour, Dictionary<string,string> statBookFive, Dictionary<string,string> statBookSix, bool login, bool Unstacking){
            #if UNITY_SERVER || UNITY_EDITOR
            Dictionary<string,string> statBook = statBookFour; 
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
           
                SignBookFivePurchase(select, nconn, InstanceID, playerData, item, statBookFive, statBookSix, login, Unstacking);
             
            }, error =>{
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void SignBookFivePurchase(string select, NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookFive, Dictionary<string,string> statBookSix, bool login, bool Unstacking){
            #if UNITY_SERVER || UNITY_EDITOR
            Dictionary<string,string> statBook = statBookFive; 
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
           
                SignBookSixPurchase(select, nconn, InstanceID, playerData, item, statBookSix, login, Unstacking);
             
            }, error =>{
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void SignBookSixPurchase(string select, NetworkConnectionToClient nconn, string InstanceID, PlayerInfo playerData, ItemSelectable item, Dictionary<string,string> statBookSix, bool login, bool Unstacking){
            #if UNITY_SERVER || UNITY_EDITOR
            Dictionary<string,string> statBook = statBookSix; 
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();

            //public struct StackingMessage : NetworkMessage
            //{
            //    public string ownerItemOne;
            //}
            
            StackingMessage message = (new StackingMessage {
                owner = sPlayer,
                ownerItemTwo = item.OwnerID,
                itemTwo = item,
                itemTwoAmount = 0
            });
            

            //here we will check to see if there are any we can stack with 
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = InstanceID,
                PlayFabId = playerData.PlayFabId,
                Data = statBook
            }, result =>{
                CharacterInventoryListItem newItem = (new CharacterInventoryListItem {
                    Key = item.GetInstanceID(),
                    Value = item
                     
                });
                if(select == "Stash"){
                    //print("select was Stash");
                    //put in tactician sheet pass to server then gucci
                    if(item.IsStackable() && !login){
                        foreach(var stashItem in sPlayer.GetTacticianSheet().StashInventoryData){
                            //if item is same and has room stack it! else pass as a new item
                            if(stashItem.Value.Item_Name == item.Item_Name && stashItem.Value.GetAvailableSpaceForStackable() >= 1 && !Unstacking){
                                int stackingItemAmount = stashItem.Value.amount;
                                message.ownerItemOne = "Tactician";
                                message.itemOneAmount = stashItem.Value.amount;
                                message.itemOne = stashItem.Value;
                                message.itemOne.Changed = true;
                                //print($"Select was {select} and we have an item we are trying to stack {stashItem.Value.Item_Name} with amount {stashItem.Value.amount}");
                                ServerRemoveItemOnUser(nconn, InstanceID);
                                StackingItemDropped(nconn, message);
                                return;
                            }
                        }
                        sPlayer.ServerPurchasedItemResult(newItem);
                    } else {
                        sPlayer.ServerPurchasedItemResult(newItem);
                    }
                } else {
                    if(select == "Tactician"){
                        //print("Select was Tactician");
                        //create logic to add to tactician inventory
                        if(item.IsStackable() && !login){
                            foreach(var tactItem in sPlayer.GetTacticianSheet().TacticianInventoryData){
                                //if item is same and has room stack it! else pass as a new item
                                if(tactItem.Value.Item_Name == item.Item_Name && tactItem.Value.GetAvailableSpaceForStackable() >= 1 && !Unstacking){
                                    int stackingItemAmount = tactItem.Value.amount;
                                    message.ownerItemOne = "Tactician";
                                    message.itemOneAmount = tactItem.Value.amount;
                                    message.itemOne = tactItem.Value;
                                    message.itemOne.Changed = true;
                                    //print($"Select was {select} and we have an item we are trying to stack {tactItem.Value.Item_Name} with amount {tactItem.Value.amount}");
                                    ServerRemoveItemOnUser(nconn, InstanceID);
                                    StackingItemDropped(nconn, message);
                                    return;
                                }
                            }
                            sPlayer.ServerTacticianItemResult(newItem);
                        } else {
                            sPlayer.ServerTacticianItemResult(newItem);
                        }
                    } else {
                        //print($"Select was {select}");
                        foreach(var sheet in sPlayer.GetInformationSheets()){
                            if(sheet.CharacterID == item.OwnerID){
                                foreach(var invItem in sheet.CharInventoryData){
                                    if(item.IsStackable()){
                                        if(invItem.Value.Item_Name == item.Item_Name && invItem.Value.GetAvailableSpaceForStackable() >= 1 && !Unstacking){
                                            int stackingItemAmount = invItem.Value.amount;
                                            message.ownerItemOne = sheet.CharacterID;
                                            message.itemOneAmount = invItem.Value.amount;
                                            message.itemOne = invItem.Value;
                                            message.itemOne.Changed = true;
                                            //print($"Select was {select} and we have an item we are trying to stack {invItem.Value.Item_Name} with amount {invItem.Value.amount}");
                                            ServerRemoveItemOnUser(nconn, InstanceID);
                                            StackingItemDropped(nconn, message);
                                            return;
                                        }
                                    }
                                }
                                PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                                {
                                    CharacterId = select,
                                    ItemInstanceId = item.GetInstanceID(),
                                    PlayFabId = playerData.PlayFabId,
                                }, result =>
                                {
                                    string _class = string.Empty;
                                    foreach(var sheet in sPlayer.GetInformationSheets()){
                                        if(sheet.CharacterID == select){
                                            foreach(var stat in sheet.CharStatData){
                                                if(stat.Key == "Class"){
                                                    _class = stat.Value;
                                                }
                                            }
                                        }
                                    }
                                    sPlayer.GetCharacterPickedUpItem(select, _class, newItem);
                                }, error =>{
                                    Debug.Log(error.ErrorMessage);
                                });
                            }
                        }
                    }
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void ServerRemoveItemOnUser(NetworkConnectionToClient nconn, string ID){
            #if UNITY_SERVER || UNITY_EDITOR
            if(ID == "NewItemGenerated"){
                return;
            }
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.RevokeInventoryItem(new RevokeInventoryItemRequest {
                PlayFabId = playerData.PlayFabId,
                ItemInstanceId = ID
            }, result => {
                Debug.Log("Item successfully revoked");
            }, error => {
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void ServerRemoveItemOnUserFromCharacter(NetworkConnectionToClient nconn, string ID, string CharacterID){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            
            PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
            {
                CharacterId = CharacterID,
                ItemInstanceId = ID,
                PlayFabId = playerData.PlayFabId,
            }, result =>
            {
                PlayFabServerAPI.RevokeInventoryItem(new RevokeInventoryItemRequest {
                    PlayFabId = playerData.PlayFabId,
                    ItemInstanceId = ID
                }, result => {
                    Debug.Log("Item successfully revoked");
                }, error => {
                    Debug.Log(error.ErrorMessage);
                });
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void ServerAddItemLogout(NetworkConnectionToClient nconn, string _itemName, string CharacterID, bool tactician, bool stash, int amount, string weight, string rarity, string ItemID){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            string ID = string.Empty;
            List<string> itemIDCreation = new List<string>();
            itemIDCreation.Add(ItemID);
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = itemIDCreation
            }, result =>
            {
                
                Dictionary<string, string> dataInput = new Dictionary<string, string>();
                if(stash){
                    dataInput.Add("TactStash", "Stashed");
                }
                if(tactician){
                    dataInput.Add("TactInventory", "InventoryItem");
                }
                dataInput.Add("Rarity_item", rarity);
                dataInput.Add("Item_Name", _itemName);
                dataInput.Add("Amount", amount.ToString());
                dataInput.Add("Weight", weight);
                foreach(var item in result.ItemGrantResults){
                    //print($"{choice} is choice {Quality} is quality");
                    ID = item.ItemInstanceId;
                    PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
                    ItemInstanceId = item.ItemInstanceId,
                    PlayFabId = playerData.PlayFabId,
                    Data = dataInput
                        }, result => {

                        if(!tactician && !stash){
                            PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                            {
                                CharacterId = CharacterID,
                                ItemInstanceId = ID,
                                PlayFabId = playerData.PlayFabId,
                            }, result =>
                            {
                            
                            }, error =>{
                                Debug.Log(error.ErrorMessage);
                            });
                        }
                            //}, result => {
                        //print($"moved and doneskies with item one going on to item two now, new amount is {message.itemOneAmount}");
                    }, error =>{
                        Debug.Log(error.ErrorMessage);
                    });
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            
            #endif
        }
        //Transferring items between tactician and characters
        void StackingItemDropped(NetworkConnectionToClient nconn, StackingMessage message){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            // we need to take all the info from the message, either change quantities on both or delete the second item. Then we need to send them back to their appropriate owner
            if(message.itemOne.OwnerID == TACTICIAN){
                // if filled we are good to go and just send to tactician and kill other item otherwise we keep going
                TacticianFullDataMessage tactSheet = message.owner.GetTacticianSheet();
                CharacterInventoryListItem ItemOneTact = (new CharacterInventoryListItem{
                    Key = message.itemOne.GetInstanceID(),
                    Value = message.itemOne
                });
                ItemOneTact.Value.amount = message.itemOneAmount + 1;
                ItemOneTact.Value.Changed = true;
                //message.owner.GetTacticianRemoveItem(ItemOneTact.Key);
                message.owner.GetTacticianNewItem(ItemOneTact);
                message.owner.TargetSendInventoryItemSelectable(ItemOneTact);
                //PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
                //    ItemInstanceId = ItemOneTact.Key,
                //    PlayFabId = playerData.PlayFabId,
                //    Data = new Dictionary<string, string> {
                //        {"Amount", ItemOneTact.Value.amount.ToString()}
                //    }
                //}, result => {
                //    //print($"moved and doneskies with item one going on to item two now, new amount is {message.itemOneAmount}");
                //}, error =>{
                //    Debug.Log(error.ErrorMessage);
                //});
            } else {
                CharacterInventoryListItem ItemOneTact = (new CharacterInventoryListItem{
                    Key = message.itemOne.GetInstanceID(),
                    Value = message.itemOne
                });
                ItemOneTact.Value.amount = message.itemOneAmount + 1;
                ItemOneTact.Value.Changed = true;
                //message.owner.RemoveCharacterItem(message.ownerItemOne, ItemOneTact.Key);
                message.owner.GetCharacterNewItem(message.ownerItemOne, ItemOneTact);
                message.owner.TargetSendInventoryItemSelectable(ItemOneTact);
                //deliver to tactician then change the amount
                //PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
                //{
                //    CharacterId = message.ownerItemOne,
                //    ItemInstanceId = ItemOneTact.Key,
                //    PlayFabId = playerData.PlayFabId,
                //}, result =>
                //{
                //   PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
                //    {
                //        ItemInstanceId = ItemOneTact.Key,
                //        PlayFabId = playerData.PlayFabId,
                //        Data = new Dictionary<string, string>
                //            {
                //                {"Amount", ItemOneTact.Value.amount.ToString()}
                //            }
                //    }, result =>
                //    {
                //        PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                //        {
                //            CharacterId = message.ownerItemOne,
                //            ItemInstanceId = ItemOneTact.Key,
                //            PlayFabId = playerData.PlayFabId,
                //        }, result =>
                //        {
                //        }, error =>{
                //            Debug.Log(error.ErrorMessage);
                //        });
                //    }, error =>{
                //        Debug.Log(error.ErrorMessage);
                //    });
                //}, error =>{
                //    Debug.Log(error.ErrorMessage);
                //});                
            }
           CharacterInventoryListItem ItemUpdate = (new CharacterInventoryListItem{
                    Key = message.itemOne.GetInstanceID(),
                    Value = message.itemOne
            });
            ItemUpdate.Value.amount = message.itemOneAmount + 1;
            message.owner.TargetSendInventoryItemSelectable(ItemUpdate); //we are amost there just gotta update the item and send it back
            #endif
        }
        void RemoveThisItem(NetworkConnectionToClient conn, ItemSelectable itemDeleting, string serial){
            CharacterInventoryListItem DeletingItem = (new CharacterInventoryListItem{
                Key = itemDeleting.GetInstanceID(),
                Value = itemDeleting
            });
            DeletingItem.Value.amount = 0;
            DeletingItem.Value.Destroying = true;
            DeletingItem.Value.Changed = true;
            ScenePlayer sPlayer = conn.identity.gameObject.GetComponent<ScenePlayer>();
            if(serial == "Tactician"){
                sPlayer.GetTacticianNewItem(DeletingItem);
                return;
            }
            if(serial == "Stash"){
                sPlayer.GetStashNewItem(DeletingItem);
                return;
            }
            if(serial != "Stash" && serial != "Tactician"){
                foreach(var sheet in sPlayer.GetInformationSheets()){
                    if(sheet.CharacterID == serial){
                        sPlayer.GetCharacterNewItem(serial, DeletingItem);
                        return;
                    }
                }
            }
        }
        void StackingItem(NetworkConnectionToClient nconn, StackingMessage message){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            // we need to take all the info from the message, either change quantities on both or delete the second item. Then we need to send them back to their appropriate owner
            if(message.itemOne.OwnerID == TACTICIAN || message.itemOne.OwnerID == "Stash"){
                // if filled we are good to go and just send to tactician and kill other item otherwise we keep going
                TacticianFullDataMessage tactSheet = message.owner.GetTacticianSheet();
                CharacterInventoryListItem ItemOneTact = (new CharacterInventoryListItem{
                    Key = message.itemOne.GetInstanceID(),
                    Value = message.itemOne
                });
                ItemOneTact.Value.amount = message.itemOneAmount;
                ItemOneTact.Value.Changed = true;
                if(ItemOneTact.Value.TacticianStash){
                    message.owner.GetStashNewItem(ItemOneTact);
                } else {
                    message.owner.GetTacticianNewItem(ItemOneTact);
                }
                message.owner.TargetSendInventoryItemSelectable(ItemOneTact);
                //PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
                //    ItemInstanceId = ItemOneTact.Key,
                //    PlayFabId = playerData.PlayFabId,
                //    Data = new Dictionary<string, string> {
                //        {"Amount", message.itemOneAmount.ToString()}
                //    }
                //}, result => {
                //    //print($"moved and doneskies with item one going on to item two now, new amount is {message.itemOneAmount}");
                //}, error =>{
                //    Debug.Log(error.ErrorMessage);
                //});
            } else {
                CharacterInventoryListItem ItemOneTact = (new CharacterInventoryListItem{
                    Key = message.itemOne.GetInstanceID(),
                    Value = message.itemOne
                });
                ItemOneTact.Value.amount = message.itemOneAmount;
                ItemOneTact.Value.Changed = true;
                message.owner.GetCharacterNewItem(message.ownerItemOne, ItemOneTact);
                message.owner.TargetSendInventoryItemSelectable(ItemOneTact);
                //deliver to tactician then change the amount
                //PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
                //{
                //    CharacterId = message.ownerItemOne,
                //    ItemInstanceId = ItemOneTact.Key,
                //    PlayFabId = playerData.PlayFabId,
                //}, result =>
                //{
                //   PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
                //    {
                //        ItemInstanceId = ItemOneTact.Key,
                //        PlayFabId = playerData.PlayFabId,
                //        Data = new Dictionary<string, string>
                //            {
                //                {"Amount", message.itemOneAmount.ToString()}
                //            }
                //    }, result =>
                //    {
                //        PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                //        {
                //            CharacterId = message.ownerItemOne,
                //            ItemInstanceId = ItemOneTact.Key,
                //            PlayFabId = playerData.PlayFabId,
                //        }, result =>
                //        {
                //        }, error =>{
                //            Debug.Log(error.ErrorMessage);
                //        });
                //    }, error =>{
                //        Debug.Log(error.ErrorMessage);
                //    });
                //}, error =>{
                //    Debug.Log(error.ErrorMessage);
                //});                
            }
            if(message.itemTwo.OwnerID == TACTICIAN || message.itemTwo.OwnerID == "Stash"){
                    TacticianFullDataMessage tactSheet = message.owner.GetTacticianSheet();
                if(message.itemTwoAmount == 0){
                    CharacterInventoryListItem ItemTwoTactRemove = (new CharacterInventoryListItem{
                        Key = message.itemTwo.GetInstanceID(),
                        Value = message.itemTwo
                    });
                    ItemTwoTactRemove.Value.amount = 0;
                    ItemTwoTactRemove.Value.Changed = true;
                    message.owner.TargetSendInventoryItemSelectable(ItemTwoTactRemove);
                    //PlayFabServerAPI.RevokeInventoryItem(new RevokeInventoryItemRequest {
                    //    PlayFabId = playerData.PlayFabId,
                    //    ItemInstanceId = message.itemTwo.GetInstanceID()
                    //}, result => {
                    //    Debug.Log("Item successfully revoked");
                    //}, error => {
                    //    Debug.Log(error.ErrorMessage);
                    //});
                    if(ItemTwoTactRemove.Value.TacticianStash){
                        message.owner.GetStashNewItem(ItemTwoTactRemove);
                    } else {
                        message.owner.GetTacticianNewItem(ItemTwoTactRemove);
                    }
                } else {
                    CharacterInventoryListItem ItemTwoTact = (new CharacterInventoryListItem{
                        Key = message.itemTwo.GetInstanceID(),
                        Value = message.itemTwo
                    });
                    ItemTwoTact.Value.amount = message.itemTwoAmount;
                    ItemTwoTact.Value.Changed = true;
                    if(ItemTwoTact.Value.TacticianStash){
                        message.owner.GetStashNewItem(ItemTwoTact);
                    } else {
                        message.owner.GetTacticianNewItem(ItemTwoTact);
                    }
                    message.owner.TargetSendInventoryItemSelectable(ItemTwoTact);
                    //PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
                    //    ItemInstanceId = ItemTwoTact.Key,
                    //    PlayFabId = playerData.PlayFabId,
                    //    Data = new Dictionary<string, string> {
                    //        {"Amount", message.itemTwoAmount.ToString()}
                    //    }
                    //}, result => {
                    //    //print("moved and doneskies with item one going on to item two now");
                    //}, error =>{
                    //    Debug.Log(error.ErrorMessage);
                    //});
                }
            } else {
                if(message.itemTwoAmount == 0){
                    CharacterInventoryListItem ItemRemove = (new CharacterInventoryListItem{
                        Key = message.itemTwo.GetInstanceID(),
                        Value = message.itemTwo
                    });
                    ItemRemove.Value.amount = message.itemTwoAmount;
                    ItemRemove.Value.Changed = true;
                    message.owner.GetCharacterNewItem(message.ownerItemTwo, ItemRemove);
                    message.owner.TargetSendInventoryItemSelectable(ItemRemove);
                    //PlayFabServerAPI.RevokeInventoryItem(new RevokeInventoryItemRequest {
                    //    PlayFabId = playerData.PlayFabId,
                    //    CharacterId = message.ownerItemTwo, // The ID of the character from which to revoke the item
                    //    ItemInstanceId = message.itemTwo.GetInstanceID()
                    //}, result => {
                    //    Debug.Log("Item successfully revoked from character");
                    //}, error => {
                    //    Debug.Log(error.ErrorMessage);
                    //});
                } else {
                    //change characters items amount and resend
                    CharacterInventoryListItem ItemTwoTact = (new CharacterInventoryListItem{
                        Key = message.itemTwo.GetInstanceID(),
                        Value = message.itemTwo
                    });
                    ItemTwoTact.Value.amount = message.itemTwoAmount;
                    ItemTwoTact.Value.Changed = true;
                    message.owner.GetCharacterNewItem(message.ownerItemTwo, ItemTwoTact);
                    message.owner.TargetSendInventoryItemSelectable(ItemTwoTact);
                    //deliver to tactician then change the amount
                    //PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
                    //{
                    //    CharacterId = message.ownerItemTwo,
                    //    ItemInstanceId = ItemTwoTact.Key,
                    //    PlayFabId = playerData.PlayFabId,
                    //}, result =>
                    //{
                    //   PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
                    //    {
                    //        ItemInstanceId = ItemTwoTact.Key,
                    //        PlayFabId = playerData.PlayFabId,
                    //        Data = new Dictionary<string, string>
                    //            {
                    //                {"Amount", message.itemOneAmount.ToString()}
                    //            }
                    //    }, result =>
                    //    {
                    //        PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                    //        {
                    //            CharacterId = message.ownerItemTwo,
                    //            ItemInstanceId = ItemTwoTact.Key,
                    //            PlayFabId = playerData.PlayFabId,
                    //        }, result =>
                    //        {
                    //        }, error =>{
                    //            Debug.Log(error.ErrorMessage);
                    //        });
                    //    }, error =>{
                    //        Debug.Log(error.ErrorMessage);
                    //    });
                    //}, error =>{
                    //    Debug.Log(error.ErrorMessage);
                    //}); 
                }
            }
            #endif
        }
        void AuthenticateThenSavePFCharacters(NetworkConnectionToClient nconn, ItemSelectable item, string request, string characterID){
            #if UNITY_SERVER || UNITY_EDITOR
            //print($"{item.GetItemName()} is being transfered to slot {characterID}");

            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                if(request == "TactEquipCharInv"){
                    TactEquipCharInv(nconn, item, playerData, characterID);//changed
                }
                if(request == "TactInvCharInv"){
                    TactInvCharInv(nconn, item, playerData, characterID);//changed
                }
                if(request == "TactBeltCharInv"){
                    TactBeltCharInv(nconn, item, playerData, characterID);//changed
                }
                if(request == "StashToCharInv"){
                    StashToCharInv(nconn, item, playerData, characterID);//changed
                }
                if(request == "CharInvTactInv"){
                    CharInvTactInv(nconn, item, playerData, characterID);//changed
                }
                if(request == "CharInvTactBelt"){
                    CharInvTactBelt(nconn, item, playerData, characterID);//changed
                }
                if(request == "CharEquipTactInv"){
                    CharEquipTactInv(nconn, item, playerData, characterID);//changed
                }
                if(request == "CharEquipTactBelt"){
                    CharEquipTactBelt(nconn, item, playerData, characterID);//changed
                }
                if(request == "CharInvStash"){
                    CharInvStash(nconn, item, playerData, characterID);//changed
                }
                if(request == "CharEquipStash"){
                    CharEquipStash(nconn, item, playerData, characterID);//changed
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void AuthenticateDropBox(NetworkConnectionToClient nconn, string itemOne, string itemTwo, EquippingData equippedData){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                if(equippedData.Request == "CharacterUnequipToTactStash"){
                    CharUnequipCharEquipSendTactStash(nconn, itemOne, itemTwo, playerData, equippedData);//changed
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void AuthenticateThenSavePFCharactersSwap(NetworkConnectionToClient nconn, ItemSelectable itemOne, ItemSelectable itemTwo, EquippingData equippedData){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                if(equippedData.Request == "CharUnequipCharEquip"){
                    CharUnequipCharEquipSame(nconn, itemOne, itemTwo, playerData, equippedData);//changed
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void AuthenticateThenSavePFCharactersEquip(NetworkConnectionToClient nconn, ItemSelectable item, string request, EquippingData equippedData){
            #if UNITY_SERVER || UNITY_EDITOR
            //print($"{item.GetItemName()} is being transfered to slot {equippedData.Slot} {request} was the request, {equippedData.CharacterSlot} was slot ");

            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                if(request == "TactBeltToCharEquip"){
                    TactBeltToCharEquip(nconn, item, playerData, equippedData);//changed
                }
                if(request == "TactInvToCharEquip"){
                    TactInvToCharEquip(nconn, item, playerData, equippedData);//changed
                }
                if(request == "StashToCharEquip"){
                    StashToCharEquip(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharInvTactEquip"){
                    CharInvTactEquip(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharInvCharInv"){
                    CharInvCharInv(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharEquipCharInv"){
                    CharEquipCharInv(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharInvCharEquip"){
                    CharInvCharEquip(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharEquipCharEquip"){
                    CharEquipCharEquip(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharEquipEquipSame"){
                    CharEquipEquipSame(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharInvEquipSame"){
                    CharInvEquipSame(nconn, item, playerData, equippedData);//changed
                }
                if(request == "CharEquipInvSame"){
                    CharEquipInvSame(nconn, item, playerData, equippedData);//changed
                }

            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void AuthenticateThenSavePF(NetworkConnectionToClient nconn, ItemSelectable item, string request){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                if(request == "StashToTactInventory"){
                    StashToTactInventory(nconn, item, playerData);//changed
                }
                if(request == "StashToTactBelt"){
                    StashToTactBelt(nconn, item, playerData);//changed
                }
                if(request == "TactInventoryToStash"){
                    TactInventoryToStash(nconn, item, playerData);//changed
                }
                if(request == "TactInvToTactBelt"){
                    TactInvToTactBelt(nconn, item, playerData);//changed
                }
                if(request == "TactEquipToStash"){
                    TactEquipToStash(nconn, item, playerData);//changed
                }
                if(request == "TactBeltToStash"){
                    TactBeltToStash(nconn, item, playerData);//changed
                } 
                if(request == "TactEquipToTactInv"){
                    TactEquipToTactInv(nconn, item, playerData);//changed
                }
                if(request == "TactEquipToTactBelt"){
                    TactEquipToTactBelt(nconn, item, playerData);//changed
                }
                if(request == "TactBeltToTactInv"){
                    TactBeltToTactInv(nconn, item, playerData);//changed
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void AuthenticateThenSavePFEquip(NetworkConnectionToClient nconn, ItemSelectable item, string request, string SlotName){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                if(request == "StashToTactEquip"){
                    StashToTactEquip(nconn, item, playerData, SlotName);//changed
                }
                if(request == "TacticianInvToTacticianEquip"){
                    TactInvToTactEquip(nconn, item, playerData, SlotName);//changed
                }
                if(request == "TactBeltToTactEquip"){
                    TactBeltToTactEquip(nconn, item, playerData, SlotName);//changed
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
            #if UNITY_SERVER || UNITY_EDITOR

        void SendItemToUserForUpdatingThenBackToDesiredSerial(NetworkConnectionToClient nconn, ItemSelectable item, EquipmentSaveData data){
            print("Launching SendItemToUserForUpdatingThenBackToDesiredSerial");

            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            string slot = null;
            slot = data.Slot;
            string durability = null;
            durability = item.Durability;
            Dictionary<string, string> DATA = new Dictionary<string, string>();
            if(data.OGTactBelt){
                DATA.Add("TactBelt", null);
            }
            if(data.OGTactInv){
                DATA.Add("TactInventory", null);
            }
            if(data.OGStash){
                DATA.Add("TactStash", null);
            }
            DATA.Add("Durability_item", durability);
            DATA.Add("Amount", item.amount.ToString());
            DATA.Add("EquippedSlot", slot);
            print("Position 1");
            PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
            {
                CharacterId = data.CharacterSlotOne,
                ItemInstanceId = item.GetInstanceID(),
                PlayFabId = playerData.PlayFabId,
            }, result =>
            {
            print("Position 2");

                PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
                ItemInstanceId = item.GetInstanceID(),
                PlayFabId = playerData.PlayFabId,
                Data = DATA
            }, result => {
            print("Position 3");

                PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                {
                    CharacterId = data.CharacterSlotTwo,
                    ItemInstanceId = item.GetInstanceID(),
                    PlayFabId = playerData.PlayFabId,
                }, result =>
                {
            print("Position 4");
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
        }
        void SendItemFromTacticianToCharacter(NetworkConnectionToClient nconn, ItemSelectable item, EquipmentSaveData data){
            print("Launching SendItemFromTacticianToCharacter");

            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            string slot = null;
            slot = data.Slot;
            string durability = null;
            durability = item.Durability;
            Dictionary<string, string> DATA = new Dictionary<string, string>();
            if(data.OGTactBelt && !data.TactBelt){
                DATA.Add("TactBelt", null);
            }
            if(data.OGTactInv  && !data.TactInv){
                DATA.Add("TactInventory", null);
            }
            if(data.OGStash && !data.Stash){
                DATA.Add("TactStash", null);
            }
            if(durability != null){
                DATA.Add("Durability_item", durability);
            }
            DATA.Add("Amount", item.amount.ToString());
            if(slot != null){
                DATA.Add("EquippedSlot", slot);
            } else {
                DATA.Add("EquippedSlot", null);
            }
            print("Position 1 tact to char");
            print($"{data.CharacterSlotOne} was CharID");
            foreach(var dataitem in DATA){
                print($"{dataitem.Key} is our key and {dataitem.Value} is our value in the Data dictionary");
            }
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
            ItemInstanceId = item.GetInstanceID(),
            PlayFabId = playerData.PlayFabId,
            Data = DATA
            }, result => {
            print("Position 2");

                PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                {
                    CharacterId = data.CharacterSlotOne,
                    ItemInstanceId = item.GetInstanceID(),
                    PlayFabId = playerData.PlayFabId,
                }, result =>
                {
            print("Position 3");

                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
        }
        void SaveTacticianChange(NetworkConnectionToClient nconn, ItemSelectable item, EquipmentSaveData data){
            print("Launching SaveTacticianChange");
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            string slot = null;
            slot = data.Slot;
            string durability = null;
            durability = item.Durability;
            Dictionary<string, string> DATA = new Dictionary<string, string>();
            if(data.OGTactBelt && !data.TactBelt){
                DATA.Add("TactBelt", null);
            }
            if(data.OGTactInv && !data.TactInv){
                DATA.Add("TactInventory", null);
            }
            if(data.OGStash && !data.Stash){
                DATA.Add("TactStash", null);
            }
            if(data.Stash){
                if(DATA.ContainsKey("TactStash")){
                    DATA.Remove("TactStash");
                }
                DATA.Add("TactStash", "Stashed");
            }
            if(data.TactInv){
                if(DATA.ContainsKey("TactInventory")){
                    DATA.Remove("TactInventory");
                }
                DATA.Add("TactInventory", "InventoryItem");
            }
            if(data.TactBelt){
                if(DATA.ContainsKey("TactBelt")){
                    DATA.Remove("TactBelt");
                }
                DATA.Add("TactBelt", "Belted");
            }
            DATA.Add("Durability_item", durability);
            DATA.Add("Amount", item.amount.ToString());
            DATA.Add("EquippedSlot", slot);
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
            ItemInstanceId = item.GetInstanceID(),
            PlayFabId = playerData.PlayFabId,
            Data = DATA
            }, result => {
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
        }
        void SendItemFromCharacterToTactician(NetworkConnectionToClient nconn, ItemSelectable item, EquipmentSaveData data){
            print("Launching SendItemFromCharacterToTactician");
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            string slot = null;
            slot = data.Slot;
            string durability = null;
            durability = item.Durability;
            Dictionary<string, string> DATA = new Dictionary<string, string>();
            if(data.OGTactBelt && !data.TactBelt){
                DATA.Add("TactBelt", null);
            }
            if(data.OGTactInv && !data.TactInv){
                DATA.Add("TactInventory", null);
            }
            if(data.OGStash && !data.Stash){
                DATA.Add("TactStash", null);
            }
            if(data.Stash){
                if(DATA.ContainsKey("TactStash")){
                    DATA.Remove("TactStash");
                }
                DATA.Add("TactStash", "Stashed");
            }
            if(data.TactInv){
                if(DATA.ContainsKey("TactInventory")){
                    DATA.Remove("TactInventory");
                }
                DATA.Add("TactInventory", "InventoryItem");
            }
            if(data.TactBelt){
                if(DATA.ContainsKey("TactBelt")){
                    DATA.Remove("TactBelt");
                }
                DATA.Add("TactBelt", "Belted");
            }
            if(durability != null){
                DATA.Add("Durability_item", durability);
            }
            DATA.Add("Amount", item.amount.ToString());
            if(slot != null){
                DATA.Add("EquippedSlot", slot);
            } else {
                DATA.Add("EquippedSlot", null);
            }
            print($"Position 1 char to tact equip data includes {data.TactInv} TactInv, {data.TactBelt} TactBelt, {data.TactEquipped} TactEquip, {data.Stash} stash, {data.OGStash} is og stash, {data.OGTactInv} OGTactInv, {data.OGTactEquipped} OGTactEquipped, {data.OGTactBelt} OGTactBelt");
            print($"{data.CharacterSlotOne} was CharID");
            foreach(var dataitem in DATA){
                print($"{dataitem.Key} is our key and {dataitem.Value} is our value in the Data dictionary");
            }
                PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
                {
                    CharacterId = data.CharacterSlotOne,
                    ItemInstanceId = item.GetInstanceID(),
                    PlayFabId = playerData.PlayFabId,
                }, result =>
                {
                    print("Position 2 char to tact");
                    PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
                    ItemInstanceId = item.GetInstanceID(),
                    PlayFabId = playerData.PlayFabId,
                    Data = DATA
                    }, result => {
                    print("Position 3 char to tact");

                    }, error =>{
                        Debug.Log(error.ErrorMessage);
                    });
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
        }
        #endif
        //Stash Calls
        void StashToTactInventory(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem itemTransferring = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().StashInventoryData){
                if(stashitem.Value.customID == item.customID){
                    itemTransferring = stashitem;
                    break;
                }
            }
            stash.GetStashRemoveItem(itemTransferring);
            itemTransferring.Value.TacticianStash = false;
            itemTransferring.Value.TacticianInventory = true;
            itemTransferring.Value.Changed = true;
            stash.GetTacticianNewItem(itemTransferring);
            #endif
        }
        void StashToTactBelt(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem itemTransferring = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().StashInventoryData){
                if(stashitem.Value.customID == item.customID){
                    itemTransferring = stashitem;
                    break;
                }
            }
            itemTransferring.Value.TacticianStash = false;
            itemTransferring.Value.TacticianBelt = true;
            itemTransferring.Value.Changed = true;
            stash.GetStashRemoveItem(itemTransferring);
            stash.GetTacticianNewItem(itemTransferring);
            #endif
        }
        void StashToTactEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string SlotName){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem itemTransferring = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().StashInventoryData){
                if(stashitem.Value.customID == item.customID){
                    itemTransferring = stashitem;
                    break;
                }
            }
            itemTransferring.Value.TacticianStash = false;
            itemTransferring.Value.TacticianEquip = true;
            itemTransferring.Value.EQUIPPEDSLOT = SlotName;
            itemTransferring.Value.Changed = true;
            stash.GetStashRemoveItem(itemTransferring);
            stash.GetTacticianNewItem(itemTransferring);
            #endif
        }
        void TactInvCharInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianInventory = false;
            stashItem.Value.INVENTORY = true;
            stashItem.Value.Changed = true;
            stash.GetCharacterNewItem(characterSlot, stashItem);
            #endif
        }
        void TactEquipCharInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianEquip = false;
            stashItem.Value.EQUIPPEDSLOT = "Unequipped";
            stashItem.Value.INVENTORY = true;
            stashItem.Value.Changed = true;
            int ItemFort = 0;
            int ItemArcana = 0;
            if(!string.IsNullOrEmpty(stashItem.Value.FORTITUDE_item)){
                if(int.Parse(stashItem.Value.FORTITUDE_item) > 0){
                    ItemFort = int.Parse(stashItem.Value.FORTITUDE_item);
                }
            }
            if(!string.IsNullOrEmpty(stashItem.Value.ARCANA_item)){
                if(int.Parse(stashItem.Value.ARCANA_item) > 0){
                    ItemArcana = int.Parse(stashItem.Value.ARCANA_item);
                }
            }
            int tacticianBonusFortitude = 0;
            int tacticianBonusArcana = 0;
            if(ItemFort > 0 || ItemArcana > 0 || ItemFort > 0 && ItemArcana > 0){
                TacticianFullDataMessage tacticianSheet = stash.GetTacticianSheet();
                tacticianBonusFortitude += int.Parse(tacticianSheet.FortitudeBonus);
                tacticianBonusArcana += int.Parse(tacticianSheet.ArcanaBonus);
                foreach(var tacticianEquipped in tacticianSheet.TacticianInventoryData){
                    if(tacticianEquipped.Value.Deleted){
                        continue;
                    }
                    if (tacticianEquipped.Value.GetTacticianEquip())
                    {
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFORTITUDE_item()))
                        {
                            tacticianBonusFortitude +=  int.Parse(tacticianEquipped.Value.GetFORTITUDE_item());
                        }
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetARCANA_item()))
                        {
                            tacticianBonusArcana += int.Parse(tacticianEquipped.Value.GetARCANA_item());
                        }
                    }
                }
                if(ItemFort > 0 || ItemArcana > 0){
                    foreach(var sheet in stash.GetInformationSheets()){
                        string _class = string.Empty;
                        int _level = 1;
                        string _core = string.Empty;
                        int curHP = 1;
                        int curMP = 1;
                        foreach(var stat in sheet.CharStatData){
                            if(stat.Key == "currentHP"){
                                curHP = int.Parse(stat.Value);
                            }
                            if(stat.Key == "currentMP"){
                                curMP = int.Parse(stat.Value);
                            }
                            if (stat.Key == "Class") {
                                _class = stat.Value;
                            }
                            if (stat.Key == "LVL") {
                                _level = int.Parse(stat.Value);
                            }
                            if (stat.Key == "CORE") {
                                _core = stat.Value;
                            }
                        }
                        (int baseStrength, int baseAgility, int baseFortitude, int baseArcana) = stash.GetCharacterStats(_class, _level, _core);
                        int maxHP = baseFortitude + tacticianBonusFortitude;
                        int maxMP = baseArcana + tacticianBonusArcana;
                        maxMP /= 7;
                        if(curHP > maxHP ){
                            curHP = maxHP;
                        }
                        if(curMP > maxMP){
                            curMP = maxMP;
                        }
                        CharacterStatListItem Health = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curHP.ToString()
                        });
                        CharacterStatListItem Mana = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curMP.ToString()
                        });
                        stash.GetCharacterUpdateHPMP(sheet.CharacterID, Health, Mana);
                    }
                }
            }
            stash.GetCharacterNewItem(characterSlot, stashItem);
            #endif
        }
        void TactBeltCharInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianBelt = false;
            stashItem.Value.INVENTORY = true;
            stashItem.Value.Changed = true;
            stash.GetCharacterNewItem(characterSlot, stashItem);
            #endif
        }
        void StashToCharInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().StashInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetStashRemoveItem(stashItem);
            stashItem.Value.TacticianStash = false;
            stashItem.Value.INVENTORY = true;
            stashItem.Value.Changed = true;
            stash.GetCharacterNewItem(characterSlot, stashItem);
            #endif
        }
        void StashToCharEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().StashInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetStashRemoveItem(stashItem);
            stashItem.Value.TacticianStash = false;
            stashItem.Value.EQUIPPED = true;
            stashItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            stashItem.Value.Changed = true;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, stashItem);
            #endif
        }
        //Tactician calls
        void TactInvToCharEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianInventory = false;
            stashItem.Value.EQUIPPED = true;
            stashItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            stashItem.Value.Changed = true;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, stashItem);
            #endif
        }
        void TactBeltToCharEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianInventory = false;
            stashItem.Value.TacticianBelt = false;
            stashItem.Value.EQUIPPED = true;
            stashItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            stashItem.Value.Changed = true;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, stashItem);
            #endif
        }
        void TactInventoryToStash(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianStash = true;
            stashItem.Value.Changed = true;
            stashItem.Value.TacticianInventory = false;
            stash.GetStashNewItem(stashItem);
            #endif
        }
        void TactBeltToStash(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianStash = true;
            stashItem.Value.TacticianBelt = false;
            stashItem.Value.Changed = true;
            stash.GetStashNewItem(stashItem);
            #endif
        }
        void TactEquipToStash(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianStash = true;
            stashItem.Value.Changed = true;
            stashItem.Value.TacticianEquip = false;
            stashItem.Value.EQUIPPEDSLOT = "Unequipped";
            int ItemFort = 0;
            int ItemArcana = 0;
            if(!string.IsNullOrEmpty(stashItem.Value.FORTITUDE_item)){
                if(int.Parse(stashItem.Value.FORTITUDE_item) > 0){
                    ItemFort = int.Parse(stashItem.Value.FORTITUDE_item);
                }
            }
            if(!string.IsNullOrEmpty(stashItem.Value.ARCANA_item)){
                if(int.Parse(stashItem.Value.ARCANA_item) > 0){
                    ItemArcana = int.Parse(stashItem.Value.ARCANA_item);
                }
            }
            int tacticianBonusFortitude = 0;
            int tacticianBonusArcana = 0;
            if(ItemFort > 0 || ItemArcana > 0 || ItemFort > 0 && ItemArcana > 0){
                TacticianFullDataMessage tacticianSheet = stash.GetTacticianSheet();
                tacticianBonusFortitude += int.Parse(tacticianSheet.FortitudeBonus);
                tacticianBonusArcana += int.Parse(tacticianSheet.ArcanaBonus);
                foreach(var tacticianEquipped in tacticianSheet.TacticianInventoryData){
                    if(tacticianEquipped.Value.Deleted){
                        continue;
                    }
                    if (tacticianEquipped.Value.GetTacticianEquip())
                    {
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFORTITUDE_item()))
                        {
                            tacticianBonusFortitude +=  int.Parse(tacticianEquipped.Value.GetFORTITUDE_item());
                        }
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetARCANA_item()))
                        {
                            tacticianBonusArcana += int.Parse(tacticianEquipped.Value.GetARCANA_item());
                        }
                    }
                }
                if(ItemFort > 0 || ItemArcana > 0){
                    foreach(var sheet in stash.GetInformationSheets()){
                        string _class = string.Empty;
                        int _level = 1;
                        string _core = string.Empty;
                        int curHP = 1;
                        int curMP = 1;
                        foreach(var stat in sheet.CharStatData){
                            if(stat.Key == "currentHP"){
                                curHP = int.Parse(stat.Value);
                            }
                            if(stat.Key == "currentMP"){
                                curMP = int.Parse(stat.Value);
                            }
                            if (stat.Key == "Class") {
                                _class = stat.Value;
                            }
                            if (stat.Key == "LVL") {
                                _level = int.Parse(stat.Value);
                            }
                            if (stat.Key == "CORE") {
                                _core = stat.Value;
                            }
                        }
                        (int baseStrength, int baseAgility, int baseFortitude, int baseArcana) = stash.GetCharacterStats(_class, _level, _core);
                        int maxHP = baseFortitude + tacticianBonusFortitude;
                        int maxMP = baseArcana + tacticianBonusArcana;
                        maxMP /= 7;
                        if(curHP > maxHP ){
                            curHP = maxHP;
                        }
                        if(curMP > maxMP){
                            curMP = maxMP;
                        }
                        CharacterStatListItem Health = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curHP.ToString()
                        });
                        CharacterStatListItem Mana = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curMP.ToString()
                        });
                        stash.GetCharacterUpdateHPMP(sheet.CharacterID, Health, Mana);
                    }
                }
            }
            stash.GetStashNewItem(stashItem);
            #endif
        }
        void TactInvToTactBelt(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianBelt = true;
            stashItem.Value.Changed = true;
            stashItem.Value.TacticianInventory = false;
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void TactInvToTactEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string SlotName){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianInventory = false;
            stashItem.Value.TacticianEquip = true;
            stashItem.Value.EQUIPPEDSLOT = SlotName;
            stashItem.Value.Changed = true;
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void TactEquipToTactInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianInventory = true;
            stashItem.Value.TacticianEquip = false;
            stashItem.Value.Changed = true;
            stashItem.Value.EQUIPPEDSLOT = "Unequipped";
            int ItemFort = 0;
            int ItemArcana = 0;
            if(!string.IsNullOrEmpty(stashItem.Value.FORTITUDE_item)){
                if(int.Parse(stashItem.Value.FORTITUDE_item) > 0){
                    ItemFort = int.Parse(stashItem.Value.FORTITUDE_item);
                }
            }
            if(!string.IsNullOrEmpty(stashItem.Value.ARCANA_item)){
                if(int.Parse(stashItem.Value.ARCANA_item) > 0){
                    ItemArcana = int.Parse(stashItem.Value.ARCANA_item);
                }
            }
            int tacticianBonusFortitude = 0;
            int tacticianBonusArcana = 0;
            if(ItemFort > 0 || ItemArcana > 0 || ItemFort > 0 && ItemArcana > 0){
                TacticianFullDataMessage tacticianSheet = stash.GetTacticianSheet();
                tacticianBonusFortitude += int.Parse(tacticianSheet.FortitudeBonus);
                tacticianBonusArcana += int.Parse(tacticianSheet.ArcanaBonus);
                foreach(var tacticianEquipped in tacticianSheet.TacticianInventoryData){
                    if(tacticianEquipped.Value.Deleted){
                        continue;
                    }
                    if (tacticianEquipped.Value.GetTacticianEquip())
                    {
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFORTITUDE_item()))
                        {
                            tacticianBonusFortitude +=  int.Parse(tacticianEquipped.Value.GetFORTITUDE_item());
                        }
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetARCANA_item()))
                        {
                            tacticianBonusArcana += int.Parse(tacticianEquipped.Value.GetARCANA_item());
                        }
                    }
                }
                if(ItemFort > 0 || ItemArcana > 0){
                    foreach(var sheet in stash.GetInformationSheets()){
                        string _class = string.Empty;
                        int _level = 1;
                        string _core = string.Empty;
                        int curHP = 1;
                        int curMP = 1;
                        foreach(var stat in sheet.CharStatData){
                            if(stat.Key == "currentHP"){
                                curHP = int.Parse(stat.Value);
                            }
                            if(stat.Key == "currentMP"){
                                curMP = int.Parse(stat.Value);
                            }
                            if (stat.Key == "Class") {
                                _class = stat.Value;
                            }
                            if (stat.Key == "LVL") {
                                _level = int.Parse(stat.Value);
                            }
                            if (stat.Key == "CORE") {
                                _core = stat.Value;
                            }
                        }
                        (int baseStrength, int baseAgility, int baseFortitude, int baseArcana) = stash.GetCharacterStats(_class, _level, _core);
                        int maxHP = baseFortitude + tacticianBonusFortitude;
                        int maxMP = baseArcana + tacticianBonusArcana;
                        maxMP /= 7;
                        if(curHP > maxHP ){
                            curHP = maxHP;
                        }
                        if(curMP > maxMP){
                            curMP = maxMP;
                        }
                        CharacterStatListItem Health = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curHP.ToString()
                        });
                        CharacterStatListItem Mana = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curMP.ToString()
                        });
                        stash.GetCharacterUpdateHPMP(sheet.CharacterID, Health, Mana);
                    }
                }
            }
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void TactEquipToTactEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string SlotName){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = (new CharacterInventoryListItem{
                Key = item.GetInstanceID(),
                Value = item
            });
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianEquip = true;
            stashItem.Value.EQUIPPEDSLOT = SlotName;
            stash.GetTacticianNewItem(stashItem);
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest {
                ItemInstanceId = item.GetInstanceID(),
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string> {
                    {"EquippedSlot", SlotName}
                }
            }, result => {
                //print("moved and doneskies");
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void TactEquipToTactBelt(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianEquip = false;
            stashItem.Value.EQUIPPEDSLOT = "Unequipped";
            stashItem.Value.TacticianBelt = true;
            stashItem.Value.Changed = true;
            int ItemFort = 0;
            int ItemArcana = 0;
            if(!string.IsNullOrEmpty(stashItem.Value.FORTITUDE_item)){
                if(int.Parse(stashItem.Value.FORTITUDE_item) > 0){
                    ItemFort = int.Parse(stashItem.Value.FORTITUDE_item);
                }
            }
            if(!string.IsNullOrEmpty(stashItem.Value.ARCANA_item)){
                if(int.Parse(stashItem.Value.ARCANA_item) > 0){
                    ItemArcana = int.Parse(stashItem.Value.ARCANA_item);
                }
            }
            int tacticianBonusFortitude = 0;
            int tacticianBonusArcana = 0;
            if(ItemFort > 0 || ItemArcana > 0 || ItemFort > 0 && ItemArcana > 0){
                TacticianFullDataMessage tacticianSheet = stash.GetTacticianSheet();
                tacticianBonusFortitude += int.Parse(tacticianSheet.FortitudeBonus);
                tacticianBonusArcana += int.Parse(tacticianSheet.ArcanaBonus);
                foreach(var tacticianEquipped in tacticianSheet.TacticianInventoryData){
                    if(tacticianEquipped.Value.Deleted){
                        continue;
                    }
                    if (tacticianEquipped.Value.GetTacticianEquip())
                    {
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFORTITUDE_item()))
                        {
                            tacticianBonusFortitude +=  int.Parse(tacticianEquipped.Value.GetFORTITUDE_item());
                        }
                        if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetARCANA_item()))
                        {
                            tacticianBonusArcana += int.Parse(tacticianEquipped.Value.GetARCANA_item());
                        }
                    }
                }
                if(ItemFort > 0 || ItemArcana > 0){
                    foreach(var sheet in stash.GetInformationSheets()){
                        string _class = string.Empty;
                        int _level = 1;
                        string _core = string.Empty;
                        int curHP = 1;
                        int curMP = 1;
                        foreach(var stat in sheet.CharStatData){
                            if(stat.Key == "currentHP"){
                                curHP = int.Parse(stat.Value);
                            }
                            if(stat.Key == "currentMP"){
                                curMP = int.Parse(stat.Value);
                            }
                            if (stat.Key == "Class") {
                                _class = stat.Value;
                            }
                            if (stat.Key == "LVL") {
                                _level = int.Parse(stat.Value);
                            }
                            if (stat.Key == "CORE") {
                                _core = stat.Value;
                            }
                        }
                        (int baseStrength, int baseAgility, int baseFortitude, int baseArcana) = stash.GetCharacterStats(_class, _level, _core);
                        int maxHP = baseFortitude + tacticianBonusFortitude;
                        int maxMP = baseArcana + tacticianBonusArcana;
                        maxMP /= 7;
                        if(curHP > maxHP ){
                            curHP = maxHP;
                        }
                        if(curMP > maxMP){
                            curMP = maxMP;
                        }
                        CharacterStatListItem Health = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curHP.ToString()
                        });
                        CharacterStatListItem Mana = (new CharacterStatListItem{
                            Key = "currentHP",
                            Value = curMP.ToString()
                        });
                        stash.GetCharacterUpdateHPMP(sheet.CharacterID, Health, Mana);
                    }
                }
            }
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void TactBeltToTactInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.TacticianInventory = true;
            stashItem.Value.TacticianBelt = false;
            stashItem.Value.Changed = true;
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void TactBeltToTactEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string SlotName){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            TacticianFullDataMessage tactSheet = stash.GetTacticianSheet();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var stashitem in stash.GetTacticianSheet().TacticianInventoryData){
                if(stashitem.Value.customID == item.customID){
                    stashItem = stashitem;
                    break;
                }
            }
            stash.GetTacticianRemoveItem(stashItem);
            stashItem.Value.EQUIPPED = true;
            stashItem.Value.EQUIPPEDSLOT = SlotName;
            stashItem.Value.TacticianBelt = false;
            stashItem.Value.Changed = true;
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        //Char calls 
        void CharInvTactInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == characterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            stashItem = invItem;
                            print($"Found our {stashItem.Value.Item_Name}!! its id is {stashItem.Value.customID} ");
                        }
                    }
                }
            }
            stash.RemoveCharacterItem(characterSlot, stashItem);
            stashItem.Value.INVENTORY = false;
            stashItem.Value.TacticianInventory = true;
            stashItem.Value.Changed = true;
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void CharInvTactEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            stashItem = invItem;
                            print($"Found our {stashItem.Value.Item_Name}!! its id is {stashItem.Value.customID} ");
                        }
                    }
                }
            }
            stash.RemoveCharacterItem(equippedData.CharacterSlot, stashItem);
            stashItem.Value.INVENTORY = false;
            stashItem.Value.TacticianEquip = true;
            stashItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            stashItem.Value.Changed = true;
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void CharInvTactBelt(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == characterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            stashItem = invItem;
                            print($"Found our {stashItem.Value.Item_Name}!! its id is {stashItem.Value.customID} ");
                        }
                    }
                }
            }
            stash.RemoveCharacterItem(characterSlot, stashItem);
            stashItem.Value.INVENTORY = false;
            stashItem.Value.TacticianBelt = true;
            stashItem.Value.Changed = true;
            stash.GetTacticianNewItem(stashItem);
            #endif
        }
        void CharInvStash(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem stashItem = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == characterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            stashItem = invItem;
                            print($"Found our {stashItem.Value.Item_Name}!! its id is {stashItem.Value.customID} ");
                        }
                    }
                }
            }
            stash.RemoveCharacterItem(characterSlot, stashItem);
            stashItem.Value.INVENTORY = false;
            stashItem.Value.TacticianStash = true;
            stashItem.Value.Changed = true;
            stash.GetStashNewItem(stashItem);
            #endif
        }
        void CharEquipStash(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int newHP = 0;
            int newMP = 0;
            bool currentValueChanged = false;
            string _class = string.Empty;
            string _core = string.Empty;
            int _level = 1;
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == characterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            sPlayer.RemoveCharacterItem(characterSlot, CharItem);
            CharItem.Value.EQUIPPED = false;
            CharItem.Value.TacticianStash = true;
            CharItem.Value.EQUIPPEDSLOT = "Unequipped";
            CharItem.Value.Changed = true;
            sPlayer.GetStashNewItem(CharItem);
            #endif
        }
        void CharEquipTactBelt(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int newHP = 0;
            int newMP = 0;
            bool currentValueChanged = false;
            string _class = string.Empty;
            string _core = string.Empty;
            int _level = 1;
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == characterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            sPlayer.RemoveCharacterItem(characterSlot, CharItem);
            CharItem.Value.EQUIPPED = false;
            CharItem.Value.TacticianBelt = true;
            CharItem.Value.EQUIPPEDSLOT = "Unequipped";
            CharItem.Value.Changed = true;
            sPlayer.GetTacticianNewItem(CharItem);
            #endif
        }
        void CharEquipTactInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, string characterSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int newHP = 0;
            int newMP = 0;
            bool currentValueChanged = false;
            string _class = string.Empty;
            string _core = string.Empty;
            int _level = 1;
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == characterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            sPlayer.RemoveCharacterItem(characterSlot, CharItem);
            CharItem.Value.EQUIPPED = false;
            CharItem.Value.TacticianInventory = true;
            CharItem.Value.EQUIPPEDSLOT = "Unequipped";
            CharItem.Value.Changed = true;
            sPlayer.GetTacticianNewItem(CharItem);
            #endif
        }
        //Character To Character
        void CharInvCharInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            //print("CharInvCharInv");
            string charOneID = equippedData.CharacterSlotOne;;
            string charTwoID = equippedData.CharacterSlotTwo;
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlotOne){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            CharItem.Value.Changed = true;
            stash.RemoveCharacterItem(charOneID, CharItem);
            stash.GetCharacterNewItem(charTwoID, CharItem);
            #endif                   
        }
        void CharEquipCharInv(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int newHP = 0;
            int newMP = 0;
            bool currentValueChanged = false;
            string _class = string.Empty;
            string _core = string.Empty;
            int _level = 1;
            string charOneID = equippedData.CharacterSlotOne;
            string charTwoID = equippedData.CharacterSlotTwo;
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlotOne){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            sPlayer.RemoveCharacterItem(charOneID, CharItem);
            CharItem.Value.INVENTORY = true;
            CharItem.Value.EQUIPPED = false;
            CharItem.Value.EQUIPPEDSLOT = "Unequipped";
            CharItem.Value.Changed = true;
            sPlayer.GetCharacterNewItem(charTwoID, CharItem);
            #endif   
        }
        void CharInvCharEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            string charOneID = equippedData.CharacterSlotOne;;
            string charTwoID = equippedData.CharacterSlotTwo;
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlotOne){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            sPlayer.RemoveCharacterItem(charOneID, CharItem);
            CharItem.Value.INVENTORY = false;
            CharItem.Value.EQUIPPED = true;
            CharItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            CharItem.Value.Changed = true;
            sPlayer.GetCharacterNewItem(charTwoID, CharItem);
            #endif       
        }
        void CharInvEquipSame(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            stash.RemoveCharacterItem(equippedData.CharacterSlot, CharItem);
            CharItem.Value.INVENTORY = false;
            CharItem.Value.EQUIPPED = true;
            CharItem.Value.Changed = true;
            CharItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, CharItem);
            /*
            PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
            {
                CharacterId = equippedData.CharacterSlot,
                ItemInstanceId = item.GetInstanceID(),
                PlayFabId = playerData.PlayFabId,
            }, result =>
            {
               PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
                {
                    ItemInstanceId = item.GetInstanceID(),
                    PlayFabId = playerData.PlayFabId,
                    Data = new Dictionary<string, string>
                        {
                            {"EquippedSlot", equippedData.Slot}
                        }
                }, result =>
                {
                    PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                    {
                        CharacterId = equippedData.CharacterSlot,
                        ItemInstanceId = item.GetInstanceID(),
                        PlayFabId = playerData.PlayFabId,
                    }, result =>
                    {
                        //print($"{item.GetItemName()} belongs to {item.GetOwnerID()}");
                    }, error =>{
                        Debug.Log(error.ErrorMessage);
                    });
                    //print("moved and doneskies");
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });         
            */
            #endif          
        }
        void SampleOfPFMoveKEEPTHIS(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
            {
                ItemInstanceId = item.GetInstanceID(),
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string>
                    {
                        {"TactStash", null}, {"TactInventory", "InventoryItem"}
                    }
            }, result =>
            {
                //print("moved and doneskies");
                 PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
                    {
                        CharacterId = item.GetOwnerID(),//charOneID,
                        ItemInstanceId = item.GetInstanceID(),
                        PlayFabId = playerData.PlayFabId,

                    }, result =>
                    {
                       PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
                        {
                            ItemInstanceId = item.GetInstanceID(),
                            PlayFabId = playerData.PlayFabId,
                            Data = new Dictionary<string, string>
                                {
                                    {"TactInventory", "InventoryItem"}
                                }
                        }, result =>
                        {
                            PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                            {
                                CharacterId = item.GetOwnerID(),
                                ItemInstanceId = item.GetInstanceID(),
                                PlayFabId = playerData.PlayFabId,
                            }, result =>
                            {
                                //print($"{item.GetItemName()} belongs to {item.GetOwnerID()}");
                            }, error =>{
                                Debug.Log(error.ErrorMessage);
                            });
                            //print("moved and doneskies");
                        }, error =>{
                            Debug.Log(error.ErrorMessage);
                        });
                    }, error =>{
                        Debug.Log(error.ErrorMessage);
                    });
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void CharEquipEquipSame(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            CharItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            CharItem.Value.Changed = true;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, CharItem);
            /*
            PlayFabServerAPI.MoveItemToUserFromCharacter( new MoveItemToUserFromCharacterRequest
            {
                CharacterId = equippedData.CharacterSlot,
                ItemInstanceId = item.GetInstanceID(),
                PlayFabId = playerData.PlayFabId,
            }, result =>
            {
               PlayFabServerAPI.UpdateUserInventoryItemCustomData( new UpdateUserInventoryItemDataRequest
                {
                    ItemInstanceId = item.GetInstanceID(),
                    PlayFabId = playerData.PlayFabId,
                    Data = new Dictionary<string, string>
                        {
                            {"EquippedSlot", equippedData.Slot}
                        }
                }, result =>
                {
                    PlayFabServerAPI.MoveItemToCharacterFromUser( new MoveItemToCharacterFromUserRequest
                    {
                        CharacterId = equippedData.CharacterSlot,
                        ItemInstanceId = item.GetInstanceID(),
                        PlayFabId = playerData.PlayFabId,
                    }, result =>
                    {
                        //print($"{item.GetItemName()} belongs to {item.GetOwnerID()}");
                    }, error =>{
                        Debug.Log(error.ErrorMessage);
                    });
                    //print("moved and doneskies");
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });    
            */
            #endif
        }
        void CharEquipCharEquip(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int newHP = 0;
            int newMP = 0;
            bool currentValueChanged = false;
            string _class = string.Empty;
            string _core = string.Empty;
            int _level = 1;
            string charOneID = equippedData.CharacterSlotOne;
            string charTwoID = equippedData.CharacterSlotTwo;
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlotOne){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            sPlayer.RemoveCharacterItem(charOneID, CharItem);
            CharItem.Value.EQUIPPEDSLOT = equippedData.Slot;
            CharItem.Value.Changed = true;
            sPlayer.GetCharacterNewItem(charTwoID, CharItem);
            
            #endif 
        }
        void CharEquipInvSame(NetworkConnectionToClient nconn, ItemSelectable item, PlayerInfo playerData, EquippingData equippedData){
            #if UNITY_SERVER || UNITY_EDITOR
            //print("Starting it up CharEquipInvSame");
            //print($"{equippedData.CharacterSlot} is the char equip data");
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int newHP = 0;
            int newMP = 0;
            bool currentValueChanged = false;
            string _class = string.Empty;
            string _core = string.Empty;
            int _level = 1;
            //bool CharFound = false;
            CharacterInventoryListItem CharItem = new CharacterInventoryListItem();
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == item.customID){
                            CharItem = invItem;
                            print($"Found our {CharItem.Value.Item_Name}!! its id is {CharItem.Value.customID} ");
                        }
                    }
                }
            }
            sPlayer.RemoveCharacterItem(equippedData.CharacterSlot, CharItem);
            CharItem.Value.EQUIPPEDSLOT = "Unequipped";
            CharItem.Value.EQUIPPED = false;
            CharItem.Value.INVENTORY = true;
            CharItem.Value.Changed = true;
            //print($"Part 3 equip to inv same, {CharItem.Value.EQUIPPEDSLOT} is equipped slot");
            sPlayer.GetCharacterNewItem(equippedData.CharacterSlot, CharItem);
            #endif
        }
        void CharUnequipCharEquipSame(NetworkConnectionToClient nconn, ItemSelectable itemOne, ItemSelectable itemTwo, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem CharItemOne = new CharacterInventoryListItem();
            CharacterInventoryListItem CharItemTwo = new CharacterInventoryListItem();
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == itemOne.customID){
                            CharItemOne = invItem;
                            print($"Found our item one !! its id is {CharItemOne.Value.customID}");
                        }
                        if(invItem.Value.customID == itemTwo.customID){
                            CharItemTwo = invItem;
                            print($"Found our item one !! its id is {CharItemTwo.Value.customID}");
                        }
                    }
                }
            }
            
            CharItemOne.Value.EQUIPPEDSLOT = "Unequipped";
            CharItemOne.Value.EQUIPPED = false;
            CharItemOne.Value.INVENTORY = true;
            CharItemOne.Value.Changed = true;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, CharItemOne);
            CharItemTwo.Value.EQUIPPEDSLOT = equippedData.Slot;
            CharItemTwo.Value.EQUIPPED = true;
            CharItemTwo.Value.Changed = true;
            CharItemTwo.Value.INVENTORY = false;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, CharItemTwo);
            #endif
        }
        void CharUnequipCharEquipSendTactStash(NetworkConnectionToClient nconn, string itemOne, string itemTwo, PlayerInfo playerData, EquippingData equippedData ){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer stash = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterInventoryListItem CharItemOne = new CharacterInventoryListItem();
            CharacterInventoryListItem CharItemTwo = new CharacterInventoryListItem();
            print($"CharUnequipCharEquipSendTactStash pos 1, IDs are item one {itemOne}, item two {itemTwo} {equippedData.CharacterSlot} is char slot, {equippedData.Slot} is the slot");
            if(equippedData.CharacterSlotOne == "Stash"){
                foreach(var invItem in stash.GetTacticianSheet().StashInventoryData){
                    if(invItem.Value.customID == itemTwo){
                        CharItemOne = invItem;
                        print($"Found our item two !! its id is {CharItemOne.Value.customID}");

                    }
                }
            }
            if(equippedData.CharacterSlotOne == "Tactician"){
                foreach(var invItem in stash.GetTacticianSheet().TacticianInventoryData){
                    if(invItem.Value.customID == itemTwo){
                        CharItemOne = invItem;
                        print($"Found our item two !! its id is {CharItemOne.Value.customID}");

                    }
                }
            }
            foreach(var sheet in stash.GetInformationSheets()){
                if(sheet.CharacterID == equippedData.CharacterSlot){
                    foreach(var invItem in sheet.CharInventoryData){
                        if(invItem.Value.customID == itemOne){
                            CharItemTwo = invItem;
                            print($"Found our item one !! its id is {CharItemTwo.Value.customID}");
                        }
                    }
                }
            }
            CharItemTwo.Value.TacticianStash = equippedData.Stash;
            CharItemTwo.Value.TacticianInventory = equippedData.TactInv;
            CharItemTwo.Value.TacticianBelt = equippedData.TactBelt;
            CharItemTwo.Value.EQUIPPEDSLOT = "Unequipped";
            CharItemTwo.Value.EQUIPPED = false;
            CharItemTwo.Value.INVENTORY = false;
            CharItemTwo.Value.Changed = true;
            print("CharUnequipCharEquipSendTactStash pos 2");

            stash.RemoveCharacterItem(equippedData.CharacterSlot, CharItemTwo);
            if(equippedData.CharacterSlotOne == "Stash"){
                stash.GetStashRemoveItem(CharItemOne);
                stash.GetStashNewItem(CharItemTwo);
            }
            if(equippedData.CharacterSlotOne == "Tactician"){
                stash.GetTacticianRemoveItem(CharItemOne);
                stash.GetTacticianNewItem(CharItemTwo);
            }
            print("CharUnequipCharEquipSendTactStash pos 3");

            CharItemOne.Value.EQUIPPEDSLOT = equippedData.Slot;
            CharItemOne.Value.EQUIPPED = true;
            CharItemOne.Value.Changed = true;
            CharItemOne.Value.INVENTORY = false;
            stash.GetCharacterNewItem(equippedData.CharacterSlot, CharItemOne);
            #endif
        }
        void CharacterDied(NetworkConnectionToClient nconn, string ID){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterStatListItem Death = (new CharacterStatListItem{
                Key = "DEATH",
                Value = DateTime.Now.ToString()
            });
            CharacterStatListItem EXPStat = (new CharacterStatListItem{
                Key = "EXP",
                Value = "0"
            });
            float EXP = 0f;
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == ID){
                    foreach(var stat in sheet.CharStatData){
                        if(stat.Key == "EXP"){
                            EXP = float.Parse(stat.Value);
                            break;
                        }
                    }
                }
            }
            // Reduce EXP by 10%
            EXP *= 0.9f;
            EXPStat.Value = EXP.ToString("F2");
            PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                CharacterId = ID,
                Data = new Dictionary<string, string>
                {
                    {"DEATH", Death.Value}, {"currentHP", "0"}, {"EXP", EXPStat.Value}, {"currentMP", "0"}
                }
            }, result =>
            {
                
                sPlayer.GetDEATHCHARACTER(ID, Death, EXPStat);
            }, error =>{
                Debug.Log(error.ErrorMessage); 
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void SaveGame(NetworkConnectionToClient nconn, CharacterSaveData savingData){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            print($"SavedGame is beginnning for this char ID {savingData.CharID}");
            PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                CharacterId = savingData.CharID,
                Data = new Dictionary<string, string>
                {
                    {"currentHP", savingData.CharHealth.ToString()}, {"currentMP", savingData.CharMana.ToString()}, {"EXP", savingData.CharExperience.ToString("F2")}, {"ClassPoints", savingData.CharClassPoints.ToString("F2")}
                }
            }, result =>
            {
                sPlayer.GetSavedGame(savingData);
                print($"SavedGame is ending for this char ID {savingData.CharID}");

            }, error =>{
                Debug.Log(error.ErrorMessage); 
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
        void ResCharacterServer(NetworkConnectionToClient nconn, string ID){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            int _lvl = 1;
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == ID){
                    foreach(var stat in sheet.CharStatData){
                        if(stat.Key == "LVL"){
                            _lvl = int.Parse(stat.Value);
                        }
                    }
                }
            }
            int amountDueNow = sPlayer.GetCharacterResCost(_lvl);
            if((long)amountDueNow <= sPlayer.Gold){
                sPlayer.Gold -= (long)amountDueNow;
                PlayFabServerAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    Amount = amountDueNow,
                    VirtualCurrency = "DK"
                }, result =>
                {
                        //stash.GoldAmountSet(nAmount);
                    sPlayer.TargetWalletAwake();
                    CharacterStatListItem Health = (new CharacterStatListItem{
                        Key = "currentHP",
                        Value = "1"
                    });
                    PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
                    {
                        PlayFabId = playerData.PlayFabId,
                        CharacterId = ID,
                        Data = new Dictionary<string, string>
                        {
                            {"currentHP", Health.Value}, {"DEATH", null}
                        }
                    }, result =>
                    {
                        sPlayer.ServerResurrectCharacter(ID, Health);
                        sPlayer.TargetWalletAwake();
                    }, error =>{
                        Debug.Log(error.ErrorMessage); 
                        Debug.Log(error.ErrorDetails);
                        Debug.Log(error.Error);
                    });
                    //print($"Client paid for res in the amount of DK coins:{amountDueNow}");
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            }
            #endif
            
        }
        void CharacterTakingDamage(NetworkConnectionToClient nconn, int newcurHP, string ID){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterStatListItem Health = (new CharacterStatListItem{
                Key = "currentHP",
                Value = newcurHP.ToString()
            });
            CharacterInventoryListItem damagedItem = (new CharacterInventoryListItem{
                Key = "LuckyRoll"
            });
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == ID){
                    System.Random rand = new System.Random();
                    int chanceRoll = rand.Next(1, 101); // Generates a random number between 1 and 100
                    if (chanceRoll <= 6){//6% chance
                        List<CharacterInventoryListItem> charInventoryData = sheet.CharInventoryData;
                        charInventoryData.RemoveAll(item => item.Value.NFT == true || item.Value.EQUIPPED == false);
                        if (charInventoryData.Count > 0) {
                            CharacterInventoryListItem randomItem = charInventoryData[rand.Next(0, charInventoryData.Count)];
                            // Assuming damagedItem is a variable you can modify, like a class or a struct
                            damagedItem.Key = randomItem.Key;
                            damagedItem.Value = randomItem.Value;
                            string dura = damagedItem.Value.Durability;
                            float durability;
                            if (!float.TryParse(dura, out durability))
                            {
                                durability = 100f;
                            }
                            durability--;
                            dura = durability.ToString(); // You should convert the updated float back to a string
                            damagedItem.Value.Durability = dura;
                            damagedItem.Value.Changed = true;
                            print($"{damagedItem.Key} took a durability loss of 1, its durability is now {damagedItem.Value.Durability}");
                            // Use randomItem as needed
                        }
                    }
                }
            }
            sPlayer.GetCharacterUpdateHPDurability(ID, Health, damagedItem);

            //PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
            //{
            //    PlayFabId = playerData.PlayFabId,
            //    CharacterId = ID,
            //    Data = new Dictionary<string, string>
            //    {
            //        {"currentHP", Health.Value}
            //    }
            //}, result =>
            //{
            //    sPlayer.ServerCombatHPUpdate(ID, Health);
            //}, error =>{
            //    Debug.Log(error.ErrorMessage); 
            //    Debug.Log(error.ErrorDetails);
            //    Debug.Log(error.Error);
            //});
            #endif
        }
        void CharacterHealed(NetworkConnectionToClient nconn, int newcurHP, string ID){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            print("CharacterHealed");

            CharacterStatListItem Health = (new CharacterStatListItem{
                Key = "currentHP",
                Value = newcurHP.ToString()
            });
            sPlayer.ServerCombatHPUpdate(ID, Health);

            //PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
            //{
            //    PlayFabId = playerData.PlayFabId,
            //    CharacterId = ID,
            //    Data = new Dictionary<string, string>
            //    {
            //        {"currentHP", Health.Value}
            //    }
            //}, result =>
            //{
            //    sPlayer.ServerCombatHPUpdate(ID, Health);
            //}, error =>{
            //    Debug.Log(error.ErrorMessage); 
            //    Debug.Log(error.ErrorDetails);
            //    Debug.Log(error.Error);
            //});
            #endif
        }
        void CharacterCastedSpell(NetworkConnectionToClient nconn, int newcurMP, string ID){
            //add spell cooldown here
            #if UNITY_SERVER || UNITY_EDITOR
            print("SpendingMP");
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            CharacterStatListItem Health = (new CharacterStatListItem{
                Key = "currentMP",
                Value = newcurMP.ToString()
            });
            sPlayer.ServerCombatHPUpdate(ID, Health);

            //PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
            //{
            //    PlayFabId = playerData.PlayFabId,
            //    CharacterId = ID,
            //    Data = new Dictionary<string, string>
            //    {
            //        {"currentMP", Health.Value}
            //    }
            //}, result =>
            //{
            //    sPlayer.ServerCombatHPUpdate(ID, Health);
            //}, error =>{
            //    Debug.Log(error.ErrorMessage); 
            //    Debug.Log(error.ErrorDetails);
            //    Debug.Log(error.Error);
            //});
            #endif
        }

        void ServerINNRoomRest(NetworkConnectionToClient nconn){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();

            if(sPlayer.Energy >= 200){
                sPlayer.Energy -= 200;
                PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
                PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    Data = new Dictionary<string, string>
                    {
                        {"energy", sPlayer.Energy.ToString()},
                    }

                }, result =>
                {
                    sPlayer.TargetUpdateEnergyDisplay(sPlayer.Energy);
                    StartCoroutine(HealPartyServer(nconn));
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
            }
            #endif
        }
        /*
        IEnumerator HealPartyServer(NetworkConnectionToClient nconn){
            ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sPlayer.GetParty().Contains(sheet.CharacterID))
                {
                    string _class = string.Empty;
                    int _level = 1;
                    string _core = string.Empty;
                    foreach(var stat in sheet.CharStatData){
                        if(stat.Key == "Class"){
                            _class = stat.Value;
                        }
                        if(stat.Key == "LVL"){
                            _level = int.Parse(stat.Value);
                        }
                        if(stat.Key == "CORE"){
                            _core = stat.Value;
                        }
                    }
                    int equipHP = 0;
                    int equipArcana = 0;
                    foreach(var charItem in sheet.CharInventoryData){
                        if(charItem.Value.EQUIPPED){
                            if(!string.IsNullOrEmpty(charItem.Value.FORTITUDE_item)){
                                if(int.Parse(charItem.Value.FORTITUDE_item) > 0){
                                    equipHP += int.Parse(charItem.Value.FORTITUDE_item);
                                }
                            }
                            if(!string.IsNullOrEmpty(charItem.Value.ARCANA_item)){
                                if(int.Parse(charItem.Value.ARCANA_item) > 0){
                                    equipArcana += int.Parse(charItem.Value.ARCANA_item);
                                }
                            }
                        }
                    }
                    (int baseStrength, int baseAgility, int baseFortitude, int baseArcana) = sPlayer.GetCharacterStats(_class, _level, _core);
                    int maxHP = equipHP + baseFortitude;
                    int maxMP = (equipArcana + baseArcana) / 7;
                    bool returned = false;
                    CharacterStatListItem Health = (new CharacterStatListItem{
                        Key = "currentHP",
                        Value = maxHP.ToString()
                    });
                    CharacterStatListItem Magic = (new CharacterStatListItem{
                        Key = "currentMP",
                        Value = maxMP.ToString()
                    });
                    sPlayer.GetINNServer(sheet.CharacterID, Health, Magic);
                    PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
                    {
                        PlayFabId = playerData.PlayFabId,
                        CharacterId = sheet.CharacterID,
                        Data = new Dictionary<string, string>
                        {
                            {"currentHP", Health.Value},
                            {"currentMP", Magic.Value}
                        }
                    }, result =>
                    {
                        returned = true;
                    }, error =>{
                        Debug.Log(error.ErrorMessage); 
                        Debug.Log(error.ErrorDetails);
                        Debug.Log(error.Error);
                    });
                    while(!returned){
                        yield return new WaitForSeconds(.1f);
                    }
                }
            }
        }
        */
            #if UNITY_SERVER || UNITY_EDITOR

        IEnumerator HealPartyServer(NetworkConnectionToClient nconn){
    ScenePlayer sPlayer = nconn.identity.gameObject.GetComponent<ScenePlayer>();
    int tactFort = int.Parse(sPlayer.GetTacticianSheet().FortitudeBonus);
    int tactArcana = int.Parse(sPlayer.GetTacticianSheet().ArcanaBonus);
    print($"{tactFort} is tact fort, {tactArcana} is tact arcana");
    PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
    
    var informationSheets = sPlayer.GetInformationSheets();
    for (int i = 0; i < informationSheets.Count; i++) {
        var sheet = informationSheets[i];
        if (sPlayer.GetParty().Contains(sheet.CharacterID)) {
            string _class = string.Empty;
            int _level = 1;
            string _core = string.Empty;
            bool Dead = false;
            var charStatDataList = sheet.CharStatData;
            for (int j = 0; j < charStatDataList.Count; j++) {
                var stat = charStatDataList[j];
                if (stat.Key == "Class") {
                    _class = stat.Value;
                }
                if (stat.Key == "LVL") {
                    _level = int.Parse(stat.Value);
                }
                if (stat.Key == "CORE") {
                    _core = stat.Value;
                }
                if(stat.Key == "DEATH"){
                    Dead = true;
                    ////print($"{MinHealth}");
                }
            }
            if(Dead){
                continue;
            }
            int equipHP = 0;
            int equipArcana = 0;
            equipHP += tactFort;
            equipArcana += tactArcana;
            var charInventoryDataList = sheet.CharInventoryData;
            for (int k = 0; k < charInventoryDataList.Count; k++) {
                var charItem = charInventoryDataList[k];
                if (charItem.Value.EQUIPPED) {
                    if (!string.IsNullOrEmpty(charItem.Value.FORTITUDE_item)) {
                        if (int.Parse(charItem.Value.FORTITUDE_item) > 0) {
                            equipHP += int.Parse(charItem.Value.FORTITUDE_item);
                        }
                    }
                    if (!string.IsNullOrEmpty(charItem.Value.ARCANA_item)) {
                        if (int.Parse(charItem.Value.ARCANA_item) > 0) {
                            equipArcana += int.Parse(charItem.Value.ARCANA_item);
                        }
                    }
                }
            }
            
            (int baseStrength, int baseAgility, int baseFortitude, int baseArcana) = sPlayer.GetCharacterStats(_class, _level, _core);
            int maxHP = equipHP + baseFortitude;
            int maxMP = (equipArcana + baseArcana) / 7;
            print($"{maxHP} is maxHP, {maxMP} is maxMP");
            bool returned = false;
            CharacterStatListItem Health = (new CharacterStatListItem {
                Key = "currentHP",
                Value = maxHP.ToString()
            });
            CharacterStatListItem Magic = (new CharacterStatListItem {
                Key = "currentMP",
                Value = maxMP.ToString()
            });
            sPlayer.GetINNServer(sheet.CharacterID, Health, Magic);
            //PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest {
            //    PlayFabId = playerData.PlayFabId,
            //    CharacterId = sheet.CharacterID,
            //    Data = new Dictionary<string, string> {
            //        {"currentHP", Health.Value},
            //        {"currentMP", Magic.Value}
            //    }
            //}, result => {
            //    returned = true;
            //}, error => {
            //    Debug.Log(error.ErrorMessage);
            //    Debug.Log(error.ErrorDetails);
            //    Debug.Log(error.Error);
            //});
            //while (!returned) {
                yield return new WaitForSeconds(.1f);
            //}
        }
    }
    sPlayer.TargetInnReset();
}
    #endif

        private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
        {
            Debug.LogFormat("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString());
            foreach (var conn in PlayFabServer.instance.playerConnections)
            {
                conn.conn.Send<MaintenanceMessage>(new MaintenanceMessage() {
                    ScheduledMaintenanceUTC = (DateTime)NextScheduledMaintenanceUtc
                });
            }
        }
        //SCENE MANAGEMENT FUNCTIONS /// OTHER FUNCTIONS
        readonly Dictionary<Match, Scene> MatchList = new Dictionary<Match, Scene>();
        readonly List<Scene> Scenes = new List<Scene>();
        [Header("MultiScene Setup")]
        [Scene]
        public string OVM;
        [Scene]
        public string TOWN;
        bool AzureLoaded;
        bool randomUniqueSceneLoaded;
        private Dictionary<string, SceneNode> sceneNodesDictionary;

        IEnumerator LoadSubScenes()
        {
            while (!AzureLoaded)
                yield return null;
            Debug.Log("Loading Scenes");
                GetAllScenes();
                
                // scene 0 = Container
                // scene 1 = OVM
                // scene 2 = TOWN
                // scene 3-x = MATCH

                yield return SceneManager.LoadSceneAsync(OVM, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                yield return SceneManager.LoadSceneAsync(TOWN, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
                GetAllScenes();

                SceneNode[] allNodes = FindObjectsOfType<SceneNode>();
                sceneNodesDictionary = new Dictionary<string, SceneNode>();
                

                foreach (SceneNode node in allNodes)
                {
                    sceneNodesDictionary.Add(node.nodeName, node);
                    //print($"Added {node.nodeName} to SceneNodesDictionary");
                }
        }
        private List<SpawnInfo> GetSpawnInfo(GameObject parent)
        {
            List<SpawnInfo> spawnInfoList = new List<SpawnInfo>();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject group = parent.transform.GetChild(i).gameObject;

                for (int j = 0; j < group.transform.childCount; j++)
                {
                    GameObject point = group.transform.GetChild(j).gameObject;
                    MobPrefabSettings mobSettings = point.GetComponent<MobPrefabSettings>();
                    spawnInfoList.Add(new SpawnInfo 
                    { 
                        MainChestLinked = mobSettings.LinkedToChest,
                        GroupName = group.name, 
                        PointName = point.name, 
                        SpawnTransform = point.transform 
                    });
                    if(mobSettings.LinkedToChest){
                        print($"{point.name} was the name of the prefab that is linked to the chest in group {group.name}");
                    }
                }
            }

            return spawnInfoList;
        }
        public List<Vector2> GeneratePoints(Vector3 spawnPoint, int numPoints)
        {
            float spacing = 1f;
            List<Vector2> points = new List<Vector2>();
            Vector2 startPos = spawnPoint;
            startPos.y -= 1; // move 1 unit down on the y-axis
            for (int i = 0; i < numPoints; i++)
            {
                points.Add(startPos);
                startPos.x += spacing;
            }
            return points;
        }
        /*
        public GameObject GetPrefabByName(string name)
        {
            if (prefabGroups.ContainsKey(name))
            {
                // If so, select a random prefab name from the group
                List<string> group = prefabGroups[name];
                name = group[UnityEngine.Random.Range(0, group.Count)];
            }
            GameObject foundPrefab = System.Array.Find(prefabs, prefab => prefab.name == name);
            if (foundPrefab == null)
            {
                Debug.LogWarning($"Prefab not found for name: {name}");
            }
            return foundPrefab;
        }
        */
        public GameObject GetPrefabByName(string name)
{
    if (prefabGroups.ContainsKey(name))
    {
        // If so, select a random prefab name from the group
        List<string> group = prefabGroups[name];
        name = group[UnityEngine.Random.Range(0, group.Count)];
    }
    GameObject foundPrefab = prefabs.Find(prefab => prefab.name == name);
    if (foundPrefab == null)
    {
        Debug.LogWarning($"Prefab not found for name: {name}");
    }
    return foundPrefab;
}
private void InstantiateDoors(GameObject doorsParent, Scene targetScene, Match match)
{
    // Loop over all direct children of the parent
    for (int i = 0; i < doorsParent.transform.childCount; i++)
    {
        GameObject doorGameObject = doorsParent.transform.GetChild(i).gameObject;
        // Get the name of the child, this should match the name of the prefab
        string prefabName = doorGameObject.name;
        DoorPrefabSettings doorSettings = doorGameObject.GetComponent<DoorPrefabSettings>();
        bool gold = true;
        bool silver = false;
        bool locked = true;
        if(doorSettings){
            gold = doorSettings.Gold;
            silver = doorSettings.Silver;
            locked = doorSettings.Locked;
        }
        // Get the correct prefab based on the name
        GameObject doorPrefab = GetPrefabByName(prefabName);
        // Check if a prefab was found
        if (doorPrefab != null){
            // Instantiate a new door at the same position and rotation as the old door
            GameObject newDoor = Instantiate(doorPrefab, doorGameObject.transform.position, doorGameObject.transform.rotation);
            NetworkServer.Spawn(newDoor);
            print($"Spawned door {newDoor.name}");
            // Move the new door to the target scene
            SceneManager.MoveGameObjectToScene(newDoor, targetScene);
            // Get the Door script of the new door
            Door doorScript = newDoor.GetComponent<Door>();
            // Check if the Door script is attached
            if (doorScript != null){
                // Set the match of the Door script
                doorScript.SetMatch(match, locked, gold, silver);
            }
            // Optionally, destroy the old door object
            Destroy(doorGameObject);
        } else {
            Debug.LogWarning($"Door prefab not found for name: {prefabName}");
        }
    }
}
private void InstantiateTraps(List<GameObject> traps, Scene targetScene, Match match)
{
    if(traps == null){
        print("Our list for this was null!");
        return;
    }
    // Loop over all direct children of the parent
    foreach (GameObject trapGameObject in traps)
    {
        // Get the name of the child, this should match the name of the prefab
        string prefabName = trapGameObject.name;
        TrapPrefabSettings trapSettings = trapGameObject.GetComponent<TrapPrefabSettings>();
        int tier = 1;
        bool hp = true;
        bool mp = false;
        bool debuff = false;
        if(trapSettings){
           (tier, hp, mp, debuff) = trapSettings.ReturnTierAndType();
        }
        if(tier <= 0){
            tier = 1;
        }
        if(!mp && !debuff){
            hp = true;
        }
        // Get the correct prefab based on the name
        GameObject trapPrefab = GetPrefabByName(prefabName);
        // Check if a prefab was found
        if (trapPrefab != null){
            // Instantiate a new door at the same position and rotation as the old door
            GameObject newTrap = Instantiate(trapPrefab, trapGameObject.transform.position, trapGameObject.transform.rotation);
            NetworkServer.Spawn(newTrap);
            print($"Spawned trap {newTrap.name}");
            // Move the new door to the target scene
            SceneManager.MoveGameObjectToScene(newTrap, targetScene);
            // Get the Door script of the new door
            TrapDrop trapScript = newTrap.GetComponent<TrapDrop>();
            // Check if the Trap script is attached
            if (trapScript != null){
                // Set the match of the Trap script
                trapScript.SetMatch(match, tier, hp, mp, debuff);
            }
            // Optionally, destroy the old door object
        } else {
            Debug.LogWarning($"Trap prefab not found for name: {prefabName}");
        }
    }
    for (int i = traps.Count - 1; i >= 0; i--){
        Destroy(traps[i]);
    }
}
private void InstantiateMiniChests(List<GameObject> miniChests, Scene targetScene, Match match)
{
    if(miniChests == null){
        print("Our list for this was null!");
        return;
    }
    foreach (GameObject miniChestGO in miniChests)
    {
        // Get the name of the child, this should match the name of the prefab
        string prefabName = "MiniChest";
        MiniChestPrefabSettings miniChest = miniChestGO.GetComponent<MiniChestPrefabSettings>();
        int tier = 1;
        if(miniChest){
            tier = miniChest.GetTier();
        }
        GameObject miniChestPrefab = GetPrefabByName(prefabName);
        // Check if a prefab was found
        if (miniChestPrefab != null){
            // Instantiate a new door at the same position and rotation as the old door
            GameObject newMiniChest = Instantiate(miniChestPrefab, miniChestGO.transform.position, miniChestGO.transform.rotation);
            NetworkServer.Spawn(newMiniChest);
            print($"Spawned MiniChest {newMiniChest.name}");
            // Move the new door to the target scene
            SceneManager.MoveGameObjectToScene(newMiniChest, targetScene);
            // Get the Door script of the new door
            MiniChest miniChestScript = newMiniChest.GetComponent<MiniChest>();
            // Check if the Trap script is attached
            if (miniChestScript != null){
                miniChestScript.SetMatch(match, tier);
                // Set the match of the Trap script
            }
            // Optionally, destroy the old door object
        } else {
            Debug.LogWarning($"MiniChest prefab not found for name: {prefabName}");
        }
    }
    for (int i = miniChests.Count - 1; i >= 0; i--){
        Destroy(miniChests[i]);
    }
}
private void InstantiateWeaponRacks(List<GameObject> weaponRacks, Scene targetScene, Match match)
{
    if(weaponRacks == null){
        print("Our list for this was null!");
        return;
    }
    // Loop over all direct children of the parent
    foreach (GameObject weaponRackGO in weaponRacks)
    {
        // Get the name of the child, this should match the name of the prefab
        string prefabName = "WeaponRack";
        WeaponRackPrefabSettings weaponRack = weaponRackGO.GetComponent<WeaponRackPrefabSettings>();
        int tier = 1;
        if(weaponRack){
            tier = weaponRack.GetTier();
        }
        GameObject weaponRackPrefab = GetPrefabByName(prefabName);
        // Check if a prefab was found
        if (weaponRackPrefab != null){
            // Instantiate a new door at the same position and rotation as the old door
            GameObject newWeaponRack = Instantiate(weaponRackPrefab, weaponRackGO.transform.position, weaponRackGO.transform.rotation);
            NetworkServer.Spawn(newWeaponRack);
            print($"Spawned weaponRack {newWeaponRack.name}");
            // Move the new door to the target scene
            SceneManager.MoveGameObjectToScene(newWeaponRack, targetScene);
            // Get the Door script of the new door
            WeaponDrop weaponRackScript = newWeaponRack.GetComponent<WeaponDrop>();
            // Check if the Trap script is attached
            if (weaponRackScript != null){
                weaponRackScript.SetMatch(match, tier);
                // Set the match of the Trap script
            }
            // Optionally, destroy the old door object
        } else {
            Debug.LogWarning($"weapon rack prefab not found for name: {prefabName}");
        }
    }
    for (int i = weaponRacks.Count - 1; i >= 0; i--){
        Destroy(weaponRacks[i]);
    }
}
private void InstantiateArmorRacks(List<GameObject> armorRacks, Scene targetScene, Match match)
{
    if(armorRacks == null){
        print("Our list for this was null!");
        return;
    }
    foreach (GameObject armorRackGO in armorRacks)
    {
        string prefabName = "ArmorRack";
        ArmorRackPrefabSettings armorSettings = armorRackGO.GetComponent<ArmorRackPrefabSettings>();
        int tier = 1;
        if(armorSettings)
        {
            tier = armorSettings.GetTier();
        }
        GameObject armorRackPrefab = GetPrefabByName(prefabName);

        if (armorRackPrefab != null)
        {
            GameObject newArmorRack = Instantiate(armorRackPrefab, armorRackGO.transform.position, armorRackGO.transform.rotation);
            NetworkServer.Spawn(newArmorRack);
            print($"Spawned armor rack {newArmorRack.name}");
            SceneManager.MoveGameObjectToScene(newArmorRack, targetScene);

            ArmorDrop armorRackScript = newArmorRack.GetComponent<ArmorDrop>();
            if (armorRackScript != null)
            {
                armorRackScript.SetMatch(match, tier);
            }
        }
        else
        {
            Debug.LogWarning($"Armor rack prefab not found for name: {prefabName}");
        }
    }
    for (int i = armorRacks.Count - 1; i >= 0; i--){
        Destroy(armorRacks[i]);
    }
}
private MainChest CreateMainChest(GameObject mainChestStatic, Scene targetScene, Match match)
{
    // Loop over all direct children of the parent
        MainChest matchMC = null;
        // Get the name of the child, this should match the name of the prefab
        MainChestPrefabSettings mcSettings = mainChestStatic.GetComponent<MainChestPrefabSettings>();
        int tier = 1;
        if(mcSettings){
            tier = mcSettings.GetTier();
        }
        // Get the correct prefab based on the name
        GameObject mainChestPrefab = GetPrefabByName("MainChest");
        // Check if a prefab was found
        if (mainChestPrefab != null){
            // Instantiate a new door at the same position and rotation as the old door
            GameObject newMainChest = Instantiate(mainChestPrefab, mainChestStatic.transform.position, mainChestStatic.transform.rotation);
            NetworkServer.Spawn(newMainChest);
            print($"Spawned door {newMainChest.name}");
            // Move the new door to the target scene
            SceneManager.MoveGameObjectToScene(newMainChest, targetScene);
            // Get the Door script of the new door
            MainChest mainChestScript = newMainChest.GetComponent<MainChest>();
            // Check if the Trap script is attached
            if (mainChestScript != null){
                // Set the match of the Trap script
                mainChestScript.SetMatch(match, tier);
                matchMC = mainChestScript;
            }
            // Optionally, destroy the old door object
           // Destroy(mainChestStatic);
        } else {
            Debug.LogWarning($"MainChest prefab not found for name: MainChest");
        }
            return matchMC;

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
       void SpawnPlayerCharactersTeam(ScenePlayer Host, Vector3 charStart, Match match, TurnManager curator, List<ScenePlayer> additonalPlayers, Scene scene){
        //we need to build the characters using adventureList, which has the scene player and int serial to find their characters
        // use player as the spawn point, then we can use each character from dictionary
        
        List<Vector2> Santaclawz = GeneratePoints(charStart, match.playerSlotPairs.Count);
        //spawn points are good now we just need to fix the logic below
        List<PlayerCharacter> PCs = new List<PlayerCharacter>();
        Dictionary<string, string> charStatsList;
        string selectionString = null;
        foreach(PlayerSlotPair pair in match.playerSlotPairs){
            int tacticianBonusArmor = 0;
            int tacticianBonusStrength = 0;
            int tacticianBonusFortitude = 0;
            int tacticianBonusAgility = 0;
            int tacticianBonusArcana = 0;
            int tacticianBonusMagicResist = 0;
            int tacticianBonusColdResist = 0;
            int tacticianBonusFireResist = 0;
            int tacticianBonusPoisonResist = 0;
            int tacticianBonusDiseaseResist = 0;

            TacticianFullDataMessage tacticianSheet = pair.player.GetTacticianSheet();
            tacticianBonusArmor += int.Parse(tacticianSheet.ArmorBonus);
            tacticianBonusStrength += int.Parse(tacticianSheet.StrengthBonus);
            tacticianBonusFortitude += int.Parse(tacticianSheet.FortitudeBonus);
            tacticianBonusAgility += int.Parse(tacticianSheet.AgilityBonus);
            tacticianBonusArcana += int.Parse(tacticianSheet.ArcanaBonus);
            foreach(var tacticianEquipped in tacticianSheet.TacticianInventoryData){
                if (tacticianEquipped.Value.GetTacticianEquip())
                {
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetArmor_item()))
                    {
                        tacticianBonusArmor += int.Parse(tacticianEquipped.Value.GetArmor_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetSTRENGTH_item()))
                    {
                        tacticianBonusStrength += int.Parse(tacticianEquipped.Value.GetSTRENGTH_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetAGILITY_item()))
                    {
                        tacticianBonusAgility += int.Parse(tacticianEquipped.Value.GetAGILITY_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFORTITUDE_item()))
                    {
                        tacticianBonusFortitude +=  int.Parse(tacticianEquipped.Value.GetFORTITUDE_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetARCANA_item()))
                    {
                        tacticianBonusArcana += int.Parse(tacticianEquipped.Value.GetARCANA_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetMagicResist_item()))
                    {
                        tacticianBonusMagicResist += int.Parse(tacticianEquipped.Value.GetMagicResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetColdResist_item()))
                    {
                        tacticianBonusColdResist += int.Parse(tacticianEquipped.Value.GetColdResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFireResist_item()))
                    {
                        tacticianBonusFireResist += int.Parse(tacticianEquipped.Value.GetFireResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetPoisonResist_item()))
                    {
                        tacticianBonusPoisonResist += int.Parse(tacticianEquipped.Value.GetPoisonResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetDiseaseResist_item()))
                    {
                        tacticianBonusDiseaseResist += int.Parse(tacticianEquipped.Value.GetDiseaseResist_item());
                    }
                }
            }
            ScenePlayer charKey = pair.player;
            string charValue = pair.slot;
            int index = UnityEngine.Random.Range(0, Santaclawz.Count);
            Vector2 CharPos = Santaclawz[index];
            Santaclawz.RemoveAt(index);
            //instantiate the players object and pass it the information to do what it needs to do
            GameObject character = null;
            charStatsList = GetInspectedCharacterList(charKey, charValue);
            foreach(var statKey in charStatsList){
                if(statKey.Key == "CharacterSprite"){
                    selectionString = statKey.Value;
                }
            }
            if(selectionString == "Player0_18"){
                character = Instantiate(Player18PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_37"){
                character = Instantiate(Player37PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_35"){
                character = Instantiate(Player35PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_7"){
                character = Instantiate(Player7PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_23"){
                character = Instantiate(Player23PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_17"){
                character = Instantiate(Player17PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_36"){
                character = Instantiate(Player36PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_33"){
                character = Instantiate(Player33PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_43"){
                character = Instantiate(Player43PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_26"){
                character = Instantiate(Player26PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_45"){
                character = Instantiate(Player45PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_6"){
                character = Instantiate(Player6PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_22"){
                character = Instantiate(Player22PrefabModel, CharPos, Quaternion.identity);//
            }
            //GameObject character 
            NetworkServer.Spawn(character.gameObject);
            // Give the player ownership of the character
            PlayerCharacter charScript = character.GetComponent<PlayerCharacter>();
            PCs.Add(charScript);
            Vector2 nully = new Vector2 ();
            charScript.SetUpCharacter(curator, CharPos, nully);
            charScript.armor += tacticianBonusArmor;
            charScript.strength += tacticianBonusStrength;
            charScript.agility += tacticianBonusAgility;
            
            charScript.fortitude += tacticianBonusFortitude;
            charScript.arcana += tacticianBonusArcana;
            charScript.MagicResist += tacticianBonusMagicResist;
            charScript.ColdResist += tacticianBonusColdResist;
            charScript.FireResist += tacticianBonusFireResist;
            charScript.PoisonResist += tacticianBonusPoisonResist;
            charScript.DiseaseResist += tacticianBonusDiseaseResist;
            int currentFortitude = charScript.fortitude;
            int previousFortitude = currentFortitude - tacticianBonusFortitude;
            int currentResistances = currentFortitude / 50;
            int previousResistances = previousFortitude / 50;
            if (currentResistances > previousResistances)
            {
                int bonusResistances = currentResistances - previousResistances;
                charScript.MagicResist += bonusResistances;
                charScript.ColdResist += bonusResistances;
                charScript.FireResist += bonusResistances;
                charScript.PoisonResist += bonusResistances;
                charScript.DiseaseResist += bonusResistances;
            }
            SceneManager.MoveGameObjectToScene(charScript.gameObject, scene);
            charScript.AssignedPayerAndMatch(charKey, match, charValue);
            NetworkIdentity identity = charScript.GetComponent<NetworkIdentity>();
            identity.AssignClientAuthority(charKey.connectionToClient);
            SendTurnManagerToPlayerCharacter(match, curator, charScript);
        }
        curator.SetPCList(PCs);
        
    }   
        void SpawnPlayerCharactersSolo(ScenePlayer player, Match match, TurnManager curator, Scene scene, Vector3 spawnPoint){
        //we need to build the characters using adventureList, which has the scene player and int serial to find their characters
        int tacticianBonusArmor = 0;
        int tacticianBonusStrength = 0;
        int tacticianBonusFortitude = 0;
        int tacticianBonusAgility = 0;
        int tacticianBonusArcana = 0;
        int tacticianBonusMagicResist = 0;
        int tacticianBonusColdResist = 0;
        int tacticianBonusFireResist = 0;
        int tacticianBonusPoisonResist = 0;
        int tacticianBonusDiseaseResist = 0;

         TacticianFullDataMessage tacticianSheet = player.GetTacticianSheet();
            tacticianBonusArmor += int.Parse(tacticianSheet.ArmorBonus);
            tacticianBonusStrength += int.Parse(tacticianSheet.StrengthBonus);
            tacticianBonusFortitude += int.Parse(tacticianSheet.FortitudeBonus);
            tacticianBonusAgility += int.Parse(tacticianSheet.AgilityBonus);
            tacticianBonusArcana += int.Parse(tacticianSheet.ArcanaBonus);

            foreach(var tacticianEquipped in tacticianSheet.TacticianInventoryData){
                if (tacticianEquipped.Value.GetTacticianEquip())
                {
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetArmor_item()))
                    {
                        tacticianBonusArmor += int.Parse(tacticianEquipped.Value.GetArmor_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetSTRENGTH_item()))
                    {
                        tacticianBonusStrength += int.Parse(tacticianEquipped.Value.GetSTRENGTH_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetAGILITY_item()))
                    {
                        tacticianBonusAgility += int.Parse(tacticianEquipped.Value.GetAGILITY_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFORTITUDE_item()))
                    {
                        tacticianBonusFortitude +=  int.Parse(tacticianEquipped.Value.GetFORTITUDE_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetARCANA_item()))
                    {
                        tacticianBonusArcana += int.Parse(tacticianEquipped.Value.GetARCANA_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetMagicResist_item()))
                    {
                        tacticianBonusMagicResist += int.Parse(tacticianEquipped.Value.GetMagicResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetColdResist_item()))
                    {
                        tacticianBonusColdResist += int.Parse(tacticianEquipped.Value.GetColdResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetFireResist_item()))
                    {
                        tacticianBonusFireResist += int.Parse(tacticianEquipped.Value.GetFireResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetPoisonResist_item()))
                    {
                        tacticianBonusPoisonResist += int.Parse(tacticianEquipped.Value.GetPoisonResist_item());
                    }
                    if (!string.IsNullOrEmpty(tacticianEquipped.Value.GetDiseaseResist_item()))
                    {
                        tacticianBonusDiseaseResist += int.Parse(tacticianEquipped.Value.GetDiseaseResist_item());
                    }
                }
            }
        // use player as the spawn point, then we can use each character from dictionary
        List<Vector2> charSpawnPoints = GeneratePoints(spawnPoint, player.GetParty().Count);
        //spawn points are good now we just need to fix the logic below
        List<PlayerCharacter> PCs = new List<PlayerCharacter>();
        string selectionString = null;
        foreach(var slot in player.GetParty()){
            selectionString = string.Empty;
            int index = UnityEngine.Random.Range(0, charSpawnPoints.Count);
            bool deadChar = false;
            //instantiate the players object and pass it the information to do what it needs to do
            GameObject character = null;
            foreach(var key in player.GetInformationSheets()){
                if(key.CharacterID == slot){
                    foreach(var KVP in key.CharStatData){
                        if(KVP.Key == "CharacterSprite"){
                            selectionString = KVP.Value;
                        }
                        if(KVP.Key == "DEATH"){
                            deadChar = true;
                        }
                    }
                    break;
                }
            }
            if(deadChar){
                continue;
            }
            Vector2 CharPos = charSpawnPoints[index];
            charSpawnPoints.RemoveAt(index);
            if(selectionString == "Player0_18"){
                character = Instantiate(Player18PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_37"){
                character = Instantiate(Player37PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_35"){
                character = Instantiate(Player35PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_7"){
                character = Instantiate(Player7PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_23"){
                character = Instantiate(Player23PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_17"){
                character = Instantiate(Player17PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_36"){
                character = Instantiate(Player36PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_33"){
                character = Instantiate(Player33PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_43"){
                character = Instantiate(Player43PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_26"){
                character = Instantiate(Player26PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_45"){
                character = Instantiate(Player45PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_6"){
                character = Instantiate(Player6PrefabModel, CharPos, Quaternion.identity);//
            }
            if(selectionString == "Player0_22"){
                character = Instantiate(Player22PrefabModel, CharPos, Quaternion.identity);//
            }
            //GameObject character 
            NetworkServer.Spawn(character.gameObject);
            // Give the player ownership of the character
            PlayerCharacter charScript = character.GetComponent<PlayerCharacter>();
            PCs.Add(charScript);
            Vector2 nully = new Vector2 ();
            charScript.SetUpCharacter(curator, CharPos, nully);
            charScript.armor += tacticianBonusArmor;
            charScript.strength += tacticianBonusStrength;
            charScript.agility += tacticianBonusAgility;
            charScript.fortitude += tacticianBonusFortitude;
            charScript.arcana += tacticianBonusArcana;
            charScript.MagicResist += tacticianBonusMagicResist;
            charScript.ColdResist += tacticianBonusColdResist;
            charScript.FireResist += tacticianBonusFireResist;
            charScript.PoisonResist += tacticianBonusPoisonResist;
            charScript.DiseaseResist += tacticianBonusDiseaseResist;
            int currentFortitude = charScript.fortitude;
            int previousFortitude = currentFortitude - tacticianBonusFortitude;
            int currentResistances = currentFortitude / 50;
            int previousResistances = previousFortitude / 50;
            if (currentResistances > previousResistances)
            {
                int bonusResistances = currentResistances - previousResistances;
                charScript.MagicResist += bonusResistances;
                charScript.ColdResist += bonusResistances;
                charScript.FireResist += bonusResistances;
                charScript.PoisonResist += bonusResistances;
                charScript.DiseaseResist += bonusResistances;
            }
            
            SceneManager.MoveGameObjectToScene(charScript.gameObject, scene);
            charScript.AssignedPayerAndMatch(player, player.currentMatch, slot);
            NetworkIdentity identity = charScript.GetComponent<NetworkIdentity>();
            identity.AssignClientAuthority(player.connectionToClient);
            SendTurnManagerToPlayerCharacter(match, curator, charScript);
        }
        curator.SetPCList(PCs);
        
    }   
        public void StartTheGame( int matchIndex, string nodeName, Match match, ScenePlayer host, string _matchID, List<ScenePlayer> playerScripts){
            StartCoroutine(CreateMatch(matchIndex, nodeName, match, host, playerScripts));
            // begin game is givng us the match number so we can pair it with the scene count and find which scene we are creating so we can reference 
            //for the players to join and then later remove it and fix any errors that appear
        }
        public void StartTheGameSolo(ScenePlayer host, int matchIndex, string nodeName, Match match){
            StartCoroutine(CreateMatchSolo(matchIndex, nodeName, match, host));
            // begin game is givng us the match number so we can pair it with the scene count and find which scene we are creating so we can reference 
            //for the players to join and then later remove it and fix any errors that appear
        }
        IEnumerator CreateMatchSolo(int matchIndex, string nodeName, Match match, ScenePlayer Host)
        {
            SceneNode matchNode = null;
            foreach(var key in sceneNodesDictionary){
                if(key.Key == Host.currentNode){
                    matchNode = key.Value;
                    break;
                }
            }
            yield return SceneManager.LoadSceneAsync(nodeName, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
            Debug.Log($"Spawned scene {SceneManager.GetSceneAt(matchIndex + 2).name}");
            MatchList.Add(match, SceneManager.GetSceneAt(matchIndex + 2));
            Scene scene = SceneManager.GetSceneAt(matchIndex + 2);
            //GameObject Evacuation = null;
            GameObject Wall = null;
            //GameObject Floor = null;
            GameObject MainChest = null;
            GameObject Doors = null;
            //GameObject PatrolPaths = null;
            GameObject SpawnPoints = null;
            GameObject CharacterSpawnPoint = null;

            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootObjects)
            {
                if(CharacterSpawnPoint == null){
                    CharacterSpawnPoint = FindGameObjectInChildren(rootObject, "CharacterSpawner");
                }
                if(Wall == null){
                    Wall = FindGameObjectInChildren(rootObject, "Walls");
                }
                if(MainChest == null){
                    MainChest = FindGameObjectInChildren(rootObject, "MainChest");
                }
                if(Doors == null){
                    Doors = FindGameObjectInChildren(rootObject, "Doors");
                }
                //if(PatrolPaths == null){
                //    PatrolPaths = FindGameObjectInChildren(rootObject, "PatrolPaths");
                //}
                if(SpawnPoints == null){
                    SpawnPoints = FindGameObjectInChildren(rootObject, "MobSpawnPoints");
                }
                //if (Floor && Wall && Evacuation && MainChest && Doors && SpawnPoints && CharacterSpawnPoint) // && PatrolPaths
                if (MainChest && SpawnPoints && Wall && CharacterSpawnPoint && Doors) // && PatrolPaths
                {
                    break;
                }
            }
            List<SpawnInfo> mobGroups = new List<SpawnInfo>();
            mobGroups = GetSpawnInfo(SpawnPoints);
            //Tilemap EvacPoints = Evacuation.GetComponent<Tilemap>();
            //Tilemap Floors = Floor.GetComponent<Tilemap>();
            Tilemap Walls = Wall.GetComponent<Tilemap>();
            // Get positions of other objects and their children
            Vector3 mainChestPosition = MainChest.transform.position;
            MainChest mainChest = null;
            DroppableManager dropManager = null;
            if(MainChest != null){
                mainChest = CreateMainChest(MainChest, scene, match);
                dropManager = MainChest.GetComponent<DroppableManager>();
                if(dropManager != null){
                    print("We had a dropManager so far so good");
                    (List<GameObject> armorRacks, List<GameObject> weaponRacks, List<GameObject> miniChests, List<GameObject> traps) = dropManager.GetPickedObjects();
                    foreach(var ar in armorRacks){
                        print($"{ar.gameObject.name} was in armor list");
                    }
                    foreach(var wr in weaponRacks){
                        print($"{wr.gameObject.name} was in weapon list");
                    }
                    foreach(var mc in miniChests){
                        print($"{mc.gameObject.name} was in mini chest list");
                    }
                    foreach(var trap in traps){
                        print($"{trap.gameObject.name} was in trap list");
                    }
                    InstantiateArmorRacks(armorRacks, scene, match);
                    InstantiateWeaponRacks(weaponRacks, scene, match);
                    InstantiateMiniChests(miniChests, scene, match);
                    InstantiateTraps(traps, scene, match);
                }
            }
            List<Vector3> doorPositions = GetChildPositions(Doors);
            Vector3 CharacterSpawnStart = CharacterSpawnPoint.transform.position;
            Dictionary<string, HashSet<Vector3>> patrolPathPositions = new Dictionary<string, HashSet<Vector3>>();
            //if(PatrolPaths != null){
            //    patrolPathPositions = GetNestedChildPositions(PatrolPaths).ToDictionary(kvp => kvp.Key, kvp => new HashSet<Vector3>(kvp.Value));
            //}
            yield return new WaitForSeconds(1f);  

            AuthorizeEnergyUpdate(Host, matchNode.EnergyEnterNodeCost);//needs foreach for multiplayer
            Host.TargetGetReadyForStart();//needs foreach for mutli
            GameObject newTurnManager = Instantiate (turnManagerPrefab);
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();          
            //turnManager.SetMatchSerial(match.matchSerial, match, Floors, Walls, EvacPoints, matchIndex + 2);
            turnManager.SetMatchSerial(match.matchSerial, match, Walls, matchIndex + 2, mainChest);

            turnManager.AddPlayer(Host);
            foreach(var tmPlayer in turnManager.GetPlayers()){
                tmPlayer.TargetWalls(turnManager.reservationWalls);
            }
            SceneManager.MoveGameObjectToScene(newTurnManager, scene);
            yield return new WaitForEndOfFrame();  
            Host.connectionToClient.Send(new SceneMessage { sceneName = OVM, sceneOperation = SceneOperation.UnloadAdditive});
            GameObject playerObject = Host.gameObject;
            int playerIndex = Host.playerIndex;
            float xOffset = playerIndex;
            Host.currentScene = nodeName;
            Host.currentNode = nodeName;
            foreach(var pMember in Host.currentMatch.playerSlotPairs){
                Host.AddMatchPartyListServer(pMember.slot);
            }
            SceneManager.MoveGameObjectToScene(Host.gameObject, scene);
            SpawnPlayerCharactersSolo(Host, match, turnManager, scene, CharacterSpawnStart);
            yield return new WaitForEndOfFrame();  
            List<Mob> mobs = new List<Mob>();
            List<Mob> mainChestMobs = new List<Mob>();
            foreach (var info in mobGroups)
            {
                // Get the correct prefab based on the enemy name saved in the SpawnInfo
                GameObject prefabToSpawn = GetPrefabByName(info.PointName);
                // Instantiate the prefab at the SpawnInfo's transform
                if (prefabToSpawn != null)
                {
                    Mob mob = null;
                    GameObject Mob = Instantiate(prefabToSpawn, info.SpawnTransform.position, Quaternion.identity);
                    NetworkServer.Spawn(Mob);
                    mob = Mob.GetComponent<Mob>();
                    //print($"{mob.cur_hp} is {mob.NAME}'s current HP");
                    mobs.Add(mob);
                    if(info.MainChestLinked){
                        mainChestMobs.Add(mob);
                    }
                    mob.groupNumber = info.GroupName;
                    mob.Origin = info.SpawnTransform.position;
                    mob.SetUpCharacter(turnManager, info.SpawnTransform.position, info.SpawnTransform.position);
                    mob.SetMATCH(match);
                    //foreach (var kvp in patrolPathPositions)
                    //{
                    //    if (kvp.Value.Contains(info.SpawnTransform.position))
                    //    {
                    //        Debug.Log("Path found for group " + kvp.Key);
                    //        mob.PatrolPath = kvp.Value.ToList();
                    //    }
                    //}
                    SceneManager.MoveGameObjectToScene(Mob, SceneManager.GetSceneAt(matchIndex + 2));
                    if (!turnManager.MobGroups.ContainsKey(info.GroupName))
                    {
                        // If the group doesn't exist, create a new list for this group
                        turnManager.MobGroups[info.GroupName] = new List<Mob>();
                    }
                    turnManager.MobGroups[info.GroupName].Add(mob);
                }
            }
            mainChest.FillOutMainChest(mainChestMobs, match);
            string tName = string.Empty;
            foreach(var stat in Host.GetTacticianSheet().TacticianStatData){
                if(stat.Key == "TacticianName"){
                    tName = stat.Value;
                    break;
                }
            }
            print($"Starting game for {tName}");
            Host.connectionToClient.Send(new SceneMessage { sceneName = nodeName, sceneOperation = SceneOperation.LoadAdditive , customHandling = true});
            turnManager.SetMobList(mobs);
            StartCoroutine(ToggleLoadBarOffWait(Host));
            yield return new WaitForSeconds(4f);
            List<PlayerCharacter> PCs = turnManager.GetPCList();
            foreach(var pc in PCs){
                pc.EnergySpark(pc, matchNode.GetVision());
            }
            List<Mob> Mobs = turnManager.GetENEMYList();
            foreach(var mob in Mobs){
                mob.EnergySpark(mob);
            }
            yield return new WaitForSeconds(1f);
            foreach(var player in turnManager.GetPlayers()){
                player.TargetSendMobList(Mobs);
            }
            if (Doors != null)
            {
                InstantiateDoors(Doors, scene, match);
            }
            //Host.RpcToggleSprite(false, Host.loadSprite);
            Host.TargetShowPartyCombatView();
        }
        
        IEnumerator CreateMatch(int matchIndex, string nodeName, Match match, ScenePlayer Host, List<ScenePlayer> playerScripts)
        {
            SceneNode matchNode = null;
            foreach(var key in sceneNodesDictionary){
                if(key.Key == Host.currentNode){
                    matchNode = key.Value;
                    break;
                }
            }
            yield return SceneManager.LoadSceneAsync(nodeName, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
            Debug.Log($"Spawned scene {SceneManager.GetSceneAt(matchIndex + 2).name}");
            MatchList.Add(match, SceneManager.GetSceneAt(matchIndex + 2));
            Scene scene = SceneManager.GetSceneAt(matchIndex + 2);
            //GameObject Evacuation = null;
            GameObject Wall = null;
            //GameObject Floor = null;
            GameObject MainChest = null;
            GameObject Doors = null;
            ////GameObject PatrolPaths = null;
            GameObject SpawnPoints = null;
            GameObject CharacterSpawnPoint = null;
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootObjects)
            {
                if(CharacterSpawnPoint == null){
                    CharacterSpawnPoint = FindGameObjectInChildren(rootObject, "CharacterSpawner");
                }
                if(Wall == null){
                    Wall = FindGameObjectInChildren(rootObject, "Walls");
                }
                if(MainChest == null){
                    MainChest = FindGameObjectInChildren(rootObject, "MainChest");
                }
                if(Doors == null){
                    Doors = FindGameObjectInChildren(rootObject, "Doors");
                }
                //if(PatrolPaths == null){
                //    PatrolPaths = FindGameObjectInChildren(rootObject, "PatrolPaths");
                //}
                if(SpawnPoints == null){
                    SpawnPoints = FindGameObjectInChildren(rootObject, "MobSpawnPoints");
                }
                //if (Floor && Wall && Evacuation && MainChest && Doors && Traps && SpawnPoints && CharacterSpawnPoint) // && PatrolPaths
                if (MainChest && SpawnPoints && CharacterSpawnPoint && Wall && Doors) // && PatrolPaths
                {
                    break;
                }
            }
            List<SpawnInfo> mobGroups = new List<SpawnInfo>();
            mobGroups = GetSpawnInfo(SpawnPoints);
            //Tilemap EvacPoints = Evacuation.GetComponent<Tilemap>();
            //Tilemap Floors = Floor.GetComponent<Tilemap>();
            Tilemap Walls = Wall.GetComponent<Tilemap>();
            // Get positions of other objects and their children
            Vector3 mainChestPosition = MainChest.transform.position;
            MainChest mainChest = null;
            DroppableManager dropManager = null;
            if(MainChest != null){
                mainChest = CreateMainChest(MainChest, scene, match);
                dropManager = MainChest.GetComponent<DroppableManager>();
                if(dropManager != null){
                    print("We had a dropManager so far so good");
                    (List<GameObject> armorRacks, List<GameObject> weaponRacks, List<GameObject> miniChests, List<GameObject> traps) = dropManager.GetPickedObjects();
                    foreach(var ar in armorRacks){
                        print($"{ar.gameObject.name} was in armor list");
                    }
                    foreach(var wr in weaponRacks){
                        print($"{wr.gameObject.name} was in weapon list");
                    }
                    foreach(var mc in miniChests){
                        print($"{mc.gameObject.name} was in mini chest list");
                    }
                    foreach(var trap in traps){
                        print($"{trap.gameObject.name} was in trap list");
                    }
                    InstantiateArmorRacks(armorRacks, scene, match);
                    InstantiateWeaponRacks(weaponRacks, scene, match);
                    InstantiateMiniChests(miniChests, scene, match);
                    InstantiateTraps(traps, scene, match);
                }
            }
            List<Vector3> doorPositions = GetChildPositions(Doors);
            Vector3 CharacterSpawnStart = CharacterSpawnPoint.transform.position;
            Dictionary<string, HashSet<Vector3>> patrolPathPositions = new Dictionary<string, HashSet<Vector3>>();
            //if(PatrolPaths != null){
            //    patrolPathPositions = GetNestedChildPositions(PatrolPaths).ToDictionary(kvp => kvp.Key, kvp => new HashSet<Vector3>(kvp.Value));
            //}
            yield return new WaitForSeconds(1f);  
            foreach(var tactician in playerScripts){
                AuthorizeEnergyUpdate(tactician, matchNode.EnergyEnterNodeCost);
                tactician.TargetGetReadyForStart();
                tactician.currentScene = nodeName;
                tactician.currentNode = nodeName;
            }
            foreach(var slot in Host.currentMatch.playerSlotPairs){
                foreach(var splay in playerScripts){
                    splay.AddMatchPartyListServer(slot.slot);
                }
            }
            GameObject newTurnManager = Instantiate (turnManagerPrefab);
            TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();          
            //turnManager.SetMatchSerial(match.matchSerial, match, Floors, Walls, EvacPoints, matchIndex + 2);
            turnManager.SetMatchSerial(match.matchSerial, match, Walls, matchIndex + 2, mainChest);

            turnManager.AddPlayers(playerScripts);
            foreach(var tmPlayer in turnManager.GetPlayers()){
                tmPlayer.TargetWalls(turnManager.reservationWalls);
            }
            SceneManager.MoveGameObjectToScene(newTurnManager, scene);
            yield return new WaitForEndOfFrame();  
            foreach(var tactician in playerScripts){
                tactician.connectionToClient.Send(new SceneMessage { sceneName = OVM, sceneOperation = SceneOperation.UnloadAdditive});
                SceneManager.MoveGameObjectToScene(tactician.gameObject, scene);
            }
            
            SpawnPlayerCharactersTeam(Host, CharacterSpawnStart,  match, turnManager, playerScripts, scene);
            yield return new WaitForEndOfFrame();  
            List<Mob> mobs = new List<Mob>();
            List<Mob> mainChestMobs = new List<Mob>();
            foreach (var info in mobGroups)
            {
                // Get the correct prefab based on the enemy name saved in the SpawnInfo
                GameObject prefabToSpawn = GetPrefabByName(info.PointName);
                // Instantiate the prefab at the SpawnInfo's transform
                if (prefabToSpawn != null)
                {
                    Mob mob = null;
                    GameObject Mob = Instantiate(prefabToSpawn, info.SpawnTransform.position, Quaternion.identity);
                    NetworkServer.Spawn(Mob);
                    mob = Mob.GetComponent<Mob>();
                    ////print($"{mob.cur_hp} is {mob.NAME}'s current HP");
                    mobs.Add(mob);
                    if(info.MainChestLinked){
                        mainChestMobs.Add(mob);
                    }
                    mob.groupNumber = info.GroupName;
                    mob.SetUpCharacter(turnManager, info.SpawnTransform.position, info.SpawnTransform.position);
                    mob.SetMATCH(match);
                    //foreach (var kvp in patrolPathPositions)
                    //{
                    //    if (kvp.Value.Contains(info.SpawnTransform.position))
                    //    {
                    //        Debug.Log("Path found for group " + kvp.Key);
                    //        mob.PatrolPath = kvp.Value.ToList();
                    //    }
                    //}
                    SceneManager.MoveGameObjectToScene(Mob, SceneManager.GetSceneAt(matchIndex + 2));
                    if (!turnManager.MobGroups.ContainsKey(info.GroupName))
                    {
                        // If the group doesn't exist, create a new list for this group
                        turnManager.MobGroups[info.GroupName] = new List<Mob>();
                    }
                    turnManager.MobGroups[info.GroupName].Add(mob);
                }
            }
            mainChest.FillOutMainChest(mainChestMobs, match);
            turnManager.SetMobList(mobs);
            foreach(var tactician in playerScripts){
                string tName = string.Empty;
                foreach(var stat in tactician.GetTacticianSheet().TacticianStatData){
                    if(stat.Key == "TacticianName"){
                        tName = stat.Value;
                        break;
                    }
                }
                print($"Starting team game for {tName}");
                tactician.connectionToClient.Send(new SceneMessage { sceneName = nodeName, sceneOperation = SceneOperation.LoadAdditive , customHandling = true});
                StartCoroutine(ToggleLoadBarOffWait(tactician));
            }
            yield return new WaitForSeconds(4f);
            List<PlayerCharacter> PCs = turnManager.GetPCList();
            foreach(var pc in PCs){
                pc.EnergySpark(pc, matchNode.GetVision());
            }
            List<Mob> Mobs = turnManager.GetENEMYList();
            foreach(var mob in Mobs){
                mob.EnergySpark(mob);
            }
            yield return new WaitForSeconds(1f);
            foreach(var player in turnManager.GetPlayers()){
                player.TargetSendMobList(Mobs);
            }
            foreach(var tactician in playerScripts){
                //tactician.RpcToggleSprite(false, tactician.loadSprite);
                tactician.TargetShowPartyCombatView();
            }
            if (Doors != null)
            {
                InstantiateDoors(Doors, scene, match);
            }
           
            //add in the floors walls evac point, main chest, min chest, armor droppable, weapon droppable, hp trapsgreen, mp traps blue, Utility traps orange, Doors, patrol paths
        }
        List<Vector3> GetChildPositions(GameObject parent)
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (Transform child in parent.transform)
            {
                positions.Add(child.position);
            }
            return positions;
        }
        Vector3 GetCharacterSpawnPosition(GameObject parent){
            Vector3 position = new Vector3();
            foreach (Transform child in parent.transform)
            {
                position = child.position;
            }
            return position;
        }
        
        Dictionary<string, HashSet<Vector3>> GetNestedChildPositions(GameObject parent)
        {
            Dictionary<string, HashSet<Vector3>> nestedPositions = new Dictionary<string, HashSet<Vector3>>();
            foreach (Transform child in parent.transform)
            {
                nestedPositions.Add(child.name, new HashSet<Vector3>(GetChildPositions(child.gameObject)));
            }
            return nestedPositions;
        }
         private GameObject FindGameObjectInChildren(GameObject parent, string name)
        {
            if (parent.name == name)
                return parent;
    
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                GameObject result = FindGameObjectInChildren(child, name);
                if (result != null)
                    return result;
            }
    
            return null;
        }
        public void SendPlayerToMatch(ScenePlayer player, string nodeName){
            Vector3 reposition = Vector3.zero;
            GameObject playerObject = player.gameObject;
            int playerIndex = player.playerIndex;
            float xOffset = playerIndex;
            player.currentScene = nodeName;
            player.currentNode = nodeName;
            if (player.currentNode == "Gates of Arudine")
            {
                reposition = new Vector3(19.5f + xOffset - 1, -2.5f, 0f);
            }
            else if (player.currentNode == "Spider Caverns")
            {
                reposition = new Vector3(19.5f + xOffset - 1, -2.5f, 0f);
            }
            else if (player.currentNode == "_FW1L1")
            {
                reposition = new Vector3(2.5f + xOffset - 1, -9.5f, 0f);
            }
            else if (player.currentNode == "_FS1L1")
            {
                reposition = new Vector3(10.5f + xOffset - 1, 24.5f, 0f);
            }
            playerObject.transform.position = reposition;
            //print($"{player.playerName} is being added to match: {player.currentMatch.matchSerial} which is at {nodeName}");
            StartCoroutine(SendingPlayers(player, nodeName));
        }
        void ChatManagerNodeOVMTransport(ChatManagerNode communicationNode)
        {
            SceneManager.MoveGameObjectToScene(communicationNode.gameObject, SceneManager.GetSceneByName("OVM"));
        }
        IEnumerator SendingPlayers(ScenePlayer player, string nodeName){
            yield return new WaitForEndOfFrame();
            //print($" {nodeName} is the scene  we are supposed to be going to");
            foreach(var key in MatchList){
                if(key.Key == player.currentMatch){
                    //print($" {nodeName} is the scene  we are supposed to be going to");
                    player.connectionToClient.Send(new SceneMessage { sceneName = OVM, sceneOperation = SceneOperation.UnloadAdditive});
                    SceneManager.MoveGameObjectToScene(player.gameObject, key.Value);
                    yield return new WaitForEndOfFrame();
                    player.connectionToClient.Send(new SceneMessage { sceneName = nodeName, sceneOperation = SceneOperation.LoadAdditive , customHandling = true});
                }
            }
            yield return new WaitForEndOfFrame();
            //player.RpcToggleSprite(false, player.loadSprite);
            player.TargetShowPartyCombatView();
            StartCoroutine(ToggleLoadBarOffWait(player));
            //player.TargetGatherSeekers();
        }
        void SendTurnManagerToPlayerCharacter(Match match, TurnManager turnManager, PlayerCharacter player){
            player.curatorTM = turnManager;
        }
        public void SendTurnManagerToMatch(TurnManager turnManager, int serial, Match match)
        {
            foreach(var key in MatchList){
                if(key.Key == match){
                    SceneManager.MoveGameObjectToScene(turnManager.gameObject, key.Value);
                    //turnManager.SetMatchSerial(serial, key.Key);
                }
            }
        }
        public void SendChatManagerNodeToMatch(ChatManagerNode chatManager, int serial, Match match)
        {
            //print($"SendingChatManagerNode to match: {serial} at {match.matchNode}");
            foreach(var key in MatchList){
                if(key.Key == match){
                    SceneManager.MoveGameObjectToScene(chatManager.gameObject, key.Value);
                }
            }
        }
        public void SendMobToMatch(Mob mob, Match currentMatch)
        {
            foreach(var key in MatchList){
                if(key.Key == currentMatch){
                    SceneManager.MoveGameObjectToScene(mob.gameObject, key.Value);
                    mob.SetMATCH(key.Key);
                }
            }
        }

        void GetAllScenes(){
            int countLoaded = SceneManager.sceneCount;
            Scene[] loadedScenes = new Scene[countLoaded];
 
            for (int i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }
            //print($"*******{countLoaded} is how many sceens are loaded on the server*******");
        }
        
        void UpdatePlayerOVMPosition(NetworkConnectionToClient nconn, string charliesTicket){
            #if UNITY_SERVER || UNITY_EDITOR
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            if(playerData.SessionTicket != charliesTicket)
            {
                //print($"{nconn.identity.gameObject} has been compromised, log information");
                return;
            }
            PlayFabServerAPI.AuthenticateSessionTicket(new AuthenticateSessionTicketRequest
            {
                SessionTicket = playerData.SessionTicket
            }, result =>
            {
                UpdatingPlayerOVM(nconn, playerData);
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
        void UpdatingPlayerOVM(NetworkConnectionToClient nconn, PlayerInfo playerData){
            #if UNITY_SERVER || UNITY_EDITOR
            ScenePlayer player = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            string saveNode = "TOWNOFARUDINE";
            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
                {
                    PlayFabId = playerData.PlayFabId,
                    Data = new Dictionary<string, string>
                    {
                        {"savedNode", saveNode},
                        {"LastScene", "OVM"}
                    }
                }, result =>
                {
                    player.TargetSendOVMRequest();
                    ClientRequestLoadScene dummy = new ClientRequestLoadScene {
                       oldScene = "TOWNOFARUDINE",
                       newScene = "OVM",
                       node = TOWNOFARUDINE,
                       login = false
                    };
                    GetCleanedSceneName(nconn, dummy);
                    //player.TargetUIOpenNodeOVM();
                }, error =>{
                    Debug.Log(error.ErrorMessage);
                });
                #endif
        }
        void GetCleanedSceneName(NetworkConnectionToClient conn, ClientRequestLoadScene msg){
            GameObject player = conn.identity.gameObject;
            //BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
            CircleCollider2D collider = player.GetComponent<CircleCollider2D>();
            NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
            ScenePlayer q = player.GetComponent<ScenePlayer>();
            if(msg.newScene != TOWNOFARUDINE){
                agent.enabled = false;
            }
            if(msg.newScene != "OVM" && msg.newScene != TOWNOFARUDINE){
                //q.RpcToggleSprite(false, q.loadSprite);
                if(collider){
                    collider.enabled = false;
                }
            } else {
                if(collider){
                    collider.enabled = true;
                }
            }
            string _sceneName = msg.newScene;
            q.justSpawned = true;
            if(msg.oldScene != "Container")
            {   
                conn.Send(new SceneMessage { sceneName = msg.oldScene, sceneOperation = SceneOperation.UnloadAdditive});
            }
            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, SceneManager.GetSceneByName(msg.newScene));
            q.currentScene = msg.newScene;
           
            if(msg.newScene == "OVM"){
                Vector2 nodeCoordinates = GetNodeCoordinates(msg.node);
                player.transform.position = nodeCoordinates;
                q.ServerAbletoClick();
                //q.RpcToggleSprite(false, q.loadSprite);
                q.TargetToggleSprite(true, q.loadSprite);
                q.ServerMapSelect(false);
                q.SetOurNode(sceneNodesDictionary, msg.node);
            }
            if(msg.newScene == TOWNOFARUDINE){
                //q.ServerMapSelect(true);
                //q.RpcToggleSprite(true, q.loadSprite);
                q.TargetToggleSprite(true, q.loadSprite);
                agent.enabled = true;
                //agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance; // No Avoidance
            }

            //if(q.currentScene == TOWNOFARUDINE && !msg.login){
            //    //q.TargetToggleSprite(false, q.loadSprite);
            //    q.TargetToggleSprite(true, q.loadSprite);
//
            //}
            conn.Send(new SceneMessage { sceneName = msg.newScene, sceneOperation = SceneOperation.LoadAdditive , customHandling = true});
           
            StartCoroutine(ToggleLoadBarOffWait(q));
        }
        public Vector2 GetNodeCoordinates(string nodeName)
        {
            float x = 5.99f;
            float y = -4.157f;

            switch (nodeName)
            {
                case "TOWNOFARUDINE":
                    x = 5.99f;
                    y = -4.157f;
                    break;
                case "Gates of Arudine":
                    x = 12.2f;
                    y = 1.93f;
                    break;
                case "Spider Caverns":
                    x = -1.82f;
                    y = 2.46f;
                    break;
                case "Lake Arudine":
                    x = 7.13f;
                    y = -6.88f;
                    break;
                default:
                    x = 5.99f;
                    y = -4.157f;
                    break;
            }

            return new Vector2(x, y);
        }
        IEnumerator ToggleLoadBarOffWait(ScenePlayer player){
            yield return new WaitForSeconds(5f);
            player.TargetToggleLoadBarOff();
        }
        void ClearMatch(string matchID, int matchSerial, Match match){
            if(!MatchList.ContainsKey(match)){
                return;
            }
            EndtheGame(matchID, matchSerial, match);
        }
        void EndtheGame(string matchID, int matchIndex, Match match){
            if(!MatchList.ContainsKey(match)){
                return;
            }
            StartCoroutine(EndMatch(matchID, matchIndex, match));
        }

        IEnumerator EndMatch(string matchID, int matchSerial, Match match){
            if(!MatchList.ContainsKey(match)){
                yield break;
            }
            foreach(var key in MatchList){
                if(key.Key == match){
                    yield return SceneManager.UnloadSceneAsync(key.Value);
                }
            }
            ENDMATCHFULLY.Invoke(match); 
            MatchList.Remove(match);

        }
        private HashSet<Match> processingMatches = new HashSet<Match>();

        void WipeoutMatch(Match match, List<ScenePlayer> players){
            if(!MatchList.ContainsKey(match)){
                return;
            }
            if (processingMatches.Contains(match))
            {
                return;
            }
            #if UNITY_SERVER || UNITY_EDITOR

            //Wiped(match);
            StartCoroutine(WipedOutCompletelyWalkOfShame( match, players));
            #endif

        }
        #if UNITY_SERVER || UNITY_EDITOR
       
    IEnumerator WipedOutCompletelyWalkOfShame(Match match, List<ScenePlayer> players){
        if(!MatchList.ContainsKey(match)){
            yield break;
        }
        // Check if the match is already being processed
        if (processingMatches.Contains(match))
        {
            yield break;
        }
            
        // Add the match to the set of processing matches
        processingMatches.Add(match);
        //print("Got to finished match in playfab server");
        string saveNode = "TOWNOFARUDINE";
        foreach(var player in players){
            player.inMatch = false;   
            player.currentNode = "TOWNOFARUDINE";
            NetworkIdentity networkIdentity = player.GetComponent<NetworkIdentity>();
            NetworkConnectionToClient nconn = networkIdentity.connectionToClient;
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            bool leftMatch = false;
            PlayFabServerAPI.UpdateUserInternalData(new UpdateUserInternalDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                Data = new Dictionary<string, string>
                {
                    {"savedNode", saveNode},
                    {"LastScene", saveNode}
                }
            }, result =>
            {
                    
                player.TargetCloseEndGameCanvas();
                if(nconn != null){
                    //print("We got the connection properly");
                }
                ClientRequestLoadScene dummy = new ClientRequestLoadScene {
                    oldScene = player.currentScene,
                    newScene = "TOWNOFARUDINE",
                    node = player.currentNode,
                    login = false
                };
                player.currentScene = "TOWNOFARUDINE";
                GetCleanedSceneName(nconn, dummy);
                leftMatch = true;
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            yield return new WaitUntil(() => leftMatch);
        }
        foreach(var key in MatchList){
            if(key.Key == match){
                yield return SceneManager.UnloadSceneAsync(key.Value);
            }
        }
        MatchList.Remove(match);
        processingMatches.Remove(match);
        ENDMATCHFULLY.Invoke(match); 
        MatchMaker.instance.FinishedMatch(match, players);
    }
        #endif

    void FinishingMatch(Match match){
        if(!MatchList.ContainsKey(match)){
            return;
        }
        if (processingMatches.Contains(match))
        {
            return;
        }
        FinishedTheGame(match);
    }
        void FinishedTheGame(Match match){
            if(!MatchList.ContainsKey(match)){
                return;
            }
            if (processingMatches.Contains(match))
            {
                return;
            }
            StartCoroutine(FinishedMatch( match));
        }
        IEnumerator FinishedMatch(Match match){
            ENDMATCHMAKER.Invoke(match);
            if(!MatchList.ContainsKey(match)){
                yield break;
            }
            // Check if the match is already being processed
            if (processingMatches.Contains(match))
            {
                yield break;
            }
            // Add the match to the set of processing matches
            processingMatches.Add(match);
            List<ScenePlayer> players = match.players;
            //print("Got to finished match in playfab server");
            foreach(var player in match.players){
                player.inMatch = false;
                player.TargetCloseEndGameCanvas();
                NetworkIdentity networkIdentity = player.GetComponent<NetworkIdentity>();
                NetworkConnectionToClient conn = networkIdentity.connectionToClient;
                if(conn != null){
                    //print("We got the connection properly");
                }
                ClientRequestLoadScene dummy = new ClientRequestLoadScene {
                        oldScene = player.currentScene,
                        newScene = "OVM",
                        node = player.currentNode,
                        login = false
                    };
                GetCleanedSceneName(conn, dummy);
            }
            foreach(var key in MatchList){
                if(key.Key == match){
                    yield return SceneManager.UnloadSceneAsync(key.Value);
                }
            }
            ENDMATCHFULLY.Invoke(match); 
            MatchList.Remove(match);
            processingMatches.Remove(match);
            MatchMaker.instance.FinishedMatch(match, players);
        }
        
        /*
                        if(result.Data.ContainsKey("WestT1Skill")){
                            p.CharacterElevenList.Add("WestT1Skill", result.Data["WestT1Skill"].Value);
                        }
                        if(result.Data.ContainsKey("WestT2TopSkill")){
                            p.CharacterElevenList.Add("WestT2TopSkill", result.Data["WestT2TopSkill"].Value);
                        }
                        if(result.Data.ContainsKey("WestT2MiddleSkill")){
                            p.CharacterElevenList.Add("WestT2MiddleSkill", result.Data["WestT2MiddleSkill"].Value);
                        }
                        if(result.Data.ContainsKey("WestT2BottomSkill")){
                            p.CharacterElevenList.Add("WestT2BottomSkill", result.Data["WestT2BottomSkill"].Value);
                        }
                        if(result.Data.ContainsKey("WestT3TopSkill")){
                            p.CharacterElevenList.Add("WestT3TopSkill", result.Data["WestT3TopSkill"].Value);
                        }
                        if(result.Data.ContainsKey("WestT3BottomSkill")){
                            p.CharacterElevenList.Add("WestT3BottomSkill", result.Data["WestT3BottomSkill"].Value);
                        }
                        if(result.Data.ContainsKey("WestT3EndSkill")){
                            p.CharacterElevenList.Add("WestT3EndSkill", result.Data["WestT3EndSkill"].Value);
                        }
                        if(result.Data.ContainsKey("EastT1Skill")){
                            p.CharacterElevenList.Add("EastT1Skill", result.Data["EastT1Skill"].Value);
                        }
                        if(result.Data.ContainsKey("EastT2TopSkill")){
                            p.CharacterElevenList.Add("EastT2TopSkill", result.Data["EastT2TopSkill"].Value);
                        }
                        if(result.Data.ContainsKey("EastT2MiddleSkill")){
                            p.CharacterElevenList.Add("EastT2MiddleSkill", result.Data["EastT2MiddleSkill"].Value);
                        }
                        if(result.Data.ContainsKey("EastT2BottomSkill")){
                            p.CharacterElevenList.Add("EastT2BottomSkill", result.Data["EastT2BottomSkill"].Value);
                        }
                        if(result.Data.ContainsKey("EastT3TopSkill")){
                            p.CharacterElevenList.Add("EastT3TopSkill", result.Data["EastT3TopSkill"].Value);
                        }
                        if(result.Data.ContainsKey("EastT3BottomSkill")){
                            p.CharacterElevenList.Add("EastT3BottomSkill", result.Data["EastT3BottomSkill"].Value);
                        }
                        if(result.Data.ContainsKey("EastT3EndSkill")){
                            p.CharacterElevenList.Add("EastT3EndSkill", result.Data["EastT3EndSkill"].Value);
                        }
                        if(result.Data.ContainsKey("SouthT1Skill")){
                            p.CharacterElevenList.Add("SouthT1Skill", result.Data["SouthT1Skill"].Value);
                        }
                        if(result.Data.ContainsKey("SouthT2LeftSkill")){
                            p.CharacterElevenList.Add("SouthT2LeftSkill", result.Data["SouthT2LeftSkill"].Value);
                        }
                        if(result.Data.ContainsKey("SouthT2MiddleSkill")){
                            p.CharacterElevenList.Add("SouthT2MiddleSkill", result.Data["SouthT2MiddleSkill"].Value);
                        }
                        if(result.Data.ContainsKey("SouthT2RightSkill")){
                            p.CharacterElevenList.Add("SouthT2RightSkill", result.Data["SouthT2RightSkill"].Value);
                        }
                        if(result.Data.ContainsKey("SouthT3LeftSkill")){
                            p.CharacterElevenList.Add("SouthT3LeftSkill", result.Data["SouthT3LeftSkill"].Value);
                        }
                        if(result.Data.ContainsKey("SouthT3RightSkill")){
                            p.CharacterElevenList.Add("SouthT3RightSkill", result.Data["SouthT3RightSkill"].Value);
                        }
                        if(result.Data.ContainsKey("SouthT3EndSkill")){
                            p.CharacterElevenList.Add("SouthT3EndSkill", result.Data["SouthT3EndSkill"].Value);
                        }
                        */
        string AppendNumber(string str, int num)
        {
            return str + " " + num.ToString();
        }
        int CalculateSpellCost(string type, int spellRank)
        {
            int rankCost;
            int rankUpCost;
            
            switch (type)
            {
                case "Starter":
                    return 25; // If type is Starter, return 25 immediately
                case "A":
                    rankCost = 15; // Rank cost for type A
                    rankUpCost = 30; // RankUpCost for type A
                    break;
                case "B":
                    rankCost = 55; // Rank cost for type B
                    rankUpCost = 110; // RankUpCost for type B
                    break;
                case "C":
                    rankCost = 200; // Rank cost for type C
                    rankUpCost = 500; // RankUpCost for type C
                    break;
                case "D":
                    rankCost = 600; // Rank cost for type D
                    rankUpCost = 1500; // RankUpCost for type D
                    break;
                case "E":
                    rankCost = 200; // Rank cost for type E
                    rankUpCost = 500; // RankUpCost for type E
                    break;
                default:
                    Debug.LogWarning("Unknown type: " + type);
                    return 0; // Default value if the type does not match any known values
            }
            // Calculate and return the spell cost using the given formula
            return (rankCost * (spellRank - 1)) + rankUpCost;
        }
        void PlayerRequestedLearnSpell(NetworkConnectionToClient nconn, LearnSpell requestedSpell, string CharID){
            #if UNITY_SERVER || UNITY_EDITOR
            //we need to make it so we can find what spell is coming in where it goes and charge the proper class points if they have enough
            string spell = requestedSpell.SpellName;
            string type = requestedSpell.SpellType;
            int currentSpellrank = requestedSpell.CurrentSpellRank;
            int requestedSpellrank = requestedSpell.RequestedSpellRank;

            float classPoints = -1;
            string upgradedSpellSlot = null;
            string upgradedSpell = null;
            int upgradeCost = CalculateSpellCost(requestedSpell.SpellType, requestedSpell.RequestedSpellRank);
            print($"learning {spell} spell and its of type {type} and its requested rank is {requestedSpell.RequestedSpellRank} and current rank is {requestedSpell.CurrentSpellRank}, the cost is going to be {upgradeCost}");

            ScenePlayer player = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            print("Made it passed calculate spell");
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            //Class Unlocks
            //(rankCost * spellRank) + spellDetails.RankUpCost
            if(spell == "Archer"){
                upgradedSpellSlot = "StartClassSkill";
            }
            if(spell == "Enchanter"){
                upgradedSpellSlot = "StartClassSkill";
            }
            if(spell == "Fighter"){
                upgradedSpellSlot = "StartClassSkill";
            }
            if(spell == "Rogue"){
                upgradedSpellSlot = "StartClassSkill";
            }
            if(spell == "Priest"){
                upgradedSpellSlot = "StartClassSkill";
            }
            if(spell == "Wizard"){
                upgradedSpellSlot = "StartClassSkill";
            }
            if(spell == "Paladin"){
                upgradedSpellSlot = "StartClassSkill";
            }
            if(spell == "Druid"){
                upgradedSpellSlot = "StartClassSkill";
            }
            //Archer
            if(spell == "Aimed Shot"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Head Shot"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Silence Shot"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Crippling Shot"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Fire Arrow"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Penetrating Shot"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Double Shot"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Bandage Wound"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Dash"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Identify Enemy"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Track"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Perception"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Sleep"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Nature's Precision"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //Enchanter
            if(spell == "Mesmerize"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Slow"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Magic Sieve"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Aneurysm"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Gravity Stun"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Weaken"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Charm"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Haste"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Root"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Invisibility"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Rune"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Purge"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Resist Magic"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Mp Transfer"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //Fighter
            if(spell == "Charge"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Heavy Swing"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Throw Stone"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Knockback"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Offensive Stance"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Critical Strike"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Double Attack"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Bash"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Taunt"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Protect"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Intimidating Roar"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Tank Stance"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Block"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Riposite"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //Priest
            if(spell == "Holy Bolt"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Turn Undead"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Critical Heal"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Undead Protection"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Smite"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Shield Bash"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Regeneration"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Heal"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Cure Poison"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Dispel"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Fortitude"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Greater Heal"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Group Heal"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Resurrect"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //Rogue
            if(spell == "Shuriken"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Tendon Slice"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Backstab"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Rush"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Blind"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Poison"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Double Attack"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Hide"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Picklock"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Steal"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Detect Traps"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Treasure Finding"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Ambidexterity"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Sneak"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //Wizard
            //West
            if(spell == "Fire"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Fireball"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Light"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Magic Missile"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Teleport"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Meteor Shower"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Incinerate"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Ice"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Spell Critical"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Ice Block"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Ice Blast"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Blizzard"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Magic Burst"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Brain Freeze"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //Paladin
            //West
            if(spell == "Holy Swing"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Undead Slayer"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Stun"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Celestial Wave"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Divine Wrath"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Shackle"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Double Attack"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Divine Armor"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Cleanse"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Angelic Shield"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Flash Of Light"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Cover"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Consecrated Ground"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Lay On Hands"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //Druid
            //West
            if(spell == "Swarm Of Insects"){
                upgradedSpellSlot = "WestT1Skill";
            }
            if (spell == "Snare"){
                upgradedSpellSlot = "WestT2TopSkill";
            }
            if (spell == "Shapeshift"){
                upgradedSpellSlot = "WestT2MiddleSpell";
            }
            if (spell == "Engulfing Roots"){
                upgradedSpellSlot = "WestT2BottomSpell";
            }
            if (spell == "Staff Specialization"){
                upgradedSpellSlot = "WestT3TopSkill";
            }
            if (spell == "Chain Lightning"){
                upgradedSpellSlot = "WestT3BottomSpell";
            }
            if (spell == "Solar Flare"){
                upgradedSpellSlot = "WestT3EndSpell";
            }
            //East
            if(spell == "Rejuvination"){
                upgradedSpellSlot = "EastT1Skill";
            }
            if (spell == "Thorns"){
                upgradedSpellSlot = "EastT2TopSkill";
            }
            if (spell == "Nature's Protection"){
                upgradedSpellSlot = "EastT2MiddleSpell";
            }
            if (spell == "Strength"){
                upgradedSpellSlot = "EastT2BottomSpell";
            }
            if (spell == "Tornado"){
                upgradedSpellSlot = "EastT3TopSkill";
            }
            if (spell == "Greater Rejuvination"){
                upgradedSpellSlot = "EastT3BottomSpell";
            }
            if (spell == "Evacuate"){
                upgradedSpellSlot = "EastT3EndSpell";
            }
            //All South
            if(spell == "Bonus Harvest"){
                upgradedSpellSlot = "SouthT1Skill";
            }
            if (spell == "Bonus Strength"){
                upgradedSpellSlot = "SouthT2LeftSkill";
            }
            if (spell == "Bonus Agility"){
                upgradedSpellSlot = "SouthT2MiddleSpell";
            }
            if (spell == "Bonus Resistances"){
                upgradedSpellSlot = "SouthT2RightSpell";
            }
            if (spell == "Bonus Fortitude"){
                upgradedSpellSlot = "SouthT3LeftSkill";
            }
            if (spell == "Bonus Arcana"){
                upgradedSpellSlot = "SouthT3RightSpell";
            }
            if (spell == "Cooldown Reduction"){
                upgradedSpellSlot = "SouthT3EndSpell";
            }
            upgradedSpell = AppendNumber(requestedSpell.SpellName, requestedSpell.RequestedSpellRank);
            print("Made it passed AppendNumber");

            CharacterSpellListItem NewSpellItem = (new CharacterSpellListItem{
                Key = upgradedSpellSlot,
                Value = upgradedSpell
            });
            bool purchased = false;
            string spellQ = "None";
            string spellE = "None";
            string spellR = "None";
            string spellF = "None";
            string slot = "None";
            bool trigger = false;
            foreach(var sheet in player.GetInformationSheets()){
                if(sheet.CharacterID == CharID){
                    
                    foreach(var charStat in sheet.CharStatData){
                        if(charStat.Key == "ClassPoints"){
                            classPoints = float.Parse(charStat.Value);
                        }
                    }
                    if(classPoints >= upgradeCost){
                        classPoints -= upgradeCost;
                        purchased = true;
                    } else {
                        return;//not enough points
                    }
                    foreach(var charSpellSLot in sheet.CharSpellData){
                        if(charSpellSLot.Key == "SPELLQ"){
                           if(charSpellSLot.Value != "None"){
                                var nameMatch = System.Text.RegularExpressions.Regex.Match(charSpellSLot.Value, @"^\D*");
                                spellQ = nameMatch.Value.Trim();
                                if(spellQ == spell){
                                    slot = "Q";
                                    trigger = true;
                                    break;
                                }
                           }
                        }
                        if(charSpellSLot.Key == "SPELLE"){
                           if(charSpellSLot.Value != "None"){
                                var nameMatch = System.Text.RegularExpressions.Regex.Match(charSpellSLot.Value, @"^\D*");
                                spellE = nameMatch.Value.Trim();
                                if(spellE == spell){
                                    slot = "E";
                                    trigger = true;
                                    break;
                                }
                           }
                        }
                        if(charSpellSLot.Key == "SPELLR"){
                           if(charSpellSLot.Value != "None"){
                                var nameMatch = System.Text.RegularExpressions.Regex.Match(charSpellSLot.Value, @"^\D*");
                                spellR = nameMatch.Value.Trim();
                                if(spellR == spell){
                                    slot = "R";
                                    trigger = true;
                                    break;
                                }
                           }
                        }
                        if(charSpellSLot.Key == "SPELLF"){
                           if(charSpellSLot.Value != "None"){
                                var nameMatch = System.Text.RegularExpressions.Regex.Match(charSpellSLot.Value, @"^\D*");
                                spellF = nameMatch.Value.Trim();
                                if(spellF == spell){
                                    slot = "F";
                                    trigger = true;
                                    break;
                                }
                           }
                        }
                    }
                    break;
                }
            }
            if(!purchased){
                print("Not Enough mula for the spell");
                return;
            }
            print("Made it passed char spell list item");

            CharacterStatListItem ClassPoints = (new CharacterStatListItem {
                Key = "ClassPoints",
                Value = classPoints.ToString()
            });
            //Send spell and new classpoints
            var request = new UpdateCharacterDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                CharacterId = CharID,
                Data = new Dictionary<string, string>
                {
                    { upgradedSpellSlot, upgradedSpell }, { "ClassPoints", classPoints.ToString()  }
                },
                Permission = UserDataPermission.Private
            };
            print("Made it passed UpdateCharacterDataRequest");

            PlayFabServerAPI.UpdateCharacterInternalData( request, 
            result =>
            {
                //print($"We made it into the characterInternalData request so it was called np");
                player.GetCharacterSpellItemPurchase(CharID, NewSpellItem, ClassPoints);
                if(trigger){
                    PlayerRequestedSpellChange(nconn, upgradedSpell, CharID, slot);
                }

            }, error =>{
                Debug.Log(error.ErrorMessage);
            });  
            #endif
        }
        void PlayerRequestedSpellChange(NetworkConnectionToClient nconn, string _spell, string charID, string slot){
            #if UNITY_SERVER || UNITY_EDITOR
             var nameMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"^\D*");
            string spell = nameMatch.Value.Trim(); // Trim any trailing spaces
            int spellRank = 1;
            // Extract spell rank
            var rankMatch = System.Text.RegularExpressions.Regex.Match(_spell, @"\d+$");
            if (rankMatch.Success) {
                spellRank = int.Parse(rankMatch.Value); // Parse the rank number
            }
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            ScenePlayer player = nconn.identity.gameObject.GetComponent<ScenePlayer>();
            string spellToBeAdded = null;
            string Qspell = "None";
            string Espell = "None";
            string Rspell = "None";
            string Fspell = "None";
            if(slot == "Q"){
                spellToBeAdded = "SPELLQ";
                Qspell = _spell;
                print($"{Qspell} is the Qspell we are seting from the start of Spell Change");
            }
            if(slot == "E"){
                spellToBeAdded = "SPELLE";
                Espell = _spell;
                print($"{Espell} is the Espell we are seting from the start of Spell Change");
            }
            if(slot == "R"){
                spellToBeAdded = "SPELLR";
                Rspell = _spell;
                print($"{Rspell} is the Rspell we are seting from the start of Spell Change");
            }
            if(slot == "F"){
                spellToBeAdded = "SPELLF";
                Fspell = _spell;
                print($"{Fspell} is the Fspell we are seting from the start of Spell Change");
            }
            Dictionary<string,string> EquippedSpells = new Dictionary<string,string>();
            CharacterSpellListItem NewSpellItem = (new CharacterSpellListItem{
                Key = spellToBeAdded,
                Value = _spell
            });
            CharacterSpellListItem updatedItem = (new CharacterSpellListItem{
                Key = "Empty",
                Value = "None"
            });
            bool emptySlot = false;
            if(Qspell == "None" && Espell == "None" && Rspell == "None" && Fspell == "None"){
                emptySlot = true;
                SendSpellList spellListEmpty = (new SendSpellList{
                SpellQ = Qspell,
                SpellE = Espell,
                SpellR = Rspell,
                SpellF = Fspell
            });
            player.GetCharacterSpellChange(charID, NewSpellItem, updatedItem, spellListEmpty);
            } 
            if(!emptySlot){

            foreach (var sheet in player.GetInformationSheets())
            {
                if (sheet.CharacterID == charID)
                {
                    foreach(var spellKey in sheet.CharSpellData){
                        if(spellKey.Key == "SPELLQ" && Qspell == "None"){
                            var matcHQ = System.Text.RegularExpressions.Regex.Match(spellKey.Value, @"^\D*");
                            string spellQ = matcHQ.Value.Trim(); // Trim any trailing spaces
                            if(spellQ == spell){
                                Qspell = "None";
                                updatedItem.Key = spellKey.Key;
                            } else {
                                Qspell = spellKey.Value;
                            }
                            print($"{Qspell} is the Qspell");
                        }
                        if(spellKey.Key == "SPELLE" && Espell == "None"){
                            var matcHE = System.Text.RegularExpressions.Regex.Match(spellKey.Value, @"^\D*");
                            string spellE = matcHE.Value.Trim(); // Trim any trailing spaces
                            if(spellE == spell){
                                Espell = "None";
                                updatedItem.Key = spellKey.Key;
                            } else {
                                Espell = spellKey.Value;
                            }
                            print($"{Espell} is the Espell");
                        }
                        if(spellKey.Key == "SPELLR" && Rspell == "None"){
                            var matcHR = System.Text.RegularExpressions.Regex.Match(spellKey.Value, @"^\D*");
                            string spellR = matcHR.Value.Trim(); // Trim any trailing spaces
                            if(spellR == spell){
                                Rspell = "None";
                                updatedItem.Key = spellKey.Key;
                            } else {
                                Rspell = spellKey.Value;
                            }
                            print($"{Rspell} is the Rspell");
                        }
                        if(spellKey.Key == "SPELLF" && Fspell == "None"){
                            var matcHF = System.Text.RegularExpressions.Regex.Match(spellKey.Value, @"^\D*");
                            string spellF = matcHF.Value.Trim(); // Trim any trailing spaces
                            if(spellF == spell){
                                Fspell = "None";
                                updatedItem.Key = spellKey.Key;
                            } else {
                                Fspell = spellKey.Value;
                            }
                            print($"{Fspell} is the Fspell");
                        }
                    }
                }
            }
            SendSpellList spellList = (new SendSpellList{
                SpellQ = Qspell,
                SpellE = Espell,
                SpellR = Rspell,
                SpellF = Fspell
            });
            player.GetCharacterSpellChange(charID, NewSpellItem, updatedItem, spellList);
            }

            var request = new UpdateCharacterDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                CharacterId = charID,
                Data =  new Dictionary<string, string>
                {
                    {"SPELLQ", Qspell}, {"SPELLE", Espell}, {"SPELLR", Rspell}, {"SPELLF", Fspell}
                },
                Permission = UserDataPermission.Private
            };
            PlayFabServerAPI.UpdateCharacterInternalData(request, result =>
            {
                Debug.Log("Character spell updated successfully");
                // Handle successful response   
            },
            error =>
            {
                Debug.LogError("Failed to update character spell: " + error.ErrorMessage);
                // Handle error response
            });
            #endif
        }
        
       
        void GetDropTable(string mobName, int tier, TurnManager curator)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            //using mobName for different droTableKeys if its not just a standard tier drop
            string dropTableKey = GenerateRarity(mobName);
            //print($"Got to droptable! {dropTableKey} is the droptable name");
            var request = new GetRandomResultTablesRequest{
                TableIDs = new List<string> { dropTableKey }
            };
            PlayFabServerAPI.GetRandomResultTables(request, result => OnGetDropTableSuccess(result, dropTableKey, tier, curator), OnPlayFabError);
                #endif
        }
        public static bool CheckForDropTable(string input)
        {
            //print($"made it to CheckForDropTable, {input} is our input");

            return input.Contains("_Common") || input.Contains("_Uncommon") || input.Contains("_Rare") || input.Contains("_UltraRare") || input.Contains("MainChest");
        }
/*
private void OnGetDropTableSuccess(GetRandomResultTablesResult result, string dropTableKey, int tier, TurnManager curator)
{
    //print($"made it to TableSuccess, {dropTableKey} is our drop table key");

            
    Dictionary<string, RandomResultTableListing> possiblePicks = result.Tables;
    // Convert dictionary keys to a list for random selection
    List<string> keys = possiblePicks.Keys.ToList();

    // Generate random index within range of dictionary keys
    int randomIndex = new System.Random().Next(0, keys.Count);
    //print($"possible picks in this droptablekey were {keys.Count}");

    // Retrieve random key-value pair from dictionary
    KeyValuePair<string, RandomResultTableListing> randomPick = possiblePicks.ElementAt(randomIndex);
    string tableChecker = randomPick.Value.TableId;
    //print($"tableChecker name is {tableChecker}");
    int randomNodeIndex = new System.Random().Next(0, randomPick.Value.Nodes.Count);
    //List<ResultTableNode> RandomResultTableListing.Nodes
    ResultTableNode randomItem = randomPick.Value.Nodes.ElementAt(randomNodeIndex);
    if(CheckForDropTable(randomItem.ResultItem)){
        var request = new GetRandomResultTablesRequest
        {
            TableIDs = new List<string> { randomItem.ResultItem }
        };
        PlayFabServerAPI.GetRandomResultTables(request, result => OnGetDropTableSuccess(result, dropTableKey, tier, curator), OnPlayFabError);
    } else {
        ProcessDropTable(randomItem.ResultItem, tier, curator);
        //print($"made it to TableSuccess, {randomItem.ResultItem} is our tableChecker key");
    }
}
private void OnPlayFabError(PlayFabError error)
{
    Debug.LogError($"PlayFab Error: {error.GenerateErrorReport()}");
}
private void ProcessDropTable(string itemName, int tier, TurnManager curator)
{
     //print($"ProcessingDropTable!!");
    Guid uniqueId = Guid.NewGuid();
    int quant = 1;
    //print("About to item roll!!");
    curator.ItemAvailableToRoll(itemName, quant, uniqueId.ToString(), tier);
}
*/
void GetMiniChestDropTable(int tier, TurnManager curator)
{
    
    #if UNITY_SERVER || UNITY_EDITOR
    string dropTableKey = "MiniChest_Tier" + tier.ToString();
    // Using mobName for different dropTableKeys if it's not just a standard tier drop
    var request = new GetRandomResultTablesRequest
    {
        TableIDs = new List<string> { dropTableKey }
    };
    PlayFabServerAPI.GetRandomResultTables(request, result => OnGetDropTableSuccess(result, dropTableKey, tier, curator), OnPlayFabError);
    #endif
}
void GetArmorRackDropTable(int tier, TurnManager curator)
{
    
    #if UNITY_SERVER || UNITY_EDITOR
    string dropTableKey = "ArmorDrop_Tier" + tier.ToString();
    // Using mobName for different dropTableKeys if it's not just a standard tier drop
    var request = new GetRandomResultTablesRequest
    {
        TableIDs = new List<string> { dropTableKey }
    };
    PlayFabServerAPI.GetRandomResultTables(request, result => OnGetDropTableSuccess(result, dropTableKey, tier, curator), OnPlayFabError);
    #endif
}
void GetWeaponRackDropTable(int tier, TurnManager curator)
{
    
    #if UNITY_SERVER || UNITY_EDITOR
    string dropTableKey = "WeaponDrop_Tier" + tier.ToString();
    // Using mobName for different dropTableKeys if it's not just a standard tier drop
    var request = new GetRandomResultTablesRequest
    {
        TableIDs = new List<string> { dropTableKey }
    };
    PlayFabServerAPI.GetRandomResultTables(request, result => OnGetDropTableSuccess(result, dropTableKey, tier, curator), OnPlayFabError);
    #endif
}
void GetMainChestDropTable(int tier, TurnManager curator)
{
    #if UNITY_SERVER || UNITY_EDITOR
    StartCoroutine(FetchBalance(tier, curator));
    string dropTableKey = "MainChest_Tier" + tier.ToString();
    // Using mobName for different dropTableKeys if it's not just a standard tier drop
    for (int i = 0; i < curator.GetPlayers().Count; i++)
    {
        print("Getting result for main chest drops!!");
        var request = new GetRandomResultTablesRequest
        {
            TableIDs = new List<string> { dropTableKey }
        };
        PlayFabServerAPI.GetRandomResultTables(request, result => OnGetDropTableSuccess(result, dropTableKey, tier, curator), OnPlayFabError);
    }
    #endif
}
public float GetDKPBalance(string jsonResponse)
    {
        float DKPAmount = 0f;
        Debug.Log("JSON Response: " + jsonResponse);
        
        // Parse the JSON response
        CurrencyBalance[] balances = JsonHelper.FromJson<CurrencyBalance>(jsonResponse);

        // Iterate through the array to find the DKP balance
        foreach (CurrencyBalance balance in balances)
        {
            if (balance.currency == "DKP")
            {
                Debug.Log("DKP Balance: " + balance.value);
                DKPAmount = float.Parse(balance.value);
                break;
            }
        }
        return DKPAmount;
    }
private double CalculateReward(int tier, float dkpBalance)
{
    double rate = 0.0;
    switch (tier)
    {
        case 1: rate = 0.000004; break;
        case 2: rate = 0.000007; break;
        case 3: rate = 0.000011; break;
        case 4: rate = 0.000019; break;
        case 5: rate = 0.000032; break;
    }

    double reward = rate * dkpBalance * (1.0 / 1000.0); // scaling down
    return reward;
}
    IEnumerator GetNFTMetadata(string metadataURI)
{
    metadataURI = "https://ipfs.io/ipfs/bafybeic2bf4nsmcczycgunv6foyhmxfqqzatawqhcwla7hqital65coqzq/metadata.json";
    UnityWebRequest www = UnityWebRequest.Get(metadataURI);
    yield return www.SendWebRequest();

    if (www.result != UnityWebRequest.Result.Success)
    {
        Debug.Log(www.error);
    }
    else
    {
        string metadataJson = www.downloadHandler.text;
        
        NFTMetadata metadata = JsonUtility.FromJson<NFTMetadata>(metadataJson);

        // Now, you can access properties like this:
        string md5hash = metadata.md5hash;
        string issuer = metadata.external_url; // or whatever field represents the issuer in your case

        Debug.Log("MD5 Hash: " + md5hash);
        Debug.Log("Issuer: " + issuer);
        // Perform your checks or operations here
    }
}
    #if UNITY_SERVER || UNITY_EDITOR

IEnumerator FetchBalance(int tier, TurnManager curator)
    {

        UnityWebRequest www = UnityWebRequest.Get(xrpscanAPI + _GAME_WALLET_ADDRESS + "/balances");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            float DKP_Amount = GetDKPBalance(www.downloadHandler.text);
            double reward = CalculateReward(tier, DKP_Amount);
            Debug.Log($"Reward for Tier {tier}: {reward} DKP");
            reward /= curator.GetPlayers().Count;
            Debug.Log($"Reward for each player is {reward} DKP");
            foreach(var player in curator.GetPlayers()){
                NetworkIdentity networkIdentity = player.GetComponent<NetworkIdentity>();
                if (networkIdentity != null)
                {
                    NetworkConnectionToClient conn = networkIdentity.connectionToClient as NetworkConnectionToClient;
                    if (conn != null)
                    {
                        if (!(conn.authenticationData is PlayerInfo playerData))
                        {
                            Debug.LogWarning("Authentication data is missing or not of type PlayerInfo.");
                            continue;
                        }

                        PlayFabServerAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest
                        {
                            PlayFabId = playerData.PlayFabId,
                            Amount = (int)reward,
                            VirtualCurrency = "DK"
                        }, result =>
                        {
                            int quant = (int)reward;
                            int newGoldAmount = quant + (int)player.Gold;
                            //print($"Quant amount is now {quant}");
                            //print($"We added gold to our account, gold amount is now {newGoldAmount}");
                            player.Gold = (long)newGoldAmount;
                            player.SetGold(player.Gold);
                        }, error =>{
                            Debug.Log(error.ErrorMessage);
                        });

                    }
                    else
                    {
                        Debug.LogWarning("Connection is not of type NetworkConnectionToClient.");
                    }
                }
                else
                {
                    Debug.LogWarning("NetworkIdentity component is missing.");
                    continue;
                }
            }
            SendingGoldAmount.Invoke(curator.GetMatch(), curator, (float)reward);
            print("Sending Gold Amount");
        }
    }
        #endif
    
private int CalculateMainChestItemsToGenerate()
{
    return new System.Random().Next(1, 6); // Returns a random integer between 1 and 5 (inclusive)
}
#if UNITY_SERVER || UNITY_EDITOR
private void OnGetDropTableSuccess(GetRandomResultTablesResult result, string dropTableKey, int tier, TurnManager curator){
    //print($"made it to TableSuccess, {dropTableKey} is our drop table key");
    Dictionary<string, RandomResultTableListing> possiblePicks = result.Tables;
    List<string> keys = possiblePicks.Keys.ToList();
    int randomIndex = new System.Random().Next(0, keys.Count);
    //print($"possible picks in this droptablekey were {keys.Count}");
    KeyValuePair<string, RandomResultTableListing> randomPick = possiblePicks.ElementAt(randomIndex);
    string tableChecker = randomPick.Value.TableId;
    //print($"tableChecker name is {tableChecker}");
    int randomNodeIndex = new System.Random().Next(0, randomPick.Value.Nodes.Count);
    ResultTableNode randomItem = randomPick.Value.Nodes.ElementAt(randomNodeIndex);
    if (CheckForDropTable(randomItem.ResultItem)){
        var request = new GetRandomResultTablesRequest{
            TableIDs = new List<string> { randomItem.ResultItem }
        };
        PlayFabServerAPI.GetRandomResultTables(request, result => OnGetDropTableSuccess(result, randomItem.ResultItem, tier, curator), OnPlayFabError);
    } else {
        FetchItemDetailsAndProcess(randomItem.ResultItem, tier, curator);
    }
}
#endif
private void FetchItemDetailsAndProcess(string itemId, int tier, TurnManager curator)
{
#if UNITY_SERVER || UNITY_EDITOR
var request = new GetCatalogItemsRequest();
request.CatalogVersion = "DragonKill_Characters_Bundles_Items";
PlayFabServerAPI.GetCatalogItems(request, result =>
{
if (result.Catalog != null && result.Catalog.Count > 0)
{
    // Search through the catalog for the item you are interested in
    foreach (var item in result.Catalog)
    {
        if (item.ItemId == itemId)
        {
            // Found the item, now use its display name
            var displayName = item.DisplayName;
            ProcessDropTable(displayName, item.ItemId,  tier, curator);
            break; // exit the loop once we found the item
        }
    }
}
}, OnPlayFabError);
#endif
}

private void ProcessDropTable(string itemName, string itemID, int tier, TurnManager curator)
{
    print($"ProcessingDropTable for {itemName} and its itemID is {itemID}");
    Guid uniqueId = Guid.NewGuid();
    int quant = 1;
    //print("About to item roll!!");
    curator.ItemAvailableToRoll(itemName, quant, uniqueId.ToString(), tier, itemID);
}

private void OnPlayFabError(PlayFabError error)
{
    Debug.LogError($"PlayFab Error: {error.GenerateErrorReport()}");
}
        public string GenerateRarity(string mobName)
        {
            float roll = UnityEngine.Random.Range(0f, 1f);
            string rarity = string.Empty;
            if (roll < 0.80f)
            {
                rarity = "_Common";
            }
            else if (roll < 0.95f)
            {
                rarity = "_Uncommon";
            }
            else if (roll < 0.99f)
            {
                rarity = "_Rare";
            }
            else
            {
                rarity = "_UltraRare";
            }
            return mobName + rarity;
        }
        
         //foreach(var item in items){
            //    itemFound = item;
            //    if(item == "GoldBar"){
            //        //print($"Item was a gold bar");
            //        int quant = 0;
            //        quant = GetGoldQuant(tier);
            //        PlayFabServerAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest
            //        {
            //            PlayFabId = playerData.PlayFabId,
            //            Amount = quant,
            //            VirtualCurrency = "DK"
            //        }, result =>
            //        {
            //            quant = (int)sPlayer.Gold + quant;
            //            //print($"Quant amount is now {quant}");
            //            //print($"We added gold to our account, gold amount is now {sPlayer.Gold}");
            //            sPlayer.Gold = (uint)(quant);
            //            sPlayer.TargetWalletAwake();
            //            return;
            //        }, error =>{
            //            Debug.Log(error.ErrorMessage);
            //        });
            //        //run the coins stuff which would be the player user coins
            //    }
            //}
            //if(itemFound == "GoldBar"){
            //    //print($"Item was a gold bar");
            //    return;
            //}
            ////print($"Item was NOT a gold bar, item is {itemFound}");
        void BuildItemDropped(ItemBuildMessage message){
            #if UNITY_SERVER || UNITY_EDITOR
            NetworkConnectionToClient nconn = message.owner.connectionToClient;
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            {
                PlayFabId = playerData.PlayFabId,
                ItemIds = message.ItemID
            }, result =>
            {
                
                foreach(var item in result.ItemGrantResults){
                    string Quality = "Plain";
                    if(item.ItemClass == "TwoHandedWeapon" || item.ItemClass == "SingleHandedWeapon" || item.ItemClass == "Head" || 
                    item.ItemClass == "Chest"  || item.ItemClass == "Waist" || item.ItemClass == "Wrists" || item.ItemClass == "Earring" ||
                    item.ItemClass == "Arms" || item.ItemClass == "Feet" || item.ItemClass == "Hands" || item.ItemClass == "Ring"
                    || item.ItemClass == "Leggings" || item.ItemClass == "Necklace" || item.ItemClass == "OffHand"
                    || item.ItemClass == "Shield" || item.ItemClass == "Shoulders" || item.ItemClass == "Arms"){
                        Quality = GetQualityDropFormula();
                    }
                    string choice = string.Empty;
                    if(!string.IsNullOrEmpty(message.needWinnerSerial)){
                        choice = message.needWinnerSerial;
                    } else {
                        choice = message.greedWinnerSerial;
                    }
                    //print($"{choice} is choice {Quality} is quality");
                    TransformItemIntoDragonKill(nconn, playerData, item, choice, Quality, 1, false, false, null);
                }
            }, error =>{
                Debug.Log(error.ErrorMessage);
            });
            #endif
        }
         void BuildNewStack(NewStackCreated message){
            #if UNITY_SERVER || UNITY_EDITOR
            print("BuildNewStack 1");
            print($"Starting BuildNewStack {message.firstStackAmount} was first amount for test {message.assignedItem.itemID} is itemID");
            NetworkConnectionToClient nconn = message.owner.connectionToClient;
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            List<string> ids = new List<string>
            {
                message.assignedItem.itemID
            };
            //var request = new GetCatalogItemsRequest();
            CharacterInventoryListItem modItem = new CharacterInventoryListItem();
            //request.CatalogVersion = "DragonKill_Characters_Bundles_Items";
            print("BuildNewStack Checking char sheets");

            if(message.selectedDestination != "Stash" && message.selectedDestination != "Tactician"){
                foreach(var sheet in message.owner.GetInformationSheets()){
                    if(sheet.CharacterID == message.selectedDestination){
                        foreach(var charItem in sheet.CharInventoryData){
                            if(charItem.Value.customID == message.assignedItem.customID){
                                //found our item to modify
                                modItem = charItem;
                                modItem.Value.amount = message.firstStackAmount;
                                modItem.Value.Changed = true;
                                //message.owner.GetCharacterNewItem(message.selectedDestination, modItem);
                                print("BuildNewStack Checking char sheets Found mod!!");
                                break;
                            }
                        }
                        if(modItem.Value != null){
                            message.owner.GetCharacterNewItem(message.selectedDestination, modItem);
                        }
                    }
                }
            }
            print("BuildNewStack Checking stash");

            if(message.selectedDestination == "Stash"){
                foreach(var stashItem in message.owner.GetTacticianSheet().StashInventoryData){
                    if(stashItem.Value.customID == message.assignedItem.customID){
                        //found our item to modify
                        modItem = stashItem;
                        modItem.Value.amount = message.firstStackAmount;
                        modItem.Value.Changed = true;
                        print("BuildNewStack Checking stash Found mod!!");
                        break;
                    }
                }
                if(modItem.Value != null){
                    message.owner.GetStashNewItem(modItem);
                }

            }
            print("BuildNewStack Checking tact");

            if(message.selectedDestination == "Tactician"){
                foreach(var tactItem in message.owner.GetTacticianSheet().TacticianInventoryData){
                    if(tactItem.Value.customID == message.assignedItem.customID){
                        //found our item to modify
                        modItem = tactItem;
                        modItem.Value.amount = message.firstStackAmount;
                        modItem.Value.Changed = true;
                        //message.owner.GetTacticianNewItem(modItem);
                        print("BuildNewStack Checking tact Found mod!!");
                        break;
                    }
                }
                if(modItem.Value != null){
                    message.owner.GetTacticianNewItem(modItem);
                }
            }
            print("BuildNewStack 2");
            // here we are going to not get the catalog we are going to just simply build a copy of this item, but build it a new customID and also set its instanceID to be ItemGenerated or whatever it was

            ItemSelectable createdItem = new ItemSelectable{
                itemType = modItem.Value.itemType, amount = message.secondStackAmount, Weight = modItem.Value.Weight, Item_Name = modItem.Value.Item_Name, Durability = modItem.Value.Durability, DurabilityMax = modItem.Value.DurabilityMax,
                ItemSpecificClass = modItem.Value.ItemSpecificClass, itemSlot = modItem.Value.itemSlot, InstanceID = "NewItemGenerated", customID = Guid.NewGuid().ToString(), OwnerID = message.selectedDestination, TacticianInventory = modItem.Value.TacticianInventory,
                OGTacticianStash = false, OGTacticianBelt = false, OGTacticianInventory = false, TacticianStash = modItem.Value.TacticianStash, INVENTORY = modItem.Value.INVENTORY, Quality_item = modItem.Value.Quality_item, itemID = modItem.Value.itemID, NFT = false, EQUIPPED = false, EQUIPPEDSLOT = "0", ItemDescription = modItem.Value.ItemDescription,
                STRENGTH_item = "0", AGILITY_item = "0", FORTITUDE_item = "0", ARCANA_item = "0", Armor_item = "0", MagicResist_item = "0", FireResist_item = "0", ColdResist_item = "0", DiseaseResist_item = "0", PoisonResist_item = "0", Rarity_item = modItem.Value.Rarity_item, Changed = true
            };
            CharacterInventoryListItem DATA = new CharacterInventoryListItem();
            DATA.Key = createdItem.InstanceID;
            DATA.Value = createdItem;
            if(message.selectedDestination == "Tactician"){
                message.owner.ServerTacticianItemResult(DATA);
            }
            if(message.selectedDestination == "Stash"){
                message.owner.ServerPurchasedItemResult(DATA);
            }
            if(message.selectedDestination != "Tactician" && message.selectedDestination != "Stash"){
                string _class = string.Empty;
                foreach(var sheet in message.owner.GetInformationSheets()){
                    if(sheet.CharacterID == message.selectedDestination){
                        foreach(var stat in sheet.CharStatData){
                            if(stat.Key == "Class"){
                                _class = stat.Value;
                            }
                        }
                    }
                }
                message.owner.GetCharacterPickedUpItem(message.selectedDestination, _class, DATA);
            }
            

            //PlayFabServerAPI.GetCatalogItems(request, result =>
            //{
            //print("BuildNewStack 3");
            //if (result.Catalog != null && result.Catalog.Count > 0)
            //{
            //    // Search through the catalog for the item you are interested in
            //    foreach (var item in result.Catalog)
            //    {
            //       if(item.ItemId == message.assignedItem.itemID){
            //        PlayFabServerAPI.GrantItemsToUser(new GrantItemsToUserRequest
            //        {
            //            PlayFabId = playerData.PlayFabId,
            //            ItemIds = ids
            //        }, result =>
            //        {
            //            print("BuildNewStack 4");
            //            print($"Starting BuildNewStack TransformItemIntoDragonKill {message.secondStackAmount} going to {message.selectedDestination}");
            //            string Quality = "Plain";
            //            foreach(var granteditem in result.ItemGrantResults){
            //                TransformItemIntoDragonKill(nconn, playerData, granteditem, message.selectedDestination, Quality, message.secondStackAmount, false, true);
            //            }
            //        }, error =>{
            //            Debug.Log(error.ErrorMessage);
            //        });
            //        break;
            //       }
            //    }
            //}
            //}, OnPlayFabError);
            //
            #endif
        }
        string GetQualityDropFormula()
        {
            System.Random random = new System.Random();
            double randomNumber = random.NextDouble() * 100;
            string Quality = "Plain";
            if (randomNumber <= 96)
            {
                Quality = "Plain";
            }
            else if (randomNumber <= 98.89)
            {
                Quality = "Fine";
            }
            else if (randomNumber <= 99.59)
            {
                Quality = "Superb";
            }
            else if (randomNumber <= 99.89)
            {
                Quality = "Magnificent";
            }
            else if (randomNumber <= 99.99)
            {
                Quality = "Kingly";
            }
            else if (randomNumber <= 100)
            {
                Quality = "Divine";
            }

            return Quality;
        }
        void GiveExpClassPoints(float experience, float classPoints, Match match){
            int playerCount = match.playerSlotPairs.Count;
            float expPerPlayer = experience / (float)playerCount;
            float cpPerPlayer = classPoints / (float)playerCount;
            SendEXPCP.Invoke(match, cpPerPlayer, expPerPlayer);
            foreach(var player in match.playerSlotPairs){
                PlayFabSaveEXPandClassPoints(player.player, player.slot, expPerPlayer, cpPerPlayer);
            }
        }
        void PlayFabSaveEXPandClassPoints(ScenePlayer sPlayer, string charSlot, float experienceValue, float classPointsValue){
            #if UNITY_SERVER || UNITY_EDITOR
            //get player netconnection, 
            NetworkConnectionToClient nconn = sPlayer.connectionToClient;
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            float EXP = 0f;
            float CP = 0f;
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == charSlot){
                    foreach(var charStat in sheet.CharStatData){
                        if(charStat.Key == "ClassPoints"){
                            CP = float.Parse(charStat.Value);
                        }
                        if(charStat.Key == "EXP"){
                            EXP = float.Parse(charStat.Value);
                        }
                        if(charStat.Key == "DEATH")
                            return;
                    }
                }
            }
            CharacterStatListItem ClassPointsItem = (new CharacterStatListItem{
                Key = "ClassPoints",
                Value = Math.Round(CP + classPointsValue, 2).ToString("F2")
            });
            CharacterStatListItem EXPItem = (new CharacterStatListItem{
                Key = "EXP",
                Value = Math.Round(EXP + experienceValue, 2).ToString("F2")
            });
            sPlayer.GetCharacterUpdateEXPCP(charSlot, ClassPointsItem, EXPItem);
            //PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
            //{
            //    PlayFabId = playerData.PlayFabId,
            //    CharacterId = charSlot,
            //    Data = new Dictionary<string, string>
            //    {
            //        {"EXP", Math.Round(EXP + experienceValue, 2).ToString("F2")},
            //        {"ClassPoints", Math.Round(CP + classPointsValue, 2).ToString("F2")}
            //    }
            //}, result =>
            //{
            //}, error =>{
            //    Debug.Log(error.ErrorMessage); 
            //    Debug.Log(error.ErrorDetails);
            //    Debug.Log(error.Error);
            //});
            #endif
        }
        void RequestingPromotion(ScenePlayer sPlayer, string charSlot){
            #if UNITY_SERVER || UNITY_EDITOR
            // Create a dictionary to store the experience required for each level
            NetworkConnectionToClient nconn = sPlayer.connectionToClient;
            PlayerInfo playerData = (PlayerInfo)nconn.authenticationData;
            Dictionary<int, float> expPerLevel = new Dictionary<int, float> {
                {1, 4000f}, {2, 8000f}, {3, 12000f}, {4, 20000f}, {5, 30000f},
                {6, 55000f}, {7, 75000f}, {8, 100000f}, {9, 145000f}, {10, 180000f},
                {11, 220000f}, {12, 265000f}, {13, 300000f}, {14, 350000f}, {15, 425000f},
                {16, 500000f}, {17, 600000f}, {18, 700000f}, {19, 800000f}, {20, 1000000f}
            };
            Dictionary<int, int> energyCostPerLevel = new Dictionary<int, int> {
                {1, 100}, {2, 200}, {3, 300}, {4, 400}, {5, 500},
                {6, 600}, {7, 700}, {8, 800}, {9, 900}, {10, 1000},
                {11, 1200}, {12, 1400}, {13, 1600}, {14, 1800}, {15, 2000},
                {16, 2200}, {17, 2400}, {18, 2600}, {19, 2800}, {20, 3000}
            };

            Dictionary<int, float> timeCostPerLevel = new Dictionary<int, float> {
                {1, 1f}, {2, 5f}, {3, 15f}, {4, 60f}, {5, 180f},
                {6, 300f}, {7, 360f}, {8, 420f}, {9, 480f}, {10, 540f},
                {11, 660f}, {12, 780f}, {13, 900f}, {14, 1020f}, {15, 1140f},
                {16, 1260f}, {17, 1380f}, {18, 1500f}, {19, 1620f}, {20, 1740f}
            };
            float EXP = 0f;
            int LVL = 0;
            bool expFound = false;
            bool lvlFound = false;
            if(sPlayer.Energy >= energyCostPerLevel[LVL]){
                // If so, increase the level and decrease EXP by the required amount
                sPlayer.Energy -= energyCostPerLevel[LVL];
            } else {
                return;
            }
            foreach(var sheet in sPlayer.GetInformationSheets()){
                if(sheet.CharacterID == charSlot){
                    foreach(var charStat in sheet.CharStatData){
                        if(charStat.Key == "LVL"){
                            LVL = int.Parse(charStat.Value);
                            lvlFound = true;
                        }
                        if(charStat.Key == "EXP"){
                            EXP = float.Parse(charStat.Value);
                            expFound = true;
                        }
                        if(expFound && lvlFound){
                            break;
                        }
                    }
                }
            }

            // Check if EXP is greater than or equal to the required experience for the next level
            if(EXP >= expPerLevel[LVL]){
                // If so, increase the level and decrease EXP by the required amount
                LVL += 1;
                EXP -= expPerLevel[LVL];
            }
            DateTime endTime = DateTime.UtcNow.AddMinutes(timeCostPerLevel[LVL]);

            // Convert the end time to a string
            string endTimeString = endTime.ToString("o"); // "o" is the round-trip format specifier

            CharacterStatListItem TimeStamp = (new CharacterStatListItem{
                Key = "TimeStampLVL",
                Value = endTimeString
            });
            CharacterStatListItem EXPItem = (new CharacterStatListItem{
                Key = "EXP",
                Value = Math.Round(EXP, 2).ToString("F2")
            });
            

            sPlayer.GetCharacterUpdateEXPLVL(charSlot, TimeStamp, EXPItem);
            PlayFabServerAPI.UpdateCharacterInternalData(new UpdateCharacterDataRequest
            {
                PlayFabId = playerData.PlayFabId,
                CharacterId = charSlot,
                Data = new Dictionary<string, string>
                {
                    {"EXP", Math.Round(EXP, 2).ToString("F2")},
                    {"TimeStampLVL", endTimeString}
                }
            }, result =>
            {
            }, error =>{
                Debug.Log(error.ErrorMessage); 
                Debug.Log(error.ErrorDetails);
                Debug.Log(error.Error);
            });
            #endif
        }
    }
    [System.Serializable]
    public class DropTable
    {
        public List<DropTableItem> items;
    }

    [System.Serializable]
    public class DropTableItem
    {
        public string id;
        public float probability;
        public Guid uniqueId;
    }
    public class PlayerConnection
    {
        #if UNITY_SERVER || UNITY_EDITOR
        
                public ConnectedPlayer ConnectedPlayer;
        #endif
                public int ConnectionId;
        public NetworkConnectionToClient conn;

    }
    public class SpawnInfo
    {
        public bool MainChestLinked { get; set; }
        public string GroupName { get; set; }
        public string PointName { get; set; }
        public Transform SpawnTransform { get; set; }
    }
}