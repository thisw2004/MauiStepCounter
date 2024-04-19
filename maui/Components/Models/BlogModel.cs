// BlogModel.cs
namespace maui.Components.Models
{
    public class BlogModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Author { get; set; }
        public DateTime CreatedAt { get; set; }
        //public int Counter { get; set; } // New property to hold the counter
    }
}