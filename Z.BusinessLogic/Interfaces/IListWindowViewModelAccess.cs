using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Models;
using Z.Models.DTO;

namespace Z.BusinessLogic.Interfaces
{
    public interface IListWindowViewModelAccess
    {
        void SelectPreviousSuggestion();
        void SelectNextSuggestion();

        SuggestionDTO SelectedSuggestion { get; }
        List<SuggestionDTO> Suggestions { set; }
    }
}
