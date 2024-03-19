using Aldebaran.Web.Pages.ReportPages.Order_Shipment.Components;
using Aldebaran.Web.Pages.ReportPages.Order_Shipment.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Order_Shipment
{
    public partial class OrderShipmentReport
    {
        #region Injections
        [Inject]
        protected ILogger<OrderShipmentReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Variables
        protected OrderShipmentFilter Filter;
        protected OrderShipmentViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new OrderShipmentViewModel()
            {
                Orders = new List<OrderShipmentViewModel.Order>
                {
                    new OrderShipmentViewModel.Order
                    {
                        OrderNumber="14457",
                        CreationDate = DateTime.Now,
                        RequestDate = DateTime.Now,
                        ExpectedReceiptDate = DateTime.Now,
                        RealReceiptDate = DateTime.Now,
                        ProviderName = "JLinG",
                        Forwarder = new OrderShipmentViewModel.Forwarder
                        {
                            ForwarderName = "DHL",
                            Phone ="3214668850",
                            Fax="24353134",
                            Email="info@dhl.com"
                        },
                        ForwarderAgent = new OrderShipmentViewModel.ForwarderAgent
                        {
                            ForwarderAgentName = "Javier Linares",
                            Phone = "3168849474",
                            Fax="546573157",
                            Email="javierl@dhl.com"
                        },
                        ImportNumber="13245636",
                        ShipmentMethodName="Transporte Aéreo",
                        EmbarkationPort="Xiamen",
                        ProformaNumber="TUMP-5033+5045/19",
                        ArrivalWarehouse = "Local",
                        Lines = new List<OrderShipmentViewModel.Line>
                                {
                                    new OrderShipmentViewModel.Line
                                    {
                                        LineCode="001",
                                        LineName = "Importados",
                                        Items = new List<OrderShipmentViewModel.Item>
                                        {
                                            new OrderShipmentViewModel.Item
                                            {
                                                InternalReference ="00110-01",
                                                ItemName="HOT PORTAMINA",
                                                References = new List<OrderShipmentViewModel.Reference>
                                                {
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        Amount=200,
                                                        Volume=0,
                                                        Weight=200,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        Amount=180,
                                                        Volume=120,
                                                        Weight=3500,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00003",
                                                        ReferenceName="Verde",
                                                        Amount=180,
                                                        Volume=546,
                                                        Weight=0,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00004",
                                                        ReferenceName="Amarillo",
                                                        Amount=180,
                                                        Volume=483,
                                                        Weight=789,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00005",
                                                        ReferenceName="Negro",
                                                        Amount=180,
                                                        Volume=0,
                                                        Weight=0,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00006",
                                                        ReferenceName="Naranja",
                                                        Amount=180,
                                                        Volume=23452,
                                                        Weight=4567,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                    },
                    new OrderShipmentViewModel.Order
                    {
                        OrderNumber="14457",
                        CreationDate = DateTime.Now,
                        RequestDate = DateTime.Now,
                        ExpectedReceiptDate = DateTime.Now,
                        RealReceiptDate = DateTime.Now,
                        ProviderName = "JLinG",
                        Forwarder = new OrderShipmentViewModel.Forwarder
                        {
                            ForwarderName = "DHL",
                            Phone ="3214668850",
                            Fax="24353134",
                            Email="info@dhl.com"
                        },
                        ForwarderAgent = new OrderShipmentViewModel.ForwarderAgent
                        {
                            ForwarderAgentName = "Javier Linares",
                            Phone = "3168849474",
                            Fax="546573157",
                            Email="javierl@dhl.com"
                        },
                        ImportNumber="13245636",
                        ShipmentMethodName="Transporte Aéreo",
                        EmbarkationPort="Xiamen",
                        ProformaNumber="TUMP-5033+5045/19",
                        ArrivalWarehouse = "Zona Franca",
                        Lines = new List<OrderShipmentViewModel.Line>
                                {
                                    new OrderShipmentViewModel.Line
                                    {
                                        LineCode="001",
                                        LineName = "Importados",
                                        Items = new List<OrderShipmentViewModel.Item>
                                        {
                                            new OrderShipmentViewModel.Item
                                            {
                                                InternalReference ="00110-01",
                                                ItemName="HOT PORTAMINA",
                                                References = new List<OrderShipmentViewModel.Reference>
                                                {
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        Amount=200,
                                                        Volume=0,
                                                        Weight=200,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        Amount=180,
                                                        Volume=120,
                                                        Weight=3500,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00003",
                                                        ReferenceName="Verde",
                                                        Amount=180,
                                                        Volume=546,
                                                        Weight=0,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00004",
                                                        ReferenceName="Amarillo",
                                                        Amount=180,
                                                        Volume=483,
                                                        Weight=789,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00005",
                                                        ReferenceName="Negro",
                                                        Amount=180,
                                                        Volume=0,
                                                        Weight=0,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00006",
                                                        ReferenceName="Naranja",
                                                        Amount=180,
                                                        Volume=23452,
                                                        Weight=4567,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                    },
                    new OrderShipmentViewModel.Order
                    {
                        OrderNumber="14457",
                        CreationDate = DateTime.Now,
                        RequestDate = DateTime.Now,
                        ExpectedReceiptDate = DateTime.Now,
                        RealReceiptDate = DateTime.Now,
                        ProviderName = "JLinG",
                        Forwarder = new OrderShipmentViewModel.Forwarder
                        {
                            ForwarderName = "DHL",
                            Phone ="3214668850",
                            Fax="24353134",
                            Email="info@dhl.com"
                        },
                        ForwarderAgent = new OrderShipmentViewModel.ForwarderAgent
                        {
                            ForwarderAgentName = "Javier Linares",
                            Phone = "3168849474",
                            Fax="546573157",
                            Email="javierl@dhl.com"
                        },
                        ImportNumber="13245636",
                        ShipmentMethodName="Transporte Aéreo",
                        EmbarkationPort="Xiamen",
                        ProformaNumber="TUMP-5033+5045/19",
                        ArrivalWarehouse = "Local",
                        Lines = new List<OrderShipmentViewModel.Line>
                                {
                                    new OrderShipmentViewModel.Line
                                    {
                                        LineCode="001",
                                        LineName = "Importados",
                                        Items = new List<OrderShipmentViewModel.Item>
                                        {
                                            new OrderShipmentViewModel.Item
                                            {
                                                InternalReference ="00110-01",
                                                ItemName="HOT PORTAMINA",
                                                References = new List<OrderShipmentViewModel.Reference>
                                                {
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00001",
                                                        ReferenceName="Blanco",
                                                        Amount=200,
                                                        Volume=0,
                                                        Weight=200,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00002",
                                                        ReferenceName="Rojo",
                                                        Amount=180,
                                                        Volume=120,
                                                        Weight=3500,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00003",
                                                        ReferenceName="Verde",
                                                        Amount=180,
                                                        Volume=546,
                                                        Weight=0,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00004",
                                                        ReferenceName="Amarillo",
                                                        Amount=180,
                                                        Volume=483,
                                                        Weight=789,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00005",
                                                        ReferenceName="Negro",
                                                        Amount=180,
                                                        Volume=0,
                                                        Weight=0,
                                                    },
                                                    new OrderShipmentViewModel.Reference
                                                    {
                                                        ReferenceCode="00006",
                                                        ReferenceName="Naranja",
                                                        Amount=180,
                                                        Volume=23452,
                                                        Weight=4567,
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
            var result = await DialogService.OpenAsync<OrderShipmentReportFilter>("Filtrar reporte de ordenes en tránsito", parameters: new Dictionary<string, object> { { "Filter", (OrderShipmentFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (OrderShipmentFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "order-shipment-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Ordenes en tránsito.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
