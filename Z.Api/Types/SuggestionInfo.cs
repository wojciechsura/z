﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Api.Types
{
    public sealed class SuggestionInfo
    {
        public SuggestionInfo(string text, string display, string comment, object data = null)
        {
            this.Text = text;
            this.Display = display;
            this.Comment = comment;            
            this.Data = data;
        }

        public string Display { get; private set; }
        public string Comment { get; private set; }
        public string Text { get; private set; }
        public object Data { get; private set; }
    }
}
