using AutoMapper;
using Slotly.Entities;
using Slotly.Models.Appointment;
using Slotly.Models.Business;
using Slotly.Models.Staff;
using Slotly.Models.User;

namespace Slotly.AutoMapper
{
    public static class MappingProfile {
        public static void Configuration(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Business, BusinessReadDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.Name));


            cfg.CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role.ToString()));

            cfg.CreateMap<BusinessService, BusinessServiceDto>()
                .ForMember(dest => dest.ServiceName,
                    opt => opt.MapFrom(src => src.Service.Name));

            cfg.CreateMap<CreateStaffDto, Staff>();
            
            cfg.CreateMap<Staff, ReadStaffDto>();

            cfg.CreateMap<StaffService, StaffServiceDto>()
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.Staff.Id))
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Staff.Name))
                .ForMember(dest => dest.Position,
                opt => opt.MapFrom(src => src.Staff.Position))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.BusinessService.Service.Name));

            cfg.CreateMap<Appointment, GetAppointmentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


        }
    }
}
