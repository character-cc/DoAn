using System.Data;
using Backend.Data.Domain.Users;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend.Data.Migrations;

[Migration(20250605_001)]
public class CreateUserRoleAddressTables : Migration
{
    public override void Up()
    {
        Create.Table(nameof(Role))
            .WithColumn(nameof(Role.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(Role.FriendlyName)).AsString(256).NotNullable()
            .WithColumn(nameof(Role.SystemName)).AsString(256).NotNullable()
            .WithColumn(nameof(Role.IsActive)).AsBoolean().WithDefaultValue(true)
            .WithColumn(nameof(Role.IsFreeShipping)).AsBoolean().WithDefaultValue(false)
            .WithColumn(nameof(Role.IsSystemRole)).AsBoolean().WithDefaultValue(false)
               .WithColumn(nameof(Address.CreatedAt)).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(Address.UpdatedAt)).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);
        

        Create.Table(nameof(User))
            .WithColumn(nameof(User.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(User.Username)).AsString(256).NotNullable()
            .WithColumn(nameof(User.FirstName)).AsString(256).NotNullable()
            .WithColumn(nameof(User.LastName)).AsString(256).NotNullable()
            .WithColumn(nameof(User.Gender)).AsInt32().NotNullable()
            .WithColumn(nameof(User.Email)).AsString(256).NotNullable()
            .WithColumn(nameof(User.PhoneNumber)).AsString(50).NotNullable()
            .WithColumn(nameof(User.PasswordHash)).AsString(512).NotNullable()
            .WithColumn(nameof(User.DateOfBirth)).AsDateTime().Nullable()
            .WithColumn(nameof(User.IsDeleted)).AsBoolean().WithDefaultValue(false)
            .WithColumn(nameof(User.IsActive)).AsBoolean().WithDefaultValue(true)
            .WithColumn(nameof(User.CreatedAt)).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(User.UpdatedAt)).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(User.Notes)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(User.LastLogin)).AsDateTime().Nullable()
            .WithColumn(nameof(User.LastLoginIp)).AsString(64).Nullable();

        Create.Table(nameof(Address))
            .WithColumn(nameof(Address.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(Address.Title)).AsString(256).NotNullable()
            .WithColumn(nameof(Address.RecipientName)).AsString(256).NotNullable()
            .WithColumn(nameof(Address.RecipientPhone)).AsString(20).NotNullable()
            .WithColumn(nameof(Address.ProvinceId)).AsInt32().NotNullable()
            .WithColumn(nameof(Address.DistrictId)).AsInt32().NotNullable()
            .WithColumn(nameof(Address.WardId)).AsInt32().NotNullable()
            .WithColumn(nameof(Address.StreetAddress)).AsString(512).NotNullable()
            .WithColumn(nameof(Address.AddressType)).AsInt32().NotNullable()
            .WithColumn(nameof(Address.Notes)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(Address.IsDefault)).AsBoolean().WithDefaultValue(false)
            .WithColumn(nameof(Address.UserId)).AsInt32().NotNullable()
            .WithColumn(nameof(Address.FullAddress)).AsString(1024).Nullable()
            .WithColumn(nameof(Address.CreatedAt)).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(Address.UpdatedAt)).AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.ForeignKey("FK_Address_User")
            .FromTable(nameof(Address)).ForeignColumn(nameof(Address.UserId))
            .ToTable(nameof(User)).PrimaryColumn(nameof(User.Id));

        Create.Table(nameof(UserRole))
            .WithColumn(nameof(UserRole.UserId)).AsInt32().NotNullable()
            .WithColumn(nameof(UserRole.RoleId)).AsInt32().NotNullable();

        Create.PrimaryKey("PK_UserRole").OnTable("UserRole").Columns("UserId", "RoleId");

        Create.ForeignKey("FK_UserRole_User")
            .FromTable(nameof(UserRole)).ForeignColumn(nameof(UserRole.UserId))
            .ToTable(nameof(User)).PrimaryColumn(nameof(User.Id));

        Create.ForeignKey("FK_UserRole_Role")
            .FromTable(nameof(UserRole)).ForeignColumn(nameof(UserRole.RoleId))
            .ToTable(nameof(Role)).PrimaryColumn(nameof(Role.Id)).OnDelete(Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_UserRole_Role").OnTable(nameof(UserRole));
        Delete.ForeignKey("FK_UserRole_User").OnTable(nameof(UserRole));
        Delete.Table(nameof(UserRole));

        Delete.ForeignKey("FK_Address_User").OnTable(nameof(Address));
        Delete.Table(nameof(Address));

        Delete.Table(nameof(User));
        Delete.Table(nameof(Role));
    }
}