using Microsoft.OpenApi.Models;

using SerialPortProxy;

using System.Globalization;

using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.ExceptionHandling;
using ZERA.WebSam.Shared.Models.Logging;

CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(string.Empty);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();

    options.UseSharedLibrary();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SourceApi",
        Description = "A Web API for controlling a source.",
        Contact = new OpenApiContact
        {
            Name = "ZERA GmbH",
            Url = new Uri("https://www.zera.de/en/contact/")
        }
    });

    options.UseSerialPortProxy();
    options.UseSharedLibrary();
});

builder.Services.AddApiVersioning();
builder.Services.AddVersionedApiExplorer(options =>
{
    // the default is ToString(), but we want "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

#pragma warning disable IDE0053 // body expression looks way more cluttered
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("localhost",
                            "127.0.0.1").
                AllowAnyMethod().
                AllowAnyHeader();
    });
});
#pragma warning restore IDE0053

builder.Services.UseSharedLibrary(builder.Configuration);

builder.Services.AddScoped<IInterfaceLogger, NoopInterfaceLogger>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
    app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
