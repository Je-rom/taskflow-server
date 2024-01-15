using Microsoft.EntityFrameworkCore;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;

namespace taskflow.Repositories.Implementations
{
    public class WorkspaceMemberRepository(TaskFlowDbContext dbContext) : IWorkspaceMemberRepository
    {
        public async Task<WorkspaceMember> CreateAsync(WorkspaceMember workspaceMember)
        {
            await dbContext.WorkspaceMembers.AddAsync(workspaceMember);
            await dbContext.SaveChangesAsync();
            return workspaceMember;
        }

        public async Task<WorkspaceMember> ShowAsync(Workspace workspace, Guid id)
        {
            return await dbContext.WorkspaceMembers
                .FirstOrDefaultAsync(x => x.Workspace.Id == id);
        }

        public async Task<WorkspaceMember> FindByUserIdAsync(Workspace workspace, Guid userId)
        {
            return await dbContext.WorkspaceMembers
                .FirstOrDefaultAsync(x => x.Workspace.Id == workspace.Id && x.User.Id == userId.ToString());
        }

        public async Task<ICollection<WorkspaceMember>> FindAllAsync(Workspace workspace)
        {
            return await dbContext.WorkspaceMembers
                .Where(x => x.Workspace.Id == workspace.Id)
                .ToListAsync();
        }

    
        public async Task<WorkspaceMember> DeleteAsync(Workspace workspace, Guid id)
        {
            var workspaceMember = await FindByUserIdAsync(workspace, id);
            if (workspaceMember == null)
            {
                return null;
            }
            
            dbContext.WorkspaceMembers.Remove(workspaceMember);
            await dbContext.SaveChangesAsync();

            return workspaceMember;
        }
    }
}

