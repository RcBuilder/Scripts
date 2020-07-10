<Query Kind="Statements">
  <Namespace>System.Data.Odbc</Namespace>
</Query>

/*
var connetionString = "Driver={Pervasive ODBC Client Interface};ServerName=CREATIVE-TEST;dbq=NONAME1r2517db;";
var query = "select top 10 * from AccTrs";
*/

/**/
var connetionString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=D:\\testDB.mdb;";
var query = "select top 10 * from Campaign_Table";


/*
var connetionString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=D:\\";
var query = "select top 10 * from tbl1.csv";
*/

/*
var connetionString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=D:\\";
var query = "select top 10 * from tbl1.txt";
*/

try
{   
	using(var connection = new OdbcConnection(connetionString)){		
		connection.Open();		
    	Console.WriteLine("Connection Open!");	    	
		
		var command = new OdbcCommand(query);
		command.Connection = connection;
		using(var reader = command.ExecuteReader()){		
			while (reader.Read()){
	            Console.WriteLine(reader[1]);
	        }
		}
	}    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}