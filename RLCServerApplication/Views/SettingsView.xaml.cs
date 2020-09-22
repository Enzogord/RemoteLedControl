using System;
using System.Windows.Controls;
using RLCServerApplication.ViewModels;

namespace RLCServerApplication.Views
{
    /// <summary>
    /// Interaction logic for SetingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            comboIpAdresses.DropDownOpened += ComboIpAdresses_DropDownOpened;
        }

        private void ComboIpAdresses_DropDownOpened(object sender, EventArgs e)
        {
            SettingsViewModel viewModel = DataContext as SettingsViewModel;
            viewModel.NetworkController.UpdateIPAddresses();
        }
    }
}
