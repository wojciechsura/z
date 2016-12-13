using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Interfaces
{
    public interface IListWindowLogic
    {
        IListWindowViewModelAccess ListWindowViewModel { set; }

        void SelectedSuggestionChanged();
    }
}
