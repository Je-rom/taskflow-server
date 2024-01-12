using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using taskflow.Repositories.Interfaces;
using taskflow.Data;
using taskflow.Models.Domain;

namespace taskflow.Repositories.Implementations
{
    public class ProjectRepository(TaskFlowDbContext dbContext) : IProjectRepository
    {
        public async Task<Project> CreatAsync(Project project)
        {
            await dbContext.Workspaces.AddAsync(project);
            await dbContext.SaveChangesAsync();
            return project;

        }

        public async Task<Project> ShowAsync(Guid id)
        {

        }

        public async async Task<ICollection<Project>> FindAllAsync()

        {

        }

        public async Task<Project> UpdateAsync(Guid id, Project project)
        {

        }

         public async Task<Project> Delete(Guid id)
         {

         }
        

       
        

        
    }
}