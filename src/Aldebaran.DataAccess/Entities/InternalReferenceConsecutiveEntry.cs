namespace Aldebaran.DataAccess.Entities
{
    /// <summary>Fila de los SP de consecutivos de referencia interna (CÓDIGO-NNN). Keyless.</summary>
    public class InternalReferenceConsecutiveEntry
    {
        public string Code { get; set; } = string.Empty;
        public int Consecutive { get; set; }
        public string InternalReference { get; set; } = string.Empty;
    }
}
