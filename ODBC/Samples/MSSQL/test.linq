<Query Kind="Statements">
  <Namespace>System.Data.Odbc</Namespace>
</Query>


var connetionString = "Driver={ODBC Driver 13 for SQL Server};server=RCBUILDER-PC\\RCBUILDERSQL2012;database=MACrawler;trusted_connection=Yes;";
var query = "select top 10 * from Sources";

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