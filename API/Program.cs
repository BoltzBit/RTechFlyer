using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using API.AWS;
using API.Model;
using API.Services;
using Microsoft.Extensions.Options;
using DotNetEnv;

Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuracoes
builder.Services.Configure<AwsOptions>(builder.Configuration.GetSection("AWS"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

builder.Services.AddScoped<IAmazonSimpleEmailService>(sp =>
{
    var options = sp.GetRequiredService<IOptions<AwsOptions>>().Value;
    var credentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
    var region = RegionEndpoint.GetBySystemName(options.Region);
    return new AmazonSimpleEmailServiceClient(credentials, region);
});

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/send-mail", async (MailRequest mailRequest, IEmailService emailService) =>
    {
        var result = await emailService.SendEmailAsync(mailRequest);
        
        return result.IsFailed ? 
            Results.BadRequest(result.Errors) :
            Results.NoContent();
    })
    .WithName("SendEmail")
    .WithOpenApi();

app.Run();