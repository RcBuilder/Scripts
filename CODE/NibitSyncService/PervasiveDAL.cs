using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers.PervasiveDBHelper;

namespace NibitSyncService
{
    public class PervasiveDAL
    {
        protected string ConnetionString { get; set; }

        public PervasiveDAL(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }

        public bool CreateAccountingTransactions(MoveinRow DataRow)
        {
            OdbcTransaction transaction = null;

            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString)) {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    var command = new OdbcCommand();

                    command.Connection = connection;
                    command.Transaction = transaction;

                    int identity = (1000 * this.GetNextDocCounter(command, eCounterNo.AccountingTransactionMovin));                    

                    var transactionId = this.GetNextTransactionId(command);

                    // -- Create Accounting-Transactions --
                    
                    if (DataRow.AccountDebit1 > 0)
                    {
                        // (1)
                        this.CreateAccountingTransaction(command, new AccountingTransaction
                        {
                            CommandNo = ++identity,
                            TransactionId = transactionId,
                            AccountId = DataRow.AccountDebit1,
                            Asmac1 = DataRow.Asmac1,
                            Asmac2 = DataRow.Asmac2,
                            PaidDate = DataRow.AsmacDate,
                            ValueDate = DataRow.ValueDate,
                            Subject = "",
                            Total = DataRow.SumDebit1,
                            CreditDebit = "ח",                                
                            DocumentCode = "1",
                            Notes = DataRow.Details,
                            IsTaxCoordCompleted = false,
                            EventDate = DataRow.AsmacDate,
                        });
                    }
                    
                    if (DataRow.AccountDebit2 > 0)
                    {
                        // (2)
                        this.CreateAccountingTransaction(command, new AccountingTransaction
                        {
                            CommandNo = ++identity,
                            TransactionId = transactionId,
                            AccountId = DataRow.AccountDebit2,
                            Asmac1 = DataRow.Asmac1,
                            Asmac2 = DataRow.Asmac2,
                            PaidDate = DataRow.AsmacDate,
                            ValueDate = DataRow.ValueDate,
                            Subject = "",
                            Total = DataRow.SumDebit2,
                            CreditDebit = "ח",                                
                            DocumentCode = "1",
                            Notes = DataRow.Details,
                            IsTaxCoordCompleted = false,
                            EventDate = DataRow.AsmacDate,
                        });
                    }
                    
                    if (DataRow.AccountCrebit1 > 0)
                    {
                        // (3)
                        this.CreateAccountingTransaction(command, new AccountingTransaction
                        {
                            CommandNo = ++identity,
                            TransactionId = transactionId,
                            AccountId = DataRow.AccountCrebit1,
                            Asmac1 = DataRow.Asmac1,
                            Asmac2 = DataRow.Asmac2,
                            PaidDate = DataRow.AsmacDate,
                            ValueDate = DataRow.ValueDate,
                            Subject = "",
                            Total = DataRow.SumCrebit1,
                            CreditDebit = "ז",                                
                            DocumentCode = "1",
                            Notes = DataRow.Details,
                            IsTaxCoordCompleted = false,
                            EventDate = DataRow.AsmacDate,
                        });
                    }

                    if (DataRow.AccountCrebit2 > 0)
                    {
                        // (4)
                        this.CreateAccountingTransaction(command, new AccountingTransaction
                        {
                            CommandNo = ++identity,
                            TransactionId = transactionId,
                            AccountId = DataRow.AccountCrebit2,
                            Asmac1 = DataRow.Asmac1,
                            Asmac2 = DataRow.Asmac2,
                            PaidDate = DataRow.AsmacDate,
                            ValueDate = DataRow.ValueDate,
                            Subject = "",
                            Total = DataRow.SumCrebit2,
                            CreditDebit = "ז",                                
                            DocumentCode = "1",
                            Notes = DataRow.Details,
                            IsTaxCoordCompleted = false,
                            EventDate = DataRow.AsmacDate,
                        });
                    }

                    this.SetNextDocCounter(command, eCounterNo.AccountingTransactionMovin);

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message}");
            }
        }

        // תנועות הנהלת חשבונות
        private bool CreateAccountingTransaction(OdbcCommand command, AccountingTransaction transaction)
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

        private int GetNextDocCounter(OdbcCommand command, eCounterNo counterNo)
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

        private bool SetNextDocCounter(OdbcCommand command, eCounterNo counterNo)
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

        private int GetNextTransactionId(OdbcCommand command, eTransactionProvider provider = eTransactionProvider.AccTrs)
        {
            try
            {
                var query = $"SELECT TrsNo FROM {provider.ToString()} ORDER BY TrsNo DESC";
                command.CommandText = query;
                return Convert.ToInt32(command.ExecuteScalar()) + 1;                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
