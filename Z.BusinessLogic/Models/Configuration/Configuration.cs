using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Z.BusinessLogic.Models.Configuration
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        public Configuration()
        {
            for (int i = 0; i < 3; i++)
            {
                var item = new LauncherShortcut { Name = $"Item {i + 1}" };
                Launcher.Items.Add(item);

                if (i < 2)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        var subitem = new LauncherShortcut { Name = $"Subitem {j + 1}" };
                        item.SubItems.Add(subitem);
                    }
                }
            }
        }

        [XmlElement("Hotkey")]
        public Hotkey Hotkey { get; set; } = new Hotkey();
        [XmlElement("MainWindow")]
        public MainWindow MainWindow { get; set; } = new MainWindow();
        [XmlElement("ProCalcWindow")]
        public ProCalcWindow ProCalcWindow { get; set; } = new ProCalcWindow();
        [XmlElement("Behavior")]
        public Behavior Behavior { get; set; } = new Behavior();
        [XmlElement("Keywords")]
        public Keywords Keywords { get; set; } = new Keywords();
        [XmlElement("General")]
        public General General { get; set; } = new General();
        [XmlElement("Launcher")]
        public Launcher Launcher { get; set; } = new Launcher();
    }
}
