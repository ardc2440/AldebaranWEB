using Aldebaran.Application.Services.Models.TrackLogs;
using Aldebaran.Application.Services.TrackLogs;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.LogPages
{
    /// <summary>
    /// Consulta del Track (log.track). Se busca por lo que conoce el usuario (artículo, documento, tercero, empleado)
    /// y/o por entidad; el detalle muestra campo / anterior / nuevo.
    /// </summary>
    public partial class TrackLog
    {
        private const int DefaultDays = 30;

        #region Injections
        [Inject] protected ILogger<TrackLog> Logger { get; set; }
        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected ITrackLogService TrackLogService { get; set; }
        #endregion

        #region Variables
        protected sealed record ActionOption(string Text, TrackAction Value);

        protected readonly IReadOnlyList<ActionOption> actions = new[]
        {
            new ActionOption("Creación", TrackAction.Created),
            new ActionOption("Modificación", TrackAction.Modified),
            new ActionOption("Eliminación", TrackAction.Deleted)
        };

        protected IReadOnlyList<TrackEntity> entities = Array.Empty<TrackEntity>();
        protected IReadOnlyList<TrackObjectType> objectTypes = Array.Empty<TrackObjectType>();
        protected IReadOnlyList<TrackObjectOption> objectOptions = Array.Empty<TrackObjectOption>();
        protected IEnumerable<TrackEntry> entries;
        protected LocalizedDataGrid<TrackEntry> entriesGrid;
        protected byte? selectedObjectType;
        protected int? selectedObjectId;
        protected string objectSearchText;
        protected string selectedEntity;
        protected TrackAction? selectedAction;
        protected DateTime? dateFrom = DateTime.Today.AddDays(-DefaultDays);
        protected DateTime? dateTo = DateTime.Today;
        protected string modifierName;
        protected string infoMessage;
        protected bool isLoading;
        protected int count;
        private int skip;
        private int top = 20;
        #endregion

        #region Presentation
        private TrackObjectType SelectedType => objectTypes.FirstOrDefault(t => t.ObjectType == selectedObjectType);

        protected string ObjectSearchTitle => SelectedType == null ? "Texto a buscar" : $"{SelectedType.Hint} + Enter";

        protected string ObjectFieldTitle => SelectedType == null ? "Resultado" : $"{SelectedType.Name} encontrado";

        protected string EntityFieldTitle => selectedObjectId == null ? "Entidad" : "Entidad (opcional: todas)";

        public static string ActionText(TrackAction action) => action switch
        {
            TrackAction.Created => "Creación",
            TrackAction.Deleted => "Eliminación",
            _ => "Modificación"
        };
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            entities = await TrackLogService.GetEntitiesAsync();
            objectTypes = await TrackLogService.GetObjectTypesAsync();
        }
        #endregion

        #region Events
        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip ?? 0;
            top = args.Top ?? top;
            await GetEntriesAsync();
        }

        /// <summary>Busca los objetos que coinciden con lo escrito; si hay uno solo, lo selecciona y consulta.</summary>
        protected async Task SearchObjectsAsync()
        {
            selectedObjectId = null;
            try
            {
                objectOptions = selectedObjectType == null
                    ? Array.Empty<TrackObjectOption>()
                    : await TrackLogService.SearchObjectsAsync(selectedObjectType.Value, objectSearchText);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(SearchObjectsAsync));
                objectOptions = Array.Empty<TrackObjectOption>();
            }

            if (objectOptions.Count == 1)
                selectedObjectId = objectOptions[0].ObjectId;

            await ApplyFiltersAsync();
        }

        protected async Task ObjectTypeChangedAsync()
        {
            selectedObjectId = null;
            objectSearchText = null;
            objectOptions = Array.Empty<TrackObjectOption>();
            await ApplyFiltersAsync();
        }

        protected async Task ApplyFiltersAsync()
        {
            skip = 0;
            await entriesGrid.FirstPage(true);
        }

        protected async Task ShowDetailAsync(TrackEntry entry)
        {
            await DialogService.OpenAsync<TrackDetailDialog>($"{entry.EntityDisplayName} - {ActionText(entry.Action)}",
                new Dictionary<string, object> { { "Entry", entry } },
                new DialogOptions { CloseDialogOnOverlayClick = true, Width = "850px" });
        }
        #endregion

        #region Private
        private TrackObjectRef SelectedObject =>
            selectedObjectType != null && selectedObjectId != null ? new TrackObjectRef(selectedObjectType.Value, selectedObjectId.Value) : null;

        private string ValidateFilters()
        {
            if (string.IsNullOrEmpty(selectedEntity) && SelectedObject == null)
                return "Elija qué buscar (artículo, documento, cliente, ...) o la entidad que desea consultar.";
            if (dateFrom == null || dateTo == null)
                return "El rango de fechas es obligatorio.";
            if (dateFrom > dateTo)
                return "La fecha inicial no puede ser mayor que la final.";
            return null;
        }

        private async Task GetEntriesAsync()
        {
            infoMessage = ValidateFilters();
            if (infoMessage != null)
            {
                entries = Array.Empty<TrackEntry>();
                count = 0;
                return;
            }

            try
            {
                isLoading = true;
                var filter = new TrackLogFilter(dateFrom!.Value, dateTo!.Value, selectedEntity, SelectedObject, selectedAction, modifierName);
                var page = await TrackLogService.GetEntriesAsync(filter, skip, top);
                entries = page.Items;
                count = page.TotalCount;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(GetEntriesAsync));
                infoMessage = "No fue posible consultar el Track.";
            }
            finally
            {
                isLoading = false;
            }
        }
        #endregion
    }
}
