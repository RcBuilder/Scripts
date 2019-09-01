<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.WebRequest.dll</Reference>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Diagnostics</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var numOfLoops = 5000;
	
	///test1(numOfLoops);	
	test2(numOfLoops);	
}

void test1(int numOfLoops){
	var sw = Stopwatch.StartNew();
	Parallel.For(0, numOfLoops, _ => { 
		MakeRequest(); 
		var unique = Guid.NewGuid().ToString();
    	//Console.WriteLine(unique);
	});	
	sw.Stop();
	Console.WriteLine("test1 > {0} sec", sw.Elapsed.TotalSeconds);
}

static HttpClient client = new HttpClient();
void test2(int numOfLoops){	
	var sw = Stopwatch.StartNew();	
	Parallel.For(0, numOfLoops, _ => { 		
		MakeRequest(client); 
		var unique = Guid.NewGuid().ToString();
    	//Console.WriteLine(unique);
	});	
	sw.Stop();
	Console.WriteLine("test2 > {0} sec", sw.Elapsed.TotalSeconds);
}

async void MakeRequest(){	
	using (var client = new HttpClient())
    using (var resonse = await client.GetAsync("https://example.com/"))
        await resonse.Content.ReadAsStringAsync();				
}

async void MakeRequest(HttpClient client){	
    using (var resonse = await client.GetAsync("https://example.com/"))
       await resonse.Content.ReadAsStringAsync();
}

