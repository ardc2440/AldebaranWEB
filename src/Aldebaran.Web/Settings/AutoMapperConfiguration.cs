using Aldebaran.Application.Services.Mappings;
using AutoMapper;

namespace Aldebaran.Web.Settings
{
    public class AutoMapperConfiguration
    {
        public static IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ApplicationServicesProfile>();
                cfg.AddProfile<ViewModelProfile>();
            });
            return config.CreateMapper();
        }
    }
}
