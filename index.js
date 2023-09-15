var express = require('express');
var router = express.Router();
const app = express();
const bodyParser = require('body-parser');
const {TxData} = require('xrpl-txdata');
const xrpl = require("xrpl");
require('dotenv').config();
const { response } = require('express');
//no longer in the xumm world. we doing straight comms to xrpl
const Verify = new TxData();
var PORT = process.env.PORT || 3000;
//const bodyParser = require('body-parser');
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
app.use(express.json());
//let currentSequence;
//
//async function getInitialSequence() {
//  const client = new xrpl.Client("wss://xrplcluster.com");
//  await client.connect();
//
//  const response = await client.request({
//    command: 'account_info',
//    account: process.env.SENDER_PUBLIC,
//  });
//
//  currentSequence = response.result.account_data.Sequence;
//  await client.disconnect();
//}
//
//getInitialSequence(); // Call this function when the server starts

//app.use(bodyParser.urlencoded({ extended: false }));
app.post('/dkpsend', async (req, res, next) => {
  //this assumes the json object coming in is called wallet
  console.log('We are listening')
 // console.log(req.body.userId + req.body.walletAddress + req.body.goldAmount);
  console.log(req.body.userId);

  //res.status(200).json({ message: 'Transaction and gold removal successful' });
  console.log(req.body.userId + req.body.walletAddress + req.body.goldAmount);
  //no longer in the xumm world. we doing straight comms to xrpl
  const client = new xrpl.Client("wss://xrplcluster.com");
  await client.connect();
  
  const dkpWallet = xrpl.Wallet.fromSeed(process.env.SENDER_SEED);
  const currency_code = "DKP"
  // Send token ----------------------------------------------------------------
  //this issue quantity is up to you. you can use math to determine what to send or just send a fixed amount
  const issue_quantity = req.body.goldAmount
  const send_token_tx = {
    "TransactionType": "Payment",
    "Account": process.env.SENDER_PUBLIC,
    "Amount": req.body.goldAmount,
    //"Amount": {
    //  "currency": currency_code,
    //  "value": issue_quantity,
    //  "issuer": "rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J"
    //},
    "Destination": req.body.walletAddress
  }
  const pay_prepared = await client.autofill(send_token_tx)
  const pay_signed = dkpWallet.sign(pay_prepared)
  const pay_result = await client.submitAndWait(pay_signed.tx_blob)
  console.log(pay_result);
  if (pay_result.result.meta.TransactionResult == "tesSUCCESS") {
    console.log(`Transaction succeeded: https://mainnet.xrpl.org/transactions/${pay_signed.hash}`)
    const userId = req.body.userId; 
    const goldAmount = req.body.goldAmount; 
    const responseObj = {
      success: 'true',
      message: 'Transaction and gold removal successful',
      details: {
          userId: userId,
          goldAmount: goldAmount,
          walletAddress: req.body.walletAddress
      }
    };
    
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
    }
  };
  res.status(200).json(responseObj);
  console.log(responseObj);
  throw `Error sending transaction: ${pay_result.result.meta.TransactionResult}`
}
client.disconnect()
});
app.post('/tokenpurchase', async (req, res, next) => {
  //this assumes the json object coming in is called wallet
  var wallet = req.get(req.body.wallet);
  console.log(req.body.wallet);
  //no longer in the xumm world. we doing straight comms to xrpl
  const client = new xrpl.Client("wss://xrplcluster.com")
  await client.connect()
  console.log('We are listening')
  //this is rHB5rfSutbJX6NByXhH7ggTb93M7Jw7jCe wallet.
  const dkpWallet = xrpl.Wallet.fromSeed(process.env.SENDER_SEED);
  const currency_code = "DKP"
  // Send token ----------------------------------------------------------------
  const issue_quantity = "5"
  const send_token_tx = {
    "TransactionType": "Payment",
    "Account": process.env.SENDER_PUBLIC,
    "Amount": "10000",
    //"Amount": {
    //  "currency": currency_code,
    //  "value": issue_quantity,
    //  "issuer": "rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J"
    //},
    "Destination": (dkpWallet)
  }
  const pay_prepared = await client.autofill(send_token_tx)
  const pay_signed = dkpWallet.sign(pay_prepared)
  const pay_result = await client.submitAndWait(pay_signed.tx_blob)
  console.log(pay_result);
  if (pay_result.result.meta.TransactionResult == "tesSUCCESS") {
    console.log(`Transaction succeeded: https://mainnet.xrpl.org/transactions/${pay_signed.hash}`)
} else {
    throw `Error sending transaction: ${pay_result.result.meta.TransactionResult}`

}
client.disconnect()
}
  
);
app.post('/createAccountWalletSignIn', async (req, res, next) => {
  //this assumes the json object coming in is called wallet
  var wallet = req.get(req.body.wallet);
  console.log(req.body.wallet);
  //no longer in the xumm world. we doing straight comms to xrpl
  const client = new xrpl.Client("wss://xrplcluster.com")
  await client.connect()
  console.log('We are listening')
  //this is rHB5rfSutbJX6NByXhH7ggTb93M7Jw7jCe wallet.
  const currency_code = "DKP"
  // Send token ----------------------------------------------------------------
  const issue_quantity = "5"
  const send_token_tx = {
    "TransactionType": "SignIn",
    "Account": req.body.wallet,
    "Amount": {
      "currency": currency_code,
      "value": issue_quantity,
      "issuer": "rM7zpZQBfz9y2jEkDrKcXiYPitJx9YTS1J"
    },
    "Destination": (dkpWallet)
  }
  const pay_prepared = await client.autofill(send_token_tx)
  const pay_signed = dkpWallet.sign(pay_prepared)
  const pay_result = await client.submitAndWait(pay_signed.tx_blob)
  console.log(pay_result);
  if (pay_result.result.meta.TransactionResult == "tesSUCCESS") {
    console.log(`Transaction succeeded: https://mainnet.xrpl.org/transactions/${pay_signed.hash}`)
} else {
    throw `Error sending transaction: ${pay_result.result.meta.TransactionResult}`

}
client.disconnect()
}
  
);
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
