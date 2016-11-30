using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Z.Common.Interfaces;
using Z.Common.Types;

namespace WebSearchModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "WebSearch";

        private const string SEARCH_ACTION = "GoogleSearch";
        private const string SEARCH_KEYWORD = "g";
        private const string SEARCH_DISPLAY = "Google";
        private const string GoogleSearchString = "https://www.google.com/#q={0}";

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

        public void ExecuteKeywordAction(string action, string expression)
        {
            switch (action)
            {
                case SEARCH_ACTION:
                    {
                        Process.Start(String.Format(GoogleSearchString, WebUtility.UrlEncode(expression)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid action!");
            }
        }
    }
}
