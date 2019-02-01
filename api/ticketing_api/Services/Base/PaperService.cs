using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Models;
using ticketing_api.Models.Views;
using OrderView = ticketing_api.Models.Views.OrderView;

namespace ticketing_api.Services.Base
{
    public class PaperService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISieveProcessor _sieveProcessor;

        public PaperService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }
    }
}

