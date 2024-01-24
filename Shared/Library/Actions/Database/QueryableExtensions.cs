using SharedLibrary.ExceptionHandling;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// Helper function for database access.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Get a single result from a query. At most two
    /// items are requested from the server.
    /// </summary>
    /// <typeparam name="T">Type of the item.</typeparam>
    /// <param name="query">Query instance.</param>
    /// <returns>The desired item</returns>
    public static T FastSingle<T>(this IQueryable<T> query)
    {
        try
        {
            return query.Take(2).Single();
        }
        catch (InvalidOperationException)
        {
            throw new Exception("Unique item could not be found in the database.");
        }
    }
}