using Microsoft.EntityFrameworkCore;
using ToDo.Models;

namespace ToDo.Data
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDoItem> ToDoItems { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Status> Statuses {  get; set; } = null!;

        //Seed date
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = "work", Name = "Work"},
                new Category { Id = "home", Name = "Home" },
                new Category { Id = "ex", Name = "Exercise" },
                new Category { Id = "shop", Name = "Shopping" },
                new Category { Id = "call", Name = "Contact" }
                );

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = "open", Name = "Open"},
                new Status { Id = "closed", Name = "Completed"}
                );
        }
    }
}
