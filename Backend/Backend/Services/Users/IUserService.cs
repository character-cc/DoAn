using System.Linq.Expressions;
using Backend.Common;
using Backend.Data.Domain.Users;
using Backend.Data.Domain.Users.Enum;
using Backend.DTO.Users;

namespace Backend.Services.Users;

public interface IUserService
{
    #region user

    Task<User> GetUserByUsernameOrEmailAsync(string userNameOrEmail);

    Task<IPagedList<User>> GetAllUsersAsync(
      string fullName = null,
      string firstName = null,
      string lastName = null,
      string email = null,
      string phone = null,
      bool? isActive = null,
      int[] userRoleId = null,
      DateTime? createdFrom = null,
      DateTime? createdTo = null,
      Expression<Func<User, object>> orderBy = null,
      bool? orderDesc = null,
      int pageIndex = 0,
      int pageSize = int.MaxValue,
      bool getOnlyTotalCount = false);

    Task<int> DeleteUserAsync(User users);

    Task<int> DeleteUsersAsync(IEnumerable<User> users);

    Task<(List<int> DeletableIds,
        Dictionary<int, DeleteUserResult> Failures)> FilterDeletableUsersAsync(List<int> userIdsToDelete, int currentUserId);


    Task<DeleteUserResult> ValidateUserDeletionAsync(int userIdToDelete, int currentUserId);
    Task<User> GetUserByIdAsync(int userId);

    Task<User> GetUserByUsernameAsync(string username);

    Task<User> GetUserByEmailAsync(string email);

    Task InsertUserAsync(User user);
    Task<BulkOperationResult> BulkSetUserActiveStatusAsync(List<int> userIds, bool isActive, int currentUserId);

    Task UpdateUserAsync(User user);

    Task<List<User>> GetUsersByIdsAsync(List<int> userIds);

    #endregion

    #region addresses
    Task<List<Address>> GetAddressesByUserIdAsync(int userId);

    Task InsertAddressAsync(Address address);

    Task UpdateAddressAsync(Address address);

    Task<Address> GetAddressByAddressIdAsync(int addressId);

    #endregion

    #region role

    Task<IPagedList<Role>> GetAllRolesAsync(
     string friendlyName = null,
     string systemName = null,
     bool? isActive = null,
     bool? isFreeShipping = null,
     bool? isSystemRole = null,
     Expression<Func<Role, object>> orderBy = null,
     bool? orderDesc = null,
     int pageIndex = 0,
     int pageSize = int.MaxValue);

    Task<List<Role>> GetRoleByIdsAsync(List<int> roleIds);

    Task<Role> GetRoleByIdAsync(int roleId);

    Task<Role> GetRoleBySystemNameAsync(string systemName);

    Task InsertRoleAsync(Role role);

    Task UpdateRoleAsync(Role role);

    Task<int> DeleteRoleAsync(Role role);


    #endregion


    #region userRole
    Task UpdateUserRolesAsync(User user, List<int> newRoles);

    Task<int> InsertUserRolesAsync(List<UserRole> userRoles);
    #endregion

}


