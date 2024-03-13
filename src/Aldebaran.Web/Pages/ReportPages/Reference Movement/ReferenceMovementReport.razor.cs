using Aldebaran.Web.Pages.ReportPages.Reference_Movement.Components;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

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
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Variables
        protected ReferenceMovementFilter Filter;
        protected ReferenceMovementViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new ReferenceMovementViewModel
            {
                Lines = new List<ReferenceMovementLine>
                {
                    new ReferenceMovementLine
                    {
                        LineCode="001",
                        LineName="Boligrafos",
                        Items= new List<ReferenceMovementItem> {
                            new ReferenceMovementItem {
                                InternalReference="ADVANT 2",
                                ItemName="Mina ADVANT 2-1",
                                References = new List<ReferenceMovementReference>
                                {
                                    new ReferenceMovementReference
                                    {
                                        ReferenceCode="001",
                                        ReferenceName="Rojo",
                                        RequestedQuantity=1,
                                        ReservedQuantity=100,
                                        Warehouses = new List<ReferenceMovementWarehouse>
                                        {
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=1,
                                                WarehouseName="Local",
                                                Amount=30000
                                            },
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=2,
                                                WarehouseName="Zona Franca",
                                                Amount=20350
                                            }
                                        }
                                    },
                                    new ReferenceMovementReference
                                    {
                                        ReferenceCode="002",
                                        ReferenceName="Azul",
                                        RequestedQuantity=45,
                                        ReservedQuantity=235,
                                        Warehouses = new List<ReferenceMovementWarehouse>
                                        {
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=1,
                                                WarehouseName="Local",
                                                Amount=18500
                                            },
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=2,
                                                WarehouseName="Zona Franca",
                                                Amount=0
                                            }
                                        }
                                    },
                                    new ReferenceMovementReference
                                    {
                                        ReferenceCode="003",
                                        ReferenceName="Amarillo",
                                        RequestedQuantity=27,
                                        ReservedQuantity=185,
                                        Warehouses = new List<ReferenceMovementWarehouse>
                                        {
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=1,
                                                WarehouseName="Local",
                                                Amount=3871
                                            }
                                        }
                                    }
                                }
                            },
                            new ReferenceMovementItem
                            {
                                InternalReference="ADVANT 3",
                                ItemName="Mina ADVANT 3-1",
                                References = new List<ReferenceMovementReference>
                                {
                                    new ReferenceMovementReference
                                    {
                                        ReferenceCode="004",
                                        ReferenceName="Plateado",
                                        RequestedQuantity=86,
                                        ReservedQuantity=79,
                                        Warehouses = new List<ReferenceMovementWarehouse>
                                        {
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=1,
                                                WarehouseName="Zona Franca",
                                                Amount=578
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ReferenceMovementLine
                    {
                        LineCode="002",
                        LineName="Mugs",
                        Items= new List<ReferenceMovementItem> {
                             new ReferenceMovementItem {
                                InternalReference="MUG",
                                ItemName="Mug con kit",
                                References = new List<ReferenceMovementReference>
                                {
                                    new ReferenceMovementReference
                                    {
                                        ReferenceCode="007",
                                        ReferenceName="Negro",
                                        RequestedQuantity=1,
                                        ReservedQuantity=100,
                                        Warehouses = new List<ReferenceMovementWarehouse>
                                        {
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=1,
                                                WarehouseName="Local",
                                                Amount=4500
                                            },
                                            new ReferenceMovementWarehouse
                                            {
                                                WarehouseId=2,
                                                WarehouseName="Zona Franca",
                                                Amount=20350
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<ReferenceMovementReportFilter>("Filtrar reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ReferenceMovementFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-movement-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Movimientos de artículos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
