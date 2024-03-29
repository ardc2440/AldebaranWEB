using Aldebaran.Application.Services;
using Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.Components;
using Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Customer_Order_Activities
{
    public partial class CustomerOrderActivityReport
    {
        #region Injections
        [Inject]
        protected ILogger<CustomerOrderActivityReport> Logger { get; set; }

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
        protected CustomerOrderActivityFilter Filter;
        protected CustomerOrderActivityViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            IsLoadingData = true; //Muestra un loader mientras la informacion es obtenida de la base de datos
            ViewModel = new CustomerOrderActivityViewModel
            {
                Customers = new List<CustomerOrderActivityViewModel.Customer> {
                    new CustomerOrderActivityViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Orders = new List<CustomerOrderActivityViewModel.Order>
                        {
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                    new CustomerOrderActivityViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Orders = new List<CustomerOrderActivityViewModel.Order>
                        {
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                    new CustomerOrderActivityViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Orders = new List<CustomerOrderActivityViewModel.Order>
                        {
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                    new CustomerOrderActivityViewModel.Customer {
                        CustomerName = "Javier Linares",
                        Phone = "3168849474",
                        Fax = "12435643",
                        Orders = new List<CustomerOrderActivityViewModel.Order>
                        {
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                            new CustomerOrderActivityViewModel.Order
                            {
                                CreationDate = DateTime.Now,
                                EstimatedDeliveryDate = DateTime.Now.AddDays(30),
                                OrderDate = DateTime.Now.AddDays(1),
                                OrderNumber = "0000018345",
                                Status = "Vencida",
                                InternalNotes = "Atendido por Viviana Mora",
                                CustomerNotes = "Dejar en recepcion",
                                References = new List<CustomerOrderActivityViewModel.Reference>
                                {
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-1),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            },
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now.AddDays(-3),
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                        }
                                    },
                                    new CustomerOrderActivityViewModel.Reference
                                    {
                                        ItemReference="5000SM",
                                        ItemName="GILDAN CAMISETA T-SHIRT",
                                        ReferenceCode="032",
                                        ReferenceName="AZUL MARINO",
                                        Amount=358,
                                        DeliveredAmount=100,
                                        InProcessAmount=258,
                                        Status="Entrega parcial",
                                        Activities = new List<CustomerOrderActivityViewModel.Activity>
                                        {
                                            new CustomerOrderActivityViewModel.Activity
                                            {
                                                CreationDate = DateTime.Now,
                                                AreaName="Analisis de inventario",
                                                EmployeeName="Javier Linares",
                                                Notes="Cliente recogera el pedido",
                                                Status="Entregado"
                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                }
            };
            //ViewModel = new CustomerOrderActivityViewModel { }; //Simula que no existe informacion para el reporte.
            await Task.Delay(10000); //Simula que el obtener informacion de la base de datos ha llevado 10sg
            IsLoadingData = false; //Oculta el loader
        }
        #endregion

        #region Events
        async Task OpenFilters()
        {
            var result = await DialogService.OpenAsync<CustomerOrderActivityReportFilter>("Filtrar reporte de actividades de pedidos", parameters: new Dictionary<string, object> { { "Filter", (CustomerOrderActivityFilter)Filter?.Clone() } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (CustomerOrderActivityFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-order-activity-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Actividades de pedidos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
