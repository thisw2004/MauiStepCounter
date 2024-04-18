// BlogModel.cs

using System.Runtime.InteropServices.JavaScript;

namespace maui.Components.Models
{
    public class StepgoalModel
    {
        public int Goal{ get; set; }
        public DateOnly Date { get; set; }
        public string? Progress { get; set; }
        public string? Achieved { get; set; }
        //public int Counter { get; set; } // New property to hold the counter
    }
}