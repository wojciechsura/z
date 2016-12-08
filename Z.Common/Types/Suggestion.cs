using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Common.Types
{
    public sealed class Suggestion
    {
        public Suggestion(string display, object data = null)
        {
            this.Display = display;
            this.Data = data;
        }

        public string Display { get; private set; }
        public object Data { get; private set; }
    }
}
