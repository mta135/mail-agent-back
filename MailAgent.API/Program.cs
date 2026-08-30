using MailAgent.Application.SendEmailWorker;
using MailAgent.Application.Service;
using MailAgent.Application.Service.Abstract;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using MailAgent.DataBaseAccess.Repositories.Real;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IEmailChannel, EmailChannel>();
builder.Services.AddHostedService<EmailBackgroundWorker>();
builder.Services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
builder.Services.AddScoped<IEmailMessageService, EmailMessageService>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
