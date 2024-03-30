using Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available.Components;
using Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available
{
    public partial class FreezoneVsAvailableReport
    {
        #region Injections
        [Inject]
        protected ILogger<FreezoneVsAvailableReport> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Variables

        protected FreezoneVsAvailableFilter Filter;
        protected FreezoneVsAvailableViewModel ViewModel;
        private bool IsBusy = false;
        private bool IsLoadingData = false;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            ViewModel = new FreezoneVsAvailableViewModel
            {
                Lines = new List<FreezoneVsAvailableViewModel.Line>
                {
                   new FreezoneVsAvailableViewModel.Line
                   {
                       LineName="Importados",
                       LineCode="001",
                       Items= new List<FreezoneVsAvailableViewModel.Item>
                       {
                           new FreezoneVsAvailableViewModel.Item
                           {
                               InternalReference="00110-01",
                               ItemName="HOT PORTAMINA",
                               References= new List<FreezoneVsAvailableViewModel.Reference>
                               {
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceCode="0001",
                                       ReferenceName="Blanco",
                                       FreeZone=15,
                                       AvailableAmount=232452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Rojo",
                                       FreeZone=897,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Verde",
                                       FreeZone=877,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Azul",
                                       FreeZone=5667,
                                       AvailableAmount=24352
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Amarillo",
                                       FreeZone=56785,
                                       AvailableAmount=525452
                                   }
                               }
                           },
                           new FreezoneVsAvailableViewModel.Item
                           {
                               InternalReference="AA973-1-01",
                               ItemName="LACE",
                               References= new List<FreezoneVsAvailableViewModel.Reference>
                               {
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Blanco",
                                       FreeZone=15,
                                       AvailableAmount=232452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Rojo",
                                       FreeZone=897,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Verde",
                                       FreeZone=877,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Azul",
                                       FreeZone=5667,
                                       AvailableAmount=24352
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Amarillo",
                                       FreeZone=56785,
                                       AvailableAmount=525452
                                   }
                               }
                           },
                           new FreezoneVsAvailableViewModel.Item
                           {
                               InternalReference="00110-01",
                               ItemName="HOT PORTAMINA",
                               References= new List<FreezoneVsAvailableViewModel.Reference>
                               {
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Blanco",
                                       FreeZone=15,
                                       AvailableAmount=232452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Rojo",
                                       FreeZone=897,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Verde",
                                       FreeZone=877,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Azul",
                                       FreeZone=5667,
                                       AvailableAmount=24352
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Amarillo",
                                       FreeZone=56785,
                                       AvailableAmount=525452
                                   }
                               }
                           },
                           new FreezoneVsAvailableViewModel.Item
                           {
                               InternalReference="AA973-1-01",
                               ItemName="LACE",
                               References= new List<FreezoneVsAvailableViewModel.Reference>
                               {
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Blanco",
                                       FreeZone=15,
                                       AvailableAmount=232452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Rojo",
                                       FreeZone=897,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Verde",
                                       FreeZone=877,
                                       AvailableAmount=23452
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Azul",
                                       FreeZone=5667,
                                       AvailableAmount=24352
                                   },
                                   new FreezoneVsAvailableViewModel.Reference
                                   {
                                       ReferenceName="Amarillo",
                                       FreeZone=56785,
                                       AvailableAmount=525452
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
            var result = await DialogService.OpenAsync<FreezoneVsAvailableReportFilter>("Filtrar reporte de Zona franca vs. Disponible", parameters: new Dictionary<string, object> { { "Filter", Filter } }, options: new DialogOptions { Width = "800px" });
            if (result == null)
                return;
            Filter = (FreezoneVsAvailableFilter)result;
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "freezone-report-container");
            var pdfBytes = await PdfService.GetBytes(html, false);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Zona franca vs Disponible.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        async Task ToggleReadMore()
        {
            await JSRuntime.InvokeVoidAsync("readMoreToggle", "toggleLink");
        }
        #endregion
    }
}
