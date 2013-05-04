using log4net;
using Nancy;
using Nancy.ModelBinding;
using System;

namespace OfxToMmex.Web
{

    public class PayeeRegex : NancyModule
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PayeeRegex));

        public PayeeRegex()
            : base("/PayeeRegex")
        {
            // Create a PetaPoco database object
            var db = new PetaPoco.Database("mmex_db");

            Get["/{id}"] = x =>
            {
                var payeeRegexModel = db.Query<Model.PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex where ID=" + x.id + ";");
                return View["Views\\PayeeRegexID.html", payeeRegexModel];
            };
            // TODO: change to work with the DELETE verb (form needs changing)
            Get["/delete/{id}"] = x =>
            {
                try
                {
                    db.Execute("DELETE FROM OfxToMmexPayeeNameRegex where ID=" + x.id + ";");
                }
                catch (Exception ex)
                {
                    // TODO: add the record ID
                    log.Info("Failed to delete the record");
                    // raise an exception
                    throw new OfxToMmex.OfxToMmexException("Failed to delete the record", ex);
                }
                return Response.AsRedirect("/PayeeRegex");
            };

            Get["/run/{id}"] = x =>
            {
                try
                {
                    // start a thread with processOne
                }
                catch (Exception ex)
                {
                    log.Info("Failed to run processOne");
                    // raise an exception
                    throw new OfxToMmex.OfxToMmexException("Failed to run processOne", ex);
                }
                return Response.AsRedirect("/PayeeRegex");
            };

            Get["/"] = x =>
            {
                var payeeRegexModel = db.Query<Model.PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex ;");
                return View["Views\\PayeeRegex.html", payeeRegexModel];
            };

            Get["/datatable"] = x =>
            {
                var payeeRegexModel = db.Query<Model.PayeeRegex>("SELECT * FROM OfxToMmexPayeeNameRegex ;");
                var ret = new
                {
                    aaData = payeeRegexModel
                };
                return Response.AsJson(ret);
            };


            Post["/add"] = parameters =>
            {
                try
                {
                    Model.PayeeRegex p = this.Bind();
                    db.Insert(p);
                }
                catch (Exception ex)
                {
                    log.Info("Failed to bind the posted details and insert into db");
                    // raise an exception
                    throw new OfxToMmex.OfxToMmexException("Failed to bind the posted details and insert into db", ex);
                }
                return Response.AsRedirect("/PayeeRegex");
            };

            Post["/update"] = parameters =>
            {
                try
                {
                    Model.PayeeRegex p = this.Bind();
                    db.Update(p);
                }
                catch (Exception ex)
                {
                    log.Info("Failed to bind the posted details and update into db");
                    // raise an exception
                    throw new OfxToMmex.OfxToMmexException("Failed to bind the posted details and update into db", ex);
                }
                return Response.AsRedirect("/PayeeRegex");
            };
        }
    }
}
