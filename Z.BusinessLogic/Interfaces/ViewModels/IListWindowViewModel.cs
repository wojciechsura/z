using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Models.DTO;

namespace Z.BusinessLogic.Interfaces.ViewModels
{
    public interface IListWindowViewModel
    {
        IListWindowAccess ListWindowAccess { set; }
        int SelectedItemIndex { get; set; }
        IEnumerable<SuggestionDTO> Suggestions { get; }
    }
}
