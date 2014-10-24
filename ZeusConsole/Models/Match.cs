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
            SetNo = 0;
            ShortMatchCode = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 100)]
        public int BetServiceMatchNo { get; set; }
        public int SetNo { get; set; }
        public int ShortMatchCode { get; set; }
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
