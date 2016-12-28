using ProjectsModule.Common;
using ProjectsModule.Models;
using ProjectsModule.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectsModule.ViewModels
{
    public class ConfigurationWindowViewModel : IProjectPathCallbacks
    {
        private readonly Configuration configuration;
        private IConfigurationWindowAccess access;

        private ObservableCollection<ProjectPathViewModel> paths;

        private void HandleOK()
        {
            access.CloseWindow();
        }

        private void HandleAdd()
        {
            paths.Add(new ProjectPathViewModel(this));
        }

        public ConfigurationWindowViewModel(Configuration configuration)
        {
            this.configuration = configuration;
            paths = new ObservableCollection<ProjectPathViewModel>(configuration.ProjectRoots
                .Select(pr => new ProjectPathViewModel(this, pr))
                .ToList());

            OKCommand = new SimpleCommand((obj) => HandleOK());
            AddCommand = new SimpleCommand((obj) => HandleAdd());
        }

        internal void Save()
        {
            configuration.ProjectRoots = paths
                .Select(p => p.Path)
                .ToList();            
        }

        public void Delete(ProjectPathViewModel path)
        {
            paths.Remove(path);
        }

        public ObservableCollection<ProjectPathViewModel> Paths => paths;

        public IConfigurationWindowAccess Access
        {
            set
            {
                access = value;
            }
        }

        internal IEnumerable<string> Validate()
        {
            foreach (var path in paths)
            {
                if (!Directory.Exists(path.Path))
                {
                    yield return $"Project directory {path.Path} does not exist!";
                }
            }
        }

        public ICommand AddCommand { get; private set; }

        public ICommand OKCommand { get; private set; }
    }
}
