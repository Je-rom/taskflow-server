using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace taskflow.Models.DTO.Request;

public class ResetPasswordRequestDto
{
    
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Username { get; set; }
    
    [Required]
    public string Token { set; get; }
    
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    
}