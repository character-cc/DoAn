using System.Linq.Expressions;
using Backend.Data.Domain.Users;

namespace Backend.Common;

public static class SortableFields
{
    public static readonly Dictionary<Type, HashSet<string>> Fields = new()
    {
        [typeof(User)] = new HashSet<string>(StringComparer.OrdinalIgnoreCase){ 
            nameof(User.FullName), 
            nameof(User.Email), 
            nameof(User.PhoneNumber), 
            nameof(User.CreatedAt) 
        },
        [typeof(Role)] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Role.FriendlyName),
            nameof(Role.SystemName),
            nameof(Role.IsActive),
            nameof(Role.CreatedAt),
            nameof(Role.UpdatedAt)
        }

    };

    public static bool IsSortable<T>(string field) =>
        Fields.TryGetValue(typeof(T), out var fields) && fields.Contains(field);


    private static readonly Dictionary<Type, Dictionary<string, LambdaExpression>> _selectors = new()
    {
        [typeof(User)] = new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(User.FullName)] = (Expression<Func<User, object>>)(u => u.FirstName + " " + u.LastName),
            [nameof(User.Email)] = (Expression<Func<User, object>>)(u => u.Email),
            [nameof(User.PhoneNumber)] = (Expression<Func<User, object>>)(u => u.PhoneNumber),
            [nameof(User.CreatedAt)] = (Expression<Func<User, object>>)(u => u.CreatedAt),
        },
         [typeof(Role)] = new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase)
         {
             [nameof(Role.FriendlyName)] = (Expression<Func<Role, object>>)(r => r.FriendlyName),
             [nameof(Role.SystemName)] = (Expression<Func<Role, object>>)(r => r.SystemName),
             [nameof(Role.IsActive)] = (Expression<Func<Role, object>>)(r => r.IsActive),
             [nameof(Role.CreatedAt)] = (Expression<Func<Role, object>>)(r => r.CreatedAt),
             [nameof(Role.UpdatedAt)] = (Expression<Func<Role, object>>)(r => r.UpdatedAt)
         }
    };

    public static Expression<Func<T, object>> GetSelector<T>(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return null;

        if (_selectors.TryGetValue(typeof(T), out var map) &&
            map.TryGetValue(field, out var expr))
        {
            return (Expression<Func<T, object>>)expr;
        }

        return null;
    }


}
