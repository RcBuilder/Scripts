using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{    
    public class ApiController : Controller
    {        
        [HttpPost]
        public JsonResult Test()
        {
            return Json(new
            {
                Status = "OK"
            });
        }
    }
}