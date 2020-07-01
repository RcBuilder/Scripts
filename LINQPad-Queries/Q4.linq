<Query Kind="Statements" />

string ConvertToBase62(int value){	
	return ConvertToBaseX(value, 62);
}

string ConvertToHEX(int value){	
	return ConvertToBaseX(value, 16);
}

string ConvertToBIN(int value){	
	return ConvertToBaseX(value, 2);
}

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
	    mod = (int)(value % baseNum); //should be safe
	    current = baseDigits.Substring(mod, 1);
		result = current + result;
	    value = value / baseNum;	
		Console.WriteLine($"mod: {mod} ({value} % {baseNum}), current: {current} (array[{mod}]), value: {value}");
	}

	return result;
}

Console.WriteLine($"result: {ConvertToBaseX(10, 2)}");  // 1010
Console.WriteLine($"result: {ConvertToBaseX(10, 8)}");	// 12
Console.WriteLine($"result: {ConvertToBaseX(10, 10)}");  // 10
Console.WriteLine($"result: {ConvertToBaseX(10, 16)}");  // A

Console.WriteLine($"result: {ConvertToBaseX(140, 12)}");  // B8
Console.WriteLine($"result: {ConvertToBaseX(140, 8)}");  // 214

Console.WriteLine($"result: {ConvertToBase62(140)}");  // 2G
Console.WriteLine($"result: {ConvertToBase62(94255)}");	 // OWF

Console.WriteLine($"result: {ConvertToHEX(140)}");  // 8C
Console.WriteLine($"result: {ConvertToBIN(250)}");  // 11111010


