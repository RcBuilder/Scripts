using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLL
{
    #region Blocker:
    public class Blocker
    {
        #region consts:
        private const int MaxSearches = 5; // max searches per x minutes
        private const int RangeInMinutes = 1; // range in minutes
        #endregion

        #region Properties:
        public static Dictionary<string, IPSearches> searches = new Dictionary<string, IPSearches>();
        #endregion

        #region isBlocked:
        public static bool isBlocked(string IP) {
            if (!searches.ContainsKey(IP))
                searches.Add(IP, new IPSearches());

            IPSearches ipsearches = searches[IP];

            TimeSpan ts = DateTime.Now - ipsearches.startDate;

            if (ts.TotalMinutes > RangeInMinutes)
                ipsearches.Reset();

            if (ipsearches.Counter >= MaxSearches)
                return true;

            ipsearches.Add();
            return false;
        }
        #endregion
    }
  
    public class IPSearches
    {
        #region Properties:
        public DateTime startDate { private set; get; }
        public int Counter { private set; get; }
        #endregion

        #region Constructor:
        public IPSearches()
        {
            this.startDate = DateTime.Now;
            this.Counter = 0;
        }
        #endregion

        #region Reset:
        public void Add()
        {
            this.Counter++; 
        }
        #endregion

        #region Reset:
        public void Reset() {
            this.startDate = DateTime.Now;
            this.Counter = 0; // reset counter
        }
        #endregion
    }
    #endregion
}