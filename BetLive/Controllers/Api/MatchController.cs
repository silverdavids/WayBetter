using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Domain.Models.Concrete;
using WebUI.DataAccessLayer;
using WebUI.Helpers;
using Domain.Models.ViewModels;

namespace BetLive.Controllers.Api
{
    public class MatchController : ApiController
    {
   //     private ApplicationDbContext db ;
   //     public MatchController() { }
   //public MatchController(ICustomController dbContext){
   //              db=dbContext.getDbContext();
   
   //}

        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: api/Match
        #region getMatches
        public async Task<IEnumerable<GameViewModel>> GetMatches()
        {
           

            var games = await db.Matches.Include(m => m.HomeTeam).Include(m => m.AwayTeam).ToListAsync();
            //var games = await BetDatabase.ShortMatchCodes.Include(s => s.Match).OrderBy(x => x.ShortCode).ToListAsync();
            //var startTime = DateTime.Now;
            if (games == null || games.Count() == 0)
            {
                return new List<GameViewModel>();
            }
           
            var filteredgames = games.Select(g => new GameViewModel
            {
                AwayScore = g.AwayScore,
                AwayTeamId = g.AwayTeamId,
                AwayTeamName = g.AwayTeam.TeamName,
                Champ = g.League,
                MatchOdds = g.MatchOdds.Select(go => new GameOddViewModel
                {
                    BetCategory = go.BetOption.BetCategory.Name,
                    BetOptionId = go.BetOptionId,
                    BetOption = go.BetOption.Option,
                    LastUpdateTime = go.LastUpdateTime,
                    Odd = go.Odd,
                    Line = go.BetOption.Line
                }).ToList(),
                GameStatus = g.GameStatus,
                HalfTimeAwayScore = g.HalfTimeAwayScore,
                HalfTimeHomeScore = g.HalfTimeHomeScore,
                HomeScore = g.HomeScore,
                HomeTeamName = g.HomeTeam.TeamName,
                HomeTeamId = g.HomeTeamId,
                MatchNo = games.IndexOf(g) + 1,
                RegistrationDate = g.RegistrationDate,
                ResultStatus = g.ResultStatus,
                SetNo = 1234,
                OldDateTime = g.StartTime,
                StartTime = String.Format("{0:dd/M/yyyy}", g.StartTime)
            }).OrderBy(s => s.StartTime);
            return filteredgames.ToList();
        }
        #endregion
        #region getBetCategories
        public async Task<IHttpActionResult> getBetCategories()
        {
            var betCategories =await db.BetCategories.ToListAsync();
            return Ok(betCategories);
        }

        #endregion
        // GET: api/Match/5
        [ResponseType(typeof(Match))]
        public async Task<IHttpActionResult> GetMatch(int id)
        {
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
        }

        // PUT: api/Match/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMatch(int id, Match match)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok("Index");
            }
           

            db.Entry(match).State = System.Data.Entity.EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchExists(id))
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

        // POST: api/Match
        [ResponseType(typeof(Match))]
        public async Task<IHttpActionResult> PostMatch(Match match)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Matches.Add(match);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MatchExists(match.BetServiceMatchNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = match.BetServiceMatchNo }, match);
        }

        // DELETE: api/Match/5
        [ResponseType(typeof(Match))]
        public async Task<IHttpActionResult> DeleteMatch(int id)
        {
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            db.Matches.Remove(match);
            await db.SaveChangesAsync();

            return Ok(match);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MatchExists(int id)
        {
            return db.Matches.Count(e => e.BetServiceMatchNo == id) > 0;
        }
    }
}