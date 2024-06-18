using System.Xml;

namespace SharedLibrary.Translations;

/// <summary>
/// 
/// </summary>
public interface ITranslationStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="xlfFile"></param>
    /// <returns></returns>
    Task<List<Translation>> UpdateTranslations(XmlDocument xlfFile);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    Task<List<Translation>> GetAllTranslations(string language);
}