
using Backend.Data.Domain.Users;
using Bogus;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

public class AppDataConnection : DataConnection
{
    public AppDataConnection(DataOptions<AppDataConnection> options)
        : base(options.Options)
    {
    }

    public ITable<User> People => this.GetTable<User>();

    public ITable<Address> Addresses => this.GetTable<Address>();

    public ITable<Role> Roles => this.GetTable<Role>();

    public ITable<UserRole> UserRoles => this.GetTable<UserRole>();


}
