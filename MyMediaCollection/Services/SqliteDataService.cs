using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MyMediaCollection.Enums;
using MyMediaCollection.Interfaces;
using MyMediaCollection.Models;

using Windows.Storage;

namespace MyMediaCollection.Services
{
    public class SqliteDataService : IDataService
    {
        private const string DbName = "mediaCollectionData.db";
        private IList<Medium> _mediums;
        private List<LocationType> _locationTypes;
        private List<ItemType> _itemTypes;

        public int SelectedItemId { get; set; }

        /// <summary>
        /// Get a connection to the database.
        /// </summary>
        /// <returns>The connection to the database.</returns>
        private async Task<SqliteConnection> GetOpenConnectionAsync()
        {
            _ = await ApplicationData.Current.LocalFolder.CreateFileAsync(DbName, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DbName);
            SqliteConnection sqliteConnection = new($"Filename={dbPath}");
            sqliteConnection.Open();
            return sqliteConnection;
        }

        /// <summary>
        /// Create the Medium table if it does not exist.
        /// </summary>
        /// <param name="daabase">The database to create the table in.</param>
        /// <returns>Nothing.</returns>
        private async Task CreateMediumTableAsync(SqliteConnection daabase)
        {
            string tableCommand = @"CREATE TABLE IF NOT EXISTS Mediums (Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                                        Name NVARCHAR(30) NOT NULL, 
                                                                        MediumType INTEGER NOT NULL)";
            SqliteCommand createTable = new(tableCommand, daabase);
            _ = await createTable.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Create the Media Item table if it does not exist.
        /// </summary>
        /// <param name="database">The database to create the Media Item table in.</param>
        /// <returns>Nothing.</returns>
        private async Task CreateMediaItemTableAsync(SqliteConnection database)
        {
            string tableCommand = @"CREATE TABLE IF NOT EXISTS MediaItems (Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                                           Name NVARCHAR(1000) NOT NULL,
                                                                           ItemType INTEGER NOT NULL,
                                                                           MediumId INTEGER NOT NULL,
                                                                           LocationType INTEGER,
                                                                           CONSTRAINT fk_mediums FOREIGN KEY(MediumId) REFERENCES Mediums(Id))";
            SqliteCommand createTable = new(tableCommand, database);
            _ = await createTable.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Insert a Medium row into the database.
        /// </summary>
        /// <param name="database">The database to insert the row in.</param>
        /// <param name="medium">The row to insert.</param>
        /// <returns>Nothing.</returns>
        private async Task InsertMediumAsync(SqliteConnection database, Medium medium)
        {
            IEnumerable<long> newIds = await database.QueryAsync<long>($@"INSERT INTO Mediums ({nameof(medium.Name)}, MediumType) 
                                                                                     VALUES (@{nameof(medium.Name)}, @{nameof(medium.MediaType)}); 
                                                                              SELECT last_insert_rowid()", medium);
            medium.Id = (int)newIds.First();
        }

        /// <summary>
        /// Get all the Medium rows from the database
        /// </summary>
        /// <param name="database">The database to get the rows from.</param>
        /// <returns>The list of rows.</returns>
        private async Task<IList<Medium>> GetAllMediumsAsync(SqliteConnection database)
        {
            IEnumerable<Medium> mediums = await database.QueryAsync<Medium>(@"SELECT Id, Name, MediumType AS MediaType FROM Mediums");
            return mediums.ToList();
        }

        /// <summary>
        /// Get all the Media Items from the database.
        /// </summary>
        /// <param name="database">The databast to get th Media Items from.</param>
        /// <returns>The list of Media Items.</returns>
        private async Task<List<MediaItem>> GetAllMediaItemsAsync(SqliteConnection database)
        {
            IEnumerable<MediaItem> mediaItems = await database.QueryAsync<MediaItem, Medium, MediaItem>(
                @"SELECT [MediaItems].[Id],
                         [MediaItems].[Name],
                         [MediaItems].[ItemType] AS MediaType,
                         [MediaItems].[LocationType] AS Location,
                         [Mediums].[Id],
                         [Mediums].[Name],
                         [Mediums].[MediumType] AS MediaType
                FROM [MediaItems]
                JOIN [Mediums]
                ON [Mediums].[Id] = [MediaItems].[MediumId]",
                (item, medium) =>
                {
                    item.MediumInfo = medium;
                    return item;
                });

            return mediaItems.ToList();
        }

        /// <summary>
        /// Insert a Media Item into the database and return its Id.
        /// </summary>
        /// <param name="database">The database to insert the item into.</param>
        /// <param name="mediaItem">The Media Item to insert.</param>
        /// <returns>The Id of the inserted Media Item.</returns>
        private async Task<int> InsertMediaItemAsync(SqliteConnection database, MediaItem mediaItem)
        {
            IEnumerable<long> newIds = await database.QueryAsync<long>(
                @"INSERT INTO MediaItems (Name, ItemType, MediumId, LocationType)
                              VALUES (@Name, @MediaType, @MediumId, @Location);
                    SELECT last_insert_rowid()",
                mediaItem);
            return (int)newIds.First();
        }

        /// <summary>
        /// Update the Media Item in the database.
        /// </summary>
        /// <param name="database">The database contining the Media Item to update.</param>
        /// <param name="mediaItem">THe Media Item to update.</param>
        /// <returns>Nothing,</returns>
        private async Task UpdateMediaItemAsync(SqliteConnection database, MediaItem mediaItem) => _ = await database.QueryAsync(
                @"UPDATE MediaItems
                        SET Name = @Name,
                            ItemType = @MediaType,
                            MediumId = @MediumId,
                            LocationType = @Location
                        WHERE Id = @Id",
                    mediaItem);

        /// <summary>
        /// Delete a Media Item from the database.
        /// </summary>
        /// <param name="database">The database to delete the Media Item from.</param>
        /// <param name="id">The Id of the Media Item to delete.</param>
        /// <returns>Nothing.</returns>
        private async Task DeleteMediaItemAsync(SqliteConnection database, int id) => await database.DeleteAsync(new MediaItem { Id = id });

        /// <summary>
        /// Populate the mediums table if the table is empty.
        /// </summary>
        private async Task PopulateMediumsAsync(SqliteConnection database)
        {
            _mediums = await GetAllMediumsAsync(database);
            if (_mediums.Count == 0)
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

                IList<Medium> mediums = new List<Medium>
                {
                    cd,
                    vinyl,
                    hardcover,
                    paperback,
                    dvd,
                    bluRay
                };

                foreach (Medium medium in mediums)
                {
                    await InsertMediumAsync(database, medium);
                }
                _mediums = await GetAllMediumsAsync(database);
            }
        }

