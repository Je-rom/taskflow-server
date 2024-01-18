using taskflow.Models.Domain;

namespace taskflow.Repositories.Interfaces;

public interface IProjectTaskRepository
{
    public Task<ProjectTask> CreateAsync(ProjectTask projectTask);

    public Task<ProjectTask> ShowAsync(Project project, Guid id);
    
    public Task<ProjectTask> UpdateAsync(Project project, Guid id);

    public Task<ProjectTask> FindByProjectTaskIdAsync(Project project, Guid userId);
    
    public Task<ICollection<ProjectMember>> FindAllAsync(Project project);
    
    public Task<ProjectTask> DeleteAsync(Project project, Guid id);

}