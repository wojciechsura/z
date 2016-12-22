﻿using System;
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

        public string DefaultKeyword { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Comment { get; private set; }
    }
}
