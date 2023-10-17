using SerialPortProxy;

namespace Tests;

class PortMock : ISerialPort
{
    private readonly Queue<string> _replies = new();

    public void Dispose()
    {
    }

    public string ReadLine() => this._replies.Dequeue();

    public void WriteLine(string command)
    {
        switch (command)
        {
            case "AAV":
                {
                    _replies.Enqueue("MT786V06.33");
                    _replies.Enqueue("AAVACK");

                    break;
                }
        }
    }
}

public class Tests
{
    [Test]
    public async Task Can_Read_Firmware_Version()
    {
        using var cut = SerialPortConnection.FromMock<PortMock>();

        var reply = await cut.Execute(SerialPortRequest.Create("AAV", "AAVACK"))[0];

        Assert.That(reply.Length, Is.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(reply[0], Is.EqualTo("MT786V06.33"));
            Assert.That(reply[1], Is.EqualTo("AAVACK"));
        });
    }

    [Test]
    public async Task Can_Use_Service()
    {
        using var sut = new SerialPortService(new SerialPortConfiguration
        {
            PortNameOrMockType = typeof(PortMock).AssemblyQualifiedName!,
            UseMockType = true
        });

        var reply = await sut.Execute(SerialPortRequest.Create("AAV", "AAVACK"))[0];

        Assert.That(reply.Length, Is.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(reply[0], Is.EqualTo("MT786V06.33"));
            Assert.That(reply[1], Is.EqualTo("AAVACK"));
        });
    }
}