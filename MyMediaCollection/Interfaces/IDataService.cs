using System.Collections.Generic;

using MyMediaCollection.Enums;
using MyMediaCollection.Models;

namespace MyMediaCollection.Interfaces
{
    public interface IDataService
    {
        /// <summary>
        /// The currently selected item's Id.
        /// </summary>
        int SelectedItemId { get; set; }

        /// <summary>
        /// Get all evailable media items.
        /// </summary>
        /// <returns>The list of items.</returns>
        IList<MediaItem> GetItems();

        /// <summary>
        /// Get the media item with the given Id.
        /// </summary>
        /// <param name="id">The Id to use.</param>
        /// <returns></returns>
        MediaItem GetItem(int id);

        /// <summary>
        /// Add a new media item to the collection.
        /// </summary>
        /// <param name="mediaItem">The media item to ad to the collection.</param>
        /// <returns></returns>
        int AddItem(MediaItem mediaItem);

        /// <summary>
        /// Update the media item in the collection.
        /// </summary>
        /// <param name="mediaItem">The media item to updaate.</param>
        void UpdateItem(MediaItem mediaItem);

        /// <summary>
        /// Get the list of item types.
        /// </summary>
        /// <returns>The list of item types."</returns>
        IList<ItemType> GetItemTypes();

        /// <summary>
        /// Gets the medium with the given name.
        /// </summary>
        /// <param name="name">The name of the medium to get.</param>
        /// <returns></returns>
        Medium GetMedium(string name);

        /// <summary>
        /// Get a list of all the available mediums.
        /// </summary>
        /// <returns>The list of available mediums.</returns>
        IList<Medium> GetMediums();

        /// <summary>
        /// Get a list of the available mediums for the given item type.
        /// </summary>
        /// <param name="itemType">The item type to get the mediums for.</param>
        /// <returns></returns>
        IList<Medium> GetMediums(ItemType itemType);

        /// <summary>
        /// Get a list of all the available media locations.
        /// </summary>
        /// <returns></returns>
        IList<LocationType> GetLocationTypes();
    }
}
