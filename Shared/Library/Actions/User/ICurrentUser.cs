using System.Security.Claims;

namespace SharedLibrary.Actions.User;

/// <summary>
/// Provide information on the current user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Report the information in the current user.
    /// </summary>
    ClaimsPrincipal User { get; }

    /// <summary>
    /// Initialze the current user from a externally provided token.
    /// </summary>
    /// <param name="userToken">A token to identify a user.</param>
    void FromToken(string userToken);
}

/// <summary>
/// 
/// </summary>
public static class ICurrentUserExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static string GetUserId(this ICurrentUser user) => LibUtils.GetUserId(user?.User) ?? "anonymous";
}