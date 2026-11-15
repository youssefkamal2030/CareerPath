using System.Threading.Tasks;

namespace EmailConfigration.EmailConfig
{
    public interface IEmailService
    {
        Task SendEmailAsync(Message message);
    }
} 