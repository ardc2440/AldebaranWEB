using System.Text.RegularExpressions;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>
    /// El PDF se genera en el servidor a partir del HTML de la pantalla, sin URL base: las imágenes con ruta relativa
    /// (/externalimages/Competencia/...) no cargarían. Este caso de uso las incrusta como data URI (base64).
    /// Las imágenes que no existen se quitan para no dejar íconos rotos. Solo toca imágenes de la carpeta indicada.
    /// </summary>
    public static class CompetitorReportImageEmbedder
    {
        private static readonly IReadOnlyDictionary<string, string> MimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

        public static async Task<string> EmbedAsync(string html, string urlPrefix, Func<string, Task<byte[]?>> readImage)
        {
            if (string.IsNullOrEmpty(html) || string.IsNullOrEmpty(urlPrefix))
                return html;
            var pattern = new Regex("<img\\b[^>]*?\\bsrc=\"" + Regex.Escape(urlPrefix) + "(?<name>[^\"]+)\"[^>]*>", RegexOptions.IgnoreCase);
            var cache = new Dictionary<string, string?>();
            var result = new System.Text.StringBuilder();
            var last = 0;
            foreach (Match match in pattern.Matches(html))
            {
                result.Append(html, last, match.Index - last);
                var fileName = Uri.UnescapeDataString(match.Groups["name"].Value);
                if (!cache.TryGetValue(fileName, out var dataUri))
                {
                    dataUri = ToDataUri(fileName, await readImage(fileName));
                    cache[fileName] = dataUri;
                }
                if (dataUri != null)
                {
                    var src = match.Groups["name"];
                    var start = src.Index - match.Index - urlPrefix.Length;
                    var tag = match.Value;
                    result.Append(tag, 0, start).Append(dataUri).Append(tag, start + urlPrefix.Length + src.Length, tag.Length - start - urlPrefix.Length - src.Length);
                }
                last = match.Index + match.Length;
            }
            result.Append(html, last, html.Length - last);
            return result.ToString();
        }

        internal static string? ToDataUri(string fileName, byte[]? content)
        {
            if (content == null || content.Length == 0 || !MimeTypes.TryGetValue(Path.GetExtension(fileName), out var mime))
                return null;
            return $"data:{mime};base64,{Convert.ToBase64String(content)}";
        }
    }
}
