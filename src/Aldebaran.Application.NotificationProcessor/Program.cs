using Aldebaran.Application.NotificationProcessor.Settings;
using Aldebaran.Application.NotificationProcessor.Workers;
using Aldebaran.Application.Services.Notificator.EmailProvider;
using Aldebaran.Application.Services.Notificator.Notify;
using Aldebaran.Application.Services.Notificator.Services;
using Aldebaran.Infraestructure.Core.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;

// Variables
var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
var amqpConnection = configuration.GetConnectionString("RabbitMqConnection") ?? throw new KeyNotFoundException("RabbitMqConnection");
var dbConnection = configuration.GetConnectionString("LogDbConnection") ?? throw new KeyNotFoundException("AldebaranDbConnection");

// Logging
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddSerilog();

Log.Logger = new LoggerConfiguration()
.ReadFrom.Configuration(configuration)
.MinimumLevel.Information()
.Enrich.FromLogContext()
.Enrich.WithProperty("Source", "NotificationProcessor")
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

// Connections
// RabbitMq
services.AddSingleton(sp =>
{
    return Policy.Handle<Exception>()
        .WaitAndRetry(10, _ => TimeSpan.FromSeconds(5), (ex, ts) =>
        {
            Console.WriteLine($"Resilience Error [{ex.Message}] since [{ts.TotalSeconds}] sec using connection [{amqpConnection}]");
        })
        .Execute(() =>
        {
            var factory = new RabbitMQ.Client.ConnectionFactory()
            {
                Uri = new Uri(amqpConnection),
                DispatchConsumersAsync = true
            };
            var conn = factory.CreateConnection();
            return conn;
        });
});

// Services
services.AddTransient<IQueue, RabbitQueue>();
services.AddTransient<IQueueSettings, QueueSettings>();
services.AddTransient<IEmailService, EmailService>();
services.AddTransient<INotificationProvider, EmailNotificationProvider>();

// HostedServices
services.AddHostedService<NotificationWorker>();

var host = builder.Build();
await host.RunAsync();