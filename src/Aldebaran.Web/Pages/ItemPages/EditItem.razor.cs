using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class EditItem
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IMeasureUnitService MeasureUnitService { get; set; }

        [Inject]
        protected ICurrencyService CurrencyService { get; set; }

        [Inject]
        protected ILineService LineService { get; set; }

        [Inject]
        protected IItemService ItemService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int ITEM_ID { get; set; }
        #endregion

        #region Variables
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        protected ServiceModel.Item Item;
        protected IEnumerable<ServiceModel.MeasureUnit> MeasureUnits;
        protected IEnumerable<ServiceModel.Currency> Currencies;
        protected IEnumerable<ServiceModel.Line> Lines;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Item = await ItemService.FindAsync(ITEM_ID);
                MeasureUnits = await MeasureUnitService.GetAsync();
                Currencies = await CurrencyService.GetAsync();
                Lines = await LineService.GetAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await ItemService.UpdateAsync(ITEM_ID, Item);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}