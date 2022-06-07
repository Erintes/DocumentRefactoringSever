using Microsoft.EntityFrameworkCore;

namespace DocumentRefactoringServer.Models
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UploadedDocument> UploadedDocuments { get; set; }
        public DbSet<EditedDocument> EditedDocuments { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
