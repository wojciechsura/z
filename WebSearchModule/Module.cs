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
        private class SearchInfo       
        {
            public SearchInfo(string action, string keyword, string display, string searchString)
            {
                Action = action;
                Keyword = keyword;
                Display = display;
                SearchString = searchString;
            }

            public string Action { get; private set; }
            public string Keyword { get; private set; }
            public string Display { get; private set; }
            public string SearchString { get; private set; }
        }

        private const string MODULE_NAME = "WebSearch";

        private List<SearchInfo> searchInfos = new List<SearchInfo>
        {
            new SearchInfo("GoogleSearch", "g", "Google", "https://www.google.com/#q={0}"),
            new SearchInfo("WikipediaSearch", "wiki", "Wikipedia", "http://pl.wikipedia.org/w/index.php?title=Specjalna:Szukaj&search={0}"),
            new SearchInfo("MSDNSearch", "msdn", "MSDN", "http://social.msdn.microsoft.com/Search/en-EN?query={0}"),
            new SearchInfo("YoutubeSearch", "y", "Youtube", "http://www.youtube.com/results?search_query={0}&page={{startPage?}}&utm_source=opensearch")
        };

        public string InternalName
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public IEnumerable<KeywordActionInfo> GetKeywordActions()
        {
            return searchInfos
                .Select(si => new KeywordActionInfo(si.Keyword, si.Action, si.Display))
                .ToList();
        }

        public void ExecuteKeywordAction(string action, string expression)
        {
            var info = searchInfos
                .FirstOrDefault(i => i.Action == action);

            if (info != null)
            {
                Process.Start(String.Format(info.SearchString, WebUtility.UrlEncode(expression)));
            }
            else
                    throw new InvalidOperationException("Invalid action!");
        }
    }
}
