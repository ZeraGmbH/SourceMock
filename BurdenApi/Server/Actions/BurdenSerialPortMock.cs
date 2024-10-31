using BurdenApi.Models;
using SerialPortProxy;

namespace BurdenApi.Actions;

/// <summary>
/// 
/// </summary>
public class BurdenSerialPortMock : ISerialPort
{
    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public byte? RawRead()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void RawWrite(byte[] command)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public string ReadLine()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WriteLine(string command)
    {
        throw new NotImplementedException();
    }
}