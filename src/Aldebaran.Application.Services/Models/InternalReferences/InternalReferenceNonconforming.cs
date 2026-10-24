namespace Aldebaran.Application.Services.Models.InternalReferences
{
    /// <summary>Referencia de artículo con el código que no cuenta como consecutivo (ej. MU-25X) y el motivo.</summary>
    public sealed record InternalReferenceNonconforming(string InternalReference, string ItemName, string Reason);
}
