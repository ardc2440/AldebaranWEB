using Aldebaran.Application.Services.InternalReferences;
using Aldebaran.Application.Services.Models.InternalReferences;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class InternalReferenceConsecutives
    {
        #region Injections

        [Inject]
        protected ILogger<InternalReferenceConsecutives> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected IInternalReferenceConsecutiveService InternalReferenceConsecutiveService { get; set; }

        #endregion

        #region Variables

        protected string code;
        protected int quantity = 1;
        protected IReadOnlyList<InternalReferenceConsecutive> consecutives;
        protected string resultTitle;
        protected string errorMessage;
        protected bool isBusy;

        #endregion

        #region Events

        /// <summary>Consecutivos generados para el código que aún no usa ningún artículo.</summary>
        protected async Task ConsultAsync()
        {
            var normalized = InternalReferenceCodeRules.Normalize(code);
            await RunAsync(async () =>
            {
                consecutives = await InternalReferenceConsecutiveService.GetAvailableAsync(normalized);
                resultTitle = consecutives.Count == 0
                    ? $"No hay consecutivos disponibles para el código {normalized}."
                    : $"Consecutivos disponibles para el código {normalized}: {consecutives.Count}";
            });
        }

        /// <summary>Genera la cantidad indicada, previa confirmación, y muestra los generados.</summary>
        protected async Task GenerateAsync()
        {
            var normalized = InternalReferenceCodeRules.Normalize(code);
            var error = InternalReferenceCodeRules.ValidateCode(normalized) ?? InternalReferenceCodeRules.ValidateQuantity(quantity);
            if (error != null)
            {
                errorMessage = error;
                return;
            }

            if (await DialogService.Confirm($"¿Está seguro que desea generar {quantity} consecutivo(s) para el código {normalized}?", "Confirmar generación",
                    new ConfirmOptions { OkButtonText = "Sí, generar", CancelButtonText = "No" }) != true)
                return;

            await RunAsync(async () =>
            {
                consecutives = await InternalReferenceConsecutiveService.GenerateAsync(normalized, quantity);
                resultTitle = $"Consecutivos generados para el código {normalized}: {consecutives.Count}";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Consecutivos de referencia interna",
                    Detail = $"Se generaron {consecutives.Count} consecutivo(s): {consecutives.First().InternalReference} a {consecutives.Last().InternalReference}.",
                    Duration = 6000
                });
            });
        }

        /// <summary>Ejecuta la acción mostrando validaciones y errores en la alerta de la página.</summary>
        private async Task RunAsync(Func<Task> action)
        {
            try
            {
                isBusy = true;
                errorMessage = null;
                code = InternalReferenceCodeRules.Normalize(code);
                await action();
            }
            catch (ArgumentException ex)
            {
                errorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(InternalReferenceConsecutives));
                errorMessage = "No se pudo completar la operación. Intente nuevamente.";
            }
            finally
            {
                isBusy = false;
            }
        }

        #endregion
    }
}
