using Backend.Data.Domain.Users;
using FluentMigrator;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace Backend.Data.Migrations;

[Migration(0)]
public class DropAllTables : Migration
{
    public override void Up()
    {
        Delete.Table(nameof(UserRole));
        Delete.Table(nameof(Address));
        Delete.Table(nameof(User));
        Delete.Table(nameof(Role));
        Execute.Sql(@"
                DELETE FROM dbo.versionInfo;");

    }

    public override void Down()
    {
        // Bỏ trống hoặc tạo lại bảng
    }
}

