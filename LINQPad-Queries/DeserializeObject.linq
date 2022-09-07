<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\MonoTouch\v1.0\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>

void Main()
{
	var sJSON = @"{
	   'destination_addresses' : [ 'עתיר ידע 1 Regues, Kfar Saba, Israel' ],
	   'origin_addresses' : [ 'Atir Yeda St 9, Kefar Sava, Israel' ],
	   'rows' : [
	      {
	         'elements' : [
	            {
	               'distance' : {
	                  'text' : '0.6 km',
	                  'value' : 593
	               },
	               'duration' : {
	                  'text' : '2 mins',
	                  'value' : 92
	               },
	               'status' : 'OK'
	            }
	         ]
	      }
	   ],
	   'status' : 'OK'
	}";
	
	var someModel = JsonConvert.DeserializeObject<GoogleDistanceResult>(sJSON);
	Console.WriteLine(someModel);
}


public class GoogleDistanceResult
{
    public class ResultRow
    {
        [JsonProperty(PropertyName = "elements")]
        public IEnumerable<ResultRowElement> Elements { get; set; }
    }

    public class ResultRowElement
    {
		[JsonProperty(PropertyName = "distance")]
        public ResultRowElementValue Distance { get; set; }
		
		[JsonProperty(PropertyName = "duration")]
		public ResultRowElementValue Duration { get; set; }		
    }
	
	public class ResultRowElementValue
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }

    /*
        "destination_addresses" : [ string ],
        "origin_addresses" : [ string ],
        "rows" : [
            {
                "elements" : [
                {
                    "distance" : {
                        "text" : string,
                        "value" : int
                    },
                    "duration" : {
                        "text" : string,
                        "value" : int
                    },
                    "status" : string
                }
                ]
            }
        ],
        "status" : string
    */

    [JsonProperty(PropertyName = "origin_addresses")]
    public IEnumerable<string> Source { get; set; }

    [JsonProperty(PropertyName = "destination_addresses")]
    public IEnumerable<string> Destination { get; set; }

    [JsonProperty(PropertyName = "rows")]
    public IEnumerable<ResultRow> Rows { get; set; }

    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }        
}