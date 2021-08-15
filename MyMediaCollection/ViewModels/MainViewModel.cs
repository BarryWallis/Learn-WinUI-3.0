using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AppUIBasics;

using Microsoft.UI.Xaml.Input;
//using Microsoft.UI.Xaml.Data;

using MyMediaCollection.Interfaces;
using MyMediaCollection.Models;

namespace MyMediaCollection.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private const string AllMediums = "All";

        private ObservableCollection<MediaItem> _allItems;

        private MediaItem _selectedMediaItem;
        public MediaItem SelectedMediaItem
        {
            get => _selectedMediaItem;
            set
            {
                _ = SetProperty(ref _selectedMediaItem, value);
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        private string _selectedMedium;
        public string SelectedMedium
        {
            get => _selectedMedium;
            set
            {
                _ = SetProperty(ref _selectedMedium, value);

                Debug.Assert(Items is not null);
                Items!.Clear();
                foreach (MediaItem item in _allItems!)
                {
                    if (_selectedMedium == "All" || _selectedMedium == item.MediaType.ToString())
                    {
                        Items.Add(item);
                    }
                }

            }
        }

        private ObservableCollection<MediaItem> _items = new();
        public ObservableCollection<MediaItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private IList<string> _mediums;

        public IList<string> Mediums
        {
            get => _mediums;
            set => SetProperty(ref _mediums, value);
        }

        #region Commands
        public ICommand AddEditCommand { get; set; }

        /// <summary>
        /// Add or edit an item in the collection.
        /// </summary>
        public void AddOrEditItem()
        {
            int selectedItemId = -1;
            if (SelectedMediaItem != null)
            {
                selectedItemId = SelectedMediaItem.Id;
            }

            navigationService.NavigateTo("ItemDetailsPage", selectedItemId);
        }

        public ICommand DeleteCommand { get; set; }

        /// <summary>
        /// Determine if an item in the collection can be delete.
        /// </summary>
        /// <returns><see langword="true"/> if an item can be deleted; otherwise <see langword="false"/>.</returns>
        private bool CanDeleteItem() => _selectedMediaItem is not null;

        private void DeleteItem()
        {
            _ = _allItems.Remove(SelectedMediaItem);
            _ = Items.Remove(SelectedMediaItem);
        }
        #endregion

        public MainViewModel(INavigationService navigationService, IDataService dataService)
        {
            this.navigationService = navigationService;
            this.dataService = dataService;
            PopulateData();
            DeleteCommand = new RelayCommand(DeleteItem, CanDeleteItem);
            AddEditCommand = new RelayCommand(AddOrEditItem);
        }

        /// <summary>
        /// Set up temporary data.
        /// </summary>
        private void PopulateData()
        {
            _items.Clear();
            dataService.GetItems().ToList().ForEach(i => _items.Add(i));
            _allItems = new ObservableCollection<MediaItem>(Items);
            _mediums = new ObservableCollection<string> { AllMediums };
            dataService.GetItemTypes().ToList().ForEach(it => _mediums.Add(it.ToString()));
            _selectedMedium = _mediums[0];
        }

        /// <summary>
        /// Add an item when the user double taps a collection item.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e">The event argument(s).</param>
#pragma warning disable IDE0060 // Remove unused parameter
        public void ListViewDoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => AddOrEditItem();
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
