using Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Models.ViewModels
{
    public class DetailInProcess
    {
        public int CUSTOMER_ORDER_DETAIL_ID { get; set; }

        public int REFERENCE_ID { get; set; }

        public string REFERENCE_DESCRIPTION { get; set; }

        public int PENDING_QUANTITY { get; set; }

        public int PROCESSED_QUANTITY { get; set; }

        public int DELIVERED_QUANTITY { get; set; }

        public int THIS_QUANTITY { get; set; }

        public string BRAND { get; set; }

        public short WAREHOUSE_ID { get; set; }

        public ItemReference ItemReference { get; set; }

        public int CustomerOrderInProcessDetailId { get; set; }
    }
}
