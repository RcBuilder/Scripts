<Query Kind="Statements" />

using (var client = new System.Net.WebClient()){
  var response = client.UploadData("https://openbook.co.il/Cart/ZCreditIPNHandler", new byte[] { });
  Console.WriteLine(System.Text.Encoding.UTF8.GetString(response));
}