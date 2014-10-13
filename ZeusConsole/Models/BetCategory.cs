using System.Collections.Generic;

namespace ZeusConsole.Models
{
    public class BetCategory
    {
        public BetCategory()
        {
            BetOptions = new HashSet<BetOption>();
        }
        public int BetCategoryId { get; set; }

        public string BetCategoryName { get; set; }

        public ICollection<BetOption> BetOptions { get; set; } 
    }
}
