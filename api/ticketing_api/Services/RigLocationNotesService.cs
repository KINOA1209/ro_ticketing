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
    public class RigLocationNotesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSieveProcessor _sieveProcessor;

        public RigLocationNotesService(ApplicationDbContext context, ISieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = (ApplicationSieveProcessor)sieveProcessor;
        }

        public async Task<PagingResults<RigLocationNoteView>> GetRigLocationNotesViewAsync(SieveModel sieveModel)
        {
            var query = _context.RigLocation.Join(_context.RigLocationNote,
                      riglocation => riglocation.Id,
                      riglocationnote => riglocationnote.RigLocationId,
                     (riglocation, riglocationnote) => new RigLocationNoteView
                     {
                         Id = riglocationnote.Id,
                         RigLocationId = riglocation,
                         RigNote = riglocationnote.RigNote,
                         IsEnabled = riglocationnote.IsEnabled
                     }).AsQueryable();

            var data = await _sieveProcessor.GetPagingDataAsync(sieveModel, query);

            return data;
        }

        public RigLocationNoteView PostRigLocationNote(RigLocationNote rigLocationNote)
        {
            RigLocationNoteView rigLocationNoteView = _context.RigLocation.Join(_context.RigLocationNote,
                                                       riglocation => riglocation.Id,
                                                       riglocationnote => riglocationnote.RigLocationId,
                                                       (riglocation, riglocationnote) => new RigLocationNoteView
                                                       {
                                                            Id = riglocationnote.Id,
                                                            RigLocationId = riglocation,
                                                            RigNote = riglocationnote.RigNote,
                                                            IsEnabled = riglocationnote.IsEnabled
                                                        }).FirstOrDefault(o => o.Id == rigLocationNote.Id);

            return rigLocationNoteView;
        }

    }
}
