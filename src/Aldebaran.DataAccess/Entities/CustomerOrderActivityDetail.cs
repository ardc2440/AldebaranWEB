using Aldebaran.DataAccess.Core;

namespace Aldebaran.DataAccess.Entities
{
    public class CustomerOrderActivityDetail : ITrackeable
    {
        public int CustomerOrderActivityDetailId { get; set; }
        public int CustomerOrderActivityId { get; set; }
        public short ActivityTypeId { get; set; }
        public int EmployeeId { get; set; }
        public int ActivityEmployeeId { get; set; }
        public ActivityType ActivityType { get; set; } = null!;
        public CustomerOrderActivity CustomerOrderActivity { get; set; } = null!;
        public Employee ActivityEmployee { get; set; } = null!;
        public Employee Employee_EmployeeId { get; set; } = null!;
    }
}
