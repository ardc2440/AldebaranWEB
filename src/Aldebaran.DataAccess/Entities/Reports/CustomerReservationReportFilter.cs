namespace Aldebaran.DataAccess.Entities.Reports
{
    /// <summary>Parámetros de SP_GET_CUSTOMER_RESERVATION_REPORT (reporte de reservas por cliente). Null o vacío = sin filtro.</summary>
    public class CustomerReservationReportFilter
    {
        public string? ReservationNumber { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public DateTime? ReservationDateFrom { get; set; }
        public DateTime? ReservationDateTo { get; set; }
        public short? StatusDocumentTypeId { get; set; }
        public int? CustomerId { get; set; }
        public string? ReferenceIds { get; set; } // ids separados por coma; vacío = todas
    }
}
