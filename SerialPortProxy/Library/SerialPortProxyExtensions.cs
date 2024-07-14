using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SerialPortProxy;

/// <summary>
/// Configure a web server.
/// </summary>
public static class SerialPortProxyConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseSerialPortProxy(this IServiceCollection services, IConfiguration configuration)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSerialPortProxy(this MvcOptions options)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSerialPortProxy(this SwaggerGenOptions options)
    {
        SwaggerModelExtender
            .AddType<SerialPortErrorCodes>()
            .Register(options);
    }
}