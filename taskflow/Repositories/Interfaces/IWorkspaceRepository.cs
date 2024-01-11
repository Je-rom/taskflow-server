using taskflow.Models.Domain;

namespace taskflow.Repositories.Interfaces
{
    public interface IWorkspaceRepository
    {
        public Task<Workspace> CreateAsync(Workspace workspace);
        public Task<Workspace> ShowAsync(Guid id);
        public Task<ICollection<Workspace>> FindAllAsync();
        public Task<Workspace> UpdateAsync(Guid id, Workspace workspace);
        public Task<Workspace> Delete(Guid id);
    }
}

