namespace Aldebaran.Application.Services.Models
{
    public class CustomerReservationDetail
    {
        public int CustomerReservationDetailId { get; set; }
        public int CustomerReservationId { get; set; }
        public int ReferenceId { get; set; }
        public int ReservedQuantity { get; set; }
        public string Brand { get; set; }
        public bool SendToCustomerOrder { get; set; }
        public CustomerReservation CustomerReservation { get; set; }
        public ItemReference ItemReference { get; set; }
        public CustomerReservationDetail()
        {
            SendToCustomerOrder = false;
        }
    }
}
