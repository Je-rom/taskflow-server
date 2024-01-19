using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Response
{
    public class WorkspaceResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public UserDto User { get; set; }
        public ICollection<WorkspaceMemberResponseDto> WorkspaceMembers { get; set; }
        public ICollection<ProjectResponseDto> Projects { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

