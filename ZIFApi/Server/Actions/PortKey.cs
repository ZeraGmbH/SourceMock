namespace ZIFApi.Actions;

/// <summary>
/// Key from meter form and service type.
/// </summary>
public readonly struct PortKey(string meterForm, string serviceType)
{
    /// <summary>
    /// The meter form.
    /// </summary>
    public readonly string MeterForm = meterForm;

    /// <summary>
    /// The service type.
    /// </summary>
    public readonly string ServiceType = serviceType;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is PortKey other && MeterForm.Equals(other.MeterForm) && ServiceType.Equals(other.ServiceType);

    /// <inheritdoc/>
    public override int GetHashCode()
        => MeterForm.GetHashCode() ^ ServiceType.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => $"{MeterForm}:{ServiceType}";
}
