using System.ComponentModel;
using System.Text;
using Aldebaran.Infraestructure.Common.Utils;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Exports
{
    /// <summary>CSV de las exportaciones (Pedidos, Back order, Artículos): campos según RFC 4180.</summary>
    public class CsvExportTests
    {
        public class Row
        {
            [DisplayName("Pedido")]
            public string OrderNumber { get; set; } = string.Empty;

            [DisplayName("Teléfono")]
            public string Phone { get; set; } = string.Empty;

            [DisplayName("Observaciones Internas")]
            public string InternalNotes { get; set; } = string.Empty;
        }

        private static async Task<string> CsvAsync(params Row[] rows) =>
            Encoding.UTF8.GetString(await new FileBytesGeneratorService(null!).GetCsvBytes(rows.ToList()));

        [Fact]
        public async Task CleanValues_AreWrittenAsBefore()
        {
            var csv = await CsvAsync(new Row { OrderNumber = "0000038390", Phone = "3204915878", InternalNotes = "ATENDIDO" });

            Assert.Equal($"Pedido,Teléfono,Observaciones Internas{Environment.NewLine}0000038390,3204915878,ATENDIDO{Environment.NewLine}", csv);
        }

        [Fact]
        public async Task ValuesWithCommaLineBreakOrQuotes_AreQuoted()
        {
            var csv = await CsvAsync(new Row { OrderNumber = "0000038343", Phone = "310 5679900,2630815", InternalNotes = "000019\nATENDIDO POR \"MILENA\"" });

            Assert.EndsWith($"0000038343,\"310 5679900,2630815\",\"000019\nATENDIDO POR \"\"MILENA\"\"\"{Environment.NewLine}", csv);
        }
    }
}
