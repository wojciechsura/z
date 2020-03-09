using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.BusinessLogic.ViewModels.Main
{
    public interface IMainWindowAccess
    {
        void Show();
        void Hide();
        void OpenConfiguration();
        void ShowList();
        void HideList();
        bool? SelectSuggestion(SuggestionChoiceViewModel suggestionChoiceViewModel);

        int CaretPosition { get; set; }
        Point Position { get; set; }
        Point RelativePosition { get; set; }
        bool IsVisible { get; }
    }
}
