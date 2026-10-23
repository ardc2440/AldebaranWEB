using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Text.RegularExpressions;

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
        protected Item Item;
        protected string errorMessage = "";
        protected IEnumerable<MeasureUnit> MeasureUnits;
        protected IEnumerable<Currency> Currencies;
        protected IEnumerable<Line> Lines;

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
                packaging = await PackagingService.FindByItemId(Item.ItemId) ?? new Packaging { ItemId = Item.ItemId };
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
                Item.Packagings = new List<Packaging> { packaging };
                await ItemService.UpdateAsync(ITEM_ID, Item);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
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

        protected async Task OnIfSaleOffChanged()
        {
            if (Item.IsSaleOff)
                Item.IsBombPrice = false;

            Item.IsSaleOff = ApplyNameMark(Item.IsSaleOff, ItemNameMarks.SaleOffSuffix, "Oferta");
            StateHasChanged();
        }

        protected async Task OnBombPriceChanged()
        {
            if (Item.IsBombPrice)
                Item.IsSaleOff = false;

            Item.IsBombPrice = ApplyNameMark(Item.IsBombPrice, ItemNameMarks.BombPriceSuffix, "Precio Bomba");
            StateHasChanged();
        }

        /// <summary>Agrega o retira el sufijo de la marca en el nombre; devuelve el estado final de la marca.</summary>
        private bool ApplyNameMark(bool marked, string suffix, string markName)
        {
            errorMessage = "";
            IsErrorVisible = false;

            if (!marked)
            {
                Item.ItemName = ItemNameMarks.Remove(Item.ItemName, suffix);
                return false;
            }

            if (ItemNameMarks.TryApply(Item.ItemName, suffix, out var itemName))
            {
                Item.ItemName = itemName;
                return true;
            }

            // Si sobrepasa los 50 caracteres, muestra un error y cancela el cambio.
            errorMessage = $"El nombre del producto no puede exceder los {ItemNameMarks.MaxNameLength} caracteres al agregar '{suffix.Trim()}' ({markName}). \n\nModifique el nombre para que tenga máximo {ItemNameMarks.MaxBaseLength(suffix)} caracteres";
            IsErrorVisible = true;
            return false;
        }
        #endregion
    }
}