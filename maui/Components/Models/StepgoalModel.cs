// BlogModel.cs
namespace maui.Components.Models
{
    public class StepgoalModel
    {
        public int Id { get; set; }
        public int Goal { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        //public int Counter { get; set; } // New property to hold the counter
    }
}