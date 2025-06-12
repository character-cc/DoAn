namespace Backend.Data.Domain.Users.Enum;

public enum UserLoginResult
{
    Successful = 1,


    CustomerNotExist = 2,


    WrongPassword = 3,

    NotActive = 4,


    Deleted = 5,


    NotRegistered = 6,

    InvalidModelState = 7
}
