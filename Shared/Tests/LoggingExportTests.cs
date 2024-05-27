using System.Xml;
using SharedLibrary.Models.Logging;

namespace SharedLibraryTests;

[TestFixture]
public class LoggingExportTests
{
    [Test]
    public void Can_Create_Logging_Entry_For_Export()
    {
        var entry = new InterfaceLogEntry
        {
            Connection = new()
            {
                Endpoint = "EP",
                Protocol = InterfaceLogProtocolTypes.Tcp,
                WebSamType = InterfaceLogSourceTypes.ReferenceMeter,
                WebSamId = "SID"
            },
            Id = "ID1",
            Info = new()
            {
                CorrelationId = "CORR",
                CreatedAt = new DateTime(2024, 5, 21, 13, 44, 5, 123, DateTimeKind.Utc),
                RunIdentifier = new DateTime(2024, 5, 20, 3, 4, 5, 6, DateTimeKind.Utc),
                SequenceCounter = 128,
                SessionId = "SESS"
            },
            Message = new()
            {
                Encoding = InterfaceLogPayloadEncodings.Raw,
                Payload = "PAYLOAD",
                PayloadType = "test",
                TransferException = "all good"
            },
            Scope = new()
            {
                Outgoing = true,
                RequestId = "RID"
            }
        };

        var exp = entry.ToExport();
        var impEntry = InterfaceLogEntry.FromExport(exp);

        Assert.Multiple(() =>
        {
            Assert.That(impEntry, Is.Not.SameAs(entry));
            Assert.That(impEntry.Id, Is.EqualTo(entry.Id));

            Assert.That(impEntry.Connection, Is.Not.SameAs(entry.Connection));
            Assert.That(impEntry.Connection.Endpoint, Is.EqualTo(entry.Connection.Endpoint));
            Assert.That(impEntry.Connection.Protocol, Is.EqualTo(entry.Connection.Protocol));
            Assert.That(impEntry.Connection.WebSamId, Is.EqualTo(entry.Connection.WebSamId));
            Assert.That(impEntry.Connection.WebSamType, Is.EqualTo(entry.Connection.WebSamType));

            Assert.That(impEntry.Info, Is.Not.SameAs(entry.Info));
            Assert.That(impEntry.Info.CorrelationId, Is.EqualTo(entry.Info.CorrelationId));
            Assert.That(impEntry.Info.CreatedAt, Is.EqualTo(entry.Info.CreatedAt));
            Assert.That(impEntry.Info.RunIdentifier, Is.EqualTo(entry.Info.RunIdentifier));
            Assert.That(impEntry.Info.SequenceCounter, Is.EqualTo(entry.Info.SequenceCounter));
            Assert.That(impEntry.Info.SessionId, Is.EqualTo(entry.Info.SessionId));

            Assert.That(impEntry.Scope, Is.Not.SameAs(entry.Scope));
            Assert.That(impEntry.Scope.Outgoing, Is.EqualTo(entry.Scope.Outgoing));
            Assert.That(impEntry.Scope.RequestId, Is.EqualTo(entry.Scope.RequestId));

            Assert.That(impEntry.Message, Is.Not.SameAs(entry.Message));
            Assert.That(impEntry.Message.Encoding, Is.EqualTo(entry.Message.Encoding));
            Assert.That(impEntry.Message.Payload, Is.EqualTo(entry.Message.Payload));
            Assert.That(impEntry.Message.PayloadType, Is.EqualTo(entry.Message.PayloadType));
            Assert.That(impEntry.Message.TransferException, Is.EqualTo(entry.Message.TransferException));
        });

        var doc = new XmlDocument();

        Assert.That(entry.ToXmlExport(doc).OuterXml, Is.EqualTo("<InterfaceLogEntry><id>ID1</id><webSamType>ReferenceMeter</webSamType><webSamId>SID</webSamId><protocol>Tcp</protocol><endpoint>EP</endpoint><requestId>RID</requestId><outgoing>True</outgoing><sessionId>SESS</sessionId><runIdentifier>2024-05-20T03:04:05.006Z</runIdentifier><correlationId>CORR</correlationId><sequenceCounter>128</sequenceCounter><createdAt>2024-05-21T13:44:05.123Z</createdAt><payload>PAYLOAD</payload><encoding>Raw</encoding><payloadType>test</payloadType><transferException>all good</transferException></InterfaceLogEntry>"));
    }
}