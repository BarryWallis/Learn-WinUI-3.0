using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using MyMediaCollection.Enums;
using MyMediaCollection.Models;

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
        private bool _isLoaded = false;
        private IList<MediaItem> _items;
        private IList<MediaItem> _allItems;
        private IList<string> _mediums;

        public MainPage()
        {
            InitializeComponent();

            ItemList.Loaded += ItemList_Loaded;
            ItemFilter.Loaded += ItemFilter_Loaded;

            Loaded += MainPage_Loaded;
        }
        /// <summary>
        /// Set up events that need to be done after all fields are initially loaded.
        /// </summary>
        /// <param name="sender">The control sending this event.</param>
        /// <param name="e">The arguments for this event.</param>
        private void MainPage_Loaded(object sender, RoutedEventArgs e) => ItemFilter.SelectionChanged += ItemFilter_SelectionChanged;

        /// <summary>
        /// The user made a new selection in the item filter.
        /// </summary>
        /// <param name="sender">The control sending this event.</param>
        /// <param name="e">The arguments for this event.</param>
        private void ItemFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(ItemFilter.SelectedValue.ToString()));
            string selectedValue = ItemFilter.SelectedValue.ToString();
            ItemList.ItemsSource = _allItems
                .Where(mi => selectedValue == "All" || selectedValue == mi.MediaType.ToString())
                .ToList();
        }

        /// <summary>
        /// Populate the item filter.
        /// </summary>
        /// <param name="sender">The control sending this event.</param>
        /// <param name="e">The arguments for this event.</param>
        private void ItemFilter_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.Assert(sender is ComboBox);
            PopulateData();
            ItemFilter.ItemsSource = _mediums;
            ItemFilter.SelectedIndex = 0;
        }

        /// <summary>
        /// Populate the item list.
        /// </summary>
        /// <param name="sender">The control sending this event.</param>
        /// <param name="e">The arguments for this event.</param>
        private void ItemList_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.Assert(sender is ListView);
            PopulateData();
            ItemList.ItemsSource = _items;
        }

        /// <summary>
        /// TODO: Add a new item to the collection.
        /// </summary>
        /// <param name="sender">The control sending this event.</param>
        /// <param name="e">The arguments for this event.</param>
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog messageDialog = new("Adding items to the collection is not yet available.", "My Media Collection");
            _ = await messageDialog.ShowAsync();
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

                _items = new List<MediaItem>
                {
                    cd, book, bluRay,
                };

                _allItems = new List<MediaItem>(_items);

                _mediums = new List<string>
                {
                    "All",
                    nameof(ItemType.Book),
                    nameof(ItemType.Music),
                    nameof(ItemType.Video),
                };
            }
        }
    }
}
