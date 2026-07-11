using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace AccessHub.API.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new AmazonSimpleEmailServiceClient(
            _configuration["AWS:AccessKey"],
            _configuration["AWS:SecretKey"],
            RegionEndpoint.APSoutheast1
        );

        var request = new SendEmailRequest
        {
            Source = _configuration["AWS:SenderEmail"],
            Destination = new Destination { ToAddresses = new List<string> { toEmail } },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body { Text = new Content(body) },
            },
        };

        await client.SendEmailAsync(request);
    }
}
