using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;

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
                        '{PervasiveDBHelper.FixHebrewWithNumbers(inventoryItem.ItemId)}', 
                        {inventoryItem.CommandNo},
                        {inventoryItem.StoreNo}, 
                        '{inventoryItem.SerialNo}', 
                        {inventoryItem.AccountId}, 
                        {inventoryItem.AgentId}, 
                        {inventoryItem.SubjectNo}, 
                        {inventoryItem.AuxQty}, 
                        '{inventoryItem.BatchNo}',                         
                        '{PervasiveDBHelper.FixHebrewWithNumbers(inventoryItem.Details)}', 
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
    }
}
