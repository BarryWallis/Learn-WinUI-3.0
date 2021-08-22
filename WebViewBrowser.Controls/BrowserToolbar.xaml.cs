using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WebViewBrowser.Controls
{
    public sealed partial class BrowserToolbar : UserControl
    {
        private const string InitialUrl = "https://www.packtpub.com/";

        public event RoutedEventHandler ReloadClicked;
        public event RoutedEventHandler UrlEntered;

        public static readonly DependencyProperty UrlSourceProperty
            = DependencyProperty.Register(nameof(UrlSource), typeof(Uri), typeof(BrowserToolbar), new PropertyMetadata(null));

        public Uri UrlSource { get => (Uri)GetValue(UrlSourceProperty); set => SetValue(UrlSourceProperty, value); }

        public BrowserToolbar()
        {
            InitializeComponent();
            UrlTextBox.Text = InitialUrl;
            UrlSource = new Uri(InitialUrl);
            UrlEntered?.Invoke(this, new RoutedEventArgs());
        }

        private void UrlTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && !string.IsNullOrWhiteSpace(UrlTextBox.Text))
            {
                UrlEntered?.Invoke(this, new RoutedEventArgs());
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e) => ReloadClicked?.Invoke(this, new RoutedEventArgs());
    }
}