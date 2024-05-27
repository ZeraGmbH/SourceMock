using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

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
    public string Id { get; set; } = null!;

    /// <summary>
    /// Connection to the device.
    /// </summary>
    [NotNull, Required]
    public InterfaceLogEntryConnection Connection { get; set; } = null!;

    /// <summary>
    /// The context of the communication.
    /// </summary>
    [NotNull, Required]
    public InterfaceLogEntryScope Scope { get; set; } = null!;

    /// <summary>
    /// System information on the log entry.
    /// </summary>
    [NotNull, Required]
    public InterfaceLogEntryInfo Info { get; set; } = null!;

    /// <summary>
    /// Payload of the data.
    /// </summary>
    public InterfaceLogPayload Message { get; set; } = null!;


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string ToExport()
    {
        /* Flatten all properties. */
        List<string> build2 = [$"\"id\": {JsonSerializer.Serialize(Id)}"];

        foreach (var prop in GetType().GetProperties())
        {
            var value = JsonSerializer.Serialize(prop.GetValue(this), LibUtils.JsonSettings);

            if (value.StartsWith('{'))
                build2.Add(value[1..^1]);
        }

        /* Manually make it a JSON object. */
        return $"{{{string.Join(",", build2)}}}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public XmlElement ToXmlExport(XmlDocument doc)
    {
        var entry = doc.CreateElement("InterfaceLogEntry");

        if (JsonSerializer.Deserialize<ExpandoObject>(ToExport()) is IDictionary<string, object> dict)
            foreach (var prop in dict)
                entry.AppendChild(doc.CreateElement(prop.Key))!.InnerText = prop.Value?.ToString() ?? "";

        return entry;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exported"></param>
    /// <returns></returns>
    public static InterfaceLogEntry FromExport(string exported)
        => FromExport(JsonNode.Parse(exported)!);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exported"></param>
    /// <returns></returns>
    public static InterfaceLogEntry FromExport(JsonNode exported)
    {
        /* Reconstruct objects from flat row. */
        var entry = exported.Deserialize<InterfaceLogEntry>(LibUtils.JsonSettings)!;

        entry.Connection = exported.Deserialize<InterfaceLogEntryConnection>(LibUtils.JsonSettings)!;
        entry.Info = exported.Deserialize<InterfaceLogEntryInfo>(LibUtils.JsonSettings)!;
        entry.Message = exported.Deserialize<InterfaceLogPayload>(LibUtils.JsonSettings)!;
        entry.Scope = exported.Deserialize<InterfaceLogEntryScope>(LibUtils.JsonSettings)!;

        if (entry.Message.IsEmpty) entry.Message = null!;

        return entry;
    }
}