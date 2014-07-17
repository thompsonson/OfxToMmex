using log4net;
using OFXSharp;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OfxToMmex.App
{
    class ProcessPayee
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessPayee));

        public static string processAll(PetaPoco.Database db, string payeeName)
        {
            // Show all Accounts    
            foreach (var a in db.Query<Model.PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex where Active=1 ;"))
            {
                log.Info(a.ID + " - " + a.Regex + " - " + a.GroupIndex);

                // Here we call Regex.Match.
                Match match = Regex.Match(payeeName, a.Regex, RegexOptions.IgnoreCase);

                // Here we check the Match instance.
                if (match.Success)
                {
                    // Finally, we get the Group value and display it.
                    payeeName = match.Groups[a.GroupIndex].Value;
                    log.Info("payeeName updated for regex: " + a.Regex + " to " + payeeName);
                }
            }

            return payeeName.Trim();
        }

        public static void processOne(PetaPoco.Database db, string regex, int indexToTake)
        {
            log.Info("Processing regex: " + regex);
            Regex re = new Regex(regex);
            
            // Show all Accounts    
            foreach (var a in db.Query<Model.Payee>("SELECT * FROM Payee_v1;"))
            {
                string[] arrPayee = re.Split(a.PAYEENAME);
                if (arrPayee.Count() > 1)
                {
                    log.Info(a.PAYEEID + " - " + a.PAYEENAME);
                    string payeeName = arrPayee[indexToTake].Replace("-", " ").Trim();
                    log.Info("  Processing to PayeeName: " + payeeName);
                    var payee = db.SingleOrDefault<Model.Payee>("SELECT * FROM PAYEE_V1 WHERE PAYEENAME=@0", payeeName);

                    if (payee == null)
                    {
                        log.Info("Inserting Payee: " + payeeName);
                        // create the payee
                        payee = new Model.Payee();
                        payee.PAYEENAME = payeeName;
                        payee.CATEGID = a.CATEGID;
                        payee.SUBCATEGID = a.SUBCATEGID;
                        // updates PAYEEID in payee as well as Inserting the payee
                        db.Insert(payee);
                    }//*/
                    log.Debug("UPDATE CHECKINGACCOUNT_V1 SET PAYEEID = " + payee.PAYEEID + " WHERE PAYEEID = " + a.PAYEEID);
                    // repoint transactions to the new payee
                    db.Execute("UPDATE CHECKINGACCOUNT_V1 SET PAYEEID = @0 WHERE PAYEEID = @1", payee.PAYEEID, a.PAYEEID);
                    //delete the old payee
                    db.Delete(a);
                }
            }
        }

    }
}
