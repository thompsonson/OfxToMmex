using log4net;
using Nancy;
using Nancy.ModelBinding;
using System;

namespace OfxToMmex.Web
{
    // a simple module to be hosted in the console app
    public class Config : NancyModule
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Config));

        public Config()
            : base("/Config")
        {
            Get["/"] = parameters =>
            {
                Model.Config config = new Model.Config();
                return View["Views\\Config.html", config];
            };

            Post["/"] = parameters =>
            {
                try
                {
                    Model.ConfigIntermediary config = new Model.ConfigIntermediary();
                    var tt = this.BindTo(config);
                    Model.Config.Rootpath = tt.Rootpath;
                    Model.Config.log4net = tt.log4net;
                    Model.Config.mmex_db = tt.mmex_db;
                }
                catch (Exception ex)
                {
                    log.Fatal("Failed to save the config details");
                    // raise an exception
                    throw new OfxToMmex.OfxToMmexException("Failed to save the config details", ex);
                }
                return Response.AsRedirect("/Config");
            };
        }
    }
}
