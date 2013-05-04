using log4net;
using Nancy.Hosting.Self;
using OFXSharp;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OfxToMmex.App
{
    public class Service
    {
        public static string RootPath { get; set; }
        public static string MonitorPath { get; set; }
        public static string ProcessingPath { get; set; }
        public static string ProcessedPath { get; set; }

        private static FileSystemWatcher watcher;
        private static readonly ILog log = LogManager.GetLogger(typeof(Service));
        private static NancyHost host;

        public static void ChangeWatchingFolder()
        {
            RootPath = Model.Config.Rootpath;
            log.Info("Changing the path to watch to " + RootPath);
            MonitorPath = System.IO.Path.Combine(RootPath, "Monitor");
            ProcessingPath = System.IO.Path.Combine(RootPath, "Processing");
            ProcessedPath = System.IO.Path.Combine(RootPath, "Processed");
            // check the folder exists
            if (!System.IO.File.Exists(RootPath))
                System.IO.Directory.CreateDirectory(RootPath);
            if (!System.IO.File.Exists(MonitorPath))
                System.IO.Directory.CreateDirectory(MonitorPath);
            if (!System.IO.File.Exists(ProcessingPath))
                System.IO.Directory.CreateDirectory(ProcessingPath);
            if (!System.IO.File.Exists(ProcessedPath))
                System.IO.Directory.CreateDirectory(ProcessedPath);
            watcher.Path = MonitorPath;
        }
        public static void StartWatchingFolder()
        {
            try
            {
                log.Info("Setting up folders and file system watcher");
                //create a filesystemwatcher class instance for monitoring a physical file system directory
                watcher = new FileSystemWatcher();
                watcher.Created += new FileSystemEventHandler(watcher_Created);
                //provide a path to instance for monitoring
                RootPath = Model.Config.Rootpath;
                MonitorPath = System.IO.Path.Combine(RootPath, "Monitor");
                ProcessingPath = System.IO.Path.Combine(RootPath, "Processing");
                ProcessedPath = System.IO.Path.Combine(RootPath, "Processed");
                // check the folder exists
                if (!System.IO.File.Exists(RootPath))
                    System.IO.Directory.CreateDirectory(RootPath);
                if (!System.IO.File.Exists(MonitorPath))
                    System.IO.Directory.CreateDirectory(MonitorPath);
                if (!System.IO.File.Exists(ProcessingPath))
                    System.IO.Directory.CreateDirectory(ProcessingPath);
                if (!System.IO.File.Exists(ProcessedPath))
                    System.IO.Directory.CreateDirectory(ProcessedPath);
                watcher.Path = MonitorPath;
                //start the monitor
                watcher.EnableRaisingEvents = true;

            }
            catch (Exception ex)
            {
                log.Fatal("Failed to set up the folders and file system watcher");
                throw new OfxToMmex.OfxToMmexException("Failed to set up the folders and file system watcher", ex);
            }
        }

        public void Start()
        {
            
            StartWatchingFolder();

            log.Info("Config loaded, watching folder (" + MonitorPath + ")... hit enter to quit");
            try
            {
                // starting the Nancy host
                log.Info("Starting the Nancy host");

                // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
                string url = "http://localhost:8080";
                host = new NancyHost(new Uri(url));
                host.Start(); // start hosting

                log.Info("Nancy started - url: " + url);

                //Config.Rootpath = "d:\\temp\\ofxtommex";
            }
            catch (Exception ex)
            {
                log.Fatal("Failed to start the Nancy Self Hosted service");
                throw new OfxToMmex.OfxToMmexException("Failed to start the Nancy Self Hosted service", ex);
            }

        }

        public void Stop() {
            log.Info("Exiting...");
            host.Stop();  // stop hosting
        }

        private static void ImportOfxToMmex(OFXDocument OfxDocument)
        {
            // Create a PetaPoco database object
            var db = new PetaPoco.Database("mmex_db");
            log.Info("PetPoco DB started");

            // Show all Accounts    
            foreach (var a in db.Query<Model.Accounts>("SELECT * FROM ACCOUNTLIST_V1;"))
            {
                log.Info(a.ACCOUNTID + " - " + a.ACCOUNTNUM + " - " + a.ACCOUNTNAME);
            }

            log.Info("-------------------------------------------------------------------------");
            log.Info(OfxDocument.Statements.Count() + " statements in the ofx file");
            log.Info("-------------------------------------------------------------------------");

            foreach (OFXStatement ofxstatment in OfxDocument.Statements)
            {
                log.Info("Account # " + ofxstatment.Account.AccountID);

                var account = db.SingleOrDefault<Model.Accounts>("SELECT * FROM ACCOUNTLIST_V1 WHERE ACCOUNTNUM=@0", ofxstatment.Account.AccountID);
                if (account == null)
                {
                    log.Info("Creating the account");
                    //get the currency ID
                    var currency = db.SingleOrDefault<Model.Currencies>("SELECT * FROM CURRENCYFORMATS_V1 WHERE CURRENCY_SYMBOL=@0", ofxstatment.Currency); //(CURRENCY_SYMBOL: document.CURDEF);
                    // defaulting to GBP if no currency
                    int currency_id = 6;
                    if (currency != null)
                        currency_id = currency.CURRENCYID;
                    // create the account 
                    account = new Model.Accounts();
                    account.ACCOUNTNUM = ofxstatment.Account.AccountID;
                    if (ofxstatment.Account.AccountType == OFXSharp.AccountType.BANK)
                    {
                        account.ACCOUNTTYPE = "Checking"; // document.ACCTTYPE;
                    }
                    else
                    {
                        account.ACCOUNTTYPE = "Term"; // document.ACCTTYPE;
                    }
                    account.ACCOUNTNAME = "Automated Import of account #" + ofxstatment.Account.AccountID;
                    account.STATUS = "Open";
                    account.HELDAT = "Bank ID - " + ofxstatment.Account.BankID;
                    account.INITIALBAL = 0;
                    account.FAVORITEACCT = "TRUE";
                    account.CURRENCYID = currency_id;
                    // updates ACCOUNTID in NewAccount as well as Inserting the account
                    db.Insert(account);
                }

                foreach (OFXSharp.Transaction trans in ofxstatment.Transactions)
                {
                    // check if the transaction is already present
                    log.Info("Checking for Transaction: " + trans.TransactionID);
                    var transaction_count = db.SingleOrDefault<long>("SELECT count(*) FROM CHECKINGACCOUNT_V1 WHERE FITID=@0 AND ACCOUNTID=@1", trans.TransactionID, account.ACCOUNTID);
                    if (transaction_count == 0)
                    {
                        log.Info("Inserting Transaction: " + trans.TransactionID);
                        // get the payeeName
                        string payeeName = ProcessPayee.processAll(db, trans.Name);
                        /*/ get Payee ID and default Categories
                        Regex re = new Regex(@"REF");
                        string[] arrPayee = re.Split(trans.Name);
                        string payeeName = arrPayee[0].Trim();
                        //*/
                        log.Info("Checking for Payee: " + payeeName);
                        var payee = db.SingleOrDefault<Model.Payee>("SELECT * FROM PAYEE_V1 WHERE PAYEENAME=@0", payeeName);

                        if (payee == null)
                        {
                            log.Info("Inserting Payee: " + payeeName);
                            // create the payee
                            payee = new Model.Payee();
                            payee.PAYEENAME = payeeName;
                            payee.CATEGID = -1;
                            payee.SUBCATEGID = -1;
                            // updates PAYEEID in payee as well as Inserting the payee
                            db.Insert(payee);
                        }

                        var newTrans = new Model.Transactions();
                        newTrans.ACCOUNTID = account.ACCOUNTID;
                        newTrans.PAYEEID = payee.PAYEEID;
                        if (trans.Amount > 0)
                        {
                            newTrans.TRANSCODE = "Deposit";
                        }
                        else
                        {
                            newTrans.TRANSCODE = "Withdrawal";
                        }
                        newTrans.TRANSAMOUNT = Math.Abs(trans.Amount);
                        newTrans.STATUS = "F";
                        newTrans.NOTES = trans.Name;
                        newTrans.CATEGID = payee.CATEGID;
                        newTrans.SUBCATEGID = payee.SUBCATEGID;
                        newTrans.TRANSDATE = trans.Date.ToString("yyyy-MM-dd");
                        newTrans.FOLLOWUPID = -1;
                        newTrans.FITID = trans.TransactionID;
                        db.Insert(newTrans);

                        // **/
                    } // else do nothing
                    else
                    {
                        log.Info("Transaction present");
                    }
                }

            }
            // Show all Accounts    
            foreach (var a in db.Query<Model.Accounts>("SELECT * FROM ACCOUNTLIST_V1;"))
            {
                log.Info(a.ACCOUNTID + " - " + a.ACCOUNTNUM + " - " + a.ACCOUNTNAME);
            }

        }

        private static void watcher_Created(object source, FileSystemEventArgs e)
        {
            try
            {
                var db = new PetaPoco.Database("mmex_db");
                //fullpath
                log.Info("Importing file: " + e.FullPath);
                Model.Workflow wf = new Model.Workflow();
                wf.filename = e.FullPath;
                wf.WatcherCreateTS = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                db.Insert(wf);
                // need to wait for file to be copied in
                while (creationComplete(e.FullPath) == false)
                {
                    // need to timeout here...

                }
                wf.ProcessStartTS = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //copy the file
                string ProcessingFilePath = ProcessingPath + "\\" + "wfID." + wf.WorkflowID + "." + DateTime.Now.ToString("yyyyMMddHHmmss.") + e.Name;
                log.Debug("ProcessingfilePath = " + ProcessingFilePath);
                File.Move(e.FullPath, ProcessingFilePath);
                wf.ImportStartTS = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                db.Update(wf);
                OFXDocumentParser doc = new OFXDocumentParser();
                OFXDocument OfxDocument = new OFXDocument();
                OfxDocument = doc.Import(new FileStream(ProcessingFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None));
                log.Info("File has been read");
                wf.DBInsertStartTS = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                db.Update(wf);
                ImportOfxToMmex(OfxDocument);
                // move the file to processed folder
                string ProcessedFilePath = ProcessedPath + "\\" + "wfID." + wf.WorkflowID + "." + DateTime.Now.ToString("yyyyMMddHHmmss.") + e.Name;
                File.Move(ProcessingFilePath, ProcessedFilePath);
                wf.filename = ProcessingFilePath;
                wf.FinishTS = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                db.Update(wf);
                log.Info("Import to Money Manager EX DB complete");
            }
            catch (Exception ex)
            {
                log.Fatal("Failed whilst processing the file");
                throw new OfxToMmex.OfxToMmexException("Failed whilst processing the file", ex);
            }
        }

        private static bool creationComplete(string fileName)
        {
            // if the file can be opened it is no longer locked and now available
            try
            {
                using (FileStream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

    }
}
