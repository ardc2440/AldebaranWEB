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
    }
}
