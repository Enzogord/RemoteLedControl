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
            viewModel.RLCProjectController.NetworkController.UpdateIPAddresses();
        }
    }
}
