using Microsoft.EntityFrameworkCore;
using _1131651.Models;

namespace _1131651.Data
{
    public class CmsContext : DbContext
    {
        public CmsContext(DbContextOptions<CmsContext> options)
            : base(options)
        {
        }

        public DbSet<User> TableUsers1131651 { get; set; }
        public DbSet<RechargeRecord> TableRechargeRecords1131651 { get; set; }
        public DbSet<PoolType> TablePoolTypes1131651 { get; set; }
        public DbSet<Package> TablePackages1131651 { get; set; }
        public DbSet<RechargeDetail> TableRechargeDetails1131651 { get; set; }
        public DbSet<UserThemeSetting> TableUserThemeSettings1131651 { get; set; }
    }
}