using Aldebaran.Web.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
var app = builder.Build();
Configure(app);
app.Run();

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.AddArchitecture();
    builder.AddInfraestructure();

    var configuration = builder.Configuration;
    var dbConnection = configuration.GetConnectionString("AldebaranDbConnection") ?? throw new KeyNotFoundException("AldebaranDbConnection");
    // Logging
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Logging.AddSerilog();

    Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Source", "Aldebaran.Web")
    .WriteTo.MSSqlServer(dbConnection, sinkOptions: new MSSqlServerSinkOptions
    {
        TableName = "logs",
        SchemaName = "log"
    }, restrictedToMinimumLevel: LogEventLevel.Information, columnOptions: new ColumnOptions
    {
        AdditionalColumns = new SqlColumn[]
        {
            new SqlColumn{ DataType= SqlDbType.NVarChar, ColumnName="Source", DataLength=100 }
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
    //app.UseRequestLocalization(options => options.AddSupportedCultures("es-CO").AddSupportedUICultures("es-CO").SetDefaultCulture("es-CO"));
    app.UseRequestLocalization();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    //app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();
}