
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using MyMediaCollection.ViewModels;

using Windows.Storage;

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

        public ItemDetailsPage()
        {
            InitializeComponent();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string haveExplainedSaveSetting = localSettings.Values[nameof(SavingTip)] as string;
            if (!bool.TryParse(haveExplainedSaveSetting, out bool result) || !result)
            {
                SavingTip.IsOpen = true;
                localSettings.Values[nameof(SavingTip)] = "true";
            }
        }

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int selectItemId = (int)e.Parameter;
            if (selectItemId > 0)
            {
                await ViewModel.InitializeItemDetailDataAsync(selectItemId);
            }
        }
    }
}
