using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UraDocs.ApiService.Domain;

namespace UraDocs.ApiService.Data;

public class UraDbContext:DbContext
{
    public UraDbContext(DbContextOptions<UraDbContext> options) : base(options)
    {
    }

    public DbSet<FileHash> FileHashes { get; set; } = null!;
}