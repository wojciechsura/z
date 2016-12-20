using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Z.BusinessLogic.ViewModels;
using Z.BusinessLogic.ViewModels.Interfaces;

namespace Z
{
    /// <summary>
    /// Interaction logic for SuggestionChoiceWindow.xaml
    /// </summary>
    public partial class SuggestionChoiceWindow : Window, ISuggestionChoiceWindowAccess
    {
        private readonly SuggestionChoiceViewModel viewModel;

        private void HandleWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                viewModel.EnterPressed();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                viewModel.EscapePressed();
                e.Handled = true;
            }
        }

        public SuggestionChoiceWindow(SuggestionChoiceViewModel viewModel)
        {
            InitializeComponent();

            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            this.viewModel = viewModel;
            viewModel.SuggestionChoiceWindowAccess = this;
            DataContext = viewModel;
        }

        public void CloseWindow(bool result)
        {
            this.DialogResult = result;
            Close();
        }
    }
}
