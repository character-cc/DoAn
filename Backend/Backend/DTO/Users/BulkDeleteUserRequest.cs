namespace Backend.DTO.Users;

public class BulkDeleteUserRequest
{
    public List<int> UserIds { get; set; } = new();
}
