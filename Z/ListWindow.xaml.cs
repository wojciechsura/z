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
using Z.Dependencies;
using Microsoft.Practices.Unity;
using Z.BusinessLogic;
using Z.BusinessLogic.ViewModels;
using System.ComponentModel;
using Z.BusinessLogic.ViewModels.Main;
using Z.Controls;

namespace Z
{
    /// <summary>
    /// Interaction logic for ListWindow.xaml
    /// </summary>
    public partial class ListWindow : BaseSubWindow, IListWindowAccess
    {
        private readonly ListViewModel viewModel;

        protected override void OnClosing(CancelEventArgs e)
        {            
            e.Cancel = true;
        }

        public ListWindow()
        {
            viewModel = Dependencies.Container.Instance.Resolve<MainViewModel>().ListViewModel;
            viewModel.ListWindowAccess = this;

            InitializeComponent();

            this.DataContext = viewModel;
        }

        public void EnsureSelectedIsVisible()
        {
            mainListBox.ScrollIntoView(mainListBox.SelectedItem);
        }

        private void HandleListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.ListDoubleClick();
        }

        private void HandleWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                viewModel.ListWindowEnterPressed();
                e.Handled = true;
            }
        }

        private void mainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainListBox.ScrollIntoView(mainListBox.SelectedItem);
        }
    }
}
