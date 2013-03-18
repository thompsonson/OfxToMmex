using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroRest;

namespace OfxToMmexConsoleApp
{
    class WebInterface
    {

    }

    public class Help
    {
        [Rest("/", IsHelp = true, Description = "Test API", DisableConstraints = true)]
        public void Index()
        {
        }
    }

    public class config
    {
        [Rest("/config", Method = "GET", Description = "Display Config")]
        public object get()
        {
            // display the config
            return true;
        }

        [Rest("/config", Method = "POST", Description = "Update Config")]
        public object post()
        {
            // update the config
            /*
            config.AppSettings.Settings["FTPHost"].Value = tbFtpHost.Text;
            config.AppSettings.Settings["FTPUserName"].Value = tbFtpUsername.Text;
            config.AppSettings.Settings["FTPUserPasswd"].Value = tbFtpUserpasswd.Text;
            config.AppSettings.Settings["FTPRemoteDir"].Value = tbFtpRemoteDir.Text;
            config.AppSettings.Settings.Remove("FTPHost");
            config.AppSettings.Settings.Add("FTPHost", tbFtpHost.Text);
            config.AppSettings.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            */
            return true;
        }

    }

    public class regex
    {
        [Rest("/payee", Method = "GET", Description = "Display Config")]
        public object get()
        {
            // display the config
            return true;
        }

        [Rest("/payee", Method = "POST", Description = "Update Config")]
        public object post()
        {
            // update the config
            return true;
        }


    }
}
