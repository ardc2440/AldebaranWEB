using Aldebaran.DataAccess;

namespace Aldebaran.Application.FileWritingService.Settings
{
    public class ContextConfiguration : IContextConfiguration
    {
        public bool TrackEnabled => true;

        public string TrackUser
        {
            get
            {
                return "System";
            }
        }
    }
}