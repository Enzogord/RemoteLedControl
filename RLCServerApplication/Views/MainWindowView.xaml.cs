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
using NAudioPlayer;
using RLCServerApplication.ViewModels;

namespace RLCServerApplication.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        public void InitPlayer()
        {
            Player player = ViewModel.Player;
            player.Init(App.Current.Dispatcher);
            player.PropertyChanged += Player_PropertyChanged;
            Player.RegisterSoundPlayer(player);
            Resources.MergedDictionaries.Clear();
            ResourceDictionary themeResources = Application.LoadComponent(new Uri("Resources/ExpressionDark.xaml", UriKind.Relative)) as ResourceDictionary;            
            Resources.MergedDictionaries.Add(themeResources);
        }

        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Player player = ViewModel.Player;
            switch(e.PropertyName) {
                case "ChannelPosition":
                    LabelTime.Content = TimeSpan.FromSeconds(player.ChannelPosition).ToString(@"hh\:mm\:ss");
                    break;
                default:
                    // Do Nothing
                    break;
            }
        }
    }
}
