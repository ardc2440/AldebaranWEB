using Aldebaran.Application.Services.Models.Reports;
using Entities = Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>Arma el árbol Compañía -> Línea -> Artículo -> Homologaciones (agrupación O(n), orden alfabético).</summary>
    internal static class CompetitorHomologationReportTreeBuilder
    {
        public static CompetitorHomologationReportResult Build(IEnumerable<Entities.CompetitorHomologationReport>? rows)
        {
            var data = rows?.ToList() ?? new List<Entities.CompetitorHomologationReport>();
            if (data.Count == 0)
                return CompetitorHomologationReportResult.Empty;

            var companies = data.GroupBy(r => r.CompetitorCompanyId)
                .Select(c => (First: c.First(), Rows: c))
                .OrderBy(c => c.First.CompanyName)
                .Select(c => new CompetitorHomologationReportCompany(c.First.CompetitorCompanyId, c.First.CompanyName, c.First.Website, BuildLines(c.Rows)))
                .ToList();
            return new CompetitorHomologationReportResult(companies);
        }

        private static IReadOnlyList<CompetitorHomologationReportLine> BuildLines(IEnumerable<Entities.CompetitorHomologationReport> rows) =>
            rows.GroupBy(r => r.LineId)
                .Select(l => (First: l.First(), Rows: l))
                .OrderBy(l => l.First.LineName)
                .Select(l => new CompetitorHomologationReportLine(l.First.LineId, l.First.LineName, BuildItems(l.Rows)))
                .ToList();

        private static IReadOnlyList<CompetitorHomologationReportItem> BuildItems(IEnumerable<Entities.CompetitorHomologationReport> rows) =>
            rows.GroupBy(r => r.ItemId)
                .Select(i => (First: i.First(), Rows: i))
                .OrderBy(i => i.First.ItemName)
                .Select(i => new CompetitorHomologationReportItem(i.First.ItemId, i.First.InternalReference, i.First.ItemName,
                    i.Rows.OrderBy(r => r.ReferenceId == null ? 0 : 1).ThenBy(r => r.ReferenceName).ThenBy(r => r.CompetitorReference)
                          .Select(ToEntry).ToList()))
                .ToList();

        private static CompetitorHomologationReportEntry ToEntry(Entities.CompetitorHomologationReport r) =>
            new(r.CompetitorHomologationId, r.ReferenceId, r.ReferenceCode, r.ReferenceName, r.CompetitorReference,
                r.CompetitorProductName, r.ProductUrl, r.CompetitorPrice, r.ImageFileName);
    }

    /// <summary>Filas planas para Excel, en el mismo orden del árbol.</summary>
    internal static class CompetitorHomologationReportExportRowBuilder
    {
        public static IReadOnlyList<CompetitorHomologationReportExportRow> Build(IEnumerable<Entities.CompetitorHomologationReport>? rows)
        {
            var tree = CompetitorHomologationReportTreeBuilder.Build(rows);
            return (from c in tree.Companies
                    from l in c.Lines
                    from i in l.Items
                    from h in i.Homologations
                    select new CompetitorHomologationReportExportRow
                    {
                        CompanyName = c.CompanyName,
                        Website = c.Website,
                        LineName = l.LineName,
                        InternalReference = i.InternalReference,
                        ItemName = i.ItemName,
                        PromosReference = h.PromosReference,
                        CompetitorReference = h.CompetitorReference,
                        CompetitorProductName = h.CompetitorProductName,
                        CompetitorPrice = h.CompetitorPrice,
                        ProductUrl = h.ProductUrl ?? string.Empty
                    }).ToList();
        }
    }
}
