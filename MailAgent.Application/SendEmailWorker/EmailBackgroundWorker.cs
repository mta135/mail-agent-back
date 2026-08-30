using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MailAgent.Application.SendEmailWorker
{
    public class EmailBackgroundWorker : BackgroundService
    {
        private readonly IEmailChannel _emailChannel;

        public EmailBackgroundWorker(IEmailChannel emailChannel)
        {
            _emailChannel = emailChannel;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (var email in /*_emailChannel.Reader.ReadAllAsync(stoppingToken)*/  _emailChannel.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                       // _logger.LogError(ex, "Failed to send email to {To}.", email.To);
                    }
                }
            }
            catch (OperationCanceledException)
            {
               // _logger.LogInformation("Email Consumer Worker is shutting down.");
            }
        }
    }
}
