using ZERA.WebSam.Shared.Provider;

namespace MockDevices.Source;

/// <summary>
/// Implementation of a source which is not configured and can therefore not be used.
/// </summary>
public class UnavailableSource(IDosage? dosage = null) : ZERA.WebSam.Shared.Models.Source.UnavailableSource(dosage), IACSourceMock, IDCSourceMock
{
}