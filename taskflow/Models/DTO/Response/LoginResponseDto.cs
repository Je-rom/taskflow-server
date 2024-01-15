using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Response
{
    public class LoginResponseDto
    {
        public required string  JwtToken { get; set; }
        
        public UserDto User { get; set; }
        
    }
}

