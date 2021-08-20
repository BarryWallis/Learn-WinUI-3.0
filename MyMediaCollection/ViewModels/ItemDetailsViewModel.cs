using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using AppUIBasics;

using Microsoft.UI.Xaml.Input;

using MyMediaCollection.Enums;
using MyMediaCollection.Interfaces;
using MyMediaCollection.Models;

namespace MyMediaCollection.ViewModels
{
    public class ItemDetailsViewModel : BindableBase
    {
        private int _selectedItemId = -1;
        private int _itemId;

        private string _itemName;
        [MinLength(2, ErrorMessage = "Item name must be at least 2 characters.")]
        [MaxLength(100, ErrorMessage = "Item name must be 100 characters or less.")]
        public string ItemName
        {
            get => _itemName;
            set
            {
                IsDirty = SetProperty(ref _itemName, value) || IsDirty;
                IsPageValid = !string.IsNullOrWhiteSpace(value);
            }
        }

        private bool _isPageValid;
        public bool IsPageValid
        {
            get => _isPageValid;
            set => SetProperty(ref _isPageValid, !string.IsNullOrWhiteSpace(ItemName)
                                                                && SelectedLocation is not null
                                                                && SelectedItemType is not null
                                                                && SelectedMedium is not null, nameof(CanBeSaved));
        }

        public bool CanBeSaved => IsDirty && IsPageValid;

        private string _selectedMedium;
        public string SelectedMedium
        {
            get => _selectedMedium;
            set
            {
                IsDirty = SetProperty(ref _selectedMedium, value) || IsDirty;
                IsPageValid = value is not null;
            }
        }


