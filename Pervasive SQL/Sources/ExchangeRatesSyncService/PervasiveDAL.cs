using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExchangeRates.GOVExchangeRatesManager;
using static Helpers.PervasiveDBHelper;

namespace ExchangeRatesSyncService
{
    public class PervasiveBaseDAL
    {
        protected string ConnetionString { get; set; }

        public PervasiveBaseDAL(string ConnetionString) {
            this.ConnetionString = ConnetionString;
        }

        public string ExecuteAsJson(string query) {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();
                var command = new OdbcCommand();
                command.Connection = connection;
                return this.ExecuteAsJson(query, command);
            }
        }

        public string ExecuteAsJson(string query, OdbcCommand command)
        {
            var step = 0;
            try
            {
                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                {
                    var sJsonResult = this.ReadAsJson(reader);
                    return sJsonResult;
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step {step})");
            }
        }

        public DataTable ExecuteAsDataTable(string query)
        {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();
                var command = new OdbcCommand();
                command.Connection = connection;
                return this.ExecuteAsDataTable(query, command);
            }
        }

        public DataTable ExecuteAsDataTable(string query, OdbcCommand command)
        {
            var step = 0;
            try
            {
                command.CommandText = query;

                var dt = new DataTable();
                using (var adapter = new OdbcDataAdapter(command))
                    adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step {step})");
            }
        }

        public object ExecuteAsScalar(string query)
        {
            using (var connection = new OdbcConnection(this.ConnetionString))
            {
                connection.Open();
                var command = new OdbcCommand();
                command.Connection = connection;
                return this.ExecuteAsScalar(query, command);
            }
        }

        public object ExecuteAsScalar(string query, OdbcCommand command)
        {
            var step = 0;
            try
            {
                command.CommandText = query;
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception($"{ex.Message} (step {step})");
            }
        }

        protected string ReadAsJson(OdbcDataReader dr)
        {
            if (dr == null || !dr.HasRows) return "[]";

            var results = new List<Dictionary<string, string>>();
            while (dr.Read())
            {
                var result = new Dictionary<string, string>();
                for (var i = 0; i < dr.VisibleFieldCount; i++)
                    result.Add(dr.GetName(i), SafeConvert.ToString(dr[i]));
                results.Add(result);
            }

            return JsonConvert.SerializeObject(results);
        }
    }

    public class PervasiveDAL : PervasiveBaseDAL
    {
        public PervasiveDAL(string ConnetionString) : base(ConnetionString) { }

        public bool SaveExchangeRate(ExchangeRate rate)
        {
            try
            {
                using (var connection = new OdbcConnection(this.ConnetionString))
                {
                    connection.Open();
                    var command = new OdbcCommand();
                    command.Connection = connection;

                    /*
                        -- SAVE (CREATE OR UPDATE)
                        IF NOT EXISTS(SELECT 1 FROM <table> WHERE Id = @Id) -- CREATE --
                        BEGIN 
	                        INSERT INTO <table>(<column-1>, <column-2>...)
	                        VALUES (<value-1>, <value-1>...)

	                        SET @Id = SCOPE_IDENTITY();
                        END 

                        -- UPDATE --
                        UPDATE	<table>
                        SET		<column-1> = <value-1>,
		                        <column-2> = <value-2>
		                        ...
		                        ...
                        WHERE	Id = @Id
	
                        SELECT @Id as 'Res' 
                    */

                    var currencyId = CurrencyCode2CurrencyId(rate.CurrencyCodeDest);
                    var dateNum = PervasiveDBHelper.ToPervasiveDate(rate.UpdatedDate.Date);

                    var existsQuery = $@"
                        SELECT  1 FROM FCBDY                       
                        WHERE   CODE = {currencyId} AND DATE = {dateNum}
                    ";
                    command.CommandText = existsQuery;
                    var isExists = Convert.ToInt32(command.ExecuteScalar()) == 1;


                    var updateQuery = $@"
                        UPDATE  FCBDY
                        SET     VALUE = {rate.RateValue}
                        WHERE   CODE = {currencyId} AND DATE = {dateNum}
                    ";

                    var insertQuery = $@"
                        INSERT INTO FCBDY
                        (
                            CODE,
                            DATE,
                            VALUE                            
                        )                        
                        (
                            SELECT 
                            {currencyId},
                            {dateNum},
                            {rate.RateValue}
                        )
                    ";

                    command.CommandText = isExists ? updateQuery : insertQuery;
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

        public int CurrencyCode2CurrencyId(string CurrencyCode)
        {
            switch (CurrencyCode.Trim().ToUpper())
            {
                default:
                case "USD": return 1;
                case "EUR": return 2;
                case "GBP": return 3;
                case "JPY": return 4;
                case "CHF": return 5;
                case "AUD": return 6;
                case "CAD": return 7;
                case "DKK": return 8;
                case "SEK": return 9;
                case "ZAR": return 10;
                case "EGP": return 11;
                case "JOD": return 12;
                case "LBL": return 13;
                case "NOK": return 14;
                case "LBP": return 15;
                case "NZD": return 21;
            }
        }
    }
}
