using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Interfaces;
using Z.Common.Types;

namespace GoogleSearchModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "Google";

        private const string SEARCH_ACTION = "Search";
        private const string SEARCH_KEYWORD = "g";
        private const string SEARCH_DISPLAY = "Google";

        public string InternalName
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public IEnumerable<KeywordActionInfo> GetKeywordActions()
        {
            return new List<KeywordActionInfo>
            {
                new KeywordActionInfo(SEARCH_KEYWORD, SEARCH_ACTION, SEARCH_DISPLAY)                
            };
        }
    }
}
