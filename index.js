var express = require('express');
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
const Sdk = new XummSdk(process.env.XUMM_PUBLIC, process.env.XUMM_PRIVATE)
app.use('/transmute', limiter);
app.use('/register', limiter);
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
app.use(express.json());
const pendingQueue = [];
let currentRequests = 0;
const MAX_REQUESTS = 95;
const WINDOW_MS = 60 * 1000; // 1 minute

const release = () => {
  const numToRelease = Math.min(pendingQueue.length, MAX_REQUESTS - currentRequests);
  for (let i = 0; i < numToRelease; i++) {
    const nextRequest = pendingQueue.shift();
    nextRequest();
  }
  currentRequests = Math.max(0, currentRequests - MAX_REQUESTS);
};

// Release 95 requests every minute
setInterval(release, WINDOW_MS);

app.post('/xummqueue', (req, res) => {
  if (currentRequests < MAX_REQUESTS) {
    currentRequests++;
    // Your processing logic here
    res.status(200).send("Request processed");
  } else {
    pendingQueue.push(() => {
      currentRequests++;
      // Your processing logic here
      res.status(200).send("Request processed");
    });
  }
});
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
app.post('/xumm-webhook', async (req, res) => {
  const timestamp = req.headers['x-xumm-request-timestamp'] || '';
    const json = req.body;

    const hmac = crypto.createHmac('sha1', process.env.XUMM_PRIVATE.replace('-', ''))
      .update(timestamp + JSON.stringify(json))
      .digest('hex');

    if (hmac !== req.headers['x-xumm-request-signature']) {
      console.warn('Signature mismatch. Possible tampering detected.');
      return res.status(401).send('Unauthorized');
    }
  const payloadId = req.body.meta.payload_uuidv4;
  const verifying = await Verify.getOne(payloadId)
  if(verifying){
    const _timestamp = Date.now();
    const isSigned = req.body.payloadResponse.signed;
    const customMetablob = req.body.custom_meta.blob;
    // You can push additional information to your pendingPayloadIds array if needed.
    pendingPayloadIds.push({ payloadId, _timestamp, isSigned, customMetablob });
    res.status(200).send("OK");
  }
  
});
// Cleanup old entries every 5 minutes
setInterval(() => {
  const fiveMinutesAgo = Date.now() - (5 * 60 * 1000);
  pendingPayloadIds = pendingPayloadIds.filter(item => item.timestamp > fiveMinutesAgo);
}, 5 * 60 * 1000);

// Unity server check endpoint
app.get('/check-payload/:payloadId', (req, res) => {
  const { payloadId } = req.params;
  if (!payload) {
    return res.json(null);
  }
  const xummDetailedResponse = {
    meta: {
      exists: true,
      uuid: payloadId,
      signed: payload.signed || false, // These are placeholders; replace with real data
      submit: false,
      resolved: true,
      expired: payload.expired || false,
    },
    custom_meta: {
     blob: payload.customMetablob // Fill this in from the stored data
    },
    response: {
      hex: payload.hex || "", // These are placeholders; replace with real data
      txid: payload.txid || "",
      account: payload.account || ""
    }
  };

  res.json(xummDetailedResponse);
});
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
