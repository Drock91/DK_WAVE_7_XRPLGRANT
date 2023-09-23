# DK_WAVE_7_XRPLGRANT
TABLE OF CONTENTS
  -

* Overview

* Features

* Technologies Used

* Architecture

* Future Plans

* Getting Started

* Contributing

* License

* Contact


OVERVIEW
-
Welcome to DragonKill.online, an innovative 2D fantasy Massively Multiplayer Online Role-Playing Game (MMORPG) that incorporates Play-to-Earn mechanisms and blockchain technology. Designed with a decentralized architecture, our metaverse offers an unparalleled, secure, and reliable gaming experience. In DragonKill, players have the opportunity to earn DKP tokens, which are seamlessly tradeable on the XRP Ledger decentralized exchange for a variety of currencies. Additionally, DKP can be converted into in-game gold, allowing players to engage with our in-game marketplace and cover associated gameplay costs.


FEATURES
  -

* Server-Authority Mechanism: Enabled by Mirror networking, ensuring synchronized and trustworthy game states.

* Currency and Assets: Utilize playfab.com-generated currencies that integrate with XRP Ledger.

* Multi-platform Support: Aimed for browser-based gaming experiences, -currently standalonendows.

TECHNOLOGIES USED
-

* Heroku App: Central controller for game servers. Responsible for scaling and load balancing.

* Unity 2020.3: The game engine behind the metaverse.

* Mirror Networking: Handles server authority for reliable information synchronization.

* XRP Ledger & Xumm Wallet: For cryptocurrency and asset management.

* xrpscan API: Fetches balances and NFT data.


ARCHITECTURE
-
* Heroku node.js App - Contains index.js-Includes necessary XRPL libraries for crypto signing 
  
  - index.js contains market order purchasing for registration if player needs to purchase DKP, it also has Xumm webhooks and other various calls to the XRP Ledger to include sending DKP to the player for transmuting in-game gold currency generated by playfab to DKP

* Unity 2020.3 - Game engine where the metaverse is constructed
  
  - PlayFabClient.cs - network manager for client
  
  - PlayFabServer.cs network manager for game server
  
  - ScenePlayer.cs networked player script

  - PlayerCharacter.cs networked character script

  - Mob.cs networked enemy script

  - MovingObject.cs inheritted class from PlayerCharacter and Mob so they can speak to each other easily and share same qualities with polymorphism

  - MatchMaking.cs script for generating different matches with players in it on the server

  - Door.cs script for opening networked doors

  - ArmorDrop.cs used for looting armor stands that are networked

  - MainChest.cs script used to deliver in-game currency to players so they can convert to DKP XLS-20 token or spend on the market in-game.

  - WeaponDrop.cs used for looting weapon stands that are networked

  - TrapDrop networked code for damaging characters

* Mirror Networking - Ensures server authority and information reliability

* dragonkill.online Website - Placeholder website currently, will houses the WebGL build for browser-based gameplay (future)

* XRP Ledger & Xumm Wallet - Manage and store game currencies

  - XRP Ledger code can be found specifically in the following scripts on the provided code line. 
  - [PlayFabServer.cs](PlayFabServer.cs)
    -
    - Method VerifyPlayerPayment line 469, used to determine if a player has already registered with an XRP wallet before registering a wallet. 
    - Method GetTransactionHistory line 496, used with VerifyPlayerPayment to get tx history of our registration xrp address to check if their wallet has paid before for registration. if it has its rejected
    - Method SubmitBlobToXRPL line 1536, used to submit a blob for processing in the XRPL
    - Method PurchaseDKPMarketPriceRegistration line 991, used to get best price of DKP from our index.js script on our heroku app using xrpl library.
    - Method RegisterTrustSet line 584, used for setting a trust line with DKP on registration into the game
    - Method CheckXummStatusAPP line 1728, used to poll our heroku app for the Xumm webhook callback. 
    - Method DKPTOGOLDTRANSMUTE line 1752, used to transmute DKP XLS-20 token to in-game gold the playfab virtual currency. 
  - Index.js
    - 
  


FUTURE PLANS
-
Transition from Windows containers to Linux containers for better transport interoperability.
WebGL build to enable browser-based gameplay. House and guild plot NFT integration into main town, sandbox marketplace to link multiple game servers for one world effect. There is so much more to come!


CONTRIBUTING
-
We appreciate all contributions. See CONTRIBUTING.md for details.


LICENSE
-
This project is licensed under the MIT License - see the LICENSE.md file for details.


CONTACT
-
Project Maintainer: [Derek Heinrichs]

Email: [MrHeinrichs12@gmail.com]

Project Link: [dragonkill.online]
