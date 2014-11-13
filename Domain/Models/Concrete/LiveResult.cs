using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Concrete
{
  public  class LiveResult
    {
      public int LiveResultId { get; set; }
      public string ResultMinute { get; set; }
      public string HomeScore { get;set;}
      public string AwayScore{get;set;}
      public DateTime TimeUpdated { get; set; }
      //[ForeignKey("BetOption")]
      //public int BetOptionId { get; set; }
      //[ForeignKey("BetCategory")]
      //public int BetCategoryId { get; set; }
      [ForeignKey("Match")]
      public int BetServiceMatchNo { get; set; }
      //Navigation Properties
      //public virtual BetCategory  BetCategory {get ;set;}
      //public virtual BetOption BetOption { get; set; }
      public virtual Match Match { get; set; }
    }
}
