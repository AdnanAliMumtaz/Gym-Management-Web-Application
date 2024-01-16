using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ManagingDbContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<TransactionFee> TransactionFees { get; set; }
        public ManagingDbContext(DbContextOptions<ManagingDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Member>()
            .HasOne(m => m.ApplicationUser)
            .WithMany(u => u.Member)
            .HasForeignKey(m => m.UserId)
            .IsRequired();
        }
    }
}
