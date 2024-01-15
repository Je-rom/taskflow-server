using Microsoft.AspNetCore.Identity;

namespace taskflow.Models.Domain;

public class User : IdentityUser
{
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Workspace> Workspaces { get; set; }
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; }
    public ICollection<ProjectMember> ProjectMembers { get; set; }
    //public ICollection<Project> Project { get; set; }

    // Constructor to set default values
    public User()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Method to update the UpdatedAt field
    public void UpdateUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
}