using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace WSCadModule
{
    public class Module : IZModule
    {
        // Private constants --------------------------------------------------

        private const string MODULE_NAME = "WSCAD";
        private const string MODULE_DISPLAY_NAME = "WSCAD";

        private const string WSCAD_ACTION = "WSCADShortcut";
        private const string WSCAD_KEYWORD = "ws";
        private const string WSCAD_KEYWORD_DISPLAY = "WSCAD";
        private const string WSCAD_KEYWORD_COMMENT = "Shortcuts to internal WsCAD services";

        private readonly Regex taskNumberRegex = new Regex(@"^[0-9]{5,6}$");

        // Private types ------------------------------------------------------

        private class OperationInfo
        {
            public OperationInfo(string word, string command, string description)
            {
                this.Word = word;
                this.Command = command;
                this.Description = description;
            }

            public string Word { get; private set; }
            public string Command { get; private set; }
            public string Description { get; private set; }
        }

        // Private fields -----------------------------------------------------

        private readonly List<OperationInfo> operations = new List<OperationInfo>
        {
            new OperationInfo("my", "https://mywscad.azurewebsites.net", "My WsCAD"),
            new OperationInfo("absencecalendar", "https://mywscad.azurewebsites.net/AbsenceCalendar/List", "Absence calendar"),
            new OperationInfo("homeoffice", "https://mywscad.azurewebsites.net/MyHomeOfficeEntries/List", "My home office"),
            new OperationInfo("tasks", "https://mywscad.azurewebsites.net/MyLoggedTasks/List", "My logged tasks"),
            new OperationInfo("creativity", "https://mywscad.azurewebsites.net/CreativityReport", "Creativity report"),
            new OperationInfo("board", "http://srv-tfs:8080/tfs/DefaultCollection/WSCAD%20SUITE/Team%20PL/_backlogs/Taskboard", "Board"),
            new OperationInfo("createbuild", "http://srv-tfs:8080/tfs/DefaultCollection/WSCAD%20SUITE/Team%20PL/_backlogs/Taskboard?_a=requirements", "Create build"),
            new OperationInfo("builds", "http://srv-tfs:8080/tfs/DefaultCollection/WSCAD%20SUITE/Team%20PL/_build", "Build list"),
            new OperationInfo("createpr", "http://srv-tfs:8080/tfs/DefaultCollection/_git/WSCAD%20SUITE/pullrequestcreate", "Create pull request"),
            new OperationInfo("pullrequests", "http://srv-tfs:8080/tfs/DefaultCollection/WSCAD%20SUITE/Team%20PL/_git/WSCAD%20SUITE/pullrequests?_a=mine", "Pull requests"),
            new OperationInfo("wiki", "http://srv-tfs:8080/tfs/DefaultCollection/WSCAD%20SUITE/Team%20PL/_wiki", "Wiki"),            
        };

        private BitmapImage icon;

        // Public methods -----------------------------------------------------

        public Module()
        {
            operations.Sort((x, y) => x.Word.CompareTo(y.Word));

            icon = new BitmapImage(new Uri("pack://application:,,,/WSCadModule;component/Resources/wscad.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<OperationInfo, bool> filter;
            if (perfectMatchesOnly)
                filter = op => op.Word.ToUpper() == enteredText.ToUpper();
            else
                filter = op => (op.Word.ToUpper().Contains(enteredText.ToUpper()) || op.Description.ToUpper().Contains(enteredText.ToUpper()));

            operations
                .Where(filter)
                .Select(op => new SuggestionInfo(op.Word, op.Word, op.Description, icon, SuggestionUtils.EvalMatch(enteredText, op.Word, op.Description)))
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));

            if (taskNumberRegex.IsMatch(enteredText))
            {
                collector.AddSuggestion(new SuggestionInfo($"{enteredText}", $"Open task {enteredText}", "Opens task in TFS", icon, 0, null, MODULE_DISPLAY_NAME));
            }
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            OperationInfo info = operations.FirstOrDefault(op => op.Word.ToUpper() == expression.ToUpper());

            if (info != null)
                Process.Start(info.Command);
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            if (taskNumberRegex.IsMatch(suggestion.Text))
            {
                Process.Start($"http://srv-tfs:8080/tfs/DefaultCollection/WSCAD%20SUITE/_workitems?id={suggestion.Text}");
            }
            else
            {
                OperationInfo info = operations.FirstOrDefault(op => op.Word.ToUpper() == suggestion.Text.ToUpper());

                if (info != null)
                    Process.Start(info.Command);
            }
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            return new List<KeywordInfo>
            {
                new KeywordInfo(WSCAD_KEYWORD, WSCAD_ACTION, WSCAD_KEYWORD_DISPLAY, WSCAD_KEYWORD_COMMENT)
            };
        }

        // Public properties --------------------------------------------------

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
                return MODULE_DISPLAY_NAME;
            }
        }

        public ImageSource Icon => icon;
    }
}
