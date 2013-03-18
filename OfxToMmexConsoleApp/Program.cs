using System;
using log4net;
using log4net.Config;
using Topshelf;

namespace OfxToMmexConsoleApp
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
                throw new OfxToMmexException("Failed to set up log4net", ex);
            }
            log.Info("log4net config loaded - starting the service");
            try
            {
                // get things moving
                //OfxToMmex.Start();

                HostFactory.Run(x =>                                 
                {
                    x.UseLog4Net(log4netConfigPath);
                    x.Service<OfxToMmex>(s =>                        
                    {
                        s.ConstructUsing(name => new OfxToMmex());
                        s.WhenStarted(OfxToMmex => OfxToMmex.Start());
                        s.WhenStopped(OfxToMmex => OfxToMmex.Stop());
                    });
                    x.RunAsLocalSystem();                            

                    x.SetDescription("Monitors folder for files with ofx data in them. Imports to specified Money Manager Ex database.");        
                    x.SetDisplayName("OfxToMmex");                       
                    x.SetServiceName("OfxToMmex");                       
                });  
            }
            catch (Exception ex)
            {
                log.Fatal("Failed to start the service");
                throw new OfxToMmexException("Failed to start the service", ex);
            }
        }

    }
}
