namespace SourceApi.Model;

public class ElectricalQuantity
{
    public double? DcComponent { get; set; }

    public ElectricalVectorQuantity? AcComponent { get; set; }
}