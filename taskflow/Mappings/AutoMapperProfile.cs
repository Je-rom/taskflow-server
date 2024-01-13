using AutoMapper;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Response;

namespace taskflow.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, ApplicationUserDto>().ReverseMap();
            CreateMap<Workspace, WorkspaceResponseDto>().ReverseMap();
            CreateMap<Project,ProjectResponseDto>().ReverseMap();
        }
    }
}

