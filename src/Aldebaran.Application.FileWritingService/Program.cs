using Aldebaran.Application.FileWritingService.Settings;
using Aldebaran.Application.FileWritingService.Workers;
using Aldebaran.DataAccess;
using Aldebaran.DataAccess.Infraestructure.Repository;
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
using System.IO;

try
{
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
    services.AddTransient<IFtpWritingConnectionRepository, FtpWritingConnectionRepository>();
    services.AddTransient<IAutomataNotificationRecipientRepository, AutomataNotificationRecipientRepository>();

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

    using (var scope = services.BuildServiceProvider().CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
        var ftpRepo = scope.ServiceProvider.GetRequiredService<IFtpWritingConnectionRepository>();
        var anyActive = ftpRepo.GetAllAsync().Result.Any(c => c.Active);
        if (!anyActive)
        {
            logger.LogError("No existe ninguna conexión FTP activa en FTP_Writing_Connections. El servicio no se iniciará.");
            return;
        }
    }

    // Resilience
    services.AddSingleton<Aldebaran.Application.FileWritingService.Resilience.ResilientExecutor>();
    // HostedServices
    services.AddHostedService<InventoryFtpPdfWorker>();
    services.AddHostedService<InventoryFtpExcelWorker>();
    services.AddTransient<IInventoryReportRepository, InventoryReportRepository>();
    services.AddTransient<IFileBytesGeneratorService, FileBytesGeneratorService>();
    services.AddTransient<IFtpClient, FtpClient>();
    services.AddSingleton<Aldebaran.Application.FileWritingService.Services.IEmailSender, Aldebaran.Application.FileWritingService.Services.EmailSender>();
    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    try
    {
        var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? ".", "filewritingservice-startup-error.txt");
        File.WriteAllText(file, ex.ToString());
    }
    catch { }

    try { Console.WriteLine(ex.ToString()); } catch { }
    throw;
}