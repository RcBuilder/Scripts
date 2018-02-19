using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MySite.API
{
    public class ProductsController : ApiController
    {
        private static DAL.Products products = new DAL.Products();

        public IEnumerable<Models.Product> GET()
        {
            return products.Get();
        }

        public IEnumerable<Models.Product> PUT(Models.Product NewProduct)
        {
            products.Add(NewProduct);
            return products.Get();
        }    

        public IEnumerable<Models.Product> DELETE(int Id)
        {
            products.Delete(Id);
            return products.Get();
        }

        public IEnumerable<Models.Product> POST(Models.Product Product)
        {
            products.Update(Product);
            return products.Get();
        }
    }
}
