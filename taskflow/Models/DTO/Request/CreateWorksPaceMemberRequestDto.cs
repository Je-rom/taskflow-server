using System.ComponentModel.DataAnnotations;

namespace taskflow.Models.DTO.Request;

public class CreateWorksPaceMemberRequestDto
{
    [Required]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", ErrorMessage = "Invalid GUID format.")]
    public string UserId { get; set; }
    
    
    public Guid? GetGuid()
    {
        if (Guid.TryParse(UserId, out Guid resultGuid))
        {
            return resultGuid;
        }
        return null;
    }
}