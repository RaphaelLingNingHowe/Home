using Microsoft.EntityFrameworkCore;
using ProgramGuard.Enums;
using ProgramGuard.Models;

namespace ProgramGuard.Data
{
    public class ProgramGuardContext : DbContext
    {
        public ProgramGuardContext(DbContextOptions<ProgramGuardContext> options)
            : base(options)
        {
        }
        public DbSet<FileList> FileLists { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<OperateLog> OperateLogs { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<PrivilegeRule> PrivilegeRules { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<PrivilegeRule> privilegeRules = new List<PrivilegeRule>
            {
                new PrivilegeRule {Id = 1, Name = "管理員" , Visible = (VISIBLE_PRIVILEGE)31, Operate = (OPERATE_PRIVILEGE)2047 },
                new PrivilegeRule {Id = 2, Name = "審核員" , Visible = (VISIBLE_PRIVILEGE)23, Operate = (OPERATE_PRIVILEGE)10 },
                new PrivilegeRule {Id = 3, Name = "用戶" , Visible = (VISIBLE_PRIVILEGE)3, Operate = (OPERATE_PRIVILEGE)3 }
            };
            builder.Entity<PrivilegeRule>().HasData(privilegeRules);
        }
    }
}
