using Aldebaran.Application.Services.TrackLogs;
using Xunit;

namespace Aldebaran.Application.Services.Tests.TrackLogs
{
    public class TrackDataParserTests
    {
        [Theory]
        [InlineData("{\"IsSpecialOrder\":\"False\"}", "No")]
        [InlineData("{\"IsSpecialOrder\":\"True\"}", "Sí")]
        [InlineData("{\"IsSpecialOrder\":false}", "No")]
        [InlineData("{\"IsSpecialOrder\":true}", "Sí")]
        public void Parse_ShowsBooleansAsSiNo(string json, string expected) =>
            Assert.Equal(expected, TrackDataParser.Parse(json)["IsSpecialOrder"]);

        [Theory]
        [InlineData("{\"Notes\":\"Prueba\"}", "Notes", "Prueba")]
        [InlineData("{\"Quantity\":15}", "Quantity", "15")]
        [InlineData("{\"Notes\":null}", "Notes", null)]
        public void Parse_KeepsOtherValues(string json, string field, string? expected) =>
            Assert.Equal(expected, TrackDataParser.Parse(json)[field]);
    }
}
