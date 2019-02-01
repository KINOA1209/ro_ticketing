//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Threading.Tasks;
//using System.Web;
//using ticketing_api.Models;
//using SendGrid;
//using SendGrid.Helpers.Mail;

//namespace ticketing_api.Infrastructure
//{
//    public class SendGridSender : IEmailSender
//    {
//        private readonly SendGridClient _client;
//        private readonly EmailAddress _from;
//        private readonly EmailAddress _notifications;

//        public SendGridSender()
//        {
//            var config = AppConfiguration.Instance.SendGrid;
//            var key = config["ApiKey"];
//            _client = new SendGridClient(key);
//            _from = new EmailAddress(config["FromAddress"], config["FromName"]);
//            _notifications = new EmailAddress(config["Notifications"], config["FromName"]);
//        }

//        public async Task SendTemplate(string templateId, AppUser toUser, Dictionary<string, string> substitutions)
//        {
//            try
//            {
//                if (substitutions == null)
//                    substitutions = new Dictionary<string, string>();
//                //substitutions.Add("[user-name]", toUser.FullName);

//                // var to = new EmailAddress(toUser.Email, toUser.FullName);
//                //var to = new EmailAddress("mnewlin@gmail.com", toUser.FullName);

//                // var msg = new SendGridMessage();
//                // msg.SetFrom(_from);
//                //msg.AddTo(toUser.Email);
//                // msg.SetTemplateId(templateId);
//                // msg.AddSubstitutions(substitutions);

//                var link = substitutions["--link--"];
//                var username = substitutions["[user-name]"];
//                var adminname = substitutions["[admin-name]"];

//                var msg = new SendGridMessage()
//                {
//                    From = _from,
//                    Subject = "Reset password",
//                    HtmlContent = "<b>Click here to reset your password : </b><a href=" + link + "> Reset Password </a>"
//                };
//                msg.AddTo(new EmailAddress(toUser.Email));
//                var response = await _client.SendEmailAsync(msg);

//                if (response.StatusCode != HttpStatusCode.Accepted)
//                {
//                    throw new NotImplementedException();
//                }
//            }
//            catch (Exception)
//            {
//                //ex.ToString();
//            }
//        }

//        public async Task SendForgotPasswordEmailAsync(AppUser appUser, string token)
//        {
//            token = HttpUtility.UrlEncode(token);

//            string templateId = AppConstants.SendGridTemplates.ForgotPassword;
//            var subs = new Dictionary<string, string>
//            {
//                {"[user-name]", appUser.FullName},
//                {"--link--", AppConfiguration.Instance.ResetPasswordLink + token}
//            };
//            await SendTemplate(templateId, appUser, subs);
//        }

//        public async Task SendNewUserEmailAsync(AppUser adminUser, AppUser appUser, string token)
//        {
//            token = HttpUtility.UrlEncode(token);

//            string templateId = AppConstants.SendGridTemplates.NewUserWelcome;
//            var subs = new Dictionary<string, string>
//            {
//                {"[user-name]", appUser.FullName},
//                {"[admin-name]", adminUser.FullName ?? AppConfiguration.Instance.AppName},
//                {"--link--", AppConfiguration.Instance.ResetPasswordLink + token}
//            };

//            await SendTemplate(templateId, appUser, subs);
//        }

//        public async Task SendNotificationAsync(string subject, string message)
//        {
//            try
//            {
//                var msg = new SendGridMessage()
//                {
//                    From = _from,
//                    Subject = subject,
//                    HtmlContent = message
//                };
//                msg.AddTo(_notifications);
//                var response = await _client.SendEmailAsync(msg);

//                if (response.StatusCode != HttpStatusCode.Accepted)
//                {
//                    throw new NotImplementedException();
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:
//            }
//        }

//        public async Task SendNotificationAsync(string recipients, string subject, string message)
//        {
//            try
//            {
//                var msg = new SendGridMessage()
//                {
//                    From = _from,
//                    Subject = subject,
//                    HtmlContent = message
//                };
//                msg.AddTo(recipients);
//                var response = await _client.SendEmailAsync(msg);

//                if (response.StatusCode != HttpStatusCode.Accepted)
//                {
//                    throw new NotImplementedException();
//                }
//            }
//            catch (Exception ex)
//            {
//                //TODO:
//            }
//        }


//        public Task SendAdminAlertAsync(AppUser admin, string message)
//        {
//            throw new NotImplementedException();
//        }

//        private async Task SendAdminAlertAsync(string templateId, AppUser admin, AppUser appUser)
//        {
//            var subs = new Dictionary<string, string>
//            {
//                { "[user-name]", appUser.FullName},
//            };
//            await SendTemplate(AppConstants.SendGridTemplates.AdminAlert, admin, subs);
//        }

//        //Admin - New User Alert
//        public async Task SendAdminAlertNewUserAsync(AppUser admin, AppUser appUser)
//        {
//            await SendAdminAlertAsync(AppConstants.SendGridTemplates.AdminAlertNewUser, admin, appUser);
//        }

//        //Admin - User Added Email
//        public async Task SendAdminAlertDeleteUserAsync(AppUser admin, AppUser appUser)
//        {
//            await SendAdminAlertAsync(AppConstants.SendGridTemplates.AdminAlertDeleteUser, admin, appUser);
//        }

//        //Admin - User Updated Email
//        public async Task SendAdminAlertUpdateUserAsync(AppUser admin, AppUser appUser)
//        {
//            await SendAdminAlertAsync(AppConstants.SendGridTemplates.AdminAlertUpdateUser, admin, appUser);
//        }

//        //Development - User Registered Succesfully Email
//        public async Task SendAdminAlertRegisterSuccessfulAsync(AppUser admin, AppUser appUser)
//        {
//            await SendAdminAlertAsync(AppConstants.SendGridTemplates.AdminAlertRegisterSuccessful, admin, appUser);
//        }

//    }
//}
