using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Backend.Common;

public static class EnumHelper
{
    public static List<EnumOptionDto> ToList<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new EnumOptionDto
            {
                Value = Convert.ToInt32(e),
                SystemName = e.ToString(),
                Label = e.GetType()
                         .GetMember(e.ToString())
                         .First()
                         .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
            })
            .ToList();
    }
}


public class EnumOptionDto
{
    public int Value { get; set; }
    public string Label { get; set; }

    public string SystemName { get; set; }
}

