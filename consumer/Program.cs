using consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

using var server = new Prometheus.KestrelMetricServer(port: 1234);
server.Start();

var host = builder.Build();
host.Run();
