using Microsoft.Extensions.Localization;

namespace Aldebaran.Infraestructure.Common.Utils
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
