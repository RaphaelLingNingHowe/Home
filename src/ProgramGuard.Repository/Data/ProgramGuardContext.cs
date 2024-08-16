using Microsoft.EntityFrameworkCore;
using ProgramGuard.Enums;
using ProgramGuard.Repository.Models;

namespace ProgramGuard.Repository.Data
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

            List<PrivilegeRule> privilegeRules = new()
            {
                new PrivilegeRule {Id = 1, Name = "管理員" , Visible = (VISIBLE_PRIVILEGE)31, Operate = (OPERATE_PRIVILEGE)4095 },
            };
            builder.Entity<PrivilegeRule>().HasData(privilegeRules);

            List<User> users = new()
            {
                new User
                {
                    Id = 1,
                    Account = "admin",
                    Name = "AdminUser",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    PrivilegeRuleId = 1,
                },
            };
            builder.Entity<User>().HasData(users);

            var passwordHistories = users.Select(u => new PasswordHistory
            {
                Id = u.Id,
                UserId = u.Id,
                Password = u.Password
            }).ToList();
            builder.Entity<PasswordHistory>().HasData(passwordHistories);
        }
    }
}
