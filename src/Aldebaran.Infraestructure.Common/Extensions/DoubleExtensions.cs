namespace Aldebaran.Infraestructure.Common.Extensions
{
    /// <summary>
    /// Metodos de extension para números de punto flotante de doble precisión
    /// <see cref="double"/>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Permite formatear números grandes de manera más legible utilizando prefijos de escala (como K, M, B para miles, millones, miles de millones, etc.).
        /// </summary>
        /// <param name="number">Numero a formatear</param>
        /// <returns>Una cadena que representa el número formateado con el prefijo de escala correspondiente.</returns>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// double myNumber = 1500;
        /// string formattedNumber = myNumber.FormatLargerNumbers();
        /// Console.WriteLine(formattedNumber); // Salida esperada: "1.5K"
        /// </code>
        /// </example>
        public static string FormatLargerNumbers(this double number)
        {
            // Si el número es cero, devuelve directamente la cadena "0".
            if (number == 0)
                return "0";
            // Define los prefijos para las escalas de mil, millón, mil millones, etc.
            string[] prefix = { string.Empty, "K", "M", "B" };
            // Obtiene el valor absoluto del número para evitar problemas con números negativos.
            var absnum = Math.Abs(number);
            double add;
            // Calcula el exponente de la potencia de 10 que indica la escala.
            // Si el valor absoluto del número es menor que 1, se ajusta la escala hacia abajo.
            if (absnum < 1)
                add = (int)Math.Floor(Math.Floor(Math.Log10(absnum)) / 3);
            else
                add = (int)(Math.Floor(Math.Log10(absnum)) / 3);
            // Calcula el número ajustado dividiendo el número original por 10 elevado a la potencia de 3 multiplicada por el exponente calculado.
            var shortNumber = number / Math.Pow(10, add * 3);
            // Formatea el número ajustado como una cadena con un solo decimal y agrega el prefijo correspondiente.
            // La conversión a Convert.ToInt32(add) se realiza para obtener el índice correcto en el arreglo de prefijos.
            return string.Format("{0}{1}", shortNumber.ToString("0.#"), (prefix[Convert.ToInt32(add)]));
        }
    }
}
