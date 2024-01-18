using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;

namespace taskflow.Repositories.Implementations;

public class ProjectTaskRepository : IProjectTaskRepository
{
    public Task<ProjectTask> CreateAsync(ProjectTask projectTask)
    {
        throw new NotImplementedException();
    }

    public Task<ProjectTask> ShowAsync(Project project, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ProjectTask> UpdateAsync(Project project, Guid id)
    {
        throw new NotImplementedException();
    }
    
    
    

    public Task<ProjectTask> FindByProjectTaskIdAsync(Project project, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<ProjectMember>> FindAllAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task<ProjectTask> DeleteAsync(Project project, Guid id)
    {
        throw new NotImplementedException();
    }
}