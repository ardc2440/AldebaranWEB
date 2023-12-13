namespace Aldebaran.DataAccess.Entities
{
    public class ShipmentMethod
    {
        public short ShipmentMethodId { get; set; }
        public string ShipmentMethodName { get; set; }
        public string ShipmentMethodNotes { get; set; }
        // Reverse navigation
        public ICollection<ShipmentForwarderAgentMethod> ShipmentForwarderAgentMethods { get; set; }
        public ShipmentMethod()
        {
            ShipmentForwarderAgentMethods = new List<ShipmentForwarderAgentMethod>();
        }
    }
}
