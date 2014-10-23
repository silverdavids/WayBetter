using Domain.Models.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebUI.DataAccessLayer;


namespace BetLive.Controllers.Api
{
    public class TerminalController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Terminal
        public IQueryable<Terminal> GetTerminal()
        {
            return db.Terminals;
        }

        // GET: api/Terminal/5
        [ResponseType(typeof(Terminal))]
        public IHttpActionResult GetTerminal(int id)
        {
            Terminal terminal = db.Terminals.Find(id);
            if (terminal == null)
            {
                return NotFound();
            }

            return Ok(terminal);
        }

        public IQueryable<Terminal> GetTerminalsByBranchId(int id)
        {
            IQueryable<Terminal> terminal = db.Terminals.Where(t => t.Branch.BranchId.Equals(id)).Include(t => t.Branch.Employees);
            if (terminal == null)
            {
                //return NotFound();
            }

            return terminal;
        }
        // PUT: api/Terminal/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTerminal(int id, Terminal terminal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != terminal.TerminalId)
            {
                return BadRequest();
            }

            db.Entry(terminal).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TerminalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Terminal
        [ResponseType(typeof(Terminal))]
        public IHttpActionResult PostTerminal(Terminal terminal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            terminal.isActive = false;
            db.Terminals.Add(terminal);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = terminal.TerminalId }, terminal);
        }

        // DELETE: api/Terminal/5
        [ResponseType(typeof(Terminal))]
        public IHttpActionResult DeleteTerminal(int id)
        {
            Terminal terminal = db.Terminals.Find(id);
            if (terminal == null)
            {
                return NotFound();
            }

            db.Terminals.Remove(terminal);
            db.SaveChanges();

            return Ok(terminal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TerminalExists(int id)
        {
            return db.Terminals.Count(e => e.TerminalId == id) > 0;
        }
    }
}