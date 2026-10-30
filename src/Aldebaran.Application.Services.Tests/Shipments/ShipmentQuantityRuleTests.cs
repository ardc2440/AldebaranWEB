using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Rules;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Shipments
{
    public class ShipmentQuantityRuleTests
    {
        private static Dictionary<int, int> Map(params (int Detail, int Quantity)[] items) => items.ToDictionary(k => k.Detail, v => v.Quantity);

        [Fact]
        public void Crear_DentroDeLoEnProceso_NoHayViolacion() =>
            Assert.Null(ShipmentQuantityRule.FindViolation(Map((1, 5), (2, 450)), Map(), Map((1, 5), (2, 450))));

        [Fact]
        public void Crear_DigitoDeMas_HayViolacion()
        {
            var violation = ShipmentQuantityRule.FindViolation(Map((1, 53)), Map(), Map((1, 5)));

            Assert.NotNull(violation);
            Assert.Equal(1, violation!.CustomerOrderDetailId);
            Assert.Equal(53, violation.RequestedQuantity);
            Assert.Equal(5, violation.AvailableQuantity);
        }

        [Fact]
        public void Crear_SegundoDespachoConEnProcesoYaDescontado_HayViolacion() =>
            Assert.NotNull(ShipmentQuantityRule.FindViolation(Map((1, 5)), Map(), Map((1, 0))));

        [Fact]
        public void Crear_DetalleSinEnProceso_HayViolacion() =>
            Assert.NotNull(ShipmentQuantityRule.FindViolation(Map((9, 1)), Map(), Map((1, 5))));

        [Fact]
        public void Editar_SubeDentroDeLoDisponible_NoHayViolacion() =>
            Assert.Null(ShipmentQuantityRule.FindViolation(Map((1, 8)), Map((1, 5)), Map((1, 3))));

        [Fact]
        public void Editar_SubePorEncimaDeLoDisponible_HayViolacionConDisponibleTotal()
        {
            var violation = ShipmentQuantityRule.FindViolation(Map((1, 9)), Map((1, 5)), Map((1, 3)));

            Assert.NotNull(violation);
            Assert.Equal(8, violation!.AvailableQuantity);
        }

        [Fact]
        public void Editar_BajaOMantiene_NoHayViolacionAunqueNoHayaEnProceso() =>
            Assert.Null(ShipmentQuantityRule.FindViolation(Map((1, 2), (2, 4)), Map((1, 5), (2, 4)), Map((1, 0), (2, 0))));

        [Fact]
        public void Editar_QuitaReferencia_NoHayViolacion() =>
            Assert.Null(ShipmentQuantityRule.FindViolation(Map((2, 4)), Map((1, 5), (2, 4)), Map()));

        [Fact]
        public void GroupByDetail_SumaPorDetalle()
        {
            var result = ShipmentQuantityRule.GroupByDetail(new[]
            {
                new CustomerOrderShipmentDetail { CustomerOrderDetailId = 1, DeliveredQuantity = 2 },
                new CustomerOrderShipmentDetail { CustomerOrderDetailId = 1, DeliveredQuantity = 3 },
                new CustomerOrderShipmentDetail { CustomerOrderDetailId = 2, DeliveredQuantity = 4 }
            });

            Assert.Equal(5, result[1]);
            Assert.Equal(4, result[2]);
        }
    }
}
