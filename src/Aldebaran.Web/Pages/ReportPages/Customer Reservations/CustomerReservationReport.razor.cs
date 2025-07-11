﻿using Aldebaran.Application.Services.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Web.Pages.ReportPages.Customer_Reservations.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using static Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel.CustomerReservationViewModel;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Reservations
{
    public partial class CustomerReservationReport
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerReservationReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected ICustomerReservationReportService CustomerReservationReportService { get; set; }
        #endregion

        #region Variables
        protected CustomerReservationFilter Filter;
        protected CustomerReservationViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        private IEnumerable<Application.Services.Models.Reports.CustomerReservationReport> DataReport { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Reset();
            }
        }
        #endregion

        #region Events
        async Task Reset()
        {
            Filter = null;
            ViewModel = null;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            await OpenFilters();
        }
        async Task RedrawReportAsync(string filter = "", CancellationToken ct = default)
        {
            try
            {
                IsLoadingData = true;
                DataReport = await CustomerReservationReportService.GetCustomerReservationReportDataAsync(filter, ct);

                ViewModel = new CustomerReservationViewModel
                {
                    Customers = await GetCustomersAsync()
                };
            }
            finally
            {
                IsLoadingData = false;
                StateHasChanged();
            }
        }

        async Task<string> SetReportFilterAsync(CustomerReservationFilter filter, CancellationToken ct = default)
        {
            var filterResult = string.Empty;

            if (filter.CreationDate.StartDate.HasValue)
                filterResult += $"@CreationDateFrom = '{(DateTime)filter.CreationDate.StartDate:yyyyMMdd}', @CreationDateTo = '{(DateTime)filter.CreationDate.EndDate:yyyyMMdd}'";

            if (filter.ReservationDate.StartDate.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReservationDateFrom = '{(DateTime)filter.ReservationDate.StartDate:yyyyMMdd}', @ReservationDateTo = '{(DateTime)filter.ReservationDate.EndDate:yyyyMMdd}'";

            if (!filter.ReservationNumber.IsNullOrEmpty())
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReservationNumber = '{filter.ReservationNumber}'";

            if (filter.StatusDocumentTypeId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@StatusDocumentTypeId = {filter.StatusDocumentTypeId}";

            if (filter.ItemReferences.Count > 0)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@ReferenceIds = '{String.Join(",", Filter.ItemReferences.Select(s => s.ReferenceId))}'";

            if (filter.CustomerId.HasValue)
                filterResult += (!filterResult.IsNullOrEmpty() ? ", " : "") + $"@CustomerId = {filter.CustomerId}";

            return filterResult;
        }
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<CustomerReservationReportFilter>("Filtrar reporte de reservas por cliente", parameters: new Dictionary<string, object> { { "Filter", (CustomerReservationFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerReservationFilter)result;

            await RedrawReportAsync(await SetReportFilterAsync(Filter));

            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-reservation-report-container");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Reservas por cliente.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "customer-reservation-report-container");
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

        async Task<List<Customer>> GetCustomersAsync(CancellationToken ct = default)
        {
            var customers = new List<Customer>();

            foreach (var customer in DataReport.Select(s => new { s.CustomerId, s.CustomerName, s.Phone, s.Fax })
                                        .DistinctBy(d => d.CustomerId)
                                        .OrderBy(o => o.CustomerName))
            {
                customers.Add(new Customer
                {
                    CustomerName = customer.CustomerName,
                    Fax = customer.Fax,
                    Phone = customer.Phone,
                    Reservations = await GetCustomerReservationsAsync(customer.CustomerId, ct)
                }); ;
            }

            return customers;
        }

        async Task<List<Reservation>> GetCustomerReservationsAsync(int customerId, CancellationToken ct = default)
        {
            var reservations = new List<Reservation>();

            foreach (var reservation in DataReport.Where(w => w.CustomerId == customerId).Select(s => new { s.ReservationId, s.ReservationNumber, s.CreationDate, s.ReservationDate, s.ExpirationDate, s.Status, s.Notes })
                                            .DistinctBy(d => d.ReservationId)
                                            .OrderBy(o => o.ReservationNumber))
            {
                reservations.Add(new Reservation
                {
                    ReservationNumber = reservation.ReservationNumber,
                    CreationDate = reservation.CreationDate,
                    ExpirationDate = reservation.ExpirationDate,
                    Notes = reservation.Notes,
                    ReservationDate = reservation.ReservationDate,
                    Status = reservation.Status,
                    Items = await GetReservationItemsAsync(reservation.ReservationId, ct)
                });
            }

            return reservations;
        }

        async Task<List<Item>> GetReservationItemsAsync(int reservationId, CancellationToken ct = default)
        {
            var items = new List<Item>();

            foreach (var item in DataReport.Where(w => w.ReservationId == reservationId).Select(s => new { s.ItemId, s.InternalReference, s.ItemName })
                                    .DistinctBy(d => d.ItemId)
                                    .OrderBy(o => o.ItemName))
            {
                items.Add(new Item
                {
                    ItemName = item.ItemName,
                    InternalReference = item.InternalReference,
                    References = await GetItemReferencesAsync(reservationId, item.ItemId, ct)
                });
            }

            return items;
        }

        async Task<List<Reference>> GetItemReferencesAsync(int reservationId, int itemId, CancellationToken ct = default)
        {
            var references = new List<Reference>();

            foreach (var reference in DataReport.Where(w => w.ReservationId == reservationId && w.ItemId == itemId)
                                        .Select(s => new { s.ReferenceCode, s.ReferenceName, s.Amount })
                                        .OrderBy(o => o.ReferenceName))
            {
                references.Add(new Reference
                {
                    ReferenceCode = reference.ReferenceCode,
                    ReferenceName = reference.ReferenceName,
                    Amount = reference.Amount
                });
            }

            return references;
        }

        #endregion
    }
}
