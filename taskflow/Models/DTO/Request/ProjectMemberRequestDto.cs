using System.ComponentModel.DataAnnotations;
using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Request
{
    public class ProjectMemberRequestDto {

        [Required]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format.")]
        public string WorkspaceId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Minimum of 1 required")]
        public string[] UserIds { get; set; }
        
        public Guid? GetGuid()
        {
            if (Guid.TryParse(WorkspaceId, out Guid resultGuid))
            {
                return resultGuid;
            }
            return null;
        }

    }
}