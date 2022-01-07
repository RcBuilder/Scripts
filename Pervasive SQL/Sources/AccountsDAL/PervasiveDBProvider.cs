using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using static Helpers.PervasiveDBHelper;

namespace AccountsDAL
{
    public class PervasiveDBProvider : IAccountsDAL
    {
        protected string ConnetionString { get; set; }
        public PervasiveDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }

        public Account GetAccount(int id) {
            try
            {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"SELECT * FROM Accounts WHERE AccNo = {id}";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) return null;                            
                        reader.Read();                        

                        return new Account
                        {
                            Id = Convert.ToInt32(reader["AccNo"]),
                            ProviderId = SafeConvert.ToString(reader["AuxAccNo"]),
                            Name = SafeConvert.ToString(reader["Name"]),
                            Address = SafeConvert.ToString(reader["Address"]),
                            City = SafeConvert.ToString(reader["City"]),
                            ZipCode = SafeConvert.ToString(reader["ZipCode"]),
                            ComercialReduc = Convert.ToSingle(reader["Reduc100"]) / 100,
                            CreditLevel = Convert.ToSingle(reader["CreditLevel"]),
                            Email = SafeConvert.ToString(reader["Email"]),
                            OsekNo = SafeConvert.ToString(reader["OsekNo"]),
                            Fax = SafeConvert.ToString(reader["Fax"]),
                            Phone1 = SafeConvert.ToString(reader["Phone1"]),
                            Phone2 = SafeConvert.ToString(reader["Phone2"]),
                            SortCode1 = Convert.ToInt32(reader["SortCode1"]),
                            SortCode2 = Convert.ToInt32(reader["SortCode2"]),
                            SortCode3 = Convert.ToInt32(reader["SortCode34"]),
                            SortCode4 = 0,
                            SortCode5 = Convert.ToInt32(reader["SortCode5"]),
                            SortCode6 = Convert.ToInt32(reader["SortCode6"]),
                            VatFlag = SafeConvert.ToString(reader["VatFlag"]),
                            CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["Cdate"])),
                            UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader["Udate"])),
                            RecurringPayment = Convert.ToSingle(reader["CreditDays"]),
                            Balance = Convert.ToSingle(reader["GrandTotal"]),                            
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
        
        public int CreateAccount(Account account) {
            try
            {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        INSERT INTO Accounts
                        (
                            AccNo,                            
                            AuxAccNo,
                            Name,
                            Address, 
                            City, 
                            ZipCode,
                            Reduc100,                             
                            CreditLevel,
                            Email,                                                 
                            OsekNo,
                            Fax,       
                            Phone1,
                            Phone2,                            
                            SortCode1,
                            SortCode2,
                            SortCode34,                            
                            SortCode5,
                            SortCode6,                            
                            VatFlag,                            
                            Cdate, 
                            Udate
                        )                        
                        (
                            SELECT 
                            {account.Id},                            
                            '{account.ProviderId}',
                            '{SafeConvert.ToPervasiveString(account.Name)}',
                            '{SafeConvert.ToPervasiveString(account.Address)}', 
                            '{SafeConvert.ToPervasiveString(account.City)}', 
                            '{account.ZipCode}',
                            {account.ComercialReduc * 100},                             
                            {account.CreditLevel},
                            '{account.Email}',                                                 
                            '{account.OsekNo}',
                            '{account.Fax}',       
                            '{account.Phone1}',
                            '{account.Phone2}',                            
                            {account.SortCode1},
                            {account.SortCode2},
                            {account.SortCode3},                            
                            {account.SortCode5},
                            {account.SortCode6},                            
                            '{account.VatFlag}',                            
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}, 
                            {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}
                        )
                    ";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    command.ExecuteNonQuery();                    
                }

                return account.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public int UpdateAccount(Account account)
        {
            try
            {                
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        UPDATE Accounts
                        SET
                            AuxAccNo = '{account.ProviderId}',
                            Name = '{SafeConvert.ToPervasiveString(account.Name)}',
                            Address = '{SafeConvert.ToPervasiveString(account.Address)}', 
                            City = '{SafeConvert.ToPervasiveString(account.City)}', 
                            ZipCode = '{account.ZipCode}',
                            Reduc100 = {account.ComercialReduc * 100},                             
                            CreditLevel = {account.CreditLevel},
                            Email = '{account.Email}',                                                 
                            OsekNo = '{account.OsekNo}',
                            Fax = '{account.Fax}',       
                            Phone1 = '{account.Phone1}',
                            Phone2 = '{account.Phone2}',                            
                            SortCode1 = {account.SortCode1},
                            SortCode2 = {account.SortCode2},
                            SortCode34 = {account.SortCode3},                            
                            SortCode5 = {account.SortCode5},
                            SortCode6 = {account.SortCode6},                            
                            VatFlag = '{account.VatFlag}',                            
                            Udate = {PervasiveDBHelper.ToPervasiveDate(DateTime.Now)}
                        WHERE AccNo = {account.Id}
                    ";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    command.ExecuteNonQuery();                    
                }

                return account.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<Account> Find(AccountSearchParams searchParams) {
            try
            {
                var accounts = new List<Account>();

                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        SELECT  * 
                        FROM    Accounts
                        WHERE   ({searchParams.AccountId} <= 0 OR AccNo = {searchParams.AccountId})        
                        AND     ('{searchParams.OsekNo ?? ""}' = '' OR OsekNo = '{searchParams.OsekNo}')    
                        AND     ({searchParams.AccountType} <= 0 OR (AccNo BETWEEN {searchParams.IdentityRange?.From ?? 0} AND {searchParams.IdentityRange?.To ?? 0}))
                        AND     ({ PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)} <= 0 OR Udate >= { PervasiveDBHelper.ToPervasiveDate(searchParams.FromUpdateDate)})
                        ORDER BY Udate DESC
                    ";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {      
                            accounts.Add(new Account
                            {
                                Id = Convert.ToInt32(reader["AccNo"]),                                   
                                ProviderId = SafeConvert.ToString(reader["AuxAccNo"]),
                                Name = SafeConvert.ToString(reader["Name"]),
                                Address = SafeConvert.ToString(reader["Address"]),
                                City = SafeConvert.ToString(reader["City"]),
                                ZipCode = SafeConvert.ToString(reader["ZipCode"]),
                                ComercialReduc= Convert.ToSingle(reader["Reduc100"]) / 100,
                                CreditLevel = Convert.ToSingle(reader["CreditLevel"]),
                                Email = SafeConvert.ToString(reader["Email"]),
                                OsekNo = SafeConvert.ToString(reader["OsekNo"]),
                                Fax = SafeConvert.ToString(reader["Fax"]),
                                Phone1 = SafeConvert.ToString(reader["Phone1"]),
                                Phone2 = SafeConvert.ToString(reader["Phone2"]),
                                SortCode1 = Convert.ToInt32(reader["SortCode1"]),
                                SortCode2 = Convert.ToInt32(reader["SortCode2"]),
                                SortCode3 = Convert.ToInt32(reader["SortCode34"]),
                                SortCode4 = 0,
                                SortCode5 = Convert.ToInt32(reader["SortCode5"]),
                                SortCode6 = Convert.ToInt32(reader["SortCode6"]),
                                VatFlag = SafeConvert.ToString(reader["VatFlag"]),
                                CreatedDate = FromPervasiveDate(Convert.ToDouble(reader["Cdate"])),
                                UpdatedDate = FromPervasiveDate(Convert.ToDouble(reader["Udate"]))                                                                
                            });
                        }
                    }

                    return accounts;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        // -----

        public bool AccountExists(int accountId)
        {
            try
            {
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

        public bool UpdateAccountingTransactionTotals(int accountId)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                        UPDATE	accounts 
                        SET		TotalCredit = (SELECT IFNULL(sum(""SUM""), 0) FROM acctrs WHERE accno={accountId} AND CreditDebit in ('†')), 
                                TotalDebit =  (SELECT IFNULL(sum(""SUM""), 0) FROM acctrs WHERE accno={accountId} AND CreditDebit in ('‡'))		                        
                        WHERE   accno={accountId}

                        UPDATE	accounts 
                        SET		grandtotal = ((openbalance + TotalCredit) - TotalDebit)	                        
                        WHERE   accno={accountId}
                    ";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /*
        public int GetNextAccountId(int accountType)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                            SELECT  AccNo 
                            FROM    Accounts 
                            WHERE   (PrcNo1 = {accountType}) 
                                    OR 
                                    ({accountType} = {(int)eAccountType.ProjectAccount} AND PrcNo1 = '') 
                            ORDER BY AccNo DESC";
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
        */

        public int GetNextAccountId(IdentityRange range) {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $@"
                            SELECT  AccNo 
                            FROM    Accounts 
                            WHERE   (AccNo BETWEEN {range.From} AND {range.To})
                            ORDER BY AccNo ASC";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    var ids = new List<int>();
                    using (var reader = command.ExecuteReader())                    
                        while (reader.Read())                        
                            ids.Add(Convert.ToInt32(reader["AccNo"]));

                    var nextId = -1;

                    // take next id
                    var maxId = ids.Max();
                    if (maxId < range.To)
                        nextId = maxId + 1;
                    else {
                        // find value in between
                        for (var i = 0; i < ids.Count - 1; i++) {
                            if (ids[i] + 1 == ids[i + 1]) continue;
                            nextId = ids[i] + 1;
                            break;
                        }
                    }

                    if (nextId == -1)
                        throw new Exception($"No available id for range {range.From} - {range.To}");

                    return nextId;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IdentityRange GetIdentityRange(int accountType)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var query = $"SELECT * FROM AccSortCodes WHERE Code = {accountType}";
                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) {

                            reader.Read();
                            return new IdentityRange
                            {
                                Code = Convert.ToInt32(reader["Code"]),
                                Desc = SafeConvert.ToString(reader["Desc"]),
                                From = Convert.ToInt32(reader["From"]),
                                To = Convert.ToInt32(reader["To"])
                            };
                        }

                        return null;
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        
        public string AccountType2AccountName(int accountType)
        {
            return ((eAccountType)accountType).GetDescription();
        }

        // -----
    }
}