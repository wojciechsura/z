using Spk.ProCalc.Engine;
using Spk.ProCalc.Engine.Numerics;
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
        private const string MODULE_DISPLAY_NAME = "Calculator";
        private const string MODULE_NAME = "Calculator";
        private const string COMMENT = "Copy result to clipboard";
        private const string ACTION_KEYWORD = "calc";
        private const string ACTION_NAME = "Calc";
        private const string ACTION_DISPLAY = "Evaluate";
        private const string ACTION_COMMENT = "Evaluate mathematical expressions";

        // Private fields -----------------------------------------------------

        private readonly Engine proCalc;
        private readonly ImageSource icon;

        // Public methods -----------------------------------------------------

        public Module()
        {
            proCalc = new Engine();
            icon = new BitmapImage(new Uri("pack://application:,,,/ProCalcModule;component/Resources/calc.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            try
            {
                var expression = proCalc.Compile(enteredText);
                var result = proCalc.Execute(expression);

                collector.AddSuggestion(new SuggestionInfo(enteredText, result.AsString(), COMMENT, null, result));
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
                var result = proCalc.Execute(expr);

                Clipboard.SetText(result.ToString());
            }
            catch
            {
                // TODO notify about failure
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            BaseNumeric result = suggestion.Data as BaseNumeric;
            Clipboard.SetText(result.AsString());
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(ACTION_KEYWORD, ACTION_NAME, ACTION_DISPLAY, ACTION_COMMENT);
        }

        // Public properties --------------------------------------------------

        public string DisplayName => MODULE_DISPLAY_NAME;

        public string Name => MODULE_NAME;

        public ImageSource Icon => icon;
    }
}
