using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Models;

namespace Z.Services.Interfaces
{
    public interface IKeywordService
    {
        KeywordAction GetKeywordAction(string keyword);
    }
}
