using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Odbc;
using System.Diagnostics;
using static Helpers.PervasiveDBHelper;

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
                        INSERT INTO {orderHeader.TableName}hdr
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
                            {orderHeader.DocumentId},
                            {orderHeader.AccountId},
                            '{SafeConvert.ToPervasiveString(orderHeader.AccountName)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.Address)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.City)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.Contact)}',
                            '{orderHeader.Phone1}',
                            '{orderHeader.Phone2}',
                            '{orderHeader.Phone3}',
                            '{orderHeader.Phone4}',
                            {orderHeader.SubjectId},
                            {orderHeader.AgentId},
                            '{SafeConvert.ToPervasiveString(orderHeader.AgentName)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.Notes)}',
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
                    INSERT INTO {order.Header.TableName}bdy
                    (
                        DOCCODE,  		
                        GridLineNo,		                                                
                        ItemNoBDY,		
                        ItemNameBDY,                            
                        ColorBDY,		    
                        MifratBDY,		
                        LDate1BDY,		
                        LENBDY,		    
                        ItemWidthBDY,		
                        ItemThincknessBDY,
                        WBDY,			                            	
                        ARIZOTBDY,		
                        ARIZOTQTYBDY,		
                        ItemQtyBDY,		
                        ItemPrcBDY,		
                        ItemPrc1BDY,		
                        ItemReducBDY,		
                        ItemSumBDY,
                        NewQtyBDY
                    )
                    (
                        SELECT 
                        {documentId},                             
                        {LineNo},                                                
                        '{SafeConvert.ToPervasiveString(orderBody.ItemId)}',
                        '{SafeConvert.ToPervasiveString(orderBody.ItemName)}',                            
                        '{orderBody.Color}',
                        '{SafeConvert.ToPervasiveString(orderBody.Notes)}',
                        {PervasiveDBHelper.ToPervasiveDate(orderBody.ProvisionDate)},
                        {orderBody.Length},
                        {orderBody.Width},
                        {orderBody.Thickness},
                        {orderBody.Weight},                        
                        '{orderBody.PacksInLot}',
                        '{orderBody.UnitsInPack}',
                        {orderBody.Quantity},
                        {orderBody.UnitPriceBeforeVAT},
                        {orderBody.UnitPrice},
                        {orderBody.DiscountPercentage},
                        {orderBody.SalePriceBeforeVAT},
                        {orderBody.Collected}
                    )
                ";

                return query;
            }

            void CreateMISMRow(OdbcCommand command)
            {                
                this.CreateMISMRow(new MismRowData
                {
                    AccountId = order.Header.AccountId,
                    AccountName = order.Header.AccountName,
                    TableName = order.Header.TableName,
                    MismNo = order.Header.DocumentId,
                    MismCode = eDocCode.Order,
                    DocDate = order.Header.ProvisionDate ?? DateTime.Now,
                    Total = order.Header.Total,
                    Vat = order.Header.Vat,
                    ProducerName = "API",
                    DocPrintPattern = $"{order.Header.TableName}Rep",
                    AccountingCommandNo = 0,  // CreateAccountingTransactions > identity (base)
                    InventoryCommandNo = 0    // CreateInventoryTransactions > identity (base)
                }, command);
            }

            // ---

            OdbcTransaction transaction = null;

            var step = 0;
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    step = 1;

                    command.Connection = connection;
                    command.Transaction = transaction;

                    command.CommandText = CreateHeaderQuery(order.Header);
                    command.ExecuteNonQuery();
                    
                    step = 2;

                    var lineNo = 1;
                    order.Body.ForEach(orderBody => {
                        command.CommandText = CreateBodyQuery(order.Header.DocumentId, lineNo++, orderBody);                        
                        command.ExecuteNonQuery();                        
                    });

                    step = 3;

                    CreateMISMRow(command);

                    step = 4;

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message} ({step})");
                throw;
            }
        }

        public bool CreateOrderMas(Order order) 
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
                            {orderHeader.DocumentId},
                            {orderHeader.AccountId},
                            '{SafeConvert.ToPervasiveString(orderHeader.AccountName)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.Address)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.City)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.Contact)}',
                            '{orderHeader.Phone1}',
                            '{orderHeader.Phone2}',
                            '{orderHeader.Phone3}',
                            '{orderHeader.Phone4}',
                            {orderHeader.SubjectId},
                            {orderHeader.AgentId},
                            '{SafeConvert.ToPervasiveString(orderHeader.AgentName)}',
                            '{SafeConvert.ToPervasiveString(orderHeader.Notes)}',
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
                        arizasugBDY,		
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
                        '{SafeConvert.ToPervasiveString(orderBody.ItemId)}',
                        '{SafeConvert.ToPervasiveString(orderBody.ItemName)}',                            
                        '{orderBody.Color}',
                        '{SafeConvert.ToPervasiveString(orderBody.Notes)}',
                        {PervasiveDBHelper.ToPervasiveDate(orderBody.ProvisionDate)},
                        {orderBody.Length},
                        {orderBody.Width},
                        {orderBody.Thickness},
                        {orderBody.Weight},
                        '{SafeConvert.ToPervasiveString(orderBody.PackType)}',
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

            void CreateMISMRow(OdbcCommand command)
            {                
                this.CreateMISMRow(new MismRowData { 
                    AccountId = order.Header.AccountId,
                    AccountName = order.Header.AccountName,
                    TableName = "orderMas",
                    MismNo = order.Header.DocumentId,
                    MismCode = eDocCode.OrderMas, 
                    DocDate = order.Header.ProvisionDate ?? DateTime.Now,
                    Total = order.Header.Total,
                    Vat = order.Header.Vat,
                    ProducerName = "API", 
                    DocPrintPattern = "orderMasRep",
                    AccountingCommandNo = 0,  // CreateAccountingTransactions > identity (base)
                    InventoryCommandNo = 0    // CreateInventoryTransactions > identity (base)
                }, command);
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

                    CreateMISMRow(command);

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

        public bool CreatePriceOffer(PriceOffer priceOffer)
        {
            // local methods 
            string CreateHeaderQuery(PriceOfferHeader priceOfferHeader)
            {
                var query = $@"
                        INSERT INTO ofer00hdr
                        (
                            DOCCODE,
                            DocNoHDR,                             
                            AccNoHDR,		    
                            AccNameHDR, 		
                            AccAddrHDR, 		
                            AccCityHDR, 		                            	
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
                            DocStatusHDR,		
                            StatHDR,		    
                            SubTotalHDR,		
                            ReductionHDR,	
                            ComReducHDR,
                            BeforVatHDR,		
                            VatSumHDR,		
                            GrandTotalHDR,	                                                        
                            MAAMPHDR,
                            RoundReducHDR,
                            DATEHDR,		    
                            LDupdHDR,
                            LTupdHDR
                        )                        
                        (
                            SELECT 
                            {priceOfferHeader.DocumentId}, 
                            {priceOfferHeader.DocumentId},
                            {priceOfferHeader.AccountId},
                            '{SafeConvert.ToPervasiveString(priceOfferHeader.AccountName)}',
                            '{SafeConvert.ToPervasiveString(priceOfferHeader.Address)}',
                            '{SafeConvert.ToPervasiveString(priceOfferHeader.City)}',                            
                            '{priceOfferHeader.Phone1}',
                            '{priceOfferHeader.Phone2}',
                            '{priceOfferHeader.Phone3}',
                            '{priceOfferHeader.Phone4}',
                            {priceOfferHeader.SubjectId},
                            {priceOfferHeader.AgentId},
                            '{SafeConvert.ToPervasiveString(priceOfferHeader.AgentName)}',
                            '{SafeConvert.ToPervasiveString(priceOfferHeader.Notes)}',
                            {priceOfferHeader.StoreNo},
                            '{priceOfferHeader.Asmac}',
                            {PervasiveDBHelper.ToPervasiveDate(priceOfferHeader.ProvisionDate)},                            
                            {priceOfferHeader.SystemStatus}, 
                            '{priceOfferHeader.Status}', 
                            {priceOfferHeader.SubTotal}, 
                            {priceOfferHeader.Discount}, 
                            {priceOfferHeader.DiscountPercentage}, 
                            {priceOfferHeader.TotalBeforeVat}, 
                            {priceOfferHeader.Vat}, 
                            {priceOfferHeader.Total},                             
                            {priceOfferHeader.VATRate},
                            {priceOfferHeader.Round},
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}, 
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)},
                            '{DateTime.Now.ToString("HH:mm")}'
                        )
                    ";

                return query;
            }
            string CreateBodyQuery(int documentId, int LineNo, PriceOfferBody priceOfferBody)
            {
                var query = $@"
                    INSERT INTO ofer00bdy
                    (
                        DOCCODE,  		
                        GridLineNo,		                        
                        ItemNoBDY,		
                        ItemNameBDY,                                                    
                        ItemReducBDY,		
                        ItemSumBDY	 
                    )
                    (
                        SELECT 
                        {documentId},                             
                        {LineNo},                        
                        '{SafeConvert.ToPervasiveString(priceOfferBody.ItemId)}',
                        '{SafeConvert.ToPervasiveString(priceOfferBody.ItemName)}',                                                    
                        {priceOfferBody.DiscountPercentage},
                        {priceOfferBody.SalePriceBeforeVAT}
                    )
                ";

                return query;
            }

            void CreateMISMRow(OdbcCommand command)
            {                
                this.CreateMISMRow(new MismRowData
                {
                    AccountId = priceOffer.Header.AccountId,
                    AccountName = priceOffer.Header.AccountName,
                    TableName = "ofer00",
                    MismNo = priceOffer.Header.DocumentId,
                    MismCode = eDocCode.PriceOffer, 
                    DocDate = priceOffer.Header.ProvisionDate ?? DateTime.Now,
                    Total = priceOffer.Header.Total,
                    Vat = priceOffer.Header.Vat,
                    ProducerName = "API",
                    DocPrintPattern = "ofer00Rep",
                    AccountingCommandNo = 0,  // CreateAccountingTransactions > identity (base)
                    InventoryCommandNo = 0    // CreateInventoryTransactions > identity (base)
                }, command);
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

                    command.CommandText = CreateHeaderQuery(priceOffer.Header);
                    command.ExecuteNonQuery();

                    var lineNo = 1;
                    priceOffer.Body.ForEach(priceOfferBody => {
                        command.CommandText = CreateBodyQuery(priceOffer.Header.DocumentId, lineNo++, priceOfferBody);
                        command.ExecuteNonQuery();
                    });

                    CreateMISMRow(command);

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
            var step = 0;

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
                            {invoiceHeader.DocumentId},
                            {invoiceHeader.AccountId},
                            '{SafeConvert.ToPervasiveString(invoiceHeader.AccountName)}',
                            '{SafeConvert.ToPervasiveString(invoiceHeader.Address)}',
                            '{SafeConvert.ToPervasiveString(invoiceHeader.City)}',
                            '{SafeConvert.ToPervasiveString(invoiceHeader.Contact)}',
                            '{invoiceHeader.Phone1}',
                            '{invoiceHeader.Phone2}',
                            '{invoiceHeader.Phone3}',
                            '{invoiceHeader.Phone4}',
                            {invoiceHeader.SubjectId},
                            {invoiceHeader.AgentId},
                            '{SafeConvert.ToPervasiveString(invoiceHeader.AgentName)}',
                            '{SafeConvert.ToPervasiveString(invoiceHeader.Notes)}',
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
                            arizasugBDY,		
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
                            '{SafeConvert.ToPervasiveString(invoiceBody.ItemId)}',
                            '{SafeConvert.ToPervasiveString(invoiceBody.ItemName)}',                            
                            '{invoiceBody.Color}',
                            '{SafeConvert.ToPervasiveString(invoiceBody.Notes)}',
                            {PervasiveDBHelper.ToPervasiveDate(invoiceBody.ProvisionDate)},
                            {invoiceBody.Length},
                            {invoiceBody.Width},
                            {invoiceBody.Thickness},
                            {invoiceBody.Weight},
                            '{SafeConvert.ToPervasiveString(invoiceBody.PackType)}',
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

            void CreateMISMRow(OdbcCommand command, int inventoryBaseIdentity, int accountingBaseIdentity)
            {                
                this.CreateMISMRow(new MismRowData
                {
                    AccountId = invoice.Header.AccountId,
                    AccountName = invoice.Header.AccountName,
                    TableName = "HeshbonitMas",
                    MismNo = invoice.Header.DocumentId,
                    MismCode = eDocCode.Invoice,
                    DocDate = invoice.Header.ProvisionDate ?? DateTime.Now,
                    Total = invoice.Header.Total,
                    Vat = invoice.Header.Vat,
                    ProducerName = "API",
                    DocPrintPattern = "HeshbonitMasRep",
                    AccountingCommandNo = accountingBaseIdentity,  // CreateAccountingTransactions > identity (base)
                    InventoryCommandNo = inventoryBaseIdentity     // CreateInventoryTransactions > identity (base)
                }, command);
            }

            void CreateInventoryTransactions(OdbcCommand command, out int baseIdentity) {
                int identity = (1000 * this.GetNextDocCounter(eCounterNo.InventoryTransaction));
                baseIdentity = identity;

                var transactionId = this.GetNextTransactionId(eTransactionProvider.InvTrs);

                invoice.Body.ForEach(invoiceBody => {
                    this.CreateInventoryTransaction(new InventoryTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = transactionId++,
                        StoreNo = invoice.Header.StoreNo,
                        ItemId = invoiceBody.ItemId,
                        CreatedDate = invoice.Header.CreatedDate ?? DateTime.Now,
                        InOut = "i",
                        DocumentCode = "H",
                        DocumentId = invoice.Header.DocumentId,
                        Notes = invoice.Header.Notes,
                        SubjectId = invoice.Header.SubjectId,
                        AgentId = invoice.Header.AgentId,
                        Asmac = invoice.Header.Asmac,
                        AccountId = invoice.Header.AccountId,
                        Quantity = -(invoiceBody.Quantity),
                        Total = invoiceBody.Total                        
                    }, command);
                });

                this.SetNextDocCounter(eCounterNo.InventoryTransaction, command);
            }

            void CreateAccountingTransactions(OdbcCommand command, out int baseIdentity)
            {
                int identity = (1000 * this.GetNextDocCounter(eCounterNo.AccountingTransaction));
                baseIdentity = identity;

                var transactionId = this.GetNextTransactionId();

                var opAcc7 = this.GetOpAcc(7);
                var opAcc1 = this.GetOpAcc(1);
                var opAcc1015 = this.GetOpAcc(1015);

                // (1)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = transactionId,
                    AccountId = invoice.Header.AccountId,
                    Asmac1 = invoice.Header.DocumentId.ToString(),
                    Asmac2 = invoice.Header.Asmac,                    
                    PaidDate = invoice.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = invoice.Header.ProvisionDate,
                    Subject = invoice.Header.SubjectId.ToString(),
                    Total = invoice.Header.Total,
                    CreditDebit = "ח",
                    DocumentId = invoice.Header.DocumentId,
                    DocumentCode = "H",
                    Notes = invoice.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = invoice.Header.CreatedDate ?? DateTime.Now,
                }, command);

                // (2)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = ++transactionId,
                    AccountId = opAcc7,
                    Asmac1 = invoice.Header.DocumentId.ToString(),
                    Asmac2 = invoice.Header.Asmac,                    
                    PaidDate = invoice.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = invoice.Header.ProvisionDate,
                    Subject = invoice.Header.SubjectId.ToString(),
                    Total = invoice.Header.TotalBeforeVat,
                    CreditDebit = "ז",
                    DocumentId = invoice.Header.DocumentId,
                    DocumentCode = "H",
                    Notes = invoice.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = invoice.Header.CreatedDate ?? DateTime.Now,
                }, command);

                // (3)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = ++transactionId,
                    AccountId = opAcc1,
                    Asmac1 = invoice.Header.DocumentId.ToString(),
                    Asmac2 = invoice.Header.Asmac,                    
                    PaidDate = invoice.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = invoice.Header.ProvisionDate,
                    Subject = invoice.Header.SubjectId.ToString(),
                    Total = invoice.Header.Vat,
                    CreditDebit = "ז",
                    DocumentId = invoice.Header.DocumentId,
                    DocumentCode = "H",
                    Notes = invoice.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = invoice.Header.CreatedDate ?? DateTime.Now,
                }, command);

                this.SetNextDocCounter(eCounterNo.AccountingTransaction, command);
            }
            // ---

            OdbcTransaction transaction = null;

            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    step = 1;

                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    // create document (header + body)
                    command.CommandText = CreateHeaderQuery(invoice.Header);
                    command.ExecuteNonQuery();

                    step = 2;

                    var lineNo = 1;
                    invoice.Body.ForEach(invoiceBody => {
                        command.CommandText = CreateBodyQuery(invoice.Header.DocumentId, lineNo++, invoiceBody);
                        command.ExecuteNonQuery();
                    });

                    step = 3;

                    // create inventory-transactions
                    int inventoryBaseIdentity = 0;
                    if (!invoice.Header.DisableInventory)                        
                        CreateInventoryTransactions(command, out inventoryBaseIdentity);

                    step = 4;

                    // create accounting-transactions
                    int accountingBaseIdentity;
                    CreateAccountingTransactions(command, out accountingBaseIdentity);

                    step = 5;

                    // update account totals 
                    this.UpdateAccountingTransactionTotals(invoice.Header.AccountId, command);

                    step = 6;

                    // update stock
                    invoice.Body.ForEach(invoiceBody => {
                        this.StoreItemUpdate(new StoreItem(invoice.Header.StoreNo, invoiceBody.ItemId) , command);
                    });

                    step = 7;

                    CreateMISMRow(command, inventoryBaseIdentity, accountingBaseIdentity);

                    step = 8;

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step: {step})");
            }
        }

        public bool CreateDeliveryNote(DeliveryNote deliveryNote)
        {
            var step = 0;

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
                        HazItraHDR,
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
                        {deliveryNoteHeader.DocumentId},
                        {deliveryNoteHeader.AccountId},
                        '{SafeConvert.ToPervasiveString(deliveryNoteHeader.AccountName)}',
                        '{SafeConvert.ToPervasiveString(deliveryNoteHeader.Address)}',
                        '{SafeConvert.ToPervasiveString(deliveryNoteHeader.City)}',
                        '{SafeConvert.ToPervasiveString(deliveryNoteHeader.Contact)}',
                        '{deliveryNoteHeader.Phone1}',
                        '{deliveryNoteHeader.Phone2}',
                        '{deliveryNoteHeader.Phone3}',
                        '{deliveryNoteHeader.Phone4}',
                        {deliveryNoteHeader.SubjectId},
                        {deliveryNoteHeader.AgentId},
                        '{SafeConvert.ToPervasiveString(deliveryNoteHeader.AgentName)}',
                        '{SafeConvert.ToPervasiveString(deliveryNoteHeader.Notes)}',
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
                            arizasugBDY,		
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
                            '{SafeConvert.ToPervasiveString(deliveryNoteBody.ItemId)}',
                            '{SafeConvert.ToPervasiveString(deliveryNoteBody.ItemName)}',                            
                            '{deliveryNoteBody.Color}',
                            '{SafeConvert.ToPervasiveString(deliveryNoteBody.Notes)}',
                            {PervasiveDBHelper.ToPervasiveDate(deliveryNoteBody.ProvisionDate)},
                            {deliveryNoteBody.Length},
                            {deliveryNoteBody.Width},
                            {deliveryNoteBody.Thickness},
                            {deliveryNoteBody.Weight},
                            '{SafeConvert.ToPervasiveString(deliveryNoteBody.PackType)}',
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

            void CreateMISMRow(OdbcCommand command, int inventoryBaseIdentity, int accountingBaseIdentity)
            {                
                this.CreateMISMRow(new MismRowData
                {
                    AccountId = deliveryNote.Header.AccountId,
                    AccountName = deliveryNote.Header.AccountName,
                    TableName = "TMMas",
                    MismNo = deliveryNote.Header.DocumentId,
                    MismCode = eDocCode.DeliveryNote,
                    DocDate = deliveryNote.Header.ProvisionDate ?? DateTime.Now,
                    Total = deliveryNote.Header.Total,
                    Vat = deliveryNote.Header.Vat,
                    ProducerName = "API",
                    DocPrintPattern = "TMMasRep",
                    AccountingCommandNo = accountingBaseIdentity,  // CreateAccountingTransactions > identity (base)
                    InventoryCommandNo = inventoryBaseIdentity     // CreateInventoryTransactions > identity (base)
                }, command);
            }

            void CreateInventoryTransactions(OdbcCommand command, out int baseIdentity)
            {
                int identity = (1000 * this.GetNextDocCounter(eCounterNo.InventoryTransaction));
                baseIdentity = identity;

                var transactionId = this.GetNextTransactionId(eTransactionProvider.InvTrs);

                deliveryNote.Body.ForEach(deliveryNoteBody => {
                    this.CreateInventoryTransaction(new InventoryTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = transactionId++,
                        StoreNo = deliveryNote.Header.StoreNo,
                        ItemId = deliveryNoteBody.ItemId,
                        CreatedDate = deliveryNote.Header.CreatedDate ?? DateTime.Now,
                        InOut = "i",
                        DocumentCode = "T",
                        DocumentId = deliveryNote.Header.DocumentId,
                        Notes = deliveryNote.Header.Notes,
                        SubjectId = deliveryNote.Header.SubjectId,
                        AgentId = deliveryNote.Header.AgentId,
                        Asmac = deliveryNote.Header.Asmac,
                        AccountId = deliveryNote.Header.AccountId,
                        Quantity = -(deliveryNoteBody.Quantity),
                        Total = deliveryNoteBody.Total
                    }, command);
                });

                this.SetNextDocCounter(eCounterNo.InventoryTransaction, command);
            }

            // ---

            OdbcTransaction transaction = null;

            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    step = 1;

                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    // create document (header + body)
                    command.CommandText = CreateHeaderQuery(deliveryNote.Header);
                    command.ExecuteNonQuery();

                    step = 2;

                    var lineNo = 1;
                    deliveryNote.Body.ForEach(deliveryNoteBody => {
                        command.CommandText = CreateBodyQuery(deliveryNote.Header.DocumentId, lineNo++, deliveryNoteBody);
                        command.ExecuteNonQuery();
                    });
                    step = 3;

                    // create inventory-transactions
                    int inventoryBaseIdentity = 0;
                    if (!deliveryNote.Header.DisableInventory)
                        CreateInventoryTransactions(command, out inventoryBaseIdentity);                   

                    step = 4;

                    // update stock
                    deliveryNote.Body.ForEach(deliveryNoteBody => {
                        this.StoreItemUpdate(new StoreItem(deliveryNote.Header.StoreNo, deliveryNoteBody.ItemId), command);
                    });

                    step = 5;

                    CreateMISMRow(command, inventoryBaseIdentity, 0);

                    step = 6;

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step: {step})");
            }
        }

        public bool CreateReceipt(Receipt receipt)
        {
            var step = 0;

            // local methods 
            string CreateHeaderQuery(ReceiptHeader receiptHeader)
            {
                var query = $@"
                        INSERT INTO KabalaMasHdr
                        (
                            DOCCODE,    
                            NoHDR,
                            AccNoHDR,		    
                            NameHDR, 		
                            AddressHDR, 		
                            CityHDR, 	
                            Asmacta2HDR,   
                            ComDateHDR,
                            SubjectHDR,
                            TrsDetailsHDR,
                            GnisTOTHDR,
                            TotNisHDR,
                            StatusHDR,
                            DocStatusHDR,                           
                            NikuyBamakorHDR,
                            CreditTypeHDR,
                            DealTypeHDR,
                            CardOwnerIDHDR,
                            CreditNoHDR,
                            TokefHDR,
                            NoOfPayesHDR,
                            FirstPayHDR,
                            OtherPayesHDR,
                            IshurNoHDR,
                            UserNameHDR,
                            MELELHDR,
                            DATEHDR                            
                        )                        
                        (
                            SELECT 
                            {receiptHeader.DocumentId},                             
                            {receiptHeader.DocumentId},
                            {receiptHeader.AccountId},
                            '{SafeConvert.ToPervasiveString(receiptHeader.AccountName)}',
                            '{SafeConvert.ToPervasiveString(receiptHeader.Address)}',
                            '{SafeConvert.ToPervasiveString(receiptHeader.City)}',    
                            '{receiptHeader.Asmac}',   
                            {PervasiveDBHelper.ToPervasiveDate(receiptHeader.ValueDate)},
                            {receiptHeader.SubjectId},    
                            '{SafeConvert.ToPervasiveString(receiptHeader.Notes)}',
                            {receiptHeader.Total + receiptHeader.Deduction}, 
                            {receiptHeader.Total}, 
                            '{receiptHeader.Status}', 
                            {receiptHeader.SystemStatus}, 
                            {receiptHeader.Deduction}, 
                            '{SafeConvert.ToPervasiveString(receiptHeader.CreditCardType)}', 
                            '{SafeConvert.ToPervasiveString(receiptHeader.DealType.GetDescription())}', 
                            '{receiptHeader.CreditCardOwnerId}', 
                            '{receiptHeader.CreditCardNumber}', 
                            '{receiptHeader.CreditCardExpiry}', 
                            {receiptHeader.NumberOfPayments}, 
                            {receiptHeader.FirstPaymentAmount}, 
                            {receiptHeader.EachPaymentAmount}, 
                            {receiptHeader.ConfirmationNumber}, 
                            '{receiptHeader.SourceName}',
                            '{SafeConvert.ToPervasiveString(receiptHeader.Notes)}',
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}                             
                        )
                    ";

                return query;
            }
            string CreateBodyQuery(int documentId, int LineNo, ReceiptBody receiptBody)
            {
                var kupaData = this.GetKUPA(receiptBody.PaymentType);
                if (kupaData == null) throw new Exception($"NO KUPA found for PaymentType {receiptBody.PaymentType}");

                var query = $@"
                        INSERT INTO KabalaMasBdy
                        (
                            DOCCODE,  		
                            GridLineNo,		
                            KupaNoBDY,
                            KupaAccBDY,                    
                            MeanBDY, 
                            vDateBDY,
                            DetailsBDY,
                            SumBDY,
                            SumNisBDY,     
                            NoOfPaysBDY,
                            LineDetailsBDY,
                            ChequeNoBDY
                        )
                        (
                            SELECT 
                            {documentId},                             
                            {LineNo},
                            {kupaData.KupaNo},
                            {kupaData.AccountNo},
                            '{this.PaymentType2KupaName(receiptBody.PaymentType)}',
                            {PervasiveDBHelper.ToPervasiveDate(receiptBody.PaymentDate)},  
                            '{SafeConvert.ToPervasiveString(receiptBody.Notes)}',
                            {receiptBody.TotalNIS},
                            {receiptBody.TotalNIS},                            
                            {receiptBody.NumberOfPayments},
                            '{"קבלה"}',
                            '{receiptBody.PaycheckNo}'
                        )
                    ";

                return query;
            }

            void CreateMISMRow(OdbcCommand command, int inventoryBaseIdentity, int accountingBaseIdentity)
            {                
                this.CreateMISMRow(new MismRowData
                {
                    AccountId = receipt.Header.AccountId,
                    AccountName = receipt.Header.AccountName,
                    TableName = "KabalaMas",
                    MismNo = receipt.Header.DocumentId,
                    MismCode = eDocCode.Receipt,
                    DocDate = receipt.Header.ValueDate ?? DateTime.Now,
                    Total = receipt.Header.Total,
                    Vat = 0,
                    ProducerName = "API",
                    DocPrintPattern = "KabalaMasRep",
                    AccountingCommandNo = accountingBaseIdentity,  // CreateAccountingTransactions > identity (base)
                    InventoryCommandNo = inventoryBaseIdentity     // CreateInventoryTransactions > identity (base)
                }, command);
            }

            void CreateAccountingTransactions(OdbcCommand command, out int baseIdentity) {
                int identity = (1000 * this.GetNextDocCounter(eCounterNo.AccountingTransaction));
                baseIdentity = identity;

                var transactionId = this.GetNextTransactionId();

                var opAcc1017 = this.GetOpAcc(1017);
                var opAcc3 = this.GetOpAcc(3);                

                // (1)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = transactionId,
                    AccountId = receipt.Header.AccountId,
                    Asmac1 = receipt.Header.DocumentId.ToString(),
                    Asmac2 = receipt.Header.Asmac,
                    OpAcc = 0,
                    PaidDate = receipt.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = receipt.Header.ValueDate,
                    Subject = string.Empty,
                    Total = receipt.Header.Total,
                    CreditDebit = "ז",
                    DocumentId = receipt.Header.DocumentId,
                    DocumentCode = eDocCode.Receipt,
                    Notes = receipt.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = receipt.Header.CreatedDate ?? DateTime.Now,
                }, command);

                // (2)
                receipt.Body.ForEach(receiptBody => {
                    var kupaData = this.GetKUPA(receiptBody.PaymentType);
                    if (kupaData == null) throw new Exception($"NO KUPA found for PaymentType {receiptBody.PaymentType}");

                    this.CreateAccountingTransaction(new AccountingTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = ++transactionId,
                        AccountId = kupaData?.KupaNo ?? 0,
                        Asmac1 = receipt.Header.DocumentId.ToString(),
                        Asmac2 = receipt.Header.Asmac,
                        OpAcc = receipt.Header.AccountId,
                        PaidDate = receipt.Header.CreatedDate ?? DateTime.Now,
                        ValueDate = receipt.Header.ValueDate,
                        Subject = string.Empty,
                        Total = receiptBody.TotalNIS,
                        CreditDebit = "ח",
                        DocumentId = receipt.Header.DocumentId,
                        DocumentCode = eDocCode.Receipt,
                        Notes = receiptBody.Notes,
                        IsTaxCoordCompleted = false,
                        EventDate = receipt.Header.CreatedDate ?? DateTime.Now,
                    }, command);
                });

                // (3)
                if (receipt.Header.Deduction > 0)
                {
                    this.CreateAccountingTransaction(new AccountingTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = ++transactionId,
                        AccountId = receipt.Header.AccountId,
                        Asmac1 = receipt.Header.DocumentId.ToString(),
                        Asmac2 = receipt.Header.Asmac,
                        OpAcc = opAcc3,
                        PaidDate = receipt.Header.CreatedDate ?? DateTime.Now,
                        ValueDate = receipt.Header.ValueDate,
                        Subject = string.Empty,
                        Total = receipt.Header.Deduction,
                        CreditDebit = "ז",
                        DocumentId = receipt.Header.DocumentId,
                        DocumentCode = eDocCode.Receipt,
                        Notes = receipt.Header.Notes,
                        IsTaxCoordCompleted = false,
                        EventDate = receipt.Header.CreatedDate ?? DateTime.Now,
                    }, command);

                    // (4)
                    this.CreateAccountingTransaction(new AccountingTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = ++transactionId,
                        AccountId = opAcc3,
                        Asmac1 = receipt.Header.DocumentId.ToString(),
                        Asmac2 = receipt.Header.Asmac,
                        OpAcc = receipt.Header.AccountId,
                        PaidDate = receipt.Header.CreatedDate ?? DateTime.Now,
                        ValueDate = receipt.Header.ValueDate,
                        Subject = string.Empty,
                        Total = receipt.Header.Deduction,
                        CreditDebit = "ח",
                        DocumentId = receipt.Header.DocumentId,
                        DocumentCode = eDocCode.Receipt,
                        Notes = receipt.Header.Notes,
                        IsTaxCoordCompleted = false,
                        EventDate = receipt.Header.CreatedDate ?? DateTime.Now,
                    }, command);
                }

                this.SetNextDocCounter(eCounterNo.AccountingTransaction, command);
            }

            void CreateKUPATransactions(OdbcCommand command) {

                var lineNo = PervasiveDBHelper.GetColumnMaxValue(command, "KupaBdy", "RecNo");                
                receipt.Body.ForEach(receiptBody =>
                {
                    var kupaData = this.GetKUPA(receiptBody.PaymentType);
                    if (kupaData == null) throw new Exception($"NO KUPA found for PaymentType {receiptBody.PaymentType}");

                    switch ((ePaymentType)receiptBody.PaymentType) {
                        case ePaymentType.Paycheck: 
                            this.CreateKupaRow(lineNo++, new KupaBody { 
                                AccountId = receipt.Header.AccountId,
                                BankAccount = receiptBody.BankAccount,
                                BankCode = receiptBody.BankCode,  
                                BranchCode = receiptBody.BranchCode,
                                DocumentId = receipt.Header.DocumentId,
                                KupaNo = kupaData.KupaNo,
                                Notes = receiptBody.Notes,
                                PaycheckNo = receiptBody.PaycheckNo,
                                PaymentType = receiptBody.PaymentType,
                                Total = receiptBody.TotalNIS,
                                NumberOfPayments = receiptBody.NumberOfPayments,
                                ValueDate = receiptBody.PaymentDate
                            }, command); // המחאה
                            break;
                        case ePaymentType.Cash:
                            this.UpdateKupaCash(kupaData.KupaNo, receiptBody.TotalNIS, command); // מזומן
                            break;
                    }
                });
            }

            // ---

            OdbcTransaction transaction = null;

            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    step = 1;

                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    // create document (header + body)
                    command.CommandText = CreateHeaderQuery(receipt.Header);
                    command.ExecuteNonQuery();

                    step = 2;

                    var lineNo = 1;
                    receipt.Body.ForEach(receiptBody => {
                        command.CommandText = CreateBodyQuery(receipt.Header.DocumentId, lineNo++, receiptBody);
                        command.ExecuteNonQuery();
                    });

                    step = 3;

                    // create accounting-transactions
                    int accountingBaseIdentity;
                    CreateAccountingTransactions(command, out accountingBaseIdentity);

                    step = 4;

                    CreateMISMRow(command, 0, accountingBaseIdentity);

                    step = 5;

                    CreateKUPATransactions(command);

                    step = 6;

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step: {step})");
            }
        }
        
        public bool CreateInvoiceReceipt(InvoiceReceipt invoiceReceipt)
        {
            var step = 0;

            // local methods 
            string CreateHeaderQuery(InvoiceHeader invoiceReceiptHeader)
            {
                var query = $@"
                        INSERT INTO HeshtMKMasHdr
                        (
                            DOCCODE,
                            DocNoHDR,                             
                            AccNoHDR,		    
                            AccNameHDR, 		
                            AccAddrHDR, 		
                            AccCityHDR, 		                            		
                            AccPhone1HDR, 		                            
                            TrsDebitSubjectHDR,
                            AgentHDR,		    
                            AgNameHDR,		
                            AccRemarksHDR,                                 
                            Asmac2HDR,		
                            ValuDateHDR,		                                
                            DocStatusHDR,		                            	    
                            SubTotalHDR,		
                            ReductionHDR,		
                            ComReducHDR,
                            BeforVatHDR,		
                            VatSumHDR,		
                            GrandTotalHDR,	                             
                            MAAMPHDR,
                            RoundReducHDR,
                            DATEHDR,		                                
                            TimeHDR
                        )                        
                        (
                            SELECT 
                            {invoiceReceiptHeader.DocumentId}, 
                            {invoiceReceiptHeader.DocumentId},
                            {invoiceReceiptHeader.AccountId},
                            '{SafeConvert.ToPervasiveString(invoiceReceiptHeader.AccountName)}',
                            '{SafeConvert.ToPervasiveString(invoiceReceiptHeader.Address)}',
                            '{SafeConvert.ToPervasiveString(invoiceReceiptHeader.City)}',                            
                            '{invoiceReceiptHeader.Phone1}',                            
                            {invoiceReceiptHeader.SubjectId},
                            {invoiceReceiptHeader.AgentId},
                            '{SafeConvert.ToPervasiveString(invoiceReceiptHeader.AgentName)}',
                            '{SafeConvert.ToPervasiveString(invoiceReceiptHeader.Notes)}',                            
                            '{invoiceReceiptHeader.Asmac}',
                            {PervasiveDBHelper.ToPervasiveDate(invoiceReceiptHeader.ProvisionDate)},                            
                            {invoiceReceiptHeader.SystemStatus},                             
                            {invoiceReceiptHeader.SubTotal}, 
                            {invoiceReceiptHeader.Discount}, 
                            {invoiceReceiptHeader.DiscountPercentage}, 
                            {invoiceReceiptHeader.TotalBeforeVat}, 
                            {invoiceReceiptHeader.Vat}, 
                            {invoiceReceiptHeader.Total},                             
                            {invoiceReceiptHeader.VATRate},
                            {invoiceReceiptHeader.Round},
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)},                             
                            '{DateTime.Now.ToString("HH:mm")}'
                        )
                    ";

                return query;
            }
            string CreateBodyQuery(int documentId, int LineNo, InvoiceBody invoiceReceiptBody)
            {
                var query = $@"
                        INSERT INTO HeshtMKMasBdy
                        (
                            DOCCODE,  		
                            GridLineNo,		
                            ItemNoBDY,		
                            ItemNameBDY, 
                            ItemNo1BDY,	    
                            MifratBDY,	                           	
                            ItemQtyBDY,		
                            ItemPrcBDY,		                            	
                            ItemReducBDY,		
                            ItemSumBDY		 
                        )
                        (
                            SELECT 
                            {documentId},                             
                            {LineNo},                            
                            '{SafeConvert.ToPervasiveString(invoiceReceiptBody.ItemId)}',
                            '{SafeConvert.ToPervasiveString(invoiceReceiptBody.ItemName)}', 
                            '{invoiceReceiptBody.Auxcode}',
                            '{SafeConvert.ToPervasiveString(invoiceReceiptBody.Notes)}',
                            {invoiceReceiptBody.Quantity},
                            {invoiceReceiptBody.UnitPriceBeforeVAT},                            
                            {invoiceReceiptBody.DiscountPercentage},
                            {invoiceReceiptBody.SalePriceBeforeVAT}
                        )
                    ";

                return query;
            }

            void CreateMISMRow(OdbcCommand command, int inventoryBaseIdentity, int accountingBaseIdentity)
            {                
                this.CreateMISMRow(new MismRowData
                {
                    AccountId = invoiceReceipt.Header.AccountId,
                    AccountName = invoiceReceipt.Header.AccountName,
                    TableName = "HeshtMKMas",
                    MismNo = invoiceReceipt.Header.DocumentId,
                    MismCode = eDocCode.InvoiceReceipt,
                    DocDate = invoiceReceipt.Header.ProvisionDate ?? DateTime.Now,
                    Total = invoiceReceipt.Header.Total,
                    Vat = invoiceReceipt.Header.Vat,
                    ProducerName = "API",
                    DocPrintPattern = "HeshtMKMasRep",
                    AccountingCommandNo = accountingBaseIdentity,  // CreateAccountingTransactions > identity (base)
                    InventoryCommandNo = inventoryBaseIdentity     // CreateInventoryTransactions > identity (base)
                }, command);
            }

            void CreateInventoryTransactions(OdbcCommand command, out int baseIdentity)
            {
                int identity = (1000 * this.GetNextDocCounter(eCounterNo.InventoryTransaction));
                baseIdentity = identity;

                var transactionId = this.GetNextTransactionId(eTransactionProvider.InvTrs);

                invoiceReceipt.Body.ForEach(invoiceReceiptBody => {
                    this.CreateInventoryTransaction(new InventoryTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = transactionId++,
                        StoreNo = invoiceReceipt.Header.StoreNo,
                        ItemId = invoiceReceiptBody.ItemId,
                        CreatedDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                        InOut = "i",
                        DocumentCode = eDocCode.InvoiceReceipt,
                        DocumentId = invoiceReceipt.Header.DocumentId,
                        Notes = invoiceReceipt.Header.Notes,
                        SubjectId = invoiceReceipt.Header.SubjectId,
                        AgentId = invoiceReceipt.Header.AgentId,
                        Asmac = invoiceReceipt.Header.Asmac,
                        AccountId = invoiceReceipt.Header.AccountId,
                        Quantity = -(invoiceReceiptBody.Quantity),
                        Total = invoiceReceiptBody.Total
                    }, command);
                });

                this.SetNextDocCounter(eCounterNo.InventoryTransaction, command);
            }

            void CreateAccountingTransactions(OdbcCommand command, out int baseIdentity)
            {
                int identity = (1000 * this.GetNextDocCounter(eCounterNo.AccountingTransaction));
                baseIdentity = identity;

                var transactionId = this.GetNextTransactionId();

                var opAcc7 = this.GetOpAcc(7);
                var opAcc1 = this.GetOpAcc(1);
                var opAcc3 = this.GetOpAcc(3);
                var opAcc1015 = this.GetOpAcc(1015);

                // [Invoice]
                // (1)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = transactionId,
                    AccountId = invoiceReceipt.Header.AccountId,
                    Asmac1 = invoiceReceipt.Header.DocumentId.ToString(),
                    Asmac2 = invoiceReceipt.Header.Asmac,
                    PaidDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = invoiceReceipt.Header.ProvisionDate,
                    Subject = invoiceReceipt.Header.SubjectId.ToString(),
                    Total = invoiceReceipt.Header.Total,
                    CreditDebit = "ח",
                    DocumentId = invoiceReceipt.Header.DocumentId,
                    DocumentCode = eDocCode.InvoiceReceipt,
                    Notes = invoiceReceipt.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                }, command);

                // (2)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = ++transactionId,
                    AccountId = opAcc7,
                    Asmac1 = invoiceReceipt.Header.DocumentId.ToString(),
                    Asmac2 = invoiceReceipt.Header.Asmac,
                    PaidDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = invoiceReceipt.Header.ProvisionDate,
                    Subject = invoiceReceipt.Header.SubjectId.ToString(),
                    Total = invoiceReceipt.Header.TotalBeforeVat,
                    CreditDebit = "ז",
                    DocumentId = invoiceReceipt.Header.DocumentId,
                    DocumentCode = eDocCode.InvoiceReceipt,
                    Notes = invoiceReceipt.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                }, command);

                // (3)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = ++transactionId,
                    AccountId = opAcc1,
                    Asmac1 = invoiceReceipt.Header.DocumentId.ToString(),
                    Asmac2 = invoiceReceipt.Header.Asmac,
                    PaidDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = invoiceReceipt.Header.ProvisionDate,
                    Subject = invoiceReceipt.Header.SubjectId.ToString(),
                    Total = invoiceReceipt.Header.Vat,
                    CreditDebit = "ז",
                    DocumentId = invoiceReceipt.Header.DocumentId,
                    DocumentCode = eDocCode.InvoiceReceipt,
                    Notes = invoiceReceipt.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                }, command);
                
                // [Receipt]
                // (1)
                this.CreateAccountingTransaction(new AccountingTransaction
                {
                    CommandNo = ++identity,
                    TransactionId = ++transactionId,
                    AccountId = invoiceReceipt.Header.AccountId,
                    Asmac1 = invoiceReceipt.Header.DocumentId.ToString(),
                    Asmac2 = invoiceReceipt.Header.Asmac,
                    OpAcc = 0,
                    PaidDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                    ValueDate = invoiceReceipt.Header.ProvisionDate,
                    Subject = string.Empty,
                    Total = invoiceReceipt.Header.Total,
                    CreditDebit = "ז",
                    DocumentId = invoiceReceipt.Header.DocumentId,
                    DocumentCode = eDocCode.InvoiceReceipt,
                    Notes = invoiceReceipt.Header.Notes,
                    IsTaxCoordCompleted = false,
                    EventDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                }, command);

                // (2)
                invoiceReceipt.Payments.ForEach(receiptBody => {
                    var kupaData = this.GetKUPA(receiptBody.PaymentType);
                    if (kupaData == null) throw new Exception($"NO KUPA found for PaymentType {receiptBody.PaymentType}");

                    this.CreateAccountingTransaction(new AccountingTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = ++transactionId,
                        AccountId = kupaData?.KupaNo ?? 0,
                        Asmac1 = invoiceReceipt.Header.DocumentId.ToString(),
                        Asmac2 = invoiceReceipt.Header.Asmac,
                        OpAcc = invoiceReceipt.Header.AccountId,
                        PaidDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                        ValueDate = invoiceReceipt.Header.ProvisionDate,
                        Subject = string.Empty,
                        Total = receiptBody.TotalNIS,
                        CreditDebit = "ח",
                        DocumentId = invoiceReceipt.Header.DocumentId,
                        DocumentCode = eDocCode.InvoiceReceipt,
                        Notes = receiptBody.Notes,
                        IsTaxCoordCompleted = false,
                        EventDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                    }, command);
                });

                if (invoiceReceipt.Header.Deduction > 0)
                {
                    // (3)
                    this.CreateAccountingTransaction(new AccountingTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = ++transactionId,
                        AccountId = invoiceReceipt.Header.AccountId,
                        Asmac1 = invoiceReceipt.Header.DocumentId.ToString(),
                        Asmac2 = invoiceReceipt.Header.Asmac,
                        OpAcc = opAcc3,
                        PaidDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                        ValueDate = invoiceReceipt.Header.ProvisionDate,
                        Subject = string.Empty,
                        Total = invoiceReceipt.Header.Deduction,
                        CreditDebit = "ז",
                        DocumentId = invoiceReceipt.Header.DocumentId,
                        DocumentCode = eDocCode.InvoiceReceipt,
                        Notes = invoiceReceipt.Header.Notes,
                        IsTaxCoordCompleted = false,
                        EventDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                    }, command);

                    // (4)
                    this.CreateAccountingTransaction(new AccountingTransaction
                    {
                        CommandNo = ++identity,
                        TransactionId = ++transactionId,
                        AccountId = opAcc3,
                        Asmac1 = invoiceReceipt.Header.DocumentId.ToString(),
                        Asmac2 = invoiceReceipt.Header.Asmac,
                        OpAcc = invoiceReceipt.Header.AccountId,
                        PaidDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                        ValueDate = invoiceReceipt.Header.ProvisionDate,
                        Subject = string.Empty,
                        Total = invoiceReceipt.Header.Deduction,
                        CreditDebit = "ח",
                        DocumentId = invoiceReceipt.Header.DocumentId,
                        DocumentCode = eDocCode.InvoiceReceipt,
                        Notes = invoiceReceipt.Header.Notes,
                        IsTaxCoordCompleted = false,
                        EventDate = invoiceReceipt.Header.CreatedDate ?? DateTime.Now,
                    }, command);
                }

                this.SetNextDocCounter(eCounterNo.AccountingTransaction, command);
            }

            void CreateKUPATransactions(OdbcCommand command)
            {
                var lineNo = PervasiveDBHelper.GetColumnMaxValue(command, "KupaBdy", "RecNo");
                invoiceReceipt.Payments.ForEach(receiptBody =>
                {
                    var kupaData = this.GetKUPA(receiptBody.PaymentType);
                    if (kupaData == null) throw new Exception($"NO KUPA found for PaymentType {receiptBody.PaymentType}");

                    switch ((ePaymentType)receiptBody.PaymentType)
                    {
                        case ePaymentType.Paycheck:
                            this.CreateKupaRow(lineNo++, new KupaBody
                            {
                                AccountId = invoiceReceipt.Header.AccountId,
                                BankAccount = receiptBody.BankAccount,
                                BankCode = receiptBody.BankCode,
                                BranchCode = receiptBody.BranchCode,
                                DocumentId = invoiceReceipt.Header.DocumentId,
                                KupaNo = kupaData.KupaNo,
                                Notes = receiptBody.Notes,
                                PaycheckNo = receiptBody.PaycheckNo,
                                PaymentType = receiptBody.PaymentType,
                                Total = receiptBody.TotalNIS,
                                NumberOfPayments = receiptBody.NumberOfPayments,
                                ValueDate = receiptBody.PaymentDate
                            }, command); // המחאה
                            break;
                        case ePaymentType.Cash:
                            this.UpdateKupaCash(kupaData.KupaNo, receiptBody.TotalNIS, command); // מזומן
                            break;
                    }
                });
            }

            // ---

            try
            {
                OdbcTransaction transaction = null;

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    step = 1;

                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    // create document (header + body)
                    command.CommandText = CreateHeaderQuery(invoiceReceipt.Header);
                    command.ExecuteNonQuery();

                    step = 2;

                    var lineNo = 1;
                    invoiceReceipt.Body.ForEach(invoiceReceiptBody => {
                        command.CommandText = CreateBodyQuery(invoiceReceipt.Header.DocumentId, lineNo++, invoiceReceiptBody);
                        command.ExecuteNonQuery();
                    });

                    step = 3;

                    // create inventory-transactions
                    int inventoryBaseIdentity = 0;
                    if (!invoiceReceipt.Header.DisableInventory)                        
                        CreateInventoryTransactions(command, out inventoryBaseIdentity);                    

                    step = 4;

                    // create accounting-transactions
                    int accountingBaseIdentity;
                    CreateAccountingTransactions(command, out accountingBaseIdentity);

                    step = 5;

                    // update account totals 
                    this.UpdateAccountingTransactionTotals(invoiceReceipt.Header.AccountId, command);

                    step = 6;

                    // update stock
                    invoiceReceipt.Body.ForEach(invoiceReceiptBody => {
                        this.StoreItemUpdate(new StoreItem(invoiceReceipt.Header.StoreNo, invoiceReceiptBody.ItemId), command);
                    });

                    step = 7;

                    CreateMISMRow(command, inventoryBaseIdentity, accountingBaseIdentity);

                    step = 8;

                    CreateKUPATransactions(command);

                    step = 8;

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step: {step})");
            }
        }

        public bool CreateAccountingTransaction(AccountingTransaction transaction)
        {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();

                var command = new OdbcCommand();
                command.Connection = connection;

                return this.CreateAccountingTransaction(transaction, command);
            }
        }

        
        public IEnumerable<Order> FindOrders(OrderSearchParams searchParams)
        {
            var hasStatHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "StatHDR");
            var hasLDupdHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "LDupdHDR");

            // local methods 
            string CreateHeaderQuery()
            {
                var query = $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}hdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    AND     ({searchParams.FromDocumentId} <= 0 OR DOCCODE >= {searchParams.FromDocumentId})
                    AND     ({searchParams.ToDocumentId} <= 0 OR DOCCODE <= {searchParams.ToDocumentId})                    
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)} <= 0 OR DATEHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)})
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)} <= 0 OR DATEHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)})                                  
                ";

                if (hasStatHDR) query += $"AND     ('{searchParams.Status ?? ""}' IN ('', '0') OR StatHDR = '{searchParams.Status}') ";
                if (hasLDupdHDR) {
                    query += $@"
                        AND({ PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)} <= 0 OR LDupdHDR >= { PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)})
                        AND({ PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)} <= 0 OR LDupdHDR <= { PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)})
                    ";      
                }                
                query += "ORDER BY DATEHDR DESC";
                return query;
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}bdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            OrderHeader ReadHeader(OdbcDataReader reader)
            {
                return new OrderHeader
                {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                    DocNo = Convert.ToInt32(reader["DocNoHDR"]),
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = SafeConvert.ToString(reader["AccNameHDR"]),
                    Address = SafeConvert.ToString(reader["AccAddrHDR"]),
                    City = SafeConvert.ToString(reader.ReadOrDefault<string>("AccCityHDR")),
                    AgentId = Convert.ToInt32(reader["AgentHDR"]),
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    SubTotal = SafeConvert.ToSingle(reader["SubTotalHDR"]),
                    Discount = SafeConvert.ToSingle(reader["ReductionHDR"]),
                    TotalBeforeVat = SafeConvert.ToSingle(reader["BeforVatHDR"]),
                    Vat = SafeConvert.ToSingle(reader["VatSumHDR"]),
                    Total = SafeConvert.ToSingle(reader["GrandTotalHDR"]),
                    CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["DATEHDR"])),

                    Contact = SafeConvert.ToString(reader.ReadOrDefault<string>("Edit205HDR")),
                    Phone1 = SafeConvert.ToString(reader.ReadOrDefault<string>("AccPhone1HDR")),
                    Phone2 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone2HDR")),
                    Phone3 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone3HDR")),
                    Phone4 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone4HDR")),
                    SubjectId = Convert.ToInt32(reader.ReadOrDefault<int>("TrsDebitSubjectHDR")),                    
                    AgentName = SafeConvert.ToString(reader.ReadOrDefault<string>("AgNameHDR")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("AccRemarksHDR")),
                    StoreNo = Convert.ToInt32(reader.ReadOrDefault<int>("StoreHDR")),
                    Asmac = SafeConvert.ToString(reader.ReadOrDefault<string>("Asmac2HDR")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("ValuDateHDR"))),
                    ReturnDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("HDateHDR"))),                    
                    Status = SafeConvert.ToString(reader.ReadOrDefault<string>("StatHDR")),                    
                    DiscountPercentage = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ComReducHDR")),                    
                    Paid = SafeConvert.ToSingle(reader.ReadOrDefault<float>("PaidHDR")),
                    VATRate = SafeConvert.ToSingle(reader.ReadOrDefault<float>("MAAMPHDR")),
                    Round = SafeConvert.ToSingle(reader.ReadOrDefault<float>("RoundReducHDR")),                    
                    UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDupdHDR"))),
                    SearchTable = searchParams.TableName
                };
            }

            OrderBody ReadBody(OdbcDataReader reader)
            {
                return new OrderBody
                {
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    ItemId = SafeConvert.ToString(reader["ItemNoBDY"]),
                    ItemName = SafeConvert.ToString(reader["ItemNameBDY"]),
                    Quantity = SafeConvert.ToSingle(reader["ItemQtyBDY"]),
                    UnitPriceBeforeVAT = SafeConvert.ToSingle(reader["ItemPrcBDY"]),                    
                    DiscountPercentage = SafeConvert.ToSingle(reader["ItemReducBDY"]),
                    SalePriceBeforeVAT = SafeConvert.ToSingle(reader["ItemSumBDY"]),

                    Barcode = SafeConvert.ToString(reader.ReadOrDefault<string>("BarcodeBDY")),
                    Auxcode = SafeConvert.ToString(reader.ReadOrDefault<string>("ChinesCodeBDY")),                    
                    Color = SafeConvert.ToString(reader.ReadOrDefault<string>("ColorBDY")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("MifratBDY")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDate1BDY"))),
                    Length = SafeConvert.ToSingle(reader.ReadOrDefault<float>("LENBDY")),
                    Width = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemWidthBDY")),
                    Thickness = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemThincknessBDY")),
                    Weight = SafeConvert.ToSingle(reader.ReadOrDefault<float>("WBDY")),
                    PackType = SafeConvert.ToString(reader.ReadOrDefault<string>("arizasugBDY")),
                    PacksInLot = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTBDY")),
                    UnitsInPack = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTQTYBDY")),
                    UnitPrice = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemPrc1BDY"))                    
                };
            }

            // ---

            try
            {
                var orders = new List<Order>();

                // ignore tables search with updated-date filter but no updated-date column
                // to prevent receiving incorrect data
                var hasUpdDateFilter = searchParams.FromUpdateDate.HasValue || searchParams.ToUpdateDate.HasValue;
                if (searchParams.PauseIfNoUpdateDateColumn && !hasLDupdHDR && hasUpdDateFilter)
                    return orders;

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    command.CommandText = CreateHeaderQuery();

                    // load orders (header)
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            orders.Add(new Order
                            {
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

        public IEnumerable<Order> FindOrdersMas(OrderSearchParams searchParams)
        {
            var hasStatHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "StatHDR");
            var hasLDupdHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "LDupdHDR");

            // local methods 
            string CreateHeaderQuery() {
                var query = $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}hdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    AND     ({searchParams.FromDocumentId} <= 0 OR DOCCODE >= {searchParams.FromDocumentId})
                    AND     ({searchParams.ToDocumentId} <= 0 OR DOCCODE <= {searchParams.ToDocumentId})                    
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)} <= 0 OR DATEHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)})
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)} <= 0 OR DATEHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)})                                        
                ";

                if (hasStatHDR) query += $"AND     ('{searchParams.Status ?? ""}' IN ('', '0') OR StatHDR = '{searchParams.Status}') ";
                if (hasLDupdHDR) {
                    query += $@"
                        AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)} <= 0 OR LDupdHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)})
                        AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)} <= 0 OR LDupdHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)})
                    ";
                }
                query += "ORDER BY DATEHDR DESC";
                return query;
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}bdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            OrderHeader ReadHeader(OdbcDataReader reader) {                
                return new OrderHeader {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                    DocNo = Convert.ToInt32(reader["DocNoHDR"]),
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = SafeConvert.ToString(reader["AccNameHDR"]),
                    Address = SafeConvert.ToString(reader["AccAddrHDR"]),
                    City = SafeConvert.ToString(reader["AccCityHDR"]),
                    AgentId = Convert.ToInt32(reader["AgentHDR"]),
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    SubTotal = SafeConvert.ToSingle(reader["SubTotalHDR"]),
                    Discount = SafeConvert.ToSingle(reader["ReductionHDR"]),
                    TotalBeforeVat = SafeConvert.ToSingle(reader["BeforVatHDR"]),
                    Vat = SafeConvert.ToSingle(reader["VatSumHDR"]),
                    Total = SafeConvert.ToSingle(reader["GrandTotalHDR"]),
                    CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["DATEHDR"])),

                    Contact = SafeConvert.ToString(reader.ReadOrDefault<string>("Edit205HDR")),
                    Phone1 = SafeConvert.ToString(reader.ReadOrDefault<string>("AccPhone1HDR")),
                    Phone2 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone2HDR")),
                    Phone3 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone3HDR")),
                    Phone4 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone4HDR")),
                    SubjectId = Convert.ToInt32(reader.ReadOrDefault<int>("TrsDebitSubjectHDR")),
                    AgentName = SafeConvert.ToString(reader.ReadOrDefault<string>("AgNameHDR")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("AccRemarksHDR")),
                    StoreNo = Convert.ToInt32(reader.ReadOrDefault<int>("StoreHDR")),
                    Asmac = SafeConvert.ToString(reader.ReadOrDefault<string>("Asmac2HDR")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("ValuDateHDR"))),
                    ReturnDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("HDateHDR"))),
                    Status = SafeConvert.ToString(reader.ReadOrDefault<string>("StatHDR")),
                    DiscountPercentage = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ComReducHDR")),
                    Paid = SafeConvert.ToSingle(reader.ReadOrDefault<float>("PaidHDR")),
                    VATRate = SafeConvert.ToSingle(reader.ReadOrDefault<float>("MAAMPHDR")),
                    Round = SafeConvert.ToSingle(reader.ReadOrDefault<float>("RoundReducHDR")),
                    UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDupdHDR"))),
                    SearchTable = searchParams.TableName
                };
            }

            OrderBody ReadBody(OdbcDataReader reader)
            {
                return new OrderBody {
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    ItemId = SafeConvert.ToString(reader["ItemNoBDY"]),
                    ItemName = SafeConvert.ToString(reader["ItemNameBDY"]),
                    Quantity = SafeConvert.ToSingle(reader["ItemQtyBDY"]),
                    UnitPriceBeforeVAT = SafeConvert.ToSingle(reader["ItemPrcBDY"]),
                    DiscountPercentage = SafeConvert.ToSingle(reader["ItemReducBDY"]),
                    SalePriceBeforeVAT = SafeConvert.ToSingle(reader["ItemSumBDY"]),

                    Barcode = SafeConvert.ToString(reader.ReadOrDefault<string>("BarcodeBDY")),
                    Auxcode = SafeConvert.ToString(reader.ReadOrDefault<string>("ChinesCodeBDY")),
                    Color = SafeConvert.ToString(reader.ReadOrDefault<string>("ColorBDY")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("MifratBDY")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDate1BDY"))),
                    Length = SafeConvert.ToSingle(reader.ReadOrDefault<float>("LENBDY")),
                    Width = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemWidthBDY")),
                    Thickness = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemThincknessBDY")),
                    Weight = SafeConvert.ToSingle(reader.ReadOrDefault<float>("WBDY")),
                    PackType = SafeConvert.ToString(reader.ReadOrDefault<string>("arizasugBDY")),
                    PacksInLot = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTBDY")),
                    UnitsInPack = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTQTYBDY")),
                    UnitPrice = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemPrc1BDY"))
                };
            }

            // ---

            try
            {
                var orders = new List<Order>();

                // ignore tables search with updated-date filter but no updated-date column
                // to prevent receiving incorrect data
                var hasUpdDateFilter = searchParams.FromUpdateDate.HasValue || searchParams.ToUpdateDate.HasValue;
                if (searchParams.PauseIfNoUpdateDateColumn && !hasLDupdHDR && hasUpdDateFilter)
                    return orders;

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
        
        public IEnumerable<PriceOffer> FindPriceOffers(PriceOfferSearchParams searchParams)
        {
            var hasStatHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "StatHDR");
            var hasLDupdHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "LDupdHDR");

            // local methods 
            string CreateHeaderQuery() {
                var query = $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}hdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    AND     ({searchParams.FromDocumentId} <= 0 OR DOCCODE >= {searchParams.FromDocumentId})
                    AND     ({searchParams.ToDocumentId} <= 0 OR DOCCODE <= {searchParams.ToDocumentId})                    
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)} <= 0 OR DATEHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)})
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)} <= 0 OR DATEHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)})                                        
                ";

                if (hasStatHDR) query += $"AND     ('{searchParams.Status ?? ""}' = '' OR StatHDR = '{searchParams.Status}') ";
                if (hasLDupdHDR) {
                    query += $@"
                        AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)} <= 0 OR LDupdHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)})
                        AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)} <= 0 OR LDupdHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToUpdateDate)})
                    ";
                }
                query += "ORDER BY DATEHDR DESC";
                return query;
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}bdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            PriceOfferHeader ReadHeader(OdbcDataReader reader) {                
                return new PriceOfferHeader
                {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                    DocNo = Convert.ToInt32(reader["DocNoHDR"]),
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = SafeConvert.ToString(reader["AccNameHDR"]),
                    Address = SafeConvert.ToString(reader["AccAddrHDR"]),
                    City = SafeConvert.ToString(reader.ReadOrDefault<string>("AccCityHDR")),
                    AgentId = Convert.ToInt32(reader["AgentHDR"]),
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    SubTotal = SafeConvert.ToSingle(reader["SubTotalHDR"]),
                    Discount = SafeConvert.ToSingle(reader["ReductionHDR"]),
                    TotalBeforeVat = SafeConvert.ToSingle(reader["BeforVatHDR"]),
                    Vat = SafeConvert.ToSingle(reader["VatSumHDR"]),
                    Total = SafeConvert.ToSingle(reader["GrandTotalHDR"]),
                    CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["DATEHDR"])),

                    Contact = SafeConvert.ToString(reader.ReadOrDefault<string>("Edit205HDR")),
                    Phone1 = SafeConvert.ToString(reader.ReadOrDefault<string>("AccPhone1HDR")),
                    Phone2 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone2HDR")),
                    Phone3 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone3HDR")),
                    Phone4 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone4HDR")),
                    SubjectId = Convert.ToInt32(reader.ReadOrDefault<int>("TrsDebitSubjectHDR")),
                    AgentName = SafeConvert.ToString(reader.ReadOrDefault<string>("AgNameHDR")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("AccRemarksHDR")),
                    StoreNo = Convert.ToInt32(reader.ReadOrDefault<int>("StoreHDR")),
                    Asmac = SafeConvert.ToString(reader.ReadOrDefault<string>("Asmac2HDR")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("ValuDateHDR"))),
                    ReturnDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("HDateHDR"))),
                    Status = SafeConvert.ToString(reader.ReadOrDefault<string>("StatHDR")),
                    DiscountPercentage = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ComReducHDR")),
                    Paid = SafeConvert.ToSingle(reader.ReadOrDefault<float>("PaidHDR")),
                    VATRate = SafeConvert.ToSingle(reader.ReadOrDefault<float>("MAAMPHDR")),
                    Round = SafeConvert.ToSingle(reader.ReadOrDefault<float>("RoundReducHDR")),
                    UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDupdHDR"))),
                    SearchTable = searchParams.TableName
                };
            }

            PriceOfferBody ReadBody(OdbcDataReader reader)
            {
                return new PriceOfferBody
                {
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    ItemId = SafeConvert.ToString(reader["ItemNoBDY"]),
                    ItemName = SafeConvert.ToString(reader["ItemNameBDY"]),
                    Quantity = SafeConvert.ToSingle(reader["ItemQtyBDY"]),
                    UnitPriceBeforeVAT = SafeConvert.ToSingle(reader["ItemPrcBDY"]),
                    DiscountPercentage = SafeConvert.ToSingle(reader["ItemReducBDY"]),
                    SalePriceBeforeVAT = SafeConvert.ToSingle(reader["ItemSumBDY"]),

                    Barcode = SafeConvert.ToString(reader.ReadOrDefault<string>("BarcodeBDY")),
                    Auxcode = SafeConvert.ToString(reader.ReadOrDefault<string>("ChinesCodeBDY")),
                    Color = SafeConvert.ToString(reader.ReadOrDefault<string>("ColorBDY")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("MifratBDY")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDate1BDY"))),
                    Length = SafeConvert.ToSingle(reader.ReadOrDefault<float>("LENBDY")),
                    Width = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemWidthBDY")),
                    Thickness = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemThincknessBDY")),
                    Weight = SafeConvert.ToSingle(reader.ReadOrDefault<float>("WBDY")),
                    PackType = SafeConvert.ToString(reader.ReadOrDefault<string>("arizasugBDY")),
                    PacksInLot = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTBDY")),
                    UnitsInPack = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTQTYBDY")),
                    UnitPrice = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemPrc1BDY"))

                };
            }

            // ---

            try
            {
                var priceOffers = new List<PriceOffer>();

                // ignore tables search with updated-date filter but no updated-date column
                // to prevent receiving incorrect data
                var hasUpdDateFilter = searchParams.FromUpdateDate.HasValue || searchParams.ToUpdateDate.HasValue;
                if (searchParams.PauseIfNoUpdateDateColumn && !hasLDupdHDR && hasUpdDateFilter)
                    return priceOffers;

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;                    

                    command.CommandText = CreateHeaderQuery();

                    // load orders (header)
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            priceOffers.Add(new PriceOffer
                            { 
                                Header = ReadHeader(reader)
                            });

                    priceOffers.ForEach(priceOffer => {                        
                        command.CommandText = CreateBodyQuery(priceOffer.Header.DocumentId);

                        // load order (items)
                        using (var reader = command.ExecuteReader())
                            while (reader.Read())
                                priceOffer.Body.Add(ReadBody(reader));
                    });                    
                }

                return priceOffers;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<Invoice> FindInvoices(InvoiceSearchParams searchParams)
        {
            var hasStatHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "StatHDR");            

            // local methods 
            string CreateHeaderQuery()
            {
                var query = $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Hdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    AND     ({searchParams.FromDocumentId} <= 0 OR DOCCODE >= {searchParams.FromDocumentId})
                    AND     ({searchParams.ToDocumentId} <= 0 OR DOCCODE <= {searchParams.ToDocumentId})                    
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)} <= 0 OR DATEHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)})
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)} <= 0 OR DATEHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)})                                        
                ";

                if (hasStatHDR) query += $"AND     ('{searchParams.Status ?? ""}' = '' OR StatHDR = '{searchParams.Status}') ";
                query += "ORDER BY DATEHDR DESC";
                return query;
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Bdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            InvoiceHeader ReadHeader(OdbcDataReader reader)
            {
                return new InvoiceHeader
                {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                    DocNo = Convert.ToInt32(reader["DocNoHDR"]),
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = SafeConvert.ToString(reader["AccNameHDR"]),
                    Address = SafeConvert.ToString(reader["AccAddrHDR"]),
                    City = SafeConvert.ToString(reader.ReadOrDefault<string>("AccCityHDR")),
                    AgentId = Convert.ToInt32(reader["AgentHDR"]),
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    SubTotal = SafeConvert.ToSingle(reader["SubTotalHDR"]),
                    Discount = SafeConvert.ToSingle(reader["ReductionHDR"]),
                    TotalBeforeVat = SafeConvert.ToSingle(reader["BeforVatHDR"]),
                    Vat = SafeConvert.ToSingle(reader["VatSumHDR"]),
                    Total = SafeConvert.ToSingle(reader["GrandTotalHDR"]),
                    CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["DATEHDR"])),

                    Contact = SafeConvert.ToString(reader.ReadOrDefault<string>("Edit205HDR")),
                    Phone1 = SafeConvert.ToString(reader.ReadOrDefault<string>("AccPhone1HDR")),
                    Phone2 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone2HDR")),
                    Phone3 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone3HDR")),
                    Phone4 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone4HDR")),
                    SubjectId = Convert.ToInt32(reader.ReadOrDefault<int>("TrsDebitSubjectHDR")),
                    AgentName = SafeConvert.ToString(reader.ReadOrDefault<string>("AgNameHDR")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("AccRemarksHDR")),
                    StoreNo = Convert.ToInt32(reader.ReadOrDefault<int>("StoreHDR")),
                    Asmac = SafeConvert.ToString(reader.ReadOrDefault<string>("Asmac2HDR")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("ValuDateHDR"))),
                    ReturnDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("HDateHDR"))),
                    Status = SafeConvert.ToString(reader.ReadOrDefault<string>("StatHDR")),
                    DiscountPercentage = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ComReducHDR")),
                    Paid = SafeConvert.ToSingle(reader.ReadOrDefault<float>("PaidHDR")),
                    VATRate = SafeConvert.ToSingle(reader.ReadOrDefault<float>("MAAMPHDR")),
                    Round = SafeConvert.ToSingle(reader.ReadOrDefault<float>("RoundReducHDR")),
                    UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDupdHDR"))),
                    SearchTable = searchParams.TableName
                };
            }

            InvoiceBody ReadBody(OdbcDataReader reader)
            {
                return new InvoiceBody
                {
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    ItemId = SafeConvert.ToString(reader["ItemNoBDY"]),
                    ItemName = SafeConvert.ToString(reader["ItemNameBDY"]),
                    Quantity = SafeConvert.ToSingle(reader["ItemQtyBDY"]),
                    UnitPriceBeforeVAT = SafeConvert.ToSingle(reader["ItemPrcBDY"]),
                    DiscountPercentage = SafeConvert.ToSingle(reader["ItemReducBDY"]),
                    SalePriceBeforeVAT = SafeConvert.ToSingle(reader["ItemSumBDY"]),

                    Barcode = SafeConvert.ToString(reader.ReadOrDefault<string>("BarcodeBDY")),
                    Auxcode = SafeConvert.ToString(reader.ReadOrDefault<string>("ChinesCodeBDY")),
                    Color = SafeConvert.ToString(reader.ReadOrDefault<string>("ColorBDY")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("MifratBDY")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDate1BDY"))),
                    Length = SafeConvert.ToSingle(reader.ReadOrDefault<float>("LENBDY")),
                    Width = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemWidthBDY")),
                    Thickness = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemThincknessBDY")),
                    Weight = SafeConvert.ToSingle(reader.ReadOrDefault<float>("WBDY")),
                    PackType = SafeConvert.ToString(reader.ReadOrDefault<string>("arizasugBDY")),
                    PacksInLot = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTBDY")),
                    UnitsInPack = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTQTYBDY")),
                    UnitPrice = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemPrc1BDY"))
                };
            }

            // ---

            try
            {
                var invoices = new List<Invoice>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    command.CommandText = CreateHeaderQuery();

                    // load invoices (header)
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            invoices.Add(new Invoice
                            {
                                Header = ReadHeader(reader)
                            });

                    invoices.ForEach(invoice => {
                        command.CommandText = CreateBodyQuery(invoice.Header.DocumentId);

                        // load invoice (items)
                        using (var reader = command.ExecuteReader())
                            while (reader.Read())
                                invoice.Body.Add(ReadBody(reader));
                    });
                }

                return invoices;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<InvoiceReceipt> FindInvoiceReceipts(InvoiceReceiptSearchParams searchParams) {
            var hasStatHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "StatHDR");

            // local methods 
            string CreateHeaderQuery()
            {
                var query = $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Hdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    AND     ({searchParams.FromDocumentId} <= 0 OR DOCCODE >= {searchParams.FromDocumentId})
                    AND     ({searchParams.ToDocumentId} <= 0 OR DOCCODE <= {searchParams.ToDocumentId})                    
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)} <= 0 OR DATEHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)})
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)} <= 0 OR DATEHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)})                                        
                ";
                
                query += "ORDER BY DATEHDR DESC";
                return query;
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Bdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            InvoiceReceiptHeader ReadHeader(OdbcDataReader reader)
            {
                return new InvoiceReceiptHeader
                {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                    DocNo = Convert.ToInt32(reader["DocNoHDR"]),
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = SafeConvert.ToString(reader["AccNameHDR"]),
                    Address = SafeConvert.ToString(reader["AccAddrHDR"]),
                    City = SafeConvert.ToString(reader.ReadOrDefault<string>("AccCityHDR")),
                    AgentId = Convert.ToInt32(reader["AgentHDR"]),
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    SubTotal = SafeConvert.ToSingle(reader["SubTotalHDR"]),
                    Discount = SafeConvert.ToSingle(reader["ReductionHDR"]),
                    TotalBeforeVat = SafeConvert.ToSingle(reader["BeforVatHDR"]),
                    Vat = SafeConvert.ToSingle(reader["VatSumHDR"]),
                    Total = SafeConvert.ToSingle(reader["GrandTotalHDR"]),
                    CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["DATEHDR"])),

                    Contact = SafeConvert.ToString(reader.ReadOrDefault<string>("Edit205HDR")),
                    Phone1 = SafeConvert.ToString(reader.ReadOrDefault<string>("AccPhone1HDR")),
                    Phone2 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone2HDR")),
                    Phone3 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone3HDR")),
                    Phone4 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone4HDR")),
                    SubjectId = Convert.ToInt32(reader.ReadOrDefault<int>("TrsDebitSubjectHDR")),
                    AgentName = SafeConvert.ToString(reader.ReadOrDefault<string>("AgNameHDR")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("AccRemarksHDR")),
                    StoreNo = Convert.ToInt32(reader.ReadOrDefault<int>("StoreHDR")),
                    Asmac = SafeConvert.ToString(reader.ReadOrDefault<string>("Asmac2HDR")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("ValuDateHDR"))),
                    ReturnDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("HDateHDR"))),
                    Status = SafeConvert.ToString(reader.ReadOrDefault<string>("StatHDR")),
                    DiscountPercentage = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ComReducHDR")),
                    Paid = SafeConvert.ToSingle(reader.ReadOrDefault<float>("PaidHDR")),
                    VATRate = SafeConvert.ToSingle(reader.ReadOrDefault<float>("MAAMPHDR")),
                    Round = SafeConvert.ToSingle(reader.ReadOrDefault<float>("RoundReducHDR")),
                    UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDupdHDR"))),
                    SearchTable = searchParams.TableName
                };
            }

            InvoiceReceiptBody ReadBody(OdbcDataReader reader)
            {
                return new InvoiceReceiptBody
                {
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    ItemId = SafeConvert.ToString(reader["ItemNoBDY"]),
                    ItemName = SafeConvert.ToString(reader["ItemNameBDY"]),
                    Quantity = SafeConvert.ToSingle(reader["ItemQtyBDY"]),
                    UnitPriceBeforeVAT = SafeConvert.ToSingle(reader["ItemPrcBDY"]),
                    DiscountPercentage = SafeConvert.ToSingle(reader["ItemReducBDY"]),
                    SalePriceBeforeVAT = SafeConvert.ToSingle(reader["ItemSumBDY"]),

                    Barcode = SafeConvert.ToString(reader.ReadOrDefault<string>("BarcodeBDY")),
                    Auxcode = SafeConvert.ToString(reader.ReadOrDefault<string>("ChinesCodeBDY")),
                    Color = SafeConvert.ToString(reader.ReadOrDefault<string>("ColorBDY")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("MifratBDY")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDate1BDY"))),
                    Length = SafeConvert.ToSingle(reader.ReadOrDefault<float>("LENBDY")),
                    Width = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemWidthBDY")),
                    Thickness = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemThincknessBDY")),
                    Weight = SafeConvert.ToSingle(reader.ReadOrDefault<float>("WBDY")),
                    PackType = SafeConvert.ToString(reader.ReadOrDefault<string>("arizasugBDY")),
                    PacksInLot = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTBDY")),
                    UnitsInPack = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTQTYBDY")),
                    UnitPrice = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemPrc1BDY"))
                };
            }

            // ---

            try
            {
                var invoiceReceipts = new List<InvoiceReceipt>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    command.CommandText = CreateHeaderQuery();

                    // load invoices (header)
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            invoiceReceipts.Add(new InvoiceReceipt
                            {
                                Header = ReadHeader(reader)
                            });

                    invoiceReceipts.ForEach(invoiceReceipt => {
                        command.CommandText = CreateBodyQuery(invoiceReceipt.Header.DocumentId);

                        // load invoice (items)
                        using (var reader = command.ExecuteReader())
                            while (reader.Read())
                                invoiceReceipt.Body.Add(ReadBody(reader));
                    });
                }

                return invoiceReceipts;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<Receipt> FindReceipts(ReceiptSearchParams searchParams)
        {
            // local methods 
            string CreateHeaderQuery()
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Hdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    AND     ({searchParams.FromDocumentId} <= 0 OR DOCCODE >= {searchParams.FromDocumentId})
                    AND     ({searchParams.ToDocumentId} <= 0 OR DOCCODE <= {searchParams.ToDocumentId})
                    AND     ('{searchParams.Status ?? ""}' = '' OR StatusHDR = '{searchParams.Status}')
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)} <= 0 OR DATEHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)})
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)} <= 0 OR DATEHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)})
                    ORDER BY DATEHDR DESC
                ";
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Bdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            ReceiptHeader ReadHeader(OdbcDataReader reader)
            {
                return new ReceiptHeader
                {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),                    
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = SafeConvert.ToString(reader["NameHDR"]),
                    Address = SafeConvert.ToString(reader["AddressHDR"]),
                    City = SafeConvert.ToString(reader.ReadOrDefault<string>("CityHDR")),
                    Total = SafeConvert.ToSingle(reader["GnisTOTHDR"]),
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["DATEHDR"])),

                    Asmac = SafeConvert.ToString(reader.ReadOrDefault<string>("Asmacta2HDR")),
                    ValueDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("ComDateHDR"))),
                    SubjectId = Convert.ToInt32(reader.ReadOrDefault<int>("SubjectHDR")),                    
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("TrsDetailsHDR")),                                                                                          
                    Deduction = SafeConvert.ToSingle(reader.ReadOrDefault<float>("NikuyBamakorHDR")), 
                    CreditCardType = SafeConvert.ToString(reader.ReadOrDefault<string>("CreditTypeHDR")),
                    DealType = (SafeConvert.ToString(reader.ReadOrDefault<string>("DealTypeHDR"))).ToEnum<eReceiptDealType>(),
                    CreditCardOwnerId = SafeConvert.ToString(reader.ReadOrDefault<string>("CardOwnerIDHDR")),
                    CreditCardNumber = SafeConvert.ToString(reader.ReadOrDefault<string>("CreditNoHDR")),
                    CreditCardExpiry = SafeConvert.ToString(reader.ReadOrDefault<string>("TokefHDR")),
                    NumberOfPayments = Convert.ToInt32(reader.ReadOrDefault<int>("NoOfPayesHDR")),
                    FirstPaymentAmount = SafeConvert.ToSingle(reader.ReadOrDefault<float>("FirstPayHDR")),
                    EachPaymentAmount = SafeConvert.ToSingle(reader.ReadOrDefault<float>("OtherPayesHDR")),
                    ConfirmationNumber = Convert.ToInt32(reader.ReadOrDefault<int>("IshurNoHDR")),                    
                    SearchTable = searchParams.TableName                    
                };
            }

            ReceiptBody ReadBody(OdbcDataReader reader)
            {
                return new ReceiptBody
                {
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    TotalNIS = SafeConvert.ToSingle(reader["SumNisBDY"]),

                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("DetailsBDY")),
                    Total = SafeConvert.ToSingle(reader.ReadOrDefault<float>("SumBDY")),   
                    NumberOfPayments = Convert.ToInt32(reader.ReadOrDefault<int>("NoOfPaysBDY")),
                    PaymentDate = FromPervasiveDate(Convert.ToDouble(reader["vDateBDY"])),
                    PaymentType = Convert.ToInt32(reader.ReadOrDefault<int>("KupaNoBDY"))
                };
            }

            // ---

            try
            {
                var receipts = new List<Receipt>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    command.CommandText = CreateHeaderQuery();

                    // load invoices (header)
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            receipts.Add(new Receipt
                            {
                                Header = ReadHeader(reader)
                            });

                    receipts.ForEach(invoice => {
                        command.CommandText = CreateBodyQuery(invoice.Header.DocumentId);

                        // load invoice (items)
                        using (var reader = command.ExecuteReader())
                            while (reader.Read())
                                invoice.Body.Add(ReadBody(reader));
                    });
                }

                return receipts;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<DeliveryNote> FindDeliveryNotes(DeliveryNoteSearchParams searchParams)
        {
            var hasStatHDR = this.IsColumnExists($"{searchParams.TableName}Hdr", "StatHDR");
            ///var hasLDupdHDR = this.IsColumnExists($"{searchParams.TableName}hdr", "LDupdHDR");
            var isTableExists = this.IsTableExists($"{searchParams.TableName}Hdr");

            // local methods 
            string CreateHeaderQuery()
            {
                var query = $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Hdr
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNoHDR = {searchParams.AccountId})
                    AND     ({searchParams.FromDocumentId} <= 0 OR DOCCODE >= {searchParams.FromDocumentId})
                    AND     ({searchParams.ToDocumentId} <= 0 OR DOCCODE <= {searchParams.ToDocumentId})                    
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)} <= 0 OR DATEHDR >= {PervasiveDBHelper.ToPervasiveDate(searchParams.FromDate)})
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)} <= 0 OR DATEHDR <= {PervasiveDBHelper.ToPervasiveDate(searchParams.ToDate)})                    
                ";

                if (hasStatHDR) query += $"AND     ('{searchParams.Status ?? ""}' = '' OR StatHDR = '{searchParams.Status}') ";
                query += "ORDER BY DATEHDR DESC";
                return query;
            }

            string CreateBodyQuery(int documentId)
            {
                return $@"
                    SELECT  * 
                    FROM    {searchParams.TableName}Bdy 
                    WHERE   DOCCODE = {documentId} 
                    ORDER BY GridLineNo ASC
                ";
            }

            DeliveryNoteHeader ReadHeader(OdbcDataReader reader)
            {
                return new DeliveryNoteHeader
                {
                    DocumentId = Convert.ToInt32(reader["DOCCODE"]),
                    DocNo = Convert.ToInt32(reader["DocNoHDR"]),
                    AccountId = Convert.ToInt32(reader["AccNoHDR"]),
                    AccountName = SafeConvert.ToString(reader["AccNameHDR"]),
                    Address = SafeConvert.ToString(reader["AccAddrHDR"]),
                    City = SafeConvert.ToString(reader.ReadOrDefault<string>("AccCityHDR")),
                    AgentId = Convert.ToInt32(reader["AgentHDR"]),
                    SystemStatus = Convert.ToByte(reader["DocStatusHDR"]),
                    SubTotal = SafeConvert.ToSingle(reader["SubTotalHDR"]),
                    Discount = SafeConvert.ToSingle(reader["ReductionHDR"]),
                    TotalBeforeVat = SafeConvert.ToSingle(reader["BeforVatHDR"]),
                    Vat = SafeConvert.ToSingle(reader["VatSumHDR"]),
                    Total = SafeConvert.ToSingle(reader["GrandTotalHDR"]),
                    CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["DATEHDR"])),

                    Contact = SafeConvert.ToString(reader.ReadOrDefault<string>("Edit205HDR")),
                    Phone1 = SafeConvert.ToString(reader.ReadOrDefault<string>("AccPhone1HDR")),
                    Phone2 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone2HDR")),
                    Phone3 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone3HDR")),
                    Phone4 = SafeConvert.ToString(reader.ReadOrDefault<string>("Phone4HDR")),
                    SubjectId = Convert.ToInt32(reader.ReadOrDefault<int>("TrsDebitSubjectHDR")),
                    AgentName = SafeConvert.ToString(reader.ReadOrDefault<string>("AgNameHDR")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("AccRemarksHDR")),
                    StoreNo = Convert.ToInt32(reader.ReadOrDefault<int>("StoreHDR")),
                    Asmac = SafeConvert.ToString(reader.ReadOrDefault<string>("Asmac2HDR")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("ValuDateHDR"))),
                    ReturnDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("HDateHDR"))),
                    Status = SafeConvert.ToString(reader.ReadOrDefault<string>("StatHDR")),
                    DiscountPercentage = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ComReducHDR")),
                    Paid = SafeConvert.ToSingle(reader.ReadOrDefault<float>("PaidHDR")),
                    VATRate = SafeConvert.ToSingle(reader.ReadOrDefault<float>("MAAMPHDR")),
                    Round = SafeConvert.ToSingle(reader.ReadOrDefault<float>("RoundReducHDR")),
                    UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDupdHDR"))),
                    SearchTable = searchParams.TableName
                };
            }

            DeliveryNoteBody ReadBody(OdbcDataReader reader)
            {
                return new DeliveryNoteBody
                {
                    LineNo = Convert.ToInt32(reader["GridLineNo"]),
                    ItemId = SafeConvert.ToString(reader["ItemNoBDY"]),
                    ItemName = SafeConvert.ToString(reader["ItemNameBDY"]),
                    Quantity = SafeConvert.ToSingle(reader["ItemQtyBDY"]),
                    UnitPriceBeforeVAT = SafeConvert.ToSingle(reader["ItemPrcBDY"]),
                    DiscountPercentage = SafeConvert.ToSingle(reader["ItemReducBDY"]),
                    SalePriceBeforeVAT = SafeConvert.ToSingle(reader["ItemSumBDY"]),

                    Barcode = SafeConvert.ToString(reader.ReadOrDefault<string>("BarcodeBDY")),
                    Auxcode = SafeConvert.ToString(reader.ReadOrDefault<string>("ChinesCodeBDY")),
                    Color = SafeConvert.ToString(reader.ReadOrDefault<string>("ColorBDY")),
                    Notes = SafeConvert.ToString(reader.ReadOrDefault<string>("MifratBDY")),
                    ProvisionDate = FromPervasiveDate(Convert.ToDouble(reader.ReadOrDefault<double>("LDate1BDY"))),
                    Length = SafeConvert.ToSingle(reader.ReadOrDefault<float>("LENBDY")),
                    Width = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemWidthBDY")),
                    Thickness = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemThincknessBDY")),
                    Weight = SafeConvert.ToSingle(reader.ReadOrDefault<float>("WBDY")),
                    PackType = SafeConvert.ToString(reader.ReadOrDefault<string>("arizasugBDY")),
                    PacksInLot = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTBDY")),
                    UnitsInPack = SafeConvert.ToString(reader.ReadOrDefault<string>("ARIZOTQTYBDY")),
                    UnitPrice = SafeConvert.ToSingle(reader.ReadOrDefault<float>("ItemPrc1BDY"))
                };
            }

            // ---

            try
            {
                var deliveryNotes = new List<DeliveryNote>();
                if (!isTableExists) return deliveryNotes;  // No such table -> return;

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    command.CommandText = CreateHeaderQuery();

                    // load deliveryNotes (header)
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            deliveryNotes.Add(new DeliveryNote
                            {
                                Header = ReadHeader(reader)
                            });

                    deliveryNotes.ForEach(deliveryNote => {
                        command.CommandText = CreateBodyQuery(deliveryNote.Header.DocumentId);

                        // load deliveryNote (items)
                        using (var reader = command.ExecuteReader())
                            while (reader.Read())
                                deliveryNote.Body.Add(ReadBody(reader));
                    });
                }

                return deliveryNotes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        
        public IEnumerable<MismRowData> FindInMISM(MISMSearchParams searchParams)
        {
            // local methods 
            string CreateQuery()
            {               
                return $@"
                    SELECT  * 
                    FROM    MISM
                    WHERE   ({searchParams.AccountId} <= 0 OR AccNo = {searchParams.AccountId})                    
                    AND     ({searchParams.MismNo} <= 0 OR MismNo = {searchParams.MismNo})
                    AND     ('{searchParams.MismCode ?? ""}' = '' OR MismCode = '{searchParams.MismCode}')
                    AND     ('{searchParams.TableName ?? ""}' = '' OR DocCode = '{searchParams.TableName}')
                    AND     ({PervasiveDBHelper.ToPervasiveDate(searchParams.DocDate)} <= 0 OR Date = {PervasiveDBHelper.ToPervasiveDate(searchParams.DocDate)})
                    ORDER BY Date DESC
                ";
            }
           
            // ---

            try
            {
                var rows = new List<MismRowData>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    command.CommandText = CreateQuery();
                    
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            rows.Add(new MismRowData
                            {
                                AccountId = Convert.ToInt32(reader["AccNo"]),
                                AccountName = SafeConvert.ToString(reader["AccName"]),
                                TableName = SafeConvert.ToString(reader["DocCode"]),
                                MismCode = SafeConvert.ToString(reader["MismCode"]),
                                MismNo = Convert.ToInt32(reader["MismNo"]),
                                DocDate = FromPervasiveDate(Convert.ToDouble(reader["Date"])),
                                Total = Convert.ToSingle(reader["MismTotal"]),
                                Vat = Convert.ToSingle(reader["MismVAT"]),
                                ProducerName = SafeConvert.ToString(reader["UserCode"]),
                                DocPrintPattern = SafeConvert.ToString(reader["PrnDocCode"]),
                                AccountingCommandNo = Convert.ToInt32(reader["AccPkuda"]),
                                InventoryCommandNo = Convert.ToInt32(reader["InvPkuda"])                                
                            });

                }

                return rows;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> dynamic tableName
        public bool DeleteOrder(int Id)
        {
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

                    command.CommandText = $@"DELETE FROM order00hdr WHERE DOCCODE = {Id}";
                    command.ExecuteNonQuery();

                    command.CommandText = $@"DELETE FROM order00bdy WHERE DOCCODE = {Id}";
                    command.ExecuteNonQuery();

                    this.DeleteMISMRow(Id, eDocCode.Order, command);

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

        public bool DeleteOrderMas(int Id)
        {                       
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

                    command.CommandText = $@"DELETE FROM orderMashdr WHERE DOCCODE = {Id}";
                    command.ExecuteNonQuery();

                    command.CommandText = $@"DELETE FROM orderMasbdy WHERE DOCCODE = {Id}";
                    command.ExecuteNonQuery();

                    this.DeleteMISMRow(Id, eDocCode.OrderMas, command);

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
        
        public bool IsExistsOrder(int Id, string TableName = "Order00")
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT 1 FROM {TableName}hdr WHERE DOCCODE = {Id}";
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

        public int UpdateOrderCollected(OrderCollected orderCollected) {
            // local methods             
            string CreateQuery(int documentId, int LineNo, float Collected)
            {
                var query = $@"
                    UPDATE  {orderCollected.TableName}bdy
                    SET     NewQtyBDY = {Collected}
                    WHERE   DOCCODE = {documentId}
                    AND     GridLineNo = {LineNo}
                ";

                return query;
            }

            // ---

            OdbcTransaction transaction = null;
            
            try
            {
                var successCount = 0;
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();
                    
                    command.Connection = connection;
                    command.Transaction = transaction;

                    orderCollected.Body.ForEach(row => {
                        command.CommandText = CreateQuery(orderCollected.DocumentId, row.LineNo, row.Collected);
                        successCount += command.ExecuteNonQuery();
                    });

                    transaction.Commit();
                }

                return successCount;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // TODO ->> Complete + BLL + Presentation
        public bool UpdateMISMRowURL(int MismNo, string MismCode, string DocumentURL)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        UPDATE  MISM
                        SET     XXX = '{DocumentURL}'
                        WHERE   MismNo = {MismNo} AND MismCode = '{MismCode}'
                    ";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // -----

        public float GetVATRate() {
            try {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT TOP 1 VatPercent FROM compdet WHERE recno=1";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    var value = Convert.ToSingle(command.ExecuteScalar());
                    if (value == 0) return 0;
                    ///throw new Exception("VAT value can't be 0"); 
                    
                    return value / 100;  // e.g: 0.17
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public int GetOpAcc(int recNo) {
            try {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT Value FROM CompAcc WHERE RecNo={recNo}";
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
       
        [Obsolete("Use ServicesProxy.AccountsProxy Instead")]
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

        public int GetNextDocCounter(eCounterNo counterNo, bool CreateIfNotExists = true) {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();

                var command = new OdbcCommand();
                command.Connection = connection;

                var value = this.GetNextDocCounter(counterNo, command);

                // no value, try to create the missing counter and try again
                if (value == 0 && CreateIfNotExists)
                {
                    this.CreateDocCounter(counterNo, 1, command);
                    value = this.GetNextDocCounter(counterNo, command);
                }

                if (value == 0) throw new Exception("No Counter Found!");
                return value;
            }
        }

        public bool SetNextDocCounter(eCounterNo counterNo)
        {
            using (var connection = new OdbcConnection(this.ConnetionString)) {
                connection.Open();

                var command = new OdbcCommand();
                command.Connection = connection;

                return this.SetNextDocCounter(counterNo, command);
            }
        }
        
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

        public bool CreateDocCounter(eCounterNo counterNo, int InitValue)
        {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();

                var command = new OdbcCommand();
                command.Connection = connection;

                return this.CreateDocCounter(counterNo, InitValue, command);
            }
        }

        public Configuration GetConfiguration()
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT * FROM CompDet";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader()) {
                        reader.Read();

                        return new Configuration {
                            ApiUserName = SafeConvert.ToString(reader["VendorName"]),
                            ApiUserKey = SafeConvert.ToString(reader["VendorKey"]),                            
                            ApiVersion = SafeConvert.ToInt32(reader["ApiVER"]),
                            CompanyName = SafeConvert.ToString(reader["name"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        
        public CompanyInfo GetCompanyInfo() {
            try
            {
                var companyInfo = new CompanyInfo();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT * FROM CompLines";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                        while (reader.Read()) {
                            var lineId = Convert.ToInt32(reader["RecNo"]);
                            companyInfo.CompanyLines[lineId] = new CompanyLine
                            {
                                LineId = lineId,
                                LineDesc = SafeConvert.ToString(reader["Desc"]),
                                LineToPrint = SafeConvert.ToString(reader["LineToPrint"]),
                            };
                        }
                }

                return companyInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public HashSet<string> GetTablesInMISM(string MismCode)
        {
            // local methods 
            string CreateQuery()
            {
                return $@"
                    SELECT  DISTINCT DocCode 
                    FROM    MISM 
                    WHERE   MismCode = '{MismCode}'
                ";
            }

            // ---

            try
            {
                var tableNames = new HashSet<string>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    command.CommandText = CreateQuery();

                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            tableNames.Add(SafeConvert.ToString(reader["DocCode"]));

                }

                return tableNames;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public KupaMappingData GetKUPA(int PaymentType)
        {
            var kupaName = this.PaymentType2KupaName(PaymentType);
            if (string.IsNullOrEmpty(kupaName)) return null;

            try {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT * FROM Absents WHERE details = '{SafeConvert.ToPervasiveString(kupaName)}'";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new KupaMappingData
                            {
                                KupaNo = SafeConvert.ToInt32(reader["qty"]),
                                KupaName = SafeConvert.ToString(reader["details"]),
                                AccountNo = SafeConvert.ToInt32(reader["date"])
                            };
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public string PaymentType2KupaName(int PaymentType)
        {
            return ((ePaymentType)PaymentType).GetDescription();
        }

        // -----

        private int GetNextDocCounter(eCounterNo counterNo, OdbcCommand command)
        {
            try
            {                
                var query = $@"SELECT Value FROM DocsCounters WHERE counterno={(int)counterNo}";
                command.CommandText = query;
                return Convert.ToInt32(command.ExecuteScalar());                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

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

        private bool CreateDocCounter(eCounterNo counterNo, int InitValue, OdbcCommand command)
        {
            try
            {                
                var query = $@"
                    INSERT INTO DocsCounters
                    (
                        CounterNo, 
                        Value, 
                        ""Desc"", 
                        DocCod, 
                        Flag, 
                        Copies, 
                        HaveLogo, 
                        CheckAlut, 
                        CheckQty, 
                        Manual, 
                        QtyValue, 
                        AuxFlag1, 
                        AuxFlag2, 
                        AuxFlag3, 
                        AuxFlag4, 
                        AuxFlag5, 
                        SugTnua
                    )    
                    VALUES
                    (
                        {(int)counterNo}, 
                        {InitValue}, 
                        '', '', '', 1, 0, 0, 0, 0, '', 0, 0, 0, 0, 0, ''
                    )";

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

        private bool UpdateAccountingTransactionTotals(int accountId, OdbcCommand command)
        {
            try
            {
                if (accountId <= 0) 
                    return false;

                var query1 = $@"
                    UPDATE	accounts 
                    SET		TotalCredit = (SELECT IFNULL(sum(""SUM""), 0) FROM acctrs WHERE accno={accountId} AND CreditDebit in ('ז')), 
                            TotalDebit =  (SELECT IFNULL(sum(""SUM""), 0) FROM acctrs WHERE accno={accountId} AND CreditDebit in ('ח'))		                        
                    WHERE   accno={accountId}
                ";

                var query2 = $@"
                    UPDATE	accounts 
                    SET		grandtotal = ((openbalance + TotalCredit) - TotalDebit)	                        
                    WHERE   accno={accountId}
                ";

                command.CommandText = query1;
                var result1 = command.ExecuteNonQuery();

                command.CommandText = query2;
                var result2 = command.ExecuteNonQuery();

                return (result1 + result2) > 0;
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

        // תנועות הנהלת חשבונות
        private bool CreateAccountingTransaction(AccountingTransaction transaction, OdbcCommand command)
        {            
            try
            {
                var query = $@"
                        INSERT INTO AccTrs
                        (
                            AccNo,
                            Asmac1,
                            Asmac2,
                            OpAcc,
                            Date,
                            ValueDate,
                            Subject,
                            ""Sum"",
                            FcSum,
                            Qty,
                            VatCode,
                            CreditDebit,
                            DocCode,
                            Details,
                            TeumCode,
                            TeumSum,
                            TeumFull,
                            EventDate,
                            AuxSum,
                            AuxCode,
                            AuxDate,
                            Pkuda,
                            TrsNo
                        )                        
                        (
                            SELECT 
                            {transaction.AccountId},                             
                            '{transaction.Asmac1}', 
                            '{transaction.Asmac2}', 
                            {transaction.OpAcc},  
                            {PervasiveDBHelper.ToPervasiveDate(transaction.PaidDate)},  
                            {PervasiveDBHelper.ToPervasiveDate(transaction.ValueDate)}, 
                            '{SafeConvert.ToPervasiveString(transaction.Subject)}', 
                            {transaction.Total}, 
                            {transaction.FcTotal}, 
                            {transaction.Quantity}, 
                            '{transaction.VatCode}', 
                            '{transaction.CreditDebit}', 
                            '{transaction.DocumentCode}', 
                            '{SafeConvert.ToPervasiveString(transaction.Notes)}', 
                            {transaction.TaxCoordCode}, 
                            {transaction.TaxCoordSum}, 
                            {(transaction.IsTaxCoordCompleted ? 1 : 0)}, 
                            {PervasiveDBHelper.ToPervasiveDate(transaction.EventDate)}, 
                            {transaction.AuxSum}, 
                            '{transaction.AuxCode}', 
                            {PervasiveDBHelper.ToPervasiveDate(transaction.AuxDate)}, 
                            {transaction.CommandNo}, 
                            {transaction.TransactionId}                                                      
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
                            '{transaction.DocumentCode}',
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

        // פעולות קופה - עדכון סכום כללי   
        private bool UpdateKupaCash(int KupaNo, float Amount, OdbcCommand command)
        {
            try
            {
                var query = $@"UPDATE KupaHdr set MEZUMAN = MEZUMAN + {Amount} Where KupaNo={KupaNo}";

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
        
        // פעולות קופה - הוספת שורה
        private bool CreateKupaRow(int LineNo, KupaBody kupaBody, OdbcCommand command) 
        {
            try
            {
                var query = $@"
                        INSERT INTO KupaBdy
                        (
                            RecNo,
                            KupaNo,
                            ValueDate,
                            ReciptDate,
                            AccNo,
                            DocNo,
                            ChequeNo,
                            BankCode,
                            BranchCode,
                            BankAcc,
                            Mean,
                            Details,
                            NoOfPays,
                            ""Sum""
                        )                        
                        (
                            SELECT 
                            {LineNo},
                            {kupaBody.KupaNo},
                            {PervasiveDBHelper.ToPervasiveDate(kupaBody.ValueDate ?? DateTime.Now)},
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)},
                            {kupaBody.AccountId},
                            {kupaBody.DocumentId},
                            '{kupaBody.PaycheckNo}',
                            {kupaBody.BankCode},
                            {kupaBody.BranchCode},
                            '{kupaBody.BankAccount}',
                            '{SafeConvert.ToPervasiveString(this.PaymentType2KupaName(kupaBody.PaymentType))}',
                            '{SafeConvert.ToPervasiveString(kupaBody.Notes)}',
                            {kupaBody.NumberOfPayments},
                            {kupaBody.Total}
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

        /*        
        public bool MISMRowExists(MismRowData rowData)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT 1 FROM MISM WHERE MismNo={rowData.MismNo}";
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
        */

        // עדכון MISM
        private bool CreateMISMRow(MismRowData rowData, OdbcCommand command)
        {
            try
            {                
                var query = $@"
                        INSERT INTO MISM
                        (
                            MismCode,
                            MismNo,
                            DocCode,
                            Date,
                            AccNo,
                            AccPkuda,
                            InvPkuda,
                            AccName,
                            MismTotal,
                            UserCode,
                            MismVAT,
                            PrnDocCode
                        )                        
                        (
                            SELECT 
                            '{rowData.MismCode}',
                            {rowData.MismNo},
                            '{rowData.TableName}',
                            {PervasiveDBHelper.ToPervasiveDate(rowData.DocDate)},
                            {rowData.AccountId},
                            {rowData.AccountingCommandNo},
                            {rowData.InventoryCommandNo},
                            '{rowData.AccountName}',
                            {rowData.Total},
                            '{rowData.ProducerName}',
                            {rowData.Vat},
                            '{rowData.DocPrintPattern}'
                        )
                    ";

                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"MismNo #{rowData.MismNo} | {ex.Message}");
            }
        }

        private bool DeleteMISMRow(int MismNo, string MismCode, OdbcCommand command)
        {
            try
            {                
                var query = $@"DELETE FROM MISM WHERE MismNo = {MismNo} AND MismCode = '{MismCode}'";

                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"MismNo #{MismNo} | {ex.Message}");
            }
        }

        private bool IsTableExists(string tableName)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@" SELECT 1 FROM X$File WHERE Xf$Name = '{tableName}'";

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
