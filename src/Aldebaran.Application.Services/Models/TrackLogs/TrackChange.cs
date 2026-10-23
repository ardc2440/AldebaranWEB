namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>Campo del registro con su valor anterior y nuevo.</summary>
    /// <param name="Field">Nombre técnico del campo.</param>
    /// <param name="Changed">El valor cambió respecto al movimiento anterior.</param>
    /// <param name="DisplayName">Nombre del campo para el usuario (el técnico si no está registrado).</param>
    /// <param name="IsTechnical">Llave interna sin valor para el usuario (se muestra solo a pedido).</param>
    public sealed record TrackChange(string Field, string? PreviousValue, string? NewValue, bool Changed, string? DisplayName = null, bool IsTechnical = false)
    {
        public string Name => DisplayName ?? Field;
    }
}
