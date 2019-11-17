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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autofac;
using RLCCore;
using RLCServerApplication.ViewModels;

namespace RLCServerApplication.Views
{
    /// <summary>
    /// Interaction logic for RemoteClientsView.xaml
    /// </summary>
    public partial class RemoteClientsView : UserControl
    {
        public RemoteClientsView()
        {
            InitializeComponent();
            comboRemovableDrives.DropDownOpened += ComboRemovableDrives_DropDownOpened;
            ListViewClients.SelectionChanged += ListViewClients_SelectionChanged;
        }

        private void ListViewClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoteClientsViewModel viewModel = DataContext as RemoteClientsViewModel;
            viewModel.ExportClientDataCommand.RaiseCanExecuteChanged();
            viewModel.ExportClientDataToSDCommand.RaiseCanExecuteChanged();
        }

        private void ComboRemovableDrives_DropDownOpened(object sender, EventArgs e)
        {
            RemoteClientsViewModel viewModel = DataContext as RemoteClientsViewModel;
            viewModel.UpdateRemovableDrives();
        }

        private void ListViewClients_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if(r.VisualHit.GetType() != typeof(ListBoxItem))
                ListViewClients.UnselectAll();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RemoteClientsViewModel viewModel = DataContext as RemoteClientsViewModel;
            viewModel.OpenClientEditorCommand.Execute(null);
        }
    }
}
