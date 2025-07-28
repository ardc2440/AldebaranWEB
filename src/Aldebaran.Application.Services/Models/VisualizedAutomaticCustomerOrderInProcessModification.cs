namespace Aldebaran.Application.Services.Models
{
    public class VisualizedAutomaticCustomerOrderInProcessModification
    {
        public int Id { get; set; }
        public required string ActionType { get; set; }
        public int Employee_Id { get; set; }             
        public DateTime Visualized_Date { get; set; } = DateTime.Now;
    }
}
