namespace taskflow.Services.Interfaces;

public interface IEmailService
{
    string SendEmailAsync(string to, string subject, string body);
}