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
using WebUI.Helpers;




namespace BetLive.Controllers.Api
{
    public class BranchController : ApiController
    {
        private ApplicationDbContext db;
        private ICustomController _userManager;
        //private readonly ApplicationDbContext db = new ApplicationDbContext();
        public BranchController(){}
        public BranchController(ICustomController _db) {
            db = _db.getDbContext();
        
        }
        // GET: api/Branch
        public IQueryable<Branch> GetBranch()
        {
            return db.Branches.Include(b=>b.Terminals).Include(b=>b.Employees);
        }

        public IList<Branch> GetBranchByCompanyId( int id=0)
        {
            var branch=db.Branches.Include(b => b.Terminals).Include(b => b.Employees).Where(b=>b.Company.CompanyId.Equals(id));
            if(branch==null){
            var emptyBranch=new List<Branch>();
            return  emptyBranch;
            }
            return branch.ToList(); 
        }

        // GET: api/Branch/5
        [ResponseType(typeof(Branch))]
        public IHttpActionResult GetBranch(int id)
        {
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return NotFound();
            }

            return Ok(branch);
        }

        // PUT: api/Branch/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBranch(int id, Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != branch.BranchId)
            {
                return BadRequest();
            }

            db.Entry(branch).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
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

        // POST: api/Branch
        [ResponseType(typeof(Branch))]
        public IHttpActionResult PostBranch(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Branches.Add(branch);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = branch.BranchId }, branch);
        }

        // DELETE: api/Branch/5
        [ResponseType(typeof(Branch))]
        public IHttpActionResult DeleteBranch(int id)
        {
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return NotFound();
            }

            db.Branches.Remove(branch);
            db.SaveChanges();

            return Ok(branch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BranchExists(int id)
        {
            return db.Branches.Count(e => e.BranchId == id) > 0;
        }
    }
}