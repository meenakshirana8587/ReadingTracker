using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ReadingTracker.Domain.Entities;

namespace ReadingTracker.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 1. Add your new DbSets!
        public DbSet<Book> Books => Set<Book>();
        public DbSet<UserBook> UserBooks => Set<UserBook>();
        public DbSet<ReadingSession> ReadingSessions => Set<ReadingSession>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Keep this first!

            // Identity Table Renaming (from Phase 1)
            builder.Entity<ApplicationUser>().ToTable("users");
            builder.Entity<IdentityRole<Guid>>().ToTable("roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");

            // 2. Configure the Enum to save as a String
            builder.Entity<UserBook>()
                .Property(ub => ub.Status)
                .HasConversion<string>();

            // 3. Optional but highly recommended: Make ISBN unique in the catalog
            builder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();
        }
    }
}
