using System.Windows;

namespace RLCServerApplication.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
            VersionText.Text = $"Версия {App.Version}";
        }
    }
}
