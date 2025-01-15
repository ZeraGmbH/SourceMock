using SerialPortProxy;

namespace ZIFApi.Actions;

/// <summary>
/// 
/// </summary>
public class PowerMaster8121SerialPortMock : ISerialPort
{
    private readonly Queue<byte> _reply = [];

    /// <inheritdoc/>
    public void Dispose() { }

    /// <inheritdoc/>
    public byte? RawRead(int? timeout = null) => _reply.TryDequeue(out var data) ? data : throw new EndOfStreamException("read without a write");

    private void Reply(params byte[] reply) => reply.ToList().ForEach(_reply.Enqueue);

    /// <inheritdoc/>
    public void RawWrite(byte[] command)
    {
        if (command[0] != 0xa5) throw new InvalidOperationException("ETX missing");
        if (command[^1] != 0x5a) throw new InvalidOperationException("STX missing");

        var crc = command[^2];
        var raw = command.Skip(1).Take(command.Length - 3).ToArray();

        if (raw[0] != raw.Length) throw new InvalidOperationException("bad length");

        if (crc != PowerMaster8121.ComputeCRC(raw)) throw new InvalidOperationException("CRC mismatch");

        switch (raw[1])
        {
            case 0x8d:
                Reply(0xA5, 0x03, 0x06, 0x8D, 0x3F, 0x5A);
                break;
            case 0x8e:
                Reply(0xA5, 0x03, 0x06, 0x8E, 0xDD, 0x5A);
                break;
            case 0xc1:
                Reply(0xA5, 0x05, 0x06, 0xC1, 0x6A, 0xEA, 0x09, 0x5A);
                break;
            case 0xc2:
                Reply(0xa5, 0x08, 0x06, 0xc2, 0x02, 0x00, 0x00, 0x00, 0x16, 0x96, 0x5a);
                break;
            case 0xc4:
                Reply(0xA5, 0x04, 0x06, 0xC4, 0x03, 0xB2, 0x5A);
                break;
            default:
                throw new NotSupportedException($"{BitConverter.ToString(raw)}");
        }
    }

    /// <inheritdoc/>
    public string ReadLine() => throw new NotImplementedException();

    /// <inheritdoc/>
    public void WriteLine(string command) => throw new NotImplementedException();
}