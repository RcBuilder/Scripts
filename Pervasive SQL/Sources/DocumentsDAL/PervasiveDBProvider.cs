using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;

namespace DocumentsDAL
{
    public class PervasiveDBProvider : IDocumentsDAL
    {
        protected string ConnetionString { get; set; }
        public PervasiveDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }

        public bool CreateOrder(Order order) 
        {
            // local methods 
            string CreateHeaderQuery(OrderHeader orderHeader)
            {
                var query = $@"
                        INSERT INTO orderMashdr
                        (
                            DOCCODE,
                            DocNoHDR,                             
                            AccNoHDR,		    
                            AccNameHDR, 		
                            AccAddrHDR, 		
                            AccCityHDR, 		
                            Edit205HDR, 		
                            AccPhone1HDR, 		
                            Phone2HDR, 
                            Phone3HDR, 
                            Phone4HDR, 
                            TrsDebitSubjectHDR,
                            AgentHDR,		    
                            AgNameHDR,		
                            AccRemarksHDR, 
                            StoreHDR,		    
                            Asmac2HDR,		
                            ValuDateHDR,		
                            HDateHDR,		    
                            DocStatusHDR,		
                            StatHDR,		    
                            SubTotalHDR,		
                            ReductionHDR,	
                            ComReducHDR,
                            BeforVatHDR,		
                            VatSumHDR,		
                            GrandTotalHDR,	                            
                            PaidHDR,
                            MAAMPHDR,
                            RoundReducHDR,
                            DATEHDR,		    
                            LDupdHDR,
                            LTupdHDR
                        )                        
                        (
                            SELECT 
                            {orderHeader.DocumentId}, 
                            {orderHeader.DocNo},
                            {orderHeader.AccountId},
                            '{PervasiveDBHelper.FixHebrewWithNumbers(orderHeader.AccountName)}',
                            '{orderHeader.Address}',
                            '{orderHeader.City}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(orderHeader.Contact)}',
                            '{orderHeader.Phone1}',
                            '{orderHeader.Phone2}',
                            '{orderHeader.Phone3}',
                            '{orderHeader.Phone4}',
                            {orderHeader.SubjectId},
                            {orderHeader.AgentId},
                            '{PervasiveDBHelper.FixHebrewWithNumbers(orderHeader.AgentName)}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(orderHeader.Notes)}',
                            {orderHeader.StoreNo},
                            '{orderHeader.Asmac}',
                            {PervasiveDBHelper.ToPervasiveDate(orderHeader.ProvisionDate)},
                            {PervasiveDBHelper.ToPervasiveDate(orderHeader.ReturnDate)},
                            {orderHeader.SystemStatus}, 
                            '{orderHeader.Status}', 
                            {orderHeader.SubTotal}, 
                            {orderHeader.Discount}, 
                            {orderHeader.DiscountPercentage}, 
                            {orderHeader.TotalBeforeVat}, 
                            {orderHeader.Vat}, 
                            {orderHeader.Total}, 
                            {orderHeader.Paid}, 
                            {orderHeader.VATRate},
                            {orderHeader.Round},
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}, 
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)},
                            '{DateTime.Now.ToString("HH:mm")}'
                        )
                    ";

                return query;
            }
            string CreateBodyQuery(int documentId, int LineNo, OrderBody orderBody)
            {
                var query = $@"
                    INSERT INTO orderMasbdy
                    (
                        DOCCODE,  		
                        GridLineNo,		
                        BarcodeBDY,
                        ChinesCodeBDY,
                        ItemNoBDY,		
                        ItemNameBDY,                            
                        ColorBDY,		    
                        MifratBDY,		
                        LDate1BDY,		
                        LENBDY,		    
                        ItemWidthBDY,		
                        ItemThincknessBDY,
                        WBDY,			    
                        ArizotSUGBDY,		
                        ARIZOTBDY,		
                        ARIZOTQTYBDY,		
                        ItemQtyBDY,		
                        ItemPrcBDY,		
                        ItemPrc1BDY,		
                        ItemReducBDY,		
                        ItemSumBDY		 
                    )
                    (
                        SELECT 
                        {documentId},                             
                        {LineNo},
                        '{orderBody.Barcode}',
                        '{orderBody.Auxcode}',
                        '{PervasiveDBHelper.FixHebrewWithNumbers(orderBody.ItemId)}',
                        '{PervasiveDBHelper.FixHebrewWithNumbers(orderBody.ItemName)}',                            
                        '{orderBody.Color}',
                        '{PervasiveDBHelper.FixHebrewWithNumbers(orderBody.Notes)}',
                        {PervasiveDBHelper.ToPervasiveDate(orderBody.ProvisionDate)},
                        {orderBody.Length},
                        {orderBody.Width},
                        {orderBody.Thickness},
                        {orderBody.Weight},
                        '{orderBody.PackType}',
                        '{orderBody.PacksInLot}',
                        '{orderBody.UnitsInPack}',
                        {orderBody.Quantity},
                        {orderBody.UnitPriceBeforeVAT},
                        {orderBody.UnitPrice},
                        {orderBody.DiscountPercentage},
                        {orderBody.SalePriceBeforeVAT}
                    )
                ";

                return query;
            }

            // ---

            OdbcTransaction transaction = null;

            try
            {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    command.CommandText = CreateHeaderQuery(order.Header);
                    command.ExecuteNonQuery();

                    var lineNo = 1;
                    order.Body.ForEach(orderBody => {
                        command.CommandText = CreateBodyQuery(order.Header.DocumentId, lineNo++, orderBody);
                        command.ExecuteNonQuery();
                    });

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public bool CreateInvoice(Invoice invoice)
        {
            // local methods 
            string CreateHeaderQuery(InvoiceHeader invoiceHeader) {
                var query = $@"
                        INSERT INTO HeshbonitMasHdr
                        (
                            DOCCODE,
                            DocNoHDR,                             
                            AccNoHDR,		    
                            AccNameHDR, 		
                            AccAddrHDR, 		
                            AccCityHDR, 		
                            Edit205HDR, 		
                            AccPhone1HDR, 		
                            Phone2HDR, 
                            Phone3HDR, 
                            Phone4HDR, 
                            TrsDebitSubjectHDR,
                            AgentHDR,		    
                            AgNameHDR,		
                            AccRemarksHDR, 
                            StoreHDR,		    
                            Asmac2HDR,		
                            ValuDateHDR,		
                            HDateHDR,		    
                            DocStatusHDR,		
                            StatHDR,		    
                            SubTotalHDR,		
                            ReductionHDR,		
                            ComReducHDR,
                            BeforVatHDR,		
                            VatSumHDR,		
                            GrandTotalHDR,	                            
                            PaidHDR,
                            MAAMPHDR,
                            RoundReducHDR,
                            DATEHDR,		    
                            LDupdHDR,
                            LTupdHDR
                        )                        
                        (
                            SELECT 
                            {invoiceHeader.DocumentId}, 
                            {invoiceHeader.DocNo},
                            {invoiceHeader.AccountId},
                            '{PervasiveDBHelper.FixHebrewWithNumbers(invoiceHeader.AccountName)}',
                            '{invoiceHeader.Address}',
                            '{invoiceHeader.City}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(invoiceHeader.Contact)}',
                            '{invoiceHeader.Phone1}',
                            '{invoiceHeader.Phone2}',
                            '{invoiceHeader.Phone3}',
                            '{invoiceHeader.Phone4}',
                            {invoiceHeader.SubjectId},
                            {invoiceHeader.AgentId},
                            '{PervasiveDBHelper.FixHebrewWithNumbers(invoiceHeader.AgentName)}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(invoiceHeader.Notes)}',
                            {invoiceHeader.StoreNo},
                            '{invoiceHeader.Asmac}',
                            {PervasiveDBHelper.ToPervasiveDate(invoiceHeader.ProvisionDate)},
                            {PervasiveDBHelper.ToPervasiveDate(invoiceHeader.ReturnDate)},
                            {invoiceHeader.SystemStatus}, 
                            '{invoiceHeader.Status}', 
                            {invoiceHeader.SubTotal}, 
                            {invoiceHeader.Discount}, 
                            {invoiceHeader.DiscountPercentage}, 
                            {invoiceHeader.TotalBeforeVat}, 
                            {invoiceHeader.Vat}, 
                            {invoiceHeader.Total}, 
                            {invoiceHeader.Paid}, 
                            {invoiceHeader.VATRate},
                            {invoiceHeader.Round},
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}, 
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)},
                            '{DateTime.Now.ToString("HH:mm")}'
                        )
                    ";

                return query;
            }
            string CreateBodyQuery(int documentId, int LineNo, InvoiceBody invoiceBody) {
                var query = $@"
                        INSERT INTO HeshbonitMasBdy
                        (
                            DOCCODE,  		
                            GridLineNo,		
                            BarcodeBDY,
                            ChinesCodeBDY,
                            ItemNoBDY,		
                            ItemNameBDY,                            
                            ColorBDY,		    
                            MifratBDY,		
                            LDate1BDY,		
                            LENBDY,		    
                            ItemWidthBDY,		
                            ItemThincknessBDY,
                            WBDY,			    
                            ArizotSUGBDY,		
                            ARIZOTBDY,		
                            ARIZOTQTYBDY,		
                            ItemQtyBDY,		
                            ItemPrcBDY,		
                            ItemPrc1BDY,		
                            ItemReducBDY,		
                            ItemSumBDY		 
                        )
                        (
                            SELECT 
                            {documentId},                             
                            {LineNo},
                            '{invoiceBody.Barcode}',
                            '{invoiceBody.Auxcode}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(invoiceBody.ItemId)}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(invoiceBody.ItemName)}',                            
                            '{invoiceBody.Color}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(invoiceBody.Notes)}',
                            {PervasiveDBHelper.ToPervasiveDate(invoiceBody.ProvisionDate)},
                            {invoiceBody.Length},
                            {invoiceBody.Width},
                            {invoiceBody.Thickness},
                            {invoiceBody.Weight},
                            '{invoiceBody.PackType}',
                            '{invoiceBody.PacksInLot}',
                            '{invoiceBody.UnitsInPack}',
                            {invoiceBody.Quantity},
                            {invoiceBody.UnitPriceBeforeVAT},
                            {invoiceBody.UnitPrice},
                            {invoiceBody.DiscountPercentage},
                            {invoiceBody.SalePriceBeforeVAT}
                        )
                    ";

                return query;
            }

            // ---

            OdbcTransaction transaction = null;

            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    command.CommandText = CreateHeaderQuery(invoice.Header);
                    command.ExecuteNonQuery();

                    var lineNo = 1;
                    invoice.Body.ForEach(invoiceBody => {
                        command.CommandText = CreateBodyQuery(invoice.Header.DocumentId, lineNo++, invoiceBody);
                        command.ExecuteNonQuery();
                    });
                    
                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public bool CreateDeliveryNote(DeliveryNote deliveryNote)
        {
            // local methods 
            string CreateHeaderQuery(DeliveryNoteHeader deliveryNoteHeader)
            {
                var query = $@"
                    INSERT INTO TMMasHdr
                    (
                        DOCCODE,
                        DocNoHDR,                             
                        AccNoHDR,		    
                        AccNameHDR, 		
                        AccAddrHDR, 		
                        AccCityHDR, 		
                        Edit205HDR, 		
                        AccPhone1HDR, 		
                        Phone2HDR, 
                        Phone3HDR, 
                        Phone4HDR, 
                        TrsDebitSubjectHDR,
                        AgentHDR,		    
                        AgNameHDR,		
                        AccRemarksHDR, 
                        StoreHDR,		    
                        Asmac2HDR,		
                        ValuDateHDR,		
                        HDateHDR,		    
                        DocStatusHDR,		
                        StatHDR,		    
                        SubTotalHDR,		
                        ReductionHDR,	
                        ComReducHDR,
                        BeforVatHDR,		
                        VatSumHDR,		
                        GrandTotalHDR,	                            
                        PaidHDR,
                        MAAMPHDR,
                        RoundReducHDR,
                        DATEHDR,		    
                        LDupdHDR,
                        LTupdHDR
                    )                        
                    (
                        SELECT 
                        {deliveryNoteHeader.DocumentId}, 
                        {deliveryNoteHeader.DocNo},
                        {deliveryNoteHeader.AccountId},
                        '{PervasiveDBHelper.FixHebrewWithNumbers(deliveryNoteHeader.AccountName)}',
                        '{deliveryNoteHeader.Address}',
                        '{deliveryNoteHeader.City}',
                        '{PervasiveDBHelper.FixHebrewWithNumbers(deliveryNoteHeader.Contact)}',
                        '{deliveryNoteHeader.Phone1}',
                        '{deliveryNoteHeader.Phone2}',
                        '{deliveryNoteHeader.Phone3}',
                        '{deliveryNoteHeader.Phone4}',
                        {deliveryNoteHeader.SubjectId},
                        {deliveryNoteHeader.AgentId},
                        '{PervasiveDBHelper.FixHebrewWithNumbers(deliveryNoteHeader.AgentName)}',
                        '{PervasiveDBHelper.FixHebrewWithNumbers(deliveryNoteHeader.Notes)}',
                        {deliveryNoteHeader.StoreNo},
                        '{deliveryNoteHeader.Asmac}',
                        {PervasiveDBHelper.ToPervasiveDate(deliveryNoteHeader.ProvisionDate)},
                        {PervasiveDBHelper.ToPervasiveDate(deliveryNoteHeader.ReturnDate)},
                        {deliveryNoteHeader.SystemStatus}, 
                        '{deliveryNoteHeader.Status}', 
                        {deliveryNoteHeader.SubTotal}, 
                        {deliveryNoteHeader.Discount}, 
                        {deliveryNoteHeader.DiscountPercentage}, 
                        {deliveryNoteHeader.TotalBeforeVat}, 
                        {deliveryNoteHeader.Vat}, 
                        {deliveryNoteHeader.Total}, 
                        {deliveryNoteHeader.Paid}, 
                        {deliveryNoteHeader.VATRate},
                        {deliveryNoteHeader.Round},
                        {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}, 
                        {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)},
                        '{DateTime.Now.ToString("HH:mm")}'
                    )
                ";

                return query;
            }
            string CreateBodyQuery(int documentId, int LineNo, DeliveryNoteBody deliveryNoteBody)
            {
                var query = $@"
                        INSERT INTO TMMasBdy
                        (
                            DOCCODE,  		
                            GridLineNo,		
                            BarcodeBDY,
                            ChinesCodeBDY,
                            ItemNoBDY,		
                            ItemNameBDY,                            
                            ColorBDY,		    
                            MifratBDY,		
                            LDate1BDY,		
                            LENBDY,		    
                            ItemWidthBDY,		
                            ItemThincknessBDY,
                            WBDY,			    
                            ArizotSUGBDY,		
                            ARIZOTBDY,		
                            ARIZOTQTYBDY,		
                            ItemQtyBDY,		
                            ItemPrcBDY,		
                            ItemPrc1BDY,		
                            ItemReducBDY,		
                            ItemSumBDY                            
                        )
                        (
                            SELECT 
                            {documentId},                             
                            {LineNo},
                            '{deliveryNoteBody.Barcode}',
                            '{deliveryNoteBody.Auxcode}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(deliveryNoteBody.ItemId)}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(deliveryNoteBody.ItemName)}',                            
                            '{deliveryNoteBody.Color}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(deliveryNoteBody.Notes)}',
                            {PervasiveDBHelper.ToPervasiveDate(deliveryNoteBody.ProvisionDate)},
                            {deliveryNoteBody.Length},
                            {deliveryNoteBody.Width},
                            {deliveryNoteBody.Thickness},
                            {deliveryNoteBody.Weight},
                            '{deliveryNoteBody.PackType}',
                            '{deliveryNoteBody.PacksInLot}',
                            '{deliveryNoteBody.UnitsInPack}',
                            {deliveryNoteBody.Quantity},
                            {deliveryNoteBody.UnitPriceBeforeVAT},
                            {deliveryNoteBody.UnitPrice},
                            {deliveryNoteBody.DiscountPercentage},
                            {deliveryNoteBody.SalePriceBeforeVAT}
                        )
                    ";

                return query;
            }

            // ---

            OdbcTransaction transaction = null;

            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    command.CommandText = CreateHeaderQuery(deliveryNote.Header);
                    command.ExecuteNonQuery();

                    var lineNo = 1;
                    deliveryNote.Body.ForEach(deliveryNoteBody => {
                        command.CommandText = CreateBodyQuery(deliveryNote.Header.DocumentId, lineNo++, deliveryNoteBody);
                        command.ExecuteNonQuery();
                    });

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> Search
        public IEnumerable<Order> FindOrders(OrderSearchParams searchParams)
        {
            string CreateHeaderQuery() {
                return $@"
                    SELECT  * 
                    FROM    orderMashdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    ORDER BY DATEHDR DESC
                ";
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    orderMasbdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            OrderHeader ReadHeader(OdbcDataReader reader) {
                /*                                                                                      
                    -- PervasiveDBHelper.FixHebrewWithNumbers:
                    AccountName
                    Contact
                    AgentName
                    Notes

                    -- PervasiveDBHelper.ToPervasiveDate:
                    ProvisionDate
                    ReturnDate
                    CreatedDate
                    UpdatedDate                    
                */

                return new OrderHeader {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                    DocNo = Convert.ToInt32(reader["DocNoHDR"]),
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = reader["AccNameHDR"].ToString().Trim(),
                    Address = reader["AccAddrHDR"].ToString().Trim(),
                    City = reader["AccCityHDR"].ToString().Trim(),
                    Contact = reader["Edit205HDR"].ToString().Trim(),
                    Phone1 = reader["AccPhone1HDR"].ToString().Trim(),
                    Phone2 = reader["Phone2HDR"].ToString().Trim(),
                    Phone3 = reader["Phone3HDR"].ToString().Trim(),
                    Phone4 = reader["Phone4HDR"].ToString().Trim(),
                    SubjectId = Convert.ToInt32(reader["TrsDebitSubjectHDR"]),
                    AgentId = Convert.ToInt32(reader["AgentHDR"]),
                    AgentName = reader["AgNameHDR"].ToString().Trim(),
                    Notes = reader["AccRemarksHDR"].ToString().Trim(),
                    StoreNo = Convert.ToInt32(reader["StoreHDR"]),
                    Asmac = reader["Asmac2HDR"].ToString().Trim(),
                    ProvisionDate = null, // TODO ValuDateHDR                   
                    ReturnDate = null, // TODO HDateHDR
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    Status = reader["StatHDR"].ToString().Trim(),
                    SubTotal = Convert.ToSingle(reader["SubTotalHDR"]),
                    Discount = Convert.ToSingle(reader["ReductionHDR"]),
                    DiscountPercentage = Convert.ToSingle(reader["ComReducHDR"]),
                    TotalBeforeVat = Convert.ToSingle(reader["BeforVatHDR"]),
                    Vat = Convert.ToSingle(reader["VatSumHDR"]),
                    Total = Convert.ToSingle(reader["GrandTotalHDR"]),
                    Paid = Convert.ToSingle(reader["PaidHDR"]),
                    VATRate = Convert.ToSingle(reader["MAAMPHDR"]),
                    Round = Convert.ToSingle(reader["RoundReducHDR"]),
                    CreatedDate = null, // TODO DATEHDR       
                    UpdatedDate = null // TODO LDupdHDR       
                };
            }

            OrderBody ReadBody(OdbcDataReader reader)
            {
                /*                                                 	
                    -- PervasiveDBHelper.FixHebrewWithNumbers:
                    ItemId
                    ItemName
                    Notes

                    -- PervasiveDBHelper.ToPervasiveDate:
                    ProvisionDate
                */
                return new OrderBody {                    
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    Barcode = reader["BarcodeBDY"].ToString().Trim(),
                    Auxcode = reader["ChinesCodeBDY"].ToString().Trim(),
                    ItemId = reader["ItemNoBDY"].ToString().Trim(),
                    ItemName = reader["ItemNameBDY"].ToString().Trim(),
                    Color = reader["ColorBDY"].ToString().Trim(),
                    Notes = reader["MifratBDY"].ToString().Trim(),
                    ProvisionDate = null, // TODO LDate1BDY
                    Length = Convert.ToSingle(reader["LENBDY"]),
                    Width = Convert.ToSingle(reader["ItemWidthBDY"]),
                    Thickness = Convert.ToSingle(reader["ItemThincknessBDY"]),
                    Weight = Convert.ToSingle(reader["WBDY"]),
                    PackType = reader["ArizotSUGBDY"].ToString().Trim(),
                    PacksInLot = reader["ARIZOTBDY"].ToString().Trim(),
                    UnitsInPack = reader["ARIZOTQTYBDY"].ToString().Trim(),
                    Quantity = Convert.ToSingle(reader["ItemQtyBDY"]),
                    UnitPriceBeforeVAT = Convert.ToSingle(reader["ItemPrcBDY"]),
                    UnitPrice = Convert.ToSingle(reader["ItemPrc1BDY"]),
                    DiscountPercentage = Convert.ToSingle(reader["ItemReducBDY"]),
                    SalePriceBeforeVAT = Convert.ToSingle(reader["ItemSumBDY"]),
                };
            }

            try
            {
                var orders = new List<Order>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;                    

                    command.CommandText = CreateHeaderQuery();

                    // load orders (header)
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            orders.Add(new Order { 
                                Header = ReadHeader(reader)
                            });
                    
                    orders.ForEach(order => {
                        command.CommandText = CreateBodyQuery(order.Header.DocumentId);

                        // load order (items)
                        using (var reader = command.ExecuteReader())
                            while (reader.Read())
                                order.Body.Add(ReadBody(reader));
                    });                    
                }

                return orders;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> Search
        public IEnumerable<Invoice> FindInvoices(InvoiceSearchParams searchParams)
        {
            return null;
        }

        // TODO ->> Search
        public IEnumerable<DeliveryNote> FindDeliveryNotes(DeliveryNoteSearchParams searchParams)
        {
            return null;
        }

        public float GetVATRate() {
            try {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT TOP 1 VatPercent FROM compdet WHERE recno=1";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    return Convert.ToSingle(command.ExecuteScalar()) / 100;  // e.g: 0.17
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public bool UsePriceRound() {
            try {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT TOP 1 ""round"" FROM compdet WHERE recno=1";
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

        public bool AccountExists(int accountId) {
            try {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT 1 FROM Accounts WHERE AccNo={accountId}";
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
    }
}
