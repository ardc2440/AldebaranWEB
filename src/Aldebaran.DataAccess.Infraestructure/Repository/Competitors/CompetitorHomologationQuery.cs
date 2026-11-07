namespace Aldebaran.DataAccess.Infraestructure.Repository.Competitors
{
    /// <summary>Filtros del listado de homologaciones (pantalla de administración).</summary>
    public sealed class CompetitorHomologationQuery
    {
        public int? CompetitorCompanyId { get; init; }
        public int? ItemId { get; init; }
        public bool IncludeInactive { get; init; }
        /// <summary>Busca en referencia y nombre del competidor, y en referencia interna / nombre del artículo.</summary>
        public string? SearchKey { get; init; }
    }
}
