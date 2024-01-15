using AutoMapper;
using taskflow.Models.Domain;
using taskflow.Models.DTO.Response;

namespace taskflow.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Workspace, WorkspaceResponseDto>().ReverseMap();
            CreateMap<Project, ProjectResponseDto>().ReverseMap();
            CreateMap<WorkspaceMember, WorkspaceMemberResponseDto>().ReverseMap();
        }
    }
}

