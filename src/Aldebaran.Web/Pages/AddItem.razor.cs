using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages
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

        protected override async Task OnInitializedAsync()
        {
            item = new Aldebaran.Web.Models.AldebaranDb.Item();

            measureUnitsForCIFMEASUREUNITID = await AldebaranDbService.GetMeasureUnits();

            currenciesForCURRENCYID = await AldebaranDbService.GetCurrencies();

            measureUnitsForFOBMEASUREUNITID = await AldebaranDbService.GetMeasureUnits();

            linesForLINEID = await AldebaranDbService.GetLines();
        }
        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.Item item;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> measureUnitsForCIFMEASUREUNITID;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Currency> currenciesForCURRENCYID;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.MeasureUnit> measureUnitsForFOBMEASUREUNITID;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Line> linesForLINEID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await AldebaranDbService.CreateItem(item);
                DialogService.Close(item);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}