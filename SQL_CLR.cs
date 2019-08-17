using System;
using System.Collections;

using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data;

public class STORED_PROCEDURES
{    
    [SqlProcedure]
    public static void Hello() {
        SqlContext.Pipe.Send("Hello From SQL CLR!"); // print
    }
   
    [SqlProcedure]
    public static void MyNameIs(SqlString name) {
        SqlContext.Pipe.Send(string.Format("Name is {0}", name));  // print + param
    }
    
    [SqlProcedure]
    public static void GetRandomId(out SqlInt32 randomId) {
        randomId = new Random().Next(1, 100);  // out param
    }

    // execute sql command
    [SqlProcedure]
    public static void RAISERROR(){
        try // MUST USE TRY CATCH CLAUSE IN ORDER TO CATCH THE ERROR IN THE CLIENT LAYER!
        {
            string Error = "Error Message From SQL CLR";
            SqlContext.Pipe.ExecuteAndSend(new SqlCommand(string.Format("RAISERROR ( '{0}', 11, 1)", Error)));
        }
        catch { }
    }

    // select 
    [SqlProcedure]
    public static void SelectUsers() {
        var metadata = new SqlMetaData[2];
        metadata[0] = new SqlMetaData("Id", SqlDbType.NVarChar, 4);
        metadata[1] = new SqlMetaData("Name", SqlDbType.NVarChar, 50);

        var record = new SqlDataRecord(metadata);
        SqlContext.Pipe.SendResultsStart(record);

        record.SetString(0, "1"); // column Id
        record.SetString(1, "Roby"); // column Name
        SqlContext.Pipe.SendResultsRow(record); // add row

        record.SetString(0, "2");
        record.SetString(1, "Ron");
        SqlContext.Pipe.SendResultsRow(record);

        record.SetString(0, "3");
        record.SetString(1, "Avi");
        SqlContext.Pipe.SendResultsRow(record);

        record.SetString(0, "4");
        record.SetString(1, "Evy");
        SqlContext.Pipe.SendResultsRow(record);

        record.SetString(0, "5");
        record.SetString(1, "Gilad");
        SqlContext.Pipe.SendResultsRow(record);

        record.SetString(0, "6");
        record.SetString(1, "Sharon");
        SqlContext.Pipe.SendResultsRow(record);

        record.SetString(0, "7");
        record.SetString(1, "Ronen");
        SqlContext.Pipe.SendResultsRow(record);

        SqlContext.Pipe.SendResultsEnd();
    }
}


/* 
     -------- FUNCTIONS ---------------

     TableDefinition: 
     ----------------
     the TableDefinition property indicates the return table structure 
 
     The return values MUST Implement the IENUMERABLE interface 
     (List,Array,ArrayList ...)
     
     The FillRowMethodName property indicates which method will bind the IEnumerable returned collection.
     each item from the collection executes this method - ITERATOR(predicate).
     the out parameter is the value returns to the SQL.
  
     Scalar:
     -------
     returns an sql type
*/
public class FUNCTIONS {

    // scalar value FUNCTION - return int
    [SqlFunction]
    public static SqlInt32 NumericName(SqlString Name) {
        int num = 0;
        foreach (char c in Name.ToString().ToCharArray())
            num += c;
        return new SqlInt32(num);
    }

    // table value FUNCTION - single column - char
    [SqlFunction(TableDefinition = "CHR nchar", FillRowMethodName = "FillCharRow")]
    public static IEnumerable ToCharsArray(SqlString sValue) {
        return new ArrayList(sValue.ToString().ToCharArray());
    }

    public static void FillCharRow(object row, out char col1) {
        col1 = (char)row;
    }

    // table value FUNCTION - single column - nvarchar
    [SqlFunction(TableDefinition = "STR nvarchar(10)", FillRowMethodName = "FillStringRow")]
    public static IEnumerable SplitSTR(SqlString sValue, char delimiter) {
        return sValue.ToString().Split(delimiter);
    }

    public static void FillStringRow(object row, out string col1) {
        col1 = (string)row;
    }

    // table value FUNCTION - single column - int
    [SqlFunction(TableDefinition = "INTGR int", FillRowMethodName = "FillIntRow")]
    public static IEnumerable SplitINT(SqlString sValue, char delimiter) {
        return sValue.ToString().Split(delimiter);
    }

    public static void FillIntRow(object row, out int col1) {
        col1 = Convert.ToInt32(row);
    }

    // table value FUNCTION - multi columns - int,nvarchar
    [SqlFunction(TableDefinition = "Id int,Name nvarchar(50)", FillRowMethodName = "FillMultiFieldsRow")]
    public static IEnumerable GetMembersStartsWith(SqlString sValue) {
        var users = new Tuple<int, string>[] {
           Tuple.Create(1,"Roby"),
           Tuple.Create(2,"Ron"),
           Tuple.Create(3,"Avi"),
           Tuple.Create(4,"Evy"),
           Tuple.Create(5,"Gilad"),
           Tuple.Create(6,"Sharon"),
           Tuple.Create(7,"Ronen"),
           Tuple.Create(8,"Shirly"),
           Tuple.Create(9,"Oren"),
           Tuple.Create(10,"Ohad"),
           Tuple.Create(11,"Shai")
        };

        var values = new ArrayList();
        foreach (var user in users)
            if (user.Item2.StartsWith(sValue.ToString()))
                values.Add(user);
        return values;
    }

    public static void FillMultiFieldsRow(object row, out int col1, out string col2) {
        col1 = ((Tuple<int, string>)row).Item1;
        col2 = ((Tuple<int, string>)row).Item2;
    }
}

/* 
     -------- AGGREGATES ---------------
        
     required methods: 
     1. Init
     2. Accumulate
     3. Merge
     4. Terminate

     note:
     do not use AGGREGATE method within another class - use namespace instead! 
*/
namespace AGGREGATES {
    // aggregation function to count names more then 10 characters length
    [Serializable]
    [SqlUserDefinedAggregate(Format.Native, Name = "CountLongNames")]
    public struct CountLongNames {
        private static readonly int maxLength = 10;
        private SqlInt32 res; // shared all rows 

        public void Init() {
            res = 0;
        }

        public void Accumulate(SqlString value) {
            if (value.ToString().Length > maxLength)
                this.res += 1;
        }

        public void Merge(CountLongNames other) {
            this.res += other.res;
        }

        public SqlInt32 Terminate() {
            return this.res;
        }
    }
}