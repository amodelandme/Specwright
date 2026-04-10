using Specwright.Infrastructure.DependencyInjection;
using Specwright.ServiceDefaults;
using Specwright.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddSpecwrightServiceDefaults();
builder.AddSpecwrightInfrastructure();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
