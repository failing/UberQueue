using StackExchange.Redis;
using UberQueue.AspNetCore.Configuration;
using UberQueue.Core.Jobs;
using UberQueue.Core.Queue;
using UberQueue.Samples;
using UberRedQueueApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;

builder.Services.AddUberRedisQueue((config, x) =>
{
    config.BatchSize = 100;
    config.Key = "jobs";

    x.AddConsumer<WeatherConsumer>(op =>
    {
        op.UseStreams = true;
    });

    x.AddConsumer<TestConsumer>();

    x.ConfigureRedis((options, endpoints) =>
    {
        endpoints.Add("localhost");
    });
});

var app = builder.Build();

app.Map("/", async () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var sampleService = scope.ServiceProvider.GetRequiredService<IRedisQueueManager>();

        var jobdata = new RecurringJobObject()
        {
            Payload = new WeatherForecast()
            {
                Date = DateTime.Now,
            },
            Recurrence = "Test"
        };

        for (int i = 0; i < 100; i++)
        {
            await sampleService.Enqueue(jobdata, DateTimeOffset.UtcNow.AddSeconds(15));
        }
    }
});


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
