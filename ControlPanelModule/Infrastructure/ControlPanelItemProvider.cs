using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanelModule.Infrastructure
{
    static class ControlPanelItemProvider
    {
        public static IEnumerable<BaseControlPanelEntry> GetEntries()
        {
            RegistryKey namespaceKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
            RegistryKey classesRoot = Registry.ClassesRoot;

            foreach (var ns in namespaceKey.GetSubKeyNames())
            {
                var entry = classesRoot.OpenSubKey($@"CLSID\{ns}");
                if (entry == null)
                    continue;

                var name = entry.GetValue(null, null) as string;

                var localizedStringLocation = entry.GetValue("LocalizedString", null) as string;

                string localizedString;
                if (localizedStringLocation != null)
                    localizedString = WinapiInterop.GetStringResource(localizedStringLocation) ?? name;
                else
                    localizedString = name;

                string infoTipLocalization = entry.GetValue("InfoTip", null) as string;
                string infoTip = infoTipLocalization != null ? WinapiInterop.GetStringResource(infoTipLocalization) : null;

                // TODO get icon
                var defaultIconKey = entry.OpenSubKey("DefaultIcon");
                var defaultIconLocation = defaultIconKey?.GetValue(null, null) as string;

                var entryCommandKey = entry.OpenSubKey(@"Shell\Open\Command");
                if (entryCommandKey != null)
                {
                    var command = entryCommandKey.GetValue(null, null) as string;

                    if (name != null && localizedString != null && command != null)
                    {
                        yield return new CommandControlPanelEntry(ns, name, localizedString, infoTip, command);
                        continue;
                    }
                }

                var shellFolderKey = entry.OpenSubKey("ShellFolder");
                if (shellFolderKey != null)
                {
                    if (name != null && localizedString != null)
                    {
                        yield return new ShellFolderControlPanelEntry(ns, name, localizedString, infoTip);
                        continue;
                    }
                }
            }
        }
    }
}
