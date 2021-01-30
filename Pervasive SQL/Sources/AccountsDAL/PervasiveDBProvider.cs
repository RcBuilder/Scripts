using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;

namespace AccountsDAL
{
    public class PervasiveDBProvider : IAccountsDAL
    {
        protected string ConnetionString { get; set; }
        public PervasiveDBProvider(string ConnetionString) {
            this.ConnetionString = ConnetionString;
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
                            '{PervasiveDBHelper.FixHebrewWithNumbers(account.Name)}',
                            '{PervasiveDBHelper.FixHebrewWithNumbers(account.Address)}', 
                            '{account.City}', 
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
                            Name = '{PervasiveDBHelper.FixHebrewWithNumbers(account.Name)}',
                            Address = '{PervasiveDBHelper.FixHebrewWithNumbers(account.Address)}', 
                            City = '{account.City}', 
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
                        AND   ('{searchParams.OsekNo ?? ""}' = '' OR OsekNo = '{searchParams.OsekNo}')        
                    ";

                    var command = new OdbcCommand(query);
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            /*                                                 	
                                -- PervasiveDBHelper.FixHebrewWithNumbers:
                                Name
                                Address
                            */
                            accounts.Add(new Account
                            {
                                Id = Convert.ToInt32(reader["AccNo"]),
                                ProviderId = reader["AuxAccNo"].ToString().Trim(),
                                Name = reader["Name"].ToString().Trim(),
                                Address = reader["Address"].ToString().Trim(),
                                City = reader["City"].ToString().Trim(),
                                ZipCode = reader["ZipCode"].ToString().Trim(),
                                ComercialReduc= Convert.ToSingle(reader["Reduc100"]) / 100,
                                CreditLevel = Convert.ToSingle(reader["CreditLevel"]),
                                Email = reader["Email"].ToString().Trim(),
                                OsekNo = reader["OsekNo"].ToString().Trim(),
                                Fax = reader["Fax"].ToString().Trim(),
                                Phone1 = reader["Phone1"].ToString().Trim(),
                                Phone2 = reader["Phone2"].ToString().Trim(),
                                SortCode1 = Convert.ToInt32(reader["SortCode1"]),
                                SortCode2 = Convert.ToInt32(reader["SortCode2"]),
                                SortCode3 = Convert.ToInt32(reader["SortCode34"]),
                                SortCode4 = 0,
                                SortCode5 = Convert.ToInt32(reader["SortCode5"]),
                                SortCode6 = Convert.ToInt32(reader["SortCode6"]),
                                VatFlag = reader["VatFlag"].ToString().Trim(),
                                CreatedDate = PervasiveDBHelper.FromPervasiveDate(Convert.ToDouble(reader["Cdate"])),
                                UpdatedDate = PervasiveDBHelper.FromPervasiveDate(Convert.ToDouble(reader["Udate"]))                                                                
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
    }
}