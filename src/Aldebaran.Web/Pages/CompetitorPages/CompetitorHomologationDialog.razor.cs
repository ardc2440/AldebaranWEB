using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Competitors;
using Aldebaran.Application.Services.Models.Competitors;
using Aldebaran.Web.Services.Competitors;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.CompetitorPages
{
    /// <summary>
    /// Crear / editar homologación. Artículo con búsqueda paginada; la referencia es opcional (sin referencia = nivel artículo)
    /// y se limpia al cambiar el artículo. Solo se ofrecen compañías activas (y la actual al editar). Devuelve true al guardar.
    /// </summary>
    public partial class CompetitorHomologationDialog : ComponentBase
    {
        #region Injections
        [Inject]
        protected ILogger<CompetitorHomologationDialog> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICompetitorHomologationService CompetitorHomologationService { get; set; }

        [Inject]
        protected ICompetitorCompanyService CompetitorCompanyService { get; set; }

        [Inject]
        protected IItemService ItemService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        #endregion

        #region Parameters
        /// <summary>null = nueva homologación.</summary>
        [Parameter]
        public int? CompetitorHomologationId { get; set; }

        /// <summary>Compañía preseleccionada al crear (filtro vigente del listado).</summary>
        [Parameter]
        public int? DefaultCompetitorCompanyId { get; set; }
        #endregion

        #region Variables
        protected CompetitorHomologationInput Input = new();
        protected IReadOnlyList<CompetitorCompany> Companies = Array.Empty<CompetitorCompany>();
        protected List<ItemOption> Items = new();
        protected int ItemCount;
        protected int? SelectedItemId;
        protected List<ReferenceOption> References = new();
        protected List<string> Errors = new();
        protected bool IsLoading = true;
        protected bool IsSaving;
        private ItemOption? _currentItem;

        /// <summary>Imagen elegida y aún no guardada (se sube al pulsar Guardar).</summary>
        private PendingImage? _pendingImage;
        private string? _savedImageFileName;
        private bool _removeSavedImage;
        /// <summary>Vista previa: la imagen nueva (data URL) o la guardada.</summary>
        protected string? PreviewUrl;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var companies = (await CompetitorCompanyService.GetAsync()).ToList();
                if (CompetitorHomologationId != null)
                    await LoadHomologationAsync(CompetitorHomologationId.Value, companies);
                else if (DefaultCompetitorCompanyId != null && companies.Any(c => c.CompetitorCompanyId == DefaultCompetitorCompanyId))
                    Input.CompetitorCompanyId = DefaultCompetitorCompanyId.Value;
                Companies = companies;
                await LoadItemsAsync(new LoadDataArgs { Skip = 0, Top = 10 });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadHomologationAsync(int competitorHomologationId, List<CompetitorCompany> companies)
        {
            var h = await CompetitorHomologationService.FindAsync(competitorHomologationId)
                    ?? throw new KeyNotFoundException($"Homologación con id {competitorHomologationId} no existe.");
            Input = new CompetitorHomologationInput
            {
                CompetitorCompanyId = h.CompetitorCompanyId,
                ItemId = h.ItemId,
                ReferenceId = h.ReferenceId,
                CompetitorReference = h.CompetitorReference,
                CompetitorProductName = h.CompetitorProductName,
                ProductUrl = h.ProductUrl,
                CompetitorPrice = h.CompetitorPrice
            };
            // La compañía actual se ofrece aunque esté inactiva, para poder editar la homologación sin cambiarla.
            if (companies.All(c => c.CompetitorCompanyId != h.CompetitorCompanyId) && await CompetitorCompanyService.FindAsync(h.CompetitorCompanyId) is { } current)
                companies.Insert(0, current with { CompanyName = $"{current.CompanyName} (inactiva)" });
            _savedImageFileName = h.ImageFileName;
            PreviewUrl = CompetitorImageStorage.Url(h.ImageFileName);
            SelectedItemId = h.ItemId;
            _currentItem = new ItemOption(h.ItemId, h.InternalReference, h.ItemName);
            await LoadReferencesAsync();
        }

        #region Artículo y referencia
        protected async Task LoadItemsAsync(LoadDataArgs args)
        {
            var skip = args.Skip ?? 0;
            var top = args.Top ?? 10;
            var (items, count) = string.IsNullOrWhiteSpace(args.Filter)
                ? await ItemService.GetAsync(skip, top)
                : await ItemService.GetAsync(skip, top, args.Filter);
            Items = items.Select(i => new ItemOption(i.ItemId, i.InternalReference, i.ItemName)).ToList();
            // El artículo elegido debe estar en la lista para que el combo muestre su nombre.
            if (_currentItem != null && Items.All(i => i.ItemId != _currentItem.ItemId))
                Items.Insert(0, _currentItem);
            ItemCount = count;
        }

        protected async Task OnItemChangedAsync(int? itemId)
        {
            SelectedItemId = itemId;
            Input.ItemId = itemId ?? 0;
            Input.ReferenceId = null;
            _currentItem = itemId == null ? null : Items.FirstOrDefault(i => i.ItemId == itemId);
            await LoadReferencesAsync();
        }

        private async Task LoadReferencesAsync() =>
            References = SelectedItemId == null
                ? new List<ReferenceOption>()
                : (await ItemReferenceService.GetByItemIdAsync(SelectedItemId.Value))
                    .OrderBy(r => r.ReferenceName)
                    .Select(r => new ReferenceOption(r.ReferenceId, $"{r.ReferenceCode} - {r.ReferenceName}{(r.IsActive ? string.Empty : " (inactiva)")}"))
                    .ToList();

        protected sealed record ItemOption(int ItemId, string InternalReference, string ItemName)
        {
            public string FullName => $"[{InternalReference}] {ItemName}";
        }

        protected sealed record ReferenceOption(int ReferenceId, string FullName);
        #endregion

        #region Imagen
        protected async Task OnImageSelectedAsync(InputFileChangeEventArgs args)
        {
            Errors = new List<string>();
            var file = args.File;
            var error = CompetitorImageRules.Validate(file.Name, file.Size);
            if (error != null)
            {
                Errors.Add(error);
                return;
            }
            await using var stream = file.OpenReadStream(CompetitorImageRules.MaxBytes);
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer);
            _pendingImage = new PendingImage(file.Name, buffer.ToArray(), file.ContentType);
            _removeSavedImage = false;
            PreviewUrl = $"data:{_pendingImage.ContentType};base64,{Convert.ToBase64String(_pendingImage.Content)}";
        }

        protected void RemoveImage()
        {
            _pendingImage = null;
            _removeSavedImage = _savedImageFileName != null;
            PreviewUrl = null;
        }

        /// <summary>Aplica la imagen tras guardar los datos: sube la nueva o quita la guardada.</summary>
        private async Task ApplyImageAsync(int competitorHomologationId)
        {
            if (_pendingImage != null)
            {
                using var content = new MemoryStream(_pendingImage.Content);
                await CompetitorHomologationService.SetImageAsync(competitorHomologationId, content, _pendingImage.FileName, _pendingImage.Content.LongLength);
            }
            else if (_removeSavedImage)
                await CompetitorHomologationService.RemoveImageAsync(competitorHomologationId);
        }

        private sealed record PendingImage(string FileName, byte[] Content, string ContentType);
        #endregion

        #region Guardar
        protected async Task SaveAsync()
        {
            Errors = new List<string>();
            try
            {
                IsSaving = true;
                if (CompetitorHomologationId == null)
                    CompetitorHomologationId = await CompetitorHomologationService.AddAsync(Input);
                else
                    await CompetitorHomologationService.UpdateAsync(CompetitorHomologationId.Value, Input);
                await ApplyImageAsync(CompetitorHomologationId.Value);
                DialogService.Close(true);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or KeyNotFoundException)
            {
                Errors = ex.Message.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error guardando la homologación con la competencia");
                Errors = new List<string> { "No se pudo guardar la homologación. Intente de nuevo." };
            }
            finally
            {
                IsSaving = false;
            }
        }

        protected void Cancel() => DialogService.Close(null);
        #endregion
    }
}
