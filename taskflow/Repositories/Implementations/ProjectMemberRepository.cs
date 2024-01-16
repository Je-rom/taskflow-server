using Microsoft.EntityFrameworkCore;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;

namespace taskflow.Repositories.Implementations;

public class ProjectMemberRepository(TaskFlowDbContext dbContext) : IProjectMemberRepository
{
    public async Task<ProjectMember> CreateAsync(ProjectMember projectMember)
    {
        await dbContext.ProjectMembers.AddAsync(projectMember);
        await dbContext.SaveChangesAsync();
        return projectMember;
    }

    public async Task<ProjectMember> ShowAsync(Project project, Guid id)
    {
        return await dbContext.ProjectMembers
            .FirstOrDefaultAsync(x => x.Project.Id == project.Id && x.User.Id == id.ToString());
    }

    public async Task<ProjectMember> FindByWorkspaceMemberIdAsync(Project project, Guid userId)
    {
        return await dbContext.ProjectMembers
            .FirstOrDefaultAsync(x => x.Project.Id == project.Id && x.User.Id == userId.ToString());

    }

    public async Task<ICollection<ProjectMember>> FindAllAsync(Project project)
    {
        return await dbContext.ProjectMembers
            .Where(x => x.Project.Id == project.Id)
            .ToListAsync();
    }

    public async Task<ProjectMember> DeleteAsync(Project project, Guid userId)
    {
        var projectMember = await FindByWorkspaceMemberIdAsync(project, userId);
        if (projectMember == null)
            return null;
            
        dbContext.ProjectMembers.Remove(projectMember);
        await dbContext.SaveChangesAsync();

        return projectMember;
    }
}