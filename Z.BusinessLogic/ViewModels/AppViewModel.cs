using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Z.BusinessLogic.Events;
using Z.BusinessLogic.Services.Interfaces;
using Z.BusinessLogic.ViewModels.Interfaces;
using Z.Wpf.Types;

namespace Z.BusinessLogic.ViewModels
{
    public class AppViewModel
    {
        private readonly IWindowService windowService;
        private IApplicationAccess applicationAccess;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;

        private void HandleTaskbarClick()
        {
            windowService.ShowMainWindow();
        }

        public void Shutdown()
        {
            eventBus.Send(new ShuttingDownEvent());

            configurationService.Save();

            applicationAccess.Shutdown();
        }

        public AppViewModel(IWindowService windowService, IConfigurationService configurationService, IEventBus eventBus)
        {
            this.windowService = windowService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;

            TaskbarClickCommand = new SimpleCommand((obj) => HandleTaskbarClick());
        }

        public ICommand TaskbarClickCommand { get; private set; }

        public IApplicationAccess ApplicationAccess
        {
            set
            {
                if (applicationAccess != null)
                    throw new InvalidOperationException("ApplicationAccess can only be set once!");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                applicationAccess = value;
            }
        }
    }
}
