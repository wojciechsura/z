using HashModule.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace HashModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "HashGenerator";
        private const string HASH_ACTION_KEYWORD = "hash";
        private const string HASH_ACTION_NAME = "Hash";

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            if (action == HASH_ACTION_NAME)
            {
                MD5 md5Hash = new MD5CryptoServiceProvider();
                var md5 = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(enteredText));
                var md5String = String.Concat(md5
                    .Select(s => s.ToString("x2")))
                    .ToLower();

                collector.AddSuggestion(new SuggestionInfo(md5String, $"{md5String.Substring(0, 16)}...", "Copy MD5 to clipboard", null, 100));
                
                SHA1 sha1Hash = new SHA1CryptoServiceProvider();
                var sha1 = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(enteredText));
                var sha1String = String.Concat(sha1
                    .Select(s => s.ToString("x2")))
                    .ToLower();

                collector.AddSuggestion(new SuggestionInfo(sha1String, $"{sha1String.Substring(0, 16)}...", "Copy SHA1 to clipboard", null, 100));
            }
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            options.PreventClose = true;
            options.ErrorText = "Choose one of suggestions to copy hash to clipboard!";
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            Clipboard.SetText(suggestion.Text);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(HASH_ACTION_KEYWORD, HASH_ACTION_NAME, Strings.Hash_ActionDisplay, Strings.Hash_ActionComment);            
        }

        public string Name => MODULE_NAME;
        public string DisplayName => Strings.Hash_ModuleDisplayName;
        public ImageSource Icon => null;
    }
}
