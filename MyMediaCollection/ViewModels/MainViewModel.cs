using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.UI.Xaml.Input;

//using Microsoft.UI.Xaml.Data;

using MyMediaCollection.Enums;
using MyMediaCollection.Models;

namespace MyMediaCollection.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private bool _isLoaded = false;
        private AppUIBasics.ObservableCollection<MediaItem> _allItems;

        private MediaItem _selectedMediaItem;
        public MediaItem SelectedMediaItem
        {
            get => _selectedMediaItem;
            set
            {
                _ = SetProperty(ref _selectedMediaItem, value);
                (DeleteCommand as RelayCommand).RaiseCanExecuteChanged();
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

        private AppUIBasics.ObservableCollection<MediaItem> _items;
        public AppUIBasics.ObservableCollection<MediaItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private IList<string> _mediums;
        private int _additionalItemCount = 0;

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
            // TODO: This is temporary until we use a real data source.
            const int StartingItemCount = 3;
            MediaItem newItem = new()
            {
                Id = StartingItemCount + ++_additionalItemCount,
                Location = LocationType.InCollection,
                MediaType = ItemType.Music,
                MediumInfo = new()
                {
                    Id = 1,
                    MediaType = ItemType.Music,
                    Name = "CD",
                },
                Name = $"CD {_additionalItemCount}",
            };

            _allItems.Add(newItem);
            Items.Add(newItem);
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

        public MainViewModel()
        {
            PopulateData();
            DeleteCommand = new RelayCommand(DeleteItem, CanDeleteItem);
            AddEditCommand = new RelayCommand(AddOrEditItem);
        }

        /// <summary>
        /// TODO: Remove temporary function to supply data during initial development
        /// </summary>
        public void PopulateData()
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                MediaItem cd = new()
                {
                    Id = 1,
                    Name = "Classical Favorites",
                    MediaType = ItemType.Music,
                    MediumInfo = new Medium
                    {
                        Id = 1,
                        MediaType = ItemType.Music,
                        Name = "CD",
                    }
                };

                MediaItem book = new()
                {
                    Id = 2,
                    Name = "Classic Fairy Tales",
                    MediaType = ItemType.Book,
                    MediumInfo = new Medium
                    {
                        Id = 2,
                        MediaType = ItemType.Book,
                        Name = "Book",
                    }
                };

                MediaItem bluRay = new()
                {
                    Id = 3,
                    Name = "The Mummy",
                    MediaType = ItemType.Video,
                    MediumInfo = new Medium
                    {
                        Id = 3,
                        MediaType = ItemType.Video,
                        Name = "Blu-Ray",
                    }
                };

                Items = new AppUIBasics.ObservableCollection<MediaItem>
                {
                    cd, book, bluRay,
                };

                _allItems = new AppUIBasics.ObservableCollection<MediaItem>();
                foreach (MediaItem item in Items)
                {
                    _allItems.Add(item);
                }

                Mediums = new List<string>
                {
                    "All",
                    nameof(ItemType.Book),
                    nameof(ItemType.Music),
                    nameof(ItemType.Video),
                };

                SelectedMedium = Mediums[0];
            }
        }
    }
}
