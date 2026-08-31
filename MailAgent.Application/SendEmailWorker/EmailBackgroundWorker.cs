using MailAgent.DataBaseAccess.DataScheme;
using MailAgent.DataBaseAccess.Repositories.Abstract;
using MailAgent.DataBaseAccess.Repositories.Real;
using MailAgent.Model.EmailMessage;
using MailAgent.Model.Enums;
using Microsoft.AspNetCore.Http.Internal;
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
        private readonly IEmailMessageRepository _emailMessageRepository;

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly SemaphoreSlim _semaphore;
        private readonly List<Task> _runningTasks = new();

        public EmailBackgroundWorker(IEmailChannel emailChannel, IServiceScopeFactory scopeFactory, IEmailMessageRepository emailMessageRepository)
        {
            _emailChannel = emailChannel;
            _emailMessageRepository = emailMessageRepository;

            _scopeFactory = scopeFactory;
            _semaphore = new SemaphoreSlim(initialCount: 5, maxCount: 5);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (var emailId in _emailChannel.ReadAllAsync(stoppingToken))
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
                (EmailMessageModel model, EmailMessage dbModel) = await GetEmailMessage(messageId);

                //await emailSender.SendAsync(message, stoppingToken);

                await SetEmailSendStatus(messageId, EmailSendStatusEnum.Sent);
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Eroare la trimiterea emailului {EmailId}", message.Id);

                try
                {
                    await SetEmailSendStatus(messageId, EmailSendStatusEnum.Failed);
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

        private async Task SetEmailSendStatus(Guid emailId, EmailSendStatusEnum status)
        {
            IEmailMessageRepository repository = GetEmailMessageRepository();
            await repository.SetEmailStatusAsync(emailId, (int)status);
        }

        private IEmailMessageRepository GetEmailMessageRepository()
        {
            using var scope = _scopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IEmailMessageRepository>();
        }


        private async Task<Tuple<EmailMessageModel, EmailMessage>> GetEmailMessage(Guid emailId)
        {
            IEmailMessageRepository repository = GetEmailMessageRepository();

            EmailMessage? dbModel = await repository.GetEmailMessageByIdAsync(emailId) ?? throw new InvalidOperationException("Email message not found");


            EmailMessageModel model = new EmailMessageModel();


            model.From = dbModel.From;
            model.Header = dbModel.Header;

            model.Subject = dbModel.Subject;
            model.Body = dbModel.Body;

            model.Footer = dbModel.Footer;

            model.To = dbModel.EmailMessageTos.Select(x => x.To).ToList();

            model.Copy = dbModel.Copies.Select(x => x.Copy ?? string.Empty).ToList();

            //model.Attachments = dbModel.Attachments.Select(a => new Attachment
            //{
            //    FileName = a.FileName,
            //    ContentType = a.ContentType,
            //    Content = a.Content // byte[]
            //}).ToList();


            return new Tuple<EmailMessageModel, EmailMessage>(model, dbModel);
        }

    }
}
