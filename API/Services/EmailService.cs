using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using API.AWS;
using API.Model;
using FluentResults;
using Microsoft.Extensions.Options;

namespace API.Services;

public class EmailService : IEmailService
{
    private const int MaxEmailQuotaAws = 200;
    private readonly IAmazonSimpleEmailService _awsClient;
    private readonly EmailOptions _emailOptions;

    public EmailService(
        IAmazonSimpleEmailService awsClient,
        IOptions<EmailOptions> emailOptions)
    {
        _awsClient = awsClient;
        _emailOptions = emailOptions.Value;
    }
    
    public async Task<Result> SendEmailAsync(MailRequest mailRequest)
    {
        try
        {
            var quota = await _awsClient.GetSendQuotaAsync();

            if(quota.SentLast24Hours > MaxEmailQuotaAws)
            {
                return Result.Fail(new Error("Limit email send exceed."));
            }

            _ = _awsClient.SendEmailAsync(
                mailRequest.SetEmailRequestCompany(_emailOptions));

            _ = _awsClient.SendEmailAsync(
                mailRequest.SetEmailRequestCostumer(_emailOptions));

            return Result.Ok();
            
        }catch(Exception ex)
        {
            return Result.Fail(new Error(ex.Message));
        }
    }
}

public static class MailParams
{
    public static SendEmailRequest SetEmailRequestCostumer(this MailRequest mailRequest, EmailOptions emailOptions)
    {
        var mailBody = new Body
            {
                Html = new Content
                {
                    Charset = "UTF-8",
                    //TODO: Inserir as informacoes que estao faltando do MailRequest
                    //TODO: fazer os templates de email
                    Data = @$"<p>{mailRequest.Message}</p>"
                }
            };

            var mailSubject = new Content($"Request: {mailRequest.ContactType}");
            var mailMessage = new Message(mailSubject, mailBody);
            var mailDestination = new Destination([emailOptions.To]);

            return new SendEmailRequest(
                emailOptions.Sender, 
                mailDestination, 
                mailMessage);
    }

    public static SendEmailRequest SetEmailRequestCompany(this MailRequest mailRequest, EmailOptions emailOptions)
    {
        var mailBody = new Body
            {
                Html = new Content
                {
                    Charset = "UTF-8",
                    //TODO: Inserir as informacoes que estao faltando do MailRequest
                    //TODO: fazer os templates de email
                    Data = @"<p>We are excide to work with you, we will contact you as son as possible.</p>"
                }
            };

        var mailSubject = new Content("Thanks for your contact!");
        var mailMessage = new Message(mailSubject, mailBody);
        var mailDestination = new Destination([mailRequest.Email]);

        return new SendEmailRequest(
            emailOptions.Sender, 
            mailDestination, 
            mailMessage);
    }
}