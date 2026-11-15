using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;

namespace EmailConfigration.EmailConfig
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string subject { get; set; }
        public string Content { get; set; }

        public Message(List<MailboxAddress> to, string subject, string content)
        {
            To = new List<MailboxAddress>(to);
            this.subject = subject;
            Content = content;
        }
    }
}
