namespace SourceApi.Model;

public class TargetLoadpointPhase : AbstractLoadpointPhase<ActivatableElectricalQuantity>
{
}

public class TargetLoadpointPhaseNG : AbstractLoadpointPhase<ActivatableElectricalVoltageQuantity, ActivatableElectricalCurrentQuantity>
{
}
