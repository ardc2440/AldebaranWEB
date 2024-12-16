using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class AdjustmentSummary
    {
        #region Injections
        [Inject]
        protected IAdjustmentService AdjustmentService { get; set; }

        [Inject]
        protected IAdjustmentDetailService AdjustmentDetailService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int Id { get; set; }
        #endregion

        #region Variables
        protected Adjustment Adjustment;
        protected IEnumerable<AdjustmentDetail> AdjustmentDetails;
        private bool IsBusy = false;
        protected bool isLoadingInProgress;
        private bool firstLine = true;
        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Adjustment = await AdjustmentService.FindAsync(Id);
                AdjustmentDetails = await AdjustmentDetailService.GetByAdjustmentIdAsync(Id);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        
        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "adjustment-summary");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", $"Ajuste{Adjustment.AdjustmentId}.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "adjustment-summary");
            }
            IsBusy = false;
        }

        void CloseWindow(MouseEventArgs args)
        {
            DialogService.Close(false);
        }

        #endregion
    }
}