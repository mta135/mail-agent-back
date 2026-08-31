using MailAgent.Application.EmailProcessingBackgroundWorker.SendWorker;
using MailAgent.Application.MessagingService;
using MailAgent.Application.Service;
using MailAgent.Application.Service.Abstract;
using MailAgent.DataBaseAccess.Contex;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using MailAgent.DataBaseAccess.Repositories.Real;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IEmailChannel, EmailChannel>();
builder.Services.AddHostedService<EmailBackgroundWorker>();
builder.Services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();

builder.Services.AddScoped<IEmailMessageService, EmailMessageService>();
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();


builder.Services.AddDbContext<MailAgentDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MailAgent")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Dita EstFarm",
        Description = "FFAppMiddleware Project. ASP.NET Web API",
        Contact = new OpenApiContact
        {
            Name = ".Net Developer: Mihai Tamazlîcaru",
            Email = string.Empty,
            Url = new Uri("https://twitter.com/spboyer"),
        },
        License = new OpenApiLicense
        {
            Name = "Dita EstFarm Licence",
            Url = new Uri("https://example.com/license"),
        },
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    //app.MapScalarApiReference();

    app.UseSwagger(); // Generates the JSON endpoints
    app.UseSwaggerUI(); // Serves the interactive HTML UI
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
