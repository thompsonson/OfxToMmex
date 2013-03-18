using log4net;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Get["/payee/{id}"] = x =>
            {
                var payeeRegexModel = db.Query<PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex where ID=" + x.id + ";");
                return View["Views\\payeeid.html", payeeRegexModel];
            };

            Get["/payee"] = x =>
            {
                var payeeRegexModel = db.Query<PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex ;");
                return View["Views\\payee.html", payeeRegexModel];
            };

            Post["/payee/add"] = parameters =>
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
                return Response.AsRedirect("/payee");
            };

            Post["/payee/update"] = parameters =>
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
                return Response.AsRedirect("/payee");
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
