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
        public Task<Project> ShowAsync(Workspace workspace, Guid id);
        public Task<Project> UpdateAsync(Workspace workspace, Guid id, Project project);
        public Task<Project> DeleteAsync(Workspace workspace, Guid id);
    }
}