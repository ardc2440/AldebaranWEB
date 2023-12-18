using Aldebaran.Web.Models.ViewModels;
using AutoMapper;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services.Mappings
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<EmployeeViewModel, ServiceModel.Employee>().ReverseMap();
        }
    }
}
