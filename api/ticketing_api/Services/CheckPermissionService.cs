using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ticketing_api.Data;
using Microsoft.EntityFrameworkCore;
using ticketing_api.Controllers.BaseController;
using Sieve.Services;
using System.Linq;
using Microsoft.Extensions.Logging;
using ticketing_api.Infrastructure;

namespace ticketing_api.Services
{
    public class CheckPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public CheckPermissionService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<bool> CheckPermissions(string module, string permission, string roleName)
        {

            var roleId = await _context.Roles.FirstAsync(x => x.Name.Equals(roleName,StringComparison.OrdinalIgnoreCase));
            var permissions = await _context.Permission
                .Join(_context.Module, a => a.ModuleId, b => b.Id, (a, b) => new
                {
                    a.Id,
                    a.RoleId,
                    a.ModuleId,
                    b.ModuleName,
                    a.IsRead,
                    a.IsCreate,
                    a.IsUpdate,
                    a.IsDelete
                }).Where(x => x.RoleId == roleId.Id).ToListAsync();

            foreach (var p in permissions)
            {
                if (p.ModuleName.ToUpper() == module.ToUpper())
                {
                    switch (permission)
                    {
                        case "Create":
                            return p.IsCreate;
                        case "Update":
                            return p.IsUpdate;
                        case "Read":
                            return p.IsRead;
                        case "Delete":
                            return p.IsDelete;
                    }
                }
            }

            return false;
        }
    }
}
