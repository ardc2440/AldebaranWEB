using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerReservationDetail : ITrackeable
    {
        public int CustomerReservationDetailId { get; set; }
        public int CustomerReservationId { get; set; }
        public int ReferenceId { get; set; }
        public int ReservedQuantity { get; set; }
        public string Brand { get; set; } = null!;
        public bool SendToCustomerOrder { get; set; }
        public CustomerReservation CustomerReservation { get; set; } = null!;
        public ItemReference ItemReference { get; set; } = null!;
        public CustomerReservationDetail()
        {
            SendToCustomerOrder = false;
        }
    }
}
