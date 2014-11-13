using Domain.Models.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ViewModels
{
   public class LiveMatch
    {
      
       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.None)]
       public string LiveMatchNo { get; set; }//this is incremental for the entire life time of the app, but there is need for a permutative numbering algorithm
       public int DailyShortCode { get; set; }  //this is reverted back to one every day
        // this property references the generated BetServiceNo from matches but it  generated as ShortMatchCodes for the live bets
        //so instead of the BetServiceNo for the lives ,we use the shorts
         [ForeignKey("Match")]
        public int? BetServiceMatchNo  { get; set; }//this is meant for the bet service match number in the macthes table just for reference
        [Required]
        public virtual Match Match { get; set; }
       //For the mean time we shall increment these numbers daily by date 
        public string SetDate { get; set; }
       
       
    }
}
