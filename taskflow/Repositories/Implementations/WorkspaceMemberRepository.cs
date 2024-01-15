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

        public Task<ICollection<Workspace>> FindAllAsync(Workspace workspace)
        {
            throw new NotImplementedException();
        }
    
        public async Task<WorkspaceMember> DeleteAsync(Workspace workspace, Guid id)
        {
            var workspaceMember = await this.ShowAsync(workspace, id);
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

