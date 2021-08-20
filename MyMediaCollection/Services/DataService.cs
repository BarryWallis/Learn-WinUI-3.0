using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyMediaCollection.Enums;
using MyMediaCollection.Interfaces;
using MyMediaCollection.Models;

namespace MyMediaCollection.Services
{
    public class DataService : IDataService
    {
        private IList<MediaItem> _mediaItems;
        private IList<ItemType> _itemTypes;
        private IList<Medium> _mediums;
        private IList<LocationType> _locationTypes;

        public DataService()
        {
            PopulateItemTypes();
            PopulateLocationTypes();
            PopulateMediums();
            PopulateMediaItems();
        }

        /// <inheritdoc/>
        public int SelectedItemId { get; set; }

        /// <summary>
        /// Add an item to the collection.
        /// </summary>
        /// <param name="mediaItem">The item to add.</param>
        /// <returns>The Id of the item added.</returns>
        public int AddItem(MediaItem mediaItem)
        {
            mediaItem.Id = _mediaItems.Max(mi => mi.Id) + 1;
            _mediaItems.Add(mediaItem);
            return mediaItem.Id;
        }

        /// <summary>
        /// Get a media item with the given Id.
        /// </summary>
        /// <param name="id">The Id of the media item to get.</param>
        /// <returns>The media item with the given Id or the default media item if none exsts.</returns>
        public MediaItem GetItem(int id)
        {
            MediaItem mediaItem = _mediaItems.FirstOrDefault(i => i.Id == id);
            if (mediaItem == default)
            {
                mediaItem.Name = string.Empty;
                mediaItem.MediumInfo = GetMediums()[0];
                mediaItem.Location = GetLocationTypes()[0];
                mediaItem.MediaType = GetItemTypes()[0];
            }

            return mediaItem;
        }

        /// <summary>
        /// Get a list of all the media items.
        /// </summary>
        /// <returns>The list of all the media items.</returns>
        public IList<MediaItem> GetItems() => _mediaItems;

        /// <summary>
        /// Get a list of all the item types.
        /// </summary>
        /// <returns>The list of all the item types.</returns>
        public IList<ItemType> GetItemTypes() => _itemTypes;

        /// <summary>
        /// Get a list of all the location types.
        /// </summary>
        /// <returns>The list of all the location types.</returns>
        public IList<LocationType> GetLocationTypes() => _locationTypes;

        /// <summary>
        /// Get the medium with the given name. 
        /// </summary>
        /// <param name="name">The name of the medium to get.</param>
        /// <returns>The medium with the given name or the default medium if none exists.</returns>
        public Medium GetMedium(string name) => _mediums.FirstOrDefault(m => m.Name == name);

        /// <summary>
        /// Get the list of mediums.
        /// </summary>
        /// <returns>The list of mediums.</returns>
        public IList<Medium> GetMediums() => _mediums;

        /// <summary>
        /// Get the list of mediums of the requested item type..
        /// </summary>
        /// <param name="itemType">The item type to filter on.</param>
        /// <returns>The list of mediums with the requested item type.</returns>
        public IList<Medium> GetMediums(ItemType? itemType) => _mediums.Where(m => m.MediaType == itemType).ToList();

        /// <summary>
        /// Update the given media item.
        /// </summary>
        /// <param name="mediaItem">The media item to update.</param>
        public void UpdateItem(MediaItem mediaItem)
        {
            int index = 0;
            foreach (MediaItem item in _mediaItems)
            {
                if (item.Id == mediaItem.Id)
                {
                    break;
                }

                index += 1;
            }

            if (index >= _mediaItems.Count)
            {
                throw new Exception("Unable to update item. Item not found in collection.");
            }

            _mediaItems[index] = mediaItem;
        }

        /// <summary>
        /// TODO: Remove temporary function to supply data during initial development
        /// </summary>
        public void PopulateMediaItems()
        {
            MediaItem cd = new()
            {
                Id = 1,
                Name = "Classical Favorites",
                MediaType = ItemType.Music,
                MediumInfo = _mediums.FirstOrDefault(m => m.Name == "CD"),
                Location = LocationType.InCollection,
            };

            MediaItem book = new()
            {
                Id = 2,
                Name = "Classic Fairy Tales",
                MediaType = ItemType.Book,
                MediumInfo = _mediums.FirstOrDefault(m => m.Name == "Hardcover"),
                Location = LocationType.InCollection,
            };

            MediaItem bluRay = new()
            {
                Id = 3,
                Name = "The Mummy",
                MediaType = ItemType.Video,
                MediumInfo = _mediums.FirstOrDefault(m => m.Name == "Blu-Ray"),
                Location = LocationType.InCollection,
            };

            _mediaItems = new List<MediaItem>
            {
                cd, book, bluRay,
            };
        }

        /// <summary>
        /// Populate the mediums list.
        /// </summary>
        private void PopulateMediums()
        {
            Medium cd = new()
            { Id = 1, MediaType = ItemType.Music, Name = "CD" };

            Medium vinyl = new()
            { Id = 2, MediaType = ItemType.Music, Name = "Vinyl" };

            Medium hardcover = new()
            { Id = 3, MediaType = ItemType.Book, Name = "Hardcover" };

            Medium paperback = new()
            { Id = 4, MediaType = ItemType.Book, Name = "Paperback" };

            Medium dvd = new()
            { Id = 5, MediaType = ItemType.Video, Name = "DVD" };

            Medium bluRay = new()
            { Id = 6, MediaType = ItemType.Video, Name = "Blu-Ray" };

            _mediums = new List<Medium>
            {
                cd,
                vinyl,
                hardcover,
                paperback,
                dvd,
                bluRay
            };
        }

        /// <summary>
        /// Populate the item types list.
        /// </summary>
        private void PopulateItemTypes()
            => _itemTypes = Enum.GetNames(typeof(ItemType)).Select(s => Enum.Parse<ItemType>(s, false)).ToList();

        /// <summary>
        /// Populate the location types list.
        /// </summary>
        private void PopulateLocationTypes()
            => _locationTypes = Enum.GetNames(typeof(LocationType)).Select(s => Enum.Parse<LocationType>(s, false)).ToList();
        public Task<IList<MediaItem>> GetItemsAsync() => throw new NotImplementedException();
        public Task<MediaItem> GetItemAsync(int id) => throw new NotImplementedException();
        public Task<int> AddItemAsync(MediaItem mediaItem) => throw new NotImplementedException();
        public Task UpdateItemAsync(MediaItem mediaItem) => throw new NotImplementedException();
        public Task InitializeDataAsync() => throw new NotImplementedException();
        public Task DeleteItemAsync(MediaItem mediaItem) => throw new NotImplementedException();
    }
}
