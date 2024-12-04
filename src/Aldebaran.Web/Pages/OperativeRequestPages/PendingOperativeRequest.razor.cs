using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.OperativeRequestPages
{
    public partial class PendingOperativeRequest
    {
        #region Injections

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        protected TooltipService TooltipService { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        protected ICancellationRequestService CancellationRequestService { get; set; }
        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }
        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        #endregion

        #region Variables

        protected bool isLoadingInProgress;
        protected string search = "";
        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;

        protected LocalizedDataGrid<CancellationRequestModel> cancellationRequestGrid;
        protected IEnumerable<CancellationRequestModel> cancellationRequests;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await Task.Yield();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events
        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip.Value;
            top = args.Top.Value;
            await GetAttendedOperationalRequestAsync();
        }

        async Task ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null)
        {
            TooltipService.Open(elementReference, content, options);
            await Task.Delay(1000);
            TooltipService.Close();
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await cancellationRequestGrid.GoToPage(0);
            await GetAttendedOperationalRequestAsync();
        }

        protected async Task GetAttendedOperationalRequestAsync()
        {
            await Task.Yield();
            (cancellationRequests, count) = await CancellationRequestService.GetAllByStatusOrderAsync(skip, top, search, true);
        }

        protected async Task ApproveRequest(CancellationRequestModel request)
        {
            var cancellationRequest = await CancellationRequestService.FindAsync(request.CancellationRequestId);
            var approvedStatus = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(cancellationRequest.DocumentType.DocumentTypeId, 2);
            var responseReason = "";

            var dialogResult = await DialogService.OpenAsync<ResponseReasonDialog>("Motivo de aprobación",
                                    new Dictionary<string, object> { { "Reason", "" }, { "Action", "Approve" } },
                                    new DialogOptions { Width = "400px", Height = "300px", Draggable = true, Resizable = true });

            if (!string.IsNullOrEmpty(dialogResult))
            {
                responseReason = (string)dialogResult;

                cancellationRequest.StatusDocumentTypeId = approvedStatus.StatusDocumentTypeId;
                cancellationRequest.StatusDocumentType = approvedStatus;
                cancellationRequest.ResponseDate = DateTime.Now;
                cancellationRequest.ResponseEmployee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                cancellationRequest.ResponseEmployeeId = cancellationRequest.ResponseEmployee.EmployeeId;
                cancellationRequest.ResponseReason = responseReason;
                try
                {
                    cancellationRequest = await CancellationRequestService.UpdateAsync(cancellationRequest);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Solicitud operativa",
                        Severity = NotificationSeverity.Success,
                        Duration = 6000,
                        Detail = $"La solicitud ha sido aprobada correctamente."
                    });
                    await cancellationRequestGrid.Reload();
                }
                catch (Exception)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = $"Error",
                        Duration = 6000,
                        Detail = $"No se ha podido aprobar la solicituud."
                    });
                }

            }
        }

        protected async Task DenyRequest(CancellationRequestModel request)
        {
            var cancellationRequest = await CancellationRequestService.FindAsync(request.CancellationRequestId);
            var deniedStatus = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(cancellationRequest.DocumentType.DocumentTypeId, 3);
            var responseReason = "";

            var dialogResult = await DialogService.OpenAsync<ResponseReasonDialog>("Motivo de rechazo", 
                                    new Dictionary<string, object> { { "Reason", "" }, { "Action","Deny" } },
                                    new DialogOptions { Width = "400px", Height = "300px", Draggable = true, Resizable = true });

            if (!string.IsNullOrEmpty(dialogResult))
            {
                responseReason = (string)dialogResult;

                cancellationRequest.StatusDocumentTypeId = deniedStatus.StatusDocumentTypeId;
                cancellationRequest.StatusDocumentType = deniedStatus;
                cancellationRequest.ResponseDate = DateTime.Now;
                cancellationRequest.ResponseEmployee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                cancellationRequest.ResponseEmployeeId = cancellationRequest.ResponseEmployee.EmployeeId;
                cancellationRequest.ResponseReason = responseReason;

                try
                {
                    cancellationRequest = await CancellationRequestService.UpdateAsync(cancellationRequest);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Solicitud operativa",
                        Severity = NotificationSeverity.Success,
                        Duration = 6000,
                        Detail = $"La solicitud ha sido rechazada correctamente."
                    });
                    await cancellationRequestGrid.Reload();
                }
                catch (Exception)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = $"Error",
                        Duration = 6000,
                        Detail = $"No se ha podido rechazar la solicituud."
                    });                    
                }
            }
        }

        #endregion
    }
}
