using taskflow.Models.Domain;

namespace taskflow.Repositories.Interfaces;

public interface IProjectTaskRepository
{
    public Task<ProjectTask> CreateAsync(ProjectTask projectTask);

    public Task<ProjectTask> ShowAsync(Project project, Guid id);
    
    public Task<ProjectTask> UpdateAsync(Project project, Guid id, ProjectTask projectTask);
    
    public Task<ICollection<ProjectTask>> FindAllAsync(Project project);
    
    public Task<ProjectTask> DeleteAsync(Project project, Guid id);

}