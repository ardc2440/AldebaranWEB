using Aldebaran.Application.Services;
using Aldebaran.Web.Pages.ReportPages.Customer_Reservations.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Reservations.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

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
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }
        #endregion

        #region Variables
        protected CustomerReservationFilter Filter;
        protected CustomerReservationViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new CustomerReservationViewModel
            {
                Customers = new List<CustomerReservationViewModel.Customer> {
                    new CustomerReservationViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Reservations = new List<CustomerReservationViewModel.Reservation>
                        {
                            new CustomerReservationViewModel.Reservation
                            {
                                CreationDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(30),
                                ReservationDate = DateTime.Now.AddDays(1),
                                ReservationNumber = "0000018345",
                                Status = "Vencida",
                                Notes = "Atendido por Viviana Mora",
                                Items = new List<CustomerReservationViewModel.Item>
                                {
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="032",
                                                ReferenceName="AZUL MARINO",
                                                Amount=358,
                                                Price=0
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="027",
                                                ReferenceName="Negro",
                                                Amount=25,
                                                Price=15024581
                                            }
                                        }
                                    },
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="VA-316",
                                        ItemName="Trolley Morral Backpack",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode=null,
                                                ReferenceName="Blanco",
                                                Amount=2342,
                                                Price=0
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode=string.Empty,
                                                ReferenceName="Silver",
                                                Amount=0,
                                                Price=0
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerReservationViewModel.Reservation
                            {
                                CreationDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(30),
                                ReservationDate = DateTime.Now.AddDays(1),
                                ReservationNumber = "0000018788",
                                Status = "Anulada",
                                Items = new List<CustomerReservationViewModel.Item>
                                {
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="OF-97",
                                        ItemName="Miniset de escritorio",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="877",
                                                ReferenceName="Blanco transparente",
                                                Amount=87,
                                                Price=789445647
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="027",
                                                ReferenceName="Negro",
                                                Amount=25,
                                                Price=15024581
                                            }
                                        }
                                    },
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="PM MU-63",
                                        ItemName="Botilito de aluminio 400ml",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="877",
                                                ReferenceName="Blanco transparente",
                                                Amount=87,
                                                Price=789445647
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new CustomerReservationViewModel.Customer {
                        CustomerName = "Andres Diaz",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Reservations = new List<CustomerReservationViewModel.Reservation>
                        {
                            new CustomerReservationViewModel.Reservation
                            {
                                CreationDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(30),
                                ReservationDate = DateTime.Now.AddDays(1),
                                ReservationNumber = "0000018345",
                                Status = "Vencida",
                                Items = new List<CustomerReservationViewModel.Item>
                                {
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="032",
                                                ReferenceName="AZUL MARINO",
                                                Amount=358,
                                                Price=0
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="027",
                                                ReferenceName="Negro",
                                                Amount=25,
                                                Price=15024581
                                            }
                                        }
                                    },
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="VA-316",
                                        ItemName="Trolley Morral Backpack",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode=null,
                                                ReferenceName="Blanco",
                                                Amount=2342,
                                                Price=0
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode=string.Empty,
                                                ReferenceName="Silver",
                                                Amount=0,
                                                Price=0
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerReservationViewModel.Reservation
                            {
                                CreationDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(30),
                                ReservationDate = DateTime.Now.AddDays(1),
                                ReservationNumber = "0000018788",
                                Status = "Anulada",
                                Items = new List<CustomerReservationViewModel.Item>
                                {
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="OF-97",
                                        ItemName="Miniset de escritorio",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="877",
                                                ReferenceName="Blanco transparente",
                                                Amount=87,
                                                Price=789445647
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="027",
                                                ReferenceName="Negro",
                                                Amount=25,
                                                Price=15024581
                                            }
                                        }
                                    },
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="PM MU-63",
                                        ItemName="Botilito de aluminio 400ml",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="877",
                                                ReferenceName="Blanco transparente",
                                                Amount=87,
                                                Price=789445647
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new CustomerReservationViewModel.Customer {
                        CustomerName = "Claudia Ardila",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Reservations = new List<CustomerReservationViewModel.Reservation>
                        {
                            new CustomerReservationViewModel.Reservation
                            {
                                CreationDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(30),
                                ReservationDate = DateTime.Now.AddDays(1),
                                ReservationNumber = "0000018345",
                                Status = "Vencida",
                                Items = new List<CustomerReservationViewModel.Item>
                                {
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="032",
                                                ReferenceName="AZUL MARINO",
                                                Amount=358,
                                                Price=0
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="027",
                                                ReferenceName="Negro",
                                                Amount=25,
                                                Price=15024581
                                            }
                                        }
                                    },
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="VA-316",
                                        ItemName="Trolley Morral Backpack",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode=null,
                                                ReferenceName="Blanco",
                                                Amount=2342,
                                                Price=0
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode=string.Empty,
                                                ReferenceName="Silver",
                                                Amount=0,
                                                Price=0
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerReservationViewModel.Reservation
                            {
                                CreationDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.AddDays(30),
                                ReservationDate = DateTime.Now.AddDays(1),
                                ReservationNumber = "0000018788",
                                Status = "Anulada",
                                Items = new List<CustomerReservationViewModel.Item>
                                {
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="OF-97",
                                        ItemName="Miniset de escritorio",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="877",
                                                ReferenceName="Blanco transparente",
                                                Amount=87,
                                                Price=789445647
                                            },
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="027",
                                                ReferenceName="Negro",
                                                Amount=25,
                                                Price=15024581
                                            }
                                        }
                                    },
                                    new CustomerReservationViewModel.Item
                                    {
                                        InternalReference="PM MU-63",
                                        ItemName="Botilito de aluminio 400ml",
                                        References= new List<CustomerReservationViewModel.Reference>
                                        {
                                            new CustomerReservationViewModel.Reference
                                            {
                                                ReferenceCode="877",
                                                ReferenceName="Blanco transparente",
                                                Amount=87,
                                                Price=789445647
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
            var result = await DialogService.OpenAsync<CustomerReservationReportFilter>("Filtrar reporte de reservas por cliente", parameters: new Dictionary<string, object> { { "Filter", (CustomerReservationFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerReservationFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-reservation-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Reservas por cliente.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
