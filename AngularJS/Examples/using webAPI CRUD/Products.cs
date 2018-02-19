using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace MySite.DAL
{
    public class Products
    {
        private readonly static string ProductsTable = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\", "Products.xml");
        private List<Models.Product> _Products = null;

        public Products() {
            LoadProducts();
        }

        private void LoadProducts() {
            /*
            _Products = new List<Models.Product>();
            
            var xDoc = XDocument.Load(ProductsTable);
            foreach (var elem in xDoc.Root.Elements("Product")) {
                _Products.Add(new Models.Product { 
                    Id = Convert.ToInt32(elem.Attribute("Id").Value), 
                    Name = elem.Element("Name").Value,
                    State = elem.Element("State") == null ? false : Convert.ToBoolean(elem.Element("State").Value)
                });
            }
            */

            using (var stream = new StreamReader(ProductsTable))
            {
                var serialize = new XmlSerializer(typeof(List<Models.Product>), new XmlRootAttribute("Products"));
                this._Products = (List<Models.Product>)serialize.Deserialize(stream);
            }              
        }

        private void SaveProducts() {
            using (var stream = new StreamWriter(ProductsTable))
            {
                var serialize = new XmlSerializer(typeof(List<Models.Product>), new XmlRootAttribute("Products"));
                serialize.Serialize(stream, this._Products);
            }    
        }

        public IEnumerable<Models.Product> Get() {
            return _Products.ToList();
        }

        public bool Add(Models.Product NewProduct)
        {                       
            _Products.Add(NewProduct);
            SaveProducts();
            return true;
        }

        public bool Delete(int Id)
        {
            var Product = _Products.Find(x => x.Id == Id);
            if (Product == null) return false;

            _Products.Remove(Product);
            SaveProducts();
            return true;
        }

        public bool Update(Models.Product ProductToUpdate)
        {
            var Product = _Products.Find(x => x.Id == ProductToUpdate.Id);
            if (Product == null) return false;

            Product.Name = ProductToUpdate.Name;
            Product.State = ProductToUpdate.State;

            SaveProducts();
            return true;
        }

    }
}