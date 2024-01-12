using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;

namespace MeterTestSystemApiTests;

[TestFixture]
public class ErrorConditionTests
{
    /// <summary>
    /// General mock for validating command reply interpretation.
    /// </summary>
    class PortMock : ISerialPort
    {
        private readonly string _reply;

        public PortMock(string reply)
        {
            _reply = reply;
        }

        private readonly Queue<string> _replies = new();

        public void Dispose()
        {
        }

        public virtual string ReadLine()
        {
            if (_replies.TryDequeue(out var reply))
                return reply;

            throw new TimeoutException("no reply in queue");
        }

        public void WriteLine(string command)
        {
            switch (command)
            {
                case "SSM":
                    {
                        _replies.Enqueue(_reply);

                        break;
                    }
            }
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Can_Parse_No_Error_Condition(bool fg)
    {
        var errors = ErrorConditionParser.Parse("00000000000000000000", fg);

        Assert.Multiple(() =>
        {
            Assert.That(errors.EmergencyStop, Is.False);
            Assert.That(errors.HasAmplifierError, Is.False);
            Assert.That(errors.HasFuseError, Is.False);
            Assert.That(errors.IctFailure, Is.Null);
            Assert.That(errors.IsolationFailure, Is.False);
            Assert.That(errors.LwlIdentCorrupted, Is.False);
            Assert.That(errors.ReferenceMeterDataTransmissionError, Is.False);
            Assert.That(errors.VoltageCurrentIsShort, Is.False);
            Assert.That(errors.WrongRangeReferenceMeter, Is.Null);

            foreach (var amplifier in Enum.GetValues<Amplifiers>())
            {
                var amperrors = errors.Amplifiers[amplifier];

                Assert.That(amperrors.HasError, Is.False);
                Assert.That(amperrors.ConnectionMissing, Is.False);
                Assert.That(amperrors.DataTransmission, Is.False);
                Assert.That(amperrors.GroupError, Is.False);
                Assert.That(amperrors.Overload, Is.False);
                Assert.That(amperrors.PowerSupply, Is.False);
                Assert.That(amperrors.ShortOrOpen, Is.False);
                Assert.That(amperrors.Temperature, Is.False);
                Assert.That(amperrors.UndefinedError, Is.False);
            }
        });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Can_Parse_All_Error_Condition(bool fg)
    {
        var errors = ErrorConditionParser.Parse("EFFFFFFFFFFFFFFFFFFFC0", fg);

        Assert.Multiple(() =>
        {
            Assert.That(errors.EmergencyStop, Is.True);
            Assert.That(errors.HasAmplifierError, Is.True);
            Assert.That(errors.HasFuseError, Is.True);
            Assert.That(errors.IctFailure, Is.EqualTo(fg ? true : null));
            Assert.That(errors.IsolationFailure, Is.True);
            Assert.That(errors.LwlIdentCorrupted, Is.True);
            Assert.That(errors.ReferenceMeterDataTransmissionError, Is.True);
            Assert.That(errors.VoltageCurrentIsShort, Is.True);
            Assert.That(errors.WrongRangeReferenceMeter, Is.EqualTo(fg ? true : null));

            foreach (var amplifier in Enum.GetValues<Amplifiers>())
            {
                var amperrors = errors.Amplifiers[amplifier];

                Assert.That(amperrors.HasError, Is.True);
                Assert.That(amperrors.ConnectionMissing, Is.True);
                Assert.That(amperrors.DataTransmission, Is.True);
                Assert.That(amperrors.GroupError, Is.True);
                Assert.That(amperrors.Overload, Is.True);
                Assert.That(amperrors.PowerSupply, Is.True);
                Assert.That(amperrors.ShortOrOpen, Is.True);
                Assert.That(amperrors.Temperature, Is.True);
                Assert.That(amperrors.UndefinedError, Is.True);
            }
        });
    }

    [TestCase("", false)]
    [TestCase("G0000000000000000000", false)]
    [TestCase("0000000000000000000", false)]
    [TestCase("0000000000000000000", true)]
    public void Will_Detect_Invalid_Patterns(string pattern, bool fg)
    {
        Assert.That(() => ErrorConditionParser.Parse(pattern, fg), Throws.Exception);
    }

    [Test]
    public async Task Can_Get_Error_Conditions_From_Serial_Port()
    {
        using var port = new PortMock("SSM42000002002000000800");
        using var device = SerialPortConnection.FromPortInstance(port, new NullLogger<SerialPortConnection>());

        var cut = new SerialPortMTMeterTestSystem(device, null!, null!, new NullLogger<SerialPortMTMeterTestSystem>(), null!);
        var errors = await cut.GetErrorConditions();

        Assert.Multiple(() =>
        {
            Assert.That(errors.EmergencyStop, Is.False);
            Assert.That(errors.HasAmplifierError, Is.False);
            Assert.That(errors.HasFuseError, Is.True);
            Assert.That(errors.IctFailure, Is.Null);
            Assert.That(errors.IsolationFailure, Is.False);
            Assert.That(errors.LwlIdentCorrupted, Is.True);
            Assert.That(errors.ReferenceMeterDataTransmissionError, Is.False);
            Assert.That(errors.VoltageCurrentIsShort, Is.False);
            Assert.That(errors.WrongRangeReferenceMeter, Is.Null);

            foreach (var amplifier in Enum.GetValues<Amplifiers>())
            {
                var amperrors = errors.Amplifiers[amplifier];

                Assert.That(amperrors.HasError, Is.False);
                Assert.That(amperrors.ConnectionMissing, Is.False);
                Assert.That(amperrors.DataTransmission, Is.EqualTo(amplifier == Amplifiers.Current1));
                Assert.That(amperrors.GroupError, Is.False);
                Assert.That(amperrors.Overload, Is.EqualTo(amplifier == Amplifiers.Current3));
                Assert.That(amperrors.PowerSupply, Is.False);
                Assert.That(amperrors.ShortOrOpen, Is.False);
                Assert.That(amperrors.Temperature, Is.EqualTo(amplifier == Amplifiers.Voltage2));
                Assert.That(amperrors.UndefinedError, Is.False);
            }
        });
    }
}