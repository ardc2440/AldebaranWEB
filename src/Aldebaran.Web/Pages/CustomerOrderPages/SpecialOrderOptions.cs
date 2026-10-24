using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public record SpecialOrderOption(bool Value, string Text);

    public static class SpecialOrderOptions
    {
        public static readonly IEnumerable<SpecialOrderOption> Items = new[]
        {
            new SpecialOrderOption(true, "Sí"),
            new SpecialOrderOption(false, "No")
        };

        public static ConfirmOptions CreationConfirmOptions => new() { OkButtonText = "Sí, crear", CancelButtonText = "No, modificar" };

        public const string CreationConfirmTitle = "Confirmar pedido especial";

        public static string CreationConfirmMessage(bool isSpecialOrder) => isSpecialOrder
            ? "Pedido especial: SÍ. ¿Confirma que este pedido ES un pedido especial y desea crearlo?"
            : "Pedido especial: NO. ¿Confirma que este pedido NO es un pedido especial y desea crearlo?";
    }
}
