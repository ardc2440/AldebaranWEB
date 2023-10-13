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
    public partial class AddItemsArea
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

            itemsForITEMID = await AldebaranDbService.GetItems();
        }
        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.ItemsArea itemsArea;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Item> itemsForITEMID;

        protected async Task FormSubmit()
        {
            try
            {
                await AldebaranDbService.CreateItemsArea(itemsArea);
                DialogService.Close(itemsArea);
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





        bool hasAREA_IDValue;

        [Parameter]
        public short AREA_ID { get; set; }

        bool hasITEM_IDValue;

        [Parameter]
        public int ITEM_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            itemsArea = new Aldebaran.Web.Models.AldebaranDb.ItemsArea();

            hasAREA_IDValue = parameters.TryGetValue<short>("AREA_ID", out var hasAREA_IDResult);

            if (hasAREA_IDValue)
            {
                itemsArea.AREA_ID = hasAREA_IDResult;
            }

            hasITEM_IDValue = parameters.TryGetValue<int>("ITEM_ID", out var hasITEM_IDResult);

            if (hasITEM_IDValue)
            {
                itemsArea.ITEM_ID = hasITEM_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }
    }
}