using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.EmailNotificationSettingsPages
{
    public partial class EmailNotificationSettings
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<EmailNotificationSettings> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected INotificationTemplateService NotificationTemplateService { get; set; }

        [Inject]
        public IEmailNotificationProviderSettingsService EmailNotificationProviderSettingsService { get; set; }
        #endregion

        #region Variables
        protected ServiceModel.EmailNotificationProvider Provider;
        protected IEnumerable<ServiceModel.EmailNotificationProvider.SecureSocketOptions> SecureSocketOptions;
        protected ServiceModel.NotificationTemplate CustomerOrderNew;
        protected ServiceModel.NotificationTemplate CustomerOrderUpdate;
        protected ServiceModel.NotificationTemplate CustomerOrderForwarding;
        protected ServiceModel.NotificationTemplate CustomerOrderCancellation;
        protected ServiceModel.NotificationTemplate CustomerReservationNew;
        protected ServiceModel.NotificationTemplate CustomerReservationUpdate;
        protected ServiceModel.NotificationTemplate CustomerReservationForwarding;
        protected ServiceModel.NotificationTemplate CustomerReservationCancellation;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        private static readonly string SettingsKey = "Sales";
        public string ConfirmPassword;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await GetEmailProviderSettings();
                await GetEmailTemplates();
                SecureSocketOptions = Enum.GetValues(typeof(ServiceModel.EmailNotificationProvider.SecureSocketOptions))
                                        .Cast<ServiceModel.EmailNotificationProvider.SecureSocketOptions>()
                                        .ToList();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        private async Task GetEmailProviderSettings()
        {
            Provider = await EmailNotificationProviderSettingsService.GetAsync(SettingsKey);
            Provider.Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
        private async Task GetEmailTemplates()
        {
            var templates = await NotificationTemplateService.GetAsync();
            CustomerOrderNew = templates.FirstOrDefault(w => w.Name == "Customer:Order:New");
            CustomerOrderUpdate = templates.FirstOrDefault(w => w.Name == "Customer:Order:Update");
            CustomerOrderForwarding = templates.FirstOrDefault(w => w.Name == "Customer:Order:Forwarding");
            CustomerOrderCancellation = templates.FirstOrDefault(w => w.Name == "Customer:Order:Cancellation");
            CustomerReservationNew = templates.FirstOrDefault(w => w.Name == "Customer:Reservation:New");
            CustomerReservationUpdate = templates.FirstOrDefault(w => w.Name == "Customer:Reservation:Update");
            CustomerReservationForwarding = templates.FirstOrDefault(w => w.Name == "Customer:Reservation:Forwarding");
            CustomerReservationCancellation = templates.FirstOrDefault(w => w.Name == "Customer:Reservation:Cancellation");
        }
        protected async Task FormSubmit()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar las configuraciones del proveedor de correo?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await EmailNotificationProviderSettingsService.UpdateAsync(SettingsKey, Provider);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Proveedor de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Configuraciones del proveedor de correo han sido actualizadas correctamente.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar el proveedor de correo.",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailProviderSettings();
            }
        }

        protected async Task UpdateCustomerOrderNew()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Creación del pedido\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerOrderNew.NotificationTemplateId, CustomerOrderNew);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Creación del pedido\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerOrderNew));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Creación del pedido\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerOrderUpdate()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Actualización del pedido\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerOrderUpdate.NotificationTemplateId, CustomerOrderUpdate);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Actualización del pedido\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerOrderUpdate));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Actualización del pedido\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerOrderForwarding()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Reenvío del pedido\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerOrderForwarding.NotificationTemplateId, CustomerOrderForwarding);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Reenvío del pedido\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerOrderForwarding));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Reenvío del pedido\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerOrderCancellation()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Cancelación del pedido\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerOrderCancellation.NotificationTemplateId, CustomerOrderCancellation);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Cancelación del pedido\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerOrderCancellation));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Cancelación del pedido\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }

        protected async Task UpdateCustomerReservationNew()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Confirmación de la reserva\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerReservationNew.NotificationTemplateId, CustomerReservationNew);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Confirmación de la reserva\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerReservationNew));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Confirmación de la reserva\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerReservationUpdate()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Actualizacion de la reserva\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerReservationUpdate.NotificationTemplateId, CustomerReservationUpdate);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Actualizacion de la reserva\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerReservationUpdate));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Actualizacion de la reserva\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerReservationForwarding()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Reenvío de la reserva\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerReservationForwarding.NotificationTemplateId, CustomerReservationForwarding);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Reenvío de la reserva\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerReservationForwarding));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Reenvío de la reserva\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        protected async Task UpdateCustomerReservationCancellation()
        {
            try
            {
                if (await DialogService.Confirm("Está seguro que desea actualizar la plantilla para \"Cancelación de la reserva\" ?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar actualización") == true)
                {
                    IsSubmitInProgress = true;
                    await NotificationTemplateService.UpdateAsync(CustomerReservationCancellation.NotificationTemplateId, CustomerReservationCancellation);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Plantilla de correo",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Plantilla de correo para \"Cancelación de la reserva\" ha sido actualizada.",
                        Duration = 6000
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(UpdateCustomerReservationForwarding));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido actualizar la plantilla para \"Reenvío de la reserva\".",
                    Duration = 6000
                });
            }
            finally
            {
                IsSubmitInProgress = false;
                await GetEmailTemplates();
            }
        }
        #endregion
    }
}
