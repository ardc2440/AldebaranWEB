using Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.Componentes;
using Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Inventory_Adjustments
{
    public partial class InventoryAdjustmentsReport
    {
        #region Injections
        [Inject]
        protected ILogger<InventoryAdjustmentsReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Variables
        protected InventoryAdjustmentsFilter Filter;
        protected InventoryAdjustmentsViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new InventoryAdjustmentsViewModel()
            {
                Adjustments = new List<InventoryAdjustmentsViewModel.Adjustment>
                {
                    new InventoryAdjustmentsViewModel.Adjustment
                    {
                        AdjustmentId=15738,
                        AdjustmentDate = DateTime.Now.AddDays(-10),
                        CreationDate = DateTime.Now,
                        AdjustmentType="Entrada",
                        AdjustmentReason="Devolucion",
                        Employee="Javier Linares",
                        Notes="Cliente con pedido #98859",
                        Warehouses = new List<InventoryAdjustmentsViewModel.Warehouse>
                        {
                            new InventoryAdjustmentsViewModel.Warehouse
                            {
                                WarehouseId = 1,
                                WarehouseName = "Local",
                                Lines = new List<InventoryAdjustmentsViewModel.Line>
                                {
                                    new InventoryAdjustmentsViewModel.Line
                                    {
                                        LineCode="001",
                                        LineName = "Importados",
                                        Items = new List<InventoryAdjustmentsViewModel.Item>
                                        {
                                            new InventoryAdjustmentsViewModel.Item
                                            {
                                                InternalReference ="00110-01",
                                                ItemName="HOT PORTAMINA",
                                                References = new List<InventoryAdjustmentsViewModel.Reference>
                                                {
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        AvailableAmount=200
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        AvailableAmount=180
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00003",
                                                        ReferenceName="Verde",
                                                        AvailableAmount=180
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00004",
                                                        ReferenceName="Amarillo",
                                                        AvailableAmount=180
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00005",
                                                        ReferenceName="Negro",
                                                        AvailableAmount=180
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00006",
                                                        ReferenceName="Naranja",
                                                        AvailableAmount=180
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new InventoryAdjustmentsViewModel.Warehouse
                            {
                                WarehouseId = 2,
                                WarehouseName = "Zona Franca",
                                Lines = new List<InventoryAdjustmentsViewModel.Line>
                                {
                                    new InventoryAdjustmentsViewModel.Line
                                    {
                                        LineCode="001",
                                        LineName = "Importados",
                                        Items = new List<InventoryAdjustmentsViewModel.Item>
                                        {
                                            new InventoryAdjustmentsViewModel.Item
                                            {
                                                InternalReference ="AA973-1-01",
                                                ItemName="LACE",
                                                References = new List<InventoryAdjustmentsViewModel.Reference>
                                                {
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        AvailableAmount=-20
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        AvailableAmount=156783
                                                    }
                                                }
                                            },
                                            new InventoryAdjustmentsViewModel.Item
                                            {
                                                InternalReference ="AA8986-01",
                                                ItemName="MERCURIO",
                                                References = new List<InventoryAdjustmentsViewModel.Reference>
                                                {
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        AvailableAmount=-20
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        AvailableAmount=156783
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new InventoryAdjustmentsViewModel.Adjustment
                    {
                        AdjustmentId=145812,
                        AdjustmentDate = DateTime.Now.AddDays(-8),
                        CreationDate = DateTime.Now,
                        AdjustmentType="Salida",
                        AdjustmentReason="Devolucion",
                        Employee="Andres Diaz",
                        Notes=null,
                        Warehouses = new List<InventoryAdjustmentsViewModel.Warehouse>
                        {
                            new InventoryAdjustmentsViewModel.Warehouse
                            {
                                WarehouseId = 2,
                                WarehouseName = "Zona Franca",
                                Lines = new List<InventoryAdjustmentsViewModel.Line>
                                {
                                    new InventoryAdjustmentsViewModel.Line
                                    {
                                        LineCode="001",
                                        LineName = "Importados",
                                        Items = new List<InventoryAdjustmentsViewModel.Item>
                                        {
                                            new InventoryAdjustmentsViewModel.Item
                                            {
                                                InternalReference ="AA973-1-01",
                                                ItemName="LACE",
                                                References = new List<InventoryAdjustmentsViewModel.Reference>
                                                {
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        AvailableAmount=-20
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        AvailableAmount=156783
                                                    }
                                                }
                                            },
                                            new InventoryAdjustmentsViewModel.Item
                                            {
                                                InternalReference ="AA8986-01",
                                                ItemName="MERCURIO",
                                                References = new List<InventoryAdjustmentsViewModel.Reference>
                                                {
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        AvailableAmount=-20
                                                    },
                                                    new InventoryAdjustmentsViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        AvailableAmount=156783
                                                    }
                                                }
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
            var result = await DialogService.OpenAsync<InventoryAdjustmentsReportFilter>("Filtrar reporte de ajustes de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InventoryAdjustmentsFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-adjustments-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Ajustes de inventario.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
