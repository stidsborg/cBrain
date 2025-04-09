using Cleipnir.Flows;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace cBrain.Flows.NewsLetter;

[GenerateFlows]
public class NewsletterFlow : Flow<MailAndRecipients>
{
    /*
     * 1. How do we keep track of how far we have gotten - avoiding resending everything if crashing?
     * 2. Can we make it more space-efficient?
     * 3. Can we parallelize execution while still avoiding resending everything if crashing?
     */
    
    public override async Task Run(MailAndRecipients mailAndRecipients)
    {
        var (recipients, subject, content) = mailAndRecipients;
        using var client = new SmtpClient();
        await client.ConnectAsync("mail.smtpbucket.com", 8025);

        for (var i = 0; i < recipients.Count; i++)
        {
            var recipient = recipients[i];
            var email = CreateEmail(recipient, subject, content);
            await client.SendAsync(email);
        }
    }

    private MimeMessage CreateEmail(EmailAddress recipient, string subject, string content)
    {
        var message = new MimeMessage();
        message.To.Add(new MailboxAddress(recipient.Name, recipient.Address));
        message.From.Add(new MailboxAddress("Cleipnir.NET", "newsletter@cleipnir.net"));

        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = content };
        return message;
    }
}

public record EmailAddress(string Name, string Address);
public record MailAndRecipients(
    IReadOnlyList<EmailAddress> Recipients,
    string Subject,
    string Content
);