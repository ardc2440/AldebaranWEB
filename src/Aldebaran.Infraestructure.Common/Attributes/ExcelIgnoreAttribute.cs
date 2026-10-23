namespace Aldebaran.Infrastructure.Common.Attributes
{
    /// <summary>La propiedad no se exporta como columna en los archivos generados.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExcelIgnoreAttribute : Attribute
    {
    }
}
