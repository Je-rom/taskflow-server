namespace taskflow.Models.Domain;

public class Workspace
{
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    
    public List<WorkspaceMember> WorkspaceMembers { get; set; }
    public List<Project> Projects { get; set; }
    
    // Constructor to set default values
    public Workspace()
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