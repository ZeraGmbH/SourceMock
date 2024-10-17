using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;

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

            Thread.Sleep(100);

            throw new TimeoutException("no reply in queue");
        }

        public void WriteLine(string command)
        {
            switch (command)
            {
                case "SSM":
                case "SM":
                    {
                        _replies.Enqueue(_reply);
                        break;
                    }
            }
        }

        public void RawWrite(byte[] command) => throw new NotImplementedException();

        public byte? RawRead() => throw new NotImplementedException();
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
    public async Task Can_Get_MT_Error_Conditions_From_Serial_Port_Async()
    {
        using var port = new PortMock("SSM42000002002000000800");
        using var device = SerialPortConnection.FromMockedPortInstance(port, new NullLogger<SerialPortConnection>());

        var cut = new SerialPortMTMeterTestSystem(device, null!, null!, new NullLogger<SerialPortMTMeterTestSystem>(), null!);

        var byEvent = new List<ErrorConditions>();

        cut.ErrorConditionsChanged += errors =>
        {
            lock (byEvent)
                byEvent.Add(errors);
        };

        var errors = await cut.GetErrorConditionsAsync(new NoopInterfaceLogger());

        Thread.Sleep(500);

        Assert.That(byEvent, Has.Count.EqualTo(1));
        Assert.That(byEvent[0], Is.Not.SameAs(errors));

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

    [Test]
    public async Task Can_Get_FG_Error_Conditions_From_Serial_Port_Async()
    {
        using var port = new PortMock("SM42000002002000000800");
        using var device = SerialPortConnection.FromMockedPortInstance(port, new NullLogger<SerialPortConnection>());

        var cut = new SerialPortFGMeterTestSystem(device, new NullLogger<SerialPortFGMeterTestSystem>(), null!);

        var byEvent = new List<ErrorConditions>();

        cut.ErrorConditionsChanged += errors =>
        {
            lock (byEvent)
                byEvent.Add(errors);
        };

        var errors = await cut.GetErrorConditionsAsync(new NoopInterfaceLogger());

        Thread.Sleep(500);

        Assert.That(byEvent, Has.Count.EqualTo(1));
        Assert.That(byEvent[0], Is.Not.SameAs(errors));

        foreach (var test in new[] { errors, byEvent[0] })
            Assert.Multiple(() =>
            {
                Assert.That(test.EmergencyStop, Is.False);
                Assert.That(test.HasAmplifierError, Is.False);
                Assert.That(test.HasFuseError, Is.True);
                Assert.That(test.IctFailure, Is.Null);
                Assert.That(test.IsolationFailure, Is.False);
                Assert.That(test.LwlIdentCorrupted, Is.True);
                Assert.That(test.ReferenceMeterDataTransmissionError, Is.False);
                Assert.That(test.VoltageCurrentIsShort, Is.False);
                Assert.That(test.WrongRangeReferenceMeter, Is.Null);

                foreach (var amplifier in Enum.GetValues<Amplifiers>())
                {
                    var amperrors = test.Amplifiers[amplifier];

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