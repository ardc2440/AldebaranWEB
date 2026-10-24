namespace Aldebaran.Application.Services.Models.InternalReferences
{
    /// <summary>Consecutivo de referencia interna pre-asignado (ej. MU-026).</summary>
    public sealed record InternalReferenceConsecutive(string Code, int Consecutive, string InternalReference);
}
