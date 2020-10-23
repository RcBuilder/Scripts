using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestReactNET.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Render()
        {
            return View();
        }

        public ActionResult Css()
        {
            return View();
        }

        public ActionResult Lifecycle()
        {
            return View();
        }
        
        public ActionResult Forms()
        {
            return View();
        }

        public ActionResult State()
        {
            return View();
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Wrapper()
        {
            return View();
        }

        public ActionResult Props()
        {
            return View();
        }

        public ActionResult API()
        {
            return View();
        }

        public ActionResult Model()
        {
            return View(new Models.SomeModel {
                Id = 100,
                Name = "Test Model",
                Price = 99.9F,
                Expiry = DateTime.Now
            });
        }

        [HttpPost]
        public ActionResult CheckModel(Models.SomeModel SomeModel)
        {            
            if (ModelState.IsValid)
                return Json(new { Status = "OK" });
            return Json(new { Status = "ERROR", State = ModelStateToJson(ModelState) });
        }

        public ActionResult CustomRouter()
        {
            return View();
        }

        // ------- 

        private static dynamic ModelStateToJson(ModelStateDictionary ModelState) {
            var errorList = (
                from item in ModelState
                where item.Value.Errors.Any()
                select new
                {
                    key = item.Key,
                    errors = item.Value.Errors.Select(e => e.ErrorMessage)
                }
            );

            return errorList;
        }
    }
}