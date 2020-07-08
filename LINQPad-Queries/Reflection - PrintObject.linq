<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Collections.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Linq.dll</Reference>
</Query>

void Main()
{	
	var objToPrint = new ObjToPrint_L0();
	objToPrint.Lvl1.Lvl2.ObjToPrint_L0 = objToPrint;  //circular reference 
	
    PrintProperties(objToPrint, new HashSet<object>());
}

class ObjToPrint_L0 {
    public string P100 => "V100";
    public int P200 => 200;
	public DateTime DT => DateTime.Now;
    public ObjToPrint_L1 Lvl1 { set; get; } = new ObjToPrint_L1();   
}

class ObjToPrint_L1 {
    public int Id1 { set; get; } = 10;
    public ObjToPrint_L2 Lvl2 { set; get; } = new ObjToPrint_L2();
}

class ObjToPrint_L2 {
    public int Id2 { set; get; } = 20;
	public IEnumerable<string> Items { set; get; } = new List<string>{
		"ABC", "BCD", "CDE"
	};
	public ObjToPrint_L0 ObjToPrint_L0 { set; get; } = null;
}

void PrintProperties<T>(T obj, HashSet<object> hashSet, int level = 0, int speceQuantity = 10) {	
	if(hashSet.Contains(obj)) return;
	hashSet.Add(obj);
	
	var type = obj.GetType();
    var space = new String(' ', level * speceQuantity);
    Console.WriteLine($"\n{space}Object of Class \"{type.Name}\"");
    Console.WriteLine($"{space}{new String('-', type.Name.Length + 22)}\n");

    var props = type.GetProperties();
    foreach (var p in props)
    {
        var innerSpace = new String(' ', space.Length + speceQuantity);
        var isReference = !(p.PropertyType.IsValueType || p.PropertyType == typeof(string));
			
		// var isList = p.PropertyType.IsAssignableFrom(typeof(IEnumerable<>));		
        var isList = p.PropertyType.GetInterfaces().FirstOrDefault(infs => infs.Name == "IEnumerable") != null && !(p.PropertyType == typeof(string));                	
		
        Console.WriteLine($"{innerSpace} {p.Name} = \"{(isReference ? "" : p.GetValue(obj))}\"");

		if(isList)
			Console.WriteLine("LIST");
        else if (isReference)
            PrintProperties(p.GetValue(obj, null), hashSet, level + 1);
    }
}