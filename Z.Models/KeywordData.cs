﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;

namespace Z.Models
{
    public class KeywordData
    {
        public KeywordData(string keyword, string actionName, string displayName, IZModule module)
        {
            Keyword = keyword;
            ActionName = actionName;
            DisplayName = displayName;
            Module = module;
        }

        public string Keyword { get; private set; }
        public string ActionName { get; private set; }
        public string DisplayName { get; private set; }
        public IZModule Module { get; set; }
    }
}