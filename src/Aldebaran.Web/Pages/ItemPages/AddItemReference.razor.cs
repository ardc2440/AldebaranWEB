using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class AddItemReference
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
            item = await AldebaranDbService.GetItemByItemId(ITEM_ID);
        }
        protected bool errorVisible;
        protected Models.AldebaranDb.ItemReference itemReference;

        protected Models.AldebaranDb.Item item;
        protected bool isSubmitInProgress;

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateItemReference(itemReference);
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





        bool hasITEM_IDValue;

        [Parameter]
        public int ITEM_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            itemReference = new Models.AldebaranDb.ItemReference();

            hasITEM_IDValue = parameters.TryGetValue<int>("ITEM_ID", out var hasITEM_IDResult);

            if (hasITEM_IDValue)
            {
                itemReference.ITEM_ID = hasITEM_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }
    }
}