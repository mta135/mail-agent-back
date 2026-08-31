using MailAgent.Application.MessagingService;
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
using System.Net.NetworkInformation;
using System.Text;

namespace MailAgent.Application.EmailProcessingBackgroundWorker.SendWorker
{
    public class EmailBackgroundWorker : BackgroundService
    {
        private readonly IEmailChannel _emailChannel;

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
            EmailMessage? dbRequestModel = null;
            EmaiMessagelRequestModel? requestModel = null;

            using var scope = _scopeFactory.CreateScope();

            IEmailMessageRepository repository = scope.ServiceProvider.GetRequiredService<IEmailMessageRepository>();
            IEmailSender emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            try
            {
                (requestModel, dbRequestModel) = await GetDbEmailMessage(messageId, repository);

                await emailSender.SendEmailAsync(requestModel, stoppingToken);

                await repository.SetEmailMessageSendStatusAsync(dbRequestModel, (int)EmailSendStatusEnum.Sent);
            }

            catch (Exception ex)
            {
                string _log = ex.ToString();
                await repository.SetEmailMessageSendStatusAsync(dbRequestModel!, (int)EmailSendStatusEnum.Failed);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Tuple<EmaiMessagelRequestModel, EmailMessage>> GetDbEmailMessage(Guid emailId, IEmailMessageRepository repository)
        {
            EmailMessage? dbModel = await repository.GetEmailMessageByIdAsync(emailId) ?? throw new InvalidOperationException("Email message not found");

            EmaiMessagelRequestModel request = new EmaiMessagelRequestModel
            {
                From = dbModel.From,
                Header = dbModel.Header,

                Subject = dbModel.Subject,
                Body = dbModel.Body,

                Footer = dbModel.Footer,

                To = [.. dbModel.EmailMessageTos.Select(x => new EmailMessageRequestToModel { EmailTo = x.To })],
                Copy = [.. dbModel.Copies.Select(x => new EmailMessageRequestCopyModel { EmailCopy = x.Copy ?? string.Empty })],

                Attachments = dbModel.Attachments.Select(a => new EmailMessageRequestAttachmentModel
                {
                    FileName = a.FileName,
                    ContentType = a.ContentType,

                    Data = a.Data 

                }).ToList()
            };

            return new Tuple<EmaiMessagelRequestModel, EmailMessage>(request, dbModel);
        }
    }
}
