using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Text.RegularExpressions;
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
                Item.Packagings.Add(packaging);
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
            errorMessage = "";
            IsErrorVisible = false;
                        
            // Verifica si el checkbox está marcado
            if (Item.IsSaleOff)
            {
                var itemName = Item.ItemName;
                if (itemName.ToUpper().Contains("OFERTA"))
                    itemName = Regex.Replace(itemName, "Oferta", "", RegexOptions.IgnoreCase).Trim();
                
                // Si no sobrepasa los 50 caracteres, agrega " - Oferta".
                if (itemName.Trim().Length + " - Oferta".Length <= 50)
                    Item.ItemName = itemName.Trim() + " - Oferta";
                else
                {
                    // Si sobrepasa los 50 caracteres, muestra un error y cancela el cambio.
                    errorMessage = "El nombre del producto no puede exceder los 50 caracteres al agregar ' - Oferta'. \n\nModifique el nombre para que tenga maimo 40 caracteres";
                    Item.IsSaleOff = false; // Revierte el cambio del checkbox.
                    IsErrorVisible = true;
                }
            }
            else
            {
                // Si no está marcado, elimina " - Oferta" si está presente.
                Item.ItemName = Item.ItemName.Replace(" - Oferta", "");
            }

            StateHasChanged();

        }
        #endregion
    }
}