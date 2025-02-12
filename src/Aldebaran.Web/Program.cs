using Aldebaran.Web.Extensions;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
var app = builder.Build();
Configure(app);
app.Run();

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.AddArchitecture();
    builder.AddInfraestructure();
    builder.Services.AddMemoryCache();

    var cultureInfo = new CultureInfo("es-CO");
    NumberFormatInfo numberFormatInfo = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
    numberFormatInfo.NumberGroupSeparator = ".";
    cultureInfo.NumberFormat = numberFormatInfo;

    Thread.CurrentThread.CurrentCulture = cultureInfo;
    Thread.CurrentThread.CurrentUICulture = cultureInfo;

    CultureInfo.CurrentCulture = cultureInfo;
    CultureInfo.CurrentUICulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    var configuration = builder.Configuration;
    var logDbConnection = configuration.GetConnectionString("LogDbConnection") ?? throw new KeyNotFoundException("LogDbConnection");

    // Logging
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Logging.AddSerilog();

    Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Source", "Aldebaran.Web")
    .WriteTo.MSSqlServer(logDbConnection, sinkOptions: new MSSqlServerSinkOptions
    {
        TableName = "logs",
        SchemaName = "log"
    }, restrictedToMinimumLevel: LogEventLevel.Information, columnOptions: new ColumnOptions
    {
        AdditionalColumns = new SqlColumn[]
        {
            new() { DataType= SqlDbType.NVarChar, ColumnName="Source", DataLength=100 }
        }
    }).CreateLogger();
}
static void Configure(WebApplication app)
{
    ArgumentNullException.ThrowIfNull(app);
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseHeaderPropagation();
    app.UseRequestLocalization();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    var imageRepositoryPath = app.Configuration.GetValue<string>("AppSettings:ImageRepositoryPath");
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imageRepositoryPath)),
        RequestPath = "/externalimages"
    });
}