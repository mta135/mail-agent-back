using MailAgent.Application.EmailProcessingBackgroundWorker.SendWorker;
using MailAgent.Application.MessagingService;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailAgent.Application.EmailProcesBackgroundWorker.ReceiveWorker
{
    public class ReceiveEmailBackgroundWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;


        private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(30);

        public ReceiveEmailBackgroundWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();

            IEmailMessageRepository repository = scope.ServiceProvider.GetRequiredService<IEmailMessageRepository>();
            IEmailSender emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();


            await emailSender.ReceiveEmailAsync(stoppingToken);

            throw new NotImplementedException();
        }
    }
}
