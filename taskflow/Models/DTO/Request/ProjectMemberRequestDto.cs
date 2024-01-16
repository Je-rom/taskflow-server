using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Request
{
    public class ProjectMemberRequestDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid WorkspaceMemberId { get; set; }

    }
}