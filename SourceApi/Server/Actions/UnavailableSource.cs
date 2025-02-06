using SourceApi.Actions.Source;
using SourceApi.Actions.SimulatedSource;
using ZERA.WebSam.Shared.Provider;

namespace SourceApi.Actions;

/// <summary>
/// Implementation of a source which is not configured and can therefore not be used.
/// </summary>
public class UnavailableSource(IDosage? dosage = null) : ZERA.WebSam.Shared.Models.Source.UnavailableSource(dosage), IACSourceMock, IDCSourceMock
{
}