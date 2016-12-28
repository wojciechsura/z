using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Services.Interfaces
{
    internal interface IZExclusiveSuggestions
    {
        bool IsExclusiveText(string text);
    }
}
