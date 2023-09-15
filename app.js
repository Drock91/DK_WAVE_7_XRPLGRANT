const express = require('express');
const app = express();
const EventEmitter = require('events');

app.use(express.json());



app.post('/', function (req, res) {
    // this is where the input from unity is coming in, the body of the wallet
    // is the address we are seeing in the console
    console.log(req.body.wallet);
    // var  = req.body.wallet; 
    // trying to add the address variable from index.js 
    // to give it the new address coming from app.post
    return res.send('You sent use a wallet of');
});
app.listen(3000, function(err){
    if (err) console.log("Error in server setup")
    console.log("Server listening on Port", 3000);
    console.log(err);
})