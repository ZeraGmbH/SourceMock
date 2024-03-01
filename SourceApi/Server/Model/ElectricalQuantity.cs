namespace SourceApi.Model;

public class ElectricalQuantity {
    public double DcComponent;
    
    public ElectricalVectorQuantity AcComponent = new();
    
    public bool On { get; set; }
}