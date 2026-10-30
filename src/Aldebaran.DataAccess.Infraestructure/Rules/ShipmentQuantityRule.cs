using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Rules
{
    /// <summary>
    /// Cantidad de un despacho que supera lo disponible en proceso para un detalle del pedido.
    /// </summary>
    public sealed record ShipmentQuantityViolation(int CustomerOrderDetailId, int RequestedQuantity, int AvailableQuantity);

    /// <summary>
    /// Regla de despachos: lo que se agrega a despachar de un detalle del pedido no puede superar su cantidad en proceso.
    /// Sin acceso a datos, para poder probarla de forma aislada.
    /// </summary>
    public static class ShipmentQuantityRule
    {
        /// <summary>
        /// Cantidad despachada por detalle de pedido.
        /// </summary>
        public static Dictionary<int, int> GroupByDetail(IEnumerable<CustomerOrderShipmentDetail> details) =>
            details.GroupBy(g => g.CustomerOrderDetailId).ToDictionary(k => k.Key, v => v.Sum(s => s.DeliveredQuantity));

        /// <summary>
        /// Primer detalle cuyo aumento supera la cantidad en proceso, o null si todo está dentro de lo disponible.
        /// </summary>
        /// <param name="newQuantities">Cantidades del despacho después del cambio, por detalle.</param>
        /// <param name="currentQuantities">Cantidades que el despacho ya tenía guardadas (vacío al crear).</param>
        /// <param name="processedQuantities">Cantidad en proceso vigente en la BD, por detalle.</param>
        public static ShipmentQuantityViolation? FindViolation(IReadOnlyDictionary<int, int> newQuantities, IReadOnlyDictionary<int, int> currentQuantities, IReadOnlyDictionary<int, int> processedQuantities)
        {
            foreach (var (detailId, newQuantity) in newQuantities)
            {
                var currentQuantity = currentQuantities.GetValueOrDefault(detailId);
                var increase = newQuantity - currentQuantity;
                if (increase <= 0)
                    continue;

                var processed = processedQuantities.GetValueOrDefault(detailId);
                if (increase > processed)
                    return new ShipmentQuantityViolation(detailId, newQuantity, processed + currentQuantity);
            }

            return null;
        }
    }
}
