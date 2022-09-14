using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.ViewModels.Configuration;
using Z.Dependencies;
using System.Windows.Input;
using Z.Wpf.Types;
using Z.BusinessLogic.ViewModels.Configuration.Base;
using Z.BusinessLogic.ViewModels.Configuration.General;
using Z.BusinessLogic.ViewModels.Configuration.Behavior;
using Z.BusinessLogic.ViewModels.Configuration.Keywords;
using Z.BusinessLogic.ViewModels.Configuration.Modules;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.ViewModels.Configuration.Launcher;
using Z.Resources;

namespace Z.BusinessLogic.ViewModels.Configuration
{
    public class ConfigurationViewModel
    {
        private readonly IConfigurationService configurationService;

        private IConfigurationWindowAccess configWindowAccess;
        private readonly List<BaseConfigurationViewModel> pages;

        private void HandleCancel()
        {
            configWindowAccess.CloseWindow();
        }

        private void HandleOk()
        {
            // Validate
            foreach (BaseConfigurationViewModel page in pages)
            {
                List<string> messages = page.Validate()?.ToList();
                if (messages != null && messages.Any())
                {
                    configWindowAccess.ShowWarning(messages.First(), Strings.Z_DialogTitle_Configuration);
                    return;
                }
            }

            foreach (BaseConfigurationViewModel viewModel in pages)
            {
                viewModel.Save();
            }

            configurationService.Save();
            configurationService.NotifyConfigurationChanged();
            configWindowAccess.CloseWindow();
        }

        public ConfigurationViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            pages = new List<BaseConfigurationViewModel>
            {
                Container.Instance.Resolve<GeneralViewModel>(),
                Container.Instance.Resolve<BehaviorViewModel>(),
                Container.Instance.Resolve<KeywordsViewModel>(),
                Container.Instance.Resolve<ModulesViewModel>(),
                Container.Instance.Resolve<LauncherViewModel>()
            };

            OkCommand = new SimpleCommand((obj) => HandleOk());
            CancelCommand = new SimpleCommand((obj) => HandleCancel());
        }

        public IConfigurationWindowAccess ConfigurationWindowAccess
        {
            set
            {
                if (configWindowAccess != null)
                    throw new InvalidOperationException("Access can be set only once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                configWindowAccess = value;
            }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public IEnumerable<BaseConfigurationViewModel> Pages => pages;
    }
}
