using System.ComponentModel.DataAnnotations;
using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Request
{
    public class ProjectMemberRequestDto {

        [Required]
        public Guid WorkspaceId { get; set; }

        [MinLength(1)]
        public List<Guid> UserId { get; set; }

    }
}