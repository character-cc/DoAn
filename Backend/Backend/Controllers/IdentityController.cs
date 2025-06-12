using Backend.Common;
using Backend.Data.Domain.Users.Enum;
using Backend.Services.Identity;
using Backend.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
public class IdentityController : PublicController
{
    private readonly IIdentityService _identityService;

    private readonly IUserService _userService;
    public IdentityController(IIdentityService identityService, IUserService userService)
    {
        _identityService = identityService;
        _userService = userService;
    }

    [HttpPost("login")]
    public virtual async Task<IActionResult> Login([FromBody] DTO.Identity.LoginRequest loginModel)
    {
        if(ModelState.IsValid)
        {
            if (await _identityService.IsUserLoggedIn())
            {
                return BadRequest(new { message = "You are already logged in." });
            }
            var usernameOrEmail = loginModel.UserNameOrEmail.Trim();
            var password = loginModel.Password;
            var loginResult = await _identityService.ValidateSignInAsync(usernameOrEmail, password);
            switch(loginResult)
            {
                case UserLoginResult.Successful:
                    {
                        var customer = await _userService.GetUserByUsernameOrEmailAsync(usernameOrEmail);
                            await _identityService.SignInCustomerAsync(customer, loginModel.RememberMe);
                            return Ok(new { customer = new { customer.Id , customer.Username, customer.Email } });
                    }
                    
                case UserLoginResult.CustomerNotExist:
                    return Unauthorized(new { message = "User does not exist." });
                case UserLoginResult.Deleted:
                    return Unauthorized(new { message = "User account has been deleted." });
                case UserLoginResult.NotActive:
                    return Unauthorized(new { message = "User account is not active." });
                case UserLoginResult.WrongPassword:
                    return Unauthorized(new { message = "Incorrect password." });
                default:
                    return BadRequest(new { message = "An unexpected error occurred." });
            }
        }

        return BadRequest(ModelState);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DTO.Identity.RegisterRequest model)
    {
        if (ModelState.IsValid)
        {
            if (await _identityService.IsUserLoggedIn())
            {
                return BadRequest(new { message = "You are already logged in." });
            }
            var result = await _identityService.RegisterCustomerAsync(model);
            switch (result)
            {
                case UserRegisterResult.Successful:
                    {
                         var user = await _userService.GetUserByUsernameOrEmailAsync(model.Username);
                         await _identityService.SignInCustomerAsync(user, true);
                         return Ok(new { message = "Registration successful." });
                    }
                   
                case UserRegisterResult.InvalidModelState:
                    return BadRequest(ModelState);
                 case UserRegisterResult.UsernameOrEmailAlreadyExists:
                    return BadRequest(new { message = "Username or email already exists." });
                default:
                    return BadRequest(new { message = "An unexpected error occurred." });
            }
        }
        return BadRequest(ModelState);
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _identityService.SignOutCustomerAsync();
        return Ok(new { message = "Logged out successfully" });
    }


}
