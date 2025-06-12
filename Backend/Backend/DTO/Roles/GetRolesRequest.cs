using Backend.Common;
using Backend.Data.Domain.Users;
using FluentValidation;

namespace Backend.DTO.Roles;

public class GetRolesRequest
{
    public string FriendlyName { get; set; }
    public string SystemName { get; set; }
    public bool? IsActive { get; set; }

    public string SortBy { get; set; }
    public bool? SortDesc { get; set; }

    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}

public class GetRolesRequestValidator : AbstractValidator<GetRolesRequest>
{
    public GetRolesRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(field => string.IsNullOrWhiteSpace(field) || SortableFields.IsSortable<Role>(field))
            .WithMessage(x =>
            {
                var allowedFields = string.Join(", ", SortableFields.Fields[typeof(Role)]);
                return $"Trường sắp xếp '{x.SortBy}' không hợp lệ. Các trường được phép: {allowedFields}.";
            });
    }
}

