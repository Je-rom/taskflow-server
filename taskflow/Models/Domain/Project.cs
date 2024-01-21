namespace taskflow.Models.Domain;

public class Project
{
    public Guid Id { get; set; }
    // public Guid WorkspaceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Workspace Workspace { get; set; }
    public ICollection<ProjectMember> ProjectMembers { get; set; }
    public ICollection<ProjectTask> ProjectTasks { get; set; }

    public Project()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
}