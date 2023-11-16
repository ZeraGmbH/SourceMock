namespace RefMeterApi.Models;

/// <summary>
/// Current status of the error measurement.
/// </summary>
public class ErrorMeasurementStatus
{
    /// <summary>
    /// 
    /// </summary>
    public ErrorMeasurementStates State { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double ErrorValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double Progress { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double Energy { get; set; }
}