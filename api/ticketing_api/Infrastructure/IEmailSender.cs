using System.Threading.Tasks;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Infrastructure
{
    public interface IEmailSender
    {
        //Task SendForgotPasswordEmailAsync(AppUser user, string token);

        //Task SendNewUserEmailAsync(AppUser admin, AppUser user, string token);

        //Task SendAdminAlertAsync(AppUser admin, string message);

        //Task SendAdminAlertNewUserAsync(AppUser admin, AppUser user);

        //Task SendAdminAlertDeleteUserAsync(AppUser admin, AppUser user);

        //Task SendAdminAlertRegisterSuccessfulAsync(AppUser admin, AppUser user);

        //Task SendAdminAlertUpdateUserAsync(AppUser admin, AppUser user);

        //Task SendNotificationAsync(string subject, string message);

        void SendNotification(string recipients, string subject, string body);

        void SendOrderNotification(AppUser appUser, OrderView order);
    }
}
