using Backend.Common;
using Backend.Controllers;
using Backend.Data.Domain.Users;
using Backend.Data.Domain.Users.Enum;
using Backend.DTO.Identity;
using Backend.DTO.Users;

namespace Backend.Services.Identity;

public interface IIdentityService
{
    Task<UserLoginResult> ValidateSignInAsync(string userNameOrEmail, string password);


    Task SignInCustomerAsync(User customer, bool isPersistent);

    Task<UserRegisterResult> RegisterCustomerAsync(RegisterRequest model);

    Task SignOutCustomerAsync();

    Task<bool> IsUserLoggedIn();

    



}
