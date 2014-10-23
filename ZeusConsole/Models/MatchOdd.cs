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
            HandicapGoals = 0;
            LastUpdateTime = DateTime.Now;
        }

        [Key]
        [Column(Order = 50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchOddId { get; set; }

        [ForeignKey("Match")]
        public int BetServiceMatchNo { get; set; }

        [Key]
        [Column(Order = 200)]
        public int SetNo { get; set; }

        [Key]
        [Column(Order = 300)]
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
