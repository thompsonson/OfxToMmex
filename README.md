OfxToMmex
=========

Project to parse [OFX] (http://www.ofx.net/) files and import into [Money Manager Ex] (http://sourceforge.net/projects/moneymanagerex/)- please note it's work in progress.

__Overview__

* Command line MONO application (currently only tested on MAC OS 10.9)
* Imports all details, creating Account, Payee and Transaction where applicable. 
* Can be configured to perfrom regular expressions on the Payees to remove unwanted details (e.g. unique references that create unwanted multiple payees). 

__Libraries Used:__

* [OFXSharp] (https://github.com/thompsonson/OFXSharp) (to read the OFX data)
* [PetaPoco] (https://github.com/toptensoftware/PetaPoco) (as ORM to MMEX sqlite db)

__On the wishlist:__

* Process payees already in DB - enter a regex and have payee info updated
* Edit default Category information for payees

__Wider wishlist:__

This in now for MMEX in general... 

* Edit share price details (have historical data and display as a graph...)
* Display net worth over time
* Have loan and asset details (e.g. House value and mortgage, Car value and loan with balloon payment).

__small footnote__

I've changed this a bit from when I first put it together. Then I was on a windows only environment and very intersted in self contained services. Since then I only use windows at work (Windows 7 laptop broke and was replaced with Mac Air, as you do... :)). So I've finally got around to moving it to Mono and had to change the architecture a bit. I currently think this is good, if I make smaller components to do what I want there's less bloat and easier to hack each one!





