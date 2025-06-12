using Backend.Common;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net.Sockets;
using AutoMapper;
using Backend.DTO.Users;
using Microsoft.EntityFrameworkCore;
using Backend.Services.Users;
using System.Security.Claims;

using Backend.Common.Utils;
using Backend.Data.Domain.Users;
using Backend.Data.Domain.Users.Enum;
using Backend.DTO.Roles;
namespace Backend.Area.Admin.Controller;


[Route("admin")]
public class UserController : AdminController
{

    private readonly IUserService _userService;

    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    private int GetCurrentUserId()
    {
        string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            return 0;
        return int.Parse(id);
    }

    public Task<string> ValidateUserRole(List<Role> roles)
    {
        if (roles == null || !roles.Any())
            return Task.FromResult("Tài khoản phải ít nhất một vai trò là 'Khách' hoặc 'Đã đăng ký'");

        var systemNames = roles.Select(r => r.SystemName).ToList();

        bool isGuest = systemNames.Contains(UserDefaults.GuestsRoleName);
        bool isRegistered = systemNames.Contains(UserDefaults.RegisteredRoleName);

        if (isGuest && isRegistered)
            return Task.FromResult("Tài khoản không được gán cả hai vai trò là 'Khách' hoặc 'Đã đăng ký'");

        if (!isGuest && !isRegistered)
            return Task.FromResult("Tài khoản phải ít nhất một vai trò là 'Khách' hoặc 'Đã đăng ký'");

        return Task.FromResult(string.Empty); 
    }


    [HttpPost("users/search")]
    public async Task<IActionResult> GetUsers([FromBody] GetUsersRequest request)
    {
        var orderBy = SortableFields.GetSelector<User>(request.SortBy);
        var sortDesc = request.SortDesc;
        if (orderBy == null)
        {
            orderBy = (Expression<Func<User, Object>>)(u => u.UpdatedAt);
            sortDesc = true;
        }

        var pagedUsers = await _userService.GetAllUsersAsync(
            fullName: request.FullName,
            email: request.Email,
            phone: request.PhoneNumber,
            isActive: request.IsActive,
            userRoleId: request.UserRoleId,
            createdFrom: request.FromDate,
            createdTo: request.ToDate,
            orderBy: orderBy,
            orderDesc: sortDesc,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize
        );

        var items = pagedUsers.Items.Select(u => _mapper.Map<UserDto>(u)).ToList();
        var pagedUsersDto = new PagedList<UserDto>(items, pagedUsers.PageIndex, pagedUsers.PageSize, pagedUsers.TotalCount);
        return Ok(pagedUsersDto);
    }

    [HttpGet("users/roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _userService.GetAllRolesAsync();
        var rolesDto = roles.Items.Select(r => _mapper.Map<RoleDto>(r)).ToList();
        return Ok(rolesDto);
    }

    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserId = GetCurrentUserId(); 

        var validationResult = await _userService.ValidateUserDeletionAsync(id, currentUserId);
        if (validationResult != DeleteUserResult.Success)
        {
            return validationResult switch
            {
                DeleteUserResult.CannotDeleteSelf => BadRequest(new { message = "Không thể xóa tài khoản của chính mình" }),
                DeleteUserResult.CannotDeleteAdmin => BadRequest(new { message = "Không thể xóa admin" }),
                DeleteUserResult.NotFound => NotFound(new { message = "Người dùng không được tìm thấy với " + id }),
                _ => StatusCode(500)
            };
        }
        var currentUser = await _userService.GetUserByIdAsync(currentUserId);
        var deleted = await _userService.DeleteUserAsync(currentUser);
        return deleted > 0 ? Ok("Deleted.") : NotFound(new { message = "Tài khoản đã thực sự bị xóa" });
    }


