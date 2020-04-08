<Query Kind="Statements">
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SDKs\Azure\.NET SDK\v2.9\bin\plugins\Diagnostics\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Collections.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Linq.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

var list = new List<string>{
	"032be263-cb41-48e3-8199-9cd3d381ee6a",
	"04d2f4f8-e1f2-4032-9bd0-6ef66a8cac78",
	"0d28cc02-28e9-4d5f-ae7e-a370eba991f2",
	"1015e1f3-d41d-45bf-879b-0f8e61b12dff"
};

foreach (var item in list){
	using (var client = new WebClient()){  
	  	client.Headers.Add("Content-Type","application/json");
	  	client.Headers.Add("Authorization", "Basic xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

	  	var sResponse = client.UploadString("http://domain.com/get_account", $@"{{
			""account_id"": ""{item}""
		}}");
		
		dynamic response = JsonConvert.DeserializeObject(sResponse);		
		if(response == null) {
			Console.WriteLine($"No Account: {sResponse}");
			continue;
		}
	  	Console.WriteLine(response._id);
	}
}