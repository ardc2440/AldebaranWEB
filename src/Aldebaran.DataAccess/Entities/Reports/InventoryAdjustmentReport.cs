﻿namespace Aldebaran.DataAccess.Entities.Reports
{
    public class InventoryAdjustmentReport
    {
        public int AdjustmentId { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string AdjustmentType { get; set; }
        public string AdjustmentReason { get; set; }
        public string Employee { get; set; }
        public string? Notes { get; set; }

        public short WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public short LineId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }

        public int ItemId { get; set; }
        public string InternalReference { get; set; }
        public string ItemName { get; set; }

        public int ReferenceId { get; set; }
        public string ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public int AvailableAmount { get; set; }
    }
}
