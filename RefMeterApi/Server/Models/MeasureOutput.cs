namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
public class MeasureOutputPhase
{
    /// <summary>
    /// 
    /// </summary>
    public double? Voltage;

    /// <summary>
    /// 
    /// </summary>
    public double? Current;

    /// <summary>
    /// 
    /// </summary>
    public double? AngleVoltage;

    /// <summary>
    /// 
    /// </summary>
    public double? AngleCurrent;

    /// <summary>
    /// 
    /// </summary>
    public double? Angle;

    /// <summary>
    /// 
    /// </summary>
    public double? ActivePower;

    /// <summary>
    /// 
    /// </summary>
    public double? ReactivePower;

    /// <summary>
    /// 
    /// </summary>
    public double? ApparentPower;
}

/// <summary>
/// 
/// </summary>
public class MeasureOutput
{
    /// <summary>
    /// 
    /// </summary>
    public int? PhaseOrder;

    /// <summary>
    /// 
    /// </summary>
    public readonly List<MeasureOutputPhase> Phases = new();

    /// <summary>
    /// 
    /// </summary>
    public double? ActivePower = new();

    /// <summary>
    /// 
    /// </summary>
    public double? ReactivePower = new();

    /// <summary>
    /// 
    /// </summary>
    public double? ApparentPower = new();

    /// <summary>
    /// 
    /// </summary>
    public double? Frequency = new();
}
