using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using MyMediaCollection.ViewModels;

using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyMediaCollection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel => App.ViewModel;

        public MainPage() => InitializeComponent();
    }
}
