using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Z.BusinessLogic.Common;
using Z.BusinessLogic.Services.Interfaces;
using Z.Models.Configuration;

namespace Z.BusinessLogic.Services
{
    class ConfigurationService : IConfigurationService
    {
        private const string CONFIG_FILENAME = "config.xml";

        private Configuration configuration;

        private string GetConfigDirectory()
        {
            string localRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!localRoaming.EndsWith("\\"))
                localRoaming += '\\';

            string result = $"{localRoaming}Spooksoft\\Z\\";
            return result;
        }

        private string GetConfigPath()
        {
            return GetConfigDirectory() + CONFIG_FILENAME;
        }

        private void OnConfigurationChanged()
        {
            if (ConfigurationChanged != null)
                ConfigurationChanged(this, EventArgs.Empty);
        }

        public ConfigurationService()
        {
            // Defaults
            configuration = new Configuration();

            // Load configuration
            Load();
        }

        public bool Save()
        {
            try
            {
                string configPath = GetConfigPath();

                var configDirectory = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(configDirectory))
                    Directory.CreateDirectory(configDirectory);

                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                using (FileStream fs = new FileStream(configPath, FileMode.Create, FileAccess.ReadWrite))
                    serializer.Serialize(fs, configuration);

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Load()
        {
            try
            {
                string configPath = GetConfigPath();

                if (File.Exists(configPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                    using (FileStream fs = new FileStream(configPath, FileMode.Open, FileAccess.Read))
                    {
                        Configuration newConfiguration = serializer.Deserialize(fs) as Configuration;

                        // Possible validation

                        configuration = newConfiguration;
                        OnConfigurationChanged();
                    }
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public Configuration Configuration
        {
            get
            {
                return configuration;
            }
        }

        public event EventHandler ConfigurationChanged;
    }
}
