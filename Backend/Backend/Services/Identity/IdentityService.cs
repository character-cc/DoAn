using System.Security.Claims;
using AutoMapper;
using Backend.Common;
using Backend.Common.Utils;
using Backend.Data;
using Backend.Data.Domain;
using Backend.Data.Domain.Users;
using Backend.Data.Domain.Users.Enum;
using Backend.DTO.Identity;
using LinqToDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services.Identity;

public class IdentityService : IIdentityService
{

    private readonly IRepository<User> _userRepository;

    private readonly IRepository<Role> _roleRepository;

    private readonly IRepository<UserRole> _userRoleRepository;

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IMapper _mapper;

    public IdentityService(IRepository<User> userRepository, 
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository, 
        IHttpContextAccessor httpContextAccessor, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<UserLoginResult> ValidateSignInAsync(string usernameOrEmail, string password)
    {
        if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
        {
            return UserLoginResult.InvalidModelState;
        }
        var user = await _userRepository.EntitySet.FirstOrDefaultAsync(c => c.Username == usernameOrEmail || c.Email == usernameOrEmail);
        if (user == null)
            return UserLoginResult.WrongPassword;
        if (user.IsDeleted)
           return UserLoginResult.Deleted;
        if (!user.IsActive)
           return UserLoginResult.NotActive;
        var passwordVerificationResult = Hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return UserLoginResult.WrongPassword;
        }
        return UserLoginResult.Successful;
    }

    public async Task SignInCustomerAsync(User customer, bool isPersistent)
    {
        if(customer.Id <= 0)
            throw new ArgumentException("Customer ID must be greater than zero.", nameof(customer));
        ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        var roleIds = await _userRoleRepository.EntitySet
            .Where(ur => ur.UserId == customer.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var roles = roleIds.Count == 0
            ? new List<Role>()
            : await _roleRepository.EntitySet
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, customer.Email),
        new Claim(ClaimTypes.NameIdentifier, customer.Username),
        new Claim(CustomClaimTypes.USER_ID, customer.Id.ToString())
    };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.SystemName)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        };

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);
    }



    public async Task<UserRegisterResult> RegisterCustomerAsync(RegisterRequest model)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        var exists = await _userRepository.EntitySet
            .Where(c => c.Username == model.Username || c.Email == model.Email)
            .AnyAsync();
        if (exists)
            return UserRegisterResult.UsernameOrEmailAlreadyExists;
        var customer = _mapper.Map<User>(model);
        customer.PasswordHash = Hasher.HashPassword(model.Password);

        var role = await _roleRepository.EntitySet
            .Where(r => r.SystemName == UserDefaults.RegisteredRoleName)
            .FirstOrDefaultAsync();

        await _userRepository.InsertAsync(customer);

        if (role != null)
        {
            await _userRoleRepository.InsertAsync(new UserRole
            {
                UserId = customer.Id,
                RoleId = role.Id
            });
        }

        return UserRegisterResult.Successful;
    }


    public async Task SignOutCustomerAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }


    public async Task<bool> IsUserLoggedIn()
    {
        return _httpContextAccessor.HttpContext!.User.Identity.IsAuthenticated;
    }

  
}
