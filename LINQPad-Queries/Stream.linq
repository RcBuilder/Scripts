<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Sockets.dll</Reference>
  <Namespace>System.Net.Sockets</Namespace>
</Query>

var str = "Some Content";
var ms = new MemoryStream();

ms.Position = 0;
var writer = new StreamWriter(ms);
writer.WriteLine(str);
writer.Flush();

ms.Position = 0;
var reader = new StreamReader(ms);
Console.WriteLine(reader.ReadLine());	

writer.Dispose();
reader.Dispose();
ms.Dispose();