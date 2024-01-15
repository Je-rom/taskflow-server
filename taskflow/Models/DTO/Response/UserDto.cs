using taskflow.Models.Domain;

namespace taskflow.Models.DTO.Response;

public class UserDto
{
    public Guid Id { get; set; }
   
    public string LastName { get; set; }
    
    public string FirstName { get; set; }
    
    public string Email { get; set; }
    
    public string UserName { get; set; }
    
    public string EmailConfirmed { get; set; }
    
    //public ICollection<Workspace> Workspaces { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}