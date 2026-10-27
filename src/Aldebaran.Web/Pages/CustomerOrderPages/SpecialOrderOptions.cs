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

        public const string ConfirmTitle = "Confirmar pedido especial";

        public static string CreationConfirmMessage(bool isSpecialOrder) => isSpecialOrder
            ? "¿Confirma que este pedido <strong>SÍ</strong> es un pedido especial y desea crearlo?"
            : "¿Confirma que este pedido <strong>NO</strong> es un pedido especial y desea crearlo?";

        public static ConfirmOptions ChangeConfirmOptions => new() { OkButtonText = "Sí, cambiar", CancelButtonText = "No, modificar" };

        public static string ChangeConfirmMessage(bool from, bool to) =>
            $"Pedido especial cambia de <strong>{ToText(from)}</strong> a <strong>{ToText(to)}</strong>. ¿Confirma el cambio?";

        private static string ToText(bool value) => value ? "SÍ" : "NO";
    }
}
