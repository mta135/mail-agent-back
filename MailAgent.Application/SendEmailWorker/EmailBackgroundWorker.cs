using MailAgent.DataBaseAccess.Repositories.Abstract;
using MailAgent.DataBaseAccess.Repositories.Real;
using MailAgent.Model.EmailMessage;
using MailAgent.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private IEmailMessageRepository _emailMessageRepository;

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly SemaphoreSlim _semaphore;
        private readonly List<Task> _runningTasks = new();

        public EmailBackgroundWorker(IEmailChannel emailChannel, IServiceScopeFactory scopeFactory)
        {
            _emailChannel = emailChannel;

            _scopeFactory = scopeFactory;
            _semaphore = new SemaphoreSlim(initialCount: 5, maxCount: 5);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (var emailId in /*_emailChannel.Reader.ReadAllAsync(stoppingToken)*/  _emailChannel.ReadAllAsync(stoppingToken))
                {
                    try
                    {

                        await _semaphore.WaitAsync(stoppingToken);

                        var task = ProcessMessageAsync(emailId, stoppingToken);
                        _runningTasks.Add(task);

                        _runningTasks.RemoveAll(t => t.IsCompleted);


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


        private async Task ProcessMessageAsync(Guid messageId, CancellationToken stoppingToken)
        {
            try
            {
                //await emailSender.SendAsync(message, stoppingToken);


                IEmailMessageRepository repository = GetEmailMessageRepository();
                await repository.SetEmailStatusAsync(messageId, (int)EmailSendStatusEnum.Sent);
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Eroare la trimiterea emailului {EmailId}", message.Id);

                try
                {
                    //using var scope = _scopeFactory.CreateScope();
                    //var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    //await UpdateStatusAsync(dbContext, message.Id, EmailStatus.Failed, ex.Message, stoppingToken);
                }
                catch (Exception updateEx)
                {
                    //_logger.LogError(updateEx, "Eroare la actualizarea statusului pentru emailul {EmailId}", message.Id);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private IEmailMessageRepository GetEmailMessageRepository()
        {
            using var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IEmailMessageRepository>();
        }
    }
}
