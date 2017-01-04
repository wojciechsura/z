using Filesystem.Common;
using Filesystem.Models;
using Filesystem.Types;
using Filesystem.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Filesystem.ViewModels
{
    public class ConfigurationWindowViewModel 
    {
        private readonly Configuration configuration;
        private IConfigurationWindowAccess access;

        private FileSearchStrategy fileSearchStrategy;
        private IEnumerable<FileSearchStrategy> fileSearchStrategies;

        private void HandleOK()
        {
            access.CloseWindow();
        }

        public ConfigurationWindowViewModel(Configuration configuration)
        {
            this.configuration = configuration;

            fileSearchStrategy = configuration.FileSearchStrategy;
            fileSearchStrategies = (FileSearchStrategy[])Enum.GetValues(typeof(FileSearchStrategy));

            OKCommand = new SimpleCommand((obj) => HandleOK());
        }

        internal void Save()
        {
            configuration.FileSearchStrategy = fileSearchStrategy;
        }

        public FileSearchStrategy FileSearchStrategy
        {
            get
            {
                return fileSearchStrategy;
            }
            set
            {
                fileSearchStrategy = value;
            }
        }

        public IEnumerable<FileSearchStrategy> FileSearchStrategies => fileSearchStrategies;

        public IConfigurationWindowAccess Access
        {
            set
            {
                access = value;
            }
        }

        public IEnumerable<string> Validate()
        {
            return null;
        }

        public ICommand OKCommand { get; private set; }
    }
}
