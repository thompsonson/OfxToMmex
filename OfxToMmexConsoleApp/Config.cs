using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.Data.SqlClient;

namespace OfxToMmexConsoleApp
{
    public class ConfigIntermediary
    {
        public string mmex_db { get; set; }
        public string Rootpath { get; set; }
        public string log4net{ get; set; }
    }

    public class Config
    {
        private static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }

        private static void UpdateConnectionString(string key, string value)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            var connectionStrings = xmlDocument.SelectNodes("configuration/connectionStrings/add");
            foreach (XmlNode node in connectionStrings)
            {
                if (node.Attributes["name"].Value == key)
                {
                    node.Attributes["connectionString"].Value = new System.Configuration.ConnectionStringSettings(key, value).ToString();
                }
            }
            xmlDocument.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public static string mmex_db { 
            get 
            { 
                return ConfigurationManager.ConnectionStrings["mmex_db"].ToString(); 
            }
            set
            {
                UpdateConnectionString("mmex_db", value);
            } 
        }
        public static string Rootpath
        {
            get 
            {
                return ConfigurationManager.AppSettings["Rootpath"]; 
            }
            set 
            {
                UpdateSetting("Rootpath", value);
                // stop and start the file watcher
                OfxToMmex.ChangeWatchingFolder();
            }
        }
        public static string log4net
        {
            get
            {
                return ConfigurationManager.AppSettings["log4net"];
            }
            set
            {
                // relevant after a restart
                UpdateSetting("log4net", value);
            }
        }
    }
    // * /*/
}
