using System.Net;
using MassTransit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Ntemtem.MainService.Consumers.Users;
using Ntemtem.MainService.Services;
using Ntemtem.MainService.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MainServiceDatabaseSettings>(builder.Configuration.GetSection("MainServiceDatabase"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqSettings = context.GetRequiredService<IOptionsMonitor<RabbitMQSettings>>();

        cfg.Host(rabbitMqSettings.CurrentValue.Host, rabbitMqSettings.CurrentValue.Port, "/", h =>
        {
            h.Username(rabbitMqSettings.CurrentValue.Username);
            h.Password(rabbitMqSettings.CurrentValue.Password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IUsersService, UsersService>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();