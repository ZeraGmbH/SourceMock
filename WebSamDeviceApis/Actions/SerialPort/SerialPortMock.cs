using SerialPortProxy;

namespace WebSamDeviceApis.Actions.SerialPort;

public class SerialPortMock : ISerialPort
{
    private readonly Queue<string> _replies = new();

    public void Dispose()
    {
    }

    public string ReadLine() => this._replies.Dequeue();

    public void WriteLine(string command)
    {
        throw new NotImplementedException(command);
    }
}
