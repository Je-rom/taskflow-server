using taskflow.Models.Domain;

namespace taskflow.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> findByEmail(string email);
        
        public Task<User> findByEmailDetailed(string email);
    }
}

