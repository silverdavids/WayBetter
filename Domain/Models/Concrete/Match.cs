using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Concrete
{
    public class Match
    {
        public Match()
        {
            Bets = new HashSet<Bet>();
            ShortMatchCodes = new HashSet<ShortMatchCode>();
            AwayScore = 0;
            HomeScore = 0;
            HalfTimeHomeScore = 0;
            HalfTimeAwayScore = 0;
            RegistrationDate = DateTime.Now;
            LiveBetFlag = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BetServiceMatchNo { get; set; }
        public string League { get; set; }
        public DateTime StartTime { get; set; }
        public string GameStatus { get; set; }
        [ForeignKey("AwayTeam")]
        public int AwayTeamId { get; set; }
        [ForeignKey("HomeTeam")]
        public int HomeTeamId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int HalfTimeHomeScore { get; set; }
        public int HalfTimeAwayScore { get; set; }
        public int? ResultStatus { get; set; }

        public bool LiveBetFlag
        {
            get; set;
        }
        [Required]
        public Team AwayTeam { get; set; }
        [Required]
        public Team HomeTeam { get; set; }
        public ICollection<Bet> Bets { get; set; }
        public ICollection<MatchOdd> MatchOdds { get; set; }
        public ICollection<ShortMatchCode> ShortMatchCodes { get; set; }
    }
}
