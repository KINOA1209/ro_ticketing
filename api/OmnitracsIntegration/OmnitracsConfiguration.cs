using System;
using System.IO;

namespace OmnitracsIntegration
{
    public class OmnitracsConfiguration
    {
        //TODO: Move to config file
        public static string CompanyId = "ROLFSONOIL";
        public static string Username = "WEBSVC@ROLFSONOIL";
        public static string Password = "s@&m$B4xKYt6";
        public static string OMASWebSvcsEndpoint = "https://services.omnitracs.com/omasWebWS/services/OMASWebSvcs";
        public static string EipWebSvcsEndpoint = "https://eipws.omnitracs.com/eipWebWS/EIPOutMsgSvcs1.0";
        public static string TMMWebSvcsEndpoint = "https://services.omnitracs.com/qtracsWebWS/services/TMMWebSvcs";

        //private static string AppSettingsFile = "appsettings.json";

        //private static OmnitracsConfiguration _instance;
        //public static OmnitracsConfiguration Instance => _instance ?? (_instance = new OmnitracsConfiguration());

        //public readonly IConfiguration Configuration;

        //private string _baseDirectory;
        //private string BaseDirectory
        //{
        //    get
        //    {
        //        if (_baseDirectory == null)
        //        {
        //            _baseDirectory = Directory.GetCurrentDirectory();
        //            while (!File.Exists(Path.Combine(_baseDirectory, AppSettingsFile)))
        //            {
        //                _baseDirectory = Directory.GetParent(_baseDirectory).FullName;
        //                if (_baseDirectory.Length == 0)
        //                    throw new Exception($"{AppSettingsFile} cannot be found.");
        //            }
        //        }

        //        return _baseDirectory;
        //    }
        //}

        //private OmnitracsConfiguration()
        //{
        //    Configuration = new ConfigurationBuilder()
        //        .SetBasePath(BaseDirectory)
        //        .AddJsonFile(AppSettingsFile, false, true)
        //        .Build();
        //}

        //public string AppName => Configuration["Name"];
        //public string AppVersion => Configuration["Version"];

        //public IConfigurationSection SendGrid => Configuration.GetSection("SendGrid");
        //public IConfigurationSection Security => Configuration.GetSection("Security");
        //public string ResetPasswordLink => Configuration["ResetPasswordLink"];
    }
}
