using Backend.Common;
using Backend.Data.Domain.Users;
using FluentValidation;

namespace Backend.DTO.Users;

public class GetUsersRequest
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public int[] UserRoleId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string SortBy { get; set; }    
    public bool? SortDesc { get; set; }    
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}   
public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
{
    public GetUsersRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(field => string.IsNullOrWhiteSpace(field) || SortableFields.IsSortable<User>( field))
            .WithMessage(x =>
            {
                var allowedFields = string.Join(", ", SortableFields.Fields[typeof(User)]);
                return $"Trường sắp xếp '{x.SortBy}' không hợp lệ. Các trường được phép: {allowedFields}.";
            });
    }
}
