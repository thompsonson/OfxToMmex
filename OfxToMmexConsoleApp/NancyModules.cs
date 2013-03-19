using log4net;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace OfxToMmexConsoleApp
{
    // a simple module to be hosted in the console app
    public class MainModule : NancyModule
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainModule));

        public MainModule()
        {
            // Create a PetaPoco database object
            var db = new PetaPoco.Database("mmex_db");

            Get["/PayeeRegex/{id}"] = x =>
            {
                var payeeRegexModel = db.Query<PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex where ID=" + x.id + ";");
                return View["Views\\PayeeRegexID.html", payeeRegexModel];
            };
            // TODO: change to work with the DELETE verb (form needs changing)
            Get["/PayeeRegex/delete/{id}"] = x =>
            {
                try{
                    db.Execute("DELETE FROM OfxToMmexPayeeNameRegex where ID=" + x.id + ";");
                }
                catch
                {
                    log.Info("Failed to bind the posted details and insert into db");
                    // raise an exception
                }
                return Response.AsRedirect("/PayeeRegex");
            };

            Get["/PayeeRegex"] = x =>
            {
                var payeeRegexModel = db.Query<PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex ;");
                return View["Views\\PayeeRegex.html", payeeRegexModel];
            };

            Post["/PayeeRegex/add"] = parameters =>
            {
                try
                {
                    PayeeRegex p = this.Bind();
                    db.Insert(p);
                }
                catch
                {
                    log.Info("Failed to bind the posted details and insert into db");
                    // raise an exception
                }
                return Response.AsRedirect("/PayeeRegex");
            };

            Post["/PayeeRegex/update"] = parameters =>
            {
                try
                {
                    PayeeRegex p = this.Bind();
                    db.Update(p);
                }
                catch
                {
                    log.Info("Failed to bind the posted details and update into db");
                    // raise an exception
                }
                return Response.AsRedirect("/PayeeRegex");
            };

            Get["/Config"] = parameters =>
            {
                //ConfigurationManager.AppSettings.AllKeys;
                
                return "test";
            };
        }
    }
    /* 
     *
     *  config.AppSettings.Settings["FTPHost"].Value = tbFtpHost.Text;
        config.AppSettings.Settings["FTPUserName"].Value = tbFtpUsername.Text;
        config.AppSettings.Settings["FTPUserPasswd"].Value = tbFtpUserpasswd.Text;
        config.AppSettings.Settings["FTPRemoteDir"].Value = tbFtpRemoteDir.Text;
        config.AppSettings.Settings.Remove("FTPHost");
        config.AppSettings.Settings.Add("FTPHost", tbFtpHost.Text);
        config.AppSettings.SectionInformation.ForceSave = true;
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");

        http://www.daniweb.com/software-development/csharp/threads/245308/writingupdating-app.config
     */
}
