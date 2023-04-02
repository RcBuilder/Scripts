<Query Kind="Statements">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\MonoTouch\v1.0\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>


var sXML = @"
	<Books>
		<Book>
			<Id>1000</Id>
			<Name>Book-A</Name>
		</Book>
		<Book>
			<Id>1001</Id>
			<Name>Book-B</Name>
		</Book>
		<Book>
			<Id>1002</Id>
			<Name>Book-C</Name>
		</Book>
	</Books>
";

// XML-2-JSON
var docIn = new XmlDocument();
docIn.LoadXml(sXML);
var sJson = JsonConvert.SerializeXmlNode(docIn);
Console.WriteLine(sJson);
/*
	{
	  "Books": {
	    "Book": [
	      {
	        "Id": "1000",
	        "Name": "Book-A"
	      },
	      {
	        "Id": "1001",
	        "Name": "Book-B"
	      },
	      {
	        "Id": "1002",
	        "Name": "Book-C"
	      }
	    ]
	  }
	}
*/

// JSON-2-XML
var docOut = JsonConvert.DeserializeXmlNode(sJson);
Console.WriteLine(docOut);
/*
	<Books>
	   <Book>
	      <Id>1000</Id>
	      <Name>Book-A</Name>
	   </Book>
	   <Book>
	      <Id>1001</Id>
	      <Name>Book-B</Name>
	   </Book>
	   <Book>
	      <Id>1002</Id>
	      <Name>Book-C</Name>
	   </Book>
	</Books>
*/