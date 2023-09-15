require('dotenv').config();
const express = require('express');
const bodyParser = require('body-parser');
const xrpl = require("xrpl");
const PlayFab = require('./PlayFabSdk/Scripts/PlayFab/PlayFab');
const PlayFabServer = require('./PlayFabSdk/Scripts/PlayFab/PlayFabServer');

const PORT = process.env.PORT || 3000;

// Initialize PlayFab settings
PlayFab.settings.titleId = process.env.PLAYFAB_TITLE_ID;
PlayFab.settings.secretKey = process.env.PLAYFAB_SECRET_KEY;

let currentSequence;

// Initialize sequence number
async function getInitialSequence() {
  const client = new xrpl.Client("wss://xrplcluster.com");
  await client.connect();
  const response = await client.request({
    command: 'account_info',
    account: process.env.SENDER_PUBLIC,
  });
  currentSequence = response.result.account_data.Sequence;
  await client.disconnect();
}

const app = express();

app.use(express.json());
app.use(bodyParser.urlencoded({ extended: false }));

// Middleware for error handling
app.use(async (error, req, res, next) => {
  res.status(500).send({error: error.message});
});

// Endpoint to handle DKP send
app.post('/dkpsend', async (req, res) => {
  try {
    // Validate and process the request here
    // ...

    // Transaction logic
    // ...
    
    // Increment the sequence number
    currentSequence++;
    
    res.status(200).send({message: 'Transaction successful.'});
  } catch (error) {
    next(error);
  }
});

// Other endpoints go here
// ...

// Initialize server
app.listen(PORT, async () => {
  try {
    await getInitialSequence();
    console.log(`Server started on port ${PORT}`);
  } catch (error) {
    console.error(`Failed to initialize server: ${error.message}`);
  }
});