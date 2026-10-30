using Aldebaran.Application.Services.Models;
using Xunit;

namespace Aldebaran.Application.Services.Tests.Alarms
{
    public class VisualizedPurchaseOrderTransitAlarmTests
    {
        [Fact]
        public void Nueva_TraeFechaYHoraActual()
        {
            var before = DateTime.Now;

            var visualized = new VisualizedPurchaseOrderTransitAlarm { PurchaseOrderTransitAlarmId = 1, EmployeeId = 1 };

            Assert.InRange(visualized.VisualizedDate, before, DateTime.Now);
        }
    }
}
