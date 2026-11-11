using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Application.Services.Reports;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Reports
{
    public class ReferenceMovementDetailRulesTests
    {
        private const short Ajustes = 1;
        private const short Pedidos = 5;

        private static ReferenceMovementReport Row(int reference, short warehouse, short? title, string? code = null, int? amount = null,
            string? owner = null, DateTime? date = null, short? op = 1) => new()
        {
            LineId = 1, LineCode = "L1", LineName = "Linea", ItemId = 1, InternalReference = "IR", ItemName = "Item",
            ReferenceId = reference, ReferenceCode = "R", ReferenceName = "Ref", WarehouseId = warehouse, WarehouseName = $"B{warehouse}", Amount = 0,
            TitleId = title, Title = title == null ? null : $"T{title}", Code = code, Date = date ?? new DateTime(2026, 1, 1),
            Owner = owner, MovementAmount = amount, Status = "Realizado", Operator = op
        };

        /// <summary>El SP repite cada línea por cada bodega de la referencia.</summary>
        private static IEnumerable<ReferenceMovementReport> PorBodegas(params ReferenceMovementReport[] lines) =>
            new short[] { 1, 2 }.SelectMany(w => lines.Select(l => Row(l.ReferenceId, w, l.TitleId, l.Code, l.MovementAmount, l.Owner, l.Date, l.Operator)));

        [Fact]
        public void MismoDocumentoEnDosBodegas_ConservaAmbasLineas()
        {
            var rows = PorBodegas(
                Row(10, 0, Ajustes, "5093", 5200, "Entrada Zona Franca por Inventario"),
                Row(10, 0, Ajustes, "5093", 921, "Entrada Bodega Local por Inventario"));

            var lines = ReferenceMovementDetailRules.MovementLines(rows, 10, Ajustes);

            Assert.Equal(2, lines.Count);
            Assert.Equal(6121, lines.Sum(s => s.MovementAmount));
        }

        [Fact]
        public void RepeticionPorBodega_NoDuplicaLineas()
        {
            var rows = PorBodegas(Row(10, 0, Pedidos, "100", 30), Row(10, 0, Pedidos, "101", 20));

            var lines = ReferenceMovementDetailRules.MovementLines(rows, 10, Pedidos);

            Assert.Equal(new[] { "100", "101" }, lines.Select(s => s.Code));
            Assert.Equal(50, lines.Sum(s => s.MovementAmount));
        }

        [Fact]
        public void LineasIdenticasDelMismoDocumento_SeConservan()
        {
            var rows = PorBodegas(Row(10, 0, Ajustes, "7", 5, "Salida Bodega Local por Muestras"), Row(10, 0, Ajustes, "7", 5, "Salida Bodega Local por Muestras"));

            Assert.Equal(2, ReferenceMovementDetailRules.MovementLines(rows, 10, Ajustes).Count);
        }

        [Fact]
        public void ReferenciaConUnaSolaBodega_TraeTodasLasLineas()
        {
            var rows = new[] { Row(10, 3, Ajustes, "1", 4), Row(10, 3, Ajustes, "1", 6), Row(10, 3, Ajustes, "2", 1) };

            Assert.Equal(11, ReferenceMovementDetailRules.MovementLines(rows, 10, Ajustes).Sum(s => s.MovementAmount));
        }

        [Fact]
        public void OrdenaPorFechaYLuegoPorCodigoNumerico()
        {
            var d1 = new DateTime(2026, 3, 1);
            var d2 = new DateTime(2026, 3, 2);
            var rows = PorBodegas(Row(10, 0, Pedidos, "9", 1, date: d2), Row(10, 0, Pedidos, "10", 1, date: d1), Row(10, 0, Pedidos, "9", 1, date: d1));

            var codes = ReferenceMovementDetailRules.MovementLines(rows, 10, Pedidos).Select(s => $"{s.Date:dd}-{s.Code}");

            Assert.Equal(new[] { "01-9", "01-10", "02-9" }, codes);
        }

        [Fact]
        public void SoloLaSeccionYReferenciaPedidas()
        {
            var rows = PorBodegas(Row(10, 0, Ajustes, "1", 1), Row(10, 0, Pedidos, "2", 1), Row(11, 0, Ajustes, "3", 1));

            Assert.Equal(new[] { "1" }, ReferenceMovementDetailRules.MovementLines(rows, 10, Ajustes).Select(s => s.Code));
        }

        [Fact]
        public void ReferenciaSinFilas_Vacio() =>
            Assert.Empty(ReferenceMovementDetailRules.MovementLines(Array.Empty<ReferenceMovementReport>(), 10, Ajustes));
    }
}
