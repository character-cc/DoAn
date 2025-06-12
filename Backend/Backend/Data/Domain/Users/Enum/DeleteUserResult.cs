namespace Backend.Data.Domain.Users.Enum;

public enum DeleteUserResult
{
    Success = 0,
    NotFound = 1,
    CannotDeleteSelf = 2,
    CannotDeleteAdmin = 3,
}
