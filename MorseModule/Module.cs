using MorseModule.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace MorseModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "MorseTranslator";
        private const string MORSE_ACTION_KEYWORD = "morse";
        private const string MORSE_ACTION_NAME = "Morse";

        private const char DOT = '.';
        private const char DASH = '-';
        private static readonly Dictionary<char, string> morseData = new Dictionary<char, string>()
            {
                { 'a', string.Concat(DOT, DASH)},
                { 'b', string.Concat(DASH, DOT, DOT, DOT)},
                { 'c', string.Concat(DASH, DOT, DASH, DOT)},
                { 'd', string.Concat(DASH, DOT, DOT)},
                { 'e', DOT.ToString()},
                { 'f', string.Concat(DOT, DOT, DASH, DOT)},
                { 'g', string.Concat(DASH, DASH, DOT)},
                { 'h', string.Concat(DOT, DOT, DOT, DOT)},
                { 'i', string.Concat(DOT, DOT)},
                { 'j', string.Concat(DOT, DASH, DASH, DASH)},
                { 'k', string.Concat(DASH, DOT, DASH)},
                { 'l', string.Concat(DOT, DASH, DOT, DOT)},
                { 'm', string.Concat(DASH, DASH)},
                { 'n', string.Concat(DASH, DOT)},
                { 'o', string.Concat(DASH, DASH, DASH)},
                { 'p', string.Concat(DOT, DASH, DASH, DOT)},
                { 'q', string.Concat(DASH, DASH, DOT, DASH)},
                { 'r', string.Concat(DOT, DASH, DOT)},
                { 's', string.Concat(DOT, DOT, DOT)},
                { 't', string.Concat(DASH)},
                { 'u', string.Concat(DOT, DOT, DASH)},
                { 'v', string.Concat(DOT, DOT, DOT, DASH)},
                { 'w', string.Concat(DOT, DASH, DASH)},
                { 'x', string.Concat(DASH, DOT, DOT, DASH)},
                { 'y', string.Concat(DASH, DOT, DASH, DASH)},
                { 'z', string.Concat(DASH, DASH, DOT, DOT)},
                { '0', string.Concat(DASH, DASH, DASH, DASH, DASH)},
                { '1', string.Concat(DOT, DASH, DASH, DASH, DASH)},
                { '2', string.Concat(DOT, DOT, DASH, DASH, DASH)},
                { '3', string.Concat(DOT, DOT, DOT, DASH, DASH)},
                { '4', string.Concat(DOT, DOT, DOT, DOT, DASH)},
                { '5', string.Concat(DOT, DOT, DOT, DOT, DOT)},
                { '6', string.Concat(DASH, DOT, DOT, DOT, DOT)},
                { '7', string.Concat(DASH, DASH, DOT, DOT, DOT)},
                { '8', string.Concat(DASH, DASH, DASH, DOT, DOT)},
                { '9', string.Concat(DASH, DASH, DASH, DASH, DOT)}
            };
        private static readonly Regex MorseRegex = new Regex("[\\.\\-]+");
        private static readonly Regex IsMorseRegex = new Regex("^[\\.\\- ]+$");

        private string Encode(string plain)
        {
            List<string> result = new List<string>();

            foreach (var ch in plain.ToLowerInvariant()) 
            {
                if (morseData.ContainsKey(ch))
                    result.Add(morseData[ch]);
            }

            return string.Join(" ", result);
        }

        private string Decode(string morse)
        {
            StringBuilder result = new StringBuilder();

            var matches = MorseRegex.Matches(morse);

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var morseString = match.Value;

                foreach (var kvp in morseData)
                {
                    if (kvp.Value.Equals(morseString))
                    {
                        result.Append(kvp.Key);
                        break;
                    }
                }
            }

            return result.ToString();
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            if (action == MORSE_ACTION_NAME)
            {
                var result = Encode(enteredText);
                var display = result.Length > 64 ? $"{result.Substring(0, 64)}..." : result;
                collector.AddSuggestion(new SuggestionInfo(result, display, null, null, 100));
            }
            else if (IsMorseRegex.IsMatch(enteredText))
            {
                var result = Decode(enteredText);
                var display = result.Length > 64 ? $"{result.Substring(0, 64)}..." : result;
                collector.AddSuggestion(new SuggestionInfo(result, display, null, null, 100));
            }
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            options.PreventClose = true;
            options.ErrorText = Strings.Morse_Message_ChoseSuggestion;
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            Clipboard.SetText(suggestion.Text);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(MORSE_ACTION_KEYWORD, MORSE_ACTION_NAME, Strings.Morse_ActionDisplay, null);
        }

        public string Name => MODULE_NAME;
        public string DisplayName => Strings.Morse_ModuleDisplayName;
        public ImageSource Icon => null;
    }
}
