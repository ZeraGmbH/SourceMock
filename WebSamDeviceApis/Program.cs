using Microsoft.OpenApi.Models;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Actions.VeinSource;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
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

switch (builder.Configuration["SourceType"])
{
    case "simulated":
        builder.Services.AddSingleton<SimulatedSource>();
        builder.Services.AddSingleton<ISource>(x => x.GetRequiredService<SimulatedSource>());
        builder.Services.AddSingleton<ISimulatedSource>(x => x.GetRequiredService<SimulatedSource>());
        break;
    case "vein":
        builder.Services.AddSingleton<VeinClient>(new VeinClient(new(), "localhost", 8080));
        builder.Services.AddSingleton<VeinSource>();
        builder.Services.AddSingleton<ISource>(x => x.GetRequiredService<VeinSource>());
        break;
    default:
        throw new NotImplementedException($"Unknown SourceType: {builder.Configuration["SourceType"]}");
}



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
