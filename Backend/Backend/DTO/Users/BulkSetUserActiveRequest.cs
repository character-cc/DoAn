namespace Backend.DTO.Users;

public class BulkSetUserActiveRequest
{
    public List<int> UserIds { get; set; } = new();
    public bool IsActive { get; set; }
}
