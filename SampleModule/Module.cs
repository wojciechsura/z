using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace SampleModule
{
    public class Module : IZModule
    {
        private class SampleEntry
        {
            public SampleEntry(string key, string comment, string message)
            {
                Key = key;
                Comment = comment;
                Message = message;
            }

            public string Key { get; private set; }
            public string Comment { get; private set; }
            public string Message { get; private set; }
        }

        private const string MODULE_DISPLAY_NAME = "Sample module";
        private const string MODULE_NAME = "Sample";

        private const string ACTION_KEYWORD = "sample";
        private const string ACTION_DISPLAY_NAME = "Sample action";
        private const string ACTION_NAME = "Sample";
        private const string ACTION_COMMENT = "This is sample action to demonstrate implementation of simple module for Z.";

        private List<SampleEntry> entries;
        private ImageSource icon;

        public Module()
        {
            icon = new BitmapImage(new Uri("pack://application:,,,/SampleModule;component/Resources/sample.png"));

            entries = new List<SampleEntry>
            {
                new SampleEntry("Sample1", "Sample action 1", "Executed sample action 1"),
                new SampleEntry("Sample2", "Sample action 2", "Executed sample action 2"),
                new SampleEntry("Sample3", "Sample action 3", "Executed sample action 3"),
            };
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<SampleEntry, bool> predicate;

            if (perfectMatchesOnly)
                predicate = (se => se.Key.ToUpper() == enteredText.ToUpper());
            else
                predicate = (se => se.Key.ToUpper().Contains(enteredText.ToUpper()));

            entries
                .Where(predicate)
                .ToList()
                .ForEach(se => collector.AddSuggestion(new SuggestionInfo(se.Key, se.Key, se.Comment, icon, SuggestionUtils.EvalMatch(enteredText, se.Key), se.Message)));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            SampleEntry entry = entries.FirstOrDefault(se => se.Key.ToUpper() == expression.ToUpper());

            if (entry != null)
            {
                StringBuilder sb = new StringBuilder();

                if (action == ACTION_NAME)
                    sb.Append("Keyword is active, ");

                sb.Append(entry.Message);

                System.Windows.MessageBox.Show(sb.ToString());
            }
            else
            {
                options.ErrorText = "Cannot execute!";
                options.PreventClose = true;
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Executed from suggestion, ");
            sb.Append(suggestion.Data as string);

            System.Windows.MessageBox.Show(sb.ToString());
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(ACTION_KEYWORD, ACTION_NAME, ACTION_DISPLAY_NAME, ACTION_COMMENT);
        }

        public string DisplayName => MODULE_DISPLAY_NAME;

        public ImageSource Icon => icon;

        public string Name => MODULE_NAME;
    }
}
