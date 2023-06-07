using Microsoft.EntityFrameworkCore;
using TextFileUpload.Models;
namespace TextFileUpload.Data
{
    public class TextFileDbContext : DbContext , IDisposable
    {
        public TextFileDbContext(DbContextOptions<TextFileDbContext> options)
            : base(options)
        {
        }

        public DbSet<TextFile> TextFiles { get; set; }
    }
}