        /// <inheritdoc/>
        public async Task InitializeDataAsync()
        {
            using SqliteConnection database = await GetOpenConnectionAsync();
            await CreateMediumTableAsync(database);
            await CreateMediaItemTableAsync(database);
            SelectedItemId = -1;
            PopulateItemTypes();
            await PopulateMediumsAsync(database);
            PopulateLocationTypes();
        }

        /// <summary>
        /// Populate the possible location types into a list.
        /// </summary>
        private void PopulateLocationTypes() => _locationTypes = new List<LocationType>
            {
                LocationType.InCollection,
                LocationType.Loaned
            };

        /// <summary>
        /// Populate the possible item types into a list.
        /// </summary>
        private void PopulateItemTypes() => _itemTypes = new List<ItemType>
            {
                ItemType.Book,
                ItemType.Music,
                ItemType.Video
            };

        /// <inheritdoc/>
        public async Task<IList<MediaItem>> GetItemsAsync()
        {
            using SqliteConnection database = await GetOpenConnectionAsync();
            return await GetAllMediaItemsAsync(database);
        }

        /// <inheritdoc/>
        public async Task<MediaItem> GetItemAsync(int id)
        {
            IList<MediaItem> mediaItems;
            using SqliteConnection database = await GetOpenConnectionAsync();
            mediaItems = await GetAllMediaItemsAsync(database);
            return mediaItems.FirstOrDefault(mi => mi.Id == id);
        }

        /// <inheritdoc/>
        public async Task<int> AddItemAsync(MediaItem mediaItem)
        {
            using SqliteConnection database = await GetOpenConnectionAsync();
            return await InsertMediaItemAsync(database, mediaItem);
        }

        /// <inheritdoc/>
        public async Task UpdateItemAsync(MediaItem mediaItem)
        {
            using SqliteConnection database = await GetOpenConnectionAsync();
            await UpdateMediaItemAsync(database, mediaItem);
        }

        ///<inheritdoc/>
        public async Task DeleteItemAsync(MediaItem mediaItem)
        {
            using SqliteConnection database = await GetOpenConnectionAsync();
            await DeleteMediaItemAsync(database, mediaItem.Id);
        }

        /// <summary>
        /// Get a list of all item types.
        /// </summary>
        /// <returns>The list of item types.</returns>
        public IList<ItemType> GetItemTypes() => _itemTypes;

        /// <summary>
        /// Get the medium with the given name.
        /// </summary>
        /// <param name="name">The name of the medium to get.</param>
        /// <returns></returns>
        public Medium GetMedium(string name) => _mediums.FirstOrDefault(m => m.Name == name);

        /// <summary>
        /// Get a lost of all the mediums.
        /// </summary>
        /// <returns>The list of mediums.</returns>
        public IList<Medium> GetMediums() => _mediums;

        /// <summary>
        /// Get all the mediums with the given item type.
        /// </summary>
        /// <param name="itemType">The item type to search for.</param>
        /// <returns>The list of mediums.</returns>
        public IList<Medium> GetMediums(ItemType? itemType) => _mediums.Where(m => m.MediaType == itemType).ToList();

        /// <summary>
        /// Get a list of all the location types.
        /// </summary>
        /// <returns>The list of location types.</returns>
        public IList<LocationType> GetLocationTypes() => _locationTypes;
    }
}
