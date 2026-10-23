using System.Text.RegularExpressions;

namespace Aldebaran.Application.Services.Models
{
    /// <summary>
    /// Sufijos que las marcas comerciales del artículo (Oferta y Precio Bomba) agregan a su nombre.
    /// Las marcas son excluyentes: al aplicar una se retira el sufijo de la otra.
    /// </summary>
    public static class ItemNameMarks
    {
        public const int MaxNameLength = 50;
        public const string SaleOffSuffix = " - Oferta";
        public const string BombPriceSuffix = " - $ BOMBA";

        /// <summary>Agrega el sufijo de la marca; false si el nombre resultante supera <see cref="MaxNameLength"/>.</summary>
        public static bool TryApply(string itemName, string suffix, out string result)
        {
            var baseName = RemoveMarks(itemName ?? string.Empty, suffix);
            if (baseName.Length + suffix.Length > MaxNameLength)
            {
                result = itemName ?? string.Empty;
                return false;
            }

            result = baseName + suffix;
            return true;
        }

        /// <summary>Retira el sufijo de la marca.</summary>
        public static string Remove(string itemName, string suffix) => (itemName ?? string.Empty).Replace(suffix, string.Empty);

        /// <summary>Máximo de caracteres del nombre base para poder aplicar la marca.</summary>
        public static int MaxBaseLength(string suffix) => MaxNameLength - suffix.Length;

        private static string RemoveMarks(string itemName, string suffix)
        {
            var name = itemName.Replace(SaleOffSuffix, string.Empty).Replace(BombPriceSuffix, string.Empty);

            // Oferta conserva su regla original: retira la palabra "Oferta" en cualquier parte del nombre.
            name = suffix == SaleOffSuffix
                ? Regex.Replace(name, "Oferta", string.Empty, RegexOptions.IgnoreCase)
                : Regex.Replace(name, Regex.Escape("$ BOMBA"), string.Empty, RegexOptions.IgnoreCase);

            return name.Trim();
        }
    }
}
