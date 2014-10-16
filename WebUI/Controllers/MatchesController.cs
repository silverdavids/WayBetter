using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Domain.Models.Concrete;
using Domain.Models.ViewModels;
using WebUI.Helpers;

namespace WebUI.Controllers
{
    public class MatchesController : CustomController
    {
        // GET: Matches
        //public async Task<ActionResult> Index()
        //{
        //    var matches = BetDatabase.Matches.Include(m => m.AwayTeam).Include(m => m.HomeTeam).Include(m => m.ShortMatchCode);
        //    return View(await matches.ToListAsync());
        //}

        // GET: Match
        public async Task<ActionResult> Index()
        {

            if (!Request.IsAjaxRequest())
            {
                //var account = await BetDatabase.Accounts.Select(a => new
                //{
                //    a.UserId,
                //    a.AmountE
                //}).SingleOrDefaultAsync(t => t.UserId == User.Identity.Name);
                //ViewBag.Balance = account.AmountE;
                return View();
            }

            //var currentSetNo = SetNumberGenerator.GetCurrentSetNumber;
            var games = await BetDatabase.ShortMatchCodes.Include(s => s.Match).OrderBy(x => x.ShortCode).ToListAsync();
            var startTime = DateTime.Now;

            var filteredgames = games.Select(g => new GameViewModel
            {
                AwayScore = g.Match.AwayScore,
                AwayTeamId = g.Match.AwayTeamId,
                AwayTeamName = g.Match.AwayTeam.TeamName,
                Champ = g.Match.League,
                MatchOdds = g.Match.MatchOdds.Select(go => new GameOddViewModel
                {
                    BetCategory = go.BetOption.BetCategory.Name,
                    BetOptionId = go.BetOptionId,
                    BetOption = go.BetOption.Option,
                    LastUpdateTime = go.LastUpdateTime,
                    Odd = go.Odd,
                    Line = go.Line
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
            // .Where(x => x.OldDateTime>startTime)
            return Json(filteredgames, JsonRequestBehavior.AllowGet);
        }

        // GET: Matches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var match = await BetDatabase.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // GET: Matches/Create
        public ActionResult Create()
        {
            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName");
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName");
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo");
            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BetServiceMatchNo,League,StartTime,GameStatus,AwayTeamId,HomeTeamId,RegistrationDate,HomeScore,AwayScore,HalfTimeHomeScore,HalfTimeAwayScore,ResultStatus")] Match match)
        {
            if (ModelState.IsValid)
            {
                BetDatabase.Matches.Add(match);
                await BetDatabase.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.AwayTeamId);
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.HomeTeamId);
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo", match.BetServiceMatchNo);
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var match = await BetDatabase.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.AwayTeamId);
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.HomeTeamId);
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo", match.BetServiceMatchNo);
            return View(match);
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BetServiceMatchNo,League,StartTime,GameStatus,AwayTeamId,HomeTeamId,RegistrationDate,HomeScore,AwayScore,HalfTimeHomeScore,HalfTimeAwayScore,ResultStatus")] Match match)
        {
            if (ModelState.IsValid)
            {
                BetDatabase.Entry(match).State = EntityState.Modified;
                await BetDatabase.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AwayTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.AwayTeamId);
            ViewBag.HomeTeamId = new SelectList(BetDatabase.Teams, "TeamId", "TeamName", match.HomeTeamId);
            ViewBag.BetServiceMatchNo = new SelectList(BetDatabase.ShortMatchCodes, "MatchNo", "MatchNo", match.BetServiceMatchNo);
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var match = await BetDatabase.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var match = await BetDatabase.Matches.FindAsync(id);
            BetDatabase.Matches.Remove(match);
            await BetDatabase.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BetDatabase.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
