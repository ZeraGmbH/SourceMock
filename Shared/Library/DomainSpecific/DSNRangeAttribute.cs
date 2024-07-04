using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// 
/// </summary>
public class DSNRangeAttribute(double min, double max) : ValidationAttribute
{
    private readonly double _min = min;

    private readonly double _max = max;

    /// <inheritdoc/>
    public override bool IsValid(object? value)
        => value is IInternalDomainSpecificNumber num && num.GetValue() >= _min && num.GetValue() <= _max;
}