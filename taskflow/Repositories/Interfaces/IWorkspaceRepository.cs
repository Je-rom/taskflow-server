using taskflow.Models.Domain;

namespace taskflow.Repositories.Interfaces
{
    public interface IWorkspaceRepository
    {
        public Task<Workspace> CreateAsync(Workspace workspace);
        public Task<Workspace> ShowAsync(Guid id);
        public Task<ICollection<Workspace>> FindAllAsync();
        public Task<Workspace> Update(Workspace workspace, Guid id);
        public Task<bool> Delete(Guid id);
    }
}

