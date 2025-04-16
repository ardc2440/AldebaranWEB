namespace Aldebaran.Web.Pages.ReportPages.Automatic_Customer_Order_In_Process_Creation.ViewModel
{
    public class AutomaticAssigmentViewModel
    {
        public List <Document> Documents { get; set; }

        public class Document
        {
            public string DocumentType { get; set; }
            public int DocumentId { get; set; }
            public string DocumentNumber { get; set; }
            public string ProviderIdentity { get; set; }
            public string ProviderName { get; set; }
            public string ProformaNumber { get; set; }
            public string ImportNumber { get; set; }
            public DateTime ReceiptDate { get; set; }
            public DateTime ConfirmationDate { get; set; }
            public List<CustomerOrder> CustomerOrders { get; set; }
        }

        public class CustomerOrder
        {
            public int CustomerOrderId { get; set; }
            public string CustomerOrderNumber { get; set; }
            public string CustomerIdentity { get; set; }
            public string CustomerName { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime EstimatedDeliveryDate { get; set; }
            public string StatusOrderName { get; set; }
            public List<CustomerOrderArticle> CustomerOrderArticles { get; set; }

        }

        public class CustomerOrderArticle
        {
            public int ItemId { get; set; }
            public string InternalReference { get; set; }
            public string ItemName { get; set; }

            public List<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        }

        public class CustomerOrderDetail
        {
            public string ReferenceName { get; set; }
            public int Requested { get; set; }
            public int Assigned { get; set; }
            public int Pending { get; set; }
        }
    }
}
