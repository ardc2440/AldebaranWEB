using Aldebaran.Application.Services.Models.InternalReferences;
using Xunit;

namespace Aldebaran.Application.Services.Tests.InternalReferences
{
    public class InternalReferenceCodeRulesTests
    {
        [Theory]
        [InlineData("mu", "MU")]
        [InlineData("  Mux  ", "MUX")]
        [InlineData("----", "----")]
        [InlineData(null, "")]
        public void Normalize_ReturnsUpperCaseTrimmedCode(string? code, string expected) =>
            Assert.Equal(expected, InternalReferenceCodeRules.Normalize(code));

        [Theory]
        [InlineData("MU")]
        [InlineData("ABCDEF")]
        [InlineData("----")]
        [InlineData("A_1%")]
        public void ValidateCode_AcceptsValidCodes(string code) =>
            Assert.Null(InternalReferenceCodeRules.ValidateCode(code));

        [Theory]
        [InlineData("", "El código es obligatorio.")]
        [InlineData("M U", "El código no puede contener espacios.")]
        [InlineData("ABCDEFG", "El código puede tener máximo 6 caracteres.")]
        public void ValidateCode_RejectsInvalidCodes(string code, string expected) =>
            Assert.Equal(expected, InternalReferenceCodeRules.ValidateCode(code));

        [Fact]
        public void MaxCodeLength_FitsInternalReferenceColumn() =>
            // CÓDIGO + "-" + 3 dígitos debe caber en items.INTERNAL_REFERENCE (varchar(10)).
            Assert.True(InternalReferenceCodeRules.MaxCodeLength + 4 <= 10);

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        public void ValidateQuantity_AcceptsPositive(int quantity) =>
            Assert.Null(InternalReferenceCodeRules.ValidateQuantity(quantity));

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void ValidateQuantity_RejectsZeroOrNegative(int quantity) =>
            Assert.Equal("La cantidad debe ser mayor que cero.", InternalReferenceCodeRules.ValidateQuantity(quantity));
    }
}
