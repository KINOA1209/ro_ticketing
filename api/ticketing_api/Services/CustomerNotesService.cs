using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticketing_api.Data;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Services
{
    public class CustomerNotesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public CustomerNotesService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<CustomerNoteView>> GetCustomerNotesViewAsync(SieveModel sieveModel)
        {
            var query = _context.Customer.Join(_context.CustomerNote,
                     customer => customer.Id,
                     customernote => customernote.CustomerId,
                    (customer, customernote) => new CustomerNoteView
                    {
                        Id = customernote.Id,
                        CustomerId = customer,
                        CustomerNotes = customernote.CustomerNotes,
                        IsEnabled = customernote.IsEnabled
                    }).AsQueryable();

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public CustomerNoteView PostCustomerNote(CustomerNote customerNote)
        {
            CustomerNoteView customerNoteView = _context.Customer.Join(_context.CustomerNote,
                                                customer => customer.Id,
                                                customernote => customernote.CustomerId,
                                                (customer, customernote) => new CustomerNoteView
                                                {
                                                    Id = customernote.Id,
                                                    CustomerId = customer,
                                                    CustomerNotes = customernote.CustomerNotes,
                                                    IsEnabled = customernote.IsEnabled
                                                }).FirstOrDefault(o => o.Id == customerNote.Id);

            return customerNoteView;
        }
    }
}
