<Query Kind="Statements" />


var abc = new List<(long a, int b, bool c)>{
	(1000, 3, false),
	(1001, 3, true),
	(1002, 2, true),
	(1003, 1, false)
};

Console.WriteLine(abc);

Console.WriteLine("-----------");

var list1 = new List<string> { "A", "B", "C", "D", "E" };
var list2 = new List<string> { "A", "B" };
var list3 = new List<string> { };
List<string> list4 = null;

Console.WriteLine(list1.Take(3));  // 3 items
Console.WriteLine(list2.Take(3));  // 2 items
Console.WriteLine(list3.Take(3));  // 0 items
Console.WriteLine(list4?.Take(3)); // null

Console.WriteLine("-----------");

int ColumnName2Index(string val, int offset = 1){		
	if(val == "") return 0;		
	var a = val[val.Length-1];
	var c = (Convert.ToInt32(a) - 65 + 1) * offset;	
	return ColumnName2Index(val.Substring(0, val.Length - 1), offset*=26) + c;	
}

Console.WriteLine($"Name {"A"} Is {ColumnName2Index("A")}");
Console.WriteLine($"Name {"C"} Is {ColumnName2Index("C")}");
Console.WriteLine($"Name {"Z"} Is {ColumnName2Index("Z")}");
Console.WriteLine($"Name {"AA"} Is {ColumnName2Index("AA")}");
Console.WriteLine($"Name {"AB"} Is {ColumnName2Index("AB")}");
Console.WriteLine($"Name {"AL"} Is {ColumnName2Index("AL")}");
Console.WriteLine($"Name {"BA"} Is {ColumnName2Index("BA")}");
Console.WriteLine($"Index {"EX"} Is {ColumnName2Index("EX")}");
Console.WriteLine($"Index {"ZY"} Is {ColumnName2Index("ZY")}");
Console.WriteLine($"Index {"ZZ"} Is {ColumnName2Index("ZZ")}");

Console.WriteLine("-----------");

string ColumnIndex2Name(int val){	
	if(val <= 0) return "";	
	var c = ((val - 1) % 26);
	return ColumnIndex2Name((val - c) / 26) + Convert.ToChar(65 + c).ToString();	
}

Console.WriteLine($"Index {1} Is {ColumnIndex2Name(1)}");
Console.WriteLine($"Index {3} Is {ColumnIndex2Name(3)}");
Console.WriteLine($"Index {25} Is {ColumnIndex2Name(25)}");
Console.WriteLine($"Index {26} Is {ColumnIndex2Name(26)}");
Console.WriteLine($"Index {27} Is {ColumnIndex2Name(27)}");
Console.WriteLine($"Index {38} Is {ColumnIndex2Name(38)}");
Console.WriteLine($"Index {53} Is {ColumnIndex2Name(53)}");
Console.WriteLine($"Index {154} Is {ColumnIndex2Name(154)}");
Console.WriteLine($"Index {701} Is {ColumnIndex2Name(701)}");
Console.WriteLine($"Index {702} Is {ColumnIndex2Name(702)}");