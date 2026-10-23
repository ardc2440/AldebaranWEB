using System.Text.RegularExpressions;

namespace Aldebaran.Web.Shared
{
    /// <summary>
    /// Ubica la imagen de un artículo en el repositorio de imágenes con la misma regla de ImageDialog
    /// (código entre corchetes del nombre + ".jpg"), sin modificar ese componente compartido.
    /// </summary>
    public static class ArticleImageLocator
    {
        public static bool Exists(string articleName, string imageRepositoryPath)
        {
            if (string.IsNullOrWhiteSpace(articleName) || string.IsNullOrWhiteSpace(imageRepositoryPath))
                return false;

            return File.Exists(Path.Combine(imageRepositoryPath, GetCode(articleName) + ".jpg"));
        }

        private static string GetCode(string articleName)
        {
            var match = Regex.Match(articleName, @"\[(.*?)\]");
            return match.Success ? match.Groups[1].Value : articleName;
        }
    }
}
