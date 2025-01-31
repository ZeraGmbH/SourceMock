using System.Text.RegularExpressions;
using BarcodeApi.Actions.Device;

namespace BarcodeApi.Actions;

/// <summary>
/// Information on system devices.
/// </summary>
public interface IInputDeviceManager
{
    /// <summary>
    /// Ask system for current list of input devices.
    /// </summary>    /// <returns></returns>
    Task<List<IInputDevice>> GetAsync();
}

/// <summary>
/// Helper class for input device manager.
/// </summary>
public static class IInputDeviceManagerExtensions
{
    private static Regex _EventReg = new("^event(\\d{1,8})$");

    private static readonly ulong _RegularKeys = new[]
    {
        KeyCodes.KEY_0,
        KeyCodes.KEY_1,
        KeyCodes.KEY_2,
        KeyCodes.KEY_3,
        KeyCodes.KEY_4,
        KeyCodes.KEY_5,
        KeyCodes.KEY_6,
        KeyCodes.KEY_7,
        KeyCodes.KEY_8,
        KeyCodes.KEY_9,
        KeyCodes.KEY_A,
        KeyCodes.KEY_B,
        KeyCodes.KEY_C,
        KeyCodes.KEY_D,
        KeyCodes.KEY_E,
        KeyCodes.KEY_F,
        KeyCodes.KEY_G,
        KeyCodes.KEY_H,
        KeyCodes.KEY_I,
        KeyCodes.KEY_J,
        KeyCodes.KEY_K,
        KeyCodes.KEY_L,
        KeyCodes.KEY_M,
        KeyCodes.KEY_N,
        KeyCodes.KEY_O,
        KeyCodes.KEY_P,
        KeyCodes.KEY_Q,
        KeyCodes.KEY_R,
        KeyCodes.KEY_S,
        KeyCodes.KEY_T,
        KeyCodes.KEY_U,
        KeyCodes.KEY_V,
        KeyCodes.KEY_W,
        KeyCodes.KEY_X,
        KeyCodes.KEY_Y,
        KeyCodes.KEY_Z,
    }
    .Select(c => ((ulong)1) << (int)c)
    .Aggregate((ulong)0, (l, r) => l | r);

    /// <summary>
    /// Load all devices and filter out potential keyboard HID devices
    /// </summary>
    /// <param name="manager">Device manager to use.</param>
    /// <returns>Any device looking like a keyboard.</returns>
    public static async Task<List<IInputDevice>> GetKeyboardHIDDevices(this IInputDeviceManager manager)
    {
        var all = await manager.GetAsync();

        return [..all.Where(a => {
            var handlers = a.GetList("Handlers");

            if(handlers == null) return false;

            /* Check for HID keyboard. */
            return handlers.Contains("kbd") && handlers.Any(h => h.StartsWith("event"));
        })];
    }

    /// <summary>
    /// Load all devices and filter out potential barcode HID devices
    /// </summary>
    /// <param name="manager">Device manager to use.</param>
    /// <param name="firstEvent">First event to check for.</param>
    /// <returns>Any device looking like a barcode keyboard.</returns>
    public static async Task<List<IInputDevice>> GetHIDBarcodeCandidateDevices(this IInputDeviceManager manager, int firstEvent)
    {
        var all = await manager.GetKeyboardHIDDevices();

        return [..all.Where(a => {
            /* Only respect connected devivces .*/
            if(a.GetProperty("Phys")?.StartsWith("usb-") != true) return false;

            /* Check for minimum scan number. */
            var handlers = a.GetList("Handlers");

            if(handlers == null) return false;

            if (handlers.All(key => {
                var match = _EventReg.Match(key);

                return !match.Success || int.Parse(match.Groups[1].Value) < firstEvent;
            }))
            return false;

            /* Check for at least regular keys to be present. */
            var bits = a.GetBits("KEY");

            if(bits == null || bits.Length < 1) return false;

            return (bits[0]&_RegularKeys) == _RegularKeys;
        })];
    }
}