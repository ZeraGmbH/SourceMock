using System.Globalization;
using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Translations;

/// <summary>
/// Translations for test reports are provided 
/// </summary>
public interface ITranslationProvider
{
    /// <summary>
    /// throws TranslationNotFoundException
    /// </summary>
    /// <param name="key"></param>
    /// <returns>Translation that is defined for the given key in the translation file</returns>
    string this[string key] { get; }

    /// <summary>
    /// 
    /// </summary>
    string this[Enum? key] { get; }

    /// <summary>
    /// 
    /// </summary>
    string this[bool? key] { get; }

    /// <summary>
    /// Defines which translation file will be used, eg. report.xlf or messages.de.xlf 
    /// from locale folder in frontend or user uploaded xlif file
    /// </summary>
    /// <param name="language"></param>
    Task SetTranslation(string language);

    /// <summary>
    /// Report the last language set by SetTranslation.
    /// </summary>
    CultureInfo CultureInfo { get; }
}

/// <summary>
/// 
/// </summary>
public static class ITranslationProviderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="translations"></param>
    /// <param name="dateTime"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string? ToString(this ITranslationProvider translations, DateTime? dateTime, string? format = null)
        => dateTime?.ToLocalTime().ToString(format, translations.CultureInfo).Replace(" ", " "); // PDF will not display https://www.compart.com/de/unicode/U+202F correctly

    /// <summary>
    /// 
    /// </summary>
    /// <param name="translations"></param>
    /// <param name="number"></param>
    /// <param name="format"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string? ToString(this ITranslationProvider translations, double? number, string? format = null, string? unit = null)
    {
        var asString = number?.ToString(format, translations.CultureInfo);

        return string.IsNullOrEmpty(asString) || string.IsNullOrEmpty(unit) ? asString : $"{asString}{unit}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="translations"></param>
    /// <param name="number"></param>
    /// <param name="format"></param>
    /// <param name="withUnit"></param>
    /// <param name="spaceUnit"></param>
    /// <returns></returns>
    public static string? ToString(this ITranslationProvider translations, IDomainSpecificNumber? number, string? format = null, bool withUnit = false, bool spaceUnit = false)
        => number?.Format(format, translations.CultureInfo, withUnit, spaceUnit);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="translations"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string WithValue(this ITranslationProvider translations, string key, string? value) => $"{translations[key]}: {value}";
}