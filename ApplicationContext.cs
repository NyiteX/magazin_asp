using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplicationDB1.Models;

namespace WebApplicationDB1
{
    public class ApplicationContext : DbContext
    {
        /*public DbSet<User> Users { get; set; } = null!;*/
        public DbSet<Tovar> Tovars { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Tovar>().HasData(
                new Tovar { Id = 1, Name = "Sergey", Discription = "discription", Price = 5, Count = 3, Url = "https://cdn.pixabay.com/photo/2023/05/23/18/12/hummingbird-8013214_640.jpg" }
                );*/
        }

    }
}
