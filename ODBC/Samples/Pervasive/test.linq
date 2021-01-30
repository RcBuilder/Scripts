<Query Kind="Statements">
  <Namespace>System.Data.Odbc</Namespace>
</Query>

var connetionString = "Driver={Pervasive ODBC Client Interface};ServerName=RcBuilder-PC;dbq=MANAGER1r7627db;Client_CSet=UTF-8;Server_CSet=CP850;";
var query = "select top 10 * from Accounts";

try
{   
	using(var connection = new OdbcConnection(connetionString)){		
		connection.Open();		
    	Console.WriteLine("Connection Open!");	    	
		
		var command = new OdbcCommand(query);
		command.Connection = connection;
		using(var reader = command.ExecuteReader()){		
			while (reader.Read()){
				var id = Convert.ToInt32(reader["AccNo"]);
				var name = reader["Name"].ToString().Trim();
				
				// fix encoding issue 
				byte[] nameBytes = Encoding.GetEncoding("windows-1255").GetBytes(name);
				name = Encoding.GetEncoding(862).GetString(nameBytes);
								
	            Console.WriteLine($"#{id} {name}");
	        }
		}
	}    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}