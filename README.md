OfxToMmex
=========

Project to parse [OFX] (http://www.ofx.net/) files and import into [Money Manager Ex] (http://sourceforge.net/projects/moneymanagerex/)- please note it's work in progress.

# Overview

* __this doesn't currently work with encrypted MMEX DBs__
* Command line MONO application (currently only tested on MAC OS 10.9)
* Imports all details, creating Account, Payee and Transaction where applicable. 
* Can be configured to perfrom regular expressions on the Payees to remove unwanted details (e.g. unique references that create unwanted multiple payees). 

# Pre-Requistes

**Please note that it requires a small change to the DB schema**

> sqlite3 Test.mmb "ALTER TABLE CHECKINGACCOUNT_V1 ADD COLUMN FITID STRING;"

> sqlite3 Test.mmb "CREATE TABLE OfxToMmexPayeeNameRegex(ID INTEGER PRIMARY KEY,regex STRING,GroupIndex INTEGER, Active INTEGER);"

The FITID column is used to ensure a transaction is imported just once (each FITID is unique to the account)

# Config 

edit __OfxToMmexConsoleApp.exe.config__ change the following two entries:

```
 <add name="mmex_db" connectionString="Data Source=./Test.mmb;" providerName="Mono.Data.SQLite"/> 
```

```
 <add key="log4net" value="./log4net.config"></add>
```

to point at your MMEX DB file and where the log4net.config is, it's the same folder as the exe.

# Importing the ofx statement

> mono OfxToMmexConsoleApp.exe my_bank_statement.ofx

# Libraries Used:

* [OFXSharp] (https://github.com/thompsonson/OFXSharp) (to read the OFX data)
* [PetaPoco] (https://github.com/toptensoftware/PetaPoco) (as ORM to MMEX sqlite db)

# On the wishlist:

* Process payees already in DB - enter a regex and have payee info updated
* Edit default Category information for payees

# Wider wishlist:

This in now for MMEX in general (see footnote)... 

* Edit share price details (have historical data and display as a graph...)
* Display net worth over time
* Have loan and asset details (e.g. House value and mortgage, Car value and loan with balloon payment).

## small footnote

I've changed this a bit from when I first put it together. Then I was on a windows only environment and very intersted in self contained services. Since then I only use windows at work (Windows 7 laptop broke and was replaced with Mac Air, as you do... :)). So I've finally got around to moving it to Mono and had to change the architecture a bit. I currently think this is good, if I make smaller components to do what I want there's less bloat and easier to hack each one!



