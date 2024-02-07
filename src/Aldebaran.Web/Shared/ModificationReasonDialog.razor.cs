using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class ModificationReasonDialog
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IModificationReasonService ModificationReasonService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public string DOCUMENT_TYPE_CODE { get; set; }

        [Parameter]
        public string TITLE { get; set; }

        #endregion

        #region Variables
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        private bool IsErrorVisible;
        protected ServiceModel.ModificationReason ModificationReason { get; set; }
        protected IEnumerable<ServiceModel.ModificationReason> ModificationReasons = new List<ServiceModel.ModificationReason>();
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            ModificationReason = new ServiceModel.ModificationReason();
            ModificationReasons = await ModificationReasonService.GetAsync(DOCUMENT_TYPE_CODE);
            IsErrorVisible = !ModificationReasons.Any();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                Submitted = true;
                if (ModificationReason.ModificationReasonId == 0)
                    return;
                var employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                var reason = new ServiceModel.Reason { EmployeeId = employee.EmployeeId, ReasonId = ModificationReason.ModificationReasonId };
                DialogService.Close(reason);
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected void Cancel(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}
