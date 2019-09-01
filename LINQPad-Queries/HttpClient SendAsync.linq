<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Diagnostics</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

static HttpClient client = new HttpClient();
async Task Main()
{
	var request = new HttpRequestMessage() {
	   RequestUri = new Uri("https://example.com/"),
	   Method = HttpMethod.Get
	};
	request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	request.Properties["RequestTimeout"] = TimeSpan.FromSeconds(1);  
	
	using (var resonse = await client.SendAsync(request)){
	        var result = await resonse.Content.ReadAsStringAsync();
			Console.WriteLine("Completed");
	}
}