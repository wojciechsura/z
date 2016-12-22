using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using Z.Api.Types;

namespace Z.Api.Interfaces
{
    public interface IZModule
    {
        /// <summary>
        /// Returns suggestions for given text entered by user
        /// </summary>
        /// <param name="enteredText">Text entered by user.</param>
        /// <param name="action">Action name, if keyword is active, otherwise null.</param>
        /// <param name="perfectMatchesOnly">Whether only perfect matches (non-partial) should be returned.</param>
        /// <param name="collector">Use this object to provide suggestions. Should not be stored.</param>
        void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector);

        /// <summary>
        /// Perform module-defined action for specified text.
        /// </summary>
        /// <param name="action">Action name, if keyword is active, otherwise null.</param>
        /// <param name="expression">Text entered by user.</param>
        /// <param name="options">Options object - allows module to specify additional application behavior.</param>
        void ExecuteKeywordAction(string action, string expression, ExecuteOptions options);

        /// <summary>
        /// Perform module-defined action for specified suggestion
        /// </summary>
        /// <param name="suggestion">Suggestion chosen by user</param>
        /// <param name="options">Options object - allows module to specify additional application behavior.</param>
        void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options);

        /// <summary>
        /// Returns all keywords defined by this module.
        /// </summary>
        IEnumerable<KeywordInfo> GetKeywordActions();

        /// <summary>
        /// Called once, after registering module in application. May be used to load configuration.
        /// </summary>
        /// <param name="context">Gives access, to some application resources. May be stored.</param>
        void Initialize(IModuleContext context);

        /// <summary>
        /// Called once, just before application closing. May be used to free resources, store some data etc.
        /// </summary>
        void Deinitialize();

        /// <summary>
        /// Returns object, which is used to display and manage module's configuration.
        /// </summary>
        IConfigurationProvider GetConfigurationProvider();

        /// <summary>
        /// Module's internal name. Can consist of letters, digits, underscore and dot (except first character).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Module's name displayed to user
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Module's 16x16 icon. May be null.
        /// </summary>
        ImageSource Icon { get; }
    }
}
