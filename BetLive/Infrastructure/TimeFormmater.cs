using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetLive.Infrastructure
{
    public  class TimeFormmater
    {
        /// <summary>
        /// This converte Minutesfrom eg 12:35 two a double
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns>double eg 12.35</returns>
        public  string formatMinutes(string minutes)
        {
            char[] del = { ':' };
            var stdate = minutes.Split(del);
            var stDateTime = stdate[0] + "." + stdate[1];
            return stDateTime;
        }

        public DateTime getFormattedStartDate(string gameNode, string startTime)
        {
            char[] del = { '.' };
            var stdate = gameNode.Split(del);
            var stDateTime = stdate[1] + "-" + stdate[0] + "-" + stdate[2]
                             + " " + startTime + ":00";
            return Convert.ToDateTime(stDateTime).ToLocalTime();

        }
    }
}