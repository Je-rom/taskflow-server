using Microsoft.EntityFrameworkCore;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;

namespace taskflow.Repositories.Implementations
{
    public class WorskpaceRepository(TaskFlowDbContext dbContext) : IWorkspaceRepository
    {
        public async Task<Workspace> CreateAsync(Workspace workspace)
        { 
            await dbContext.Workspaces.AddAsync(workspace);
            await dbContext.SaveChangesAsync();
            return workspace;
        }

        public async Task<Workspace> ShowAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Workspace>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Workspace> Update(Workspace workspace, Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

