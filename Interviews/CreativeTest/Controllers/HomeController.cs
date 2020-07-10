using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CreativeTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            var dbProvider = new DAL.DBProviderFactory().Produce(ConfigurationManager.AppSettings["DBProvider"]);
            return View(dbProvider.GetTransactions());
        }

        public ActionResult Details(int Id)
        {
            var dbProvider = new DAL.DBProviderFactory().Produce(ConfigurationManager.AppSettings["DBProvider"]);
            return View(dbProvider.GetTransactionDetails(Id));
        }
    }
}