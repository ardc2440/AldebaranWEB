using Aldebaran.Web.Pages.ReportPages.Provider_References.Components;
using Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Stock;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Provider_References
{
    public partial class ProviderReferencesReport
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
        protected ProviderReferencesFilter Filter;
        protected ProviderReferencesViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new ProviderReferencesViewModel()
            {
                Providers = new List<ProviderReferencesViewModel.Provider> {
                    new ProviderReferencesViewModel.Provider
                    {
                        ProviderCode="JLING",
                        ProviderName= "JLING S.A.",
                        ContactPerson="Javier Linares",
                        Email="javierl@gmlsoftware.com",
                        Fax="651421123174",
                        Phone="3168849474",
                        ProviderAddress="Cra 83 13-50 Bogota, Colombia",
                        Lines = new List<ProviderReferencesViewModel.Line>
                        {
                            new ProviderReferencesViewModel.Line
                            {
                                LineCode="001",
                                LineName = "Importados",
                                Items = new List<ProviderReferencesViewModel.Item>
                                {
                                    new ProviderReferencesViewModel.Item
                                    {
                                        InternalReference ="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=200,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200,
                                                Warehouses = new List<ProviderReferencesViewModel.Warehouse>
                                                {
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=1,
                                                        WarehouseName="Local",
                                                        Amount=150
                                                    },
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=2,
                                                        WarehouseName="Zona Franca",
                                                        Amount=50
                                                    }
                                                }
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=70,
                                                Warehouses = new List<ProviderReferencesViewModel.Warehouse>
                                                {
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=1,
                                                        WarehouseName="Local",
                                                        Amount=70
                                                    }
                                                }
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00003",
                                                ReferenceName="Verde",
                                                ProviderReferenceName="Green",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00004",
                                                ReferenceName="Amarillo",
                                                ProviderReferenceName="Yellow",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00005",
                                                ReferenceName="Negro",
                                                ProviderReferenceName="Black",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00006",
                                                ReferenceName="Naranja",
                                                ProviderReferenceName="Orange",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    },
                                    new ProviderReferencesViewModel.Item
                                    {
                                         InternalReference ="AA973-1-01",
                                        ItemName="LACE",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    },
                                    new ProviderReferencesViewModel.Item
                                    {
                                        InternalReference ="AA8986-01",
                                        ItemName="MERCURIO",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ProviderReferencesViewModel.Provider
                    {
                        ProviderCode="JLING",
                        ProviderName= "JLING S.A.",
                        ContactPerson="Javier Linares",
                        Email="javierl@gmlsoftware.com",
                        Fax="651421123174",
                        Phone="3168849474",
                        ProviderAddress="Cra 83 13-50 Bogota, Colombia",
                        Lines = new List<ProviderReferencesViewModel.Line>
                        {
                            new ProviderReferencesViewModel.Line
                            {
                                LineCode="001",
                                LineName = "Importados",
                                Items = new List<ProviderReferencesViewModel.Item>
                                {
                                    new ProviderReferencesViewModel.Item
                                    {
                                        InternalReference ="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=200,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200,
                                                Warehouses = new List<ProviderReferencesViewModel.Warehouse>
                                                {
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=1,
                                                        WarehouseName="Local",
                                                        Amount=150
                                                    },
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=2,
                                                        WarehouseName="Zona Franca",
                                                        Amount=50
                                                    }
                                                }
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=70,
                                                Warehouses = new List<ProviderReferencesViewModel.Warehouse>
                                                {
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=1,
                                                        WarehouseName="Local",
                                                        Amount=70
                                                    }
                                                }
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00003",
                                                ReferenceName="Verde",
                                                ProviderReferenceName="Green",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00004",
                                                ReferenceName="Amarillo",
                                                ProviderReferenceName="Yellow",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00005",
                                                ReferenceName="Negro",
                                                ProviderReferenceName="Black",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00006",
                                                ReferenceName="Naranja",
                                                ProviderReferenceName="Orange",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    },
                                    new ProviderReferencesViewModel.Item
                                    {
                                         InternalReference ="AA973-1-01",
                                        ItemName="LACE",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    },
                                    new ProviderReferencesViewModel.Item
                                    {
                                        InternalReference ="AA8986-01",
                                        ItemName="MERCURIO",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ProviderReferencesViewModel.Provider
                    {
                        ProviderCode="JLING",
                        ProviderName= "JLING S.A.",
                        ContactPerson="Javier Linares",
                        Email="javierl@gmlsoftware.com",
                        Fax="651421123174",
                        Phone="3168849474",
                        ProviderAddress="Cra 83 13-50 Bogota, Colombia",
                        Lines = new List<ProviderReferencesViewModel.Line>
                        {
                            new ProviderReferencesViewModel.Line
                            {
                                LineCode="001",
                                LineName = "Importados",
                                Items = new List<ProviderReferencesViewModel.Item>
                                {
                                    new ProviderReferencesViewModel.Item
                                    {
                                        InternalReference ="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=200,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200,
                                                Warehouses = new List<ProviderReferencesViewModel.Warehouse>
                                                {
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=1,
                                                        WarehouseName="Local",
                                                        Amount=150
                                                    },
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=2,
                                                        WarehouseName="Zona Franca",
                                                        Amount=50
                                                    }
                                                }
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=70,
                                                Warehouses = new List<ProviderReferencesViewModel.Warehouse>
                                                {
                                                    new ProviderReferencesViewModel.Warehouse
                                                    {
                                                        WarehouseId=1,
                                                        WarehouseName="Local",
                                                        Amount=70
                                                    }
                                                }
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00003",
                                                ReferenceName="Verde",
                                                ProviderReferenceName="Green",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00004",
                                                ReferenceName="Amarillo",
                                                ProviderReferenceName="Yellow",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00005",
                                                ReferenceName="Negro",
                                                ProviderReferenceName="Black",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00006",
                                                ReferenceName="Naranja",
                                                ProviderReferenceName="Orange",
                                                AvailableAmount=180,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    },
                                    new ProviderReferencesViewModel.Item
                                    {
                                         InternalReference ="AA973-1-01",
                                        ItemName="LACE",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            }
                                        }
                                    },
                                    new ProviderReferencesViewModel.Item
                                    {
                                        InternalReference ="AA8986-01",
                                        ItemName="MERCURIO",
                                        References = new List<ProviderReferencesViewModel.Reference>
                                        {
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00001",
                                                ReferenceName="Blanco",
                                                ProviderReferenceName="White",
                                                AvailableAmount=-20,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
                                            },
                                            new ProviderReferencesViewModel.Reference
                                            {
                                                ReferenceCode="00002",
                                                ReferenceName="Rojo",
                                                ProviderReferenceName="Red",
                                                AvailableAmount=156783,
                                                ConfirmedAmount=80,
                                                ReservedAmount=200
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
            var result = await DialogService.OpenAsync<ProviderReferencesReportFilter>("Filtrar reporte de referencias del proveedor", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (ProviderReferencesFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "provider-references-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Referencias del proveedor.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
