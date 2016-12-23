using PowerModule.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace PowerModule
{
    public class Module : IZModule
    {
        // Private types ------------------------------------------------------

        private class PowerInfo
        {
            public PowerInfo(string command, string display, string comment, Action action)
            {
                Command = command;
                Display = display;
                Comment = comment;
                Action = action;
            }

            public string Command { get; private set; }
            public string Display { get; private set; }
            public string Comment { get; private set; }
            public Action Action { get; private set; }
        }

        private const string MODULE_DISPLAY_NAME = "Power";
        private const string MODULE_NAME = "Power";
        private const string ACTION_KEYWORD = "power";
        private const string ACTION_NAME = "Power";
        private const string ACTION_DISPLAY_NAME = "Power";
        private const string ACTION_COMMENT = "Computer power commands";

        // Private fields -----------------------------------------------------

        private readonly List<PowerInfo> powerInfos = new List<PowerInfo>
        {
            new PowerInfo("shutdown", "Shutdown", "Closes Windows and shuts down the computer.", () => WinapiInterop.Shutdown()),
            new PowerInfo("reboot", "Reboot", "Closes Windows and reboots the computer.", () => WinapiInterop.Reboot()),
            new PowerInfo("sleep", "Sleep", "Puts computer in the suspended state.", () => WinapiInterop.Sleep()),
            new PowerInfo("logoff", "Log off", "Logs off current user", () => WinapiInterop.Logoff()),
            new PowerInfo("hibernate", "Hibernate", "Hibernates current Windows session and shuts down the computer.", () => WinapiInterop.Hibernate()),
            new PowerInfo("lock", "Lock", "Locks the current session", () => WinapiInterop.Lock())
        };

        private readonly ImageSource icon;

        // Public methods -----------------------------------------------------

        public Module()
        {
            icon = new BitmapImage(new Uri("pack://application:,,,/PowerModule;component/Resources/power.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<PowerInfo, bool> filter;
            if (perfectMatchesOnly)
                filter = pi => pi.Command.ToUpper() == enteredText.ToUpper();
            else
                filter = pi => pi.Command.ToUpper().Contains(enteredText.ToUpper());

            powerInfos
                .Where(filter)
                .Select(pi => new SuggestionInfo(pi.Command, pi.Display, pi.Comment, icon, pi))
                .OrderBy(pi => pi.Display)
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            var powerInfo = powerInfos.Where(pi => pi.Command.ToUpper() == expression.ToUpper())
                .FirstOrDefault();

            if (powerInfo != null)
                powerInfo.Action();
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            PowerInfo info = suggestion.Data as PowerInfo;
            if (info != null)
                info.Action();
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(ACTION_KEYWORD, ACTION_NAME, ACTION_DISPLAY_NAME, ACTION_COMMENT);
        }

        // Public properties --------------------------------------------------

        public string DisplayName
        {
            get
            {
                return MODULE_DISPLAY_NAME;
            }
        }

        public string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public ImageSource Icon => icon;
    }
}
