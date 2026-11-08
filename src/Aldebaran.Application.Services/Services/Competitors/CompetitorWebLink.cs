namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>
    /// Direcciones web de la competencia (sitio de la compañía y enlace del producto). Solo se aceptan y se
    /// publican como enlace direcciones http/https absolutas: así un texto como "javascript:..." nunca termina en un href.
    /// </summary>
    public static class CompetitorWebLink
    {
        /// <summary>Recorta espacios; vacío = null; sin esquema se antepone https://.</summary>
        public static string? Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            var url = value.Trim();
            return url.Contains("://", StringComparison.Ordinal) ? url : "https://" + url;
        }

        /// <summary>Dirección absoluta http/https con dominio (contiene un punto).</summary>
        public static bool IsWebUrl(string? value) =>
            Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            && uri.Host.Contains('.');

        /// <summary>Valor seguro para un href (normalizado y validado); null si no es una dirección web válida.</summary>
        public static string? ToSafeHref(string? value)
        {
            var url = Normalize(value);
            return IsWebUrl(url) ? url : null;
        }
    }
}
