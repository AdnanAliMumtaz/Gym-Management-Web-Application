using Microsoft.EntityFrameworkCore;
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

            /*SeedData(builder);*/
        }

        /*private void SeedData(ModelBuilder builder)
        {
            // Dummy Member
            builder.Entity<Member>().HasData(
                new Member
                {
                    MemberID = 1,
                    MemberFirstName = "John",
                    MemberLastName = "Doe",
                    MemberEmail = "aliali@gmail.com",
                    MemberPhoneNumber = "07405353595",

                }
            );

            // Dummy TransactionFee
            builder.Entity<TransactionFee>().HasData(
                new TransactionFee
                {
                    TransactionFeeId = 1,
                    Amount = 100.00m, // Adjust the amount as needed
                    DatePaid = DateTime.Now,
                    MemberId = 1 // Associate with the dummy Member
                }
            );
        }*/
    }
}
