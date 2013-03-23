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
                catch (Exception ex)
                {
                    log.Info("Failed to bind the posted details and insert into db");
                    // raise an exception
                    throw new OfxToMmexException("Failed to bind the posted details and insert into db", ex);
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
                catch (Exception ex)
                {
                    log.Info("Failed to bind the posted details and insert into db");
                    // raise an exception
                    throw new OfxToMmexException("Failed to bind the posted details and insert into db", ex);
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
                catch (Exception ex)
                {
                    log.Info("Failed to bind the posted details and update into db");
                    // raise an exception
                    throw new OfxToMmexException("Failed to bind the posted details and update into db", ex);
                }
                return Response.AsRedirect("/PayeeRegex");
            };

            Get["/Config"] = parameters =>
            {
                Config config = new Config();
                return View["Views\\Config.html", config];
            };

            Post["/Config"] = parameters =>
            {
                try
                {
                    ConfigIntermediary config = new ConfigIntermediary();
                    var tt = this.BindTo(config);
                    Config.Rootpath = tt.Rootpath;
                    Config.log4net = tt.log4net;
                    Config.mmex_db = tt.mmex_db;
                }
                catch (Exception ex)
                {
                    log.Fatal("Failed to save the config details");
                    // raise an exception
                    throw new OfxToMmexException("Failed to save the config details", ex);
                }
                return Response.AsRedirect("/Config");
            };
        }
    }
}
