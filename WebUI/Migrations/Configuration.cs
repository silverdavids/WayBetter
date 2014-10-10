using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain.Models.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebUI.DataAccessLayer;

namespace WebUI.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            #region Roles and Admin Seed
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roles = roleManager.Roles.Count();
            if (roles > 0) return;
            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("Teller"));
            roleManager.Create(new IdentityRole("Manager"));

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var hasher = new PasswordHasher();
            var teller = new ApplicationUser
            {
                Email = "igakigongo@gmail.com",
                EmailConfirmed = true,
                IsActivated = true,
                PasswordHash = hasher.HashPassword("Teller.Test1"),
                PhoneNumber = "+256752433373",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                UserName = "TellerTest",
            };
            manager.Create(teller);
            manager.AddToRole(teller.Id, "Teller");

            var admin = new ApplicationUser
            {
                Email = "igaedward@ymail.com",
                EmailConfirmed = true,
                IsActivated = true,
                PasswordHash = hasher.HashPassword("BetWay.Admin1"),
                PhoneNumber = "+256791678338",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                UserName = "BetWayAdmin"
            };

            manager.Create(admin);
            manager.AddToRole(admin.Id, "Admin");

            var betmanager = new ApplicationUser
            {
                Email = "betwayadmin@gmail.com",
                EmailConfirmed = true,
                IsActivated = true,
                PasswordHash = hasher.HashPassword("Test.Manager1"),
                PhoneNumber = "+256791678338",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                UserName = "TestManager"
            };

            manager.Create(betmanager);
            manager.AddToRole(betmanager.Id, "Manager");

            #endregion
        }
    }
}
