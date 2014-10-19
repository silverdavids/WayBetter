using System.Data.Entity;
using ZeusConsole.Models;

namespace ZeusConsole
{
    public class ZeusDbContext: DbContext
    {
         public ZeusDbContext()
            : base("BetConnection")
        {
        }

        public DbSet<Match> Matches { get; set; }

        public DbSet<MatchOdd> MatchOdds { get; set; }

        public DbSet<BetCategory> BetCategories { get; set; }

        public DbSet<BetOption> BetOptions { get; set; }
    }
}
