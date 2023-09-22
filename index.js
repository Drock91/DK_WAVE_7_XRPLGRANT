var express = require('express');
const axios = require('axios');
const rateLimit = require('express-rate-limit');
const crypto = require('crypto');
var router = express.Router();
const app = express();
const bodyParser = require('body-parser');
const {TxData} = require('xrpl-txdata');
const xrpl = require("xrpl");
require('dotenv').config();
const { response } = require('express');
const Verify = new TxData();
const {XummSdk} = require('xumm-sdk')
var PORT = process.env.PORT || 3000;
const limiter = rateLimit({
  windowMs: 60 * 1000,
  max: 10
});
const headers = {
  'x-api-key': process.env.XUMM_PUBLIC,
  'x-api-secret': process.env.XUMM_PRIVATE,
}
const Sdk = new XummSdk(process.env.XUMM_PUBLIC, process.env.XUMM_PRIVATE)
//app.use('/transmute', limiter);
//app.use('/register', limiter);
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
app.use(express.json());
//let pendingQueue = [];
let currentRequests = 0;
const MAX_REQUESTS = 95;
const WINDOW_MS = 60 * 1000; // 1 minute

const resetRateLimit = () => {
  currentRequests = 0;
};
setInterval(resetRateLimit, WINDOW_MS);

// Release 95 requests every minute

app.post('/xummqueue', (req, res) => {
  const id = req.query.id;  // Extracting the ID from the URL query parameter
  if (id === undefined) {
    res.status(400).send("ID is required");
    return;
  }
  //if (!pendingQueue.some(item => item.id === id)) {
  //  pendingQueue.push({ id, timestamp: Date.now() });
  //}
  // Your processing logic here, you can now use the 'id'

  if (currentRequests < MAX_REQUESTS) {
    currentRequests++;
    res.status(200).send("Request processed");
  } else {
    res.status(400).send("ID is required");
  }
});
//api endpoint for transmuting gold to DKP 
//Adding a security key system here that will provide game servers with a token on start up that is maintained here in heroku. 
//The token will be how servers are able to unlock this endpoint to make transmutes happen for players
app.post('/dkpsend', async (req, res, next) => {
  console.log('We are listening')
  console.log(req.body.userId + req.body.walletAddress + req.body.goldAmount);
  const client = new xrpl.Client("wss://xrplcluster.com");
  await client.connect();
  const dkpWallet = xrpl.Wallet.fromSeed(process.env.SENDER_SEED);
  const currency_code = "DKP"
  // Send token ----------------------------------------------------------------
  const issue_quantity = req.body.goldAmount
  const send_token_tx = {
    "TransactionType": "Payment",
    "Account": process.env.SENDER_PUBLIC,
    "Amount": {
      "currency": currency_code,
      "value": req.body.goldAmount,
      "issuer": "rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J"
    },
    "Destination": req.body.walletAddress
  }
  const pay_prepared = await client.autofill(send_token_tx)
  const pay_signed = dkpWallet.sign(pay_prepared)
  const pay_result = await client.submitAndWait(pay_signed.tx_blob)
  console.log(pay_result);
  if (pay_result.result.meta.TransactionResult == "tesSUCCESS") {
    console.log(`Transaction succeeded: https://mainnet.xrpl.org/transactions/${pay_signed.hash}`)
    let xrpBalance = await client.getXrpBalance(req.body.walletAddress);
    let dkpBalance = "0";
    const balances = await client.getBalances(req.body.walletAddress);
    for (const balance of balances) {
      if (balance.currency === 'DKP') {
        dkpBalance = balance.value;
      }
    }
    const responseObj = {
      success: 'true',
      message: 'Transaction and gold removal successful',
      details: {
        userId: req.body.userId,
        goldAmount: req.body.goldAmount,
        walletAddress: req.body.walletAddress,
        xrpBalance: xrpBalance,
        dkpBalance: dkpBalance
      }
    }   
    res.status(200).json(responseObj);
  console.log(responseObj);
} else {
  console.log(responseObj);
  console.log("failed");
  const responseObj = {
    success: 'false',
    message: 'Transaction and gold removal successful',
    details: {
        userId: userId,
        goldAmount: goldAmount,
        walletAddress: req.body.walletAddress
        //xrpBalance: xrpBalance,  // Added XRP balance
        //dkpBalance: dkpBalance   // Added DKP balance
    }
  };
  res.status(200).json(responseObj);
  console.log(responseObj);
  throw `Error sending transaction: ${pay_result.result.meta.TransactionResult}`
}
client.disconnect()
});
let pendingPayloadIds = [];

