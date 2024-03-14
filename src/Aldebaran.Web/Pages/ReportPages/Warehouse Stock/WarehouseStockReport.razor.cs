using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.Components;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Stock
{
    public partial class WarehouseStockReport
    {
        #region Injections
        [Inject]
        protected ILogger<WarehouseStockReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Variables
        protected WarehouseStockFilter Filter;
        protected WarehouseStockViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new WarehouseStockViewModel()
            {
                Warehouses = new List<WarehouseStockViewModel.Warehouse>
                {
                    new WarehouseStockViewModel.Warehouse
                    {
                        WarehouseId = 1,
                        WarehouseName = "Local",
                        Lines = new List<WarehouseStockViewModel.Line>
                        {
                            new WarehouseStockViewModel.Line
                            {
                                LineCode="001",
                                LineName = "Importados",
                                Items = new List<WarehouseStockViewModel.Item>
                                {
                                    new WarehouseStockViewModel.Item
                                    {
                                        InternalReference ="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        References = new List<WarehouseStockViewModel.Reference>
                                        {
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=200
                                            },
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=180
                                            },
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00003",
                                                ReferenceName="Verde",
                                                ProviderReferenceName="Green",
                                                AvailableAmount=180
                                            },
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00004",
                                                ReferenceName="Amarillo",
                                                ProviderReferenceName="Yellow",
                                                AvailableAmount=180
                                            },
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00005",
                                                ReferenceName="Negro",
                                                ProviderReferenceName="Black",
                                                AvailableAmount=180
                                            },
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00006",
                                                ReferenceName="Naranja",
                                                ProviderReferenceName="Orange",
                                                AvailableAmount=180
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new WarehouseStockViewModel.Warehouse
                    {
                        WarehouseId = 2,
                        WarehouseName = "Zona Franca",
                        Lines = new List<WarehouseStockViewModel.Line>
                        {
                            new WarehouseStockViewModel.Line
                            {
                                LineCode="001",
                                LineName = "Importados",
                                Items = new List<WarehouseStockViewModel.Item>
                                {
                                    new WarehouseStockViewModel.Item
                                    {
                                        InternalReference ="AA973-1-01",
                                        ItemName="LACE",
                                        References = new List<WarehouseStockViewModel.Reference>
                                        {
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20
                                            },
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783
                                            }
                                        }
                                    },
                                    new WarehouseStockViewModel.Item
                                    {
                                        InternalReference ="AA8986-01",
                                        ItemName="MERCURIO",
                                        References = new List<WarehouseStockViewModel.Reference>
                                        {
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20
                                            },
                                            new WarehouseStockViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783
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
            var result = await DialogService.OpenAsync<WarehouseStockReportFilter>("Filtrar reporte de existencias de artículos", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (WarehouseStockFilter)result;
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
        async Task Download()
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "warehouse-stock-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Existencias de artículos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
