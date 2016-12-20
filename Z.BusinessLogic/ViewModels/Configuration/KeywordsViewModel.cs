using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.BusinessLogic.ViewModels.Configuration
{
    public class KeywordsViewModel : BaseConfigurationViewModel
    {
        public const string PAGE_DISPLAY_NAME = "Keywords";

        public KeywordsViewModel(IConfigurationService configurationService)
        {

        }

        public override void Save()
        {

        }

        public override string DisplayName
        {
            get
            {
                return PAGE_DISPLAY_NAME;
            }
        }
    }
}
