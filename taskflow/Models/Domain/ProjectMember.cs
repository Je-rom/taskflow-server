namespace taskflow.Models.Domain;

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    //public Guid WorkspaceMemberId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; }
    public User User { get; set; }
    public ICollection<ProjectTask> ProjectTasks { get; set; }

    public ProjectMember()
    {
        CreatedAt = DateTime.UtcNow;
    }
}