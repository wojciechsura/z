using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Z.BusinessLogic.ViewModels.Configuration.Keywords
{
    public class KeywordOverrideViewModel : INotifyPropertyChanged
    {
        private bool @override;
        private string keyword;
        private string defaultKeyword;
        private string moduleName;
        private string actionName;
        private string moduleDisplayName;
        private string actionDisplayName;
        private ImageSource icon;

        public KeywordOverrideViewModel(string defaultKeyword, string keywordOverride, string moduleName, string actionName, string moduleDisplayName, string actionDisplayName, ImageSource icon)
        {
            this.keyword = keywordOverride;
            this.defaultKeyword = defaultKeyword;
            this.@override = keywordOverride != defaultKeyword;
            this.moduleName = moduleName;
            this.actionName = actionName;
            this.moduleDisplayName = moduleDisplayName;
            this.actionDisplayName = actionDisplayName;
            this.icon = icon;
        }

        private void OnOverrideChanged()
        {
            if (!@override)
            {
                PublishKeyword(DefaultKeyword);
            }
        }

        private void OnKeywordChanged()
        {
            if (keyword != DefaultKeyword)
            {
                PublishOverride(true);
            }
        }

        private void PublishKeyword(string defaultKeyword)
        {
            keyword = defaultKeyword;
            OnPropertyChanged(nameof(Keyword));
        }

        private void PublishOverride(bool @override)
        {
            this.@override = @override;
            OnPropertyChanged(nameof(Override));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }        

        public string ModuleName => moduleName;

        public string ActionName => actionName;

        public string ModuleDisplayName => moduleDisplayName;

        public string ActionDisplayName => actionDisplayName;

        public string DefaultKeyword => defaultKeyword;

        public string Keyword
        {
            get
            {
                return keyword;
            }
            set
            {
                keyword = value;
                OnKeywordChanged();
            }
        }

        public bool Override
        {
            get
            {
                return @override;
            }
            set
            {
                @override = value;
                OnOverrideChanged();
            }
        }

        public ImageSource Icon => icon;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
