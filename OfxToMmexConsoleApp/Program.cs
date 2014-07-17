using System;
using log4net;
using log4net.Config;

namespace OfxToMmex
{
    class Program
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            string log4netConfigPath;
            try
            {
                // Set up a simple configuration that logs on the console.
                // BasicConfigurator.Configure();
                log4netConfigPath = System.Configuration.ConfigurationManager.AppSettings["log4net"];
                //XmlConfigurator.Configure(new System.IO.FileInfo(args[0]));
                XmlConfigurator.Configure(new System.IO.FileInfo(log4netConfigPath));
            }
            catch (Exception ex)
            {
                throw new OfxToMmex.OfxToMmexException("Failed to set up log4net", ex);
            }
            log.Info("log4net config loaded - starting the service");
        	
			log.Info("Processing ofx file " + args [0]);

			OfxToMmex.App.CmdLine.processFile(args[0]);
			//OfxToMmex.App.CmdLine.processFile("/Users/matt/Downloads/data-1.ofx");
		
        }

    }
}
