using Microsoft.EntityFrameworkCore;
using taskflow.Models.Domain;
using taskflow.Repositories.Interfaces;
using taskflow.Data;

namespace taskflow.Repositories.Implementations;

public class ProjectTaskRepository(TaskFlowDbContext dbContext) : IProjectTaskRepository
{
    public async Task<ProjectTask> CreateAsync(ProjectTask projectTask)
    {
         await dbContext.ProjectTasks.AddAsync(projectTask);
         await dbContext.SaveChangesAsync();
         return projectTask;

    }

    public async Task<ProjectTask> DeleteAsync(Project project, Guid id)
    {
        var projectTask = await dbContext.ProjectTasks
                .FirstOrDefaultAsync(p => p.Id == id && p.Project.Id == project.Id);
        if (project == null)
            return null;

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();
        return projectTask;



    }

    public async Task<ICollection<ProjectTask>> FindAllAsync(Project project)
    {
        return await dbContext.ProjectTasks
           .Where(x => x.Project.Id == project.Id)
           .ToListAsync();

    }

    public async Task<ProjectTask> ShowAsync(Project project, Guid id)
    {
            var projectTask = await dbContext.ProjectTasks
                     .FirstOrDefaultAsync(p => p.Id == id && p.Project.Id == project.Id);
            if (project == null)
                return null;
            return projectTask;
    }

    public async Task<ProjectTask> UpdateAsync(Project project, Guid id, ProjectTask projectTask)
    {
        var updateTask = await dbContext.ProjectTasks
                   .FirstOrDefaultAsync(p => p.Id == id && p.Project.Id == project.Id);
        if (updateTask == null)
            return null;
        updateTask.Name = projectTask.Name;
        updateTask.Description = projectTask.Description;
        updateTask.StartDate = projectTask.StartDate;
        updateTask.EndDate = projectTask.EndDate;

        await dbContext.SaveChangesAsync();
        return updateTask;



    }
}

































/*public async Task<ProjectTask> CreateAsync(ProjectTask projectTask)
{
    await dbContext.ProjectTasks.AddAsync(projectTask);
    await dbContext.SaveChangesAsync();
    return projectTask
    }

public async Task<ProjectTask> ShowAsync(ProjectTask projectTask, Guid id)
{
    var project await dbContext.ProjectTask
      .FirstOrDefaultAsync(p => p.Id == id && p.ProjectTask.Id == projectTask.Id);

    return project;

}

public Task<ProjectTask> UpdateAsync(ProjectTask projectTask, Guid id)
{


}


public async Task<ProjectTask> FindByProjectTaskIdAsync(Guid id)
{
    var projectTask = await dbContext.ProjectTask
       .FirstOrDefaultAsync(p => p.Id == id);

    return projectTask;
}

public Task<ICollection<ProjectMember>> FindAllAsync(ProjectTask projectTask)
{
    throw new NotImplementedException();
}

public async Task<ProjectTask> DeleteAsync(ProjectTask projectTask, Guid id)
{
    var projectTask = await dbContext.ProjectTask
       .FirstOrDefaultAsync(p => p.Id == id && p.ProjectTask.Id == projectTask.Id)

           if (projectTask == null)
        return null;
    dbContext.ProjectTask.Remove(projectTask);
    await dbContext.SaveChangesAsync();
    return projectTask;
}
*/