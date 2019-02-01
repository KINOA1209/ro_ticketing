using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Models;

namespace ticketing_api.Services
{
    public class SettingService
    {
        private readonly ApplicationDbContext _context;

        public SettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetNewAfePo()
        {
            string afe_po;
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
                var setting = _context.Setting.AsQueryable().FirstOrDefault(x => x.Key == AppConstants.Settings.AFEPO);
                var value = Convert.ToInt32(setting != null ? setting.Value : "1000");
                afe_po = value.ToString();

                if (setting != null)
                {
                    setting.Value = (value+1).ToString();
                    _context.Setting.Update(setting);
                }
                else
                {
                    setting = new Setting();
                    setting.Key = AppConstants.Settings.AFEPO;
                    setting.Value = (value + 1).ToString();
                    _context.Setting.Add(setting);
                }

                _context.SaveChanges();
//                scope.Complete();
//            }

            return afe_po;
        }
    }
}
