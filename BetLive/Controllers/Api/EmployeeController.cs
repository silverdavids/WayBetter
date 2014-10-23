using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Web.Mvc;
using Domain.Models.Concrete;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using WebUI.DataAccessLayer;

namespace BetLive.Controllers.Api
{
    public class EmployeeController: ApiController{
          private readonly ApplicationDbContext _betDatabase = new ApplicationDbContext();
        public async Task<List<Employee>> GetEmployees(int branchId = 0)
        {
            var employees = await _betDatabase.Employees.Where(e => e.BranchId == branchId).ToListAsync();
            return employees;
        }

        // GET: api/Employ
        public async Task<IHttpActionResult> GetPerson(int id = 0)
        {
            var employees = await _betDatabase.Employees.Where(e => e.BranchId == id).Select(e=>new
            {
                PersonId=e.PersonId,
                Name=e.FirstName
            }).ToListAsync();

            return Ok(employees);

            /*var empNameIds = new List<EmployeeNameId>();
            
            //var employeesIdsToSelectFrom = new HashSet<int>(employeesForThisBranch.Select(e=>e.UserId));
            //var terminalIdsForThisBranch = new HashSet<int>(BetDatabase.Terminal.Where(t => t.BranchId.Equals(id)).Select(t => t.TerminalId));
            //var employeesNamesToSendToClient = new HashSet<int>(BetDatabase.Shift.Where(s=>s.isClosed!=true).Select(s=>s.UserId));
            employeesForThisBranch.ForEach(e => new AcceptVerbsAttribute{});
            foreach (var employee in employeesForThisBranch)
            {
                var _empNameId = new EmployeeNameId {Id = employee.Id, Name = employee.FirstName};

                empNameIds.Add(_empNameId);
            
            }

            return empNameIds;*/
        }

        // GET: api/Employ/5
        public IHttpActionResult GetEmployee(int id)
        {
            var employee = _betDatabase.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employ/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.PersonId)
            {
                return BadRequest();
            }

            _betDatabase.Entry(employee).State = EntityState.Modified;

            try
            {
                _betDatabase.SaveChanges();
            }
            catch (Exception)
            {
                if (!EmployeeExists(id))
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

        private bool EmployeeExists(int id)
        {
            var emp=_betDatabase.Employees.SingleOrDefaultAsync(e => e.BranchId == id);
            if (emp==null)
            {
                return false;
            }
            return true;
        }
             

        // POST: api/Employ
        [ResponseType(typeof(Employee))]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _betDatabase.Employees.Add(employee);
            _betDatabase.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = employee.PersonId }, employee);
        }

         //DELETE: api/Employ/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(string id)
        {
            var employee = _betDatabase.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            _betDatabase.Employees.Remove(employee);
            _betDatabase.SaveChanges();

            return Ok(employee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _betDatabase.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}