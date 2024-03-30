using Aldebaran.Web.Pages.ReportPages.Customer_Sales.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Sales.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Sales
{
    public partial class CustomerSalesReport
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerSalesReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        #region Variables
        protected CustomerSalesFilter Filter;
        protected CustomerSalesViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new CustomerSalesViewModel
            {
                Customers = new List<CustomerSalesViewModel.Customer> {
                    new CustomerSalesViewModel.Customer
                    {
                        CustomerName="Javier Linares",
                        Fax="123165434",
                        Phone="3168849474",
                        Orders = new List<CustomerSalesViewModel.Order>
                        {
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                        }
                    },
                    new CustomerSalesViewModel.Customer
                    {
                        CustomerName="Javier Linares",
                        Fax="123165434",
                        Phone="3168849474",
                        Orders = new List<CustomerSalesViewModel.Order>
                        {
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                        }
                    },
                    new CustomerSalesViewModel.Customer
                    {
                        CustomerName="Javier Linares",
                        Fax="123165434",
                        Phone="3168849474",
                        Orders = new List<CustomerSalesViewModel.Order>
                        {
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                            new CustomerSalesViewModel.Order
                            {
                                CreationDate= DateTime.Now,
                                EstimatedDeliveryDate= DateTime.Now.AddDays(7),
                                OrderDate= DateTime.Now,
                                InternalNotes ="Realizado por Enrique",
                                Status="Completado",
                                OrderNumber="123456132334",
                                References= new List<CustomerSalesViewModel.Reference>
                                {
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    },
                                    new CustomerSalesViewModel.Reference
                                    {
                                        ItemReference="00110-01",
                                        ItemName="HOT PORTAMINA",
                                        ReferenceCode="00001",
                                        ReferenceName="Blanco",
                                        Amount=150,
                                        DeliveredAmount=80,
                                        Shipments= new List<CustomerSalesViewModel.Shipment>
                                        {
                                            new CustomerSalesViewModel.Shipment
                                            {
                                               TrackingNumber="87135" ,
                                               DeliveryNote="ABC75-84",
                                               Notes="Despachado por Andres",
                                               ShipmentDate=DateTime.Now,
                                               ShipmentMethodName="Aereo",
                                               References = new List<CustomerSalesViewModel.ShipmentReference>
                                               {
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=50,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=30,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00001",
                                                       ReferenceName="Blanco",
                                                   },
                                                   new CustomerSalesViewModel.ShipmentReference
                                                   {
                                                       Amount=200,
                                                       ItemReference="00110-01",
                                                       ItemName="HOT PORTAMINA",
                                                       ReferenceCode="00002",
                                                       ReferenceName="Rojo",
                                                   }
                                               }
                                            }
                                        }
                                    }
                                }
                            },
                        }
                    }
                }
            };
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<CustomerSalesReportFilter>("Filtrar reporte de ventas por cliente", parameters: new Dictionary<string, object> { { "Filter", (CustomerSalesFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerSalesFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-sales-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Ventas por cliente.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
