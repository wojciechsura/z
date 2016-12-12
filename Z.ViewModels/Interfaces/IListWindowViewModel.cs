using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Models.DTO;

namespace Z.ViewModels.Interfaces
{
    public interface IListWindowViewModel
    {
        IListWindowAccess ListWindowAccess { set; }

        IEnumerable<SuggestionDTO> Suggestions { get; }
    }
}
