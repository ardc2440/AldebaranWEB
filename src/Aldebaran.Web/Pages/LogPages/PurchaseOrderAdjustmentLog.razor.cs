using Aldebaran.Application.Services.Models.AlarmLogs;
using Aldebaran.Application.Services.Models.PurchaseOrderLogs;
using Aldebaran.Application.Services.PurchaseOrderLogs;
using Microsoft.AspNetCore.Components;

namespace Aldebaran.Web.Pages.LogPages
{
    public partial class PurchaseOrderAdjustmentLog
    {
        [Inject] protected IPurchaseOrderLogService PurchaseOrderLogService { get; set; }

        protected override Task<PagedResult<PurchaseOrderAdjustmentRequestItem>> FetchAsync(DateTime from, DateTime to, string searchKey, int skip, int take) =>
            PurchaseOrderLogService.GetAdjustmentRequestsAsync(from, to, searchKey, skip, take);
    }
}
