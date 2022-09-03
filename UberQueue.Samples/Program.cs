using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();
builder.Services.AddSingleton<IDatabase>(db);
//builder.Services.AddUberRedisQueue((config, x) =>
//{
//    config.BatchSize = 100;
//    config.Key = "jobs";

//    x.AddConsumer<TestConsumer>();
//    x.AddConsumer<TestConsumerWithDI>();

//    x.ConfigureRedis((options, endpoints) =>
//    {
//        endpoints.Add("localhost");
//    });
//});

var app = builder.Build();

app.Map("/", async () =>
{
    using (var scope = app.Services.CreateScope())
    {
        //var sampleService = scope.ServiceProvider.GetRequiredService<IRedisQueueService>();

        //var jobdata = new RecurringJobObject()
        //{
        //    Payload = new WeatherForecast()
        //    {
        //        Date = DateTime.Now,
        //    },
        //    PayloadType = typeof(WeatherForecast),
        //    Recurrence = "Test"
        //};
        //await sampleService.Enqueue("jobs", jobdata, DateTimeOffset.UtcNow.AddSeconds(2));
    }
});


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
