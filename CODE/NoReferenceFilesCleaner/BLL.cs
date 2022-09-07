using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Code
{
    public class BLL
    {
        protected DAL Dal { get; set; } = new DAL();
        
        public IEnumerable<Entities.VideoResourceData> GetVideoResources()
        {
            return this.Dal.GetVideoResources();            
        }

        public IEnumerable<Entities.BookResourceData> GetBookResources()
        {
            return this.Dal.GetBookResources();
        }
    }
}
