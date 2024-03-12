using Aldebaran.Web.Pages.ReportPages.Inventory.Components;
using Aldebaran.Web.Pages.ReportPages.Inventory.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Inventory
{
    public partial class InventoryReport
    {
        #region Injections
        [Inject]
        protected ILogger<InventoryReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Variables
        protected InventoryFilter Filter;
        protected InventoryViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new InventoryViewModel
            {
                Lines = new List<InventoryLine>
                {
                    new InventoryLine
                    {
                        LineName = "Importados",
                        Items = new List<InventoryItem>{
                            new InventoryItem {
                                ItemName = "LAPIZ NEON",
                                InternalReference = "22-01",
                                InventoryReferences = new List<InventoryReference>
                                {
                                    new InventoryReference
                                    {
                                        ReferenceName="Amarillo",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Verde",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Naranja",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    }
                                },
                                PurchaseOrders = new List<InventoryPurchaseOrder>
                                {
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    },
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    }
                                }
                            },
                            new InventoryItem {
                                ItemName = "LAPIZ NEON",
                                InternalReference = "22-01",
                                InventoryReferences = new List<InventoryReference>
                                {
                                    new InventoryReference
                                    {
                                        ReferenceName="Amarillo",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Verde",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Naranja",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    }
                                },
                                PurchaseOrders = new List<InventoryPurchaseOrder>
                                {
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    },
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    }
                                }
                            },
                            new InventoryItem {
                                ItemName = "LAPIZ NEON",
                                InternalReference = "22-01",
                                InventoryReferences = new List<InventoryReference>
                                {
                                    new InventoryReference
                                    {
                                        ReferenceName="Amarillo",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Verde",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Naranja",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    }
                                },
                                PurchaseOrders = new List<InventoryPurchaseOrder>
                                {
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    },
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new InventoryLine
                    {
                        LineName = "Locales",
                        Items = new List<InventoryItem>{
                            new InventoryItem {
                                ItemName = "LAPIZ NEON",
                                InternalReference = "22-01",
                                InventoryReferences = new List<InventoryReference>
                                {
                                    new InventoryReference
                                    {
                                        ReferenceName="Amarillo",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Verde",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Naranja",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    }
                                },
                                PurchaseOrders = new List<InventoryPurchaseOrder>
                                {
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    },
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    }
                                }
                            },
                            new InventoryItem {
                                ItemName = "LAPIZ NEON",
                                InternalReference = "22-01",
                                InventoryReferences = new List<InventoryReference>
                                {
                                    new InventoryReference
                                    {
                                        ReferenceName="Amarillo",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Verde",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Naranja",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    }
                                },
                                PurchaseOrders = new List<InventoryPurchaseOrder>
                                {
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    },
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    }
                                }
                            },
                            new InventoryItem {
                                ItemName = "LAPIZ NEON",
                                InternalReference = "22-01",
                                InventoryReferences = new List<InventoryReference>
                                {
                                    new InventoryReference
                                    {
                                        ReferenceName="Amarillo",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Verde",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    },
                                    new InventoryReference
                                    {
                                        ReferenceName="Naranja",
                                        AvailableAmount=24211,
                                        Total=70000,
                                    }
                                },
                                PurchaseOrders = new List<InventoryPurchaseOrder>
                                {
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
                                        }
                                    },
                                    new InventoryPurchaseOrder
                                    {
                                        Date = DateTime.Now,
                                        Warehouse = "Local",
                                        Total = 30000,
                                        Activity = new InventoryActivity
                                        {
                                            Date= DateTime.Now,
                                            Description = "Orden en produccion, (PENDIENTE CONFIRMACION) fecha estimada de salida del pierto Jul 10."
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
            var result = await DialogService.OpenAsync<InventoryReportFilter>("Filtrar reporte de inventario", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (InventoryFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "inventory-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Inventario.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
