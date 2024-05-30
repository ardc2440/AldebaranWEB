using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
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

        [Inject]
        protected IPackagingService PackagingService { get; set; }
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

        protected Packaging packaging { get; set; }
        
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
                packaging = await PackagingService.FindByItemId(Item.ItemId)??new Packaging { ItemId = Item.ItemId};                
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
                Item.Packagings.Add(packaging);
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