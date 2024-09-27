using Aldebaran.Application.FileWritingService.Settings;
using Aldebaran.Application.FileWritingService.Workers;
using Aldebaran.DataAccess;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using Aldebaran.Infraestructure.Common.Utils;
using Aldebaran.Infraestructure.Core.Model;
using Aldebaran.Infraestructure.Core.Ssh;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Ftp File Writing Service";
});
var services = builder.Services;
var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
services.Configure<FtpSettings>(configuration.GetSection("FtpSettings"));
var logDbConnection = configuration.GetConnectionString("LogDbConnection") ?? throw new KeyNotFoundException("LogDbConnection");
var dbConnection = configuration.GetConnectionString("AldebaranDbConnection") ?? throw new KeyNotFoundException("AldebaranDbConnection");
services.AddDbContext<AldebaranDbContext>(options => { options.UseSqlServer(dbConnection); }, ServiceLifetime.Transient, ServiceLifetime.Transient);
services.AddScoped<IContextConfiguration, ContextConfiguration>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddSerilog();
Log.Logger = new LoggerConfiguration()
.ReadFrom.Configuration(configuration)
.MinimumLevel.Information()
.Enrich.FromLogContext()
.Enrich.WithProperty("Source", "FileWritingService")
.WriteTo.MSSqlServer(logDbConnection, sinkOptions: new MSSqlServerSinkOptions
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

// HostedServices
services.AddHostedService<InventoryFtpPdfWorker>();
//services.AddHostedService<InventoryFtpExcelWorker>();
services.AddTransient<IInventoryReportRepository, InventoryReportRepository>();
services.AddTransient<IFileBytesGeneratorService, FileBytesGeneratorService>();
services.AddTransient<IFtpClient, FtpClient>();
var host = builder.Build();
await host.RunAsync();