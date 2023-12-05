using Microsoft.Extensions.Localization;

namespace Aldebaran.Web.Resources
{
    public interface ISharedStringLocalizer
    {
        public LocalizedString this[string key]
        {
            get;
        }

        LocalizedString GetLocalizedString(string key);
    }
}
