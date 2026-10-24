using Aldebaran.Application.Services.Models.InternalReferences;
using Xunit;

namespace Aldebaran.Application.Services.Tests.InternalReferences
{
    public class InternalReferenceCodeRulesTests
    {
        private const string InvalidCharacters = "El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).";

        [Theory]
        [InlineData("mu", "MU")]
        [InlineData("  Mux  ", "MUX")]
        [InlineData("pm va", "PM VA")]
        [InlineData("ñu", "ÑU")]
        [InlineData(null, "")]
        public void Normalize_ReturnsUpperCaseTrimmedCode(string? code, string expected) =>
            Assert.Equal(expected, InternalReferenceCodeRules.Normalize(code));

        [Theory]
        [InlineData("M")]
        [InlineData("MU")]
        [InlineData("VA")]
        [InlineData("ABCDE")]
        [InlineData("PM VA")]
        [InlineData("B10")]
        [InlineData("123")]
        [InlineData("ÑU")]
        [InlineData("A B C")]
        public void ValidateCode_AcceptsValidCodes(string code) =>
            Assert.Null(InternalReferenceCodeRules.ValidateCode(code));

        [Theory]
        [InlineData("", "El código es obligatorio.")]
        [InlineData("ABCDEF", "El código puede tener máximo 5 caracteres.")]
        [InlineData("TAPA MU", "El código puede tener máximo 5 caracteres.")]
        [InlineData("M-U", InvalidCharacters)]
        [InlineData("----", InvalidCharacters)]
        [InlineData("A_1%", InvalidCharacters)]
        [InlineData("M  U", InvalidCharacters)]
        [InlineData("M.U", InvalidCharacters)]
        [InlineData("MÚ", InvalidCharacters)]
        public void ValidateCode_RejectsInvalidCodes(string code, string expected) =>
            Assert.Equal(expected, InternalReferenceCodeRules.ValidateCode(code));

        [Fact]
        public void LongestSuggestion_FitsInternalReferenceColumn() =>
            // CÓDIGO de 5 + "-" + 9999 debe caber en items.INTERNAL_REFERENCE (varchar(10)).
            Assert.True(InternalReferenceCodeRules.Format("ABCDE", InternalReferenceCodeRules.MaxConsecutive).Length <= 10);

        [Theory]
        [InlineData("MU", 5, "MU-005")]
        [InlineData("MU", 25, "MU-025")]
        [InlineData("VA", 999, "VA-999")]
        [InlineData("VA", 1000, "VA-1000")]
        [InlineData("PM VA", 9999, "PM VA-9999")]
        public void Format_PadsToThreeDigitsAndGrowsToFour(string code, int consecutive, string expected) =>
            Assert.Equal(expected, InternalReferenceCodeRules.Format(code, consecutive));

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(1000)]
        public void ValidateQuantity_AcceptsBetweenOneAndMax(int quantity) =>
            Assert.Null(InternalReferenceCodeRules.ValidateQuantity(quantity));

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        [InlineData(1001)]
        public void ValidateQuantity_RejectsOutOfRange(int quantity) =>
            Assert.Equal("La cantidad debe estar entre 1 y 1000.", InternalReferenceCodeRules.ValidateQuantity(quantity));
    }
}
