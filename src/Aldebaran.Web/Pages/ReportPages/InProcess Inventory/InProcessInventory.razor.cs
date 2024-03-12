using Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.Components;
using Aldebaran.Web.Pages.ReportPages.InProcess_Inventory.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.InProcess_Inventory
{
    public partial class InProcessInventory
    {
        #region Injections
        [Inject]
        protected ILogger<InProcessInventory> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Variables
        protected InProcessInventoryFilter Filter;
        protected InProcessInventoryViewModel ViewModel;
        List<InProcessWarehouse> UniqueWarehouses = new List<InProcessWarehouse>();
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new InProcessInventoryViewModel
            {
                Items = new List<InProcessItem>
                {
                    new InProcessItem
                    {
                        InternalReference ="001",
                        ItemName="ADVANT 2-1",
                        References = new List<InProcessReference>
                        {
                            new InProcessReference
                            {
                                ReferenceName="Azul",
                                InProcessAmount=24,
                                Warehouses = new List<InProcessWarehouse>
                                {
                                    new InProcessWarehouse { WarehouseId=1, WarehouseName="Local", Amount=10000 },
                                    new InProcessWarehouse { WarehouseId=2, WarehouseName="Zona franca", Amount=10000 },
                                }
                            },
                            new InProcessReference
                            {
                                ReferenceName="Rojo metalico",
                                InProcessAmount=98,
                                Warehouses = new List<InProcessWarehouse>
                                {
                                    new InProcessWarehouse { WarehouseId=1, WarehouseName="Local", Amount=15000 },
                                }
                            }
                        }
                    },
                    new InProcessItem
                    {
                        InternalReference ="003",
                        ItemName="ARIA",
                        References = new List<InProcessReference>
                        {
                            new InProcessReference
                            {
                                ReferenceName="Amarillo",
                                InProcessAmount=24,
                                Warehouses = new List<InProcessWarehouse>
                                {
                                    new InProcessWarehouse { WarehouseId=2, WarehouseName="Zona franca", Amount=10000 },
                                }
                            },
                            new InProcessReference
                            {
                                ReferenceName="Negro",
                                InProcessAmount=98,
                                Warehouses = new List<InProcessWarehouse>
                                {
                                    new InProcessWarehouse { WarehouseId=1, WarehouseName="Local", Amount=15000 },
                                }
                            }
                        }
                    }
                }
            };
            UniqueWarehouses = ViewModel.Items
                .SelectMany(item => item.References.SelectMany(reference => reference.Warehouses))
                .DistinctBy(w => w.WarehouseId)
                .ToList();
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<InProcessInventoryReportFilter>("Filtrar reporte de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InProcessInventoryFilter)result;
            //Todo: Aplicar filtro de refenrecias al ViewModel
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
        }
        async Task RemoveFilters()
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar los filtros establecidos?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                Filter = null;
                //Todo: Remover filtro de refenrecias al ViewModel
                await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink", false);
            }
        }
        async Task Download(MouseEventArgs args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inprocess-inventory-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Inventario en proceso.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
