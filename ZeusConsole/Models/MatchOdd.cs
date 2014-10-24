using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZeusConsole.Models
{
    public class MatchOdd
    {
        public MatchOdd()
        {
            SetNo = 0;
            ShortMatchCode = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchOddId { get; set; }

        [ForeignKey("Match")]
        public int BetServiceMatchNo { get; set; }

        public int SetNo { get; set; }

        public int ShortMatchCode { get; set; }

        [ForeignKey("BetOption")]
        public int BetOptionId { get; set; }
        public decimal Odd { get; set; }
        public int? HandicapGoals { get; set; }
        public DateTime? LastUpdateTime { get; set; }

        public virtual BetOption BetOption { get; set; }

        public Match Match { get; set; }
    }
}
