using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaybu.Compare.Data;
using Zaybu.Compare.Interfaces;
using Zaybu.Compare.Types;

namespace Zaybu.Compare.UI.WPF.ViewModels
{
    public class ResultsViewModel : ViewModelBase
    {
        private ZCompareResults _results;
        private IResult _selectedItem;
        private string _selectedItemSummary;

        public ZCompareResults Results
        {
            get { return _results; }
            set
            {
                _results = value;
                RaisePropertyChanged("Results");
            }
        }

        public IResult SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_selectedItem != null) SelectedItemSummary = _selectedItem.GetSummary();
                RaisePropertyChanged("SelectedItem");
            }
        }

        public string SelectedItemSummary
        {
            get
            {
                if (_selectedItem == null) return String.Empty;
                else return _selectedItemSummary;
            }
            set
            {
                _selectedItemSummary = value;
                RaisePropertyChanged("SelectedItemSummary");
            }
        }

        #region Commands

        public RelayCommand Compare
        {
            get
            {
                return new RelayCommand(() =>
                {

                    // Tutorial Step 1 & Step 2 code
                    Product productA = SampleData.CreateProduct(1);
                    Product productB = SampleData.CreateProduct(1);

                    productB.Description = "Product B";
                    productB.ImageData[1] = 34;

                    ZCompare.SetPropertyDescription(typeof(Product), "Description", typeof(string), "Description Of Product");

                    var results = ZCompare.Compare(productA, productB);

                    Results = results;


                    /*
                    // Tutorial Step 3 code
                    Supplier supplierA = SampleData.CreateSupplier(1);
                    Supplier supplierB = SampleData.CreateSupplier(2);

                    var results = ZCompare.Compare(supplierA, supplierB);

                    var productResults = results.GetResult(supplierA.Products);

                    var productResult = results.GetResult(supplierA.Products[1]);

                    var productDescriptionResult = results.GetResult(supplierA.Products[0], "Description");

                    string productSummary = productResult.GetSummary();

                    Results = results;
                    */

                    /*
                    // Tutorial - Step 6 code

                    Supplier supplierA = SampleData.CreateSupplier(5);
                    Supplier supplierB = SampleData.CreateSupplier(6);

                    ZCompare.RegisterComparitor(new SupplierCustomComparitor());
                    ZCompare.RegisterComparitor(new DateTimeComparitor());

                    var results = ZCompare.Compare(supplierA, supplierB);

                    var createdResult = results.GetResult(supplierA, "Created");

                    Results = results;
                    */

                    /*
                    // Example  code
                    List<Supplier> listA = SampleData.CreateSuppliers(4);
                    List<Supplier> listB = new List<Supplier>();

                    // listB has 1 item the same as listA, 1 modified and 1 deletion           
                    listB.Add(listA[0]); // First item in both lists are identical
                    listB.Add(SampleData.CreateSupplier(1)); // This item has the same id (1), but will be different
                    listB.Add(SampleData.CreateSupplier(9)); // This item is added, id = 9
                    // Items 3 & 4 in listA are not in listB (deleted)

                    var results = ZCompare.Compare(listA, listB);

                    Results = results;
                    */


                    //#region MICRO ORM TUTORIAL CODE

                    //List<Supplier> originalSuppliers = SampleData.CreateSuppliers(6);
                    //List<Supplier> updatedSuppliers = SampleData.CreateSuppliers(6);
                    //updatedSuppliers[0].Name = "New Supplier Name";
                    //updatedSuppliers[2].Addresses["Head Office"].PostCode.Inner = "PC99"; // Modify an address PostCode for a supplier
                    //updatedSuppliers[2].Products.RemoveAt(0); // Delete a product from this supplier
                    //updatedSuppliers[2].Products.Add(SampleData.CreateProduct(20)); // Add a product to this supplier
                    //updatedSuppliers[2].Products[2].ImageData = new byte[2] { 34, 36 }; // Change a property of a product
                    //updatedSuppliers[3].Name = "New Supplier Name";
                    //updatedSuppliers[3].Addresses.Add("Northern HQ", SampleData.CreateAddressList(3)[0]); // Add an Address
                    //updatedSuppliers[3].Products.Add(SampleData.CreateProduct(21)); // Add a product to this supplier
                    //updatedSuppliers[3].Products[1].Code = new ProductCode { Category = 'K', ProductID = 923 };
                    //updatedSuppliers.RemoveAt(4); // Delete a Supplier
                    //updatedSuppliers.Add(SampleData.CreateSupplier(7)); // Add a Supplier

                    //var results = ZCompare.Compare(originalSuppliers, updatedSuppliers);
                    //var supplierResults = results.GetResults<Supplier>(originalSuppliers, true);

                    //// Top level changes to suppliers
                    //supplierResults.ForEach(s =>
                    //{
                    //    if (s.Status == ResultStatus.Added)
                    //    {
                    //        ORM.Insert(s.ChangedToValue);
                    //    }
                    //    else if (s.Status == ResultStatus.Deleted)
                    //    {
                    //        ORM.Delete<Supplier>(s.OriginalValue.ID);
                    //    }
                    //    else if (s.Status == ResultStatus.Changed)
                    //    {
                    //        ORM.Update(s.ChangedToValue);
                    //    }
                    //});


                    //// Adding a Supplier and all it's child objects
                    //supplierResults.ForEach(s =>
                    //{
                    //    if (s.Status == ResultStatus.Added)
                    //    {
                    //        ORM.Insert(s.ChangedToValue);

                    //        s.ChangedToValue.Products.ForEach(p =>
                    //        {
                    //            // Insert all the new Products
                    //            ORM.Insert(p);
                    //        });

                    //        s.ChangedToValue.Addresses.ToList().ForEach(a =>
                    //        {
                    //            // Insert all the new Addresses
                    //            ORM.Insert(a);
                    //        });
                    //    }
                    //});

                    //// Modifying suppliers and all it's child objects
                    //supplierResults.ForEach(s =>
                    //{
                    //    if (s.Status == ResultStatus.Changed)
                    //    {
                    //        if (s.ChangesType.HasFlag(ChangesType.ValueTypes)) ORM.Update(s.ChangedToValue);

                    //        var productResults = results.GetResults<Product>(s.OriginalValue.Products, true);
                    //        productResults.ForEach(p =>
                    //        {
                    //            // Insert, Update or Delete Products                                
                    //        });

                    //        var addressResults = results.GetResults<Address>(s.OriginalValue.Addresses, true);
                    //        addressResults.ForEach(a =>
                    //        {
                    //            // Insert, Update or Delete Addresses                                
                    //        });
                    //    }
                    //});

                    //// Complete structure
                    //supplierResults.ForEach(s =>
                    //{
                    //    if (s.Status == ResultStatus.Added)
                    //    {
                    //        ORM.Insert(s.ChangedToValue);

                    //        s.ChangedToValue.Products.ForEach(p =>
                    //        {
                    //            // Insert all the new Products
                    //            ORM.Insert(p);
                    //        });

                    //        s.ChangedToValue.Addresses.ToList().ForEach(a =>
                    //        {
                    //            // Insert all the new Addresses
                    //            ORM.Insert(a);
                    //        });
                    //    }
                    //    else if (s.Status == ResultStatus.Deleted)
                    //    {
                    //        s.OriginalValue.Products.ForEach(p =>
                    //        {
                    //            // Delete all Products
                    //            ORM.Delete<Product>(p.ID);
                    //        });

                    //        s.OriginalValue.Addresses.ToList().ForEach(a =>
                    //        {
                    //            // Delete all Addresses
                    //            ORM.Delete<Address>(a.Value.ID);
                    //        });

                    //        // Delete the Supplier entry
                    //        ORM.Delete<Supplier>(s.ID);
                    //    }
                    //    else if (s.Status == ResultStatus.Changed)
                    //    {
                    //        if (s.ChangesType.HasFlag(ChangesType.ValueTypes)) ORM.Update(s.ChangedToValue);

                    //        var productResults = results.GetResults<Product>(s.OriginalValue.Products, true);
                    //        productResults.ForEach(p =>
                    //        {
                    //            // Insert, Update or Delete Products                                
                    //        });

                    //        var addressResults = results.GetResults<Address>(s.OriginalValue.Addresses, true);
                    //        addressResults.ForEach(a =>
                    //        {
                    //            // Insert, Update or Delete Addresses                                
                    //        });
                    //    }
                    //});

                    //Results = results;

                    //#endregion
                    ShowSummary.Execute(null);

                });
            }
        }

        public RelayCommand ShowSummary
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedItem = Results.Root;
                });
            }
        }

        #endregion
    }

    public static class ORM {
        public static int Insert<T>(T insertObject)
        {
            Debug.WriteLine($"Insert : {insertObject.ToString()}");
            return 999; 
        }

        public static void Update<T>(T updateObject)
        {
            Debug.WriteLine($"Update : {updateObject.ToString()}");
        }

        public static void Delete<T>(int id)
        {
            Debug.WriteLine($"Delete : {id} : {typeof(T).ToString()}");
        }
    }

}
