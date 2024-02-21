using Newtonsoft.Json;

namespace SharedLibrary;

/// <summary>
/// 
/// </summary>
public static class Utils
{
    /// <summary>
    /// Copys values of object, not its reference
    /// </summary>
    /// <typeparam name="T">Any type of objects</typeparam>
    /// <param name="self">object to clone</param>
    /// <returns>a clone of the object</returns>
    public static T DeepCopy<T>(T self) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(self))!;
}