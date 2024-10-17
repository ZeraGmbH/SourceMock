using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;
using ZIFApi.Models;

namespace ZIFApi.Actions;

/// <summary>
/// 
/// </summary>
public class PowerMaster8121([FromKeyedServices("ZIF")] ISerialPortConnection factory) : IZIFDevice
{
    private static readonly byte[] _crcTable =
    {
        0,  94, 188, 226,  97,  63, 221, 131, 194, 156, 126,  32, 163, 253,  31,  65,
        157, 195,  33, 127, 252, 162,  64,  30,  95,   1, 227, 189,  62,  96, 130, 220,
        35, 125, 159, 193,  66,  28, 254, 160, 225, 191,  93,   3, 128, 222,  60,  98,
        190, 224,   2,  92, 223, 129,  99,  61, 124,  34, 192, 158,  29,  67, 161, 255,
        70,  24, 250, 164,  39, 121, 155, 197, 132, 218,  56, 102, 229, 187,  89,   7,
        219, 133, 103,  57, 186, 228,   6,  88,  25,  71, 165, 251, 120,  38, 196, 154,
        101,  59, 217, 135,   4,  90, 184, 230, 167, 249,  27,  69, 198, 152, 122,  36,
        248, 166,  68,  26, 153, 199,  37, 123,  58, 100, 134, 216,  91,   5, 231, 185,
        140, 210,  48, 110, 237, 179,  81,  15,  78,  16, 242, 172,  47, 113, 147, 205,
        17,  79, 173, 243, 112,  46, 204, 146, 211, 141, 111,  49, 178, 236,  14,  80,
        175, 241,  19,  77, 206, 144, 114,  44, 109,  51, 209, 143,  12,  82, 176, 238,
        50, 108, 142, 208,  83,  13, 239, 177, 240, 174,  76,  18, 145, 207,  45, 115,
        202, 148, 118,  40, 171, 245,  23,  73,   8,  86, 180, 234, 105,  55, 213, 139,
        87,   9, 235, 181,  54, 104, 138, 212, 149, 203,  41, 119, 244, 170,  72,  22,
        233, 183,  85,  11, 136, 214,  52, 106,  43, 117, 151, 201,  74,  20, 246, 168,
        116,  42, 200, 150,  21,  75, 169, 247, 182, 232,  10,  84, 215, 137, 107,  53
    };

    private static byte ComputeCRC(IEnumerable<byte> data)
    {
        byte crc = 0;

        foreach (var dt in data)
            crc = _crcTable[(byte)(crc ^ dt)];

        return crc;
    }

    private enum ReadState { Start, End, Length, Data, Checksum };

    private static byte[] ReadResponse(ISerialPort port, byte cmd)
    {
        var data = new List<byte>();
        int length = 0;

        for (var state = ReadState.Start; ;)
        {
            var next = port.RawRead() ?? throw new EndOfStreamException("no more data");

            switch (state)
            {
                case ReadState.Start:
                    if (next == 0xa5) state = ReadState.Length;

                    break;
                case ReadState.Length:
                    if (next < 1)
                        state = ReadState.Start;
                    else
                    {
                        data.Clear();
                        data.Add(next);

                        length = next;

                        state = --length == 0 ? ReadState.Checksum : ReadState.Data;
                    }

                    break;
                case ReadState.Data:
                    data.Add(next);

                    if (--length == 0) state = ReadState.Checksum;

                    break;
                case ReadState.Checksum:
                    var crc = ComputeCRC(data);

                    state = crc == next ? ReadState.End : ReadState.Start;

                    break;
                case ReadState.End:
                    if (next == 0x5a)
                    {
                        if (data[2] != cmd) throw new InvalidOperationException("Out-Of-Band reply");

                        return data[1] switch
                        {
                            0x06 => [.. data.Skip(3)],
                            0x15 => throw new InvalidOperationException("got NACK"),
                            _ => throw new InvalidOperationException("ACK expected"),
                        };
                    }

                    state = ReadState.Start;

                    break;
            }
        }
    }

    /// <inheritdoc/>
    public Task<TResponse> Execute<TResponse>(Command<TResponse> cmd, IInterfaceLogger logger) where TResponse : Response
    {
        // See if we know about this command.
        if (!_CommandParsers.TryGetValue(cmd.GetType(), out var parser))
            throw new ArgumentException($"unsupported ZIF command type {cmd.GetType}", nameof(cmd));

        // Create the byte sequence representing the command.
        var bytes = parser.Item1(cmd);

        // - STX and length of command bytes.
        var buffer = new List<byte> { 0xa5, checked((byte)(1 + bytes.Length)) };

        // - Command bytes.
        buffer.AddRange(bytes);

        // - CRC8(MAXIM) checksum.
        buffer.Add(ComputeCRC(buffer.Skip(1)));

        // - ETX
        buffer.Add(0x5a);

        // It's now time to get exclusive access to the serial port.
        return factory.CreateExecutor(InterfaceLogSourceTypes.DeviceUnderTest).RawExecute(logger, port =>
        {
            // Send to device.
            port.RawWrite([.. buffer]);

            // Read the raw response.
            var response = ReadResponse(port, buffer[2]);

            // Parse and report.
            return (TResponse)parser.Item2(response);
        });
    }

    /// <summary>
    /// 8121 specifc algorithms to create command sequences and parse responses.
    /// </summary>
    private static readonly Dictionary<Type, Tuple<Func<Command, byte[]>, Func<byte[], Response>>> _CommandParsers = new()
    {
        // Retrieve the software version. 
        { typeof(GetVersion), Tuple.Create<Func<Command, byte[]>, Func<byte[], Response>>(
            (cmd) => [0xc2],
            (data) => new VersionInfo { Major = BitConverter.ToInt32(data), Minor = data[4] }
        ) }
    };


}