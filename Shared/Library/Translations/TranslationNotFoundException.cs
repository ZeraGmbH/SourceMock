namespace SharedLibrary.Translations;

/// <summary>
/// Report the key that does not have a translation 
/// </summary>
/// <remarks>
/// Given missingTranslation could not be found in translations
/// </remarks>
/// <param name="missingTranslation"></param>
public class TranslationNotFoundException(string missingTranslation) : Exception(missingTranslation)
{
}