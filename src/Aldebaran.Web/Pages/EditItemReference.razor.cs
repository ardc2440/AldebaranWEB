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
    public partial class EditItemReference
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

        [Parameter]
        public int REFERENCE_ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            itemReference = await AldebaranDbService.GetItemReferenceByReferenceId(REFERENCE_ID);

            itemsForITEMID = await AldebaranDbService.GetItems();
        }
        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.ItemReference itemReference;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Item> itemsForITEMID;

        protected async Task FormSubmit()
        {
            try
            {
                await AldebaranDbService.UpdateItemReference(REFERENCE_ID, itemReference);
                DialogService.Close(itemReference);
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





        bool hasITEM_IDValue;

        [Parameter]
        public int ITEM_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            itemReference = new Aldebaran.Web.Models.AldebaranDb.ItemReference();

            hasITEM_IDValue = parameters.TryGetValue<int>("ITEM_ID", out var hasITEM_IDResult);

            if (hasITEM_IDValue)
            {
                itemReference.ITEM_ID = hasITEM_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }
    }
}