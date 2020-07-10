<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\MonoTouch\v1.0\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.XML.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.Serialization.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.XmlSerializer.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Xml</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
</Query>

void Main()
{	
	// check whether a number is a POW of 2
	Console.WriteLine(255&8);
	Console.WriteLine((int)0b11111111&8);
	Console.WriteLine(0b11111111&0b00001000); 		
	
	// --
	
	// Deserialize AnonymousType
	var ModelSchema = new { 
		Id = 0,
		Name = "" 
	};
	
	var requestPayload = @"{ 
		'Id': 100,
		'Name': 'John Doe'
	}";
	
	var result = JsonConvert.DeserializeAnonymousType(requestPayload, ModelSchema);
	Console.WriteLine(result.Name);
	
	// --
	
	var personA = new Person{ 
		FullName = new Name { 
			FirstName = "JOHN",
			LastName = "DOE"
		},
		Age = 35,
		City = "Tel-Aviv"
	};
	var personB = new Person{ 
		FullName = new Name { 
			FirstName = "JOHN",
			LastName = "DOE"
		},
		Age = 40,
		City = "Herzliya" 
	};
	
	Console.WriteLine(new PersonComparerLevel1().Equals(personA, personB));
	
	// --

	var jsonTokens = new Dictionary<string, JToken>();
	jsonTokens.Add("a", JToken.FromObject(new { a = 1, b = 2 }));
	jsonTokens.Add("b", 1);
	jsonTokens.Add("c", "some value");
	jsonTokens.Add("d", JToken.FromObject(new SomeClass { Id = 100, Name = "classA" }));
	
	Console.WriteLine(JsonConvert.SerializeObject(jsonTokens));


	// --

	var ser = new XmlSerializer(typeof(SomeClass));
	var sb = new StringBuilder();
	using (var writer = new StringWriter(sb))
	{                
	    ser.Serialize(writer, new SomeClass { 
			Id = 1, 
			Name = "AAA" 
		});
	
	    var strXML = sb.ToString();
	    Console.WriteLine(strXML);
	}
	
	// --
	
	var string2int = Convert.ChangeType("1234", typeof(Int32));
	Console.WriteLine(string2int);
	var int2string = Convert.ChangeType(1234, typeof(String));
	Console.WriteLine(int2string);
	var float2int = Convert.ChangeType(1234.55, typeof(Int32));
	Console.WriteLine(float2int);
	
	// --
	
	var converted = DynamicConvert(typeof(Int32).FullName, "1234");
	Console.WriteLine(converted.GetType());	// System.Int32
	Console.WriteLine(converted);	// 1234
	
	var res = JsonConvert.SerializeObject(new SomeClass2 {
		P1 = "V1",
		P2 = "V2",
		P3 = "V3"
	});
	Console.WriteLine(res);  // {"P1":"V1","CustomName":"V2"}
}

// CLASSES --------------------

public class SomeClass
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
}

public class SomeClass2 {
    [JsonProperty]
    public string P1 { set; get; }

    [JsonProperty(PropertyName = "CustomName")]
    public string P2  { set; get; }    

    [JsonIgnore]
    public string P3  { set; get; }    
}

public class Person
{
    public Name FullName { get; set; }
    public string City { get; set; }
	public Int16 Age { get; set; }
}

public class Name
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public override string ToString()
    {
        return $"{this.FirstName}_{this.LastName}";
    }
}

// COMPARERS ----------------------
public class PersonComparerLevel1 : IEqualityComparer<Person> {
    public bool Equals(Person x, Person y) {
        return (x.FullName.ToString() == y.FullName.ToString());
    }

    public int GetHashCode(Person obj) {
        return obj.FullName.ToString().GetHashCode();
    }
}

// METHODS ----------------------
static object DynamicConvert(string TypeName, object Item) {
      var entityType = Type.GetType(TypeName);
      var entity = Convert.ChangeType(Item, entityType);
	  return entity;       
}