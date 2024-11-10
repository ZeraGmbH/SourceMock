using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.ConfigurationNG.Source;

/// <summary>
/// 
/// </summary>
[JsonDerivedType(typeof(FG30xSourceProviderConfiguration), typeDiscriminator: "FG30x")]
[JsonDerivedType(typeof(MockSourceProviderConfiguration), typeDiscriminator: "Mock")]
[JsonDerivedType(typeof(MT786SourceProviderConfiguration), typeDiscriminator: "MT786")]
[JsonDerivedType(typeof(RestSourceProviderConfiguration), typeDiscriminator: "REST")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "_type")]
public abstract class SourceProviderConfiguration
{
}
