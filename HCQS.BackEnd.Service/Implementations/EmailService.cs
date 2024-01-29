using HCQS.BackEnd.Common.ConfigurationModel;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.Service.Contracts;
using MailKit.Security;
using MimeKit;

namespace HCQS.BackEnd.Service.Implementations
{
    public class EmailService : GenericBackendService, IEmailService
    {
        private BackEndLogger _logger;
        private EmailConfiguration _emailConfiguration;

        public EmailService(BackEndLogger logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
            _emailConfiguration = Resolve<EmailConfiguration>();
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("QK Back End Project", _emailConfiguration.User));
                message.To.Add(new MailboxAddress("Customer", recipient));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;
                message.Body = bodyBuilder.ToMessageBody();

                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate(_emailConfiguration.User, _emailConfiguration.ApplicationPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
        }
    }
}