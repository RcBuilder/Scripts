using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Common
{
    public class Helper
    {
        // MVC Model To Json State
        public static dynamic ModelStateToJson(ModelStateDictionary ModelState)
        {
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
