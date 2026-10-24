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

        protected string referenceCode;
        protected int quantity = 1;
        protected IReadOnlyList<InternalReferenceConsecutive> consecutives;
        protected IReadOnlyList<InternalReferenceNonconforming> nonconforming;
        protected string resultTitle;
        protected string errorMessage;
        protected bool isBusy;

        #endregion

        #region Events

        /// <summary>Consecutivos generados para el código que aún no usa ningún artículo.</summary>
        protected async Task ConsultAsync()
        {
            var normalized = InternalReferenceCodeRules.Normalize(referenceCode);
            await RunAsync(async () =>
            {
                consecutives = await InternalReferenceConsecutiveService.GetAvailableAsync(normalized);
                nonconforming = await InternalReferenceConsecutiveService.GetNonconformingAsync(normalized);
                resultTitle = consecutives.Count == 0
                    ? $"No hay consecutivos disponibles para el código {normalized}."
                    : $"Consecutivos disponibles para el código {normalized}: {consecutives.Count}";
            });
        }

        /// <summary>Completa hasta la cantidad requerida de disponibles (crea solo los que faltan), previa confirmación.</summary>
        protected async Task GenerateAsync()
        {
            var normalized = InternalReferenceCodeRules.Normalize(referenceCode);
            var error = InternalReferenceCodeRules.ValidateCode(normalized) ?? InternalReferenceCodeRules.ValidateQuantity(quantity);
            if (error != null)
            {
                errorMessage = error;
                return;
            }

            if (await DialogService.Confirm($"¿Está seguro que desea completar {quantity} consecutivo(s) disponibles para el código {normalized}? Solo se generan los que falten.", "Confirmar generación",
                    new ConfirmOptions { OkButtonText = "Sí, generar", CancelButtonText = "No" }) != true)
                return;

            await RunAsync(async () =>
            {
                consecutives = await InternalReferenceConsecutiveService.GenerateAsync(normalized, quantity);
                nonconforming = await InternalReferenceConsecutiveService.GetNonconformingAsync(normalized);
                var created = consecutives.Where(c => c.IsNew).ToList();
                resultTitle = $"Consecutivos disponibles para el código {normalized}: {consecutives.Count} (generados ahora: {created.Count})";
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = created.Count > 0 ? NotificationSeverity.Success : NotificationSeverity.Info,
                    Summary = "Consecutivos de referencia interna",
                    Detail = created.Count > 0
                        ? $"Se generaron {created.Count} consecutivo(s): {created.First().InternalReference} a {created.Last().InternalReference}."
                        : $"Ya existían {consecutives.Count} consecutivo(s) disponibles; no fue necesario generar.",
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
                referenceCode = InternalReferenceCodeRules.Normalize(referenceCode);
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
