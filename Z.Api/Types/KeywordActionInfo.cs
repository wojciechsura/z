using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Api.Types
{
    public sealed class KeywordInfo
    {
        public KeywordInfo(string defaultKeyword, string name, string displayName, string comment)
        {
            DefaultKeyword = defaultKeyword;
            Name = name;
            DisplayName = displayName;
            Comment = comment;
        }

        /// <summary>
        /// Default keyword for this action (can be overridden by user in settings)
        /// </summary>
        public string DefaultKeyword { get; private set; }
        /// <summary>
        /// Internal name for this action (should be unique among actions in module)
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Name of the action as displayed to user in main window
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// Human-readable comment for action, describing what it does.
        /// </summary>
        public string Comment { get; private set; }
    }
}
