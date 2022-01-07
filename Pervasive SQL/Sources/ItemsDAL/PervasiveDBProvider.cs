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
                            Patur,
                            SITESYNC,
                            KUPASYNC,                            
                            TITLE,                            
                            UitemDate
                        )                        
                        (
                            SELECT 
                            '{SafeConvert.ToPervasiveString(item.Id)}',  
                            '{SafeConvert.ToPervasiveString(item.ItemNameHE)}',
                            '{SafeConvert.ToPervasiveString(item.ItemNameEN)}',                            
                            '{item.Barcode}',
                            '{SafeConvert.ToPervasiveString(item.Characteristic1)}',
                            '{SafeConvert.ToPervasiveString(item.Characteristic2)}',                            
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
                            {(item.VatFlag ? 1 : 0)},
                            {(item.SyncToSite ? 1 : 0)},  
                            {(item.SyncToFunds ? 1 : 0)}, 
                            '{(item.IsBlocked ? SafeConvert.ToPervasiveString("כ") : "")}',
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}
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
                            ItemName = '{SafeConvert.ToPervasiveString(item.ItemNameHE)}',
                            Ename = '{SafeConvert.ToPervasiveString(item.ItemNameEN)}',
                            Barcode = '{item.Barcode}',
                            ifun1 = '{SafeConvert.ToPervasiveString(item.Characteristic1)}',
                            ifun2 = '{SafeConvert.ToPervasiveString(item.Characteristic2)}',                            
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
                            Patur = {(item.VatFlag ? 1 : 0)},
                            SITESYNC = {(item.SyncToSite ? 1 : 0)},
                            KUPASYNC = {(item.SyncToFunds ? 1 : 0)},
                            TITLE = '{(item.IsBlocked ? SafeConvert.ToPervasiveString("כ") : "")}',
                            UitemDate = {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}
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

        public int GetStock(string id, int storeId) {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT SUM(QTY) FROM invtrs WHERE itemno='{id}'";
                    if (storeId > 0) query += $@" and store={storeId}";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    var result = command.ExecuteScalar();
                    if (result.ToString() == "") result = "0";
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"GetStock {ex.Message}");
            }
        }

        public int GetStockInOrders(string id)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        SELECT  SUM(ItemQtyBDY - QtySupBDY)
                        FROM    Order00BDY o1, Order00HDR o2 
                        WHERE   o1.Doccode=o2.Doccode 
                        AND     DocStatusHDR=0 and ItemNoBDY='{id}'
                    ";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    var result = command.ExecuteScalar();
                    if (result.ToString() == "") result = "0";
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("GetStockInOrders");
            }
        }

        public IEnumerable<Item> Find(ItemSearchParams searchParams) {
            var hasSiteSync = this.IsColumnExists($"Items", "SITESYNC");
            var hasKupaSync = this.IsColumnExists($"Items", "KUPASYNC");
            var hasUpdDate = this.IsColumnExists($"Items", "UitemDate");

            try
            {
                var items = new List<Item>();

                // ignore tables search with updated-date filter but no updated-date column
                // to prevent receiving incorrect data
                var hasUpdDateFilter = searchParams.FromUpdateDate.HasValue || searchParams.ToUpdateDate.HasValue;
                if (searchParams.PauseIfNoUpdateDateColumn && !hasUpdDate && hasUpdDateFilter) 
                    return items;

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        SELECT  * 
                        FROM    Items
                        WHERE   ('{searchParams.ItemId ?? ""}' = '' OR ItemNo = '{searchParams.ItemId}')       
                        AND     ('{searchParams.Barcode ?? ""}' = '' OR Barcode = '{searchParams.Barcode}')                                                                           
                        AND     ({searchParams.SortCode} = 0 OR SortCode1 = {searchParams.SortCode} OR SortCode2 = {searchParams.SortCode} OR SortCode3 = {searchParams.SortCode})                                                   
                    ";
                    /// AND     (totqty >= {searchParams.QtyInStock})       

                    if (searchParams.IsBlocked.HasValue) query += $"AND(TITLE {(searchParams.IsBlocked.Value ? "=" : "<>")} '{ SafeConvert.ToPervasiveString("כ")}') ";
                    if (hasSiteSync) query += $"AND SITESYNC = {(searchParams.SyncToSite ? 1 : 0)} ";
                    if (hasKupaSync) query += $"AND KUPASYNC = {(searchParams.SyncToFunds ? 1 : 0)} ";
                    if (hasUpdDate) {
                        query += $@"
                        AND({ PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)} <= 0 OR UitemDate >= { PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)})
                        AND({ PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)} <= 0 OR UitemDate <= { PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)}) 
                    ";
                    }
                    query += "ORDER BY ItemNo DESC";                    

                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    /// throw new Exception(query);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                Id = SafeConvert.ToString(reader["ItemNo"]),
                                ItemNameHE = SafeConvert.ToString(reader["ItemName"]),
                                ItemNameEN = SafeConvert.ToString(reader["Ename"]),
                                Barcode = SafeConvert.ToString(reader["Barcode"]),
                                Characteristic1 = SafeConvert.ToString(reader["ifun1"]),
                                Characteristic2 = SafeConvert.ToString(reader["ifun2"]),
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
                                Unit = SafeConvert.ToString(reader["Unit"]),
                                ///QtyInStock = SafeConvert.ToSingle(reader["totqty"]),
                                VatFlag = SafeConvert.ToBoolean(reader["Patur"]),
                                SyncToSite = SafeConvert.ToBoolean(reader.ReadOrDefault<bool>("SITESYNC")),
                                SyncToFunds = SafeConvert.ToBoolean(reader.ReadOrDefault<bool>("KUPASYNC")),
                                IsBlocked = SafeConvert.ToBoolean(reader["TITLE"]),                                     
                                UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("UitemDate"))),
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

        public IEnumerable<ItemBarcode> GetBarcodes(string Id) {
            try
            {
                var barcodes = new List<ItemBarcode>();
                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        SELECT  * 
                        FROM    BARCODES
                        WHERE   ItemNo = '{Id}'                                            
                    ";
                    
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())                    
                        while (reader.Read())                        
                            barcodes.Add(new ItemBarcode {
                                ItemId = SafeConvert.ToString(reader["ItemNo"]),
                                Barcode = SafeConvert.ToString(reader["Barcode"]),
                                Notes = SafeConvert.ToString(reader["Remarks"])
                            }); 

                    return barcodes;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<ItemBarcode> GetBarcodes()
        {
            try
            {
                var barcodes = new List<ItemBarcode>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        SELECT  * 
                        FROM    BARCODES
                    ";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            barcodes.Add(new ItemBarcode
                            {
                                ItemId = SafeConvert.ToString(reader["ItemNo"]),
                                Barcode = SafeConvert.ToString(reader["Barcode"]),
                                Notes = SafeConvert.ToString(reader["Remarks"])
                            });

                    return barcodes;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // ---

        private bool IsColumnExists(string tableName, string columnName)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@" SELECT  1
                                    FROM    X$File,X$Field
                                    WHERE   Xf$Id=Xe$File
                                    AND     Xf$Name = '{tableName}'
                                    AND     Xe$Name = '{columnName}'";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    return Convert.ToInt32(command.ExecuteScalar()) == 1;
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
