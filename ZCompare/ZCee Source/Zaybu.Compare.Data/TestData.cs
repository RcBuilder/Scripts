using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaybu.Compare.Data
{
    public class TestData
    {
        #region DataSet

        public static DataSet DataSetA
        {
            get
            {
                var ds = new DataSet("DataSet A");
                ds.CaseSensitive = true;
                ds.Tables.Add(DataTableA);
                return ds;
            }
        }

        public static DataSet DataSetB
        {
            get
            {
                var ds = new DataSet("DataSet B");
                ds.CaseSensitive = false;
                ds.Tables.Add(DataTableB);
                return ds;
            }
        }

        public static DataTable DataTableA
        {
            get
            {
                var dt = new DataTable("DataTable A");
                dt.Namespace = "namespace://siteA.com";
                dt.Columns.Add("Column 1A");

                for (int count = 0; count < 2; count++)
                {
                    dt.Rows.Add(new object[] { 'x' });
                }
                return dt;
            }
        }

        public static DataTable DataTableB
        {
            get
            {
                var dt = new DataTable("DataTable B");
                dt.Namespace = "namespace://siteB.com";
                dt.Columns.Add("Column 1B");

                for (int count = 0; count < 2; count++)
                {
                    dt.Rows.Add(new object[] { 'y' });
                }

                return dt;
            }
        }

        #endregion

        #region Arrays

        public static char[] CharArrayA
        {
            get { return new char[5] { 'a', 'b', 'c', 'd', 'e' }; }
        }

        public static char[] CharArrayB
        {
            get { return new char[5] { 'a', 'b', 'c', 'd', 'x' }; }
        }

        public static char[] CharArrayC
        {
            get { return new char[6] { 'a', 'b', 'c', 'd', 'e', 'f' }; }
        }

        public static char?[] CharNullableArrayA
        {
            get { return new char?[5] { 'a', 'b', 'c', 'd', 'e' }; }
        }

        public static char?[] CharNullableArrayB
        {
            get { return new char?[5] { null, 'b', 'c', 'd', 'x' }; }
        }

        public static char?[] CharNullableArrayC
        {
            get { return new char?[6] { 'a', 'b', 'c', 'd', 'e', null }; }
        }

        public static byte[] ByteArrayA
        {
            get { return new byte[5] { 1, 2, 3, 4, 5 }; }
        }

        public static byte[] ByteArrayB
        {
            get { return new byte[5] { 1, 2, 3, 4, 9 }; }
        }

        public static byte[] ByteArrayC
        {
            get { return new byte[6] { 1, 2, 3, 4, 5, 6 }; }
        }

        public static byte?[] ByteNullableArrayA
        {
            get { return new byte?[5] { 1, 2, 3, 4, 5 }; }
        }

        public static byte?[] ByteNullableArrayB
        {
            get { return new byte?[5] { null, 2, 3, 4, 9 }; }
        }

        public static byte?[] ByteNullableArrayC
        {
            get { return new byte?[6] { 1, 2, 3, 4, 5, null }; }
        }

        #endregion

        #region Dictionaries

        public static Dictionary<int, string> DictionaryA
        {
            get
            {
                var dct = new Dictionary<int, string>();
                dct.Add(1, "A");
                dct.Add(2, "B");
                dct.Add(3, "C");
                dct.Add(4, "D");
                dct.Add(5, "E");
                return dct;
            }
        }

        public static Dictionary<int, string> DictionaryB
        {
            get
            {
                var dct = new Dictionary<int, string>();
                dct.Add(1, "A");
                dct.Add(2, "B");
                dct.Add(3, "X");
                dct.Add(4, "D");
                dct.Add(5, "Y");
                return dct;
            }
        }

        public static Dictionary<int, string> DictionaryC
        {
            get
            {
                var dct = new Dictionary<int, string>();
                dct.Add(1, "A");
                dct.Add(2, "B");
                dct.Add(3, "X");
                dct.Add(4, "D");
                dct.Add(9, "Y");
                return dct;
            }
        }

        public static Dictionary<int, string> DictionaryD
        {
            get
            {
                var dct = new Dictionary<int, string>();
                dct.Add(1, "A");
                dct.Add(2, "B");
                dct.Add(3, "X");
                dct.Add(4, "D");
                dct.Add(9, "Y");
                dct.Add(10, "Z");
                return dct;
            }
        }


        public static SortedDictionary<int, string> SortedDictionaryA
        {
            get
            {
                var dct = new SortedDictionary<int, string>();
                dct.Add(1, "A");
                dct.Add(2, "B");
                dct.Add(3, "C");
                dct.Add(4, "D");
                dct.Add(5, "E");
                return dct;
            }
        }

        public static SortedDictionary<int, string> SortedDictionaryB
        {
            get
            {
                var dct = new SortedDictionary<int, string>();
                dct.Add(3, "X");
                dct.Add(4, "D");
                dct.Add(5, "Y");
                dct.Add(1, null);
                dct.Add(2, "B");
                return dct;
            }
        }

        public static SortedDictionary<int, string> SortedDictionaryC
        {
            get
            {
                var dct = new SortedDictionary<int, string>();
                dct.Add(3, "X");
                dct.Add(4, "D");
                dct.Add(9, "Y");
                dct.Add(1, "A");
                dct.Add(2, "B");
                return dct;
            }
        }

        public static SortedDictionary<int, string> SortedDictionaryD
        {
            get
            {
                var dct = new SortedDictionary<int, string>();
                dct.Add(1, "A");
                dct.Add(2, "B");
                dct.Add(3, "X");
                dct.Add(4, "D");
                dct.Add(9, "Y");
                dct.Add(10, "Z");
                return dct;
            }
        }
        #endregion

        #region Hash

        public static HashSet<string> HashSetA
        {
            get
            {
                HashSet<string> hashSet = new HashSet<string>();
                hashSet.Add("a");
                hashSet.Add("b");
                hashSet.Add("c");
                hashSet.Add("d");
                hashSet.Add("e");
                return hashSet;
            }
        }

        public static HashSet<string> HashSetB
        {
            get
            {
                HashSet<string> hashSet = new HashSet<string>();
                hashSet.Add("x");
                hashSet.Add("b");
                hashSet.Add("c");
                hashSet.Add("d");
                hashSet.Add("y");
                return hashSet;
            }
        }

        public static HashSet<string> HashSetC
        {
            get
            {
                HashSet<string> hashSet = new HashSet<string>();
                hashSet.Add("x");
                hashSet.Add("b");
                hashSet.Add("c");
                hashSet.Add("d");
                hashSet.Add(null);
                return hashSet;
            }
        }

        #endregion

        #region Collections

        public static Collection<decimal> CollectionA
        {
            get
            {
                var collection = new Collection<decimal>();
                collection.Add((decimal)1.1234);
                collection.Add((decimal)2.1234);
                collection.Add((decimal)3.1234);
                collection.Add((decimal)4.1234);
                collection.Add((decimal)5.1234);
                return collection;
            }
        }

        public static Collection<decimal> CollectionB
        {
            get
            {
                var collection = new Collection<decimal>();
                collection.Add((decimal)1.12340);
                collection.Add((decimal)2.123400);
                collection.Add((decimal)3.1234000);
                collection.Add((decimal)4.123);
                collection.Add((decimal)5.12);
                return collection;
            }
        }

        public static Collection<decimal> CollectionC
        {
            get
            {
                var collection = new Collection<decimal>();
                collection.Add((decimal)1.12340);
                collection.Add((decimal)2.123400);
                collection.Add((decimal)3.1234000);
                collection.Add((decimal)4.123);
                collection.Add((decimal)5.12);
                collection.Add((decimal)5.12);
                return collection;
            }
        }

        public static NameValueCollection NameValueCollectionA
        {
            get
            {
                NameValueCollection nv = new NameValueCollection();
                nv.Add("nvNameA1", "nvValueA1");
                nv.Add("nvNameA2", "nvValueA2");
                nv.Add("nvNameA3", "nvValueA3");
                return nv;
            }
        }

        public static NameValueCollection NameValueCollectionB
        {
            get
            {
                NameValueCollection nv = new NameValueCollection();
                nv.Add("nvNameB1", "nvValueB1");
                nv.Add("nvNameB2", "nvValueB2");
                nv.Add("nvNameB3", "nvValueB3");
                return nv;
            }
        }

        public static NameValueCollection NameValueCollectionC
        {
            get
            {
                NameValueCollection nv = new NameValueCollection();
                nv.Add("nvNameB1", "nvValueB1");
                nv.Add("nvNameB2", "nvValueB2");
                nv.Add("nvNameB3c", "nvValueB3c");
                nv.Add("nvNameB4", "nvValueB4");
                return nv;
            }
        }

        #endregion

        #region DateTime

        public static DateTime DateTimeA
        {
            get { return new DateTime(2016, 1, 10, 13, 33, 28); }
        }

        public static DateTime DateTimeB
        {
            get { return new DateTime(2016, 1, 15, 6, 49, 5); }
        }

        public static DateTime? DateTimeNullableA
        {
            get { return new DateTime?(new DateTime(2016, 1, 10, 13, 33, 28)); }
        }

        public static DateTime? DateTimeNullableB
        {
            get { return new DateTime?(new DateTime(2016, 1, 15, 6, 49, 5)); }
        }

        public static TimeSpan TimeSpanA
        {
            get { return new TimeSpan(1, 10, 13, 33, 28); }
        }

        public static TimeSpan TimeSpanB
        {
            get { return new TimeSpan(1, 15, 6, 49, 5); }
        }

        public static TimeSpan? TimeSpanNullableA
        {
            get { return new TimeSpan?(new TimeSpan(1, 10, 13, 33, 28)); }
        }

        public static TimeSpan? TimeSpanNullableB
        {
            get { return new TimeSpan?(new TimeSpan(1, 15, 6, 49, 5)); }
        }

        #endregion

        #region Custom

        #region Definitions

        public class Customer
        {
            public int ID { get; set; }
            public Dictionary<string, Address> Addresses { get; set; }
            public object BusinessGuide { get; set; }

            public Customer()
            {
                Addresses = new Dictionary<string, Address>();
            }
        }

        public class Supplier
        {
            public int ID { get; set; }
            public List<Customer> Customers { get; set; }

            public Supplier()
            {
                Customers = new List<Customer>();
            }
        }

        public class Address
        {
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public PostCode PostCode { get; set; }
        }

        public class PostCode
        {
            public string Outer { get; set; }
            public string Inner { get; set; }
        }

        #endregion

        public static Supplier SupplierA
        {
            get
            {
                Supplier supplier = new Supplier();
                supplier.Customers.Add(CustomerA);
                supplier.ID = 1;
                return supplier;
            }
        }

        public static Supplier SupplierB
        {
            get
            {
                Supplier supplier = new Supplier();
                supplier.Customers.Add(CustomerB);
                supplier.ID = 2;
                return supplier;
            }
        }

        public static Customer CustomerA
        {
            get
            {
                Customer customer = new Customer();
                customer.Addresses.Add("Home", AddressA);
                customer.ID = 1;
                return customer;
            }
        }

        public static Customer CustomerB
        {
            get
            {
                Customer customer = new Customer();
                customer.Addresses.Add("Work", AddressB);
                customer.ID = 2;
                return customer;
            }
        }

        public static Address AddressA
        {
            get
            {
                return new Address { AddressLine1 = "A Street", AddressLine2 = "A Place", PostCode = PostCodeA };
            }
        }

        public static Address AddressB
        {
            get
            {
                return new Address { AddressLine1 = "B Street", AddressLine2 = "B Place", PostCode = PostCodeB };
            }
        }

        public static PostCode PostCodeA
        {
            get
            {
                return new PostCode { Outer = "A00", Inner = "A11" };
            }
        }

        public static PostCode PostCodeB
        {
            get
            {
                return new PostCode { Outer = "B00", Inner = "B11" };
            }
        }

        #endregion

        #region Interfaces

        public static TestInterfaceParent TestInterfaceParentA
        {
            get
            {
                return new TestInterfaceParent { Children = new List<ITestInterface> { new TestInterfaceObjectA(), new TestInterfaceObjectA() } };
            }
        }

        public static TestInterfaceParent TestInterfaceParentB
        {
            get
            {
                return new TestInterfaceParent { Children = new List<ITestInterface> { new TestInterfaceObjectB(), new TestInterfaceObjectB() } };
            }
        }

        public static TestInterfaceParent TestInterfaceParentMixed
        {
            get
            {
                return new TestInterfaceParent { Children = new List<ITestInterface> { new TestInterfaceObjectA(), new TestInterfaceObjectB() } };
            }
        }

        #endregion

        #region Enums and Structs

        public static ZCompareOptions EnumAndStructObjectA
        {
            get { return new ZCompareOptions { ID = 1, Option = ZCompareOption.DifferencesOnly, Result = new ZCompareStruct { ID = 1, Value = 1.1111f } }; }
        }

        public static ZCompareOptions EnumAndStructObjectB
        {
            get { return new ZCompareOptions { ID = 2, Option = ZCompareOption.Simple, Result = new ZCompareStruct { ID = 2, Value = 2.22f } }; }
        }

        #endregion
    }
}
