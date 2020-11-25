using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalService.Dto;
using TechnicalService.Models;

namespace TechnicalService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Works, WorkDto>();
            CreateMap<WorkDto, Works>();
            CreateMap<UserDto, Users>();
            CreateMap<Users, UserDto>();
        }
    }
}
