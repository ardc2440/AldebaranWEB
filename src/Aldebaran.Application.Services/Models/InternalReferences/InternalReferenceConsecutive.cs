namespace Aldebaran.Application.Services.Models.InternalReferences
{
    /// <summary>Consecutivo de referencia interna disponible (ej. MU-026). IsNew = creado en la última generación.</summary>
    public sealed record InternalReferenceConsecutive(string Code, int Consecutive, string InternalReference, bool IsNew);
}
