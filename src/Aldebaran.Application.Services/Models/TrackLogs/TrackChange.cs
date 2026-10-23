namespace Aldebaran.Application.Services.Models.TrackLogs
{
    /// <summary>Campo del registro con su valor anterior y nuevo.</summary>
    /// <param name="Changed">El valor cambió respecto al movimiento anterior.</param>
    public sealed record TrackChange(string Field, string? PreviousValue, string? NewValue, bool Changed);
}
