using Microsoft.AspNetCore.Identity;
using taskflow.Models.Domain;
using taskflow.Services.Interfaces;

namespace taskflow.Services.Impls;

public class ApplicationUserService(
    UserManager<User> manager
    ) : IApplicationUserService {
    
    public Task<User> FindOneAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> FindByEmailAsync(string email)
    {
        return manager.FindByEmailAsync(email);
    }
    
    public Task<User> FindByUserNameAsync(string username)
    {
        return manager.FindByNameAsync(username);
    }

    public Task<IdentityResult> CreateAsync(User identityUser, string password)
    {
        return manager.CreateAsync(identityUser, password);
    }

    public Task<bool> CheckPasswordAsync(User user, string password)
    {
        return manager.CheckPasswordAsync(user, password);
    }

    
}