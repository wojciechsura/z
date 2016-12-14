﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Api.Types
{
    public sealed class KeywordInfo
    {
        public KeywordInfo(string defaultKeyword, string actionName, string displayName)
        {
            DefaultKeyword = defaultKeyword;
            ActionName = actionName;
            DisplayName = displayName;
        }

        public string DefaultKeyword { get; private set; }
        public string ActionName { get; private set; }
        public string DisplayName { get; private set; }
    }
}