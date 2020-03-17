using PowerModule.Infrastructure;
using PowerModule.Resources;
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
using Z.Api.Utils;

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

        private const string MODULE_NAME = "Power";
        private const string ACTION_KEYWORD = "power";
        private const string ACTION_NAME = "Power";

        // Private fields -----------------------------------------------------

        private readonly List<PowerInfo> powerInfos = new List<PowerInfo>
        {
            new PowerInfo("shutdown", Strings.Power_Shutdown, Strings.Power_Shutdown_Comment, () => WinapiInterop.Shutdown()),
            new PowerInfo("reboot", Strings.Power_Reboot, Strings.Power_Reboot_Comment, () => WinapiInterop.Reboot()),
            new PowerInfo("sleep", Strings.Power_Sleep, Strings.Power_Sleep_Comment, () => WinapiInterop.Sleep()),
            new PowerInfo("logoff", Strings.Power_LogOff, Strings.Power_LogOff_Comment, () => WinapiInterop.Logoff()),
            new PowerInfo("hibernate", Strings.Power_Hibernate, Strings.Power_Hibernate_Comment, () => WinapiInterop.Hibernate()),
            new PowerInfo("lock", Strings.Power_Lock, Strings.Power_Lock_Comment, () => WinapiInterop.Lock())
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
                .Select(pi => new SuggestionInfo(pi.Command, pi.Display, pi.Comment, icon, SuggestionUtils.EvalMatch(enteredText, pi.Command), pi))
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
            yield return new KeywordInfo(ACTION_KEYWORD, ACTION_NAME, Strings.Power_ActionDisplayName, Strings.Power_ActionComment);
        }

        // Public properties --------------------------------------------------

        public string DisplayName
        {
            get
            {
                return Strings.Power_ModuleDisplayName;
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
