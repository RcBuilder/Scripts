using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using static Helpers.PervasiveDBHelper;

namespace InventoryDAL
{
    public class PervasiveDBProvider : IInventoryDAL
    {
        protected string ConnetionString { get; set; }
        public PervasiveDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }
        
        public BulkInsertStatus CreateInventory(Inventory inventory)
        {
            string CreateInventoryItemQuery(long transactionId, InventoryItem inventoryItem)
            {
                var query = $@"
                    INSERT INTO INVTRS
                    (
                        TrsNo,
                        ItemNo, 
                        Pkuda,
                        Store, 
                        SerNo, 
                        AccNo, 
                        Agent, 
                        Subject, 
                        AuxQty, 
                        BatchNo,                         
                        Details, 
                        DocCode, 
                        ExpireDate, 
                        FcSum, 
                        InOut,                         
                        Qty,
                        Color,                         
                        Size,                                                 
                        ""Sum"", 
                        Voucher,                        
                        date                        
                    )                        
                    (
                        SELECT   
                        {transactionId},
                        '{SafeConvert.ToPervasiveString(inventoryItem.ItemId)}', 
                        {inventoryItem.CommandNo},
                        {inventoryItem.StoreNo}, 
                        '{inventoryItem.SerialNo}', 
                        {inventoryItem.AccountId}, 
                        {inventoryItem.AgentId}, 
                        {inventoryItem.SubjectNo}, 
                        {inventoryItem.AuxQty}, 
                        '{inventoryItem.BatchNo}',                         
                        '{SafeConvert.ToPervasiveString(inventoryItem.Details)}', 
                        '{inventoryItem.DocumentCode}', 
                        {PervasiveDBHelper.ToPervasiveDate(inventoryItem.ExpireDate)}, 
                        {inventoryItem.FcTotal}, 
                        '{inventoryItem.InOut}',                         
                        {inventoryItem.Quantity},
                        '{inventoryItem.Color}',                         
                        '{inventoryItem.Size}',                                                 
                        {inventoryItem.Total}, 
                        {inventoryItem.Voucher},                        
                        {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}                        
                    )
                ";

                return query;
            }

            OdbcTransaction transaction = null;
            var result = new BulkInsertStatus();

            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;
                    
                    inventory.Items.ForEach(inventoryItem => {
                        var transactionId = PervasiveDBHelper.GetColumnMaxValue(command, "INVTRS", "TrsNo");
                        command.CommandText = CreateInventoryItemQuery(transactionId, inventoryItem);
                        command.ExecuteNonQuery();

                        result.NumOfSuccesses++;
                    });

                    transaction.Commit();
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                result.NumOfFailures++;
                throw;
            }
        }
        
        public InventoryStatus GetStockStatus()
        {
            try
            {
                var inventoryStatus = new InventoryStatus();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT SUM(QTY), itemno, store FROM invtrs GROUP BY itemno, store";                    
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inventoryStatus.Items.Add(new InventoryStatusItem
                            {
                                Quantity = Convert.ToSingle(reader[0]),
                                ItemId = reader[1].ToString().Trim(),
                                StoreNo = Convert.ToInt32(reader[2])
                            });
                        }
                    }

                    return inventoryStatus;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public InventoryStatus GetStockStatus(string itemId)
        {
            try
            {
                var inventoryStatus = new InventoryStatus();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                            SELECT  SUM(QTY), itemno, store 
                            FROM    invtrs 
                            WHERE   itemno = '{itemId}'
                            GROUP BY itemno, store";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inventoryStatus.Items.Add(new InventoryStatusItem
                            {
                                Quantity = Convert.ToSingle(reader[0]),
                                ItemId = reader[1].ToString().Trim(),
                                StoreNo = Convert.ToInt32(reader[2])
                            });
                        }
                    }

                    return inventoryStatus;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> duplicate with DocumentsDAL
        public bool StoreItemExists(StoreItem storeItem)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT 1 FROM storebdy WHERE StoreNo={storeItem.StoreNo} AND Itemno = '{storeItem.ItemId}'";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    return Convert.ToInt32(command.ExecuteScalar()) == 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // update stock
        public bool StoreItemUpdate(StoreItem storeItem)
        {            
            try
            {
                OdbcTransaction transaction = null;

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();
                    
                    command.Connection = connection;
                    command.Transaction = transaction;

                    var result = this.StoreItemUpdate(storeItem, command);

                    transaction.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public bool CreateInventoryTransaction(InventoryTransaction transaction)
        {
            int identity = (1000 * this.GetNextDocCounter(eCounterNo.InventoryTransaction));
            transaction.CommandNo = ++identity;

            var transactionId = this.GetNextTransactionId(eTransactionProvider.InvTrs);
            transaction.TransactionId = transactionId++;

            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();

                var command = new OdbcCommand();
                command.Connection = connection;

                var result = this.CreateInventoryTransaction(transaction, command);
                this.SetNextDocCounter(eCounterNo.InventoryTransaction, command);

                return result;
            }
        }

        public IEnumerable<Store> GetStores()
        {
            try
            {
                var stores = new List<Store>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT * FROM storeHDR";                    
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stores.Add(new Store
                            {
                                No = SafeConvert.ToInt32(reader["No"]),
                                Name = SafeConvert.ToString(reader["Name"])
                            });
                        }
                    }

                    return stores;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> duplicate with DocumentsDAL
        public int GetNextDocCounter(eCounterNo counterNo)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT Value FROM DocsCounters WHERE counterno={(int)counterNo}";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    var value = Convert.ToInt32(command.ExecuteScalar());
                    if (value == 0) throw new Exception("No Counter Found!");

                    return value;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> duplicate with DocumentsDAL
        public bool SetNextDocCounter(eCounterNo counterNo)
        {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();

                var command = new OdbcCommand();
                command.Connection = connection;

                return this.SetNextDocCounter(counterNo, command);
            }
        }

        // TODO ->> duplicate with DocumentsDAL
        public int GetNextTransactionId(eTransactionProvider provider = eTransactionProvider.AccTrs)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $"SELECT TrsNo FROM {provider.ToString()} ORDER BY TrsNo DESC";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    return Convert.ToInt32(command.ExecuteScalar()) + 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // -----

        // TODO ->> duplicate with DocumentsDAL
        private bool SetNextDocCounter(eCounterNo counterNo, OdbcCommand command)
        {
            try
            {
                var query = $@"
                    UPDATE DocsCounters 
                    SET Value = (Value + 1) 
                    WHERE counterno={(int)counterNo}
                ";

                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // update stock
        private bool StoreItemUpdate(StoreItem storeItem, OdbcCommand command)
        {
            try
            {
                var query1 = $@"
                    INSERT INTO storebdy (StoreNo, Itemno) 
                    (SELECT {storeItem.StoreNo}, '{storeItem.ItemId}')
                ";

                var query2 = $@"                    
                    UPDATE  storebdy 
                    SET     totqty = (SELECT SUM(qty) FROM invtrs WHERE itemno='{storeItem.ItemId}' and store={storeItem.StoreNo}) 
                    WHERE   itemno='{storeItem.ItemId}' and storeno={storeItem.StoreNo}
                ";

                var rowsAffected = 0;
                if (!this.StoreItemExists(storeItem))
                {
                    command.CommandText = query1;
                    var result1 = command.ExecuteNonQuery();
                    rowsAffected += result1;
                }

                command.CommandText = query2;
                var result2 = command.ExecuteNonQuery();
                rowsAffected += result2;

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> duplicate with DocumentsDAL
        // תנועות מלאי        
        private bool CreateInventoryTransaction(InventoryTransaction transaction, OdbcCommand command)
        {
            try
            {
                var query = $@"
                        INSERT INTO InvTrs
                        (
                            Store,		
                            ItemNo,		
                            InOut,		
                            DocCode,		
                            Details,		
                            Subject,		
                            Agent,	    
                            Voucher,     
                            AccNo,      
                            Qty,         
                            ""Sum"",
                            FcSum,		
                            AuxQty,      
                            SerNo,       
                            Color,		
                            Size,		
                            BatchNo,  
                            Pkuda,
                            TrsNo,
                            ExpireDate,  
                            DATE                                    
                        )                        
                        (
                            SELECT 
                            {transaction.StoreNo},
                            '{transaction.ItemId}',
                            '{transaction.InOut}',
                            '{transaction.DocumentId}',
                            '{SafeConvert.ToPervasiveString(transaction.Notes)}',
                            {transaction.SubjectId},
                            {transaction.AgentId},
                            '{transaction.Asmac}',
                            {transaction.AccountId},
                            {transaction.Quantity},
                            {transaction.Total}, 
                            {transaction.FcTotal}, 
                            {transaction.AuxQuantity},
                            '{transaction.SerialNo}',
                            '{transaction.Color}',
                            '{transaction.Size}',
                            '{transaction.BatchNo}',                            
                            {transaction.CommandNo}, 
                            {transaction.TransactionId}, 
                            {PervasiveDBHelper.ToPervasiveDate(transaction.ExpireDate)},
                            {PervasiveDBHelper.ToPervasiveDate(transaction.CreatedDate)}                                                                        
                        )
                    ";

                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
