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
        public async Task<IHttpActionResult> GetMatches()
        {
            var currentSetNo = SetNumberGenerator.GetCurrentSetNumber;
            var games = await db.ShortMatchCodes.Include(s => s.Match).Include(mo => mo.Match.GameOdds.Select(m => m.BetOption)).OrderBy(x => x.ShortCode).ToListAsync();
            var startTime = DateTime.Now;

            var filteredgames = games.Select(g => new GameViewModel
            {
                AwayScore = g.Match.AwayScore,
                AwayTeamId = g.Match.AwayTeamId,
                AwayTeamName = g.Match.AwayTeam.TeamName,
                Champ = g.Match.Champ,
                GameOdds = g.Match.GameOdds.Select(go => new GameOddViewModel
                {
                    BetCategory = go.BetOption.BetCategory.Name,
                    BetOptionId = go.BetOptionId,
                    BetOption = go.BetOption.Option,
                    LastUpdateTime = go.LastUpdateTime,
                    Odd = go.Odd,
                    HandicapGoals = go.HandicapGoals
                }).ToList(),
                GameStatus = g.Match.GameStatus,
                HalfTimeAwayScore = g.Match.HalfTimeAwayScore,
                HalfTimeHomeScore = g.Match.HalfTimeHomeScore,
                HomeScore = g.Match.HomeScore,
                HomeTeamName = g.Match.HomeTeam.TeamName,
                HomeTeamId = g.Match.HomeTeamId,
                MatchNo = g.ShortCode,
                RegistrationDate = g.Match.RegistrationDate,
                ResultStatus = g.Match.ResultStatus,
                SetNo = g.SetNo,
                OldDateTime = g.Match.StartTime,
                StartTime = String.Format("{0:dd/M/yyyy}", g.Match.StartTime)
            }).Where(x => x.OldDateTime > startTime).OrderBy(s => s.StartTime);
            return Ok(filteredgames);
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

            if (id != match.MatchNo)
            {
                return BadRequest();
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
                if (MatchExists(match.MatchNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = match.MatchNo }, match);
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
            return db.Matches.Count(e => e.MatchNo == id) > 0;
        }
    }
}