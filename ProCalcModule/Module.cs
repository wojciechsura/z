using ProCalc.NET;
using ProCalc.NET.Numerics;
using ProCalcModule.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace ProCalcModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "Calculator";
        private const string ACTION_KEYWORD = "calc";
        private const string ACTION_NAME = "Calc";

        // Private fields -----------------------------------------------------

        private readonly ProCalcCore proCalc;
        private readonly ImageSource icon;

        // Public methods -----------------------------------------------------

        public Module()
        {
            proCalc = new ProCalcCore();
            icon = new BitmapImage(new Uri("pack://application:,,,/ProCalcModule;component/Resources/calc.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            try
            {
                var expression = proCalc.Compile(enteredText);
                var result = proCalc.Execute(expression);

                collector.AddSuggestion(new SuggestionInfo(enteredText, result.AsString(), Strings.Calculator_Comment, null, 100, result, Strings.Calculator_ModuleDisplayName));
            }
            catch
            {
                // Do nothing
            }
        }
        
        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            try
            {
                var expr = proCalc.Compile(expression);
                var result = proCalc.Execute(expr, null, false);

                Clipboard.SetText(result.ToString());
            }
            catch (Exception e)
            {
                options.ErrorText = string.Format(Strings.Calculator_Message_CannotEvaluate, e.Message);
                options.PreventClose = true;
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            BaseNumeric result = suggestion.Data as BaseNumeric;
            Clipboard.SetText(result.AsString());
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(ACTION_KEYWORD, ACTION_NAME, Strings.Calculator_ActionDisplayName, Strings.Calculator_ActionComment);
        }

        // Public properties --------------------------------------------------

        public string DisplayName => Strings.Calculator_ActionComment;

        public string Name => MODULE_NAME;

        public ImageSource Icon => icon;
    }
}
