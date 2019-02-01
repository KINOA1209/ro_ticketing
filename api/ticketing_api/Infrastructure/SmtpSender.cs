using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Models;
using ticketing_api.Models.Views;
using ticketing_api.Services;

namespace ticketing_api.Infrastructure
{
    public class SmtpSender : IEmailSender
    {
        private ApplicationDbContext _context;

        public SmtpSender(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SendNotification(string recipients, string subject, string body)
        {
            try
            {
                //todo: move the server address, username and password to the config file
                SmtpClient client = new SmtpClient("smtp.office365.com");
                client.UseDefaultCredentials = false;
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("order_notifications@rolfsonoil.com", "L8uAbEFrbenRac83");

                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress("order_notifications@rolfsonoil.com");

                foreach (var address in recipients.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mailMessage.To.Add(address);
                }
                
                mailMessage.Body = body;
                mailMessage.Subject = subject;
                client.Send(mailMessage);
            }
            catch(Exception Ex)
            {
            }
        }

        public void SendOrderNotification(AppUser appUser, OrderView order)
        {
            var notifications = _context.NotificationTemplate.Where(x => x.NotificationTypeId == 1 && x.TriggerId == 1);
            foreach (var n in notifications)
            {
                var body = n.Content;
                var subject = n.Subject;
                var recipients = n.Recipients;

                var tokenService = TokenService.Instance;
                recipients = tokenService.ReplaceUserTokens(appUser, recipients);
                recipients = tokenService.ReplaceOrderTokens(order, recipients);
                body = tokenService.ReplaceUserTokens(appUser, body);
                body = tokenService.ReplaceOrderTokens(order, body);
                subject = tokenService.ReplaceUserTokens(appUser, subject);
                subject = tokenService.ReplaceOrderTokens(order, subject);

                SendNotification(recipients,subject,body);
            }
        }
    }
}
