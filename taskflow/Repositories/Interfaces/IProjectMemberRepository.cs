using taskflow.Models.Domain;

namespace taskflow.Repositories.Interfaces;

public interface IProjectMemberRepository
{
    public Task<ProjectMember> CreateAsync(ProjectMember projectMember);

    public Task<ProjectMember> ShowAsync(Project project, Guid id);

    public Task<ProjectMember> FindByWorkspaceMemberIdAsync(Project project, Guid userId);
    
    public Task<ICollection<ProjectMember>> FindAllAsync(Project project);
    
    public Task<ProjectMember> DeleteAsync(Project project, Guid id);

}