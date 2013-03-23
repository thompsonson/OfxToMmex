using PetaPoco;
using System.Collections.Generic;
using System.Configuration;

namespace OfxToMmexConsoleApp
{
    [PetaPoco.TableName("ACCOUNTLIST_V1")]
    [PetaPoco.PrimaryKey("ACCOUNTID")]
    [PetaPoco.ExplicitColumns]
    public class Accounts 
    {

        [PetaPoco.Column] public int ACCOUNTID { get; set; }
        [PetaPoco.Column] public string ACCOUNTNAME { get; set; }
        [PetaPoco.Column] public string ACCOUNTTYPE { get; set; }
        [PetaPoco.Column] public string ACCOUNTNUM { get; set; }
        [PetaPoco.Column] public string STATUS { get; set; }
        [PetaPoco.Column] public string NOTES { get; set; }
        [PetaPoco.Column] public string HELDAT { get; set; }
        [PetaPoco.Column] public string WEBSITE { get; set; }
        [PetaPoco.Column] public string CONTACTINFO { get; set; }
        [PetaPoco.Column] public string ACCESSINFO { get; set; }
        [PetaPoco.Column] public long INITIALBAL { get; set; }
        [PetaPoco.Column] public string FAVORITEACCT { get; set; }
        [PetaPoco.Column] public int CURRENCYID { get; set; }

    }

    
    [PetaPoco.TableName("CURRENCYFORMATS_V1")]
    [PetaPoco.PrimaryKey("CURRENCYID")]
    [PetaPoco.ExplicitColumns]
    public class Currencies 
    {
        [PetaPoco.Column] public int CURRENCYID { get; set; }
        [PetaPoco.Column] public string CURRENCYNAME { get; set; }
        [PetaPoco.Column] public string PFX_SYMBOL { get; set; }
        [PetaPoco.Column] public string SFX_SYMBOL { get; set; }
        [PetaPoco.Column] public string DECIMAL_POINT { get; set; }
        [PetaPoco.Column] public string GROUP_SEPARATOR { get; set; }
        [PetaPoco.Column] public string UNIT_NAME { get; set; }
        [PetaPoco.Column] public string CENT_NAME { get; set; }
        [PetaPoco.Column] public string SCALE { get; set; }
        [PetaPoco.Column] public long BASECONVRATE { get; set; }
        [PetaPoco.Column] public string CURRENCY_SYMBOL { get; set; }
    }

    [PetaPoco.TableName("CHECKINGACCOUNT_V1")]
    [PetaPoco.PrimaryKey("TRANSID")]
    [PetaPoco.ExplicitColumns]
    public class Transactions 
    {
        [PetaPoco.Column] public int TRANSID { get; set; }
        [PetaPoco.Column] public int ACCOUNTID { get; set; }
        [PetaPoco.Column] public int TOACCOUNTID { get; set; }
        [PetaPoco.Column] public int PAYEEID { get; set; }
        [PetaPoco.Column] public string TRANSCODE { get; set; }
        [PetaPoco.Column] public decimal TRANSAMOUNT { get; set; }
        [PetaPoco.Column] public string STATUS { get; set; }
        [PetaPoco.Column] public string TRANSACTIONNUMBER { get; set; }
        [PetaPoco.Column] public string NOTES { get; set; }
        [PetaPoco.Column] public int CATEGID { get; set; }
        [PetaPoco.Column] public int SUBCATEGID { get; set; }
        [PetaPoco.Column] public string TRANSDATE { get; set; }
        [PetaPoco.Column] public int FOLLOWUPID { get; set; }
        [PetaPoco.Column] public double TOTRANSAMOUNT { get; set; }
        [PetaPoco.Column] public string FITID { get; set; }
        
    }

    [PetaPoco.TableName("PAYEE_V1")]
    [PetaPoco.PrimaryKey("PAYEEID")]
    [PetaPoco.ExplicitColumns]
    public class Payee
    {
        [PetaPoco.Column] public int PAYEEID { get; set; }
        [PetaPoco.Column] public string PAYEENAME { get; set; }
        [PetaPoco.Column] public int CATEGID { get; set; }
        [PetaPoco.Column] public int SUBCATEGID { get; set; }
    }

    [PetaPoco.TableName("OfxToMmexWorkflow")]
    [PetaPoco.PrimaryKey("WorkflowID")]
    [PetaPoco.ExplicitColumns]
    public class Workflow
    {
        [PetaPoco.Column]
        public int WorkflowID { get; set; }
        [PetaPoco.Column]
        public string filename { get; set; }
        [PetaPoco.Column]
        public string Status { get; set; }
        [PetaPoco.Column]
        public string WatcherCreateTS { get; set; }
        [PetaPoco.Column]
        public string ImportStartTS { get; set; }
        [PetaPoco.Column]
        public string ProcessStartTS { get; set; }
        [PetaPoco.Column]
        public string DBInsertStartTS { get; set; }
        [PetaPoco.Column]
        public string FinishTS { get; set; }
        [PetaPoco.Column]
        public string LogFile { get; set; }
    }

    [PetaPoco.TableName("OfxToMmexPayeeNameRegex")]
    [PetaPoco.PrimaryKey("ID")]
    [PetaPoco.ExplicitColumns]
    public class PayeeRegex
    {
        [PetaPoco.Column]
        public int ID { get; set; }
        [PetaPoco.Column]
        public string Regex { get; set; }
        [PetaPoco.Column]
        public int GroupIndex { get; set; }
        [PetaPoco.Column]
        public bool Active { get; set; }
    }

}
