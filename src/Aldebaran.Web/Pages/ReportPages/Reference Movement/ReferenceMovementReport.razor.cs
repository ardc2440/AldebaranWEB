﻿using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.Components;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement
{
    public partial class ReferenceMovementReport
    {
        #region Injections
        [Inject]
        protected ILogger<ReferenceMovementReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IReferenceMovementReportService ReferenceMovementReportService { get; set; }
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int? ReferenceId { get; set; }
        [Parameter]
        public bool IsModal { get; set; } = false;
        #endregion
        #region Variables
        protected ReferenceMovementFilter Filter;
        protected ReferenceMovementViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.ReferenceMovementReport> DataReport { get; set; }
        #endregion

        #region Overrides

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Reset();
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (ReferenceId != null)
            {
                await SetFiltersByParameters();
            }
        }

        #endregion

        #region Events
        async Task Reset()
        {
            await SetFiltersByParameters();
            ViewModel = null;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            await OpenFilters();
        }
        async Task SetFiltersByParameters()
        {
            if (ReferenceId != null)
            {
                var reference = await ItemReferenceService.FindAsync(ReferenceId.Value);
                Filter = new ReferenceMovementFilter
                {
                    ItemReferences = new List<Application.Services.Models.ItemReference> { reference },
                    LockReferenceSelection = true,
                    AllMovementCheckVisible = true
                };
            }
            else
            {
                Filter = null;
            }
        }
        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;

                DataReport = await ReferenceMovementReportService.GetReferenceMovementReportDataAsync(filter, ct);

                ViewModel = new ReferenceMovementViewModel
                {
                    Lines = (await GetReporLinesAsync(ct)).ToList()
                };
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        async Task<string> SetReportFilterAsync(ReferenceMovementFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (filter.MovementDate.StartDate.HasValue)
                filterResult += $"@InitialMovementDate = '{(DateTime)filter.MovementDate.StartDate:yyyyMMdd}', @FinalMovementDate = '{(DateTime)filter.MovementDate.EndDate:yyyyMMdd}'";

            if (filter.ItemReferences.Count > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'";

            if (filter.ShowInactiveItems)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + "@InactiveItems = 1";

            if (filter.ShowInactiveReferences)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + "@InactiveReferences = 1";

            return filterResult;
        }

        async Task OpenFilters()
        {
            var dialogOptions = new DialogOptions
            {
                Width = "800px"
            };
            if (Filter != null && Filter.LockReferenceSelection)
            {
                dialogOptions.CloseDialogOnEsc = false;
            }
            var result = await DialogService.OpenAsync<ReferenceMovementReportFilter>("Filtrar reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: dialogOptions);
            if (result == null)
            {
                if (IsModal && !Filter.AllRequiredFieldsCompleted)
                {
                    DialogService.Close(null);
                }
                return;
            }
            Filter = (ReferenceMovementFilter)result;

            await RedrawReportAsync(await SetReportFilterAsync(Filter));
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            StateHasChanged();
        }

        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                await Reset();
            }
        }
        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-movement-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Movimientos de artículos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "inventory-movement-report-container");
            }
            IsBusy = false;
        }

        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        async Task ToggleReadMorePage()
        {
            await JSRuntime.InvokeVoidAsync("readMoreTogglePage", "toggleLinkPage");
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion

        #region Fill Data Report

        protected async Task<IEnumerable<ReferenceMovementViewModel.Line>> GetReporLinesAsync(CancellationToken ct = default)
        {
            var lines = new List<ReferenceMovementViewModel.Line>();

            foreach (var line in DataReport.Select(s => new { s.LineId, s.LineName, s.LineCode })
                                    .DistinctBy(d => d.LineId).OrderBy(o => o.LineName))
            {
                lines.Add(new ReferenceMovementViewModel.Line
                {
                    LineName = line.LineName,
                    LineCode = line.LineCode,
                    Items = (await GetReportItemsByLineIdAsync(line.LineId, ct)).ToList()
                });
            }

            return lines;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Item>> GetReportItemsByLineIdAsync(short lineId, CancellationToken ct = default)
        {
            var items = new List<ReferenceMovementViewModel.Item>();

            foreach (var item in DataReport.Where(w => w.LineId == lineId).Select(s => new { s.ItemId, s.InternalReference, s.ItemName })
                                    .DistinctBy(d => d.ItemId).OrderBy(o => o.ItemName))
            {
                items.Add(new ReferenceMovementViewModel.Item
                {
                    InternalReference = item.InternalReference,
                    ItemName = item.ItemName,
                    References = (await GetReferencesByItemIdAsync(item.ItemId, ct)).ToList()
                });
            }

            return items;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Reference>> GetReferencesByItemIdAsync(int itemId, CancellationToken ct = default)
        {
            var reportReferences = new List<ReferenceMovementViewModel.Reference>();

            foreach (var reference in DataReport.Where(w => w.ItemId == itemId).Select(s => new { s.ReferenceId, s.ReferenceName, s.RequestedQuantity, s.ReservedQuantity, s.ReferenceCode })
                                        .DistinctBy(d => d.ReferenceId).OrderBy(o => o.ReferenceName))
            {
                reportReferences.Add(new ReferenceMovementViewModel.Reference
                {
                    ReferenceName = reference.ReferenceName,
                    RequestedQuantity = reference.RequestedQuantity,
                    ReservedQuantity = reference.ReservedQuantity,
                    ReferenceCode = reference.ReferenceCode,
                    Warehouses = (await GetWarehousesByReferenceIdAsync(reference.ReferenceId, ct)).ToList(),
                    Movements = (await GetMovementsByReferenceIdAsync(reference.ReferenceId, ct)).ToList(),
                }); ;
            }

            return reportReferences;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Warehouse>> GetWarehousesByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var warehouses = new List<ReferenceMovementViewModel.Warehouse>();

            foreach (var warehouse in DataReport.Where(w => w.ReferenceId == referenceId)
                                        .Select(s => new { s.WarehouseId, s.Amount, s.WarehouseName })
                                        .DistinctBy(d => d.WarehouseId).OrderBy(o => o.WarehouseName))
            {
                warehouses.Add(new ReferenceMovementViewModel.Warehouse
                {
                    WarehouseId = warehouse.WarehouseId,
                    Amount = warehouse.Amount,
                    WarehouseName = warehouse.WarehouseName,
                });

            }

            return warehouses;

        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.Movements>> GetMovementsByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var movements = new List<ReferenceMovementViewModel.Movements>();

            foreach (var movement in DataReport.Where(w => w.ReferenceId == referenceId && w.TitleId != null)
                                        .Select(s => new { s.TitleId, s.Title })
                                        .DistinctBy(d => d.TitleId).OrderBy(o => o.TitleId))
            {
                movements.Add(new ReferenceMovementViewModel.Movements
                {
                    Title = movement.Title,
                    Details = (await GetMovementDetailsByTitleIdAsync(referenceId, (short)movement.TitleId, ct)).ToList()
                });
            }

            return movements;
        }

        protected async Task<IEnumerable<ReferenceMovementViewModel.MovementDetail>> GetMovementDetailsByTitleIdAsync(int referenceId, short titleId, CancellationToken ct = default)
        {
            var movementDetails = new List<ReferenceMovementViewModel.MovementDetail>();

            foreach (var detail in DataReport.Where(w => w.ReferenceId == referenceId && w.TitleId == titleId)
                                        .Select(s => new { s.Code, s.Date, s.Owner, s.MovementAmount, s.Status, s.Operator })
                                        .DistinctBy(d => d.Code)
                                        .OrderBy(o => o.Date)
                                        .OrderBy(o => o.Code))
            {
                movementDetails.Add(new ReferenceMovementViewModel.MovementDetail
                {
                    Code = detail.Code,
                    Date = (DateTime)detail.Date,
                    Owner = detail.Owner,
                    Amount = (int)detail.MovementAmount,
                    Status = detail.Status,
                    Operator = (short)detail.Operator 
                });
            }

            return movementDetails;
        }

        #endregion

    }
}
