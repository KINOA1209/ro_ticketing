using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Models;

namespace ticketing_api.Infrastructure
{
    public static class AppConstants
    {
        public static class AppRoles
        {
            public static readonly string SysAdmin = "SYSADMIN";
            public static readonly string Admin = "ADMIN";
            public static readonly string Driver = "DRIVER";
            public static readonly string Sales = "SALES";
        }

        public static class OrderStatuses {
            public static readonly byte Preticket = 0;
            public static readonly byte Open = 1;
            public static readonly byte Assigned = 2;
            //public static readonly byte Loaded = 3;
            public static readonly byte Delivered = 4;
            public static readonly byte Voided = 5;
        }

        public static class PaperFormats
        {
            public static readonly byte Mobile = 0;
            public static readonly byte Web = 1;
        }


        public static class Settings
        {
            public static readonly string AFEPO = "AFEPO";
        }

        public static class SendGridTemplates
        {
            //TODO setup send grid templates and id here
            public static readonly string NewUserWelcome = "";
            public static readonly string ForgotPassword = "";
            public static readonly string AdminAlert = "";
            public static readonly string AdminAlertNewUser = "";
            public static readonly string AdminAlertUpdateUser = "";
            public static readonly string AdminAlertDeleteUser = "";
            public static readonly string AdminAlertRegisterSuccessful = "";
        }

    }
}
