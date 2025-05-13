using API.Model;
using FluentResults;

namespace API.Services;

public interface IEmailService
{
    Task<Result> SendEmailAsync(MailRequest mailRequest);
}