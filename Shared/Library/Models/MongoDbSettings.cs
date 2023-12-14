namespace SharedLibrary.Models;

/// <summary>
/// Configuration data to access a MongoDb database.
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// Endpoint of the database server.
    /// </summary>
    public string ServerName { get; set; } = "localhost";

    /// <summary>
    /// Port on the database server to connect to.
    /// </summary>
    public int ServerPort { get; set; } = 27017;

    /// <summary>
    /// Name of the database.
    /// </summary>
    public string DatabaseName { get; set; } = "";

    /// <summary>
    /// Optional name of the user.
    /// </summary>
    public string UserName { get; set; } = "";

    /// <summary>
    /// Optional password.
    /// </summary>
    public string Password { get; set; } = "";
}