using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebApplication1.Areas.Identity.Data;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class WebDbContext : IdentityDbContext<ApplicationUser>
{
    public WebDbContext(DbContextOptions<WebDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuration for the one-to-many relationship
        builder.Entity<Member>()
            .HasOne(m => m.ApplicationUser)
            .WithMany(u => u.Member)
            .HasForeignKey(m => m.UserId)
            .IsRequired();
    }
}
