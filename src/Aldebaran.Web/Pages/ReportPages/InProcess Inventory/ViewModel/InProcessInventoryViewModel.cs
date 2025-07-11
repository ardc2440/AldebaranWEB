﻿namespace Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.ViewModel
{
    public class InProcessInventoryViewModel
    {
        public List<Line> Lines { get; set; }

        public class Line
        {
            public string LineCode { get; set; }
            public string LineName { get; set; }
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            public string InternalReference { get; set; }
            public string ItemName { get; set; }
            public List<Reference> References { get; set; }
        }
        public class Reference
        {
            public string ReferenceName { get; set; }
            public int InProcessAmount { get; set; }
            public List<Warehouse> Warehouses { get; set; }
        }

        public class Warehouse
        {
            public short WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public int Amount { get; set; }
        }
    }

}
