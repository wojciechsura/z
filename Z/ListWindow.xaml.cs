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
using Z.ViewModels.Interfaces;
using Microsoft.Practices.Unity;

namespace Z
{
    /// <summary>
    /// Interaction logic for ListWindow.xaml
    /// </summary>
    public partial class ListWindow : Window, IListWindowAccess
    {
        private readonly IListWindowViewModel viewModel;

        public ListWindow()
        {
            InitializeComponent();

            viewModel = Container.Instance.Resolve<IListWindowViewModel>();
            viewModel.ListWindowAccess = this;

            this.DataContext = viewModel;
        }

        public void EnsureSelectedIsVisible()
        {
            mainListBox.ScrollIntoView(mainListBox.SelectedItem);
        }
    }
}
