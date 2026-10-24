using System.Text.RegularExpressions;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    /// <summary>
    /// Filas del resumen del pedido de uso interno: se ven al imprimir o guardar PDF,
    /// pero se retiran del documento que se envía por correo al cliente.
    /// </summary>
    public static class InternalOnlyContent
    {
        public const string CssClass = "internal-only";

        private static readonly Regex InternalRows = new($@"<tr\b[^>]*\bclass=""{CssClass}""[^>]*>.*?</tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Remove(string html) => string.IsNullOrEmpty(html) ? html : InternalRows.Replace(html, string.Empty);
    }
}
