using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Interfaces;
using Z.ViewModels.Interfaces;

namespace Z.ViewModels
{
    class ListWindowViewModel : IListWindowViewModel, IListWindowViewModelAccess
    {
        private IListWindowAccess access;
        private IListWindowLogic logic;

        public ListWindowViewModel(IListWindowLogic logic)
        {
            this.logic = logic;
        }

        public IListWindowAccess ListWindowAccess
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (access != null)
                    throw new InvalidOperationException("Access can be set only once!");

                access = value;
            }
        }
    }
}
