using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using taskflow.Repositories.Interfaces;
using taskflow.Data;
using taskflow.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace taskflow.Repositories.Implementations
{
    public class ProjectRepository(TaskFlowDbContext dbContext) : IProjectRepository
    {
        public async Task<Project> CreateAsync(Project project)
        {
            await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();
            return project;       
        }

        public async Task<Project> ShowAsync(Workspace workspace, Guid id)
        {
            var project = await dbContext.Projects
                .Include(x => x.ProjectTasks)
                .Include(w => w.ProjectMembers)
                .ThenInclude(wm => wm.User)  // Add a
                .FirstOrDefaultAsync(p => p.Id == id && p.Workspace.Id == workspace.Id);

            return project;
        }
        
        public async Task<Project> FindById(Guid id)
        {
            var project = await dbContext.Projects
                .Include(w => w.ProjectMembers)
                .ThenInclude(wm => wm.User)  // Add a
                .FirstOrDefaultAsync(p => p.Id == id);
            return project;
        }

        public async Task<Project> UpdateAsync(Workspace workspace, Guid id, Project project)
        {
            var updateProject = await dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.Workspace.Id == workspace.Id);
            if (updateProject == null)
                return null;
            
            updateProject.Name = project.Name;
            updateProject.Description = project.Description;
            updateProject.StartDate = project.StartDate;
            updateProject.EndDate = project.EndDate;

            await dbContext.SaveChangesAsync();
            return updateProject;
        }
        
        public async Task<Project> DeleteAsync(Workspace workspace, Guid id)
        {
            var project = await dbContext.Projects
                .Include(x => x.ProjectTasks)
                .Include(w => w.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == id && p.Workspace.Id == workspace.Id);
            if (project == null)
                return null;
            
            dbContext.RemoveRange(project.ProjectTasks);
            dbContext.RemoveRange(project.ProjectMembers);
            
            dbContext.Projects.Remove(project);
            await dbContext.SaveChangesAsync();
            return project;
        }     

    }
    
}