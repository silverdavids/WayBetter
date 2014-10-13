namespace Domain.Models.ViewModels
{
   public  class MatchScores
    {
      public int MatchNo { get; set; }
      public string League { get; set; }
       public string HomeTeam { get; set; }
       public string AwayTeam { get; set; }
       public int HomeScore { get; set; }
       public int AwayScore { get; set; }
       public int HalfTimeHomeScore { get; set; }
       public int HalfTimeAwayScore { get; set; }
       public string StartTime { get; set; }
   
   }
}
