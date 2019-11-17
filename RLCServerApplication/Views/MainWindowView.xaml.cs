using RLCServerApplication.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;

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
            ViewModel.Player.Init(Application.Current.Dispatcher);
            Player.RegisterSoundPlayer(ViewModel.Player);
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Shutdown(0);
            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(MessageBox.Show("Все не сохраненные данные будут утеряны, продолжить?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }
    }
}
