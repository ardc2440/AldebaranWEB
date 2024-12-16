using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class AddItem
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

        #region Variables
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        protected ServiceModel.Item Item;
        protected IEnumerable<ServiceModel.MeasureUnit> MeasureUnits;
        protected IEnumerable<ServiceModel.Currency> Currencies;
        protected IEnumerable<ServiceModel.Line> Lines;
        protected List<string> ValidationErrors;

        protected float Weight;
        protected float Height;
        protected float Width;
        protected float Length;
        protected int Quantity;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Item = new ServiceModel.Item
                {
                    IsActive = true
                };
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
                ValidationErrors = new List<string>();
                var itemNameAlreadyExists = await ItemService.ExistsByItemName(Item.ItemName);
                if (itemNameAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un artículo con el mismo nombre.");
                }
                var internalReferenceAlreadyExists = await ItemService.ExistsByIternalReference(Item.InternalReference);
                if (internalReferenceAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe un artículo con la misma referencia interna.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }
                Item.Packagings.Add(new ServiceModel.Packaging { Height = Height, Length = Length, Weight=Weight, Width = Width, Quantity = Quantity });
                await ItemService.AddAsync(Item);
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