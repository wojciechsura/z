using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace ProcessModule
{
    public class Module : IZModule
    {
        private const string MODULE_DISPLAY_NAME = "Kill process";
        private const string MODULE_NAME = "Process";

        private const string KILL_KEYWORD = "kill";
        private const string KILL_ACTION = "Kill";
        private const string KILL_KEYWORD_DISPLAY = "Kill process";
        private const string KILL_KEYWORD_COMMENT = "Kills chosen process";

        private ImageSource icon;

        public Module()
        {
            icon = new BitmapImage(new Uri("pack://application:,,,/ProcessModule;component/Resources/process.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            if (action == KILL_ACTION)
            {
                Func<Process, bool> func;
                if (perfectMatchesOnly)
                    func = (p => p.ProcessName.ToUpper() == enteredText.ToUpper());
                else
                    func = (p => p.ProcessName.ToUpper().Contains(enteredText.ToUpper()));

                foreach (var p in Process.GetProcesses()
                    .Where(func)
                    .OrderBy(p => p.ProcessName))
                {
                    string filename;

                    try
                    {                         
                        filename = p.MainModule.FileName;
                    }
                    catch
                    {
                        filename = "(unknown path)";
                    }

                    collector.AddSuggestion(new SuggestionInfo(p.ProcessName, p.ProcessName, $"{p.Id}, {filename}", icon, SuggestionUtils.EvalMatch(enteredText, p.ProcessName), p.Id));
                }
            }
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            if (action == KILL_ACTION)
            {
                List<Process> processes = System.Diagnostics.Process.GetProcesses()
                    .Where(p => p.ProcessName.ToUpper() == expression.ToUpper())
                    .ToList();
                 
                if (processes.Count == 1)
                {
                    processes[0].Kill();
                }
                else
                {
                    options.PreventClose = true;
                }
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            Process process = Process.GetProcessById((int)suggestion.Data);
            if (process != null)
                process.Kill();
            else
                options.PreventClose = true;
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(KILL_KEYWORD, KILL_ACTION, KILL_KEYWORD_DISPLAY, KILL_KEYWORD_COMMENT);
        }

        public string DisplayName => MODULE_DISPLAY_NAME;

        public ImageSource Icon => icon;

        public string Name => MODULE_NAME;
    }
}
