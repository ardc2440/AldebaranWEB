using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared.ViewModel;
namespace Aldebaran.Web.Pages.ReportPages.Reference_Movement.ViewModel
{
    public class ReferenceMovementFilter
    {
        public DateRange MovementDate { get; set; } = new();
        public List<ItemReference> ItemReferences { get; set; } = new List<ItemReference>();
        public bool LockReferenceSelection { get; set; } = false;
        public bool AllMovementCheckVisible { get; set; } = false;
        public bool ShowAllMovement { get; set; } = false;
        public bool AllRequiredFieldsCompleted => MovementDate.StartDate != null && MovementDate.EndDate != null && ItemReferences.Any();
        /// <summary>Artículo activo (Sí por defecto, como el reporte mostraba antes): true = Sí, false = No, null = Todos.</summary>
        public bool? IsItemActive { get; set; } = true;
        /// <summary>Referencia activa (Sí por defecto): true = Sí, false = No, null = Todos.</summary>
        public bool? IsReferenceActive { get; set; } = true;
        /// <summary>Precio bomba: true = Sí, false = No, null = Todos.</summary>
        public bool? BombPrice { get; set; }


    }
}
