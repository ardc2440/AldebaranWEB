namespace Aldebaran.Application.Services.Models
{
    public class NotificationBase
    {
        public string NotificationId { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotifiedMailList { get; set; }
        public NotificationStatus NotificationState { get; set; }
        public string? NotificationSendingErrorMessage { get; set; }

        public NotificationBase()
        {
            NotifiedMailList = "";
            NotificationId = Guid.NewGuid().ToString();
            NotificationDate = DateTime.Now;
        }
    }
}
