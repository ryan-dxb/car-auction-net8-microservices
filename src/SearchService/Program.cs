using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetRetryPolicy());

// MassTransit
builder.Services.AddMassTransit(config =>
{
    config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("SearchService", false));

    config.UsingRabbitMq((ctx, cfg) =>
    {
        /* cfg.Host("rabbitmq://localhost"); */

        cfg.ReceiveEndpoint("search-service-auction-created", e =>
        {
            // Will retry 3 times with 5 seconds interval between each retry if an exception is thrown
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            e.ConfigureConsumer<AuctionCreatedConsumer>(ctx);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDB(app);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
});


app.Run();


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));