OfxToMmex
=========

Project to parse [OFX] (http://www.ofx.net/) files and import into [Money Manager Ex] (http://sourceforge.net/projects/moneymanagerex/)- please note it's work in progress.

__Overview__

* Monitors folder for OFX formatted files
* Imports all details, creating Account, Payee and Transaction where applicable. 
* Can be configured to perfrom regular expressions on the Payees to remove unwanted details (e.g. unique references that create unwanted multiple payees). 

__Libraries Used:__

* [OFXSharp] (https://github.com/thompsonson/OFXSharp) (to read the OFX data)
* [PetaPoco] (https://github.com/toptensoftware/PetaPoco) (as ORM to MMEX sqlite db)
* [Top Shelf] (https://github.com/phatboyg/Topshelf) (to run as a service)
* [Nancy] (https://github.com/NancyFx/Nancy) (to allow editing of config via a web interface)

__On the wishlist:__

* Edit app.config settings via web interface
* Process payees - enter a regex and have payee info updated
* Edit default Category information for payees

__Wider wishlist:__

* Edit share price details (have historical data and display as a graph...)
* Display net worth over time
* Have loan and asset details (e.g. House value and mortgage, Car value and loan with balloon payment).

