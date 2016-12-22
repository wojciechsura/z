using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;
using Z.Api.Types;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WebSearchModule
{
    public class Module : IZModule
    {
        private class SearchInfo       
        {
            public SearchInfo(string action, string keyword, string display, string comment, string searchString)
            {
                Action = action;
                Keyword = keyword;
                Display = display;
                Comment = comment;
                SearchString = searchString;
            }

            public string Action { get; private set; }
            public string Keyword { get; private set; }
            public string Display { get; private set; }
            public string Comment { get; private set; }
            public string SearchString { get; private set; }
        }

        private const string MODULE_NAME = "WebSearch";
        private const string MODULE_DISPLAY_NAME = "Web search";

        private readonly ImageSource icon;

        private List<SearchInfo> searchInfos = new List<SearchInfo>
        {
            new SearchInfo("GoogleSearch", "g", "Google", "Search with Google", "https://www.google.com/#q={0}"),
            new SearchInfo("WikipediaSearch", "wiki", "Wikipedia", "Search on Wikipedia", "http://pl.wikipedia.org/w/index.php?title=Specjalna:Szukaj&search={0}"),
            new SearchInfo("MSDNSearch", "msdn", "MSDN", "Search on MSDN", "http://social.msdn.microsoft.com/Search/en-EN?query={0}"),
            new SearchInfo("YoutubeSearch", "y", "Youtube", "Search on Youtube", "http://www.youtube.com/results?search_query={0}&page={{startPage?}}&utm_source=opensearch"),
            new SearchInfo("MapsSearch", "maps", "Google Maps", "Search on Google Maps", "https://www.google.pl/maps/search/{0}"),
            new SearchInfo("DevDocs", "dd", "DevDocs.io", "Search on DevDocs.io", "http://devdocs.io/#q={0}"),
            new SearchInfo("StackOverflow", "so", "StackOverflow", "Search on StackOverflow", "http://stackoverflow.com/search?q={0}")
        };

        public string InternalName
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public string DisplayName
        {
            get
            {
                return MODULE_DISPLAY_NAME;
            }
        }

        public Module()
        {
            icon = new BitmapImage(new Uri("pack://application:,,,/WebSearchModule;component/Resources/search.png"));
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            return searchInfos
                .Select(si => new KeywordInfo(si.Keyword, si.Action, si.Display, si.Comment))
                .ToList();
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
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

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            // No suggestions available for this module
        }

        public void CollectSuggestions(string enteredText, string keywordAction, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            // No suggestions available for this module
        }

        public ImageSource Icon => icon;        
    }
}
