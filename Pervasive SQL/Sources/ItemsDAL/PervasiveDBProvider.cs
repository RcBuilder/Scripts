using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using Entities;
using Helpers;
using static Helpers.PervasiveDBHelper;

namespace ItemsDAL
{
    public class PervasiveDBProvider : IItemsDAL
    {
        protected string ConnetionString { get; set; }
        public PervasiveDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }

        public string CreateItem(Item item)
        {
            try
            {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        INSERT INTO Items
                        (
                            ItemNo,                            
                            ItemName,
                            Ename,
                            Barcode,
                            ifun1,
                            ifun2,                            
                            FC1,
                            FC2,
                            FC3,
                            FC4,
                            FC5,
                            FC6,
                            FC7,
                            FC8,
                            FC9,
                            FC10,
                            Prc1,
                            Prc2,
                            Prc3,
                            Prc4,
                            Prc5,
                            Prc6,
                            Prc7,
                            Prc8,
                            Prc9,
                            Prc10,                            
                            SortCode1,
                            SortCode2,
                            SortCode3,
                            Supl1,
                            Supl2,
                            Supl3,
                            Supl4,
                            Unit,
                            Patur
                        )                        
                        (
                            SELECT 
                            '{PervasiveDBHelper.FixHebrewWithNumbers(item.Id)}',  
                            '{PervasiveDBHelper.FixHebrewWithNumbers(item.ItemNameHE)}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(item.ItemNameEN)}',                            
                            '{item.Barcode}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(item.Characteristic1)}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(item.Characteristic2)}',                            
                            {item.CurrencyCode1},
                            {item.CurrencyCode2},
                            {item.CurrencyCode3},
                            {item.CurrencyCode4},
                            {item.CurrencyCode5},
                            {item.CurrencyCode6},
                            {item.CurrencyCode7},
                            {item.CurrencyCode8},
                            {item.CurrencyCode9},
                            {item.CurrencyCode10},
                            {item.Price1},
                            {item.Price2},
                            {item.Price3},
                            {item.Price4},
                            {item.Price5},
                            {item.Price6},
                            {item.Price7},
                            {item.Price8},
                            {item.Price9},
                            {item.Price10},                            
                            {item.SortCode1},
                            {item.SortCode2},
                            {item.SortCode3},
                            {item.SupplerId1},
                            {item.SupplerId2},
                            {item.SupplerId3},
                            {item.SupplerId4},
                            '{item.Unit}',
                            '{item.VatFlag}'                            
                        )";
              
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    command.ExecuteNonQuery();                    
                }

                return item.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public string UpdateItem(Item item)
        {
            try
            {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        UPDATE Items
                        SET                            
                            ItemName = '{PervasiveDBHelper.FixHebrewWithNumbers(item.ItemNameHE)}',
                            Ename = '{PervasiveDBHelper.FixHebrewWithNumbers(item.ItemNameEN)}',
                            Barcode = '{item.Barcode}',
                            ifun1 = '{PervasiveDBHelper.FixHebrewWithNumbers(item.Characteristic1)}',
                            ifun2 = '{PervasiveDBHelper.FixHebrewWithNumbers(item.Characteristic2)}',                            
                            FC1 = {item.CurrencyCode1},
                            FC2 = {item.CurrencyCode2},
                            FC3 = {item.CurrencyCode3},
                            FC4 = {item.CurrencyCode4},
                            FC5 = {item.CurrencyCode5},
                            FC6 = {item.CurrencyCode6},
                            FC7 = {item.CurrencyCode7},
                            FC8 = {item.CurrencyCode8},
                            FC9 = {item.CurrencyCode9},
                            FC10 = {item.CurrencyCode10},
                            Prc1 = {item.Price1},
                            Prc2 = {item.Price2},
                            Prc3 = {item.Price3},
                            Prc4 = {item.Price4},
                            Prc5 = {item.Price5},
                            Prc6 = {item.Price6},
                            Prc7 = {item.Price7},
                            Prc8 = {item.Price8},
                            Prc9 = {item.Price9},
                            Prc10 = {item.Price10},            
                            SortCode1 = {item.SortCode1},
                            SortCode2 = {item.SortCode2},
                            SortCode3 = {item.SortCode3},
                            Supl1 = {item.SupplerId1},
                            Supl2 = {item.SupplerId2},
                            Supl3 = {item.SupplerId3},
                            Supl4 = {item.SupplerId4},
                            Unit = '{item.Unit}',
                            Patur = '{item.VatFlag}'
                        WHERE ItemNo = '{item.Id}'
                    ";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }

                return item.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public int GetStock(int id, int storeId) {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT SUM(QTY) FROM invtrs WHERE itemno={id}";
                    if (storeId > 0) query += $@" and store={storeId}";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<Item> Find(ItemSearchParams searchParams) {
            try
            {
                var items = new List<Item>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        SELECT  * 
                        FROM    Items
                        WHERE   ('{searchParams.ItemId ?? ""}' = '' OR ItemNo = '{searchParams.ItemId}')       
                        AND     ('{searchParams.Barcode ?? ""}' = '' OR Barcode = '{searchParams.Barcode}')                               
                    ";
                    /// AND     (totqty >= {searchParams.QtyInStock})       

                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            /*                                                 	
                                -- PervasiveDBHelper.FixHebrewWithNumbers:
                                Id
                                ItemNameHE
                                ItemNameEN
                                Characteristic1
                                Characteristic2
                            */
                            items.Add(new Item
                            {
                                Id = reader["ItemNo"].ToString().Trim(),
                                ItemNameHE = reader["ItemName"].ToString().Trim(),
                                ItemNameEN = reader["Ename"].ToString().Trim(),
                                Barcode = reader["Barcode"].ToString().Trim(),
                                Characteristic1 = reader["ifun1"].ToString().Trim(),
                                Characteristic2 = reader["ifun2"].ToString().Trim(),
                                CurrencyCode1 = SafeConvert.ToInt32(reader["FC1"]),
                                CurrencyCode2 = SafeConvert.ToInt32(reader["FC2"]),
                                CurrencyCode3 = SafeConvert.ToInt32(reader["FC3"]),
                                CurrencyCode4 = SafeConvert.ToInt32(reader["FC4"]),
                                CurrencyCode5 = SafeConvert.ToInt32(reader["FC5"]),
                                CurrencyCode6 = SafeConvert.ToInt32(reader["FC6"]),
                                CurrencyCode7 = SafeConvert.ToInt32(reader["FC7"]),
                                CurrencyCode8 = SafeConvert.ToInt32(reader["FC8"]),
                                CurrencyCode9 = SafeConvert.ToInt32(reader["FC9"]),
                                CurrencyCode10 = SafeConvert.ToInt32(reader["FC10"]),
                                Price1 = SafeConvert.ToSingle(reader["Prc1"]),
                                Price2 = SafeConvert.ToSingle(reader["Prc2"]),
                                Price3 = SafeConvert.ToSingle(reader["Prc3"]),
                                Price4 = SafeConvert.ToSingle(reader["Prc4"]),
                                Price5 = SafeConvert.ToSingle(reader["Prc5"]),
                                Price6 = SafeConvert.ToSingle(reader["Prc6"]),
                                Price7 = SafeConvert.ToSingle(reader["Prc7"]),
                                Price8 = SafeConvert.ToSingle(reader["Prc8"]),
                                Price9 = SafeConvert.ToSingle(reader["Prc9"]),
                                Price10 = SafeConvert.ToSingle(reader["Prc10"]),
                                SortCode1 = SafeConvert.ToInt32(reader["SortCode1"]),
                                SortCode2 = SafeConvert.ToInt32(reader["SortCode2"]),
                                SortCode3 = SafeConvert.ToInt32(reader["SortCode3"]),
                                SupplerId1 = SafeConvert.ToInt32(reader["Supl1"]),
                                SupplerId2 = SafeConvert.ToInt32(reader["Supl2"]),
                                SupplerId3 = SafeConvert.ToInt32(reader["Supl3"]),
                                SupplerId4 = SafeConvert.ToInt32(reader["Supl4"]),
                                Unit = reader["Unit"].ToString().Trim(),
                                ///QtyInStock = SafeConvert.ToSingle(reader["totqty"]),
                                VatFlag = reader["Patur"].ToString().Trim(),                                
                                ProviderId = 0                                
                            });
                        }
                    }

                    return items;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
