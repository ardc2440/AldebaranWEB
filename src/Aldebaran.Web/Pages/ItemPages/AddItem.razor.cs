using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class AddItem
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        
        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected bool isSubmitInProgress;
        protected bool errorVisible;
        protected Models.AldebaranDb.Item item;
        protected IEnumerable<Models.AldebaranDb.MeasureUnit> measureUnitsForCIFMEASUREUNITID;
        protected IEnumerable<Models.AldebaranDb.Currency> currenciesForCURRENCYID;
        protected IEnumerable<Models.AldebaranDb.MeasureUnit> measureUnitsForFOBMEASUREUNITID;
        protected IEnumerable<Models.AldebaranDb.Line> linesForLINEID;

        protected override async Task OnInitializedAsync()
        {
            item = new Models.AldebaranDb.Item();
            item.IS_ACTIVE = true;

            measureUnitsForCIFMEASUREUNITID = await AldebaranDbService.GetMeasureUnits();
            currenciesForCURRENCYID = await AldebaranDbService.GetCurrencies();
            measureUnitsForFOBMEASUREUNITID = await AldebaranDbService.GetMeasureUnits();
            linesForLINEID = await AldebaranDbService.GetLines();
        }
       
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateItem(item);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}