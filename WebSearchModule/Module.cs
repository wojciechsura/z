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
using Z.Api;
using WebSearchModule.Resources;

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

        private readonly ImageSource icon;

        private List<SearchInfo> searchInfos = new List<SearchInfo>
        {
            new SearchInfo("GoogleSearch", "g", Strings.WebSearch_Google, Strings.WebSearch_Google_Comment, "https://www.google.com/#q={0}"),
            new SearchInfo("WikipediaSearch", "wiki", Strings.WebSearch_Wikipedia, Strings.WebSearch_Wikipedia_Comment, "http://pl.wikipedia.org/w/index.php?title=Specjalna:Szukaj&search={0}"),
            new SearchInfo("MSDNSearch", "msdn", Strings.WebSearch_MSDN, Strings.WebSearch_MSDN_Comment, "http://social.msdn.microsoft.com/Search/en-EN?query={0}"),
            new SearchInfo("YoutubeSearch", "y", Strings.WebSearch_Youtube, Strings.WebSearch_Youtube_Comment, "http://www.youtube.com/results?search_query={0}&page={{startPage?}}&utm_source=opensearch"),
            new SearchInfo("MapsSearch", "maps", Strings.WebSearch_GoogleMaps, Strings.WebSearch_GoogleMaps_Comment, "https://www.google.pl/maps/search/{0}"),
            new SearchInfo("DevDocs", "dd", Strings.WebSearch_DevDocs, Strings.WebSearch_DevDocs_Comment, "http://devdocs.io/#q={0}"),
            new SearchInfo("StackOverflow", "so", Strings.WebSearch_StackOverflow, Strings.WebSearch_StackOverflow_Comment, "http://stackoverflow.com/search?q={0}")
        };

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

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            // No suggestions available for this module
        }

        public string Name
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
                return Strings.WebSearch_ModuleDisplayName;
            }
        }

        public ImageSource Icon => icon;
    }
}
