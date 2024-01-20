
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using taskflow.Models.Domain;

namespace taskflow.Data
{
    public class TaskFlowDbContext : IdentityDbContext<User>
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> dbContextOptions) : base(dbContextOptions) {}
        
        public DbSet<User> Users { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure your database connection here
            optionsBuilder.UseSqlServer("TaskFlowConnectionString");
        
            // Configure query splitting behavior to avoid the warning
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        }*/
        
    }
}

