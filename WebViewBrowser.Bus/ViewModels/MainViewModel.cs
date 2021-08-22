namespace WebViewBrowser.Bus.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _urlSource = "https://www.packtpub.com/";
        public string UrlSource
        {
            get => _urlSource;
            set => _ = SetProperty(ref _urlSource, value);
        }

    }
}
