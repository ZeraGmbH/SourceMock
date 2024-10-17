namespace ZIFApi.Models;

/// <summary>
/// 
/// </summary>
public abstract class Command
{
}

/// <summary>
/// 
/// </summary>
public abstract class Command<TRespose> : Command where TRespose : Response
{
}