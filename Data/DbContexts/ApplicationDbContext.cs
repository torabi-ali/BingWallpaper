using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    public DbSet<ImageInfo> Images { get; set; }
}