    [HttpDelete("users/bulk-delete")]
    
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteUserRequest request)
    {
        if (request.UserIds == null || !request.UserIds.Any())
            return BadRequest("No user IDs provided.");

        var currentUserId = GetCurrentUserId(); 

        var (deletableIds, failures) = await _userService.FilterDeletableUsersAsync(request.UserIds, currentUserId);

        var usersToDelete = await _userService.GetUsersByIdsAsync(deletableIds);
        var deletedIds = await _userService.DeleteUsersAsync(usersToDelete);

        return Ok(new
        {
            Deleted = deletedIds,
            Skipped = failures.Select(f => new { UserId = f.Key, Reason = f.Value.ToString() })
        });
    }


    [HttpPatch("users/set-active")]
    public async Task<IActionResult> BulkSetIsActive([FromBody] BulkSetUserActiveRequest request)
    {
        if (request.UserIds == null || request.UserIds.Count == 0)
            return BadRequest("No user IDs provided.");

        var currentUserId = GetCurrentUserId();

        var result = await _userService.BulkSetUserActiveStatusAsync(request.UserIds, request.IsActive, currentUserId);

        return Ok(result);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Invalid user data" });
        }

        if (!string.IsNullOrEmpty(request.Username) && await _userService.GetUserByUsernameAsync(request.Username) != null)
        {
            return BadRequest(AppErrorResponse.From(nameof(request.Username) , "Username đã tồn tại"));
        }
        if (!string.IsNullOrEmpty(request.Email) && await _userService.GetUserByEmailAsync(request.Email) != null)
        {

            return BadRequest(AppErrorResponse.From(nameof(request.Email), "Email đã tồn tại"));
        }
        var requestRoles = await _userService.GetRoleByIdsAsync(request.RoleIds);
        var validationMessage = await ValidateUserRole(requestRoles);
        if (!string.IsNullOrEmpty(validationMessage))
        {
            return BadRequest(AppErrorResponse.From("Role", validationMessage));
        }
        var user = _mapper.Map<User>(request);
        user.PasswordHash = Hasher.HashPassword(request.Password);
        user.Roles = requestRoles;
           
        await _userService.InsertUserAsync(user);
        var userRoles = requestRoles.Select(r => new UserRole
        {
            UserId = user.Id,
            RoleId = r.Id,
        }).ToList();
        await _userService.InsertUserRolesAsync(userRoles);
        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    [HttpPut("users")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Invalid user data" });
        }

        var requestRoles = await _userService.GetRoleByIdsAsync(request.RoleIds);
        var validationMessage = await ValidateUserRole(requestRoles);
        if (!string.IsNullOrEmpty(validationMessage))
        {
            return BadRequest(AppErrorResponse.From("Role", validationMessage));
        }
        var existingUser = await _userService.GetUserByIdAsync(request.Id);
        if (existingUser == null)
        {
            return BadRequest(new { messegae = "User not found" });
        }
        _mapper.Map(request, existingUser); 
        if (!string.IsNullOrEmpty(request.Password))
        {
            existingUser.PasswordHash = Hasher.HashPassword(request.Password);

        }
        await _userService.UpdateUserRolesAsync(existingUser, request.RoleIds);
        await _userService.UpdateUserAsync(existingUser);
        var userDto = _mapper.Map<UserDto>(existingUser);
        return Ok(userDto);
    }

    [HttpGet("users/{userId:int}/addresses")]
    public async Task<IActionResult> GetAddressesByUserId(int userId)
    {
        var addresses = await _userService.GetAddressesByUserIdAsync(userId);

        var addressDtos = _mapper.Map<List<AddressDto>>(addresses);
        return Ok(addressDtos);
    }

    [HttpPost("users/{userId:int}/addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Invalid address data" });
        }
        var user = await _userService.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { message = "Người dùng không được tìm thấy với ID " + request.UserId });
        }
        var address = _mapper.Map<Address>(request);
        address.User = user;
        await _userService.InsertAddressAsync(address);
        var addressDto = _mapper.Map<AddressDto>(address);
        return Ok(addressDto);
    }

    [HttpPut("users/{userId:int}/addresses")]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Invalid address data" });
        }
        var user = await _userService.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { message = "Người dùng không được tìm thấy với ID " + request.UserId });
        }
        var address = await _userService.GetAddressByAddressIdAsync(request.Id);
        _mapper.Map(request, address);
        await _userService.UpdateAddressAsync(address);
        var addressDto = _mapper.Map<AddressDto>(address);
        return Ok(addressDto);
    }


    #region roles

    [HttpPost("roles/search")]
    public async Task<IActionResult> GetRoles([FromBody] GetRolesRequest request)
    {
        var orderBy = SortableFields.GetSelector<Role>(request.SortBy);
        var sortDesc = request.SortDesc;
        if (orderBy == null)
        {
            orderBy = r => r.UpdatedAt;
            sortDesc = true;
        }

        var pagedRoles = await _userService.GetAllRolesAsync(
            friendlyName: request.FriendlyName,
            systemName: request.SystemName,
            isActive: request.IsActive,
            orderBy: orderBy,
            orderDesc: sortDesc,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize
        );

        var items = pagedRoles.Items.Select(u => _mapper.Map<RoleDto>(u)).ToList();

        var result = new PagedList<RoleDto>(items, pagedRoles.PageIndex, pagedRoles.PageSize, pagedRoles.TotalCount);
        return Ok(result);
    }

    [HttpGet("roles/{id:int}")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var role = await _userService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound(new { message = $"Vai trò không được tìm thấy với ID {id}" });
        }
        var roleDto = _mapper.Map<RoleDto>(role);
        return Ok(roleDto);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Invalid role data" });
        }
        if (await _userService.GetRoleBySystemNameAsync(request.SystemName) != null)
        {
            return BadRequest(AppErrorResponse.From(nameof(request.SystemName), "System name đã tồn tại"));
        }
        var role = _mapper.Map<Role>(request);
        await _userService.InsertRoleAsync(role);
        var roleDto = _mapper.Map<RoleDto>(role);
        return Ok(roleDto);
    }

    [HttpPut("roles")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Invalid role data" });

        var existingRole = await _userService.GetRoleByIdAsync(request.Id);
        if (existingRole == null)
            return NotFound(new { message = $"Vai trò không được tìm thấy với ID {request.Id}" });

        if (existingRole.IsSystemRole && existingRole.SystemName != request.SystemName)
        {
            return BadRequest(AppErrorResponse.From(nameof(request.SystemName), "Không được sửa tên hệ thống của vai trò hệ thống"));
        }

        if (!string.IsNullOrWhiteSpace(request.SystemName)
            && existingRole.SystemName != request.SystemName
            && await _userService.GetRoleBySystemNameAsync(request.SystemName) != null)
        {
            return BadRequest(AppErrorResponse.From(nameof(request.SystemName), "Tên hệ thống đã tồn tại"));
        }

        _mapper.Map(request, existingRole);
        await _userService.UpdateRoleAsync(existingRole);

        var roleDto = _mapper.Map<RoleDto>(existingRole);
        return Ok(roleDto);
    }

    [HttpDelete("roles/{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _userService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound(new { message = $"Vai trò không được tìm thấy với ID {id}" });

        if (role.IsSystemRole)
            return BadRequest(new { message = "Không được phép xóa vai trò hệ thống" });

        var deleted = await _userService.DeleteRoleAsync(role);
        return deleted > 0 ? Ok(new { message = "Deleted." }) : StatusCode(500, new { message = "Không thể xóa vai trò" });
    }


    #endregion
}
