namespace Aldebaran.DataAccess
{
    public interface IContextConfiguration
    {
        public bool TrackEnabled { get; }
        public string TrackUser { get; }
    }
}