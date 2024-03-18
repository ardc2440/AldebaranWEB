namespace Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel
{
    public class ProviderReferencesViewModel
    {
        public List<Provider> Providers { get; set; }
        public class Provider
        {
            public string ProviderCode { get; set; }
            public string ProviderName { get; set; }
            public string ProviderAddress { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
            public string ContactPerson { get; set; }
            public List<Line> Lines { get; set; }
        }

        public class Line
        {
            public string LineCode { get; set; }
            public string LineName { get; set; }
            public List<Item> Items { get; set; }
        }
        public class Item
        {
            public string InternalReference { get; set; }
            public string ItemName { get; set; }
            public List<Reference> References { get; set; }
        }
        public class Reference
        {
            public string ReferenceCode { get; set; }
            public string ReferenceName { get; set; }
            public string ProviderReferenceName { get; set; }
            public int ConfirmedAmount { get; set; }
            public int ReservedAmount { get; set; }
            public int AvailableAmount { get; set; }
        }
    }
}
