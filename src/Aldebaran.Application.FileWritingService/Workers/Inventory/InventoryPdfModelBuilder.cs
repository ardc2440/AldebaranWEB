using Aldebaran.Application.FileWritingService.Workers.Inventory.Models;
using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.FileWritingService.Workers.Inventory
{
    /// <summary>
    /// Construye el modelo jerárquico del PDF (Línea → Artículo → Referencia → Orden → Actividad).
    /// Cada nivel se indexa una sola vez con ToLookup en lugar de recorrer todas las filas por cada elemento.
    /// Conserva los mismos criterios de agrupación, filtros y orden del cálculo anterior.
    /// </summary>
    internal static class InventoryPdfModelBuilder
    {
        public static InventoryPdfViewModel Build(IReadOnlyCollection<InventoryReport> data)
        {
            var rowsByLine = data.ToLookup(r => r.LineId);
            var rowsByItem = data.ToLookup(r => r.ItemId);
            var rowsByReference = data.ToLookup(r => r.ReferenceId);

            return new InventoryPdfViewModel
            {
                Lines = data.DistinctBy(r => r.LineId)
                            .OrderBy(r => r.LineName)
                            .Select(line => BuildLine(line, rowsByLine[line.LineId], rowsByItem, rowsByReference))
                            .ToList()
            };
        }

        private static InventoryPdfViewModel.Line BuildLine(InventoryReport line, IEnumerable<InventoryReport> lineRows, ILookup<int, InventoryReport> rowsByItem, ILookup<int, InventoryReport> rowsByReference)
        {
            return new InventoryPdfViewModel.Line
            {
                LineName = line.LineName,
                Items = lineRows.DistinctBy(r => r.ItemId)
                                .OrderBy(r => r.ItemName)
                                .Select(item => BuildItem(item, rowsByItem[item.ItemId], rowsByReference))
                                .ToList()
            };
        }

        private static InventoryPdfViewModel.Item BuildItem(InventoryReport item, IEnumerable<InventoryReport> itemRows, ILookup<int, InventoryReport> rowsByReference)
        {
            return new InventoryPdfViewModel.Item
            {
                InternalReference = item.InternalReference,
                ItemName = item.ItemName,
                References = itemRows.DistinctBy(r => r.ReferenceId)
                                     .OrderBy(r => r.ReferenceName)
                                     .Select(reference => BuildReference(reference, rowsByReference[reference.ReferenceId]))
                                     .ToList()
            };
        }

        private static InventoryPdfViewModel.Reference BuildReference(InventoryReport reference, IEnumerable<InventoryReport> referenceRows)
        {
            var rowsByOrder = referenceRows.ToLookup(r => r.PurchaseOrderId);

            return new InventoryPdfViewModel.Reference
            {
                ReferenceName = reference.ReferenceName,
                AvailableAmount = reference.AvailableAmount,
                LocalWarehouse = reference.LocalWarehouse,
                FreeZone = reference.FreeZone,
                PurchaseOrders = referenceRows.Where(r => r.PurchaseOrderId > 0)
                                              .DistinctBy(r => r.PurchaseOrderId)
                                              .OrderBy(r => r.OrderDate)
                                              .Select(order => BuildPurchaseOrder(order, rowsByOrder[order.PurchaseOrderId]))
                                              .ToList()
            };
        }

        private static InventoryPdfViewModel.PurchaseOrder BuildPurchaseOrder(InventoryReport order, IEnumerable<InventoryReport> orderRows)
        {
            return new InventoryPdfViewModel.PurchaseOrder
            {
                Date = order.OrderDate,
                Total = order.Total ?? 0,
                Warehouse = order.Warehouse,
                Activities = orderRows.Where(r => r.Description != null && r.Description.Trim().Length > 0)
                                      .Select(r => new InventoryPdfViewModel.Activity
                                      {
                                          Date = r.ActivityDate,
                                          Description = r.Description
                                      })
                                      .ToList()
            };
        }
    }
}
