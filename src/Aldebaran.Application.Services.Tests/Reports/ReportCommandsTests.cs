using System.Data;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Xunit;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Tests.Reports
{
    /// <summary>Llamadas parametrizadas a los SP de reportes que antes concatenaban los filtros en el SQL.</summary>
    public class ReportCommandsTests
    {
        private const string Injection = "1'; DROP TABLE items; --";

        [Fact]
        public void SinFiltros_SoloEjecutaElSp()
        {
            Assert.Equal("EXEC dbo.SP_GET_PURCHASE_ORDER_REPORT", ReportCommands.PurchaseOrder(new Entities.PurchaseOrderReportFilter()).Sql);
            Assert.Equal("EXEC dbo.SP_GET_CUSTOMER_RESERVATION_REPORT", ReportCommands.CustomerReservation(new Entities.CustomerReservationReportFilter()).Sql);
            Assert.Equal("EXEC dbo.SP_GET_INVENTORY_ADJUSTMENT_REPORT", ReportCommands.InventoryAdjustment(new Entities.InventoryAdjustmentReportFilter()).Sql);
            Assert.Equal("EXEC dbo.SP_GET_WAREHOUSE_STOCK_REPORT", ReportCommands.WarehouseStock(new Entities.WarehouseStockReportFilter()).Sql);
            Assert.Equal("EXEC dbo.SP_GET_WAREHOUSE_TRANSFER_REPORT", ReportCommands.WarehouseTransfer(new Entities.WarehouseTransferReportFilter()).Sql);
        }

        [Fact]
        public void FiltroNulo_Error() =>
            Assert.Throws<ArgumentNullException>(() => ReportCommands.PurchaseOrder(null!));

        [Fact]
        public void TextoMalicioso_ViajaComoValorDelParametro_NoEnElSql()
        {
            var command = ReportCommands.PurchaseOrder(new Entities.PurchaseOrderReportFilter { ImportNumber = Injection, EmbarkationPort = Injection, ProformaNumber = Injection, OrderNumber = Injection });
            Assert.DoesNotContain("DROP", command.Sql);
            Assert.Equal("EXEC dbo.SP_GET_PURCHASE_ORDER_REPORT @OrderNumber = @OrderNumber, @ImportNumber = @ImportNumber, @EmbarkationPort = @EmbarkationPort, @ProformaNumber = @ProformaNumber", command.Sql);
            Assert.All(command.Parameters, p => Assert.Equal(Injection.Trim(), p.Value));
        }

        [Fact]
        public void OrdenDeCompra_TiposTamanosYFechasSinHora()
        {
            var command = ReportCommands.PurchaseOrder(new Entities.PurchaseOrderReportFilter
            {
                OrderNumber = " 123 ",
                CreationDateFrom = new DateTime(2026, 1, 2, 15, 30, 0),
                CreationDateTo = new DateTime(2026, 2, 3),
                ProviderId = 7,
                ShipmentMethodId = 2,
                StatusDocumentTypeId = 3,
                ReferenceIds = "1,2",
                RequestedQuantityDifference = true
            });
            var p = command.Parameters.ToDictionary(x => x.ParameterName);
            Assert.Equal("123", p["@OrderNumber"].Value);
            Assert.Equal(10, p["@OrderNumber"].Size);
            Assert.Equal(SqlDbType.Date, p["@CreationDateFrom"].SqlDbType);
            Assert.Equal(new DateTime(2026, 1, 2), p["@CreationDateFrom"].Value);
            Assert.Equal(7, p["@ProviderId"].Value);
            Assert.Equal(2, p["@ShipmentMethodId"].Value);
            Assert.Equal("1,2", p["@ReferenceIds"].Value);
            Assert.Equal(true, p["@RequestedQuantityDifference"].Value);
        }

        [Fact]
        public void OrdenDeCompra_DiferenciaCantidadEnNo_NoSeEnvia() =>
            Assert.DoesNotContain(ReportCommands.PurchaseOrder(new Entities.PurchaseOrderReportFilter { RequestedQuantityDifference = false }).Parameters,
                p => p.ParameterName == "@RequestedQuantityDifference");

        [Fact]
        public void Reservas_TodosLosFiltros()
        {
            var command = ReportCommands.CustomerReservation(new Entities.CustomerReservationReportFilter
            {
                ReservationNumber = "R1", CreationDateFrom = new DateTime(2026, 1, 1), CreationDateTo = new DateTime(2026, 1, 31),
                ReservationDateFrom = new DateTime(2026, 2, 1), ReservationDateTo = new DateTime(2026, 2, 28),
                StatusDocumentTypeId = 1, CustomerId = 9, ReferenceIds = "5"
            });
            Assert.Equal(new[] { "@ReservationNumber", "@CreationDateFrom", "@CreationDateTo", "@ReservationDateFrom", "@ReservationDateTo", "@StatusDocumentTypeId", "@CustomerId", "@ReferenceIds" },
                command.Parameters.Select(p => p.ParameterName));
        }

        [Fact]
        public void Ajustes_TodosLosFiltros()
        {
            var command = ReportCommands.InventoryAdjustment(new Entities.InventoryAdjustmentReportFilter
            {
                AdjustmentId = 1, AdjustmentDateFrom = DateTime.Today, AdjustmentDateTo = DateTime.Today, CreationDateFrom = DateTime.Today, CreationDateTo = DateTime.Today,
                AdjustmentTypeId = 1, AdjustmentReasonId = 2, EmployeeId = 3, ReferenceIds = "4"
            });
            Assert.Equal(9, command.Parameters.Length);
        }

        [Fact]
        public void Traslados_NacionalizacionLimitadaA30()
        {
            var command = ReportCommands.WarehouseTransfer(new Entities.WarehouseTransferReportFilter { NationalizationNumber = "N-1", StatusId = 2, TargetWarehouseId = 1, SourceWarehouseId = 2 });
            var p = command.Parameters.ToDictionary(x => x.ParameterName);
            Assert.Equal(30, p["@NationalizationNumber"].Size);
            Assert.Equal(2, p["@StatusId"].Value);
            Assert.Equal(4, p.Count);
        }

        [Fact]
        public void ExistenciasPorBodega_BodegaYReferencias()
        {
            var command = ReportCommands.WarehouseStock(new Entities.WarehouseStockReportFilter { WarehouseId = 3, ReferenceIds = "1,2" });
            Assert.Equal("EXEC dbo.SP_GET_WAREHOUSE_STOCK_REPORT @WarehouseId = @WarehouseId, @ReferenceIds = @ReferenceIds", command.Sql);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("1,2", "1,2")]
        public void SoloReferencias_SiempreEnviaElParametro(string? ids, string expected)
        {
            var command = ReportCommands.ByReferenceIds("dbo.SP_GET_INVENTORY_REPORT", ids);
            Assert.Equal("EXEC dbo.SP_GET_INVENTORY_REPORT @ReferenceIds = @ReferenceIds", command.Sql);
            Assert.Equal(expected, Assert.Single(command.Parameters).Value);
        }
    }
}
