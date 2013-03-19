OfxToMmex
=========

Project to parse OFX files and import into Money Manager Ex - please note it's Alpha\Work in progress currently.

Overview

Monitors folder for OFX formatted files
Imports all details, creating Account, Payee and Transaction where applicable. 
Can be configured to perfrom regular expressions on the Payees to remove unwanted details (e.g. unique references that create unwanted multiple payees). 


Uses:

OFXSharp  (to read the OFX data)
PetaPoco (as ORM to MMEX sqlite db)
Top Shelf (to run as a service)
Nancy (to allow editing of config via a web interface)

On the wishlist:

Edit app.config settings via web interface
Process payees - enter a regex and have payee info updated
Edit default Category information for payees

Wider wishlist:

Edit share price details (have historical data and display as a graph...)
Display net worth over time
Have loan and asset details (e.g. House value and mortgage, Car value and loan with balloon payment).



