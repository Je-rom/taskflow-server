using Microsoft.EntityFrameworkCore;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;

namespace taskflow.Repositories.Implementations
{
    public class WorskpaceRepository(TaskFlowDbContext dbContext) : IWorkspaceRepository
    {
        public Guid UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<Workspace> CreateAsync(Workspace workspace)
        { 
            await dbContext.Workspaces.AddAsync(workspace);
            await dbContext.SaveChangesAsync();
            return workspace;
        }

        public async Task<Workspace> ShowAsync(Guid id)
        {
            return await dbContext.Workspaces.Include("User")
                .FirstOrDefaultAsync(x => x.Id == id);
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
            var existingWorkspace = await dbContext.Workspaces
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingWorkspace == null)
            {
                return null;
            }

            existingWorkspace.Name = workspace.Name;
            existingWorkspace.Description = workspace.Description;

            await dbContext.SaveChangesAsync();
            return existingWorkspace;
        }

        public async Task<Workspace> Delete(Guid id)
        {
            var workspaceDeleteById = await dbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == id);
            if ( workspaceDeleteById == null)
            {
                return null;
            }

            dbContext.Workspaces.Remove(workspaceDeleteById);
            await dbContext.SaveChangesAsync();

            return workspaceDeleteById;
        }

       
    }
}

