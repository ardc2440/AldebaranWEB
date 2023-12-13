namespace Aldebaran.DataAccess.Entities
{
    public class Currency
    {
        public short CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        // Reverse navigation
        public ICollection<Item> Items { get; set; }
        public Currency()
        {
            Items = new List<Item>();
        }
    }
}
