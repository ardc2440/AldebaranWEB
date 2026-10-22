namespace Aldebaran.DataAccess.Core.Atributes
{
    /// <summary>
    /// Marca una propiedad cuyo valor no debe quedar legible en log.track (contrasenas, secretos).
    /// Sin claves: se oculta todo el valor. Con claves: el valor es un JSON y solo se ocultan
    /// las propiedades cuyo nombre contenga alguna de las claves (sin distinguir mayusculas).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TrackMaskedAttribute : Attribute
    {
        public string[] JsonKeys { get; }

        public TrackMaskedAttribute(params string[] jsonKeys)
        {
            JsonKeys = jsonKeys ?? Array.Empty<string>();
        }
    }
}
