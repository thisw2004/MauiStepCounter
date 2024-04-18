// BlogModel.cs
namespace maui.Components.Models
{
    public class StepgoalModel
    {
        public int Id { get; set; }
        public int Goal { get; set; }
        public DateTime Date { get; set; }
        public int Progress { get; set; }
        public Boolean Achieved { get; set; }
        //public int Counter { get; set; } // New property to hold the counter
    }
}