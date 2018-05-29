using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaybu.Compare.Data.BaseClasses;

namespace Zaybu.Compare.Data
{
    public class Supplier
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [Description("Suppliers Main Products")]
        public List<Product> Products { get; set; }
        public Dictionary<string, Address> Addresses { get; set; }        
        public DateTime Created { get; set; }
        public SupplierStatus Status { get; set; }
        public Array Contacts { get; set; }
        public object Logo { get; set; }
    }

    public class Product
    {
        private ProductCode _code;

        [Description("Unique ID Field")]
        public int ID { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        [Description("Inventory Code")]
        public Guid InventoryID { get; set; }
        public ProductCode Code { get { return _code; } set { _code = value; } }
        public byte[] ImageData { get; set; }
    }

    public class Address
    {
        public int ID { get; set; }
        public string AddressLine { get; set; }
        public PostCode PostCode { get; set; }
    }

    public class PostCode
    {
        public string Inner { get; set; }
        public string Outer { get; set; }
    }

    public struct ProductCode
    {
        public int ProductID;
        public char Category;
    }

    public enum SupplierStatus
    {
        Active, InActive
    }

    public class Contact
    {
        public string Name { get; set; }
    }

    public class Promotion : EntityBase
    {
        public int PromotionId { get; set; }
        public List<TimeSpan> ApplicableDates { get; set; }
        public decimal Cost { get; set; }
    }

    public class SampleData
    {
        public static List<Supplier> CreateSuppliers(int numberToCreate = 10)
        {
            List<Supplier> suppliers = new List<Supplier>();

            while (numberToCreate > 0)
            {
                suppliers.Add(CreateSupplier(numberToCreate));
                numberToCreate--;
            }

            return suppliers;
        }

        public static Supplier CreateSupplier(int seed = 0)
        {
            Supplier supplier = new Supplier();

            supplier.ID = seed;
            supplier.Name = SupplierNames.GetSeededEntry(seed);
            supplier.Created = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            supplier.Status = new Random().Next(1, 2) == 1 ? SupplierStatus.InActive : SupplierStatus.Active;
            supplier.Logo = new System.Drawing.Bitmap(100, 100).PhysicalDimension;
            supplier.Addresses = CreateAddresses(new Random(seed).Next(2, 4));
            supplier.Products = CreateProducts(new Random(seed).Next(2, 10));
            supplier.Contacts = CreateContacts(new Random(seed).Next(1, 4)).ToArray();
            return supplier;
        }


        public static List<Product> CreateProducts(int seed)
        {
            List<Product> products = new List<Product>();

            while (seed > 0)
            {
                products.Add(CreateProduct(seed));
                seed--;
            }

            return products;
        }

        public static Product CreateProduct(int seed)
        {
            Product product = new Product();

            product.ID = seed;
            ProductCode pc = new ProductCode();
            pc.ProductID = seed;
            pc.Category = Convert.ToChar(64 + seed);
            product.Code = pc;
            product.Description = Products.GetSeededEntry(seed);
            product.InventoryID = new Guid(new String(seed.ToString().ToCharArray()[0], 32));
            product.Price = Convert.ToSingle(Math.Round(seed * 1.12345, 2));
            product.ImageData = CreateByteArray(seed);

            return product;
        }

        public static byte[] CreateByteArray(int seed)
        {
            int totalBytes = new Random(seed).Next(10, seed + 10);
            byte[] bytes = new byte[totalBytes];

            new Random(seed).NextBytes(bytes);

            return bytes;
        }

        public static Dictionary<string, Address> CreateAddresses(int seed)
        {
            Dictionary<string, Address> addresses = new Dictionary<string, Address>();

            while (seed > 0)
            {
                addresses.Add(AddressNames.GetSeededEntry(seed), CreateAddress(seed));
                seed--;
            }

            return addresses;
        }

        public static List<Address> CreateAddressList(int seed)
        {
            List<Address> addresses = new List<Address>();

            while (seed > 0)
            {
                addresses.Add(CreateAddress(seed));
                seed--;
            }

            return addresses;
        }

        private static Address CreateAddress(int seed)
        {
            Address address = new Address();
            address.ID = seed;
            address.AddressLine = AddressLine1s.GetSeededEntry(seed);
            address.PostCode = CreatePostCode(seed);
            return address;
        }

        private static PostCode CreatePostCode(int seed)
        {
            PostCode pc = new PostCode();
            var parts = PostCodes.GetSeededEntry(seed).Split(' ');
            pc.Inner = parts[0];
            pc.Outer = parts[1];
            return pc;
        }

        public static List<Contact> CreateContacts(int seed)
        {
            var contacts = new List<Contact>();
            while (seed > 0)
            {
                contacts.Add(CreateContact(seed));
                seed--;
            }
            return contacts;
        }

        private static Contact CreateContact(int seed)
        {
            Contact contact = new Contact();
            contact.Name = FirstNames.GetSeededEntry(seed) + " " + LastNames.GetSeededEntry(seed);
            return contact;
        }

        public static Simples CreateSimple()
        {
            Simples simple = new Simples();

            simple.Bool = true;
            simple.Char = 'a';
            simple.Byte = 1;
            simple.BoolNullable = null;
            simple.CharNullable = null;
            simple.ByteNullable = null;
            simple.Int16 = 2;
            simple.Int32 = 3;
            simple.Int64 = 4;
            simple.Int16Nullable = null;
            simple.Int32Nullable = null;
            simple.Int64Nullable = null;
            simple.Single = 1.10f;
            simple.Double = 2.20;
            simple.Decimal = (decimal)3.30;
            simple.SingleNullable = null;
            simple.DoubleNullable = null;
            simple.DecimalNullable = null;
            simple.Date = new DateTime(2020, 01, 01, 12, 0, 0);
            simple.DateNullable = null;
            simple.CharArray = new char[5] { 'a', 'b', 'c', 'd', 'e' };
            simple.ByteArray = new byte[5] { 1, 2, 3, 4, 5 };
            simple.String = "Simple string";

            return simple;
        }


        public static Customer CustomerA()
        {
            Customer customer = new Customer { Name = "Customer A", Id = 1, Latitude = (Single)(-2.140000), Longitude = (Single)(52.450) };
            customer.RetailOutlets = SampleData.CreateOutlets(5);
            customer.RetailOutlets[3].Departments = CreateExtendedDepartments();
            customer.RetailOutlets[4].Departments = CreateAllDepartments();
            customer.SpecialEvents.Add(new DateTime(2020, 12, 25), "Christmas Party");
            customer.SpecialEvents.Add(new DateTime(2020, 8, 10), "Summer Festival");
            return customer;
        }

        public static Customer CustomerB()
        {
            Customer customer = new Customer { Name = "Customer B", Id = 2, Latitude = (Single)(-2.160000), Longitude = (Single)(52.950) };
            customer.RetailOutlets = SampleData.CreateOutlets(8);
            customer.SpecialEvents.Add(new DateTime(2021, 12, 31), "New Years Eve Dinner");
            customer.SpecialEvents.Add(new DateTime(2020, 8, 10), "Summer Festival");
            return customer;
        }

        public static List<RetailOutlet> CreateOutlets(int number, OutletType outletType = OutletType.Online, int floors = 1)
        {
            List<RetailOutlet> outlet = new List<RetailOutlet>();
            for (int index = 0; index < number; index++)
            {
                outlet.Add(CreateOutlet(index + 1, outletType, floors, CreateStandardDepartments()));
            }
            return outlet;
        }

        public static RetailOutlet CreateOutlet(int id, OutletType roomType = OutletType.Online, int floors = 1, SortedDictionary<Department, string> departments = null, List<Promotion> promotions = null)
        {
            var outlet = new RetailOutlet { Id = id, OutletType = roomType, Floors = floors };
            outlet.Number = (floors * 100) + outlet.Number;
            outlet.Departments = departments;
            if (promotions != null) outlet.Promotions = promotions;
            return outlet;
        }

        public static SortedDictionary<Department, string> CreateStandardDepartments()
        {
            SortedDictionary<Department, string> departments = new SortedDictionary<Department, string>();
            departments.Add(Department.Home, "No Place Like");
            departments.Add(Department.Garden, "Green Place");
            return departments;
        }

        public static SortedDictionary<Department, string> CreateExtendedDepartments()
        {
            SortedDictionary<Department, string> departments = CreateStandardDepartments();
            departments.Add(Department.Electrical, "Buzz");
            departments.Add(Department.Travel, "Jet way");
            return departments;
        }

        public static SortedDictionary<Department, string> CreateAllDepartments()
        {
            SortedDictionary<Department, string> departments = CreateExtendedDepartments();
            departments.Add(Department.Automotive, "Pit stop");
            return departments;
        }

        public static ICollection<Customer> CustomerCollection()
        {
            ICollection<Customer> customerCollection = new List<Customer>();
            customerCollection.Add(CustomerA());
            customerCollection.Add(CustomerB());
            return customerCollection;
        }

        public static Dictionary<int, Customer> CustomerDictionary()
        {
            Dictionary<int, Customer> customerDictionary = new Dictionary<int, Customer>();
            customerDictionary.Add(1, CustomerA());
            customerDictionary.Add(2, CustomerB());
            return customerDictionary;
        }

        private static List<string> FirstNames = new List<string> { "Danae", "Marquis", "Willis", "Luciana", "Deirdre", "Don", "Neal", "Casie", "Akiko", "Marvel", "Robbie", "Isaiah", "Elizbeth", "Christiane", "Steve", "Lonnie", "Desmond", "Vallie", "Nicol", "Celeste" };
        private static List<string> LastNames = new List<string> { "Mccomas", "Mcneil", "Weldy", "Singleterry", "Hambly", "Yearwood", "Penley", "Kennard", "Way", "Hollifield", "Snedden", "Luick", "Warrington", "Neri", "Seidman", "Calvo", "Mitchum", "Finks", "Dyson", "Proehl" };
        private static List<string> AddressLine1s = new List<string> { "11-17 S St Mary's Gate", "Unnamed Road", "Pen Dref", "58-72 Basingstoke Rd", "205 Kelynmead Rd", "3 Wilby Cl", "49 Bell Ln", "7 Oxford Mews", "1 Greenbank", "21 James St S", "4 New Houses", "4 Alma Rd", "33 W Bars", "4 Lode Way", "283 Manford Way", "6 Hill Park Brae", "Hayes Ln", "39 East Rd", "4 Grange Cliffe Cl", "11 Westcroft", "135 Acacia Rd" };
        private static List<string> PostCodes = new List<string> { "DN31 1JE", "BT93 6FU", "LL46 2SG", "RG2 0EL", "B33 8LJ", "BL8 1XU", "WF7 7JJ", "BN3 3NF", "PL11 3HH", "BF3 BT2", "PH42 4RL", "KA27 8BA", "S40 1AG", "CH61 PE16", "IG7 4AA", "IV8 8PL", "RH13 0SL", "M12 5QY", "S11 9JE", "YO43 4TX", "SO19 7JT" };
        private static List<string> Products = new List<string> { "Algebraic Gyro-stabilized Astrocomputer", "Atomic Piano", "Chocolate Volt Pencil", "Computerized Proton Electrounderwear", "Dimensional", "Pinball", "Electrical Cell Pinball", "Emergency Electromagnetic Retrostinker", "Fabulous Elevator", "Fusion-powered Chemical Whineranoid", "Geometric Cryospazzer", "Iso 900-compliant Gastrointestinal Fluroadapter", "Internet-enabled Cog Rapper", "Internet-enabled Gauss Kilodoodler", "Magnetic Sneezerator", "Military-grade Ultrasonic", "Barfer-o-mat", "Nuclear Motor Moleculotripper", "Nuclear-powered Antimatter Mechaspazzer", "Programmable Vodka Melterorg", "Refittable Nailfile", "Ultrasonic Home Economics Biotargeter" };
        private static List<string> SupplierNames = new List<string> { "The Obtuse Rabbit Company", "Peaceful Skunk", "The Beta Meerkat Company", " Stormy Knife Company", "Hot Lime Company", "The Red Squirrel Company", "Intelligent Duck", "Cloudy Cat", "Complicated Cow", "The Cheeky Panda Company" };
        private static List<string> AddressNames = new List<string> { "Office", "Depot", "Warehouse", "Head Office", "Research Centre", "Regional Office" };
    }


    public static class Extensions
    {
        public static T GetSeededEntry<T>(this List<T> list, int seed)
        {
            int rem = 0;
            if (list.Count > seed) return list[seed];
            Math.DivRem(list.Count, seed, out rem);
            return list[rem];
        }
    }

}
