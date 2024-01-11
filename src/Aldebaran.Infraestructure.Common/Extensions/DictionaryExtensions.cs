namespace Aldebaran.Infraestructure.Common.Extensions
{
    /// <summary>
    /// Metodos de extension para strings
    /// <see cref="Dictionary"/>
    /// </summary>
    public static class DictionaryExtensions
    {
        public static void AddOrUpdate<T, K>(this Dictionary<T, K> dict, T key, K value)
        {
            if (dict == null)
                dict = new Dictionary<T, K>();
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static bool? Get<T>(this Dictionary<T, bool> dict, T key, bool value)
        {
            if (dict == null)
                return false;
            var contains = dict.ContainsKey(key);
            return contains && dict[key] == value;
        }
    }
}
