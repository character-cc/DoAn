namespace Backend.Data.Domain.Users.Enum;

public enum UserRegisterResult
{
    Successful = 1,

    UsernameOrEmailAlreadyExists = 2,

    InvalidModelState = 3,

}
