using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces.IEmails
{
    public interface IEmailService
    {
        Task SendAsync(
      EmailMessage message,
      CancellationToken cancellationToken = default);
    }
} 