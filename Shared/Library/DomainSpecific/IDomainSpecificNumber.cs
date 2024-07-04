namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Base class for all domain specific numbers.
/// </summary>
public interface IDomainSpecificNumber
{
    /// <summary>
    /// The natural unit of the number.
    /// </summary>
    string Unit { get; }
}

/// <summary>
/// Base class for all domain specific numbers.
/// </summary>
public interface IDomainSpecificNumber<T> : IDomainSpecificNumber where T : IDomainSpecificNumber<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    static abstract T operator +(T left, T right);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    static abstract T operator -(T left, T right);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltage"></param>
    /// <returns></returns>
    static abstract bool operator !(T voltage);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="factor"></param>
    /// <returns></returns>
    static abstract T operator *(T left, double factor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="factor"></param>
    /// <returns></returns>
    static abstract T operator *(double factor, T left);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    static abstract double operator /(T left, T right);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <returns></returns>
    static abstract explicit operator double(T left);
}
