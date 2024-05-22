using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SharedLibrary.Models.Logging;

/// <summary>
/// Describe an interface activity.
/// </summary>
public class InterfaceLogEntry : IDatabaseObject
{
    /// <summary>
    /// Unique identifier of the database entry.
    /// </summary>
    [NotNull, Required]
    public required string Id { get; set; }

    /// <summary>
    /// Connection to the device.
    /// </summary>
    [NotNull, Required]
    public required InterfaceLogEntryConnection Connection { get; set; }

    /// <summary>
    /// The context of the communication.
    /// </summary>
    [NotNull, Required]
    public required InterfaceLogEntryScope Scope { get; set; }

    /// <summary>
    /// System information on the log entry.
    /// </summary>
    [NotNull, Required]
    public required InterfaceLogEntryInfo Info { get; set; }

    /// <summary>
    /// Payload of the data.
    /// </summary>
    public InterfaceLogPayload Message { get; set; } = null!;

    private static readonly JsonSerializerSettings _settings = new()
    {
        ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() },
        Converters = { new StringEnumConverter() }
    };

    private static readonly JsonSerializer _serializer = new() { ContractResolver = _settings.ContractResolver, Converters = { _settings.Converters[0] } };

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private JObject ToJsonObject()
    {
        var asJson = JObject.FromObject(this, _serializer);

        /* Flatten object structure. */
        var build = new JObject { ["id"] = asJson["id"] };

        if (asJson["connection"] is JObject connection)
            foreach (var prop in connection)
                build[prop.Key] = prop.Value;

        if (asJson["info"] is JObject info)
            foreach (var prop in info)
                build[prop.Key] = prop.Value;

        if (asJson["scope"] is JObject scope)
            foreach (var prop in scope)
                build[prop.Key] = prop.Value;

        if (asJson["message"] is JObject message)
            foreach (var prop in message)
                build[prop.Key] = prop.Value;

        return build;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string ToExport() => JsonConvert.SerializeObject(ToJsonObject());

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public XmlElement ToXmlExport(XmlDocument doc)
    {
        var entry = doc.CreateElement("InterfaceLogEntry");
        var build = ToJsonObject();

        foreach (var prop in build)
            entry.AppendChild(doc.CreateElement(prop.Key))!.InnerText = prop.Value?.ToString() ?? "";

        return entry;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exported"></param>
    /// <returns></returns>
    public static InterfaceLogEntry FromExport(string exported)
    {
        /* Reconstruct objects from flat row. */
        var entry = JsonConvert.DeserializeObject<InterfaceLogEntry>(exported, _settings)!;

        entry.Connection = JsonConvert.DeserializeObject<InterfaceLogEntryConnection>(exported, _settings)!;
        entry.Info = JsonConvert.DeserializeObject<InterfaceLogEntryInfo>(exported, _settings)!;
        entry.Message = JsonConvert.DeserializeObject<InterfaceLogPayload>(exported, _settings)!;
        entry.Scope = JsonConvert.DeserializeObject<InterfaceLogEntryScope>(exported, _settings)!;

        if (entry.Message.IsEmpty) entry.Message = null!;

        return entry;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exported"></param>
    /// <returns></returns>
    public static InterfaceLogEntry FromExport(JObject exported)
    {
        /* Reconstruct objects from flat row. */
        var entry = exported.ToObject<InterfaceLogEntry>(_serializer)!;

        entry.Connection = exported.ToObject<InterfaceLogEntryConnection>(_serializer)!;
        entry.Info = exported.ToObject<InterfaceLogEntryInfo>(_serializer)!;
        entry.Message = exported.ToObject<InterfaceLogPayload>(_serializer)!;
        entry.Scope = exported.ToObject<InterfaceLogEntryScope>(_serializer)!;

        if (entry.Message.IsEmpty) entry.Message = null!;

        return entry;
    }
}