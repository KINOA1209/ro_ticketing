using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ticketing_api.Infrastructure
{
    public class AppConfiguration
    {
        private static string AppSettingsFile = "appsettings.json";

        private static AppConfiguration _instance;
        public static AppConfiguration Instance => _instance ?? (_instance = new AppConfiguration());

        public readonly IConfiguration Configuration;

        private string _baseDirectory;
        private string BaseDirectory
        {
            get
            {
                if (_baseDirectory == null)
                {
                    _baseDirectory = Directory.GetCurrentDirectory();
                    while (!File.Exists(Path.Combine(_baseDirectory, AppSettingsFile)))
                    {
                        _baseDirectory = Directory.GetParent(_baseDirectory).FullName;
                        if (_baseDirectory.Length == 0)
                            throw new Exception($"{AppSettingsFile} cannot be found.");
                    }
                }

                return _baseDirectory;
            }
        }

        private AppConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(BaseDirectory)
                .AddJsonFile(AppSettingsFile, false, true)
                .Build();
        }

        public string AppName => Configuration["Name"];
        public string AppVersion => Configuration["Version"];

        public IConfigurationSection SendGrid => Configuration.GetSection("SendGrid");
        public IConfigurationSection Security => Configuration.GetSection("Security");
        public string ResetPasswordLink => Configuration["ResetPasswordLink"];
    }
}
