using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OFXSharp;
using System.IO;
using System.Text.RegularExpressions;

namespace OfxToMmexConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Into Main");

            OFXDocumentParser doc = new OFXDocumentParser();

            OFXDocument OfxDocument = new OFXDocument();

            OfxDocument = doc.Import(new FileStream("D:\\Barclays.qbo", FileMode.Open));
            //OfxDocument = doc.Import(new FileStream("D:\\amex.qfx", FileMode.Open));// 

            Console.WriteLine("File imported");

            // Create a PetaPoco database object
            var db = new PetaPoco.Database("mmex_db");
            Console.WriteLine("PetPoco DB started");

            // Show all Accounts    
            foreach (var a in db.Query<Accounts>("SELECT * FROM ACCOUNTLIST_V1;"))
            {
                Console.WriteLine("{0} - {1} - {2}", a.ACCOUNTID, a.ACCOUNTNUM, a.ACCOUNTNAME);
            }

            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine(OfxDocument.Statements.Count() + " statements in the ofx file");
            Console.WriteLine("-------------------------------------------------------------------------");

            foreach (OFXStatement ofxstatment in OfxDocument.Statements)
            {
                Console.WriteLine("Account # " + ofxstatment.Account.AccountID);

                var account = db.SingleOrDefault<Accounts>("SELECT * FROM ACCOUNTLIST_V1 WHERE ACCOUNTNUM=@0", ofxstatment.Account.AccountID);
                if (account == null)
                {
                    Console.WriteLine("Creating the account");
                    //get the currency ID
                    var currency = db.SingleOrDefault<Currencies>("SELECT * FROM CURRENCYFORMATS_V1 WHERE CURRENCY_SYMBOL=@0", ofxstatment.Currency); //(CURRENCY_SYMBOL: document.CURDEF);
                    // defaulting to GBP if no currency
                    int currency_id = 6;
                    if (currency != null)
                        currency_id = currency.CURRENCYID;
                    // create the account 
                    account = new Accounts();
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
                    Console.WriteLine("Checking for Transaction: " + trans.TransactionID);
                    var transaction_count = db.SingleOrDefault<long>("SELECT count(*) FROM CHECKINGACCOUNT_V1 WHERE FITID=@0", trans.TransactionID);
                    if (transaction_count == 0)
                    {
                        Console.WriteLine("Inserting Transaction: " + trans.TransactionID);
                        // get Payee ID and default Categories
                        Regex re = new Regex(@"REF");
                        string[] arrPayee = re.Split(trans.Name);
                        string payeeName = arrPayee[0].Trim();
                        Console.WriteLine("Checking for Payee: " + payeeName);
                        var payee = db.SingleOrDefault<Payee>("SELECT * FROM PAYEE_V1 WHERE PAYEENAME=@0", payeeName);

                        if (payee == null)
                        {
                            Console.WriteLine("Inserting Payee: " + payeeName);
                            // create the payee
                            payee = new Payee();
                            payee.PAYEENAME = payeeName;
                            payee.CATEGID = -1;
                            payee.SUBCATEGID = -1;
                            // updates PAYEEID in payee as well as Inserting the payee
                            db.Insert(payee);
                        }

                        var newTrans = new Transactions();
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
                        newTrans.TRANSAMOUNT = Math.Abs( trans.Amount);
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
                        Console.WriteLine("Transaction present");
                    }
                }

            }

            Console.WriteLine("-------------------------------------------------------------------------");

            // Show all Accounts    
            foreach (var a in db.Query<Accounts>("SELECT * FROM ACCOUNTLIST_V1;"))
            {
                Console.WriteLine("{0} - {1} - {2}", a.ACCOUNTID, a.ACCOUNTNUM, a.ACCOUNTNAME);
            }

            Console.ReadLine();
        }
    }
}
