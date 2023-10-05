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
app.use('/transmute', limiter);
app.use('/register', limiter);
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
  const apiKeyFromRequest = req.headers['x-api-key'];
    const apiKeyFromEnv = process.env.CONVO_KEY;

    if (apiKeyFromRequest !== apiKeyFromEnv) {
        res.status(401).json({message: 'Unauthorized'});
        return;
    }
  const id = req.query.id;  // Extracting the ID from the URL query parameter 
  //id will be how we queue players to use the xumm queue, need to contact xumm for possible rate increase or possibly get multiple X apps to handle volitile requests
  if (id === undefined) {
    res.status(400).send("ID is required");
    return;
  }
  if (currentRequests < MAX_REQUESTS) {
    currentRequests++;
    res.status(200).send("Request processed");
  } else {
    res.status(400).send("ID is required");
  }
});
//api endpoint for transmuting gold to DKP 
app.post('/dkpsend', async (req, res, next) => {
  console.log('We are listening')
  const apiKeyFromRequest = req.headers['x-api-key'];
    const apiKeyFromEnv = process.env.CONVO_KEY;

    if (apiKeyFromRequest !== apiKeyFromEnv) {
        res.status(401).json({message: 'Unauthorized'});
        return;
    }
  if (!req.body.userId || !req.body.walletAddress || !req.body.goldAmount) {
    res.status(400).send("Bad Request: Missing required parameters.");
    return;
  }
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
  const apiKeyFromRequest = req.headers['x-api-key'];
    const apiKeyFromEnv = process.env.CONVO_KEY;

    if (apiKeyFromRequest !== apiKeyFromEnv) {
        res.status(401).json({message: 'Unauthorized'});
        return;
    }
    // Parse the desired amount from the request body and convert it to a float
  const desiredAmount = parseFloat(req.body.amount);  // Assuming amount is passed as a string
    // Initialize an XRPL client and connect to it
  const client = new xrpl.Client('wss://xrplcluster.com');
  await client.connect();
   // Request the order book for XRP to DKP
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
    // Initialize variables to hold aggregate liquidity and best rate
  let aggregateLiquidity = 0;
  let bestRate = 0;
    // Loop through sorted offers to find the best rate that can fulfill the desired amount
  for (const offer of orderBook.result.offers) {
    const availableDKP = parseFloat(offer.TakerGets.value);
    aggregateLiquidity += availableDKP;
    if (aggregateLiquidity >= desiredAmount) {
      bestRate = parseFloat(offer.quality);
      break;
    }
  }
   // Check if there's enough liquidity to fulfill the request
  if (bestRate === 0) {
    console.log('Not enough liquidity to fulfill 50,000 DKP.');
    res.json({ meta: { error: 'Not enough liquidity' } });
    return;
  }
  const bestMarketPrice = ((bestRate / 1000000 )* desiredAmount).toString();
  console.log(bestMarketPrice + " was our best market price");
  //sending the best market price for the amount we have requested, this wil be created in the game server now via xumm rest api
  const xummDetailedResponse = {
    meta: {
      bestMarketPrice: bestMarketPrice,
    }
  };
  res.json(xummDetailedResponse);
});
function calculateBestMarketPrice(offers, targetAmount) {
  let remainingDKP = targetAmount;
  let totalXRP = 0;

  // Sort the offers by rate in ascending order
  //offers.sort((a, b) => parseFloat(a.quality) - parseFloat(b.quality));
  offers.sort((a, b) => parseFloat(b.quality) - parseFloat(a.quality));

  for (const offer of offers) {
    const availableDKP = parseFloat(offer.TakerGets.value);
    const rate = parseFloat(offer.quality);

    console.log(`Evaluating offer: Available DKP: ${availableDKP}, Rate: ${rate}`);

    if (remainingDKP <= 0) break;

    const buyAmount = Math.min(availableDKP, remainingDKP);
    const costInXRP = buyAmount * rate;

    remainingDKP -= buyAmount;
    totalXRP += costInXRP;

    console.log(`Buying amount: ${buyAmount}, Cost in XRP: ${costInXRP}`);
    console.log(`Remaining DKP after this offer: ${remainingDKP}, Total XRP so far: ${totalXRP}`);
  }

  if (remainingDKP > 0) {
    console.log('Not enough DKP offers to fulfill the order.');
    return null;
  }

  const bestMarketPrice = totalXRP / targetAmount;
  console.log(`Final best market price: ${bestMarketPrice}`);

  return (bestMarketPrice * 100).toString();
}
  app.get('/check-payload/:payloadId/:walletAddress', async(req, res) => {
    const apiKeyFromRequest = req.headers['x-api-key'];
    const apiKeyFromEnv = process.env.CONVO_KEY;

    if (apiKeyFromRequest !== apiKeyFromEnv) {
        res.status(401).json({message: 'Unauthorized'});
        return;
    }
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
app.listen(PORT, function(error){
  if (!error) 
  console.log("Server listening on Port", PORT);
  else
  console.log("Error Occured");
})

