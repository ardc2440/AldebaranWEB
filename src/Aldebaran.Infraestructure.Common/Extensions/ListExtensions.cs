namespace Aldebaran.Infraestructure.Common.Extensions
{
    public static class ListExtensions
    {
        public static bool IsEqual<T>(this List<T> first, List<T> second) where T : class
        {
            if (first == null || second == null || first.Count != second.Count)
            {
                return false;
            }

            for (var i = 0; i < first.Count; i++)
            {
                var a = first[i];
                var b = second[i];

                for (var j = 0; j < a.GetType().GetProperties().Length; j++)
                {
                    var val1 = a.GetType().GetProperties()[j].GetValue(a);
                    var val2 = b.GetType().GetProperties()[j].GetValue(b);

                    if ((val1 == null && val2 != null) || (val1 != null && val2 == null))
                        return false;

                    if (val1 != null && val2 != null && (val1.GetType() == Type.GetType("System.Int32") ||
                                                         val1.GetType() == Type.GetType("System.Int64") ||
                                                         val1.GetType() == Type.GetType("System.Double") ||
                                                         val1.GetType() == Type.GetType("System.Decimal") ||
                                                         val1.GetType() == Type.GetType("System.DateTime") ||
                                                         val1.GetType() == Type.GetType("System.String") ||
                                                         val1.GetType() == Type.GetType("System.Boolean")))
                        if (!val1.Equals(val2))
                            return false;
                }
            }

            return true;
        }
    }
}
