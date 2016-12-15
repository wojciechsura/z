using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanelModule.Infrastructure
{
    class CommandControlPanelEntry : BaseControlPanelEntry
    {
        public CommandControlPanelEntry(string ns, string name, string displayName, string command) : base(ns, name, displayName)
        {
            this.Command = command;
        }

        public string Command { get; private set; }
    }
}
