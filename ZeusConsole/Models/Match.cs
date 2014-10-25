using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZeusConsole.Models
{
    public class Match
    {
        public Match()
        {
            MatchOdds  = new HashSet<MatchOdd>();
            AwayScore = 0;
            HomeScore = 0;
            HalfTimeHomeScore = 0;
            HalfTimeAwayScore = 0;
            RegistrationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BetServiceMatchNo { get; set; }
        public string League { get; set; }
        public DateTime StartTime { get; set; }
        public string GameStatus { get; set; }
        public string AwayTeamName { get; set; }
        public string HomeTeamName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int HalfTimeHomeScore { get; set; }
        public int HalfTimeAwayScore { get; set; }
        public int? ResultStatus { get; set; }
        public ICollection<MatchOdd> MatchOdds { get; set; }
    }
}
