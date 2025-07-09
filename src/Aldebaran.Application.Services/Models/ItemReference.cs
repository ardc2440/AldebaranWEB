namespace Aldebaran.Application.Services.Models
{
    public class ItemReference
    {
        public int ReferenceId { get; set; }
        public int ItemId { get; set; }
        public string ReferenceCode { get; set; }
        public string ProviderReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public string ProviderReferenceName { get; set; }
        public string Notes { get; set; }
        public int InventoryQuantity { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int WorkInProcessQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsSoldOut { get; set; }
        public int AlarmMinimumQuantity { get; set; }
        public int PurchaseOrderVariation { get; set; }
        public int MinimumQuantityPercent { get; set; }
        public int MinimumLocalWarehouseQuantity { get; set; }

        public bool HavePurchaseOrderDetail { get; set; }
        // Reverse navigation
        public ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }
        public ICollection<ProviderReference> ProviderReferences { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public ICollection<ReferencesWarehouse> ReferencesWarehouses { get; set; }
        public ICollection<WarehouseTransferDetail> WarehouseTransferDetails { get; set; }
        public Item Item { get; set; }

        /// <summary>
        /// Propiedad de solo negocio. No mapear con la base de datos ni usar en AutoMapper.
        /// True si AlarmMinimumQuantity > 0, false si es igual a 0.
        /// Al setear en false, AlarmMinimumQuantity se pone en 0.
        /// </summary>
        private bool _alarmMinimumQuantityActive;
        public bool AlarmMinimumQuantityActive
        {
            get => _alarmMinimumQuantityActive;
            set
            {
                _alarmMinimumQuantityActive = value;
                if (!value)
                {
                    AlarmMinimumQuantity = 0;
                }
            }
        }

        /// <summary>
        /// Sincroniza la propiedad de negocio AlarmMinimumQuantityActive según el valor de AlarmMinimumQuantity.
        /// Llamar después de mapear o deserializar desde la base de datos.
        /// </summary>
        public void SyncAlarmMinimumQuantityActive()
        {
            _alarmMinimumQuantityActive = AlarmMinimumQuantity > 0;
        }

        public ItemReference()
        {
            InventoryQuantity = 0;
            OrderedQuantity = 0;
            ReservedQuantity = 0;
            WorkInProcessQuantity = 0;
            IsActive = true;
            IsSoldOut = false;
            AlarmMinimumQuantity = 0;
            PurchaseOrderVariation = 0; 
            MinimumQuantityPercent = 0;
            MinimumLocalWarehouseQuantity = 0;
            AdjustmentDetails = new List<AdjustmentDetail>();
            CustomerOrderDetails = new List<CustomerOrderDetail>();
            CustomerReservationDetails = new List<CustomerReservationDetail>();
            ProviderReferences = new List<ProviderReference>();
            PurchaseOrderDetails = new List<PurchaseOrderDetail>();
            ReferencesWarehouses = new List<ReferencesWarehouse>();
            // Inicializa la propiedad de negocio según el valor de AlarmMinimumQuantity
            _alarmMinimumQuantityActive = AlarmMinimumQuantity > 0;
        }
    }
}
