using Aldebaran.Application.Services.InventoryMinimumAlerts.Models;
using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.InventoryMinimumAlerts
{
    /// <summary>Grupo del Excel: primera alarma del artículo + inventario de todas sus referencias activas.</summary>
    internal sealed record ArticleAlarmGroup(InventoryMinimumDto Alarm, IReadOnlyList<ArticleReferenceInventory> Inventory);

    /// <summary>
    /// Arma las filas agrupadas del Excel del correo (funciones puras, sin acceso a datos).
    /// Padre = alarma (imagen y cifras de su referencia); hijos = referencias del artículo,
    /// marcando las que están en alarma (D9).
    /// </summary>
    internal static class InventoryMinimumGroupedExportBuilder
    {
        private const string InAlarmText = "Sí";

        public static List<InventoryMinimumGroupedExportDto> Build(IReadOnlyCollection<InventoryMinimumDto> alarms, IReadOnlyList<ArticleAlarmGroup> groups)
        {
            var minimumByReference = alarms
                .GroupBy(alarm => alarm.ReferenceId)
                .ToDictionary(group => group.Key, group => group.First().MinimumQuantity);

            return groups.Select(group => BuildParent(group, minimumByReference)).ToList();
        }

        private static InventoryMinimumGroupedExportDto BuildParent(ArticleAlarmGroup group, IReadOnlyDictionary<int, int> minimumByReference)
        {
            var alarm = group.Alarm;
            var alarmReference = group.Inventory.FirstOrDefault(row => row.ReferenceId == alarm.ReferenceId);

            var parent = alarmReference is null ? FromAlarmOnly(alarm) : FromInventory(alarmReference, alarm.ArticleName);
            parent.ImagePath = alarm.ImagePath;
            parent.InAlarm = InAlarmText;
            parent.MinimumQuantity = alarm.MinimumQuantity;
            parent.Children = group.Inventory.Select(row => BuildChild(row, minimumByReference)).ToList();

            return parent;
        }

        private static InventoryMinimumGroupedExportDto BuildChild(ArticleReferenceInventory row, IReadOnlyDictionary<int, int> minimumByReference)
        {
            var child = FromInventory(row, $"{row.ReferenceCode} - {row.ReferenceName}");

            if (minimumByReference.TryGetValue(row.ReferenceId, out var minimumQuantity))
            {
                child.InAlarm = InAlarmText;
                child.MinimumQuantity = minimumQuantity;
            }

            return child;
        }

        private static InventoryMinimumGroupedExportDto FromInventory(ArticleReferenceInventory row, string description) => new()
        {
            Description = description,
            LocalWarehouse = row.LocalWarehouse,
            FreeZone = row.FreeZone,
            PhysicalStock = row.PhysicalStock,
            InTransit = row.InTransit,
            Committed = row.Committed,
            Available = row.Available
        };

        /// <summary>Respaldo si el inventario no trae la referencia de la alarma: cifras de la alarma, sin desglose por bodega.</summary>
        private static InventoryMinimumGroupedExportDto FromAlarmOnly(InventoryMinimumDto alarm)
        {
            var committed = alarm.ReservedQuantity + alarm.OrderedQuantity;

            return new InventoryMinimumGroupedExportDto
            {
                Description = alarm.ArticleName,
                PhysicalStock = alarm.AvailableQuantity,
                InTransit = alarm.InTransitQuantity,
                Committed = committed,
                Available = alarm.AvailableQuantity + alarm.InTransitQuantity - committed
            };
        }
    }
}
