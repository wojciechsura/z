using RunModule.Resources;
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

namespace RunModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "Run";
        private const string ACTION_KEYWORD = "run";
        private const string ACTION_NAME = "Run";

        private ImageSource icon;

        public Module()
        {
            icon = new BitmapImage(new Uri("pack://application:,,,/RunModule;component/Resources/run.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            collector.AddSuggestion(new SuggestionInfo(enteredText, $"Run {enteredText}", Strings.Run_Comment, icon, 0));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            try
            {
                Process.Start(expression);
            }
            catch 
            {
                options.ErrorText = RunModule.Resources.Strings.Run_Message_CannotExecute;
                options.PreventClose = true;
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            try
            {
                Process.Start(suggestion.Text);
            }
            catch
            {
                options.ErrorText = RunModule.Resources.Strings.Run_Message_CannotExecute;
                options.PreventClose = true;
            }
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(ACTION_KEYWORD, ACTION_NAME, Strings.Run_ActionDisplayName, Strings.Run_ActionComment);
        }

        public string Name => MODULE_NAME;

        public string DisplayName => Strings.Run_ModuleDisplayName;

        public ImageSource Icon => icon;
    }
}
