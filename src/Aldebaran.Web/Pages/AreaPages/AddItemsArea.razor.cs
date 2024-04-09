using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.AreaPages
{
    public partial class AddItemsArea : ComponentBase
    {
        #region Injections
        [Inject]
        protected ILogger<AddItemsArea> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemAreaService ItemAreaService { get; set; }

        [Inject]
        protected IAreaService AreaService { get; set; }

        [Inject]
        protected IItemService ItemService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public short AREA_ID { get; set; }
        #endregion

        #region Variables
        protected ServiceModel.ItemsArea ItemArea;
        protected ServiceModel.Area Area;
        protected List<ServiceModel.Item> AvailableItemsForSelection = new List<ServiceModel.Item>();
        protected bool IsSubmitInProgress;
        protected bool IsErrorVisible;
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ItemArea = new ServiceModel.ItemsArea() { AreaId = AREA_ID };
                Area = await AreaService.FindAsync(AREA_ID);
                var currentItemsInArea = await ItemAreaService.GetByAreaIdAsync(AREA_ID);
                var items = await ItemService.GetAsync();
                // Articulos disponibles para seleccion, Articulos excepto los ya seleccionados
                AvailableItemsForSelection = items.Where(w => !currentItemsInArea.Any(x => x.ItemId == w.ItemId)).ToList();
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
                await ItemAreaService.AddAsync(ItemArea);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
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
        protected async Task ItemHandler(ServiceModel.Item item)
        {
            ItemArea.ItemId = item?.ItemId ?? 0;
        }
        #endregion

    }
}