        private string _selectedLocation;
        public string SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                IsDirty = SetProperty(ref _selectedLocation, value) || IsDirty;
                IsPageValid = value is not null;
            }
        }

        private ObservableCollection<string> _mediums = new();

        public void DefaultItemDetailData()
        {
            ItemName = "New Item";
            SelectedLocation = dataService.GetLocationTypes()[0].ToString();
            SelectedItemType = dataService.GetItemTypes()[0].ToString();
            SelectedMedium = dataService.GetMediums(dataService.GetItemTypes()[0])[0].Name;
            IsDirty = false;
            IsPageValid = false;
        }

        private ObservableCollection<string> _itemTypes = new();
        private ObservableCollection<string> _locationTypes = new();

        private string _selectedItemType;
        public string SelectedItemType
        {
            get => _selectedItemType;
            set
            {
                if (SetProperty(ref _selectedItemType, value))
                {
                    IsDirty = true;
                    Mediums.Clear();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        dataService.GetMediums((ItemType)Enum.Parse(typeof(ItemType), SelectedItemType))
                            .Select(m => m.Name)
                            .ToList()
                            .ForEach(n => Mediums.Add(n));
                        Debug.Assert(Mediums.Count > 0);
                    }

                    //(SaveCommand as RelayCommand).RaiseCanExecuteChanged();
                    IsPageValid = !string.IsNullOrWhiteSpace(value);
                }
            }
        }

        //public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            private set => SetProperty(ref _isDirty, value, nameof(CanBeSaved));
        }

        public ObservableCollection<string> Mediums { get => _mediums; set => SetProperty(ref _mediums, value); }

        public ObservableCollection<string> ItemTypes { get => _itemTypes; set => SetProperty(ref _itemTypes, value); }

        public ObservableCollection<string> LocationTypes { get => _locationTypes; set => SetProperty(ref _locationTypes, value); }

        public ItemDetailsViewModel(INavigationService navigationService, IDataService dataService)
        {
            this.navigationService = navigationService;
            this.dataService = dataService;
            //SaveCommand = new RelayCommand(SaveItem, CanSaveItem);
            CancelCommand = new RelayCommand(Cancel);
            PopulateLists();
            IsDirty = false;
            IsPageValid = false;

        }

        /// <summary>
        /// Populate the properties with the exising item data.
        /// </summary>
        /// <param name="dataService">The data service to recieve the data from.</param>
        private async Task PopulateExistingItemAsync(IDataService dataService)
        {
            if (_selectedItemId > 0)
            {
                MediaItem item = await dataService.GetItemAsync(_selectedItemId);
                Mediums.Clear();
                dataService.GetMediums(item.MediaType).Select(m => m.Name).ToList().ForEach(n => Mediums.Add(n));
                _itemId = item.Id;
                ItemName = item.Name;
                SelectedMedium = item.MediumInfo.Name;
                SelectedLocation = item.Location.ToString();
                SelectedItemType = item.MediaType.ToString();
            }
        }

        /// <summary>
        /// Populate the list properties with the existing item data,
        /// </summary>
        private void PopulateLists()
        {
            ItemTypes.Clear();
            Enum.GetNames(typeof(ItemType)).ToList().ForEach(it => ItemTypes.Add(it));

            LocationTypes.Clear();
            Enum.GetNames(typeof(LocationType)).ToList().ForEach(lt => LocationTypes.Add(lt));

            Mediums = new();
        }

        /// <summary>
        /// Cancel the update.
        /// </summary>
        private void Cancel() => navigationService.GoBack();

        /// <summary>
        /// Save a media item.
        /// </summary>
        private async Task SaveItemAsync()
        {
            MediaItem mediaItem;
            if (_itemId > 0)
            {
                mediaItem = await dataService.GetItemAsync(_itemId);
                mediaItem.Name = ItemName;
                mediaItem.Location = (LocationType)Enum.Parse(typeof(LocationType), SelectedLocation);
                mediaItem.MediaType = (ItemType)Enum.Parse(typeof(ItemType), SelectedItemType);
                mediaItem.MediumInfo = dataService.GetMedium(SelectedMedium);
                await dataService.UpdateItemAsync(mediaItem);
            }
            else
            {
                mediaItem = new()
                {
                    Name = ItemName,
                    Location = (LocationType)Enum.Parse(typeof(LocationType), SelectedLocation),
                    MediaType = (ItemType)Enum.Parse(typeof(ItemType), SelectedItemType),
                    MediumInfo = dataService.GetMedium(SelectedMedium)
                };

                _ = await dataService.AddItemAsync(mediaItem);
            }
        }

        /// <summary>
        /// Does the current item need to be saved and can it be saved? 
        /// </summary>
        /// <returns><see langword="true"/> if it needs to be saved and can be saved, otherwise <see langword="false"/>.</returns>
        //private bool CanSaveItem() => IsDirty
        //                              && !string.IsNullOrWhiteSpace(ItemName)
        //                              && !string.IsNullOrWhiteSpace(SelectedItemType)
        //                              && !string.IsNullOrWhiteSpace(SelectedMedium)
        //                              && !string.IsNullOrWhiteSpace(SelectedLocation);

        /// <summary>
        /// Iniitialize the data for the item details page.
        /// </summary>
        /// <param name="selectItemId">The seleted item when the page is initialized.</param>
        public async Task InitializeItemDetailDataAsync(int selectItemId)
        {
            _selectedItemId = selectItemId;
            PopulateLists();
            await PopulateExistingItemAsync(dataService);
            IsDirty = false;
            IsPageValid = false;
        }

        /// <summary>
        /// Save the current item and initilize the page to add a new item.
        /// </summary>
        public async Task SaveItemAndContinueAsync()
        {
            await SaveItemAsync();
            _itemId = 0;
            ItemName = string.Empty;
            SelectedMedium = null;
            SelectedLocation = null;
            SelectedItemType = null;
            IsDirty = false;
            IsPageValid = false;
        }

        /// <summary>
        /// Save the current item and return to the previous page.
        /// </summary>
        public async Task SaveItemAndReturnAsync()
        {
            await SaveItemAsync();
            navigationService.GoBack();
        }
    }
}