namespace Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available.ViewModel
{
    public class FreezoneVsAvailableViewModel
    {
        public List<Line> Lines { get; set; }

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
            public int AvailableAmount { get; set; }
            public int FreeZone { get; set; }
            public int Difference => AvailableAmount - FreeZone;
        }
    }
}
