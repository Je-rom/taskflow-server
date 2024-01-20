using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Response
{
    public class ProjectTaskResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Stage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProjectMemberResponseDto ProjectMember { get; set; }
    }
}
