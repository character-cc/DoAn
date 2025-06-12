using System;
using System.Text.Json;
using Backend.Common.FakeData;
using Backend.Common.Mapper;
using Backend.Data;
using Backend.Data.Domain;
using Backend.Data.Migrations;
using Backend.DTO.Identity;
using Backend.Services.Identity;
using Backend.Services.Users;
using FluentMigrator.Runner;
using FluentValidation;
using FluentValidation.AspNetCore;
using LinqToDB.AspNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using LinqToDB.Configuration;
using LinqToDB;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(CreateUserRoleAddressTables).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());


builder.Services.AddControllers();

builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
{
    return options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });



builder.Services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfile)); 
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterModelValidator>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    runner.MigrateUp();


    if (app.Environment.IsDevelopment())
{
        //await FakeDataSeeder.SeedAsync(app.Services);
    }
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
