using Microsoft.EntityFrameworkCore;
using SlcBot.Data.Entities;

namespace SlcBot.Data.Contexts;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }

    public DbSet<BaseEntity> Entities { get; set; } = null!;
    public DbSet<ServerEvent> ServerEvents { get; set; } = null!;
    public DbSet<EventAttendee> EventAttendees { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseEntity>().HasKey(x => x.Id);
    }
}