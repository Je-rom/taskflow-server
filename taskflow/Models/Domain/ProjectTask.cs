namespace taskflow.Models.Domain;

public class ProjectTask
{
    public Guid Id { get; set; }
    //public Guid ProjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TaskStage Stage { get; set; }
    public Guid ProjectMemberId { get; set; } // Project assignee
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; }
    
    
    public ProjectMember ProjectMember { get; set; }


    public ProjectTask()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}