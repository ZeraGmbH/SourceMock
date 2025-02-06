using System.Globalization;
using ZERA.WebSam.Shared.Models.MeterTestSystem;

namespace ZeraDevices.MeterTestSystem;

/// <summary>
/// Provide an error condition model instance based on some reply 
/// over the serial line.
/// </summary>
public static class ErrorConditionParser
{
    /// <summary>
    /// Helper class to analyse a reply string.
    /// </summary>
    private class Flags
    {
        /// <summary>
        /// Reply string converted to bytes - actual values are only nibbles between 0 
        /// and F.
        /// </summary>
        private readonly byte[] _flags;

        /// <summary>
        /// Analyse a response string and create a byte array.
        /// </summary>
        /// <param name="pattern">String pattern of nibbles.</param>
        /// <exception cref="FormatException">Some characters are not nibbles.</exception>
        /// <exception cref="ArgumentException">The response string must have at least 20 characters.</exception>
        public Flags(string pattern)
        {
            /* Must all be hex numbers. */
            _flags = pattern.Select(p => byte.Parse(new string(p, 1), NumberStyles.HexNumber)).ToArray();

            /* Must have a minimum length. */
            if (_flags.Length < 20) throw new ArgumentException("too few flags", nameof(pattern));

        }

        /// <summary>
        /// Retrieve a single bit from the response.
        /// </summary>
        /// <param name="index">Character index to test.</param>
        /// <param name="mask">Bit mask to test.</param>
        /// <returns>Set if the related bit is set.</returns>
        public bool this[int index, byte mask] => (_flags[index] & mask) == mask;

        /// <summary>
        /// Retrieve a single bit from the response.
        /// </summary>
        /// <param name="index">Character index to test.</param>
        /// <param name="mask">Bit mask to test.</param>
        /// <returns>Set if the related bit is set or null if the requested character index is not available.</returns>
        public bool? GetOptionalFlag(int index, byte mask) => index < _flags.Length ? (_flags[index] & mask) == mask : null;
    }

    /// <summary>
    /// Helper layout to access individuel amplifier error indicators.
    /// </summary>
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
    /// Analyse a response string.
    /// </summary>
    /// <param name="pattern">Some response string.</param>
    /// <param name="fg">Set if the response comes from an FG301 which includes
    /// a bit more error indicators as the MT786.</param>
    /// <returns>Error conditions fitting the pattern.</returns>
    public static ErrorConditions Parse(string pattern, bool fg)
    {
        /* Analyse the input. */
        var flags = new Flags(pattern);

        /* Retrieve all global error flags. */
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

        /* Extract the error flags for all amplifiers. */
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

        /* Finally add the FG301 extension information. */
        if (fg)
        {
            result.IctFailure = flags.GetOptionalFlag(20, 0x8);
            result.WrongRangeReferenceMeter = flags.GetOptionalFlag(20, 0x4);
        }

        /* Report result. */
        return result;
    }
}