// XUMM webhook handling
// this is our callback for when a player makes an input on their xumm app
app.post('/xumm-webhook', async (req, res) => {
  console.log("starting webhook code")
  const timestamp = req.headers['x-xumm-request-timestamp'] || '';
    const json = req.body;

    const hmac = crypto.createHmac('sha1', process.env.XUMM_PRIVATE.replace('-', ''))
      .update(timestamp + JSON.stringify(json))
      .digest('hex');

    if (hmac !== req.headers['x-xumm-request-signature']) {
      console.warn('Signature mismatch. Possible tampering detected.');
      return res.status(401).send('Unauthorized');
    }
  //console.log("This was our req body:", JSON.stringify(req.body, null, 2));
  //console.log("This was our req headers:", JSON.stringify(req.headers, null, 2));
  //console.log("This was our payloadResponse:", JSON.stringify(req.body.payloadResponse, null, 2));
  //console.log("This was our custom_meta:", JSON.stringify(req.body.custom_meta, null, 2));
  const payloadId = req.body.payloadResponse.payload_uuidv4;
  //if(!payloadId === null){
    console.log(req.body.payloadResponse.payload_uuidv4 + " this was our payloadResponse!")
    //const verifying = await Verify.getOne(payloadId)
    //if(verifying){
    //  console.log("we made it to verify from xumm hook")
    //  // You can push additional information to your pendingPayloadIds array if needed.
    //  //pendingPayloadIds.push({ payloadId, _timestamp, isSigned, customMetablob });
    //}
    const _timestamp = Date.now();
    const isSigned = req.body.payloadResponse.signed;
    const customMetablob = req.body.custom_meta.blob;
    const txid = req.body.payloadResponse.txid;

    // You can push additional information to your pendingPayloadIds array if needed.
    console.log("adding to pendingPayloads !! ************************ LOOK FOR THIS IN LOG")
    pendingPayloadIds.push({ payloadId, _timestamp, isSigned, customMetablob, txid});
  //}
  
  res.status(200).send("OK");
});
// Cleanup old entries every 5 minutes
setInterval(() => {
  const fiveMinutesAgo = Date.now() - (5 * 60 * 1000);
  pendingPayloadIds = pendingPayloadIds.filter(item => item._timestamp > fiveMinutesAgo);
}, 5 * 60 * 1000);
async function getPayloadInfo(payloadId) {
  try {
    const response = await axios.get(`https://xumm.app/api/v1/platform/payload/${payloadId}`, { headers });
    if (response.status === 200) {
      return response;
    } else {
      return null;
    }
  } catch (error) {
    console.error(error);
    return null;
  }
}
//calculate the market price of cheapest dkp order to fill for our player
// so they get the best price and the order has enough liquidity
app.post('/GetMarketPrice', async (req, res) => {
  const desiredAmount = parseFloat(req.body.amount);  // Assuming amount is passed as a string
  const client = new xrpl.Client('wss://xrplcluster.com');
  await client.connect();
  const orderBook = await client.request({
    command: 'book_offers',
    taker_pays: {
      currency: 'XRP'
    },
    taker_gets: {
      currency: 'DKP',
      issuer: 'rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J'
    },
    limit: 1000
  });
  await client.disconnect();
  // Sort the offers by rate in ascending order
  orderBook.result.offers.sort((a, b) => parseFloat(a.quality) - parseFloat(b.quality));
  let aggregateLiquidity = 0;
  let bestRate = 0;
  for (const offer of orderBook.result.offers) {
    const availableDKP = parseFloat(offer.TakerGets.value);
    aggregateLiquidity += availableDKP;
    if (aggregateLiquidity >= desiredAmount) {
      bestRate = parseFloat(offer.quality);
      break;
    }
  }
  if (bestRate === 0) {
    console.log('Not enough liquidity to fulfill 50,000 DKP.');
    res.json({ meta: { error: 'Not enough liquidity' } });
    return;
  }
  const bestMarketPrice = ((bestRate / 1000000 )* desiredAmount).toString();
  console.log(bestMarketPrice + " was our best market price");
  const xummDetailedResponse = {
    meta: {
      bestMarketPrice: bestMarketPrice,
    }
  };
  res.json(xummDetailedResponse);
});

  app.get('/check-payload/:payloadId/:walletAddress', async(req, res) => {
  if (currentRequests < MAX_REQUESTS) {
    currentRequests++;
  } else {
    res.status(400).send("ID is required");
  }
  if (pendingPayloadIds.length > 0) {
    console.log("Contents of pendingPayloadIds array:");
    console.log(pendingPayloadIds);
  } else {
    console.log("The pendingPayloadIds array is empty.");
    return res.json(null);
  }
  const { payloadId, walletAddress } = req.params;
  //const { payloadId } = req.params;
  if(req.params === null){
    console.log("The params are empty.");
    return res.json(null);
  }
  const payload = pendingPayloadIds.find(item => item.payloadId === payloadId);
  if (!payload) {
    console.log("The payload not found!!");

    return res.json(null);
  }
  let expired = false;
  let fiveMinutesAgo = Date.now() - (5 * 60 * 1000);
  const payloadInfo = await getPayloadInfo(payloadId);
  if (payloadInfo) {
    
    //console.log("Payload info:", payloadInfo.headers);
    console.log("This was our payloadInfo:", JSON.stringify(payloadInfo.data, null, 2));
    const payloadType = payloadInfo.data.payload.tx_type;
    let addressToUse = null;
    // Check if 'walletAddress' is not null, not empty, and not "Undefined"
    if (walletAddress && walletAddress !== "Undefined") {
      addressToUse = walletAddress;
    }
    // Check if 'payloadInfo.data.response.account' is not null and not empty
    if (payloadInfo && payloadInfo.data && payloadInfo.data.response && payloadInfo.data.response.account) {
      addressToUse = payloadInfo.data.response.account;
    }
    //if not a trustset we can check this, if it is a first call for registration there will be no trustline check unfortunately so if its a decline send them back to start
    if(payloadType !== "TrustSet"){
      const hasTrustline = await checkTrustline(addressToUse);
    if (hasTrustline) {
      if (payload._timestamp <= fiveMinutesAgo) {
        expired = true;
        const xummDetailedResponse = {
          meta: {
            TrustLineNotSet: false,
            exists: true,
            uuid: payloadId,
            signed: payload.isSigned, 
            submit: false,
            resolved: false,
            expired: expired,
          },
          custom_meta: {
           blob: payload.customMetablob 
          },
          response: {
            hex: "timeStampExpired",
            txid: "NoTx",
            account: "NoAccount"
          }
        };
        if (!pendingPayloadIds.some(item => item === payload)) {
          pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
        }
        console.log("Sending xummDetailedResponse: EXPIRED TX w/trustline ", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object
      
        return res.json(xummDetailedResponse);
       }
      if(!payload.isSigned){
        const xummDetailedResponse = {
          meta: {
            TrustLineNotSet: false,
            exists: true,
            uuid: payloadId,
            signed: false, 
            submit: false,
            resolved: false,
            expired: expired,
          },
          custom_meta: {
           blob: payload.customMetablob 
          },
          response: {
            hex: payloadInfo.data.response.hex,
            txid: payload.txid,
            account: payloadInfo.data.response.account
          }
        };
        console.log("Sending xummDetailedResponse: NON SIGNER THEY CANCELLED w/trustline ", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object
      
        return res.json(xummDetailedResponse);
       }
      console.log("The account has the required trustline.");
      // Perform your logic here
    } else {
      if (payload._timestamp <= fiveMinutesAgo) {
        expired = true;
        const xummDetailedResponse = {
          meta: {
            TrustLineNotSet: true,
            exists: true,
            uuid: payloadId,
            signed: payload.isSigned, 
            submit: false,
            resolved: false,
            expired: expired,
          },
          custom_meta: {
           blob: payload.customMetablob 
          },
          response: {
            hex: "timeStampExpired",
            txid: "NoTx",
            account: "NoAccount"
          }
        };
        if (!pendingPayloadIds.some(item => item === payload)) {
          pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
        }
        console.log("Sending xummDetailedResponse: EXPIRED TX no trustline ", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object
      
        return res.json(xummDetailedResponse);
       }
      if(!payload.isSigned && addressToUse !== null){
        const xummDetailedResponse = {
          meta: {
            NoAddressSendBackToMain: false,
            TrustLineNotSet: true,
            exists: true,
            uuid: payloadId,
            signed: false, 
            submit: false,
            resolved: false,
            expired: expired,
          },
          custom_meta: {
           blob: payload.customMetablob 
          },
          response: {
            hex: payloadInfo.data.response.hex,
            txid: payload.txid,
            account: payloadInfo.data.response.account
          }
        };
        
        console.log("Sending xummDetailedResponse: NON SIGNER THEY CANCELLED and no trustline !", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object
        if (!pendingPayloadIds.some(item => item === payload)) {
          pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
        }
        return res.json(xummDetailedResponse);
        
       } else {
        if(!payload.isSigned && addressToUse === null){
          const xummDetailedResponse = {
            meta: {
              NoAddressSendBackToMain: true,
              exists: true,
              uuid: payloadId,
              signed: false, 
              submit: false,
              resolved: false,
              expired: expired,
            },
            custom_meta: {
             blob: payload.customMetablob 
            },
            response: {
              hex: payloadInfo.data.response.hex,
              txid: payload.txid,
              account: payloadInfo.data.response.account
            }
          };
          console.log("Sending xummDetailedResponse: NON SIGNER THEY CANCELLED sending back to the registration page !", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object
          if (!pendingPayloadIds.some(item => item === payload)) {
            pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
          }
          return res.json(xummDetailedResponse);
        }
        
       }
      console.log("The account does not have the required trustline.");
      const xummDetailedResponse = {
        meta: {
          TrustLineNotSet: true,
          exists: true,
          uuid: payloadId,
          signed: false, 
          submit: false,
          resolved: false,
          expired: expired,
        },
        custom_meta: {
         blob: payload.customMetablob 
        },
        response: {
          hex: payloadInfo.data.response.hex,
          txid: payload.txid,
          account: payloadInfo.data.response.account
        }
      };
      if (!pendingPayloadIds.some(item => item === payload)) {
        pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
      }
      console.log("Sending xummDetailedResponse: NO TRUSTLINE ", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object

      return res.json(xummDetailedResponse);
    }
    }
    
    if(!payload.isSigned && payloadType === "TrustSet"){
      const xummDetailedResponse = {
        meta: {
          exists: true,
          uuid: payloadId,
          signed: false, 
          submit: false,
          resolved: false,
          expired: expired,
        },
        custom_meta: {
         blob: payload.customMetablob 
        },
        response: {
          hex: payloadInfo.data.response.hex,
          txid: payload.txid,
          account: payloadInfo.data.response.account
        }
      };
      console.log("Sending xummDetailedResponse: NON SIGNER THEY CANCELLED no trustline ", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object
      if (!pendingPayloadIds.some(item => item === payload)) {
        pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
      }
      return res.json(xummDetailedResponse);
     }

    if(walletAddress !== payloadInfo.data.response.signer && walletAddress !== "Undefined"){
      //we need to discard this payload 
      const xummDetailedResponse = {
        meta: {
          wrongSigner: true,
          exists: true,
          uuid: payloadId,
          signed: false, 
          submit: false,
          resolved: false,
          expired: expired,
        },
        custom_meta: {
         blob: payload.customMetablob 
        },
        response: {
          hex: payloadInfo.data.response.hex,
          txid: payload.txid,
          account: payloadInfo.data.response.account
        }
      };
     
      if (!pendingPayloadIds.some(item => item === payload)) {
        pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
      }
      console.log("BAD SIGNER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
      console.log("Sending xummDetailedResponse: WRONG SIGNER ", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object

      return res.json(xummDetailedResponse);

    }
  } else {
    console.log("Could not retrieve payload info");
  }
  console.log("GOOD TO GO SEND IT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
  const xummDetailedResponse = {
    meta: {
      exists: true,
      uuid: payloadId,
      signed: payload.isSigned, 
      submit: false,
      resolved: true,
      expired: expired,
    },
    custom_meta: {
     blob: payload.customMetablob 
    },
    response: {
      hex: payloadInfo.data.response.hex,
      txid: payload.txid,
      account: payloadInfo.data.response.account
    }
  };
  console.log("Sending xummDetailedResponse: GOOD RESPOSNE THIS WAS CORRECT!! ", JSON.stringify(xummDetailedResponse, null, 2)); // Log the object
  if (!pendingPayloadIds.some(item => item === payload)) {
    pendingPayloadIds = pendingPayloadIds.filter(item => item !== payload);
  }
  res.json(xummDetailedResponse);
});
async function checkTrustline(account) {
  // Connect to XRPL
  const client = new xrpl.Client('wss://xrplcluster.com')
  await client.connect()

  // Fetch account lines (trustlines)
  const accountLines = await client.request({
    command: 'account_lines',
    account: account,
  })

  await client.disconnect()

  // Check if the account has the specified trustline
  const trustlines = accountLines.result.lines || []
  for (const line of trustlines) {
    if (line.currency === "DKP" && line.account === "rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J") {
      return true // Trustline found
    }
  }

  return false // Trustline not found
}
app.post('/balance', async (req, res, next) => {
  //this assumes the json object coming in is called wallet
  var wallet = req.get(req.body.wallet);
  console.log(req.body.wallet);
  //no longer in the xumm world. we doing straight comms to xrpl
  const client = new xrpl.Client("wss://xrplcluster.com")
  await client.connect()
  console.log('We are listening')
  // Get info from the ledger about the address we just funded
  const response = await client.request({
    "command": "account_lines",
    "account": req.body.wallet,
    "ledger_index": "validated",
    "peer": "rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J"
})
var json = {};
json.balance = response.result.lines[0].balance;
json.currency = response.result.lines[0].currency;
res.send(response.result.lines[0].balance);
console.log("Client has " + response.result.lines[0].balance + " DKP in their wallet.");
client.disconnect()
}
);
app.listen(PORT, function(error){
  if (!error) 
  console.log("Server listening on Port", PORT);
  else
  console.log("Error Occured");
})
