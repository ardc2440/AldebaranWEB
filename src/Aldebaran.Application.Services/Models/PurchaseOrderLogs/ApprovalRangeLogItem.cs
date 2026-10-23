namespace Aldebaran.Application.Services.Models.PurchaseOrderLogs
{
    /// <summary>Valores de una tolerancia de ajustes de OC.</summary>
    public sealed record ApprovalRangeValues(int QuantityFrom, int QuantityTo, decimal Percent, bool IsActive)
    {
        public string Range => $"{QuantityFrom} - {QuantityTo}";
    }

    /// <summary>Cambio de una tolerancia de ajustes de OC: valor anterior y nuevo.</summary>
    public sealed record ApprovalRangeLogItem(int LogId, DateTime EventDate, ApprovalRangeValues Previous, ApprovalRangeValues Current,
        string ChangeReason, string EmployeeName);
}
