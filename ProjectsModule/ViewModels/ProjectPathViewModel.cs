using ProjectsModule.Common;
using ProjectsModule.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectsModule.ViewModels
{
    public class ProjectPathViewModel
    {
        private IProjectPathCallbacks callbacks;

        private void HandleDelete()
        {
            callbacks.Delete(this);
        }

        private void Init()
        {
            this.DeleteCommand = new SimpleCommand((obj) => HandleDelete());
        }

        public ProjectPathViewModel(IProjectPathCallbacks callbacks)
        {
            this.callbacks = callbacks;

            Init();
        }

        public ProjectPathViewModel(IProjectPathCallbacks callbacks, string path)
        {
            this.callbacks = callbacks;
            this.Path = path;

            Init();
        }

        public string Path { get; set; }

        public ICommand DeleteCommand { get; private set; }
    }
}
