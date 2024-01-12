using System.Globalization;
using System.Text.RegularExpressions;
using MeterTestSystemApi.Models;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public static class ErrorConditionParser
{
    private class Flags
    {
        private readonly byte[] _flags;

        public Flags(string pattern, bool fg)
        {
            /* Must all be hex numbers. */
            _flags = pattern.Select(p => Byte.Parse(new string(p, 1), NumberStyles.HexNumber)).ToArray();

            /* Must have a minimum length. */
            if (_flags.Length < (fg ? 21 : 20)) throw new ArgumentException(nameof(pattern));

        }

        public bool this[int index, byte mask] => (_flags[index] & mask) == mask;
    }

    private static readonly Dictionary<Amplifiers, byte>[] _AmplifierMasks = [
        new(){
            { Amplifiers.Auxiliary2, 0x8 },
            { Amplifiers.Auxiliary1, 0x4 },
            { Amplifiers.Current3, 0x2 },
            { Amplifiers.Current2, 0x1 }, },
        new(){
            { Amplifiers.Current1, 0x8 },
            { Amplifiers.Voltage3, 0x4 },
            { Amplifiers.Voltage2, 0x2 },
            { Amplifiers.Voltage1, 0x1 },
        } ];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="fg"></param>
    /// <returns></returns>
    public static ErrorConditions Parse(string pattern, bool fg)
    {
        /* Analyse the input. */
        var flags = new Flags(pattern, fg);

        /* Create result. */
        var result = new ErrorConditions
        {
            EmergencyStop = flags[1, 0x8],
            HasAmplifierError = flags[0, 0x8],
            HasFuseError = flags[0, 0x4],
            IsolationFailure = flags[1, 0x4],
            LwlIdentCorrupted = flags[1, 0x2],
            ReferenceMeterDataTransmissionError = flags[1, 0x1],
            VoltageCurrentIsShort = flags[0, 0x2],
        };

        for (var i = 0; i < _AmplifierMasks.Length; i++)
            foreach (var amplifier in _AmplifierMasks[i])
            {
                result.Amplifiers[amplifier.Key].HasError = flags[2 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].ShortOrOpen = flags[4 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].Temperature = flags[6 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].PowerSupply = flags[8 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].Overload = flags[10 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].GroupError = flags[12 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].ConnectionMissing = flags[14 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].DataTransmission = flags[16 + i, amplifier.Value];
                result.Amplifiers[amplifier.Key].UndefinedError = flags[18 + i, amplifier.Value];
            }

        if (fg)
        {
            result.IctFailure = flags[20, 0x8];
            result.WrongRangeReferenceMeter = flags[20, 0x4];
        }

        /* Report result. */
        return result;
    }
}
