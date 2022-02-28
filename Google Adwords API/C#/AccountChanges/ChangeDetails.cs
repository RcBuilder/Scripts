using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAdwordsAPI
{
    public class ChangeDetails
    {
        public string AccountId { get; set; }       
        public bool HasChanged { get; set; }

        public ChangeDetails(string AccountId, bool HasChanged)
        {
            this.AccountId = AccountId;
            this.HasChanged = HasChanged;
        }
    }
}
