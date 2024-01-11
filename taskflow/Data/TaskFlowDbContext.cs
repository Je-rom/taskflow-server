
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using taskflow.Models.Domain;

namespace taskflow.Data
{
    public class TaskFlowDbContext : IdentityDbContext<User>
    {

        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> dbContextOptions) : base(dbContextOptions) {}
        
        /*public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }*/
        /*public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }*/
        

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the interceptor for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.ClrType).Property<DateTime>("CreatedAt");
                modelBuilder.Entity(entityType.ClrType).Property<DateTime>("UpdatedAt");
            }
        }*/
        
        /*public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }*/

        // Updated field before Creating or Deleting
        /*private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is not null)
                {
                    var now = DateTime.UtcNow;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Property("CreatedAt").CurrentValue = now;
                            entry.Property("UpdatedAt").CurrentValue = now;
                            break;

                        case EntityState.Modified:
                            entry.Property("UpdatedAt").CurrentValue = now;
                            break;
                    }
                }
            }
        }*/
        // Create properties based on the available entities
        /*public DbSet<User> Users { get; set; }*/
        
    }
}

