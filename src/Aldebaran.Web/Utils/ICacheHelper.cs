namespace Aldebaran.Web.Utils
{
    public interface ICacheHelper
    {
        Task<List<T>> GetCache<T>(string key) where T : class;
        Task UpdateCache<T>(string key, List<T> list) where T : class;
    }
}
