<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.NetworkInformation.dll</Reference>
  <Namespace>System.Net</Namespace>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

Console.WriteLine(14.ToString("x")); 		// E
Console.WriteLine(14.ToString("x2")); 		// 0E
Console.WriteLine(14.ToString("x4")); 		// 000E
Console.WriteLine(Convert.ToString(14, 2)); // 1110
Console.WriteLine(Convert.ToString(14, 16));// E
Console.WriteLine(Convert.ToString(14, 8)); // 16
Console.WriteLine(Convert.ToString(14, 10));// 14
Console.WriteLine(Convert.ToString(16, 8));// 20

string ConvertToBaseX(int value, int baseToUse){
	if(baseToUse < 2) return null; // min base 2

	var charactersSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
	var baseDigits = new string(charactersSet.Take(baseToUse).ToArray());
	var baseNum = baseDigits.Length;
	Console.WriteLine($"base {baseNum} ({baseDigits})");
	
	var result = value == 0 ? "0" : "";
	var mod = 0;
	var current = "";
	
	while (value != 0)
	{
	    mod = (int)(value % baseNum); 
	    current = baseDigits.Substring(mod, 1);
	    result = current + result;
	    value = value / baseNum;	
	    Console.WriteLine($"mod: {mod} ({value} % {baseNum}), current: {current} (array[{mod}]), value: {value}");
	}

	return result;
}
Console.WriteLine($"result: {ConvertToBaseX(14, 13)}");  // A