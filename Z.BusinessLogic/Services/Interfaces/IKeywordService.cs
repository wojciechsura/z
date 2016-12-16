using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Models;

namespace Z.BusinessLogic.Services.Interfaces
{
    public interface IKeywordService
    {
        KeywordData GetKeywordAction(string keyword);
        IEnumerable<KeywordData> GetKeywords();
    }
}
