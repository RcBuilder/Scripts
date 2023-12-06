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
using static Helpers.PervasiveDBHelper;

namespace TaxesILGateway
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

    public class PervasiveDAL : PervasiveBaseDAL {
        public PervasiveDAL(string ConnetionString) : base(ConnetionString) { }
        
        public bool SaveTaxesResultId(string TaxesResultId, int MismNo, char MismCode)
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
                    
                    var existsQuery = $@"
                        SELECT 1 FROM  MismHak                        
                        WHERE   MismNo = {MismNo} AND MismCode = '{MismCode}'
                    ";
                    command.CommandText = existsQuery;
                    var isExists = Convert.ToInt32(command.ExecuteScalar()) == 1;


                    var updateQuery = $@"
                        UPDATE  MismHak
                        SET     HAK = '{TaxesResultId}'
                        WHERE   MismNo = {MismNo} AND MismCode = '{MismCode}'
                    ";

                    var insertQuery = $@"
                        INSERT INTO MismHak
                        (
                            HAK,
                            MismNo,
                            MismCode                            
                        )                        
                        (
                            SELECT 
                            '{TaxesResultId}',
                            {MismNo},
                            '{MismCode}'
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
       
        public char InvoiceType2MismCode(int InvoiceType) {
            switch (InvoiceType) {
                case 310: return 'h';
                case 320: return 'M';
                case 330: return 'Z';                            
                default: return 'H';
            }
        }
    }
}
