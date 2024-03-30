using Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.Components;
using Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
namespace Aldebaran.Web.Pages.ReportPages.Warehouse_Transfers
{
    public partial class WarehouseTransfersReport
    {
        #region Injections
        [Inject]
        protected ILogger<WarehouseTransfersReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        #endregion

        #region Variables
        protected WarehouseTransfersFilter Filter;
        protected WarehouseTransfersViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new WarehouseTransfersViewModel()
            {
                WarehouseTransfers = new List<WarehouseTransfersViewModel.WarehouseTransfer>
                {
                    new WarehouseTransfersViewModel.WarehouseTransfer
                    {
                        Date = DateTime.Now,
                        SourceWarehouseName="Bodega local",
                        TargetWarehouseName="Zona franca",
                        Reason="BRAND NEW B-1-2",
                        NationalizationNumber="9418-1",
                        RegistrationDate=DateTime.Now,
                        References = new List<WarehouseTransfersViewModel.Reference>
                        {
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            }
                        }
                    },
                    new WarehouseTransfersViewModel.WarehouseTransfer
                    {
                        Date = DateTime.Now,
                        SourceWarehouseName="Bodega local",
                        TargetWarehouseName="Zona franca",
                        Reason="BRAND NEW B-1-2",
                        NationalizationNumber="9418-1",
                        RegistrationDate=DateTime.Now,
                        References = new List<WarehouseTransfersViewModel.Reference>
                        {
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            }
                        }
                    },
                    new WarehouseTransfersViewModel.WarehouseTransfer
                    {
                        Date = DateTime.Now,
                        SourceWarehouseName="Bodega local",
                        TargetWarehouseName="Zona franca",
                        Reason="BRAND NEW B-1-2",
                        NationalizationNumber="9418-1",
                        RegistrationDate=DateTime.Now,
                        References = new List<WarehouseTransfersViewModel.Reference>
                        {
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            }
                        }
                    },
                    new WarehouseTransfersViewModel.WarehouseTransfer
                    {
                        Date = DateTime.Now,
                        SourceWarehouseName="Bodega local",
                        TargetWarehouseName="Zona franca",
                        Reason="BRAND NEW B-1-2",
                        NationalizationNumber="9418-1",
                        RegistrationDate=DateTime.Now,
                        References = new List<WarehouseTransfersViewModel.Reference>
                        {
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            }
                        }
                    },
                    new WarehouseTransfersViewModel.WarehouseTransfer
                    {
                        Date = DateTime.Now,
                        SourceWarehouseName="Bodega local",
                        TargetWarehouseName="Zona franca",
                        Reason="BRAND NEW B-1-2",
                        NationalizationNumber="9418-1",
                        RegistrationDate=DateTime.Now,
                        References = new List<WarehouseTransfersViewModel.Reference>
                        {
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
                            },
                            new WarehouseTransfersViewModel.Reference
                            {
                                ItemReference="5000SM",
                                ItemName="GILDAN CAMISETA T-SHIRT",
                                ReferenceCode="032",
                                ReferenceName="AZUL MARINO",
                                Amount=358,
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
            var result = await DialogService.OpenAsync<WarehouseTransfersReportFilter>("Filtrar reporte de traslados entre bodegas", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (WarehouseTransfersFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "warehouse-transfer-report-container");
            var pdfBytes = await PdfService.GetBytes(html, true);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Traslados entre bodegas.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
