using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using taskflow.Repositories.Interfaces;
using taskflow.Data;
using taskflow.Models.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace taskflow.Repositories.Implementations
{
    public class ProjectRepository(TaskFlowDbContext dbContext) : IProjectRepository
    {
        public async Task<Project> CreateAsync(Project project)
        {
            var createProject = await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();
            return project;       
        }
        public async Task<Project> Delete(Guid id)
        {
            var DeleteProjectById = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id);
            {
                if (DeleteProjectById is null)
                {
                    return null;
                }

                dbContext.Projects.Remove(DeleteProjectById);
                await dbContext.SaveChangesAsync();
                return DeleteProjectById;
            }     
        }

        public Task<ICollection<Project>> FindAllAsync()
        {     
            throw new NotImplementedException();
        }

        public async Task<Project> ShowAsync(Guid id)
        {
            var showId = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if(showId != null)
            {
                return showId;
            }
            else
            {
                return null;
            }  
        }

        public async Task<Project> UpdateAsync(Guid id, Project project)
        {
            var updateProject = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (updateProject is null)
            {
                return null;
            }
            updateProject.Name = project.Name;
            updateProject.Description = project.Description;

           await dbContext.SaveChangesAsync();

           return updateProject;
            
        }
    }
}