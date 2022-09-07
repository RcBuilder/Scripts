<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Reference Assemblies\Microsoft\Framework\MonoTouch\v1.0\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.XML.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.Serialization.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.XmlSerializer.dll</Reference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Xml</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
</Query>

public class SomeModelWithDictionary{
	public int id { get; set; }
	public IEnumerable<Dictionary<string, string>> attributes { get; set; }
}

public class BaseClass {        
    public string P1 { get; set; }
    public int P2 { get; set; }
    public bool P3 { get; set; }    
}

public class DerivedClass : BaseClass {        
    public string P4 { get; set; }
    public float P5 { get; set; } 
	
	public static DerivedClass Clone(BaseClass Source){
        var serialized = JsonConvert.SerializeObject(Source);
        var clone = JsonConvert.DeserializeObject<DerivedClass>(serialized);
        return clone;
    }
}

public enum eCurrency { ILS = 1, USD = 2, EUR = 3 }
public class Currency1{
	public eCurrency Value { get; set; } = eCurrency.USD;	
}
public class Currency2{
	[JsonConverter(typeof(StringEnumConverter))]
	public eCurrency Value { get; set; } = eCurrency.USD;
}

[Flags]
/*
enum eLetter : int { 
	A = 1, 
	B = 2, 
	C = 4, 
	D = 8 
}
*/
enum eLetter : int { 
	A = 1 << 0, 
	B = 1 << 1, 
	C = 1 << 2, 
	D = 1 << 3 
}

void Main()
{	
	// Load Derived Class From Base Class Using Clone
	var bc = new BaseClass{
		P1 = "AAA",
		P2 = 300,
		P3 = true
	};
	var dc = DerivedClass.Clone(bc);
	dc.P4 = "BBB";
	dc.P5 = 12.90F;
	Console.WriteLine(dc);
	
	// --
	
	var c1 = new Currency1();
	var c2 = new Currency2();
	Console.WriteLine(JsonConvert.SerializeObject(c1));  // {"Value":2}
	Console.WriteLine(JsonConvert.SerializeObject(c2));  // {"Value":"USD"}
	
	// --

	// Bitwise 
	var letters = (eLetter.B | eLetter.D);
	Console.WriteLine(letters.ToString());  // B, D
	Console.WriteLine((int)letters); // 10
	
	var hasA = (letters & eLetter.A) == eLetter.A;
	Console.WriteLine(hasA); // false
	
	var hasB = (letters & eLetter.B) == eLetter.B;
	Console.WriteLine(hasB); // true
	
	var hasD = (letters & eLetter.D) == eLetter.D;
	Console.WriteLine(hasD); // true
	
	var isD = (letters | eLetter.D) == eLetter.D;
	Console.WriteLine(isD); // false

	var hasNoA = !hasA;
	Console.WriteLine(hasNoA); // true

	// --

	// Serialization
	var someModelJson = @"{
	  'id': 77137,	  
	  'attributes': [
	    {
	      'sku': 'aaa',
		  'price': 38.99,
	    }
	  ]
	}";

	var someModel = JsonConvert.DeserializeObject<SomeModelWithDictionary>(someModelJson);
	Console.WriteLine(someModel.attributes);  // {"sku":"aaa","price":"38.99"}
	
	
	someModel = new SomeModelWithDictionary{
		id = 100,
		attributes = new List<Dictionary<string, string>> {
            new Dictionary<string, string> {
                ["sku"] = "bbb",
                ["price"] = "89.19"
            }
        }
	};
	someModelJson = JsonConvert.SerializeObject(someModel);
	Console.WriteLine(someModelJson);  // {"id":100,"attributes":[{"sku":"bbb","price":"89.19"}]}

	return;


	// check whether a number is a POW of 2
	Console.WriteLine(255&8);
	Console.WriteLine((int)0b11111111&8);
	Console.WriteLine(0b11111111&0b00001000); 		
	
	// --
	
	var lst = new List<IProduct>{
		new ProductA(),
		new ProductA(),
		new ProductB(),
		new ProductC()		
	};
	
	var a = lst.OfType<ProductA>();
	var b = lst.OfType<ProductB>();
	var c = lst.OfType<ProductC>();
	var d = lst.OfType<ProductD>();
	
	Console.WriteLine($"found {a.Count()} items of type ProductA");
	Console.WriteLine($"found {b.Count()} items of type ProductB");
	Console.WriteLine($"found {c.Count()} items of type ProductC");
	Console.WriteLine($"found {d.Count()} items of type ProductD");
	
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
	
	var CartItemSchema = new { 		
		Name = "",
		Price = 0.0F,
		Units = 0
	};
	
	var CartSchema = new { 
		CartId = 0,
		Items = new[]{ CartItemSchema }		
	};
	
	var cartPayload = @"{ 
		'CartId': 100,
		'Items': [
			{
				'Name': 'Item-A',
				'Price': 35.9,
				'Units': 3
			},
			{
				'Name': 'Item-B',
				'Price': 210,
				'Units': 1
			},
			{
				'Name': 'Item-C',
				'Price': 83,
				'Units': 12
			},
		]
	}";
	
	var cart = JsonConvert.DeserializeAnonymousType(cartPayload, CartSchema);
	Console.WriteLine(cart.CartId);
	foreach(var item in cart.Items)
		Console.WriteLine($"{item.Name} -> Units: {item.Units}, Price:{item.Price}");
	
	return;
	
	// --
	
	using (var client = new WebClient()){    
		client.DownloadString("http://rcb.co.il");
	}
	
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

interface IProduct{}
class ProductA : IProduct{}
class ProductB : IProduct{}
class ProductC : IProduct{}
class ProductD : IProduct{}

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