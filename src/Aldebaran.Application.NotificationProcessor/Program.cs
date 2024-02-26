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

// Variables
var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
var amqpConnection = configuration.GetConnectionString("RabbitMqConnection") ?? throw new KeyNotFoundException("RabbitMqConnection");

// Logging
services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
});

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