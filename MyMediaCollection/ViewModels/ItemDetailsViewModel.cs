using System;
using System.Linq;

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
        public string ItemName
        {
            get => _itemName;
            set => IsDirty = SetProperty(ref _itemName, value);
        }

        private string _selectedMedium;
        public string SelectedMedium
        {
            get => _selectedMedium;
            set => IsDirty = SetProperty(ref _selectedMedium, value);
        }

        private string _selectedLocation;
        public string SelectedLocation { get => _selectedLocation; set => IsDirty = SetProperty(ref _selectedLocation, value); }

        private ObservableCollection<string> _mediums = new();
        private ObservableCollection<string> _itemTypes = new();
        private ObservableCollection<string> _locationTypes = new();

        private bool _isDirty;

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
                    }

                    (SaveCommand as RelayCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    (SaveCommand as RelayCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<string> Mediums { get => _mediums; set => SetProperty(ref _mediums, value); }

        public ObservableCollection<string> ItemTypes { get => _itemTypes; set => SetProperty(ref _itemTypes, value); }

        public ObservableCollection<string> LocationTypes { get => _locationTypes; set => SetProperty(ref _locationTypes, value); }

        public ItemDetailsViewModel(INavigationService navigationService, IDataService dataService)
        {
            this.navigationService = navigationService;
            this.dataService = dataService;
            SaveCommand = new RelayCommand(SaveItem, CanSaveItem);
            CancelCommand = new RelayCommand(Cancel);
            PopulateLists();
            PopulateExistingItem(dataService);
            IsDirty = false;
        }

        /// <summary>
        /// Populate the properties with the exising item data.
        /// </summary>
        /// <param name="dataService">The data service to recieve the data from.</param>
        private void PopulateExistingItem(IDataService dataService)
        {
            if (_selectedItemId > 0)
            {
                MediaItem item = dataService.GetItem(_selectedItemId);
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
        private void SaveItem()
        {
            MediaItem mediaItem;
            if (_itemId > 0)
            {
                mediaItem = dataService.GetItem(_itemId);
                mediaItem.Name = ItemName;
                mediaItem.Location = (LocationType)Enum.Parse(typeof(LocationType), SelectedLocation);
                mediaItem.MediaType = (ItemType)Enum.Parse(typeof(ItemType), SelectedItemType);
                mediaItem.MediumInfo = dataService.GetMedium(SelectedMedium);
                dataService.UpdateItem(mediaItem);
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
                _ = dataService.AddItem(mediaItem);
            }

            navigationService.GoBack();
        }

        /// <summary>
        /// Does the current item need to be saved and can it be saved? 
        /// </summary>
        /// <returns><see langword="true"/> if it needs to be saved and can be saved, otherwise <see langword="false"/>.</returns>
        private bool CanSaveItem() => IsDirty
                                      && !string.IsNullOrWhiteSpace(ItemName)
                                      && !string.IsNullOrWhiteSpace(SelectedItemType)
                                      && !string.IsNullOrWhiteSpace(SelectedMedium)
                                      && !string.IsNullOrWhiteSpace(SelectedLocation);

        /// <summary>
        /// Iniitialize the data for the item details page.
        /// </summary>
        /// <param name="selectItemId">The seleted item when the page is initialized.</param>
        public void InitializeItemDetailData(int selectItemId)
        {
            _selectedItemId = selectItemId;
            PopulateLists();
            PopulateExistingItem(dataService);
            IsDirty = false;
        }
    }
}