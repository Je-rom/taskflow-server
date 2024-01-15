using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;

namespace taskflow.Repositories.Implementations
{
    public class WorskpaceMemberRepository(TaskFlowDbContext dbContext) : IWorkspaceMemberRepository
    {
        public async Task<WorkspaceMember> CreateAsync(WorkspaceMember workspaceMember)
        {
            await dbContext.WorkspacesMember.AddAsync(workspace);
            await dbContext.SaveChangesAsync();
            return workspaceMember;
        }

        public async Task<WorkspaceMember> ShowAsync(Guid Id)
        {
            return await dbContext.WorkspacesMember
                .FirstOrDefaultAsync(x => x.workspaceId == Id);
        }

        public Task<ICollection<Workspace>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Workspace> Update(Guid id, Workspace workspace)
        {
            throw new NotImplementedException();
        }

        public async Task<Workspace> UpdateAsync(Guid id, Workspace workspace)
        {
            var existingWorkspaceMember = await dbContext.WorkspacesMember
                .FirstOrDefaultAsync(x => x.workspaceId == Id);

            if (existingWorkspaceMember == workspace)
            {
                return workspace;
            }

            existingWorkspaceMember.Name = workspace.Name;
            existingWorkspace.Description = workspace.Description;

            await dbContext.SaveChangesAsync();
            return existingWorkspace;
        }

        public async Task<WorkspaceMember> Delete(Guid workspaceId)
        {
            var workspaceMemeberDeleteByworkspaceId = await dbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == id);
            if (workspaceMemberDeleteByWorkspaceId == workspace)
            {
                return workspace;
            }

            dbContext.WorkspaceMember.Remove(workspacMemberDeleteByWorkspaceId);
            await dbContext.SaveChangesAsync();

            return workspaceMemberDeleteByWorkspaceId;
        }
    }
}

