using IWshRuntimeLibrary;
using ShortcutModule;
using StartMenuModule.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api.Interfaces;
using Z.Api.Types;


namespace StartMenuModule
{
    public class Module : BaseFilesystemShortcutModule
    {
        private const string MODULE_NAME = "StartMenu";

        private const string ACTION_KEYWORD = "start";
        private const string ACTION_NAME = "Start";

        private readonly ImageSource icon;

        protected override List<(string path, bool recursive)> GetShortcutDirectories()
        {
            return new List<(string path, bool recursive)>
            {
                (Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), true),
                (Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), true)
            };
        }

        protected override string FormatError(string errorText)
        {
            return string.Format(Resources.Strings.Start_CannotExecute, errorText);
        }

        // Protected properties -----------------------------------------------

        protected override string ActionKeyword => ACTION_KEYWORD;
        protected override string ActionName => ACTION_NAME;
        protected override string ActionDisplay => Strings.Start_ActionDisplayName;
        protected override string ActionComment => Strings.Start_ActionComment;

        // Public methods -----------------------------------------------------

        public Module()
        {
            icon = new BitmapImage(new Uri("pack://application:,,,/StartMenuModule;component/Resources/winlogo.png"));
        }

        // Public properties --------------------------------------------------

        public override string DisplayName => Strings.Start_ModuleDisplayName;

        public override string Name => MODULE_NAME;

        public override ImageSource Icon => icon;
    }
}
