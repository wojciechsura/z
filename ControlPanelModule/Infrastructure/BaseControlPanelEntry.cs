using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanelModule.Infrastructure
{
    abstract class BaseControlPanelEntry
    {
        public BaseControlPanelEntry(string ns, string name, string displayName)
        {
            Namespace = ns;
            Name = name;
            DisplayName = displayName;
        }

        public string Namespace { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
    }
}
