using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.ViewModels.Interfaces;

namespace Z.ViewModels
{
    class ViewModelFactory : IViewModelFactory
    {
        public IMainWindowViewModel GenerateMainWindowViewModel(IMainWindowAccess access)
        {
            return new MainWindowViewModel(access);
        }
    }
}
