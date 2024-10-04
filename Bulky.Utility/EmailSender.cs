using Microsoft.AspNetCore.Identity.UI.Services;

namespace Bulky.Utility;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // TODO: logic to send email implement later.
        return Task.CompletedTask;
    }
}