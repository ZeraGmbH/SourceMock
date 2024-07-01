using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

public class TargetLoadpointPhase : AbstractLoadpointPhase<ActivatableElectricalQuantity>
{
}

public class TargetLoadpointPhaseNGX : AbstractLoadpointPhase<ActivatableElectricalQuantity<Voltage>, ActivatableElectricalQuantity<Current>>
{
}
