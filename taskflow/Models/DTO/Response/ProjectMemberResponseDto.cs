using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Response
{
    public class ProjectMemberResponseDto
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserDto User { get; set; }

    }
}
