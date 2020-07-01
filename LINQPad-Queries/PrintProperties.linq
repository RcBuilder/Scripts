<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.Parallel.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Thread.dll</Reference>
  <Namespace>System.Threading</Namespace>
</Query>

void Main()
{
	PrintProperties(typeof(ObjToPrint_L0));
}

class ObjToPrint_L0 {
    public string P100 => "V100";
    public int P200 => 200;
    public ObjToPrint_L1 Lvl1 { set; get; }
    public DateTime DT => DateTime.Now;
}

class ObjToPrint_L1 {
    public int Id1 { set; get; }
    public ObjToPrint_L2 Lvl1 { set; get; }
}

class ObjToPrint_L2 {
    public int Id2 { set; get; }
}

void PrintProperties(Type obj, int level = 0) {
    var props = obj.GetProperties();
    foreach (var p in props)
    {
        Console.WriteLine($"{new String('-', level)} {p.Name}");
        if (!(p.PropertyType.IsValueType || p.PropertyType == typeof(string)))
            PrintProperties(p.PropertyType, level + 1);
    }
}