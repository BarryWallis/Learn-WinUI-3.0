
using MyMediaCollection.Enums;

#nullable enable
namespace MyMediaCollection.Models
{
    /// <summary>
    /// The medium an item in the collection belongs to (e.g., hardcover, paperback, DVD, etc.) 
    /// </summary>
    public class Medium
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ItemType MediaType { get; set; }
    }
}
