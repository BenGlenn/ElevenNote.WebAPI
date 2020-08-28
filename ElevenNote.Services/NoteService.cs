using ElevenNote.Data;
using ElevenNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services
{
    public class NoteService
    {
        private readonly Guid _userId;

        public NoteService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateNote(NoteCreate model)
        {
            var entity =
                new Note()
                {
                    OwnerId = _userId,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedUtc = DateTimeOffset.Now
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Notes.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<NoteListItem> GetNotes() // PLEASE BREAK THIS METHOD DOWN //
        {   //creates a temporary instance of ApplicationDbContext
            using (var ctx = new ApplicationDbContext())
            {// instantiates an IQueryable called query
                var query = 
                    ctx   // <--- our ApplicationDbContext object from line 40
                        .Notes      // <--- Notes DbSet<Note>
                        .Where(e => e.OwnerId == _userId)  //<--- filters through Notes for entities with an OwnerId that matches the current User's Id
                        .Select(    //<--- Iterates through the entities that passed through the filter, and performs whatever code you put inside its body
                            e =>  //<--- uses a lamba to take whatever entity the .Select method is currently running code for (e is our Note entity)
                                new NoteListItem    //<--- creates a new NoteListItem to essentially "copy" the properties from e (our Note) to the NoteListItem
                                {
                                    NoteId = e.NoteId,
                                    Title = e.Title,
                                    CreatedUtc = e.CreatedUtc
                                }                           //<--- This NoteListItem is added to our IQueryable object query
                        );

                return query.ToArray();     // <--- converts our IQueryable object to an Array
            }
        }
    }
}
