namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
public class MeasureOutputPhase
{
    /// <summary>
    /// 
    /// </summary>
    public double? Voltage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? Current { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? AngleVoltage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? AngleCurrent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? Angle { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? ActivePower { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? ReactivePower { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? ApparentPower { get; set; }
}

/// <summary>
/// 
/// </summary>
public class MeasureOutput
{
    /// <summary>
    /// 
    /// </summary>
    public int? PhaseOrder { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<MeasureOutputPhase> Phases { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public double? ActivePower { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? ReactivePower { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? ApparentPower { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? Frequency { get; set; }
}
