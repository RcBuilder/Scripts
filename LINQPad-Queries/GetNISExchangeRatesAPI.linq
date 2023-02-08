<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\MonoTouch\v1.0\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>

const string SERVER_URL = "http://www.boi.org.il/currency.xml"; 

async void Main()
{	
    using(var client = new HttpClient()) {    	    
	    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));	    
	    var sJson = await client.GetStringAsync(SERVER_URL);	  
		var rates = JsonConvert.DeserializeObject<ExchangeRatesDTO>(sJson);
		Console.WriteLine(rates);
	}
	
	/*
	using(var client = new System.Net.WebClient()) {    	    
	    client.Headers["Content-Type"] = "application/json";	    
	    var sJson = client.DownloadString(SERVER_URL);	  
		var rates = JsonConvert.DeserializeObject<ExchangeRatesDTO>(sJson);
		Console.WriteLine(rates);
	}
	*/
}

class ExchangeRatesDTO {
	public ExchangeRate[] exchangeRates { get; set; }
}
class ExchangeRate {
	public string key { get; set; }
	public float currentExchangeRate { get; set; }
	public float currentChange { get; set; }	
	public DateTime lastUpdate { get; set; }	
}