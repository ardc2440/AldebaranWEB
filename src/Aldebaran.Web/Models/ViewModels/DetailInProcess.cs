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

        /// <summary>
        /// Cantidad original del traslado desde la base de datos.
        /// Esta propiedad preserva el valor original y no debe modificarse una vez establecida,
        /// excepto cuando se vuelve a cargar desde la base de datos.
        /// Se usa para validaciones de stock que requieren conocer la cantidad original.
        /// </summary>
        public int ORIGINAL_THIS_QUANTITY { get; set; }

        public string BRAND { get; set; }

        public short WAREHOUSE_ID { get; set; }

        public ItemReference ItemReference { get; set; }

        public int CustomerOrderInProcessDetailId { get; set; }
    }
}
