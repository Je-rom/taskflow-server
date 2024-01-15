namespace taskflow.Models.Domain;

public class WorkspaceMember
{
    public Guid Id { get; set; }
    //public Guid UserId { get; set; }
    //public Guid WorkspaceId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; }
    public Workspace Workspace { get; set; }

    public WorkspaceMember()
    {
        CreatedAt = DateTime.UtcNow;
    }
}