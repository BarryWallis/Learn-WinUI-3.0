
using Microsoft.UI.Xaml;

using WebViewBrowser.Bus.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WebViewBrowser
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            MainWebView.Source = BrowserToolbar.UrlSource;
        }

        private void BrowserToolbar_ReloadClicked(object sender, RoutedEventArgs e) => MainWebView.Reload();

        private void BrowserToolbar_UrlEntered(object sender, RoutedEventArgs e) => MainWebView.Source = BrowserToolbar.UrlSource;
    }
}
