using System.ComponentModel.DataAnnotations;

namespace taskflow.Models.DTO.Request;

public class ForgotPasswordRequestDto
{
    
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
}