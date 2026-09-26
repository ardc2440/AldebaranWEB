namespace Aldebaran.Web.Shared
{
    /// <summary>
    /// Extiende MultiReferencePicker SIN modificarlo (hereda su markup y comportamiento) para
    /// exponer la selección en los tres niveles de la jerarquía: Líneas, Artículos y Referencias.
    /// Permite filtros donde la referencia no es obligatoria (ej. reporte de Artículos y Referencias).
    /// </summary>
    public class HierarchicalReferencePicker : MultiReferencePicker
    {
        public IReadOnlyList<short> GetSelectedLineIds() => SelectedLineIds?.ToList() ?? new List<short>();

        public IReadOnlyList<int> GetSelectedItemIds() => SelectedItemIds?.ToList() ?? new List<int>();

        public IReadOnlyList<int> GetSelectedReferenceIds() => SelectedReferenceIds?.ToList() ?? new List<int>();

        /// <summary>
        /// Restaura una selección previa en cualquier nivel, respetando la jerarquía
        /// (cada nivel se aplica después de cargar las opciones del nivel superior).
        /// Descarta los valores que no pertenecen al nivel superior o que ya no están disponibles,
        /// porque la lógica base asume selecciones consistentes.
        /// </summary>
        public void RestoreSelection(IEnumerable<short> lineIds, IEnumerable<int> itemIds, IEnumerable<int> referenceIds)
        {
            SelectedLineIds = KeepAvailableLines(lineIds);
            OnLineChange();
            SelectedItemIds = KeepItemsOfSelectedLines(itemIds);
            OnItemChange();
            SelectedReferenceIds = KeepReferencesOfSelectedItems(referenceIds);
            OnReferenceChange();
            StateHasChanged();
        }

        private List<short> KeepAvailableLines(IEnumerable<short> lineIds) =>
            Available().Select(r => r.Item.LineId).Distinct().Intersect(lineIds ?? Enumerable.Empty<short>()).ToList();

        private List<int> KeepItemsOfSelectedLines(IEnumerable<int> itemIds) =>
            Available().Where(r => SelectedLineIds.Contains(r.Item.LineId)).Select(r => r.ItemId).Distinct()
                       .Intersect(itemIds ?? Enumerable.Empty<int>()).ToList();

        private List<int> KeepReferencesOfSelectedItems(IEnumerable<int> referenceIds) =>
            Available().Where(r => SelectedItemIds.Contains(r.ItemId)).Select(r => r.ReferenceId)
                       .Intersect(referenceIds ?? Enumerable.Empty<int>()).ToList();

        private IEnumerable<global::Aldebaran.Application.Services.Models.ItemReference> Available() =>
            AvailableItemReferencesForSelection ?? Enumerable.Empty<global::Aldebaran.Application.Services.Models.ItemReference>();
    }
}
