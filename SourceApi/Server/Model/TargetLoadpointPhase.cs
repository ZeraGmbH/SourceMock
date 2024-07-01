using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

public class TargetLoadpointPhase : AbstractLoadpointPhase<ActivatableElectricalQuantity>
{
}

public class TargetLoadpointPhaseNG : AbstractLoadpointPhase<ActivatableElectricalQuantity<Voltage>, ActivatableElectricalQuantity<Current>>
{
}
