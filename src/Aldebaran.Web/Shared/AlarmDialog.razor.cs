using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class AlarmDialog
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IAlarmTypeService AlarmTypeService { get; set; }

        [Inject]
        protected IAlarmMessageService AlarmMessageService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IAlarmService AlarmService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        #endregion

        #region Parameters

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public short DocumentTypeId { get; set; }

        [Parameter]
        public int DocumentId { get; set; }

        #endregion

        #region Variables
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        private bool IsErrorVisible;
        protected ServiceModel.Alarm alarm { get; set; }
        protected IEnumerable<ServiceModel.AlarmMessage> alarmMessages = new List<ServiceModel.AlarmMessage>();
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            alarm = new ServiceModel.Alarm
            {
                CreationDate = DateTime.Now,
                DocumentId = DocumentId,
                IsActive = true
            };

            alarmMessages = await AlarmMessageService.GetByDocumentTypeIdAsync(DocumentTypeId);
            IsErrorVisible = !alarmMessages.Any();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                Submitted = true;
                if (alarm.AlarmMessageId == 0)
                    return;
                alarm = await AlarmService.AddAsync(alarm);
                DialogService.Close(alarm);
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
