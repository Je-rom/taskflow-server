using Microsoft.EntityFrameworkCore;
using taskflow.Data;
using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;

namespace taskflow.Repositories.Implementations
{
    public class UserRepository(TaskFlowDbContext dbContext) : IUserRepository
    {
        public Task<User> findByEmail(string email)
        {
            return dbContext.Users
                .Include("Workspaces")
                .FirstOrDefaultAsync(x => x.Email == email);
        }
        
        public async Task<User> findByEmailDetailed(string email)
        {
            return await dbContext.Users
               // .Include("Workspaces")
                /*.Include("Workspaces.Projects")*/
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}

