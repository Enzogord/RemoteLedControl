using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
