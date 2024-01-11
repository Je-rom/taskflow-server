using Microsoft.AspNetCore.Identity;
using taskflow.Models.Domain;

namespace taskflow.Services.Interfaces;

public interface IApplicationUserService
{
    public  Task<User> FindOneAsync();

    public Task<User> FindByEmailAsync(string email);

    public Task<User> FindByUserNameAsync(string username);

    public Task<IdentityResult> CreateAsync(User user, string password);

    public Task<bool> CheckPasswordAsync(User user, string password);
}