using Aldebaran.Application.Services.Models.AlarmLogs;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>
    /// Datos de presentación de cada contexto de la consulta de visualización de alarmas:
    /// segmento de la URL, título y rol que lo habilita.
    /// </summary>
    public sealed record AlarmLogContextInfo(string Route, AlarmLogContext Context, string Title, string Role)
    {
        public const string AdministratorRole = "Administrador";

        public static IReadOnlyList<AlarmLogContextInfo> All { get; } = new[]
        {
            new AlarmLogContextInfo("purchase-orders", AlarmLogContext.PurchaseOrders, "Visualización de alarmas de órdenes de compra", "Log de ordenes de compra"),
            new AlarmLogContextInfo("customer-orders", AlarmLogContext.CustomerOrders, "Visualización de alarmas de pedidos", "Log de pedidos"),
            new AlarmLogContextInfo("customer-reservations", AlarmLogContext.CustomerReservations, "Visualización de alarmas de reservas", "Log de reservas"),
            new AlarmLogContextInfo("inventory", AlarmLogContext.Inventory, "Visualización de alarmas de inventario", "Log de inventarios")
        };

        public static AlarmLogContextInfo FromRoute(string route) =>
            All.FirstOrDefault(c => string.Equals(c.Route, route, StringComparison.OrdinalIgnoreCase));

        /// <summary>Roles de log que tiene el usuario (los únicos que interesan a la consulta).</summary>
        public static IReadOnlyList<string> UserLogRoles(SecurityService security) =>
            All.Select(c => c.Role).Append(AdministratorRole).Where(r => security.IsInRole(r)).ToList();
    }
}
