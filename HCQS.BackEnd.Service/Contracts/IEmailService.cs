namespace HCQS.BackEnd.Service.Contracts
{
    public interface IEmailService
    {
        public void SendEmail(string recipient, string subject, string body);
    }
}