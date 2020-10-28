<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Namespace>System.Net</Namespace>
</Query>

// UPLOAD 
using (var client = new WebClient())
{
    client.Credentials = new NetworkCredential("morningloveUser", "xxxxxxxxxx");
    client.UploadFile("ftp://ftp-eu.site4now.net/test.txt", "D:\\test1.txt");
}

// DOWNLOAD
using (var client = new WebClient())
{
    client.Credentials = new NetworkCredential("morningloveUser", "xxxxxxxxxx");
    client.DownloadFile("ftp://ftp-eu.site4now.net/test.txt", "D:\\test1_copy.txt");
}