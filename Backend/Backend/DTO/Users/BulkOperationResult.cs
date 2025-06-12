namespace Backend.DTO.Users;

public class BulkOperationResult
{
    public List<int> SuccessIds { get; set; } = new();
    public List<BulkErrorItem> Errors { get; set; } = new();
}

public class BulkErrorItem
{
    public int Id { get; set; }
    public string Reason { get; set; }

    public BulkErrorItem(int id, string reason)
    {
        Id = id;
        Reason = reason;
    }
}