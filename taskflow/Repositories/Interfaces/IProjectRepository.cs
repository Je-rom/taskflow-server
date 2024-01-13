using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using taskflow.Models.Domain;

namespace taskflow.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        public Task<Project> CreateAsync(Project project);
        public Task<Project> ShowAsync(Guid id);
        public Task<ICollection<Project>> FindAllAsync();
        public Task<Project> UpdateAsync(Guid id, Project project );
        public Task<Project> Delete(Guid id);
        
    }
}