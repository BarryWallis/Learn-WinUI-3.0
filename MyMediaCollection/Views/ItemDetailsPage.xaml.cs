using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using MyMediaCollection.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyMediaCollection.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemDetailsPage : Page
    {
        public ItemDetailsViewModel ViewModel { get; } = (Application.Current as App)?.Container.GetService<ItemDetailsViewModel>();

        public ItemDetailsPage() => InitializeComponent();

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int selectItemId = (int)e.Parameter;
            if (selectItemId > 0)
            {
                ViewModel.InitializeItemDetailData(selectItemId); 
            }
        }
    }
}
