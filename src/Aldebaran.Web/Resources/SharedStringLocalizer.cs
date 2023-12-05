using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Aldebaran.Web.Resources
{
    public class SharedStringLocalizer : ISharedStringLocalizer
    {
        private readonly IStringLocalizer _localizer;
        public SharedStringLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("Resources.SharedResource", $"{assemblyName.Name}");
        }
        public LocalizedString this[string key] => _localizer[key];
        public LocalizedString GetLocalizedString(string key)
        {
            return _localizer[key];
        }
        public class SharedResource { }
    }
}
