using Aldebaran.Application.Services;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Orders.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text.Encodings.Web;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Orders
{
    public partial class CustomerOrderReport
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerOrderReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        #region Variables
        protected CustomerOrderFilter Filter;
        protected CustomerOrderViewModel ViewModel;
        private bool IsBusy = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new CustomerOrderViewModel
            {
                Customers = new List<CustomerOrderViewModel.Customer> {
                    new CustomerOrderViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Orders = new List<CustomerOrderViewModel.Order>
                        {
                            new CustomerOrderViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderViewModel.Reference>
                                {
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial"
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerOrderViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderViewModel.Reference>
                                {
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new CustomerOrderViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Orders = new List<CustomerOrderViewModel.Order>
                        {
                            new CustomerOrderViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderViewModel.Reference>
                                {
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerOrderViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderViewModel.Reference>
                                {
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new CustomerOrderViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Orders = new List<CustomerOrderViewModel.Order>
                        {
                            new CustomerOrderViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderViewModel.Reference>
                                {
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerOrderViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderViewModel.Reference>
                                {
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new CustomerOrderViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Shipments = new List<CustomerOrderViewModel.Shipment>
                                        {
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    }
                                                }
                                            },
                                            new CustomerOrderViewModel.Shipment
                                            {
                                                CreationDate= DateTime.Now,
                                                DeliveryNote="7891256",
                                                TrackingNumber="8791AG-015",
                                                ShipmentMethodName="Directo",
                                                Notes = "01/02/2016 CC FACTURA NO.217521",
                                                References = new List<CustomerOrderViewModel.ShipmentReference>
                                                {
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
                                                    },
                                                    new CustomerOrderViewModel.ShipmentReference
                                                    {
                                                        ItemReference="5000SM",
                                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                                        ReferenceCode="032",
                                                        ReferenceName="AZUL MARINO",
                                                        Amount=10,
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
            var result = await DialogService.OpenAsync<CustomerOrderReportFilter>("Filtrar reporte de ordenes por cliente", parameters: new Dictionary<string, object> { { "Filter", (CustomerOrderFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerOrderFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-order-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Ordenes por cliente.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                NavigationManager.NavigateTo($"export/aldebarandb/customer-order/csv(fileName='{UrlEncoder.Default.Encode("Ordenes por cliente")}')", true);
            }

            if (args == null || args.Value == "xlsx")
            {
                NavigationManager.NavigateTo($"export/AldebaranDb/customer-order/excel(fileName='{UrlEncoder.Default.Encode("Ordenes por cliente")}')", true);
            }
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
