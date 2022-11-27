using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using yoga.Models;

namespace yoga.Data
{
    public class YogaAppDbContext:  IdentityDbContext<AppUser, IdentityRole, string>
    {
        public YogaAppDbContext(DbContextOptions<YogaAppDbContext> options):base(options)
        {
            
        }
        public DbSet<TechearMemberShip> TechearMemberShips{get;set;}
        public DbSet<Platform> Platforms{get;set;}
        public DbSet<MembershipCard> MembershipCards { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Notification> Notification { get; set; }
    }
}