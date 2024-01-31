namespace BarcodeApi.Actions.Device;

/// <summary>
/// Key code mappings for input events.
/// </summary>
public static class KeyMaps
{
    /// <summary>
    /// Regular bar codes.
    /// </summary>
    public static readonly Dictionary<KeyCodes, char> BarcodeRegular = new() {
        {KeyCodes.KEY_0, '0'},
        {KeyCodes.KEY_1, '1'},
        {KeyCodes.KEY_2, '2'},
        {KeyCodes.KEY_3, '3'},
        {KeyCodes.KEY_4, '4'},
        {KeyCodes.KEY_5, '5'},
        {KeyCodes.KEY_6, '6'},
        {KeyCodes.KEY_7, '7'},
        {KeyCodes.KEY_8, '8'},
        {KeyCodes.KEY_9, '9'},
        {KeyCodes.KEY_A, 'a'},
        {KeyCodes.KEY_B, 'b'},
        {KeyCodes.KEY_C, 'c'},
        {KeyCodes.KEY_D, 'd'},
        {KeyCodes.KEY_E, 'e'},
        {KeyCodes.KEY_F, 'f'},
        {KeyCodes.KEY_G, 'g'},
        {KeyCodes.KEY_H, 'h'},
        {KeyCodes.KEY_I, 'i'},
        {KeyCodes.KEY_J, 'j'},
        {KeyCodes.KEY_K, 'k'},
        {KeyCodes.KEY_L, 'l'},
        {KeyCodes.KEY_M, 'm'},
        {KeyCodes.KEY_N, 'n'},
        {KeyCodes.KEY_O, 'o'},
        {KeyCodes.KEY_P, 'p'},
        {KeyCodes.KEY_Q, 'q'},
        {KeyCodes.KEY_R, 'r'},
        {KeyCodes.KEY_S, 's'},
        {KeyCodes.KEY_T, 't'},
        {KeyCodes.KEY_U, 'u'},
        {KeyCodes.KEY_V, 'v'},
        {KeyCodes.KEY_W, 'w'},
        {KeyCodes.KEY_X, 'x'},
        {KeyCodes.KEY_Y, 'y'},
        {KeyCodes.KEY_Z, 'z'},
        {KeyCodes.KEY_DOT, '.'},
        {KeyCodes.KEY_MINUS, '-'},
        {KeyCodes.KEY_SLASH, '/'},
        {KeyCodes.KEY_SPACE, ' '},
    };

    /// <summary>
    /// Shifted bar codes.
    /// </summary>
    public static readonly Dictionary<KeyCodes, char> BarcodeShifted = new() {
        {KeyCodes.KEY_4, '$'},
        {KeyCodes.KEY_5, '%'},
        {KeyCodes.KEY_8, '*'},
        {KeyCodes.KEY_A, 'A'},
        {KeyCodes.KEY_B, 'B'},
        {KeyCodes.KEY_C, 'C'},
        {KeyCodes.KEY_D, 'D'},
        {KeyCodes.KEY_E, 'E'},
        {KeyCodes.KEY_F, 'F'},
        {KeyCodes.KEY_G, 'G'},
        {KeyCodes.KEY_H, 'H'},
        {KeyCodes.KEY_I, 'I'},
        {KeyCodes.KEY_J, 'J'},
        {KeyCodes.KEY_K, 'K'},
        {KeyCodes.KEY_L, 'L'},
        {KeyCodes.KEY_M, 'M'},
        {KeyCodes.KEY_N, 'N'},
        {KeyCodes.KEY_O, 'O'},
        {KeyCodes.KEY_P, 'P'},
        {KeyCodes.KEY_Q, 'Q'},
        {KeyCodes.KEY_R, 'R'},
        {KeyCodes.KEY_S, 'S'},
        {KeyCodes.KEY_T, 'T'},
        {KeyCodes.KEY_U, 'U'},
        {KeyCodes.KEY_V, 'V'},
        {KeyCodes.KEY_W, 'W'},
        {KeyCodes.KEY_X, 'X'},
        {KeyCodes.KEY_Y, 'Y'},
        {KeyCodes.KEY_Z, 'Z'},
        {KeyCodes.KEY_EQUAL, '+'},
    };
}