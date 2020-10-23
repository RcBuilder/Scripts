using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;

namespace Website.Controllers
{
    public class FormsController : Controller
    {
        [HttpPost]
        public JsonResult RegistrationStep1(Models.SomeModel SomeModel)
        {
            if (ModelState.IsValid)
                return Json(new { Status = "OK" });
            return Json(new { Status = "ERROR", State = Helper.ModelStateToJson(ModelState) });
        }

        [HttpPost]
        public JsonResult RegistrationStep2(Models.SomeModel SomeModel)
        {
            if (ModelState.IsValid)
                return Json(new { Status = "OK" });
            return Json(new { Status = "ERROR", State = Helper.ModelStateToJson(ModelState) });
        }

        [HttpPost]
        public JsonResult RegistrationStep3(Models.SomeModel SomeModel)
        {
            if (ModelState.IsValid)
                return Json(new { Status = "OK" });
            return Json(new { Status = "ERROR", State = Helper.ModelStateToJson(ModelState) });
        }
    }
}