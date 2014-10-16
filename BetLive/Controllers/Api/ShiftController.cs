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

using Domain.Models.Concrete;

using System.Data.Entity.Core.Objects;




namespace BetLive.Controllers.Api
{
    public class ShiftController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Shift
        public IEnumerable<Shift> GetShift()
        { 
            var shifts= db.Shifts.Include(s=>s.Terminal);
            return shifts.AsEnumerable();
        }
        // GET: api/Shift
        public List<Shift> GetShiftTerminalWithCashier(int id,DateTime dateFrom,DateTime dateTo)
        {

            if (dateFrom != null)
            {
                var _dateFrom = dateFrom.Date;
                var _dateTo = dateTo.Date;
                
            
            }

            var shiftToSend = new List<Shift>();
            var terminals = db.Terminals.Include(t => t.Shifts).Where(t => t.BranchId == id).ToList();
            var shifts = db.Shifts.Where(shift => EntityFunctions.DiffDays(shift.StartTime, dateFrom) == 0);
                        
            //var shifts = db.Shifts.Include(s => s.Terminal).Where(s=>s.StartTime.DayOfYear==dateFrom.DayOfYear&&s.StartTime.Year==dateFrom.Year).ToList();
            var employees = db.Employees.Where(e => e.BranchId == id).ToList();
            var empPersonIdHs=new HashSet<int>( employees.Select(i=>i.PersonId));
            foreach (var terminal in terminals)
            {
                              
                    var _shift = new Shift();
                    _shift = shifts.Where(s => s.TerminalId==terminal.TerminalId /*&& s.isClosed!=true*/).SingleOrDefault();
                    if (_shift!=null)
                    {
                        _shift.Terminal = terminal;
                        if (_shift.PersonId != null)
                        {
                            if (empPersonIdHs.Contains((int)_shift.PersonId))
                            {
                                _shift.Cashier = employees.SingleOrDefault(e => e.PersonId == _shift.PersonId);

                            }
                        }
                    //terminals.Single(t => t.TerminalId.Equals(terminal.TerminalId)).Shifts.Add(_shift);
                    }                
                    else {
                        _shift = new Shift();
                        _shift.ShiftId = 0;
                        _shift.Terminal = terminal;
                       // terminals.Single(t => t.TerminalId.Equals(terminal.TerminalId)).Shifts.Add(new Shift());
                }
                    shiftToSend.Add(_shift);
            }


            return shiftToSend; 
        }
        public IHttpActionResult stopShift(int id)
        {
            if (id != null)
            {
                var shift = Queryable.SingleOrDefault(db.Shifts.Include(e=>e.Terminal), e=>e.ShiftId==id);
                if (shift != null)

                    if (shift.TerminalId != null)
                    {
                        shift.Terminal.isActive = true;
                    }
                    shift.IsClosed = true;
                    db.Entry(shift).State = System.Data.Entity.EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                        return Ok("Success");

                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        throw;
                    }
                }
                else {
                    return NotFound();
                }
            return Ok(HttpStatusCode.NoContent);
            }
           
      
        // GET: api/Shift/5
        [ResponseType(typeof(Shift))]
        public IHttpActionResult GetShift(int id)
        {
            Shift shift = db.Shifts.Find(id);
            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
        }

        // PUT: api/Shift/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShift(int id, Shift shift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shift.ShiftId)
            {
                return BadRequest();
            }

            db.Entry(shift).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftExists(id))
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

        // POST: api/Shift
        [ResponseType(typeof(Shift))]
        public IHttpActionResult PostShift(Shift shift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            db.Shifts.Add(shift);
            db.SaveChanges();

            //if (shift.TerminalId != null)
            //{
            //    var terminal = db.Terminals.SingleOrDefault(t => t.TerminalId == shift.TerminalId);
            //    terminal.isActive = true;
            //    db.Entry(terminal).State = EntityState.Modified;
            //    db.SaveChanges();
               
            //}

            return CreatedAtRoute("DefaultApi", new { id = shift.ShiftId }, shift);
        }
        //public IHttpActionResult stopShift(int? id)
        //{
        //    if (id==null)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    var _shift = db.Shifts.Include(s=>s.Terminal).SingleOrDefault(s=>s.ShiftId==id);
        //    if (_shift != null)
        //    {
        //        _shift.IsClosed = true;
        //        if (_shift.Terminal!=null)
        //        {
        //            _shift.Terminal.isActive = false;
        //        }
               
        //        db.Entry(_shift).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
           

            //if (shift.TerminalId != null)
            //{
            //    var terminal = db.Terminals.SingleOrDefault(t => t.TerminalId == shift.TerminalId);
            //    terminal.isActive = true;
            //    db.Entry(terminal).State = EntityState.Modified;
            //    db.SaveChanges();

            //}

           // return Ok("Success");
       // }

        // DELETE: api/Shift/5
        [ResponseType(typeof(Shift))]
        public IHttpActionResult DeleteShift(int id)
        {
            Shift shift = db.Shifts.Find(id);
            if (shift == null)
            {
                return NotFound();
            }

            db.Shifts.Remove(shift);
            db.SaveChanges();

            return Ok(shift);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShiftExists(int id)
        {
            return db.Shifts.Count(e => e.ShiftId == id) > 0;
        }
    }
}