using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Common.Types
{
    public class KeywordActionInfo
    {
        public KeywordActionInfo(string defaultKeyword, string internalActionName, string displayName)
        {
            DefaultKeyword = defaultKeyword;
            InternalActionName = internalActionName;
            DisplayName = displayName;
        }

        public string DefaultKeyword { get; private set; }
        public string InternalActionName { get; private set; }
        public string DisplayName { get; private set; }
    }
}
