using Bogus;
using Microsoft.AspNetCore.Identity;
using Backend.Common.Utils;
using Backend.Data.Domain;
using Backend.Data.Domain.Users;
using Backend.Data.Domain.Users.Enum;
using Backend.Data;
using LinqToDB;

namespace Backend.Common.FakeData;

public static class FakeDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleRepo = scope.ServiceProvider.GetRequiredService<IRepository<Role>>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
        var userRoleRepo = scope.ServiceProvider.GetRequiredService<IRepository<UserRole>>();

        var existingRoles = await roleRepo.EntitySet.ToListAsync();
        var roleDict = existingRoles.ToDictionary(r => r.SystemName);

        var systemRoles = new List<Role>
    {
        new Role
        {
            FriendlyName = "Administrator",
            SystemName = UserDefaults.AdminRoleName,
            IsSystemRole = true,
            IsActive = true,
            IsFreeShipping = false
        },
        new Role
        {
            FriendlyName = "Registered User",
            SystemName = UserDefaults.RegisteredRoleName,
            IsSystemRole = true,
            IsActive = true,
            IsFreeShipping = false
        },
        new Role
        {
            FriendlyName = "Guest",
            SystemName = UserDefaults.GuestsRoleName,
            IsSystemRole = true,
            IsActive = true,
            IsFreeShipping = false
        }
    };

        var newRoles = systemRoles
            .Where(r => !roleDict.ContainsKey(r.SystemName))
            .ToList();

        if (newRoles.Any())
        {
            await roleRepo.InsertAsync(newRoles);
        }

        if (!await userRepo.EntitySet.AnyAsync())
        {
            var allRoles = await roleRepo.EntitySet.ToListAsync();

            var faker = new Faker<User>()
                .RuleFor(u => u.Username, f => f.Internet.UserName())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.DateOfBirth, f => DateTime.UtcNow)
                .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow)
                .RuleFor(u => u.UpdatedAt, f => DateTime.UtcNow)
                .RuleFor(u => u.IsActive, f => true)
                .RuleFor(u => u.Roles, f => new List<Role> { f.PickRandom(allRoles) });

            var fakeUsers = faker.Generate(10);

            var adminRole = allRoles.First(r => r.SystemName == UserDefaults.AdminRoleName);
            var adminUser = new User
            {
                Username = "admin",
                FirstName = "Admin",
                LastName = "Super",
                Email = "admin@example.com",
                PhoneNumber = "0000000000",
                Gender = Gender.Male,
                PasswordHash = Hasher.HashPassword("admin"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                Roles = new List<Role> { adminRole }
            };

            fakeUsers.Add(adminUser);
            await userRoleRepo.InsertAsync(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            var users = await userRepo.InsertAsync(fakeUsers);
        }
    }
}
