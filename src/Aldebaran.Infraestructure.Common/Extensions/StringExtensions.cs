using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Aldebaran.Infraestructure.Common.Extensions
{
    /// <summary>
    /// Metodos de extension para strings
    /// <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Valor por defecto para cifrar y descifrar una cadena de caracteres
        /// </summary>
        private const string Salt = "30811BF8E9DD44198EAB8301636B612C";

        /// <summary>
        /// Método de extensión que reemplaza etiquetas en una cadena con valores proporcionados en un diccionario.
        /// </summary>
        /// <param name="value">La cadena en la que se realizará el reemplazo de etiquetas.</param>
        /// <param name="wildcard">El carácter que indica el comienzo de una etiqueta.</param>
        /// <param name="tags">Diccionario que contiene las etiquetas y sus valores de reemplazo.</param>
        /// <returns>Una cadena con las etiquetas reemplazadas por sus valores correspondientes.</returns>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// string template = "Hola, #nombre!";
        /// var dictionary = new Dictionary<string, string> { { "nombre", "Juan" } };
        /// string resultado = template.ToRender('#', dictionary);
        /// Console.WriteLine(resultado); // Salida esperada: "Hola, Juan!"
        /// </code>
        /// </example>
        public static string ToRender(this string value, char wildcard, Dictionary<string, string> tags)
        {
            // Define una expresión regular que busca el carácter de comodín seguido de una palabra (\w+).
            Regex re = new($@"\{wildcard}(\w+)", RegexOptions.Compiled);
            // Reemplaza cada coincidencia encontrada por su valor correspondiente en el diccionario de etiquetas.
            string output = re.Replace(value, match =>
            {
                var key = match.Groups[1].Value;
                if (tags.ContainsKey(key))
                    return tags[match.Groups[1].Value];
                return $"{wildcard}{key}";
            });
            // Retorna la cadena resultante después de realizar todos los reemplazos.
            return output;
        }

        /// <summary>
        /// Permite cifrar con una llave una cadena de caracteres
        /// </summary>
        /// <param name="text">Cadena a cifrar</param>
        /// <param name="salt">Llave de cifrado</param>
        /// <example>
        /// <returns>Cadena de caracteres cifrada</returns>
        /// <code>
        /// string cadena = "Hola mundo";
        /// Console.WriteLine(cadena.Encrypt()); //returns: xeajS4v0ltUsKnFKU0+HA45RzLagXRsMC7E2RzG3wi7VSKQU/u9z2ikAeSVpIiHw
        /// </code>
        /// </example>
        public static string Encrypt(this string text, string salt = Salt)
        {
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));
            var key = Encoding.UTF8.GetBytes(salt);
            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }
            var iv = aesAlg.IV;
            var decryptedContent = msEncrypt.ToArray();
            var result = new byte[iv.Length + decryptedContent.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Permite descifrar con una llave una cadena de caracteres
        /// </summary>
        /// <param name="text">Cadena a descifrar</param>
        /// <param name="salt">Llave de descifrado</param>
        /// <returns>Cadena de caracteres descifrada</returns>
        /// <example>
        /// <code>
        /// string cipherText = "xeajS4v0ltUsKnFKU0+HA45RzLagXRsMC7E2RzG3wi7VSKQU/u9z2ikAeSVpIiHw";
        /// Console.WriteLine(cipherText.Encrypt()); //returns: Hola mundo
        /// </code>
        /// </example>
        public static string Decrypt(this string text, string salt = Salt)
        {
            if (string.IsNullOrEmpty(salt))
                throw new ArgumentNullException(nameof(salt));
            var fullCipher = Convert.FromBase64String(text);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
            var key = Encoding.UTF8.GetBytes(salt);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }
            return result;
        }
    }
}
