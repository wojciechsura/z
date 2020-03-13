using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Config;
using Z.BusinessLogic.ViewModels.Configuration.Base;
using Z.Common.Types;

namespace Z.BusinessLogic.ViewModels.Configuration.Behavior
{
    public class BehaviorViewModel : BaseConfigurationViewModel, INotifyPropertyChanged
    {
        private const string PAGE_DISPLAY_NAME = "Behavior";

        private readonly IConfigurationService configurationService;

        private EnterBehavior enterBehavior;
        private SuggestionSorting suggestionSorting;
        private int suggestionDelay;

        public BehaviorViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            var configuration = configurationService.Configuration;
            this.enterBehavior = configuration.Behavior.EnterBehavior;
            this.suggestionDelay = configuration.Behavior.SuggestionDelay;
            this.suggestionSorting = configuration.Behavior.SuggestionSorting;
        }

        public override void Save()
        {
            var configuration = configurationService.Configuration;
            configuration.Behavior.EnterBehavior = enterBehavior;
            configuration.Behavior.SuggestionDelay = suggestionDelay;
            configuration.Behavior.SuggestionSorting = suggestionSorting;
        }

        public override IEnumerable<string> Validate()
        {
            return null;
        }

        public override string DisplayName
        {
            get
            {
                return PAGE_DISPLAY_NAME;
            }
        }

        public EnterBehavior EnterBehavior
        {
            get
            {
                return enterBehavior;
            }
            set
            {
                enterBehavior = value;
            }
        }

        public int SuggestionDelay
        {
            get
            {
                return suggestionDelay;
            }
            set
            {
                suggestionDelay = value;
            }
        }

        public SuggestionSorting SuggestionSorting
        {
            get
            {
                return suggestionSorting;
            }
            set
            {
                suggestionSorting = value;
            }
        }
    }
}
